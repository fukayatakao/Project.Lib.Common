using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Project.Lib {
    /// <summary>
    /// 送るメッセージデータ
    /// </summary>
    public struct MessageObject{
		public MessageObject(int type, object data) {
			TypeId = type;
			Data = data;
		}
		public int TypeId;				//メッセージのタイプ
		public object Data;				//データの中身
	}
    /// <summary>
    /// MessageSystemで使用する設定
    /// </summary>
    public interface IMessageSetting {
        //メッセージの受け取り関数名はこの名前でないと受け取れない
        string RecvFuncName { get; }
        //メッセージクラスの中にこの文字列のメンバ変数が存在しないといけない
        string IdFieldName { get; }
        //レイヤーの最大数
        int GroupMax { get; }
        //メッセージクラス
        List<System.Type> MessageType { get; }
    }

    /// <summary>
    /// オブジェクト間処理の仲介
    /// </summary>
    public class MessageSystem {
		//隠れシングルトン
		private static MessageSystem instance_;
		private CoroutineTaskParallel taskParallel_ = new CoroutineTaskParallel();
		/// <summary>
		/// インスタンス作成
		/// </summary>
		public static void Create(IMessageSetting setting) {
			Debug.Assert(instance_ == null, "already create instance");
			instance_ = new MessageSystem(setting);
		}

		/// <summary>
		/// インスタンス破棄
		/// </summary>
		public static void Destroy() {
			instance_ = null;
		}
		/// <summary>
		/// 実行処理
		/// </summary>
		public static void Execute() {
			instance_.taskParallel_.Execute();
		}
		/// <summary>
		/// 積まれているタスクを消化する
		/// </summary>
		public static void Dispatch() {
			while (!instance_.taskParallel_.IsEnd()) {
				instance_.taskParallel_.Execute();
			}
		}

		/// <summary>
		/// Receptor生成
		/// </summary>
		public static Receptor CreateReceptor<T, U>(T owner, params U[] list) where U : Enum {
			return CreateReceptorImpl<T, U>(owner, list);
		}
		/// <summary>
		/// Receptor生成(実体)
		/// </summary>
		private static Receptor CreateReceptorImpl<T, U>(T owner, params U[] list) {
			int group = 0;

			if(list.Length == 0) {
				group = ~0;
			}
			for (int i = 0, max = list.Length; i < max; i++) {
				group |= 1 << (int)(object)list[i];
			}
			
			var receptor = new Receptor<T>(owner, group);

			Register<T>(receptor);

			return receptor;
		}



		/// <summary>
		/// Receptor生成
		/// </summary>
		public static void DestroyReceptor(Receptor receptor) {
			//シーン遷移時などMessageSystemのインスタンス破棄後にデストラクタから呼ばれる場合があるので対策
			if (instance_ == null)
				return;
			instance_.taskParallel_.Add(UnRegister(receptor));
		}

		/// <summary>
		/// Receptor生成
		/// </summary>
		public static Receptor CreateReceptor<T>(T owner, params int[] list) {
			var receptor = new Receptor<T>(owner, 0);
			MessageSystem ms = MessageSystem.instance_;
			ms.handler_[receptor.Address] = receptor.Handling;
			return receptor;
		}



		/// <summary>
		/// ハンドリング対象を追加
		/// </summary>
		private static void Register<T>(Receptor<T> receptor) {
			MessageSystem ms = MessageSystem.instance_;

			int address = receptor.Address;
			int group = receptor.Group;

			ms.handler_[address] = receptor.Handling;
			for (int i = 0; i < ms.groupMax_; i++) {
				if ((group >> i & 1) == 1) {
					(ms.groupHandler_[i])[address] = receptor.Handling;
				}
			}
		}
		/// <summary>
		/// ハンドリング対象を削除
		/// </summary>
		private static IEnumerator UnRegister(Receptor receptor) {
			//1フレーム待ってから遅延で削除
			yield return null;
			MessageSystem ms = MessageSystem.instance_;

			int address = receptor.Address;
			int group = receptor.Group;

			ms.handler_.Remove(address);
			for (int i = 0; i < ms.groupMax_; i++) {
				if ((group >> i & 1) == 1) {
					ms.groupHandler_[i].Remove(address);
				}
			}
		}


		/// <summary>
		/// インスタンス管理を外部で行わせるためのクラス
		/// </summary>
		public class Receptor {
			public virtual int Address { get; }
			public virtual int Group { get; }
		}

		/// <summary>
		/// ハンドラの扱う型ごとに派生したクラス
		/// </summary>
		private class Receptor<T> : Receptor {
			T owner_;
			//ユニークなアドレス
			private int address_;
			public override int Address { get { return address_; } }
			//ハンドラーグループ
			private int group_;
			public override int Group { get { return group_; } }
			//受信のdelegate関数を配列化
			private System.Action<T, MessageObject>[] recieveFunc_;

			/// <summary>
			/// コンストラクタ
			/// </summary>
			public Receptor(T owner, int group = ~0) {
				//すでにキャッシュされたものがある場合はReflectionで拾ってこないでそちらを使う
				object[] functions;

                MessageSystem ms = MessageSystem.instance_;

                ms.recieverCache_.TryGetValue(typeof(T), out functions);
				if(functions == null){
					recieveFunc_ = new Action<T, MessageObject>[ms.message_.Length];

					for (int i = 0, max = ms.message_.Length; i < max; i++) {
						Debug.Assert(ms.message_ [i] != null, "message class error");
						//関数名が"Recv"で引数が(T, MessageObject)になっている関数を探す。
						System.Reflection.MethodInfo info = ms.message_ [i].GetMethod (ms.recvFuncName_, System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic, null, new System.Type[] {typeof(T), typeof(MessageObject)}, null);
						//見つかったら受信可能なメッセージタイプなので受信関数をdelegateでキャッシュする。
						if (info != null) {
							recieveFunc_ [i] = (System.Action<T, MessageObject>)Delegate.CreateDelegate (typeof(System.Action<T, MessageObject>), info);
						}
					}
                    ms.recieverCache_[typeof(T)] = recieveFunc_;
				}else{
					recieveFunc_ = functions as Action<T, MessageObject>[];
				}
				owner_ = owner;
				group_ = group;
				address_ = ms.uniqueCounter_.GetUniqueId();
            }

			/// <summary>
			/// ハンドリング
			/// </summary>
			public void Handling (MessageObject msg){
				//MessageSettingでメッセージクラスとして登録されてないクラスのメッセージを飛ばすとエラーになるのでチェック
				//※BattleMessageクラスしか登録してないのにTownMessage飛ばしても登録外なので処理できない
				Debug.Assert(msg.TypeId >= 0 && msg.TypeId < recieveFunc_.Length, "id range error:" + msg.TypeId);
				if (recieveFunc_ [msg.TypeId] != null) {
					recieveFunc_ [msg.TypeId] (owner_, msg);
				}
			}
		}
		private UniqueCounter uniqueCounter_ = new UniqueCounter();
		//受信関数をまとめたクラスのインスタンスをキャッシュするディクショナリ。このクラスのインスタンスが破棄されたら消えるようにstaticはつけない。
		Dictionary<System.Type, object[]> recieverCache_ = new Dictionary<Type, object[]>();

        //メッセージの受信を受け付けるクラス。メッセージの種類が増えるごとに追加する。
        private System.Type[] message_;
        //メッセージの受け取り関数名はこの名前でないと受け取れない
        string recvFuncName_;

		private int groupMax_;
		//メッセージを受け取るハンドラ
		private Dictionary<int, System.Action<MessageObject>> handler_;
		private Dictionary<int, System.Action<MessageObject>>[] groupHandler_;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		private MessageSystem(IMessageSetting setting) {
			var list = new List<Type>(typeof(SystemMessage).GetNestedTypes());
			setting.MessageType.AddRange(list);
			message_ = setting.MessageType.ToArray();

			recvFuncName_ = setting.RecvFuncName;
            handler_ = new Dictionary<int, System.Action<MessageObject>>();
			groupMax_ = setting.GroupMax;
			groupHandler_ = new Dictionary<int, System.Action<MessageObject>>[setting.GroupMax];
			for (int i = 0; i < setting.GroupMax; i++) {
				groupHandler_[i] = new Dictionary<int, Action<MessageObject>>();
			}

			for (int i = 0, max = message_.Length; i < max; i++) {
				System.Reflection.FieldInfo info = message_[i].GetField(setting.IdFieldName, System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
				Debug.Assert(info != null, "not found message id field:" + message_[i].ToString());
				info.SetValue(null, i);
			}
		}
		/// <summary>
		/// メッセージ一斉送信処理
		/// </summary>
		public static void Broadcast(MessageObject msg){
			//MessageSystemのインスタンスが存在しない場合、メッセージの送信は無視される。
			if (instance_ == null) {
				Debug.Log("message system is not available");
				return;
			}
			foreach (var handler in instance_.handler_.Values) {
				handler(msg);
			}
		}
		/// <summary>
		/// メッセージ一斉送信処理
		/// </summary>
		/// <remarks>
		/// 特定のレイヤーでフィルタリングするバージョン
		/// </remarks>
		public static void Broadcast(MessageObject msg, int group){
			//MessageSystemのインスタンスが存在しない場合、メッセージの送信は無視される。
			if (instance_ == null){
				Debug.Log("message system is not available");
				return;
			}
			//@note groupHandler_をforeachで回すとメッセージ処理中でReceptorが追加されたときにエラーになってしまう。
			//      ループ中に要素が変動してエラーになるのを避けるためにToListを使って別コレクションを作って処理
			List<System.Action<MessageObject>> handler = instance_.groupHandler_[group].Values.ToList();
			foreach (var handle in handler) {
				handle (msg);
			}
		}
	}


}
