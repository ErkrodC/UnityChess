using UnityChess;
using UnityEngine;

public class GameManager : MonoBehaviour {
	public static Game Game;
	
	//Events
	public GameEvent NewGameStarted;

	public void Start() {
		//in for testing
		StartNewGame(Mode.HvH);
	}

	public void StartNewGame(Mode mode) {
		Game = new Game(mode);
		NewGameStarted.Raise();
	}
}