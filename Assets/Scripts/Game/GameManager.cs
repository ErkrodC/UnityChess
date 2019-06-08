#pragma warning disable 0649

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityChess;
using UnityEngine;

public class GameManager : MonoBehaviourSingleton<GameManager> {
	public event Action NewGameStarted;
	public event Action GameEndedEvent;
	public event Action GameResetToHalfMove;
	public event Action MoveExecutedEvent;
	
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
	
	[SerializeField] private UnityChessDebug unityChessDebug;
	private Game game;
	private Queue<Movement> moveQueue;
	private FENInterchanger fenInterchanger;
	private PGNInterchanger pgnInterchanger;
	private CancellationTokenSource promotionUITaskCancellationTokenSource;

	public void Start() {
		VisualPiece.VisualPieceMoved += OnPieceMoved;
		
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
	
	public void EnqueueValidMove(Movement move) => moveQueue.Enqueue(move);
	public string ExportToFEN() => fenInterchanger.Export(game);
	public string ExportToPGN() => pgnInterchanger.Export(game);
	public void StartNewGame(int mode) => StartNewGame((Mode) mode); // NOTE Used for binding to UnityEvent response, probably a cleaner way...

	public void StartNewGame(Mode mode) {
		game = new Game(mode, GameConditions.NormalStartingConditions);
		NewGameStarted?.Invoke();
	}

	public void ResetGameToHalfMoveIndex(int halfMoveIndex) {
		if (!game.ResetGameToHalfMoveIndex(halfMoveIndex)) return;
		
		UIManager.Instance.SetActivePromotionUI(false);
		promotionUITaskCancellationTokenSource.Cancel();
		GameResetToHalfMove?.Invoke();
	}
	
	public async void OnPieceMoved() => await TryExecuteMove(moveQueue.Dequeue());

	public async Task<bool> TryExecuteMove(Movement move) {
		if (move is SpecialMove specialMove) await HandleSpecialMoveBehaviour(specialMove);

		if (!game.TryExecuteMove(move)) return false;

		HalfMove latestHalfMove = HalfMoveTimeline.Current;
		if (latestHalfMove.CausedCheckmate || latestHalfMove.CausedStalemate) {
			BoardManager.Instance.SetActiveAllPieces(false);
			GameEndedEvent?.Invoke();
		} else BoardManager.Instance.EnsureOnlyPiecesOfSideAreEnabled(game.CurrentTurnSide);

		MoveExecutedEvent?.Invoke();

		return true;
	}
	
	private async Task<bool> HandleSpecialMoveBehaviour(SpecialMove specialMove) {
		switch (specialMove) {
			case CastlingMove castlingMove:
				BoardManager.Instance.CastleRook(castlingMove.AssociatedPiece.Position);
				return true;
			case EnPassantMove enPassantMove:
				BoardManager.Instance.TryDestroyVisualPiece(enPassantMove.AssociatedPiece.Position);
				return true;
			case PromotionMove promotionMove:
				UIManager.Instance.SetActivePromotionUI(true);
				BoardManager.Instance.SetActiveAllPieces(false);

				ElectedPiece choice = await Task.Run(UIManager.Instance.GetUserPromotionPieceChoice, promotionUITaskCancellationTokenSource.Token);
				if (promotionUITaskCancellationTokenSource.Token.IsCancellationRequested) return false;
				
				promotionMove.AssociatedPiece = PromotionUtil.GeneratePromotionPiece(choice, promotionMove.End, game.CurrentTurnSide);
				BoardManager.Instance.TryDestroyVisualPiece(promotionMove.End);
				BoardManager.Instance.CreateAndPlacePieceGO(promotionMove.AssociatedPiece);

				UIManager.Instance.SetActivePromotionUI(false);
				BoardManager.Instance.SetActiveAllPieces(true);
				return true;
			default:
				return false;
		}
	}

	private void OnPieceMoved(Square movedPieceInitialSquare, Transform movedPieceTransform, Transform closedBoardSquareTransform) {
		Square closestSquare = SquareUtil.StringToSquare(closedBoardSquareTransform.name);
	
		Movement move = new Movement(movedPieceInitialSquare, closestSquare);
		if (TryExecuteMove(move).Result) {
			BoardManager.Instance.TryDestroyVisualPiece(closestSquare);
			movedPieceTransform.parent = closedBoardSquareTransform;
			movedPieceTransform.position = closedBoardSquareTransform.position;
			EnqueueValidMove(move);
		} else {
			movedPieceTransform.position = movedPieceTransform.parent.position;
#if DEBUG_VIEW
			UnityChessDebug.ShowLegalMovesInLog(GameManager.Instance.CurrentBoard[movedPieceInitialSquare]);
#endif
		}
	}
}

#pragma warning restore 0649