using System.Collections.Generic;
using System.Threading.Tasks;
using UnityChess;
using UnityEngine;

public class GameManager : MonoBehaviourSingleton<GameManager> {
	public Game Game;
	public Queue<Movement> MoveQueue;
	[SerializeField] private GameEvent NewGameStartedEvent = null;
	[SerializeField] private GameEvent GameEndedEvent = null;
	[SerializeField] private GameEvent GameResetToTurnEvent = null;
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
	private IGameExporter exporter;
	private GameConditions startingGameConditions;
	
	public Board CurrentBoard => Game.BoardHistory.Last;
	public History<HalfMove> PreviousMoves => Game.PreviousMoves;
	public HalfMove LatestHalfMove => PreviousMoves.Last;
	
	public void Start() {
		MoveQueue = new Queue<Movement>();
		exporter = new FENExporter();
#if GAME_TEST
		StartNewGame(Mode.HumanVsHuman);
#endif
		
#if DEBUG_VIEW
		unityChessDebug.enabled = true;
#endif
	}

	public void StartNewGame(int mode) => StartNewGame((Mode) mode);
	public void StartNewGame(Mode mode) {
		Game = new Game(mode);
		startingGameConditions = GameConditions.NormalStartingConditions;
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

	public void ResetGameToTurn(int turnIndex) {
		Game.ResetGameToTurn(turnIndex);
		GameResetToTurnEvent.Raise();
	}
	
	public string Export() => exporter.Export(CurrentBoard, PreviousMoves.GetCurrentBranch(), GameExporterUtil.GetEndingGameConditions(startingGameConditions, CurrentBoard, PreviousMoves.GetCurrentBranch()));

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
				promotionMove.AssociatedPiece = PromotionUtil.GeneratePromotionPiece(choice, promotionMove.End, Game.CurrentTurnSide);
				BoardManager.Instance.DestroyPieceAtPosition(promotionMove.End);
				BoardManager.Instance.CreateAndPlacePieceGO(promotionMove.AssociatedPiece);
				
				UIManager.Instance.DeactivatePromotionUI();
				BoardManager.Instance.SetActiveAllPieces(true);
				break;
		}
		
		ExecuteTurn(specialMove);
	}

	private void ExecuteTurn(Movement move) {
		Game.ExecuteTurn(move);

		HalfMove latestHalfMove = LatestHalfMove;
		if (latestHalfMove.CausedCheckmate || latestHalfMove.CausedStalemate) {
			BoardManager.Instance.SetActiveAllPieces(false);
			GameEndedEvent.Raise();
		} else {
			BoardManager.Instance.EnsureOnlyPiecesOfSideAreEnabled(Game.CurrentTurnSide);
		}
	}

}