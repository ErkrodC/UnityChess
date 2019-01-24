using System.Collections.Generic;
using System.Threading.Tasks;
using UnityChess;
using UnityEngine;

public class GameManager : MonoBehaviourSingleton<GameManager> {
	public Board CurrentBoard => game.BoardTimeline.Current;
	public Side CurrentTurnSide => game.CurrentTurnSide;
	public Timeline<HalfMove> PreviousMoves => game.PreviousMoves;
	public int HalfMoveCount => game.HalfMoveCount;
	
	public Queue<Movement> MoveQueue { get; private set; }
	
	[SerializeField] private GameEvent NewGameStartedEvent = null;
	[SerializeField] private GameEvent GameEndedEvent = null;
	[SerializeField] private GameEvent GameResetToHalfMoveEvent = null;
	[SerializeField] private GameEvent MoveExecutedEvent = null;
	[SerializeField] private UnityChessDebug unityChessDebug = null;
	public List<Piece> CurrentPieces {
		get {
			currentPiecesBacking.Clear();
			for (int file = 1; file <= 8; file++)
				for (int rank = 1; rank <= 8; rank++) {
					Piece piece = CurrentBoard[file, rank];
					if (piece != null) currentPiecesBacking.Add(piece);
				}

			return currentPiecesBacking;
		}
	}
	private readonly List<Piece> currentPiecesBacking = new List<Piece>();
	private FENInterchanger fenInterchanger;
	private PGNInterchanger pgnInterchanger;
	private Game game;
	
	public void Start() {
		MoveQueue = new Queue<Movement>();
		fenInterchanger = new FENInterchanger();
		pgnInterchanger = new PGNInterchanger();
#if GAME_TEST
		StartNewGame(Mode.HumanVsHuman);
#endif
		
#if DEBUG_VIEW
		unityChessDebug.enabled = true;
#endif
	}

	public void StartNewGame(int mode) => StartNewGame((Mode) mode);
	public void StartNewGame(Mode mode) {
		game = new Game(mode, GameConditions.NormalStartingConditions);
		NewGameStartedEvent.Raise();
	}

	public void OnPieceMoved() {
		Movement move = MoveQueue.Dequeue();

		if (move is SpecialMove specialMove) {
			HandleSpecialMoveExecution(specialMove);
		} else {
			ExecuteTurn(move);
		}
	}

	public void ResetGameToHalfMoveIndex(int halfMoveIndex) {
		game.ResetGameToHalfMoveIndex(halfMoveIndex);
		GameResetToHalfMoveEvent.Raise();
	}
	
	public string ExportToFEN() => fenInterchanger.Export(game);
	public string ExportToPGN() => pgnInterchanger.Export(game);

	private async void HandleSpecialMoveExecution(SpecialMove specialMove) {
		switch (specialMove) {
			case CastlingMove castlingMove:
				BoardManager.Instance.CastleRook(castlingMove.AssociatedPiece.Position);
				break;
			case EnPassantMove enPassantMove:
				BoardManager.Instance.DestroyPieceAtPosition(enPassantMove.AssociatedPiece.Position);
				break;
			case PromotionMove promotionMove:
				UIManager.Instance.ActivatePromotionUI();
				BoardManager.Instance.SetActiveAllPieces(false);
				
				Task<ElectedPiece> getUserChoiceTask = new Task<ElectedPiece>(UIManager.Instance.GetUserPromotionPieceChoice);
				getUserChoiceTask.Start();
				ElectedPiece choice = await getUserChoiceTask;
				promotionMove.AssociatedPiece = PromotionUtil.GeneratePromotionPiece(choice, promotionMove.End, game.CurrentTurnSide);
				BoardManager.Instance.DestroyPieceAtPosition(promotionMove.End);
				BoardManager.Instance.CreateAndPlacePieceGO(promotionMove.AssociatedPiece);
				
				UIManager.Instance.DeactivatePromotionUI();
				BoardManager.Instance.SetActiveAllPieces(true);
				break;
		}
		
		ExecuteTurn(specialMove);
	}

	private void ExecuteTurn(Movement move) {
		game.ExecuteTurn(move);

		HalfMove latestHalfMove = PreviousMoves.Current;
		if (latestHalfMove.CausedCheckmate || latestHalfMove.CausedStalemate) {
			BoardManager.Instance.SetActiveAllPieces(false);
			GameEndedEvent.Raise();
		} else {
			BoardManager.Instance.EnsureOnlyPiecesOfSideAreEnabled(game.CurrentTurnSide);
		}

		MoveExecutedEvent.Raise();
	}

	public bool MoveIsLegal(Movement baseMove, out Movement foundLegalMove) => game.MoveIsLegal(baseMove, out foundLegalMove);
}