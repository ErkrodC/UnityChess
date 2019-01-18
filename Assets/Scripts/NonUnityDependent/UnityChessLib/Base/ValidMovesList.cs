using System;
using System.Collections;
using System.Collections.Generic;

namespace UnityChess {
	public class ValidMovesList : IEnumerable<Movement> {
		public int Count => list.Count;
		
		private readonly List<Movement> list;

		public ValidMovesList() => list = new List<Movement>();
		private ValidMovesList(List<Movement> list) => this.list = list;

		public void Add(Movement move) => list.Add(move);
		public void Clear() => list.Clear();
		public ValidMovesList DeepCopy() => new ValidMovesList(list.ConvertAll(move => new Movement(move)));

		internal bool FindValidMoveUsingBaseMove(Movement baseMove, out Movement actualMove) {
			foreach (Movement validMove in list) {
				if (validMove.Start == baseMove.Start && validMove.End == baseMove.End) {
					actualMove = validMove;
					return true;
				}
			}

			actualMove = null;
			return false;
		}
		
		public Movement this[int i] {
			get => list[i];
			set => list[i] = value;
		}
		
		public IEnumerator<Movement> GetEnumerator() => list.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
