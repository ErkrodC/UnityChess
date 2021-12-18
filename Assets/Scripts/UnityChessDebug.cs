using System.Collections.Generic;
using UnityChess;
using UnityEngine;
using UnityEngine.UI;

public class UnityChessDebug : MonoBehaviourSingleton<UnityChessDebug> {
	[SerializeField] private GameObject debugBoard = null;
	[SerializeField] private int fontSize = 20;

	private void Awake() {
		for (int file = 1; file <= 8; file++) {
			for (int rank = 1; rank <= 8; rank++) {
				Transform squareTransform = debugBoard.transform.Find($"{SquareUtil.SquareToString(file, rank)}");
				Text squareText = squareTransform.GetComponentInChildren<Text>();
				squareText.fontSize = fontSize;
				squareText.resizeTextForBestFit = false;
			}
		}
	}

	private void Update() {
		UpdateBoardDebugView(GameManager.Instance.CurrentBoard);
	}

	public static void ShowLegalMovesInLog(ICollection<Movement> legalMoves) {
		string debugMessage = $"# of valid moves: {legalMoves?.Count ?? 0}\n";
		if (legalMoves != null) {
			foreach (Movement validMove in legalMoves) {
				debugMessage += $"{validMove}\n";
			}
		}
		
		Debug.LogWarning(debugMessage);
	}

	private void UpdateBoardDebugView(Board board) {
		for (int file = 1; file <= 8; file++) {
			for (int rank = 1; rank <= 8; rank++) {
				Piece piece = board[file, rank];
				Transform squareTransform = debugBoard.transform.Find($"{SquareUtil.SquareToString(file, rank)}");

				Text squareText = squareTransform.GetComponentInChildren<Text>();

				Image squareBackground = squareTransform.GetComponent<Image>();
				switch (piece) {
					case { Owner: Side.Black }:
						squareBackground.color = Color.black;
						squareText.color = Color.white;
						squareText.text = piece.ToTextArt();
						break;
					case { Owner: Side.White }:
						squareBackground.color = Color.white;
						squareText.color = Color.black;
						squareText.text = piece.ToTextArt();
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