using UnityChess;
using UnityEngine;

public class GameManager : MonoBehaviour {
	public static Game Game;

	private BoardManager boardManager;
	public GameObject PieceInHand;

	public void Start() {
		boardManager = GameObject.FindWithTag("Board").GetComponent<BoardManager>();
		StartNewGame(Mode.HvH);
		boardManager.PopulateStartBoard();
	}

	public void StartNewGame(Mode mode) {
		Game = new Game(mode);
	}

	public void GrabPiece(GameObject piece) {
		PieceInHand = piece;
		GetComponent<Animator>().SetBool("HoldingPiece", true);
	}

	public void DropPieceInHand() {
		GetComponent<Animator>().SetBool("HoldingPiece", false);
	}
}