using System;

namespace Project.Lib {
	public static class TimeUtil {
		//基準となる時間(1970/01/01 00:00)
		private static DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0);

		/// <summary>
		/// 現在時刻のUnixTime(秒)を取得
		/// </summary>
		public static long GetUnixTimeSeconds(DateTime dateTime) {
			return (long)(dateTime - UnixEpoch).TotalSeconds;
		}
		/// <summary>
		/// UnixTime(秒)からDateTimeへ変換
		/// </summary>
		public static DateTime GetDateTimeSeconds(long unixTimeSeconds) {
			return UnixEpoch.AddSeconds(unixTimeSeconds);
		}
		/// <summary>
		/// DateTimeからUnixTime(ミリ秒)へ変換
		/// </summary>
		public static long GetUnixTime(DateTime dateTime) {
			return (long)(dateTime - UnixEpoch).TotalMilliseconds;
		}
		/// <summary>
		/// UnixTime(ミリ秒)からDateTimeへ変換
		/// </summary>
		public static DateTime GetDateTime(long unixTime) {
			return UnixEpoch.AddMilliseconds(unixTime);
		}
	}
}
