using System;
using System.Collections.Generic;

namespace UnityChess {
	public class Timeline<T> {
		public T Current => list[headIndex];
		public int Span => list.Count;
		public bool IsUpToDate => headIndex == list.Count - 1;
		public int HeadIndex {
			get => headIndex;
			set => headIndex = Math.Min(value, list.Count - 1);
		}

		private int headIndex;
		private readonly List<T> list;
		private int FutureElementsStartIndex => headIndex + 1;
		private int NumFutureElements => list.Count - FutureElementsStartIndex;

		public Timeline() {
			headIndex = -1;
			list = new List<T>();
		}
		
		public List<T> GetStartToCurrent() => list.GetRange(0, headIndex + 1);

		public List<T> PopFuture() {
			List<T> elementRange = list.GetRange(FutureElementsStartIndex, NumFutureElements);
			Prune();
			return elementRange;
		}

		public void AddNext(T element) {
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