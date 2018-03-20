using System;
using UnityEditor;
using UnityEngine;

namespace UnityChess {

	public static class PieceFactory {

		public static Piece NewPiece(PieceType type, Square startingPosition, Side side) {
			SerializedObject serializedObject = new SerializedObject(type);

			switch (serializedObject.FindProperty("m_name").ToString()) {
				case "Bishop":
					return new Bishop(startingPosition, side);
				case "King":
					return new King(startingPosition, side);
				case "Knight":
					return new Knight(startingPosition, side);
				case "Pawn":
					return new Pawn(startingPosition, side);
				case "Queen":
					return new Queen(startingPosition, side);
				case "Rook":
					return new Rook(startingPosition, side);
				default:
					throw new NullReferenceException("PieceType not set. Be sure to assign a piece type to PieceBeh component on piece gameObject's.");
			}
		}
	}
}