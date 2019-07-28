#pragma warning disable 0649

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityChess;
using UnityEngine;

public class GameManager : MonoBehaviourSingleton<GameManager> {
	public event Action NewGameStarted;
	public event Action GameEnded;
	public event Action GameResetToHalfMove;
	public event Action MoveExecuted;
	
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
		promotionUITaskCancellationTokenSource?.Cancel();
		GameResetToHalfMove?.Invoke();
	}

	public bool TryExecuteMove(Movement move) {
		if (!game.TryExecuteMove(move)) return false;

		HalfMove latestHalfMove = HalfMoveTimeline.Current;
		if (latestHalfMove.CausedCheckmate || latestHalfMove.CausedStalemate) {
			BoardManager.Instance.SetActiveAllPieces(false);
			GameEnded?.Invoke();
		} else BoardManager.Instance.EnsureOnlyPiecesOfSideAreEnabled(game.CurrentTurnSide);

		MoveExecuted?.Invoke();

		return true;
	}
	
	private async Task<SpecialMove> HandleSpecialMoveBehaviour(SpecialMove specialMove) {
		switch (specialMove) {
			case CastlingMove castlingMove:
				BoardManager.Instance.CastleRook(castlingMove.AssociatedPiece.Position);
				return castlingMove;
			case EnPassantMove enPassantMove:
				BoardManager.Instance.TryDestroyVisualPiece(enPassantMove.AssociatedPiece.Position);
				return enPassantMove;
			case PromotionMove promotionMove:
				UIManager.Instance.SetActivePromotionUI(true);
				BoardManager.Instance.SetActiveAllPieces(false);

				Func<ElectedPiece> task = UIManager.Instance.GetUserPromotionPieceChoice;

				promotionUITaskCancellationTokenSource?.Cancel();
				promotionUITaskCancellationTokenSource = new CancellationTokenSource();
				ElectedPiece choice = await Task.Run(task, promotionUITaskCancellationTokenSource.Token);
				
				UIManager.Instance.SetActivePromotionUI(false);
				BoardManager.Instance.SetActiveAllPieces(true);

				if (promotionUITaskCancellationTokenSource == null) { return null; }
				else if (promotionUITaskCancellationTokenSource.Token.IsCancellationRequested) return null;
				
				promotionMove.AssociatedPiece = PromotionUtil.GeneratePromotionPiece(choice, promotionMove.End, game.CurrentTurnSide);
				BoardManager.Instance.TryDestroyVisualPiece(promotionMove.Start);
				BoardManager.Instance.TryDestroyVisualPiece(promotionMove.End);
				BoardManager.Instance.CreateAndPlacePieceGO(promotionMove.AssociatedPiece);

				promotionUITaskCancellationTokenSource = null;
				return promotionMove;
			default:
				return null;
		}
	}

	private async void OnPieceMoved(Square movedPieceInitialSquare, Transform movedPieceTransform, Transform closestBoardSquareTransform) {
		Square closestSquare = SquareUtil.StringToSquare(closestBoardSquareTransform.name);

		bool foundLegalMove = game.TryGetLegalMove(movedPieceInitialSquare, closestSquare, out Movement move);
		
		if (move is SpecialMove specialMove) {
			if ((move = await HandleSpecialMoveBehaviour(specialMove)) == null) { return; }
		}
		
		if (foundLegalMove && TryExecuteMove(move)) {
			if (!(move is SpecialMove)) { BoardManager.Instance.TryDestroyVisualPiece(move.End); }
			if (move is PromotionMove) { movedPieceTransform = BoardManager.Instance.GetPieceGOAtPosition(move.End).transform; }
			
			movedPieceTransform.parent = closestBoardSquareTransform;
			movedPieceTransform.position = closestBoardSquareTransform.position;
			
			EnqueueValidMove(move);
		} else {
			movedPieceTransform.position = movedPieceTransform.parent.position;
#if DEBUG_VIEW
			UnityChessDebug.ShowLegalMovesInLog(CurrentBoard[movedPieceInitialSquare]);
#endif
		}
	}
}

#pragma warning restore 0649