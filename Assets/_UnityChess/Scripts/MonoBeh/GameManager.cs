using UnityChess;
using UnityEngine;
using static UnityChessDebug;

public class GameManager : MonoBehaviour {
	public static GameManager Instance;
	public Game Game;
	public MoveHistory MoveHistory;
	
	//Events
	public GameEvent NewGameStarted;
	
	//Debug
	public bool Debug;
	public GameObject DebugView;
	
	private void Awake() {
		if (Instance == null) {
			DontDestroyOnLoad(gameObject);
			Instance = this;
		} else if (Instance != this) {
			Destroy(gameObject);
		}
	}
	
	public void Start() {
		if (Debug) {
			StartNewGame(Mode.HvH);
		}
	}

	private void Update() {
		if (Debug) {
			UpdateBoardDebugView(Game.BoardList.Last.Value);
			UpdateMoveHistoryDebugView(Game.PreviousMoves);
		}
	}

	public void StartNewGame(Mode mode) {
		Game = new Game(mode);
		NewGameStarted.Raise();
	}

	public void OnPieceMoved() {
		Movement move = MoveHistory.Pop();
		
		//Debug.Log($"{Game.CurrentTurn}\t{move}");
		Game.ExecuteTurn(move);
	}
}