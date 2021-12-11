using UnityChess;
using UnityEngine;
using UnityEngine.UI;

public class UnityChessDebug : MonoBehaviourSingleton<UnityChessDebug> {
	[SerializeField] private GameObject debugBoard = null;

	private void Update() {
		UpdateBoardDebugView(GameManager.Instance.CurrentBoard);
	}

	public static void ShowLegalMovesInLog(Piece piece) {
		string debMsg = $"# of valid moves: {piece.LegalMoves.Count}\n";
		foreach (Movement validMove in piece.LegalMoves) {
			debMsg += $"{validMove}\n";
		}
		Debug.Log(debMsg);
	}

	private void UpdateBoardDebugView(Board board) {
		for (int file = 1; file <= 8; file++) {
			for (int rank = 1; rank <= 8; rank++) {
				Piece piece = board[file, rank];
				Transform squareTransform = debugBoard.transform.Find($"{SquareUtil.FileRankToSquareString(file, rank)}");

				Text squareText = squareTransform.GetComponentInChildren<Text>();

				Image squareBackground = squareTransform.GetComponent<Image>();
				switch (piece) {
					case { Color: Side.Black }:
						squareBackground.color = Color.black;
						squareText.color = Color.white;
						squareText.text = piece.ToString();
						break;
					case { Color: Side.White }:
						squareBackground.color = Color.white;
						squareText.color = Color.black;
						squareText.text = piece.ToString();
						break;
					default:
						squareBackground.color = Color.gray;
						squareText.text = string.Empty;
						break;
				}
			}
		}
	}
}