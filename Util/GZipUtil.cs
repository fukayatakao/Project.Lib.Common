using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Compression;


namespace Project.Lib {

	public static class GZipUtil {
		private const int MSTDATA_DECOMPRESS_READ_BUFFER_SIZE = (128 * 1024);
		/// <summary>
		/// Json文字列を圧縮したバイナリデータを解凍する
		/// </summary>
		public static string decompress(byte[] binary) {
			//解凍結果の格納場所
			System.IO.MemoryStream output = new System.IO.MemoryStream();
			//作業用のバッファを確保
			byte[] buffer = new byte[MSTDATA_DECOMPRESS_READ_BUFFER_SIZE];


			//解凍作業
			using (GZipStream gzip_stream = new GZipStream(new System.IO.MemoryStream(binary), CompressionMode.Decompress, true)) {
				while (true) {
					int read_size = gzip_stream.Read(buffer, 0, buffer.Length);
					//終わったらループを抜ける
					if (read_size == 0) {
						break;
					}

					output.Write(buffer, 0, read_size);
				}
			}

			return Encoding.UTF8.GetString(output.ToArray());
		}
		/// <summary>
		/// Json文字列を圧縮したバイナリデータを圧縮する
		/// </summary>
		public static byte[] compress(string text) {
			//解凍結果の格納場所
			System.IO.MemoryStream output = new System.IO.MemoryStream();

			using (GZipStream gzip_stream = new GZipStream(output, CompressionMode.Compress)) {
				using (var writer = new System.IO.StreamWriter(gzip_stream)) {
					writer.Write(text);
				}
			}
			return output.ToArray();
		}
	}
}

