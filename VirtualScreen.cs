using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


namespace Project.Lib {
    /// <summary>
    /// 仮想スクリーン用座標計算
    /// </summary>
    [DefaultExecutionOrder(-1)]
	public class VirtualScreen : MonoBehaviour{
        //ピクセル座標から仮想スクリーン座標への変換倍率
        public static Vector2 Pixel2Screen = new Vector2();
        //仮想スクリーン座標からピクセル座標の変換倍率
        public static Vector2 Screen2Pixel = new Vector2();
        //仮想スクリーンサイズ。Inspectorで設定。NGUIで使ってるものと同じ値にする。
        [SerializeField]
		private int manualWidth = 1280;
		[SerializeField]
		private int manualHeight = 1136;

		[SerializeField]
		private Constraint constraint = Constraint.FitHeight;

        //初期化終わったか
        private bool enable_ = false;

		//縦横どこを基準にするか
		public enum Constraint
		{
			Fit,
			Fill,
			FitWidth,
			FitHeight,
		}
        /// <summary>
        /// インスタンス生成時処理
        /// </summary>
		private void Awake(){
			instance_ = this;
			float calcActiveHeight = activeHeight(constraint, manualWidth, manualHeight);
			ResizeTransformScale (calcActiveHeight);

            Pixel2Screen.x = (float)VirtualScreen.ScreenWidth / Screen.width;
            Pixel2Screen.y = (float)VirtualScreen.ScreenHeight / Screen.height;

			Screen2Pixel.x = (float)Screen.width / VirtualScreen.ScreenWidth;
 			Screen2Pixel.y = (float)Screen.height / VirtualScreen.ScreenHeight;

            enable_ = true;
       }

#if UNITY_EDITOR
        private void Update ()
		{
            //Unity上だとウィンドウのサイズが常に変化する可能性あるので毎回計算させておく
            float calcActiveHeight = activeHeight(constraint, manualWidth, manualHeight);
			ResizeTransformScale (calcActiveHeight);

            //Unityはウィンドウのサイズを変えると変換比率も変わるので常に計算させる。
            Pixel2Screen.x = (float)VirtualScreen.ScreenWidth / Screen.width;
            Pixel2Screen.y = (float)VirtualScreen.ScreenHeight / Screen.height;

			Screen2Pixel.x = (float)Screen.width / VirtualScreen.ScreenWidth;
 			Screen2Pixel.y = (float)Screen.height / VirtualScreen.ScreenHeight;

        }
#endif

        /// <summary>
        /// 仮想スクリーンの高さを元にスケールをかける
        /// </summary>
        /// <param name="calcHeight">Calculate height.</param>
        private void ResizeTransformScale(float calcActiveHeight){
			if (calcActiveHeight > 0f )
			{
				float size = 2f / calcActiveHeight;

				Vector3 ls = transform.localScale;

				if (!(Mathf.Abs(ls.x - size) <= float.Epsilon) ||
					!(Mathf.Abs(ls.y - size) <= float.Epsilon) ||
					!(Mathf.Abs(ls.z - size) <= float.Epsilon))
				{
					transform.localScale = new Vector3(size, size, size);
				}
			}
		}

		/// <summary>
		/// ピクセルサイズを元に仮想スクリーン座標への変換値を計算する(?)
		/// </summary>
		/// <remarks>
		/// NGUIのUIRootから移植
		/// </remarks>
		private int activeHeight(Constraint cons, int width, int height)
		{
			if (cons == Constraint.FitHeight)
				return height;

			Vector2 screen = screenSize;
			float aspect = screen.x / screen.y;
			float initialAspect = (float)width / height;

			switch (cons) {
			case Constraint.FitWidth:
				return Mathf.RoundToInt (width / aspect);
			case Constraint.Fit:
				return (initialAspect > aspect) ? Mathf.RoundToInt (width / aspect) : height;
			case Constraint.Fill:
				return (initialAspect < aspect) ? Mathf.RoundToInt (width / aspect) : height;
			}
			return height;
		}


		//@note NGUIから移植
#if UNITY_EDITOR
		static System.Reflection.MethodInfo s_GetSizeOfMainGameView;
		static Vector2 mGameSize = Vector2.one;

		/// <summary>
		/// Unity上でのGameウィンドウのサイズを取得する
		/// </summary>
		static private Vector2 screenSize
		{
			get
			{
				if (!Application.isPlaying)
				{
					if (s_GetSizeOfMainGameView == null)
					{
						System.Type type = System.Type.GetType("UnityEditor.GameView,UnityEditor");
						s_GetSizeOfMainGameView = type.GetMethod("GetSizeOfMainGameView",
							System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
					}
					mGameSize = (Vector2)s_GetSizeOfMainGameView.Invoke(null, null);
				}
				return mGameSize;
			}
		}
#else
		/// <summary>
		/// Size of the game view cannot be retrieved from Screen.width and Screen.height when the game view is hidden.
		/// </summary>

		static private Vector2 screenSize { get { return new Vector2(Screen.width, Screen.height); } }
#endif
		//入力制御をつけるgameobject名
		const string VirtualScreenObjerctName = "VirtualScreen";
		private static GameObject screenObject_;

		/// <summary>
		/// インスタンス作成
		/// </summary>
		public static GameObject Create() {
			if (screenObject_ == null) {
				//ゲームオブジェクト未作成の場合作る
				screenObject_ = new GameObject(VirtualScreenObjerctName);
				instance_ = screenObject_.AddComponent<VirtualScreen>();
				GameObject.DontDestroyOnLoad(screenObject_);
			}

			return screenObject_;
		}

		/// <summary>
		/// インスタンスの存在チェック
		/// </summary>
		public static bool IsValid() {
			return instance_ != null && instance_.enable_;
		}

		//隠れシングルトン
		private static VirtualScreen instance_;

		/// <summary>
		/// 仮想スクリーンの幅
		/// </summary>
		public static int ScreenWidth {
			get {
				Debug.Assert(instance_ != null, "instance error: VirtualScreenUtil");
				return (int)(Screen.width * (ScreenHeight / (float)Screen.height));
			}
		}

		/// <summary>
		/// 仮想スクリーンの高さ
		/// </summary>
		public static int ScreenHeight {
			get {
				Debug.Assert(instance_ != null, "instance error: VirtualScreenUtil");
				return instance_.manualHeight;
			}
		}


        /// <summary>
        /// 座標をピクセルから仮想スクリーンに変換
        /// </summary>
        public static Vector2 PixelToScreenPos(Vector2 pos) {
			Debug.Assert(instance_ != null, "instance error: VirtualScreenUtil");
			pos.Set(
                pos.x * Pixel2Screen.x - 0.5f * ScreenWidth,
                pos.y * Pixel2Screen.y - 0.5f * ScreenHeight);
            return pos;
        }

        /// <summary>
        /// 変化量をピクセルから仮想スクリーンに変換
        /// </summary>
        public static Vector2 PixelToScreenDelta(Vector2 delta) {
			Debug.Assert(instance_ != null, "instance error: VirtualScreenUtil");
			delta.Set(
                delta.x * Pixel2Screen.x,
                delta.y * Pixel2Screen.y);
            return delta;
        }

    }
}
