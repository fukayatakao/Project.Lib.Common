using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Project.Lib {
	public static class UnityUtil {
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		//インスタンスに関するUtil関数

		/// <summary>
		/// 子要素から指定した名前のtransformを取得
		/// </summary>
		public static Transform SearchTransform(Transform trans, string name, bool isActiveOnly=false) {
			Transform[] transArray = trans.GetComponentsInChildren<Transform> (!isActiveOnly);
			for (int i = 0, count = transArray.Length; i < count; i++) {
				if (transArray[i].name == name) {
					return transArray[i];
				}
			}
			return null;
		}
		/// <summary>
		/// 子階層を含めたすべてのgameobjectのレイヤーを設定する
		/// </summary>
		public static void SetLayer(Transform trans, int layer, bool isActiveOnly=false){
			Transform[] transArray = trans.GetComponentsInChildren<Transform> (!isActiveOnly);
			for (int i = 0, count = transArray.Length; i < count; i++) {
				transArray [i].gameObject.layer = layer;
			}
		}
		/// <summary>
		/// 安全にゲームオブジェクトの生成をする
		/// </summary>
		public static GameObject Instantiate(GameObject asset) {
			if (asset == null) {
				Debug.Assert(false, "Instantiate source is null. use dummy."); 
				return new GameObject("dummy");
			}

			return GameObject.Instantiate(asset);
		}

		/// <summary>
		/// 空のゲームオブジェクトの生成する
		/// </summary>
		public static GameObject InstantiateChild(Transform parent, string name) {
			GameObject obj = new GameObject(name);
			Transform trans = obj.transform;
            trans.SetParent(parent);
            trans.localPosition = Vector3.zero;
			trans.localRotation = Quaternion.identity;
			trans.localScale = Vector3.one;

			return obj;
		}
		/// <summary>
		/// 空のゲームオブジェクトの生成する
		/// </summary>
		public static GameObject InstantiateChild(Transform parent, string name, Vector3 localPosition, Quaternion localRotation, Vector3 localScale) {
			GameObject obj = new GameObject(name);
			Transform trans = obj.transform;
            trans.SetParent(parent);
            trans.localPosition = localPosition;
			trans.localRotation = localRotation;
			trans.localScale = localScale;

			return obj;

		}
		/// <summary>
		/// 指定したオブジェクトの子にインスタンスを生成する
		/// </summary>
		public static GameObject InstantiateChild(Transform parent, GameObject prefab, Vector3 localPosition, Quaternion localRotation, Vector3 localScale) {
			GameObject obj = Instantiate(prefab);
			Transform trans = obj.transform;
            trans.SetParent(parent);
            trans.localPosition = localPosition;
			trans.localRotation = localRotation;
			trans.localScale = localScale;
			return obj;
		}
		/// <summary>
		/// 指定したオブジェクトの子にインスタンスを生成する
		/// </summary>
		public static GameObject InstantiateChild(Transform parent, GameObject prefab) {
			GameObject obj = Instantiate(prefab);
			Transform trans = obj.transform;
			trans.SetParent(parent);
			trans.localPosition = Vector3.zero;
			trans.localRotation = Quaternion.identity;
			trans.localScale = Vector3.one;
			return obj;
		}


		/// <summary>
		/// エディタとゲーム中どちらでも破棄出来る
		/// </summary>
		public static void DestroyGameObject(Transform trans) {
			if (trans == null)
				return;
			DestroyGameObject(trans.gameObject);
		}

		/// <summary>
		/// エディタとゲーム中どちらでも破棄出来る
		/// </summary>
		public static void DestroyGameObject(GameObject obj) {
			if (obj == null)
				return;
			//実行中は遅れて破棄
			if (Application.isPlaying) {
				GameObject.Destroy(obj);
			//エディタでは即時破棄
			} else {
				GameObject.DestroyImmediate(obj);
			}
		}
		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		//c#nativeに関するUtil関数

		/// <summary>
		/// セーフティに呼び出す
		/// </summary>
		public static void Call(System.Action action) {
			Debug.Assert (action != null, "call action is null: " + action.ToString());
			if (action != null) {
				action();
			}
		}
        /// <summary>
        /// 自力でコルーチン実行
        /// </summary>
        public static void MoveNext(ref IEnumerator coroutine) {
            //コルーチンがセットされてたら実行
            if (coroutine != null) {
                bool ret = coroutine.MoveNext();
                //タスク終わったら念のため明示的にnull入れとく
                if (!ret){
                    coroutine = null;
                }
            }

        }

		//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		//そのほか

		/// <summary>
		/// 指定した色のテクスチャを作成する
		/// </summary>
		public static Texture2D CreateColorTexture(Color color)
		{
			Texture2D createTexture = new Texture2D(1, 1);
			createTexture.SetPixel(0, 0, color);
			createTexture.Apply();
			return createTexture;
		}

    }

}
