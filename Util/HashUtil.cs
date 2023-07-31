using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

using System.Security.Cryptography;

namespace Project.Lib {


	public static class HashUtil {
		/// <summary>
		/// バイナリデータのMD5ハッシュを計算
		/// </summary>
		public static string ComputeMd5Hash(byte[] data) {
			byte[] local_basedata_hash_binary = MD5.Create().ComputeHash(data);
			return System.BitConverter.ToString(local_basedata_hash_binary).ToLower().Replace("-", "");

		}
	}
}