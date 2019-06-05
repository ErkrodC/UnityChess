#pragma warning disable 0649

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityChess;
using UnityEngine;

public class GameManager : MonoBehaviourSingleton<GameManager> {
	public Board CurrentBoard => game.BoardTimeline.Current;
	public Side CurrentTurnSide => game.CurrentTurnSide;
	public Timeline<HalfMove> HalfMoveTimeline => game.HalfMoveTimeline;
	public int HalfMoveCount => game.LatestHalfMoveIndex;
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
	} private readonly List<Piece> currentPiecesBacking = new List<Piece>();
	
	[SerializeField] private GameEvent NewGameStartedEvent;
	[SerializeField] private GameEvent GameEndedEvent;
	[SerializeField] private GameEvent GameResetToHalfMoveEvent;
	[SerializeField] private GameEvent MoveExecutedEvent;
	[SerializeField] private UnityChessDebug unityChessDebug;
	private Game game;
	private Queue<Movement> moveQueue;
	private FENInterchanger fenInterchanger;
	private PGNInterchanger pgnInterchanger;
	private CancellationTokenSource promotionUITaskCancellationTokenSource;

	public void Start() {
		moveQueue = new Queue<Movement>();
		fenInterchanger = new FENInterchanger();
		pgnInterchanger = new PGNInterchanger();
		promotionUITaskCancellationTokenSource = new CancellationTokenSource();
#if GAME_TEST
		StartNewGame(Mode.HumanVsHuman);
#endif
		
#if DEBUG_VIEW
		unityChessDebug.enabled = true;
#endif
	}
	
	public bool MoveIsLegal(Movement baseMove, out Movement foundLegalMove) => game.MoveIsLegal(baseMove, out foundLegalMove);
	public void EnqueueValidMove(Movement validMove) => moveQueue.Enqueue(validMove);
	public string ExportToFEN() => fenInterchanger.Export(game);
	public string ExportToPGN() => pgnInterchanger.Export(game);
	public void StartNewGame(int mode) => StartNewGame((Mode) mode); // NOTE Used for binding to UnityEvent response, probably a cleaner way...

	public void StartNewGame(Mode mode) {
		game = new Game(mode, GameConditions.NormalStartingConditions);
		NewGameStartedEvent.Raise();
	}

	public void ResetGameToHalfMoveIndex(int halfMoveIndex) {
		if (!game.ResetGameToHalfMoveIndex(halfMoveIndex)) return;
		
		UIManager.Instance.SetActivePromotionUI(false);
		promotionUITaskCancellationTokenSource.Cancel();
		GameResetToHalfMoveEvent.Raise();
	}
	
	public async void OnPieceMoved() => await ExecuteMove(moveQueue.Dequeue());

	private async Task ExecuteMove(Movement move) {
		if (move is SpecialMove specialMove && !await HandleSpecialMoveBehaviour(specialMove)) {
			promotionUITaskCancellationTokenSource = new CancellationTokenSource();
			return;
		}

		game.ExecuteTurn(move);

		HalfMove latestHalfMove = HalfMoveTimeline.Current;
		if (latestHalfMove.CausedCheckmate || latestHalfMove.CausedStalemate) {
			BoardManager.Instance.SetActiveAllPieces(false);
			GameEndedEvent.Raise();
		} else BoardManager.Instance.EnsureOnlyPiecesOfSideAreEnabled(game.CurrentTurnSide);

		MoveExecutedEvent.Raise();
	}
	
	private async Task<bool> HandleSpecialMoveBehaviour(SpecialMove specialMove) {
		switch (specialMove) {
			case CastlingMove castlingMove:
				BoardManager.Instance.CastleRook(castlingMove.AssociatedPiece.Position);
				return true;
			case EnPassantMove enPassantMove:
				BoardManager.Instance.DestroyPieceAtPosition(enPassantMove.AssociatedPiece.Position);
				return true;
			case PromotionMove promotionMove:
				UIManager.Instance.SetActivePromotionUI(true);
				BoardManager.Instance.SetActiveAllPieces(false);

				ElectedPiece choice = await Task.Run(UIManager.Instance.GetUserPromotionPieceChoice, promotionUITaskCancellationTokenSource.Token);
				if (promotionUITaskCancellationTokenSource.Token.IsCancellationRequested) return false;
				
				promotionMove.AssociatedPiece = PromotionUtil.GeneratePromotionPiece(choice, promotionMove.End, game.CurrentTurnSide);
				BoardManager.Instance.DestroyPieceAtPosition(promotionMove.End);
				BoardManager.Instance.CreateAndPlacePieceGO(promotionMove.AssociatedPiece);

				UIManager.Instance.SetActivePromotionUI(false);
				BoardManager.Instance.SetActiveAllPieces(true);
				return true;
			default:
				return false;
		}
	}
}

#pragma warning restore 0649