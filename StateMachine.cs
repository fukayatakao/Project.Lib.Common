using UnityEngine;

namespace Project.Lib {
	/// <summary>
	/// 状態クラス
	/// </summary>
	public abstract class IState<T> : MonoPretender
	{
		// 状態に入ったときに実行
		public abstract void Enter(T entity);
		// 常時実行
		public abstract void Execute(T entity);
		// 状態から抜けるときに実行
		public abstract void Exit(T entity);
	}
	/// <summary>
	/// 状態マシン
	/// </summary>
	public class StateMachine<T>
	{ 
		//現在の状態
		protected int currentNo_ = -1;
		public int CurrentStateNo{ get { return currentNo_; } }

        //状態クラスの置き場所
        protected IState<T>[] stateArray_;
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public StateMachine(int max)
		{
			stateArray_ = new IState<T>[max];
		}

		/// <summary>
		/// 実行処理
		/// </summary>
		public void Execute(T owner) {
			stateArray_[currentNo_].Execute(owner);
		}
		/// <summary>
		/// 状態の登録
		/// </summary>
		public void Register(IState<T> state, int num) {
			state.HideComponent();
			stateArray_[num] = state;
		}


		/// <summary>
		/// 状態の登録
		/// </summary>
		public void Register<U>(GameObject obj, int num) where U : IState<T>, new () {
			stateArray_[num] = MonoPretender.Create<U>(obj);
			stateArray_[num].HideComponent();

		}
		/// <summary>
		/// 状態の破棄
		/// </summary>
		public void UnRegisterAll(){
			for(int i = 0, max = stateArray_.Length; i < max; i++) {
				if (stateArray_[i] == null)
					continue;
				MonoPretender.Destroy(stateArray_[i]);
			}
		}

		/// <summary>
		/// 現在の状態クラスを取得
		/// </summary>
		public IState<T> GetState(int stateNo) {
			return stateArray_[stateNo];
		}

		/// <summary>
		/// 開始状態を指定
		/// </summary>
		public void SetFirstState(int num) {
			Debug.Assert(stateArray_[num] != null, "change state is not regist:" + num);
			currentNo_ = num;
			stateArray_[currentNo_].ShowComponent();
		}

		/// <summary>
		/// 状態変更
		/// </summary>
		public void ChangeState(T owner, int stateNo, bool force = false) {
			//同じ状態への遷移は無視
			if (currentNo_ == stateNo && !force)
				return;

			//未登録のstateを使おうとしたらAssert
			Debug.Assert(stateArray_[currentNo_] != null, "current state is not regist:" + currentNo_);
			Debug.Assert(stateArray_[stateNo] != null, "change state is not regist:" + stateNo);

			stateArray_[currentNo_].Exit(owner);
			stateArray_[currentNo_].HideComponent();
			currentNo_ = stateNo;
			stateArray_[currentNo_].ShowComponent();
			stateArray_[currentNo_].Enter(owner);

		}
	}


}
