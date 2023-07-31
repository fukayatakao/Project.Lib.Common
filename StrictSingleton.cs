using System;
using System.Collections;
using System.Collections.Generic;


namespace Project.Lib {

	/// <summary>
	/// インスタンスの生成・破棄を明示的に要求するシングルトン
	/// </summary>
	/// <remarks>
	/// 明示的にインスタンス破棄しないとアプリ終了するまで残り続けるので注意
	/// <remarks>
	public class StrictSingleton<T> where T : class, new() {
		private static T instance_;
		public static T I { get { return instance_; } }

		/// <summary>
		/// 明示的にインスタンスを作る
		/// </summary>
		public static void CreateInstance(){
			if (instance_ == null)
				instance_ = new T ();
		}

		/// <summary>
		/// 明示的にインスタンスを破棄
		/// </summary>
		public static void DestroyInstance(){
			instance_ = null;
		}

	}
	/// <summary>
	/// 普通のシングルトン
	/// </summary>
	public class Singleton<T> where T : class, new() {
		private static T instance_;
		public static T I {
			get {
				if(instance_ == null) {
					instance_ = new T();
				}
				return instance_;
			}
		}
	}
}
