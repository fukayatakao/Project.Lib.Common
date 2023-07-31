using UnityEngine;
using System.Collections;

namespace Project.Lib {
	/// <summary>
	/// 数学関数
	/// </summary>
	public static class MathUtil
	{ 
		/// <summary>
		/// エルミート補間
		/// </summary>
		public static float HermiteC(float v0, float t0, float v1, float t1, float s)
		{
			float SS   = s * s;
			float SS_S = s * s - s;
			float b1 = SS_S * s - SS_S;
			float b2 = SS_S * s;
			float a2 = SS - 2f * b2;

			return v0 - a2 * v0 + a2 * v1 + b1 * t0 + b2 * t1;
		}

		/// <summary>
		/// エルミート補間(ベクトル)
		/// </summary>
		public static Vector3 CalcHermite(Vector3 p0, Vector3 v0, Vector3 p1, Vector3 v1, float p)
		{
			Vector3 pos;
			pos.x = HermiteC(p0.x, v0.x, p1.x, v1.x, p);
			pos.y = HermiteC(p0.y, v0.y, p1.y, v1.y, p);
			pos.z = HermiteC(p0.z, v0.z, p1.z, v1.z, p);
			return pos;
		}

		/// <summary>
		/// イーズ関数
		/// </summary>
		/// <remarks>
		/// S字曲線の計算(0 <= t <= 1)
		/// 引数の値が0と1近辺では小さい変化、0.5前後で最大の変化
		/// </remarks>
		public static float	Ease(float t)
		{
			float t3 = t*t*t;		//tの3乗
			float t4 = t3*t;		//tの4乗
			float t5 = t4*t;		//tの5乗

			//返り値の範囲は0〜1
			float ret = (6 * t5) - (15 * t4) + (10 * t3);

			return ret;
		}


		public static int BitCount(int bits)
		{
			bits = (bits & 0x55555555) + ((bits >> 1) & 0x55555555);	// or bits & 0xAAAAAAAA >> 1
			bits = (bits & 0x33333333) + ((bits >> 2) & 0x33333333);	// or bits & 0xCCCCCCCC >> 2
			bits = (bits & 0x0f0f0f0f) + ((bits >> 4) & 0x0f0f0f0f);	// or bits & 0xF0F0F0F0 >> 4
			bits = (bits & 0x00ff00ff) + ((bits >> 8) & 0x00ff00ff);	// or bits & 0xFF00FF00 >> 8
			return (bits & 0x0000ffff) + ((bits >>16) & 0x0000ffff);	// or bits & 0xFFFF0000 >> 16
		}

		/// <summary>
		/// Y軸回転のクォータニオン
		/// </summary>
		public static Quaternion RotateEulerY(float degAngle){
			float half = degAngle * Mathf.Deg2Rad * 0.5f;
			return new Quaternion (0f, Mathf.Sin (half), 0f, Mathf.Cos (half));
		}

		/// <summary>
		/// X軸回転のクォータニオン
		/// </summary>
		public static Quaternion RotateEulerX(float degAngle){
			float angle = degAngle * Mathf.Deg2Rad * 0.5f;
			return new Quaternion (Mathf.Sin (angle), 0f, 0f, Mathf.Cos (angle));

		}
		/// <summary>
		/// Z軸回転のクォータニオン
		/// </summary>
		public static Quaternion RotateEulerZ(float degAngle){
			float angle = degAngle * Mathf.Deg2Rad * 0.5f;
			return new Quaternion (0f, 0f, Mathf.Sin (angle), Mathf.Cos (angle));

		}
        /// <summary>
        /// ベクトル方向を向くクォータニオン(Y軸固定)
        /// </summary>
        public static Quaternion LookAtY(Vector3 vec) {
            vec.y = 0f;
            float len = vec.magnitude;
            //長さ0の場合は単位クォータニオン返す
            if (len == 0f)
                return Quaternion.identity;
            //半角定理を使って高速でクォータニオンを直接作る
            float nx = vec.z / len;
            float qz = Mathf.Sqrt((1f - nx) * 0.5f) * Mathf.Sign(vec.x);
            float qw = Mathf.Sqrt((1f + nx) * 0.5f);
            return new Quaternion(0f, qz, 0f, qw);
        }
    }


}
