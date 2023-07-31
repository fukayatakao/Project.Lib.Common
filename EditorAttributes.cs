using UnityEditor;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Project.Lib {
	//この属性はUnityEditorのときのみ有効。アプリには含まれない。

	/// <summary>
	/// 関数の説明情報を持たせる
	/// </summary>
	[System.Diagnostics.Conditional( "UNITY_EDITOR" )]
	[AttributeUsage (AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public class FunctionAttribute : Attribute
	{
		private string text_;
		public string Text { get { return text_; } }

        public FunctionAttribute(string text){
			text_ = text;
        }

	}

	/// <summary>
	/// 関数の肯定説明情報を持たせる
	/// </summary>
	/// <remarks>
	/// 真偽判定関数の説明を付ける
	/// </remarks>
	[System.Diagnostics.Conditional("UNITY_EDITOR")]
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public class AffirmativeAttribute : Attribute {
		private string text_;
		public string Text { get { return text_; } }

		public AffirmativeAttribute(string text) {
			text_ = text;
		}
	}
	/// <summary>
	/// 真偽判定関数の説明情報を持たせる
	/// </summary>
	[System.Diagnostics.Conditional("UNITY_EDITOR")]
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public class CheckFunctionAttribute : Attribute {
		//肯定説明
		private string affiermative_;
		public string Affiermative { get { return affiermative_; } }
		//否定説明
		private string negative_;
		public string Negative { get { return negative_; } }

		public CheckFunctionAttribute(string affiermative, string negative) {
			affiermative_ = affiermative + "[真]";
			negative_ = negative + "[偽]";
		}
	}

	/// <summary>
	/// 関数の引数説明情報を持たせる
	/// </summary>
	[System.Diagnostics.Conditional( "UNITY_EDITOR" )]
	[AttributeUsage (AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
	public class ArgAttribute : Attribute
	{
		//何番目の引数か
		private int index_;
		public int Index { get { return index_; } }
		//引数の型
		private Type type_;
		public Type Type { get { return type_; } }
		//引数の内容
		private string text_;
		public string Text { get { return text_; } }

		//デフォルトの値
		private object value_;
		public object Value { get { return value_; } }
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ArgAttribute (int index, Type type, string text, object value)
		{
			index_ = index;
			type_ = type;
			text_ = text;
			//Vector3型の場合はカンマ区切りの文字列で渡す
			if (type == typeof(Vector3)) {
				//型チェック
				Debug.Assert (value.GetType() == typeof(string), "value type unmatch " + text + ": arg = " + type_.Name + ", value = " + value.GetType ().Name);
				string[] v = ((string)value).Split (',');
				value_ = new Vector3(float.Parse(v[0]), float.Parse(v[1]), float.Parse(v[2]));
			} else {
				//引数の型とデフォルト値の型が一致しないときはエラー出しておく
				Debug.Assert (type_ == value.GetType (), "value type unmatch"  + text + ": arg = " + type_.Name + ", value = " + value.GetType ().Name);
				value_ = value;
			}
		}
	}

	/// <summary>
	/// 列挙子の説明を持たせる
	/// </summary>
	[System.Diagnostics.Conditional( "DEVELOP_BUILD" )]
	[AttributeUsage(AttributeTargets.Enum|AttributeTargets.Field, AllowMultiple = false)]
	public class FieldAttribute : Attribute
	{
		/// <summary>
		/// EnumのFieldをすべて取得
		/// </summary>
		public static string[] GetFields(System.Type enumType) {
			//Enum型でない場合は無視
			if (!enumType.IsEnum) {
				return null;
			}

			List<string> enumFields = new List<string>();
			foreach (var value in System.Enum.GetValues(enumType)) {
				enumFields.Add(FieldAttribute.GetField(value));
			}

			return enumFields.ToArray();
		}


		/// <summary>
		/// フィールドの文字列を取得
		/// </summary>
		public static string GetField(object value){
			FieldAttribute attr = System.Attribute.GetCustomAttribute(value.GetType().GetField(value.ToString()), typeof(FieldAttribute)) as FieldAttribute;
			if (attr == null)
				return value.ToString();

			return attr.field_;
		}

		//設定した文字列
		private string field_;
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public FieldAttribute(string text) { 
			field_ = text; 
		}
	}
	/// <summary>
	/// 曲線データの情報
	/// </summary>
	[System.Diagnostics.Conditional("UNITY_EDITOR")]
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public class CurveAttribute : Attribute {
		//曲線データに使う引数の番号
		private int[] indexArray_;
		public int[] IndexArray { get { return indexArray_; } }
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public CurveAttribute(int[] index) {
			indexArray_ = index;
		}
	}
	/// <summary>
	/// リソース情報
	/// </summary>
	[System.Diagnostics.Conditional("UNITY_EDITOR")]
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
	public class ResourceAttribute : Attribute {
		//引数の番号
		private int index_;
		public int Index { get { return index_; } }
		//パス解決タイプ
		private int path_;
		public int Path { get { return path_; } }
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ResourceAttribute(int path, int index) {
			index_ = index;
			path_ = path;
		}

	}


    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public class ReadOnlyAttribute : PropertyAttribute {

    }
#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyDrawer : PropertyDrawer {
        public override float GetPropertyHeight(SerializedProperty property,
                                                GUIContent label) {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        public override void OnGUI(Rect position,
                                   SerializedProperty property,
                                   GUIContent label) {
            GUI.enabled = false;
            EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = true;
        }
    }
#endif
}
