using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace Project.Lib {
	public class EditorGUIContent
	{
		private static GUIContent playContent;
		private static GUIContent pauseContent;
		private static GUIContent loopContent;
		private static GUIContent effectContent;
		private static GUIContent throughContent;
		private static GUIContent stopContent;

		/// <summary>
		/// 折り畳み
		/// </summary>
		public static bool Foldout(string title, bool display)
		{
			return Foldout( title, display, Color.white );
		}
		public static bool Foldout(string title, bool display, Color textColor)
		{
			var style = new GUIStyle("ShurikenModuleTitle");
			GUIStyleState styleState = new GUIStyleState();

			style.font = new GUIStyle(EditorStyles.label).font;
			style.border = new RectOffset(5, 5, 4, 4);
			style.fixedHeight = 22;
			style.contentOffset = new Vector2(20f, -3f);
			styleState.textColor = textColor;
			style.normal = styleState;

			var rect = GUILayoutUtility.GetRect(16f, 27f, style);
			GUI.Box(rect, title, style);

			var e = Event.current;

			var toggleRect = new Rect(rect.x + 4f, rect.y + 2f, 13f, 13f);
			if (e.type == EventType.Repaint)
			{
				EditorStyles.foldout.Draw(toggleRect, false, false, display, false);
			}

			if (e.type == EventType.MouseDown && rect.Contains(e.mousePosition))
			{
				display = !display;
				e.Use();
			}

			return display;
		}
        /// <summary>
        /// ポーズコンテンツ
        /// </summary>
        public static GUIContent PlusContent(string label = "") {
            if (pauseContent == null) {
                pauseContent = new GUIContent();
                pauseContent.image = EditorGUIUtility.Load("icons/d_PauseButton.png") as Texture2D;
            }

            pauseContent.text = label;

            return pauseContent;
        }

        /// <summary>
        /// プレイコンテンツ
        /// </summary>
        public static GUIContent PlayContent(string label = "")
		{
			if (playContent == null)
			{
				playContent = new GUIContent();
				playContent.image = EditorGUIUtility.Load("icons/d_PlayButton.png") as Texture2D;
			}

			playContent.text = label;
			return playContent;
		}

		/// <summary>
		/// ポーズコンテンツ
		/// </summary>
		public static GUIContent PauseContent(string label = "")
		{
			if (pauseContent == null)
			{
				pauseContent = new GUIContent();
				pauseContent.image = EditorGUIUtility.Load("icons/d_PauseButton.png") as Texture2D;
			}

			pauseContent.text = label;

			return pauseContent;
		}

		/// <summary>
		/// ループコンテンツ
		/// </summary>
		public static GUIContent LoopContent(string label = "")
		{
			if (loopContent == null)
			{
				loopContent = new GUIContent();
				loopContent.image = EditorGUIUtility.Load("icons/d_RotateTool.png") as Texture2D;
			}

			loopContent.text = label;

			return loopContent;
		}

		/// <summary>
		/// エフェクトコンテンツ
		/// </summary>
		public static GUIContent EffectContent(string label = "")
		{
			if (effectContent == null)
			{
				effectContent = new GUIContent();
				effectContent.image = EditorGUIUtility.Load("icons/d_Particle Effect.png") as Texture2D;
			}

			effectContent.text = label;
			return effectContent;
		}

		/// <summary>
		/// スルーコンテンツ
		/// </summary>
		public static GUIContent ThroughContent(string label = "") {
			if (throughContent == null) {
				throughContent = new GUIContent();
				throughContent.image = EditorGUIUtility.Load("PlayButton On") as Texture2D;
			}

			throughContent.text = label;
			return throughContent;
		}

		/// <summary>
		/// ストップコンテンツ
		/// </summary>
		public static GUIContent StopContent(string label = "") {
			if (stopContent == null) {
				stopContent = new GUIContent();
				stopContent.image = EditorGUIUtility.Load("PauseButton On") as Texture2D;
			}

			stopContent.text = label;
			return stopContent;
		}

		/// <summary>
		/// 指定した色のテクスチャを作成する
		/// </summary>
		public static Texture2D CreateColorTexture(Color color) {
            Texture2D createTexture = new Texture2D(1, 1);
            createTexture.SetPixel(0, 0, color);
            createTexture.Apply();
            return createTexture;
        }
    }
}
