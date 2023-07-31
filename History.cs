using UnityEngine;
using System.Collections.Generic;

namespace Project.Lib {
	/// <summary>
	/// 履歴管理クラス
	/// </summary>
	public class History<T> {
		Stack<T> history_ = new Stack<T>();
		Stack<T> temp_ = new Stack<T>();


		/// <summary>
		/// 履歴の次のウィンドウを切り替え表示
		/// </summary>
		public T Next(T currnet) {
			if (temp_.Count <= 0)
				return currnet;
			history_.Push(currnet);
			return temp_.Pop();
		}
		/// <summary>
		/// 履歴の前のウィンドウを切り替え表示
		/// </summary>
		public T Prev(T currnet) {
			if (history_.Count <= 0)
				return currnet;
			temp_.Push(currnet);
			return history_.Pop();
		}
		/// <summary>
		/// 履歴を記録
		/// </summary>
		public void RecordHistory(T currnet) {
			//現在位置以降の記録は削除して新規履歴を入れる
			history_.Push(currnet);
			temp_.Clear();
		}
	}
}
