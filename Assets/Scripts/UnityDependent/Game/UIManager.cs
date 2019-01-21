using System;
using System.Collections.Generic;
using UnityChess;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviourSingleton<UIManager> {
	[SerializeField] private GameObject promotionUI = null;
	[SerializeField] private GameObject gameEndUI = null;
	[SerializeField] private Text resultText = null;
	[SerializeField] private Image whiteTurnIndicator = null;
	[SerializeField] private Image blackTurnIndicator = null;
	[SerializeField] private GameObject moveHistoryContentParent = null;
	[SerializeField] private GameObject MoveUIPrefab = null;
	
	private bool userHasMadePromotionPieceChoice;
	private ElectedPiece userPromotionPieceChoice = ElectedPiece.None;
	private List<MoveUI> moveUIs;

	private void Start() {
		moveUIs = new List<MoveUI>();
	}

	public void OnNewGameStarted() {
		moveUIs.Clear();
		whiteTurnIndicator.enabled = true;
		blackTurnIndicator.enabled = false;
	}

	public void OnGameEnded() {
		bool checkMated = GameManager.Instance.checkmated;
		bool staleMated = GameManager.Instance.stalemated;

		if (checkMated) resultText.text = $"{GameManager.Instance.Game.CurrentTurnSide.Complement()} Wins!";
		else if (staleMated) resultText.text = "Draw.";
		gameEndUI.SetActive(true);
	}

	public void OnPieceMoved() {
		whiteTurnIndicator.enabled = !whiteTurnIndicator.enabled;
		blackTurnIndicator.enabled = !blackTurnIndicator.enabled;

		AddMoveToHistory(GameManager.Instance.LatestTurn, GameManager.Instance.Game.CurrentTurnSide.Complement());
	}

	public void ActivatePromotionUI() => promotionUI.gameObject.SetActive(true);

	public void DeactivatePromotionUI() => promotionUI.gameObject.SetActive(false);

	public ElectedPiece GetUserPromotionPieceChoice() {
		while (!userHasMadePromotionPieceChoice) { }
		
		userHasMadePromotionPieceChoice = false;
		return userPromotionPieceChoice;
	}

	public void OnElectionButton(int choice) {
		userPromotionPieceChoice = (ElectedPiece) choice;
		userHasMadePromotionPieceChoice = true;
	}

	private void AddMoveToHistory(Turn latestTurn, Side latestTurnSide) {
		switch (latestTurnSide) {
			case Side.Black:
				MoveUI latestMoveUI = moveUIs[moveUIs.Count - 1];
				latestMoveUI.BlackMoveText.text = GetMoveText(latestTurn);
				latestMoveUI.BlackMoveButton.enabled = true;
				
				break;
			case Side.White:
				GameObject newMoveUIGO = Instantiate(MoveUIPrefab, moveHistoryContentParent.transform);
				MoveUI newMoveUI = newMoveUIGO.GetComponent<MoveUI>();

				newMoveUI.TurnNumber = GameManager.Instance.Game.TurnCount / 2 + 1;
				newMoveUI.MoveNumberText.text = $"{newMoveUI.TurnNumber}.";
				newMoveUI.WhiteMoveText.text = GetMoveText(latestTurn);
				newMoveUI.WhiteMoveButton.enabled = true;
				
				moveUIs.Add(newMoveUI);
				break;
		}
	}

	private static string GetMoveText(Turn turn) {
		string moveText = "";
		string captureText = turn.CapturedPiece ? "x" : "";
		string suffix = turn.CausedCheckmate ? "#" :
		                turn.CausedCheck     ? "+" : "";
		switch (turn.Piece) {
			case Pawn _:
				if (turn.CapturedPiece) moveText += $"{SquareUtil.FileIntToCharMap[turn.Move.Start.File]}x";
				moveText += $"{SquareUtil.SquareToString(turn.Move.End)}{suffix}";
				break;
			case Knight _:
				moveText += $"N{captureText}{SquareUtil.SquareToString(turn.Move.End)}{suffix}";
				break;
			case Bishop _:
				moveText += $"B{captureText}{SquareUtil.SquareToString(turn.Move.End)}{suffix}";
				break;
			case Rook _:
				moveText += $"R{captureText}{SquareUtil.SquareToString(turn.Move.End)}{suffix}";
				break;
			case Queen _:
				moveText += $"Q{captureText}{SquareUtil.SquareToString(turn.Move.End)}{suffix}";
				break;
			case King _:
				if (turn.Move is CastlingMove) moveText += turn.Move.End.File == 3 ? $"O-O-O{suffix}" : $"O-O{suffix}";
				else moveText += $"K{captureText}{SquareUtil.SquareToString(turn.Move.End)}{suffix}";
				break;
		}

		return moveText;
	}
}