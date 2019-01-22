using System.Collections.Generic;
using UnityChess;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviourSingleton<UIManager> {
	[SerializeField] private GameObject promotionUI = null;
	[SerializeField] private Text resultText = null;
	[SerializeField] private InputField GameStringInputField = null;
	[SerializeField] private Image whiteTurnIndicator = null;
	[SerializeField] private Image blackTurnIndicator = null;
	[SerializeField] private GameObject moveHistoryContentParent = null;
	[SerializeField] private GameObject moveUIPrefab = null;
	[SerializeField] private Text[] boardInfoTexts = null;
	[SerializeField] private Color backgroundColor = new Color(0.39f, 0.39f, 0.39f);
	[SerializeField] private Color textColor = new Color(1f, 0.71f, 0.18f);
	[SerializeField, Range(-0.25f, 0.25f)] private float buttonColorDarkenAmount = 0f;
	[SerializeField, Range(-0.25f, 0.25f)] private float moveHistoryAlternateColorDarkenAmount = 0f;
	
	private bool userHasMadePromotionPieceChoice;
	private ElectedPiece userPromotionPieceChoice = ElectedPiece.None;
	private History<MoveUI> moveUIs;
	private Color buttonColor;

	private void Start() {
		moveUIs = new History<MoveUI>();
		foreach (Text boardInfoText in boardInfoTexts) {
			boardInfoText.color = textColor;
		}

		buttonColor = new Color(backgroundColor.r - buttonColorDarkenAmount, backgroundColor.g - buttonColorDarkenAmount, backgroundColor.b - buttonColorDarkenAmount);
	}

	public void OnNewGameStarted() {
		UpdateGameStringInputField();
		ValidateIndicators();
		
		for (int i = 0; i < moveHistoryContentParent.transform.childCount; i++) {
			Destroy(moveHistoryContentParent.transform.GetChild(i).gameObject);
		}
		
		moveUIs.Clear();

		resultText.gameObject.SetActive(false);
	}

	public void OnGameEnded() {
		HalfMove latestHalfMove = GameManager.Instance.PreviousMoves.Last;

		if (latestHalfMove.CausedCheckmate) resultText.text = $"{latestHalfMove.Piece.Color} Wins!";
		else if (latestHalfMove.CausedStalemate) resultText.text = "Draw.";

		resultText.gameObject.SetActive(true);
	}

	public void OnPieceMoved() {
		UpdateGameStringInputField();
		whiteTurnIndicator.enabled = !whiteTurnIndicator.enabled;
		blackTurnIndicator.enabled = !blackTurnIndicator.enabled;

		AddMoveToHistory(GameManager.Instance.PreviousMoves.Last, GameManager.Instance.CurrentTurnSide.Complement());
	}

	public void OnGameResetToTurn() {
		UpdateGameStringInputField();
		moveUIs.HeadIndex = GameManager.Instance.TurnCount / 2;
		ValidateIndicators();
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

	private void AddMoveToHistory(HalfMove latestHalfMove, Side latestTurnSide) {
		int turnCount = GameManager.Instance.TurnCount;
		if (moveUIs.HeadIndex + 1 < moveUIs.Count) {
			resultText.gameObject.SetActive(false);
			List<MoveUI> poppedMoveUIs = moveUIs.PopRange(moveUIs.HeadIndex + 1, moveUIs.Count - (moveUIs.HeadIndex + 1));
			foreach (MoveUI poppedMoveUI in poppedMoveUIs) Destroy(poppedMoveUI.gameObject);
		}
		
		switch (latestTurnSide) {
			case Side.Black:
				MoveUI latestMoveUI = moveUIs.Last;
				latestMoveUI.BlackMoveText.text = latestHalfMove.ToAlgebraicNotation();
				latestMoveUI.BlackMoveButton.enabled = true;
				
				break;
			case Side.White:
				GameObject newMoveUIGO = Instantiate(moveUIPrefab, moveHistoryContentParent.transform);
				MoveUI newMoveUI = newMoveUIGO.GetComponent<MoveUI>();
				newMoveUI.backgroundImage.color = backgroundColor;
				newMoveUI.whiteMoveButtonImage.color = buttonColor;
				newMoveUI.blackMoveButtonImage.color = buttonColor;
				newMoveUI.MoveNumberText.color = textColor;
				newMoveUI.WhiteMoveText.color = textColor;
				newMoveUI.BlackMoveText.color = textColor;

				newMoveUI.TurnNumber = turnCount / 2 + 1;
				if (newMoveUI.TurnNumber % 2 == 0) newMoveUI.SetAlternateColor(moveHistoryAlternateColorDarkenAmount);
				newMoveUI.MoveNumberText.text = $"{newMoveUI.TurnNumber}.";
				newMoveUI.WhiteMoveText.text = latestHalfMove.ToAlgebraicNotation();
				newMoveUI.BlackMoveText.text = "";
				newMoveUI.BlackMoveButton.enabled = false;
				newMoveUI.WhiteMoveButton.enabled = true;
				
				moveUIs.AddLast(newMoveUI);
				break;
		}
	}

	private void ValidateIndicators() {
		Side currentTurnSide = GameManager.Instance.CurrentTurnSide;
		whiteTurnIndicator.enabled = currentTurnSide == Side.White;
		blackTurnIndicator.enabled = currentTurnSide == Side.Black;
	}

	private void UpdateGameStringInputField() => GameStringInputField.text = GameManager.Instance.ExportToFEN();
}