using System;
using System.Collections.Generic;

namespace UnityChess {
	public class History<T> {
		public T Last => list[headIndex];
		
		public int HeadIndex {
			get => headIndex;
			set => headIndex = Math.Min(value, list.Count - 1);
		}
		
		private int headIndex;
		private readonly List<T> list;
		
		public History() {
			list = new List<T>();
			HeadIndex = -1;
		}

		public void AddLast(T element) {
			if (headIndex < list.Count - 1) list.RemoveRange(headIndex + 1, list.Count - 1 - headIndex);
			list.Add(element);
			headIndex++;
		}
	}
}