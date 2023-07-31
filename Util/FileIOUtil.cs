using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Project.Lib {

	public static class FileIOUtil {
		/// <summary>
		/// ローカルファイルをロード
		/// </summary>
		public static byte[] Load(string path) {
			using (System.IO.FileStream fs = new System.IO.FileStream(path, System.IO.FileMode.Open, System.IO.FileAccess.Read)) {
				int length = (int)fs.Length;
				byte[] temp = new byte[length];
				fs.Read(temp, 0, length);

				return temp;
			}
		}
		/// <summary>
		/// ローカルファイルにセーブ
		/// </summary>
		public static void Save(string path, byte[] data) {
			//上書きセーブ
			using (System.IO.FileStream fs = System.IO.File.Create(path)) {
				fs.Write(data, 0, data.Length);
			}
		}
		/// <summary>
		/// ファイルの存在チェック
		/// </summary>
		public static bool Exists(string path) {
			return System.IO.File.Exists(path);
		}

		/// <summary>
		/// ファイルの削除
		/// </summary>
		public static void Delete(string path) {
			System.IO.File.Delete(path);
		}
	}
}
