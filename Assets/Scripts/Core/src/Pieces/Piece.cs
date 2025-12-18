using System.Collections.Generic;
using UnityChess.Util;

namespace UnityChess.Core {
	/// <summary>Base class for any chess piece.</summary>
	public abstract class Piece {
		public Side Owner { get; protected set; }

		protected Piece(Side owner) {
			Owner = owner;
		}

		public abstract Piece DeepCopy();

		public abstract Dictionary<(Square, Square), Movement> CalculateLegalMoves(
			Board board,
			GameConditions gameConditions,
			Square position
		);

		public override string ToString() => $"{Owner} {GetType().Name}";

		public abstract string ToTextArt();
	}

	public abstract class Piece<T> : Piece where T : Piece<T>, new() {
		protected Piece(Side owner) : base(owner) { }

		public override Piece DeepCopy() {
			return new T {
				Owner = Owner
			};
		}
	}
}