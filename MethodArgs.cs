using System;
using System.Collections.Generic;
using UnityEngine;


namespace Project.Lib {
	/// <summary>
	/// refrectionで処理を文字列化するとき用のユーティリティ
	/// </summary>
	public static class MethodArgs {
		/// <summary>
		/// 引数を文字列化する
		/// </summary>
		public static string[] SerializeArgs(object[] objectArgs) {
			if (objectArgs == null)
				return null;

			string[] args = new string[objectArgs.Length];
			for (int i = 0; i < objectArgs.Length; i++) {
				//@note 使用可能な型は1.Enum, 2.string, 3.Vector3型, 4.Parse関数を持っている型
				//引数の型が使用可能かチェック
				Debug.Assert(objectArgs[i].GetType().IsEnum || objectArgs[i].GetType() == typeof(string) || objectArgs[i].GetType() == typeof(Vector3) || objectArgs[i].GetType().GetMethod("Parse", new Type[] { typeof(string) }) != null, objectArgs[i].GetType().ToString() + " is not support");
				if (objectArgs[i].GetType() == typeof(Vector3)) {
					Vector3 vec = (Vector3)objectArgs[i];
					args[i] = objectArgs[i].GetType().ToString() + ":" + vec.x.ToString() + "," + vec.y.ToString() + "," + vec.z.ToString();
				} else {
					args[i] = objectArgs[i].GetType().AssemblyQualifiedName + ":" + objectArgs[i].ToString();
				}
			}

			return args;
		}

		static string Vector3TypeName = typeof(Vector3).ToString();
		/// <summary>
		/// 引数をプログラムで使える状態に変換
		/// </summary>
		public static object[] DeserializeArgs(string[] args) {
			if (args == null)
				return null;
			object[] objectArgs = new object[args.Length];
			for (int i = 0, max = args.Length; i < max; i++) {
				string[] arg = args[i].Split(':');
				//Vector3の場合は最初に検査する。
				if (arg[0] == Vector3TypeName) {
					string[] v = arg[1].Split(',');
					objectArgs[i] = new Vector3(float.Parse(v[0]), float.Parse(v[1]), float.Parse(v[2]));
					continue;
				}
				//UnityのクラスとかはType.GetTypeでnullが返ってくるので紛れ込んでいたらエラー出す
				Type t = Type.GetType(arg[0]);
				Debug.Assert(t != null, "argment type error: " + arg[0]);

				//型がenumだった場合
				if (t.IsEnum) {
					//enum用のParseをかける
					objectArgs[i] = Enum.Parse(t, arg[1]);
				//string型の場合はParse不要
				} else if (t == typeof(string)) {
					objectArgs[i] = arg[1];
				//それ以外は"Parse"関数があることを期待してParseを呼ぶ。無いとエラーがthrowされるので注意
				} else {
					objectArgs[i] = t.InvokeMember("Parse", System.Reflection.BindingFlags.InvokeMethod, null, null, new object[] { arg[1] });
				}
			}

			return objectArgs;
		}	
		
