using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace Project.Lib {
	/// <summary>
	/// ユーザー制御処理のインタフェース
	/// </summary>
	public interface IHaveControl {
		// 割り込みチェック
		bool Interrupt();
		// 制御開始
		void Begin();
		// 制御終了
		bool IsEnd();
		// 制御リクエストを却下された
		void Reject();
		// ポーリングで処理する優先順位
		int Priority { get; }
	}
	/// <summary>
	/// ユーザー操作の制御を振り分け
	/// </summary>
	public class UserOperation {
        List<IHaveControl> polingList_ = new List<IHaveControl>();
		public List<IHaveControl> PollingList { get { return polingList_; } }
		IHaveControl current_;
        IHaveControl default_;

        MessageSystem.Receptor receptor_;

		HashSet<int> ignorePriority_ = new HashSet<int>();
#if UNITY_EDITOR
		//エディタ専用
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public UserOperation() {
		}
#endif
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public UserOperation(int MessageLayer) {
			receptor_ = MessageSystem.CreateReceptor(this, MessageLayer);
#if DEVELOP_BUILD
			//OperationInfo.Init(polingList_);
#endif
		}

		/// <summary>
		/// デフォルト操作をセット
		/// </summary>
		public void SetDefault(int priority) {
			//登録されたリストの中からデフォルトにする制御を検索
			IHaveControl ctrl = null;
			for (int i = 0, max = polingList_.Count; i < max; i++) {
				if(polingList_[i].Priority == (int)priority) {
					ctrl = polingList_[i];
					break;
				}
			}
			Debug.Assert(ctrl != null, "not found default control");
			//デフォルト制御としてセットして制御開始
			default_ = ctrl;
			current_ = default_;
			current_.Begin();
		}

		/// <summary>
		/// デフォルト操作に戻す
		/// </summary>
		public void Reset() {
			current_.Reject();
			current_ = default_;
			current_.Begin();
		}

		/// <summary>
		/// 無視レベルに追加
		/// </summary>
		public void AddIgnore(int priority) {
			ignorePriority_.Add(priority);
			if(current_.Priority == priority) {
				current_.Reject();
			}
		}

		/// <summary>
		/// 無視レベルから削除
		/// </summary>
		public void RemoveIgnore(int priority) {
			ignorePriority_.Remove(priority);
		}

		/// <summary>
		/// ポーリングで監視する対象に登録
		/// </summary>
		public void Register(IHaveControl ctrl, bool conflict) {
            //２重で登録しようとした場合はあさーと
            Debug.Assert(conflict == true || polingList_.FindIndex(n => n.Priority == ctrl.Priority) < 0, "already register control");


            //追加してpriority順に並べ替え（そんなに数はないので重くないはず
            polingList_.Add(ctrl);
            polingList_.Sort((a, b) =>
            {
                if (a.Priority > b.Priority)
                    return 1;
                else if (a.Priority < b.Priority)
                    return -1;
                else
                    return 0;
            });
        }

		/// <summary>
		/// 登録解除
		/// </summary>
		public void UnRegister(IHaveControl ctrl) {
			polingList_.Remove(ctrl);
		}


        /// <summary>
        /// 実行処理
        /// </summary>
        public void Execute() {
            if (current_ == null)
                return;

            //制御中のコントロールが終わったら一番優先度の低いもの=デフォルト状態にセットする
            if (current_.IsEnd()) {
                current_ = default_;
                current_.Begin();
            }

            //制御開始するかチェック
            for (int i = 0, max = (int)polingList_.Count; i < max; i++) {
				//無視プライオリティの場合はチェックなし
				if (ignorePriority_.Contains(polingList_[i].Priority))
					continue;
                //現在動いている制御以下の優先度の場合は割り込ませない
                if (current_.Priority <= polingList_[i].Priority)
                    return;

                //割り込みチェック
                if (polingList_[i].Interrupt()) {
					//今の操作をリジェクトして新しい操作を始める
					current_.Reject();
					polingList_[i].Begin();
					current_ = polingList_[i];
                    return;
                }
            }
        }

    }
}
