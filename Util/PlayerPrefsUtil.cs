using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Project.Lib {
	/// <summary>
	/// PlayerPrefsそのままだとレジストリに書き込まれて確認が面倒なのでローカルに保存する
	/// </summary>
	public static class PlayerPrefsUtil {
		// 文字列保存/取得
		public static void SetString(string key, string value) { Save(key, value); }
		public static string GetString(string key) { return Load(key); }

		// キーの存在チェック
		public static bool HasKey(string key) { return File.Exists(GetFileName(key)); }
		/// キーを削除
		public static void DeleteKey(string key) { File.Delete(GetFileName(key)); }
		// すべてのキーを削除
		public static void DeleteAll() { if (Directory.Exists(path_)) { Directory.Delete(path_, true); } }


		static readonly string path_ = UnityEngine.Application.dataPath + "/../Prefs/";
		/// <summary>
		/// ファイル名取得
		/// </summary>
		private static string GetFileName(string key) {
			return path_ + key + ".txt";
		}
		/// <summary>
		/// ローカルファイルに保存する
		/// </summary>
		private static void Save(string key, string value) {
			if (!Directory.Exists(path_)) {
				Directory.CreateDirectory(path_);
			}

			using (StreamWriter sw = new StreamWriter(GetFileName(key), false, Encoding.UTF8)) {
				sw.Write(value);
			}
		}

		/// <summary>
		/// ローカルファイルからロードする
		/// </summary>
		private static string Load(string key) {
			if (!File.Exists(GetFileName(key)))
				return null;
			using (StreamReader sr = new StreamReader(GetFileName(key), Encoding.UTF8)) {
				return sr.ReadToEnd();
			}
		}

	}
}
