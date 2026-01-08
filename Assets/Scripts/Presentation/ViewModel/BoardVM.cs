using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityChess.Core;
using UnityChess.Core.Util;
using UnityChess.DependencyInjection;
using UnityChess.Presentation.View;

namespace UnityChess.Presentation.ViewModel {
	public class BoardVM : IViewModel {
		public event Action<PieceSetDefinition> pieceSetChanged;

		public readonly PieceVM[,] currentBoard = CreateEmptyPieceVMArray();
		public Square? selectedSquare;
		public List<Square> highlightedSquares = new();
		public Side currentSideToMove;
		public float whiteTimeRemaining;
		public float blackTimeRemaining;

		public PieceSetDefinition activePieceSet {
			get => _activePieceSet;
			set {
				_activePieceSet = value;
				pieceSetChanged?.Invoke(value);
			}
		} private PieceSetDefinition _activePieceSet;

		public Func<string, string, Task<bool>> onPieceDropped;

		private static PieceVM[,] CreateEmptyPieceVMArray() {
			PieceVM[,] result = new PieceVM[8, 8];

			for (int file = 0; file < 8; file++)
			for (int rank = 0; rank < 8; rank++) {
				result[file, rank] = new PieceVM();
			}

			return result;
		}
	}
}