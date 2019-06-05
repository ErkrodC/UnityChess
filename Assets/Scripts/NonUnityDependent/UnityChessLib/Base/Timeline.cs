using System;
using System.Collections.Generic;

namespace UnityChess {
	public class Timeline<T> {
		public T Current => list[headIndexBacking];
		public int Span => list.Count;
		public bool IsUpToDate => headIndexBacking == list.Count - 1;
		public int HeadIndex {
			get => headIndexBacking;
			set => headIndexBacking = Math.Min(value, list.Count - 1);
		} private int headIndexBacking;
		
		private readonly List<T> list;
		private int FutureElementsStartIndex => headIndexBacking + 1;
		private int NumFutureElements => list.Count - FutureElementsStartIndex;

		public Timeline() {
			headIndexBacking = -1;
			list = new List<T>();
		}
		
		public List<T> GetStartToCurrent() => list.GetRange(0, headIndexBacking + 1);

		public List<T> PopFuture() {
			List<T> elementRange = list.GetRange(FutureElementsStartIndex, NumFutureElements);
			Prune();
			return elementRange;
		}

		public void AddNext(T element) {
			Prune();
			list.Add(element);
			headIndexBacking++;
		}

		private void Prune() {
			if (!IsUpToDate) list.RemoveRange(FutureElementsStartIndex, NumFutureElements);
		}
		
		public void Clear() {
			list.Clear();
			headIndexBacking = -1;
		}
	}
}