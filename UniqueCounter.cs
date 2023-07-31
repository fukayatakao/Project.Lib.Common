using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Project.Lib {
	public class UniqueCounter {
		public int Id { get; private set; }
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public UniqueCounter() {
			Id = 0;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public UniqueCounter(int id) {
			Id = id;
		}

		/// <summary>
		/// ユニークなIDを取得
		/// </summary>
		public int GetUniqueId() {
			Id++;
			return Id;
		}

	}
}
