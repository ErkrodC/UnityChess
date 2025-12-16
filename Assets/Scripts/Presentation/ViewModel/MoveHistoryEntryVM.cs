namespace UnityChess.Presentation.ViewModel {
	public class MoveHistoryEntryVM {
		public int moveNumber;
		public string whiteMoveString;
		public string blackMoveString;

		public int whiteHalfMoveIndex => _moveIndex * 2;
		public int blackHalfMoveIndex => _moveIndex * 2 + 1;

		private int _moveIndex => moveNumber - 1;
	}
}