using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace Project.Lib {
	public static partial class SystemMessage {
		/// <summary>
		/// ユーザー制御の振り分けシステムに登録
		/// </summary>
		public static class UnRegisterControl {
			//メッセージ種別のID
			private static int ID = -1;
			//送付するデータ内容xv
			private class Data {
				public IHaveControl control;
			}

			/// <summary>
			/// メッセージを送る
			/// </summary>
			public static void Broadcast(IHaveControl haveControl) {
				MessageSystem.Broadcast(
					new MessageObject(ID, new Data { control = haveControl })
				);
			}

			/// <summary>
			/// メッセージを受信して処理
			/// </summary>
			private static void Recv(UserOperation assign, MessageObject msg) {
				Data data = (Data)msg.Data;
				assign.UnRegister(data.control);
			}
		}
	}
}
