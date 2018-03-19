﻿namespace UnityChess {
    /// <summary>
    ///     Used to describe which side's turn it currently is.
    /// </summary>
    public enum Side {
		Black,
		White
	}

	public static class SideMethods {
		public static Side Complement(this Side side) {
			return side == Side.White ? Side.Black : Side.White;
		}
	}
}