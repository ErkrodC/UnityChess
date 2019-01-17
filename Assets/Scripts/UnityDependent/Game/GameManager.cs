using System.Collections.Generic;
using System.Linq;
using UnityChess;
using UnityEngine;

public class GameManager : MonoBehaviourSingleton<GameManager> {
	public Game Game;
	public Stack<Movement> MoveHistory;
	public GameEvent NewGameStarted;
	public GameObject DebugView;
	public Piece[] CurrentPieces => Game.BoardList.Last.Value.BasePieceList.OfType<Piece>().ToArray();
	public Board CurrentBoard => Game.BoardList.Last.Value;
	public LinkedList<Turn> PreviousMoves => Game.PreviousMoves;
	
	public void Start() {
		MoveHistory = new Stack<Movement>();
#if GAME_TEST
		StartNewGame(Mode.HvH);
#endif
	}

	public void StartNewGame(Mode mode) {
		Game = new Game(mode);
		NewGameStarted.Raise();
	}

	public void OnPieceMoved() {
		Movement move = MoveHistory.Pop();
		Piece piece = CurrentBoard.GetPiece(move.Start);
		Game.ExecuteTurn(piece, move);
	}
}