		//バイナリで保存する形式をサポートしている型
		public enum SupportType : int{
			NotSupport		= 0,
			TypeBool		= 1,
			TypeInt			= 2,
			TypeShort		= 3,
			TypeLong		= 4,
			TypeFloat		= 5,
			TypeDouble		= 6,
			TypeVector3		= 7,
		}
		/// <summary>
		/// 引数をアセットに保存する(バイナリ対応
		/// </summary>
		public static void SerializeArgs(object[] objectArgs, out List<int> types, out string[] args, out byte[] binary) {
			SerializeArgsBinary(objectArgs, out types, out binary);
			SerializeArgsString(objectArgs, out args);
		}
		/// <summary>
		/// 引数をバイナリ化できるものはバイナリ保存する
		/// </summary>
		public static void SerializeArgsBinary(object[] objectArgs, out List<int> types, out byte[] binary) {
			List<byte> binaryList= new List<byte>();
			types = new List<int>();
			for (int i = 0; i < objectArgs.Length; i++) {
				Type type = objectArgs[i].GetType();
				//int型の場合
				if (type == typeof(int)) {
					types.Add((int)SupportType.TypeInt);
					binaryList.AddRange(BitConverter.GetBytes((int)objectArgs[i]));
				//float型の場合
				} else if (type == typeof(float)) {
					types.Add((int)SupportType.TypeFloat);
					binaryList.AddRange(BitConverter.GetBytes((float)objectArgs[i]));
				//bool型の場合
				} else if (type == typeof(bool)) {
					types.Add((int)SupportType.TypeBool);
					binaryList.AddRange(BitConverter.GetBytes((bool)objectArgs[i]));
				//long型の場合
				} else if (type == typeof(long)) {
					types.Add((int)SupportType.TypeLong);
					binaryList.AddRange(BitConverter.GetBytes((long)objectArgs[i]));
				//short型の場合
				} else if (type == typeof(short)) {
					types.Add((int)SupportType.TypeShort);
					binaryList.AddRange(BitConverter.GetBytes((short)objectArgs[i]));
				//double型の場合
				} else if (type == typeof(double)) {
					types.Add((int)SupportType.TypeDouble);
					binaryList.AddRange(BitConverter.GetBytes((double)objectArgs[i]));
				//Vector3型の場合
				} else if (type == typeof(Vector3)) {
					types.Add((int)SupportType.TypeVector3);
					Vector3 vec = (Vector3)objectArgs[i];
					byte[] bin = new byte[sizeof(float) * 3];
					Array.Copy(BitConverter.GetBytes(vec.x), 0, bin, sizeof(float) * 0, sizeof(float));
					Array.Copy(BitConverter.GetBytes(vec.y), 0, bin, sizeof(float) * 1, sizeof(float));
					Array.Copy(BitConverter.GetBytes(vec.z), 0, bin, sizeof(float) * 2, sizeof(float));
					binaryList.AddRange(bin);
				//上記のどれでもない場合はサポート外
				} else {
					types.Add((int)SupportType.NotSupport);
				}
			}

			binary = binaryList.ToArray();
			//return SerializeArgs(objectArgs);
		}
		/// <summary>
		/// 引数を文字列化する
		/// </summary>
		private static void SerializeArgsString(object[] objectArgs, out string[] args) {
			args = new string[objectArgs.Length];
			for (int i = 0; i < objectArgs.Length; i++) {
				//@note 使用可能な型は1.Enum, 2.string, 3.Vector3型, 4.Parse関数を持っている型
				//引数の型が使用可能かチェック
				Debug.Assert(objectArgs[i].GetType().IsEnum || objectArgs[i].GetType() == typeof(string) || objectArgs[i].GetType() == typeof(Vector3) || objectArgs[i].GetType().GetMethod("Parse", new Type[] { typeof(string) }) != null, objectArgs[i].GetType().ToString() + " is not support");
				if (objectArgs[i].GetType() == typeof(Vector3)) {
					Vector3 vec = (Vector3)objectArgs[i];
					args[i] = objectArgs[i].GetType().ToString() + ":" + vec.x.ToString() + "," + vec.y.ToString() + "," + vec.z.ToString();
				} else {
					args[i] = objectArgs[i].GetType().AssemblyQualifiedName + ":" + objectArgs[i].ToString();
				}
			}
		}
		/// <summary>
		/// 引数をプログラムで使える状態に変換
		/// </summary>
		public static object[] DeserializeArgs(string[] argsString, List<int> types, byte[] argsBinary) {
			object[] args;
			if (types.Count == 0) {
				//Debug.Log("Deserialize use string args");
				args = DeserializeArgs(argsString);
			} else {
				//Debug.Log("Deserialize use binary args");
				args = new object[types.Count];
				DeserializeArgsBinary(types, argsBinary, ref args);
				DeserializeArgsString(types, argsString, ref args);
			}
			return args;
		}
		/// <summary>
		/// 引数をプログラムで使える状態に変換(バイナリデータを処理
		/// </summary>
		private static void DeserializeArgsBinary(List<int> types, byte[] argsBinary, ref object[] args) {
			if (argsBinary == null)
				return;
			int seeker = 0;
			//object[] args = new object[max];
			for (int i = 0, max = types.Count; i < max; i++) {
				//int型の場合
				if (types[i] == (int)SupportType.TypeInt) {
					args[i] = BitConverter.ToInt32(argsBinary, seeker);
					seeker += sizeof(int);
				//float型の場合
				} else if (types[i] == (int)SupportType.TypeFloat) {
					args[i] = BitConverter.ToSingle(argsBinary, seeker);
					seeker += sizeof(float);
				//bool型の場合
				} else if (types[i] == (int)SupportType.TypeBool) {
					args[i] = BitConverter.ToBoolean(argsBinary, seeker);
					seeker += sizeof(bool);
				//long型の場合
				} else if (types[i] == (int)SupportType.TypeLong) {
					args[i] = BitConverter.ToInt64(argsBinary, seeker);
					seeker += sizeof(long);
				//short型の場合
				} else if (types[i] == (int)SupportType.TypeShort) {
					args[i] = BitConverter.ToInt16(argsBinary, seeker);
					seeker += sizeof(short);
				//double型の場合
				} else if (types[i] == (int)SupportType.TypeDouble) {
					args[i] = BitConverter.ToDouble(argsBinary, seeker);
					seeker += sizeof(double);
				//Vector3型の場合
				} else if (types[i] == (int)SupportType.TypeVector3) {
					Vector3 vec = new Vector3();
					vec.x = BitConverter.ToSingle(argsBinary, seeker);
					seeker += sizeof(float);
					vec.y = BitConverter.ToSingle(argsBinary, seeker);
					seeker += sizeof(float);
					vec.z = BitConverter.ToSingle(argsBinary, seeker);
					seeker += sizeof(float);
					args[i] = vec;
				//上記のどれでもない場合はサポート外
				} else {
				}
			}

			//return args;
		}
		/// <summary>
		/// 引数をプログラムで使える状態に変換(バイナリサポートしてない型を処理
		/// </summary>
		private static void DeserializeArgsString(List<int> types, string[] argsString, ref object[] args) {
			if (argsString == null)
				return;
			//object[] objectArgs = new object[argsString.Length];
			for (int i = 0, max = argsString.Length; i < max; i++) {
				//バイナリサポート型はそちらでロードしているはずなのでここでは無視
				if (types[i] != (int)SupportType.NotSupport)
					continue;

				string[] arg = argsString[i].Split(':');
				//UnityのクラスとかはType.GetTypeでnullが返ってくるので紛れ込んでいたらエラー出す
				Type t = Type.GetType(arg[0]);
				Debug.Assert(t != null, "argment type error: " + arg[0]);

				//文字列型の場合
				if (t == typeof(string)) {
					args[i] = arg[1];
				//@note enum型はintキャストして値だけ保存すると新しい列挙子が途中に追加される、順番が入れ替えられるなどしたときに対応できなくなるので文字列で妥協
				//型がenumだった場合
				} else if (t.IsEnum) {
					//enum用のParseをかける
					args[i] = Enum.Parse(t, arg[1]);
				//それ以外は"Parse"関数があることを期待してParseを呼ぶ。無いとエラーがthrowされるので注意
				} else {
					args[i] = t.InvokeMember("Parse", System.Reflection.BindingFlags.InvokeMethod, null, null, new object[] { arg[1] });
				}
			}
		}	
	}
}
