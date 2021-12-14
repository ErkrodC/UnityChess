using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityChess;
using UnityEngine;

public class GameManager : MonoBehaviourSingleton<GameManager> {
	public static event Action NewGameStartedEvent;
	public static event Action GameEndedEvent;
	public static event Action GameResetToHalfMoveEvent;
	public static event Action MoveExecutedEvent;
	
	public Board CurrentBoard => game.BoardTimeline.Current;
	public Side SideToMove => game.SideToMove;
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
	private FENSerializer fenSerializer;
	private PGNSerializer pgnSerializer;
	private CancellationTokenSource promotionUITaskCancellationTokenSource;
	private ElectedPiece userPromotionChoice = ElectedPiece.None;
	private IGameSerializer currentSerializer;
	private Dictionary<GameSerializationType, IGameSerializer> serializersByType;
	private GameSerializationType selectSerializationType = GameSerializationType.FEN;

	public void Start() {
		VisualPiece.VisualPieceMoved += OnPieceMoved;
		
		moveQueue = new Queue<Movement>();

		serializersByType = new Dictionary<GameSerializationType, IGameSerializer> {
			[GameSerializationType.FEN] = new FENSerializer(),
			[GameSerializationType.PGN] = new PGNSerializer()
		};
		
#if GAME_TEST
		StartNewGame(Mode.HumanVsHuman);
#endif
		
#if DEBUG_VIEW
		unityChessDebug.gameObject.SetActive(true);
		unityChessDebug.enabled = true;
#endif
	}
	
	public string SerializeGame() {
		return serializersByType.TryGetValue(selectSerializationType, out IGameSerializer serializer)
			? serializer?.Serialize(game)
			: null;
	}

	public void StartNewGame(Mode mode) {
		game = new Game(mode, GameConditions.NormalStartingConditions);
		NewGameStartedEvent?.Invoke();
	}

	public void ResetGameToHalfMoveIndex(int halfMoveIndex) {
		if (!game.ResetGameToHalfMoveIndex(halfMoveIndex)) return;
		
		UIManager.Instance.SetActivePromotionUI(false);
		promotionUITaskCancellationTokenSource?.Cancel();
		GameResetToHalfMoveEvent?.Invoke();
	}

	private bool TryExecuteMove(Movement move) {
		if (!game.TryExecuteMove(move)) {
			return false;
		}

		HalfMove latestHalfMove = HalfMoveTimeline.Current;
		if (latestHalfMove.CausedCheckmate || latestHalfMove.CausedStalemate) {
			BoardManager.Instance.SetActiveAllPieces(false);
			GameEndedEvent?.Invoke();
		} else {
			BoardManager.Instance.EnsureOnlyPiecesOfSideAreEnabled(game.SideToMove);
		}

		MoveExecutedEvent?.Invoke();

		return true;
	}
	
	private async Task<bool> TryHandleSpecialMoveBehaviourAsync(SpecialMove specialMove) {
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

				promotionUITaskCancellationTokenSource?.Cancel();
				promotionUITaskCancellationTokenSource = new CancellationTokenSource();
				
				ElectedPiece choice = await Task.Run(GetUserPromotionPieceChoice, promotionUITaskCancellationTokenSource.Token);
				
				UIManager.Instance.SetActivePromotionUI(false);
				BoardManager.Instance.SetActiveAllPieces(true);

				if (promotionUITaskCancellationTokenSource == null
				    || promotionUITaskCancellationTokenSource.Token.IsCancellationRequested
				) { return false; }

				promotionMove.SetPromotionPiece(
					PromotionUtil.GeneratePromotionPiece(choice, promotionMove.End, game.SideToMove)
				);
				BoardManager.Instance.TryDestroyVisualPiece(promotionMove.Start);
				BoardManager.Instance.TryDestroyVisualPiece(promotionMove.End);
				BoardManager.Instance.CreateAndPlacePieceGO(promotionMove.AssociatedPiece);

				promotionUITaskCancellationTokenSource = null;
				return true;
			default:
				return false;
		}
	}
	
	private ElectedPiece GetUserPromotionPieceChoice() {
		while (userPromotionChoice == ElectedPiece.None) { }

		ElectedPiece result = userPromotionChoice;
		userPromotionChoice = ElectedPiece.None;
		return result;
	}
	
	public void ElectPiece(ElectedPiece choice) {
		userPromotionChoice = choice;
	}

	private async void OnPieceMoved(Square movedPieceInitialSquare, Transform movedPieceTransform, Transform closestBoardSquareTransform) {
		Square closestSquare = SquareUtil.StringToSquare(closestBoardSquareTransform.name);

		if (!game.TryGetLegalMove(movedPieceInitialSquare, closestSquare, out Movement move)) {
			movedPieceTransform.position = movedPieceTransform.parent.position;
#if DEBUG_VIEW
			Piece movedPiece = CurrentBoard[movedPieceInitialSquare];
			game.TryGetLegalMovesForPiece(movedPiece, out ICollection<Movement> legalMoves);
			UnityChessDebug.ShowLegalMovesInLog(legalMoves);
#endif
			return;
		}

		if ((move is not SpecialMove specialMove || await TryHandleSpecialMoveBehaviourAsync(specialMove))
		    && TryExecuteMove(move)
		) {
			if (move is not SpecialMove) {
				BoardManager.Instance.TryDestroyVisualPiece(move.End);
			}

			if (move is PromotionMove) {
				movedPieceTransform = BoardManager.Instance.GetPieceGOAtPosition(move.End).transform;
			}

			movedPieceTransform.parent = closestBoardSquareTransform;
			movedPieceTransform.position = closestBoardSquareTransform.position;

			moveQueue.Enqueue(move);
		}
	}

	public bool HasLegalMoves(Piece piece) {
		return game.TryGetLegalMovesForPiece(piece, out _);
	}
}