using UnityChess.Core.Util;
using UnityEngine;

namespace UnityChess.Presentation.View {
	[CreateAssetMenu(fileName = "PieceSetDefinition", menuName = "UnityChess/Piece Set")]
	public class PieceSetDefinition : ScriptableObject {
		public string key => name;
		[field: SerializeField] public string displayName { get; private set; }
		[field: SerializeField] public Sprite previewIcon { get; private set; }

		[SerializeField, Header("Piece Sprites")] private Sprite _whitePawn;
    	[SerializeField] private Sprite _whiteKnight;
    	[SerializeField] private Sprite _whiteBishop;
    	[SerializeField] private Sprite _whiteRook;
    	[SerializeField] private Sprite _whiteQueen;
    	[SerializeField] private Sprite _whiteKing;
    	[SerializeField] private Sprite _blackPawn;
    	[SerializeField] private Sprite _blackKnight;
    	[SerializeField] private Sprite _blackBishop;
    	[SerializeField] private Sprite _blackRook;
    	[SerializeField] private Sprite _blackQueen;
    	[SerializeField] private Sprite _blackKing;

    	public Sprite GetSprite(PieceType pieceType, Side side) {
    		return side switch {
    			Side.White => pieceType switch {
				    PieceType.Pawn => _whitePawn,
				    PieceType.Knight => _whiteKnight,
				    PieceType.Bishop => _whiteBishop,
				    PieceType.Rook => _whiteRook,
				    PieceType.Queen => _whiteQueen,
				    PieceType.King => _whiteKing,
				    _ => null
			    },
    			Side.Black => pieceType switch {
				    PieceType.Pawn => _blackPawn,
				    PieceType.Knight => _blackKnight,
				    PieceType.Bishop => _blackBishop,
				    PieceType.Rook => _blackRook,
				    PieceType.Queen => _blackQueen,
				    PieceType.King => _blackKing,
				    _ => null
			    },
    			_ => null
    		};
    	}
	}
}