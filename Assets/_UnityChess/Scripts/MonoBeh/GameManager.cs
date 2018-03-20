using UnityChess;
using UnityEngine;

public class GameManager : MonoBehaviour {
	[HideInInspector] public static GameManager Instance;
	public Game Game;
	public MoveHistory MoveHistory;
	
	//Events
	public GameEvent NewGameStarted;
	
	private void Awake() {
		if (Instance == null) {
			DontDestroyOnLoad(gameObject);
			Instance = this;
		} else if (Instance != this) {
			Destroy(gameObject);
		}
	}
	
	public void Start() {
		//in for testing
		StartNewGame(Mode.HvH);
	}

	public void StartNewGame(Mode mode) {
		Game = new Game(mode);
		NewGameStarted.Raise();
	}

	public void OnPieceMoved() {
		Game.ExecuteTurn(MoveHistory.Pop());
	}
}