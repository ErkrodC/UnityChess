using System;
using System.Collections.Generic;

namespace UnityChess {
	public class History<T> {
		public T Last => list[headIndex];
		public bool IsUpToDate => headIndex == list.Count - 1;
		public int HeadIndex {
			get => headIndex;
			set => headIndex = Math.Min(value, list.Count - 1);
		}

		private int FutureElementsStartIndex => headIndex + 1;
		private int NumFutureElements => list.Count - FutureElementsStartIndex;
		
		private int headIndex;
		private readonly List<T> list;
		
		public History() {
			headIndex = -1;
			list = new List<T>();
		}
		
		public List<T> GetCurrent() => list.GetRange(0, headIndex + 1);

		public List<T> PopFuture() {
			List<T> elementRange = list.GetRange(FutureElementsStartIndex, NumFutureElements);
			list.RemoveRange(FutureElementsStartIndex, NumFutureElements);
			return elementRange;
		}

		public void AddLast(T element) {
			Prune();
			list.Add(element);
			headIndex++;
		}

		private void Prune() {
			if (IsUpToDate) return;
			list.RemoveRange(FutureElementsStartIndex, NumFutureElements);
		}
		
		public void Clear() {
			list.Clear();
			headIndex = -1;
		}
	}
}