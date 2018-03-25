using System.Collections.Generic;
using UnityChess;
using UnityEngine;
using UnityEngine.UI;

public static class UnityChessDebug {
	
	public static void ShowLegalMovesInLog(Piece piece) {
		
		string debMsg = $"# of valid moves: {piece.ValidMoves.Count}\n";
		foreach (Movement validMove in piece.ValidMoves) {
			debMsg += $"{validMove}\n";
		}
		Debug.Log(debMsg);
	}
	
	public static void UpdateBoardDebugView(Board board) {
		GameObject debugBoard = GameManager.Instance.DebugView.transform.Find("Board").gameObject;
		debugBoard.SetActive(true);

		for (int file = 1; file <= 8; file++) {
			for (int rank = 1; rank <= 8; rank++) {
				BasePiece basePiece = board.GetBasePiece(file, rank);

				string pieceToString = "";
				
				if (basePiece is Piece) {
					pieceToString += ((Piece)basePiece).Side.ToString().Substring(0, 1);
					pieceToString += ((Piece)basePiece).GetType().Name;
				}
				
				Transform rankGO = debugBoard.transform.Find($"Rank{rank}");
				Transform squareGO = rankGO.transform.Find($"File{file}");
				Text squareText = squareGO.GetComponentInChildren<Text>();
			
				squareText.text = pieceToString;
			}
		}
	}

	public static void UpdateMoveHistoryDebugView(LinkedList<Movement> moveHistory) {
		GameObject debugMoveHistory = GameManager.Instance.DebugView.transform.Find("MoveHistory").gameObject;
		debugMoveHistory.SetActive(true);
		
		// TODO finish writing
	}
}