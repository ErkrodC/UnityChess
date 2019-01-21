using System;
using System.Collections.Generic;

namespace UnityChess {
	public class History<T> {
		public T Last => list[headIndex];
		public int Count => list.Count;
		
		public int HeadIndex {
			get => headIndex;
			set => headIndex = Math.Min(value, list.Count - 1);
		}
		
		private int headIndex;
		private readonly List<T> list;
		
		public History() {
			headIndex = -1;
			list = new List<T>();
		}

		public void AddLast(T element) {
			if (headIndex < list.Count - 1) list.RemoveRange(headIndex + 1, list.Count - 1 - headIndex);
			list.Add(element);
			headIndex++;
		}

		public void Clear() {
			list.Clear();
			headIndex = -1;
		}

		public List<T> PopRange(int index, int count) {
			List<T> elementRange = list.GetRange(index, count);
			list.RemoveRange(index, count);
			return elementRange;
		}
	}
}