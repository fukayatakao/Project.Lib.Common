using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;


namespace Project.Lib {
    /// <summary>
    /// 複数のコルーチンを順次自力実行する
    /// </summary>
	public class CoroutineTaskList {
		private List<IEnumerator> taskList_ = new List<IEnumerator>();


		/// <summary>
		/// Taskを追加
		/// </summary>
		public void Add(IEnumerator task)
		{
			taskList_.Add (task);
		}

		/// <summary>
		/// Taskを追加
		/// </summary>
		public void AddRange(List<IEnumerator> task)
		{
			taskList_.AddRange (task);
		}

		/// <summary>
		/// Taskをクリア
		/// </summary>
		public void Clear()
		{
			taskList_.Clear ();
		}
		/// <summary>
		/// タスクがすべて終了しているか？
		/// </summary>
		public bool IsEnd()
		{
			if (taskList_.Count == 0)
				return true;
			else
				return false;
		}
		/// <summary>
		/// 残りタスク数を取得
		/// </summary>
		public int Count()
		{
			return taskList_.Count;
		}
		/// <summary>
		/// タスク実行
		/// </summary>
		public void Execute()
		{
			while (taskList_.Count > 0) {
				//task実行
				bool ret = taskList_ [0].MoveNext ();
				//タスク終わっていたら次を即開始
				if (!ret) {
					taskList_.RemoveAt (0);
				//タスク継続なら一旦抜ける
				} else {
					break;
				}
			}
		}
	}
	/// <summary>
	/// 複数のコルーチンを順次自力実行する
	/// </summary>
	public class CoroutineTaskParallel {
		private List<IEnumerator> parallelTaskList_ = new List<IEnumerator>();

		/// <summary>
		/// Taskを追加
		/// </summary>
		public void Add(IEnumerator task) {
			parallelTaskList_.Add(task);
		}

		/// <summary>
		/// Taskを追加
		/// </summary>
		public void AddRange(List<IEnumerator> task) {
			parallelTaskList_.AddRange(task);
		}

		/// <summary>
		/// Taskをクリア
		/// </summary>
		public void Clear() {
			parallelTaskList_.Clear();
		}
		/// <summary>
		/// タスクがすべて終了しているか？
		/// </summary>
		public bool IsEnd() {
			if (parallelTaskList_.Count == 0)
				return true;
			else
				return false;
		}
		/// <summary>
		/// 動作タスク数を取得
		/// </summary>
		public int Count() {
			return parallelTaskList_.Count;
		}
		/// <summary>
		/// タスク実行
		/// </summary>
		public void Execute() {
			for (int i = 0, max = parallelTaskList_.Count; i < max; i++) {
				//task実行
				bool ret = parallelTaskList_[i].MoveNext();
				//タスク終わっていたら削除
				if (!ret) {
					parallelTaskList_.RemoveAt(i);
					i--;
					max = parallelTaskList_.Count;
				}
			}
		}
	}

	/// <summary>
	/// 単体のコルーチンを自力実行
	/// </summary>
	public class CoroutineTask {
        private IEnumerator task_;

        /// <summary>
        /// Taskを実行
        /// </summary>
        public void Play(IEnumerator task) {
            task_ = task;
        }
        /// <summary>
        /// Taskをクリア
        /// </summary>
        public void Clear() {
            task_ = null;
        }
        /// <summary>
        /// タスクが終了しているか？
        /// </summary>
        public bool IsEnd() {
            if (task_ == null)
                return true;
            else
                return false;
        }
        /// <summary>
        /// タスク実行
        /// </summary>
        public void Execute() {
            if (task_ == null)
                return;
            //task実行
            bool ret = task_.MoveNext();
            //タスク終わっていたら後始末
            if (!ret) {
                task_ = null;
            }
        }
    }

}
