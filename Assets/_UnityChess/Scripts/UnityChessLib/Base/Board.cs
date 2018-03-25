using System.Collections.Generic;
using System.Linq;

namespace UnityChess {
	/// <summary>
	///     A 120-length, 1-D representation of a chessboard.
	/// </summary>
	public class Board {
		public static EmptyPiece EmptyPiece = new EmptyPiece();
		public static InvalidPiece InvalidPiece = new InvalidPiece();

		/// <summary>
		///     Creates a Board with initial chess game position.
		/// </summary>
		public Board() {
			BasePieceList = Enumerable.Range(0, 120).Select(i => (BasePiece) null).ToList();
			SetStartingPosition();
		}

		/// <summary>
		///     Creates a deep copy of the passed Board.
		/// </summary>
		public Board(Board board) {
			BasePieceList = Enumerable.Range(0, 120).Select(i => (BasePiece) null).ToList();
			SetBlankBoard();

			//creates deep copy (makes copy of each piece and deep copy of their respective ValidMoves lists) of board (list of BasePiece's)
			//this may be a memory hog since each Board has a list of Piece's, and each piece has a list of Movement's
			//avg number turns/Board's per game should be around ~80. usual max number of pieces per board is 32
			// TODO optimize this method
			foreach (Piece piece in board.BasePieceList.OfType<Piece>()) {
				BasePieceList[piece.Position.AsIndex()] = piece.Clone();
			}

			InitKings();
		}

		public King WhiteKing { get; set; }
		public King BlackKing { get; set; }

		/*
		Boards will be represented as a 10x12 grid in a one dimensional list.
		The 8x8 chessboard is in the center, viewed from above with white on the side closer to index 0.

			 110  111 112 ... 117 118  119                                  Compass:             (0, 1)
			 100  101 102 ... 107 108  109                                                        Black
			    |---------------------|               ^                    (-1, 1) Black-Queenside  |   Black-Kingside (1, 1)
			   .|              .   98 | 99          R |                                          \  |  /
			   .|            .        | .           a |               (-1,0) Queenside--------------*------------------Kingside (1, 0)
			   .|          .          | .           n |                                          /  |  \
			  30| 31 32 33 ...        | 39          k |                  (-1, -1) White-Queenside   |   White-Kingside (1, -1)
			  20| 21 22 23 ...        | 29            ------------>                               White
			    |---------------------|         (1,1)       file                                 (0, -1)
			  10  11 12 13  ... 17 18   19
			   0   1  2  3  ...  7  8   9
		*/
		public List<BasePiece> BasePieceList { get; private set; }

		/// <summary>
		///     Used to remove all pieces from the board.
		/// </summary>
		public void SetBlankBoard() {
			for (int i = 1; i <= 8; i++) {
				for (int j = 1; j <= 8; j++) {
					BasePieceList[Square.FileRankAsIndex(i, j)] = EmptyPiece;
				}
			}

			for (int i = 0; i <= 19; i++) {
				BasePieceList[i] = InvalidPiece;
				BasePieceList[i + 100] = InvalidPiece;
			}

			for (int i = 20; i <= 90; i += 10) {
				BasePieceList[i] = InvalidPiece;
				BasePieceList[i + 9] = InvalidPiece;
			}

			WhiteKing = null;
			BlackKing = null;
		}

		/// <summary>
		///     Used to reset the Board to initial chess game position.
		/// </summary>
		public void SetStartingPosition() {
			SetBlankBoard();

			//Row 2/Rank 7 and Row 7/Rank 2, both rows of pawns
			for (int i = 31; i <= 38; i++) {
				BasePieceList[i] = new Pawn(new Square(i), Side.White);
				BasePieceList[i + 50] = new Pawn(new Square(i + 50), Side.Black);
			}

			//Rows 1 & 8/Ranks 8 & 1, back rows for both players
			BasePieceList[21] = new Rook(new Square(21), Side.White);
			BasePieceList[22] = new Knight(new Square(22), Side.White);
			BasePieceList[23] = new Bishop(new Square(23), Side.White);
			BasePieceList[24] = new Queen(new Square(24), Side.White);
			BasePieceList[25] = new King(new Square(25), Side.White);
			BasePieceList[26] = new Bishop(new Square(26), Side.White);
			BasePieceList[27] = new Knight(new Square(27), Side.White);
			BasePieceList[28] = new Rook(new Square(28), Side.White);

			BasePieceList[91] = new Rook(new Square(91), Side.Black);
			BasePieceList[92] = new Knight(new Square(92), Side.Black);
			BasePieceList[93] = new Bishop(new Square(93), Side.Black);
			BasePieceList[94] = new Queen(new Square(94), Side.Black);
			BasePieceList[95] = new King(new Square(95), Side.Black);
			BasePieceList[96] = new Bishop(new Square(96), Side.Black);
			BasePieceList[97] = new Knight(new Square(97), Side.Black);
			BasePieceList[98] = new Rook(new Square(98), Side.Black);

			WhiteKing = BasePieceList[25] as King;
			BlackKing = BasePieceList[95] as King;
		}

		/// <summary>
		///     Used to execute a move.
		/// </summary>
		public void MovePiece(Movement move) {
			LiftPiece(move.Piece);
			PlacePiece(move.Piece, move.End);

			move.Piece.HasMoved = true;

			(move as SpecialMove)?.HandleAssociatedPiece(this);
		}

		public void PlacePiece(Piece piece) {
			BasePieceList[piece.Position.AsIndex()] = piece;
		}

		public void PlacePiece(Piece piece, Square position) {
			BasePieceList[position.AsIndex()] = piece;
			piece.Position.CopyPosition(position);
		}

		public void PlacePiece(Piece piece, int index) {
			Square s = new Square(index);

			BasePieceList[index] = piece;
			piece.Position.CopyPosition(s);
		}

		public void PlacePiece(Piece piece, int file, int rank) {
			PlacePiece(piece, Square.FileRankAsIndex(file, rank));
		}

		public void LiftPiece(Piece piece) {
			BasePieceList[piece.Position.AsIndex()] = EmptyPiece;
		}

		public BasePiece GetBasePiece(Square position) {
			return BasePieceList[position.AsIndex()];
		}

		public BasePiece GetBasePiece(int index) {
			return BasePieceList[index];
		}

		public BasePiece GetBasePiece(int file, int rank) {
			return GetBasePiece(Square.FileRankAsIndex(file, rank));
		}

		public Piece GetPiece(Square position) {
			return GetBasePiece(position) as Piece;
		}

		public Piece GetPiece(int index) {
			return GetBasePiece(index) as Piece;
		}

		public Piece GetPiece(int file, int rank) {
			return GetBasePiece(file, rank) as Piece;
		}

		public void InitKings() {
			WhiteKing = (King) BasePieceList.Single(bp => bp is King && (bp as King).Side == Side.White);
			BlackKing = (King) BasePieceList.Single(bp => bp is King && (bp as King).Side == Side.Black);
		}
	}
}