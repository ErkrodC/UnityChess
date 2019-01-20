using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityChess;
using UnityEngine;

public class GameManager : MonoBehaviourSingleton<GameManager> {
	public Game Game;
	public Queue<Movement> MoveQueue;
	[SerializeField] private GameEvent NewGameStartedEvent;
	[SerializeField] private GameEvent GameEndedEvent;
	[SerializeField] private UnityChessDebug unityChessDebug;
	public Piece[] CurrentPieces => Game.BoardList.Last.Value.BasePieceList.OfType<Piece>().ToArray();
	public Board CurrentBoard => Game.BoardList.Last.Value;
	public LinkedList<Turn> PreviousMoves => Game.PreviousMoves;
	[HideInInspector] public bool checkmated;
	[HideInInspector] public bool stalemated;
	[HideInInspector] public bool @checked;
	
	public void Start() {
		MoveQueue = new Queue<Movement>();
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

		Board currentBoard = CurrentBoard;
		Side currentTurnSide = Game.CurrentTurnSide;
		checkmated = Rules.IsPlayerCheckmated(currentBoard, currentTurnSide);
		stalemated = Rules.IsPlayerStalemated(currentBoard, currentTurnSide);
		@checked = Rules.IsPlayerInCheck(currentBoard, currentTurnSide);

		if (checkmated || stalemated) {
			BoardManager.Instance.SetActiveAllPieces(false);
			GameEndedEvent.Raise();
		} else {
			BoardManager.Instance.EnsureOnlyPiecesOfSideAreEnabled(Game.CurrentTurnSide);
		}
	}
}