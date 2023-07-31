using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Lib {
	/// <summary>
	/// MonoBehaviour継承を切り替えるクラス
	/// </summary>
	/// <remarks>
	/// 基本的にMonoBehaviour継承はさせたくない
	/// でもInspectorに変数が出せるといろいろ便利なので普段はMonoBehaviour継承させて、リリース時にはMonoBehaviour継承しないようにするクラスを作る
	/// </remarks>
#if USE_MONOBEHAVIOUR
	public class MonoPretender : MonoBehaviour {
#else
	public class MonoPretender {
#endif
		private bool isAlive_;
		/// <summary>
		/// インスタンス生成
		/// </summary>
		public static T Create<T>(GameObject obj) where T : MonoPretender, new() {
#if USE_MONOBEHAVIOUR
			T instance = obj.AddComponent<T>();
#else
			T instance = new T();
#endif
			instance.Create(obj);
			instance.isAlive_ = true;

			return instance;
        }

		/// <summary>
		/// インスタンス破棄
		/// </summary>
		public static void Destroy(MonoPretender comp) {
			if (comp == null)
				return;
			comp.Destroy();
#if USE_MONOBEHAVIOUR
			GameObject.Destroy(comp);
#endif
			comp.isAlive_ = false;
		}

        /// <summary>
        /// サブコンポーネントがある場合はここで生成
        /// </summary>
        protected virtual void Create(GameObject obj) {
        }

		protected virtual void Destroy() {
		}
#if USE_MONOBEHAVIOUR
		public void OnDestroy(){
#else
		~MonoPretender() {
#endif
			Debug.Assert(!isAlive_, "alread alive instance:" + GetType().ToString());
		}
		/// <summary>
		/// コンポーネントを隠す
		/// </summary>
		[System.Diagnostics.Conditional("UNITY_EDITOR")]
		public void HideComponent() {
#if USE_MONOBEHAVIOUR
			hideFlags |= HideFlags.HideInInspector;
#endif
		}

        /// <summary>
        /// コンポーネントを表示する
        /// </summary>
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public void ShowComponent() {
#if USE_MONOBEHAVIOUR
            hideFlags ^= HideFlags.HideInInspector;
#endif
        }
    }

}
