using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace Project.Lib {
	/// <summary>
	/// デバッグ用関数
	/// </summary>
	public static class EditorUtil
	{ 
		/// <summary>
		/// 選択リストを表示する
		/// </summary>
		public static int SelectList(string[] list, int index)
		{
			for (int i = 0; i < list.Length; i++)
			{
				bool flag = GUILayout.Toggle(index == i, list[i], "PreferencesKeysElement");
				if (flag != (index == i))
				{
					return i;
				}
			}

			return index;
		}

		/// <summary>
		/// ライン描画
		/// </summary>
		public static void DrawLine()
		{
			GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(1));
		}
		


		/// <summary>
		/// 検索ボックス
		/// </summary>
        public static string SearchField(string text)
        {

			Rect rect = GUILayoutUtility.GetRect(16f, 24f, 16f, 16f, new GUILayoutOption[]
            {
                GUILayout.Width(250f) // 検索ボックスのサイズ
            });
            rect.x += 4f;
            rect.y += 4f;
	 
			Rect buttonRect = rect;
            buttonRect.x += rect.width;
            buttonRect.width = 14f;

			//検索文字列
            text = EditorGUI.TextField(rect, text);
	
            return text;
        }

		//ロードしたリソースのキャッシュ用
		private static Dictionary<string, GUIContent> iconContentDict_ = new Dictionary<string, GUIContent>();
		/// <summary>
		/// アイコン取得
		/// </summary>
		public static GUIContent GetIconContent(string iconName, string label = "")
		{
			if (!iconContentDict_.ContainsKey(iconName))
			{
				iconContentDict_[iconName] = new GUIContent();
				iconContentDict_[iconName].image = EditorGUIUtility.Load(iconName) as Texture2D;
			}

			iconContentDict_[iconName].text = label;

			return iconContentDict_[iconName];
		}
		/// <summary>
		/// ディレクトリの中にあるアセット名の一覧を取得
		/// </summary>
		public static string[] GetFilenames(string path, string ext="asset") {
			path = path.TrimEnd('/');
			string dir_path = Application.dataPath + path.Substring("Assets".Length);
			string[] filenameArray = System.IO.Directory.GetFiles(dir_path, "*." + ext);
			for (int i = 0; i < filenameArray.Length; i++) {
				filenameArray[i] = filenameArray[i].Split('\\')[1];
				filenameArray[i] = filenameArray[i].Replace("." + ext, "");
			}
			return filenameArray;
		}
		/// <summary>
		/// ディレクトリの中にあるアセット名の一覧を取得
		/// </summary>
		public static string[] GetDirectories(string path) {
			path = path.TrimEnd('/');
			string dir_path = Application.dataPath + path.Substring("Assets".Length);
			string[] directoryArray = System.IO.Directory.GetDirectories(dir_path, "*");
			for (int i = 0; i < directoryArray.Length; i++) {
				directoryArray[i] = directoryArray[i].Split('\\')[1];
			}
			return directoryArray;
		}
	}
}
