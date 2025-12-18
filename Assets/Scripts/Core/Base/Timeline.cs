using System;
using System.Collections;
using System.Collections.Generic;

namespace UnityChess.Core {
	public class Timeline<T> : IList<T> {
		public T Head {
			get {
				if (0 <= headIndexBacking && headIndexBacking < list.Count) {
					return list[headIndexBacking];
				}

				return default;
			}
		}

		public int HeadIndex {
			get => headIndexBacking;
			set => headIndexBacking = Math.Min(value, list.Count - 1);
		} private int headIndexBacking;

		public int Count => list.Count;
		public bool IsReadOnly => false;
		public bool IsUpToDate => headIndexBacking == list.Count - 1;

		private readonly List<T> list;
		private int FutureElementsStartIndex => headIndexBacking + 1;
		private int NumFutureElements => list.Count - FutureElementsStartIndex;

		public Timeline() {
			headIndexBacking = -1;
			list = new List<T>();
		}

		public void Add(T element) => AddNext(element);

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
			if (!IsUpToDate) {
				list.RemoveRange(FutureElementsStartIndex, NumFutureElements);
			}
		}

		public void Clear() {
			list.Clear();
			headIndexBacking = -1;
		}

		public bool Contains(T item) {
			return list.Contains(item);
		}

		public void CopyTo(T[] array, int arrayIndex) {
			list.CopyTo(array, arrayIndex);
		}

		public bool Remove(T item) {
			return list.Remove(item);
		}

		public IEnumerator<T> GetEnumerator() {
			return list.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}

		public int IndexOf(T item) {
			return list.IndexOf(item);
		}

		public void Insert(int index, T item) {
			list.Insert(index, item);
		}

		public void RemoveAt(int index) {
			list.RemoveAt(index);
		}

		public T this[int index] {
			get => list[index];
			set => list[index] = value;
		}
	}
}