namespace UnityChess {
	public static class BitboardUtil {
		public const ulong WhiteToMoveMask = 0x0000_0000_0000_0001;
		public const ulong HalfMoveClockMask = 0x0000_0000_0000_00FE;
		public const ulong WhiteEnPassantMask = Rank2Mask;
		public const ulong BlackEnPassantMask = Rank3Mask;
		public const ulong WhiteKingsideCastleMask = 0x0000_0000_0100_0000;
		public const ulong WhiteQueensideCastleMask = 0x0000_0000_0200_0000;
		public const ulong BlackKingsideCastleMask = 0x0000_0000_0400_0000;
		public const ulong BlackQueensideCastleMask = 0x0000_0000_0800_0000;

		public const ulong Rank1Mask = 0x0000_0000_0000_00FF;
		public const ulong Rank2Mask = 0x0000_0000_0000_FF00;
		public const ulong Rank3Mask = 0x0000_0000_00FF_0000;
		public const ulong Rank4Mask = 0x0000_0000_FF00_0000;
		public const ulong Rank5Mask = 0x0000_00FF_0000_0000;
		public const ulong Rank6Mask = 0x0000_FF00_0000_0000;
		public const ulong Rank7Mask = 0x00FF_0000_0000_0000;
		public const ulong Rank8Mask = 0xFF00_0000_0000_0000;

		public const ulong FileAMask = 0x0101_0101_0101_0101;
		public const ulong FileBMask = 0x0202_0202_0202_0202;
		public const ulong FileCMask = 0x0404_0404_0404_0404;
		public const ulong FileDMask = 0x0808_0808_0808_0808;
		public const ulong FileEMask = 0x1010_1010_1010_1010;
		public const ulong FileFMask = 0x2020_2020_2020_2020;
		public const ulong FileGMask = 0x4040_4040_4040_4040;
		public const ulong FileHMask = 0x8080_8080_8080_8080;

		public static readonly ulong[] RankMasks = {
			Rank1Mask,
			Rank2Mask,
			Rank3Mask,
			Rank4Mask,
			Rank5Mask,
			Rank6Mask,
			Rank7Mask,
			Rank8Mask
		};

		public static readonly ulong[] FileMasks = {
			FileAMask,
			FileBMask,
			FileCMask,
			FileDMask,
			FileEMask,
			FileFMask,
			FileGMask,
			FileHMask
		};
	}
}