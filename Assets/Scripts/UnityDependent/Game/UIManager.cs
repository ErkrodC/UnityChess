using System;
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
	[SerializeField] private Scrollbar moveHistoryScrollbar = null;
	[SerializeField] private GameObject moveUIPrefab = null;
	[SerializeField] private Text[] boardInfoTexts = null;
	[SerializeField] private Color backgroundColor = new Color(0.39f, 0.39f, 0.39f);
	[SerializeField] private Color textColor = new Color(1f, 0.71f, 0.18f);
	[SerializeField, Range(-0.25f, 0.25f)] private float buttonColorDarkenAmount = 0f;
	[SerializeField, Range(-0.25f, 0.25f)] private float moveHistoryAlternateColorDarkenAmount = 0f;
	
	private bool userHasMadePromotionPieceChoice;
	private ElectedPiece userPromotionPieceChoice = ElectedPiece.None;
	private Timeline<FullMoveUI> moveUITimeline;
	private Color buttonColor;

	private void Start() {
		moveUITimeline = new Timeline<FullMoveUI>();
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
		
		moveUITimeline.Clear();

		resultText.gameObject.SetActive(false);
	}

	public void OnGameEnded() {
		HalfMove latestHalfMove = GameManager.Instance.PreviousMoves.Current;

		if (latestHalfMove.CausedCheckmate) resultText.text = $"{latestHalfMove.Piece.Color} Wins!";
		else if (latestHalfMove.CausedStalemate) resultText.text = "Draw.";

		resultText.gameObject.SetActive(true);
	}

	public void OnMoveExecuted() {
		UpdateGameStringInputField();
		whiteTurnIndicator.enabled = !whiteTurnIndicator.enabled;
		blackTurnIndicator.enabled = !blackTurnIndicator.enabled;

		AddMoveToHistory(GameManager.Instance.PreviousMoves.Current, GameManager.Instance.CurrentTurnSide.Complement());
	}

	public void OnGameResetToHalfMove() {
		UpdateGameStringInputField();
		moveUITimeline.HeadIndex = GameManager.Instance.HalfMoveCount / 2;
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

	public void ResetGameToFirstHalfMove() => GameManager.Instance.ResetGameToHalfMoveIndex(0);
	
	public void ResetGameToPreviousHalfMove() => GameManager.Instance.ResetGameToHalfMoveIndex(Math.Max(0, GameManager.Instance.HalfMoveCount - 1));

	public void ResetGameToNextHalfMove() => GameManager.Instance.ResetGameToHalfMoveIndex(Math.Min(GameManager.Instance.HalfMoveCount + 1, GameManager.Instance.PreviousMoves.Span - 1));

	public void ResetGameToLastHalfMove() => GameManager.Instance.ResetGameToHalfMoveIndex(GameManager.Instance.PreviousMoves.Span - 1);

	public void StartNewGame(int mode) => GameManager.Instance.StartNewGame(mode);

	private void AddMoveToHistory(HalfMove latestHalfMove, Side latestTurnSide) {
		RemoveAlternateHistory();
		int turnCount = GameManager.Instance.HalfMoveCount;
		
		switch (latestTurnSide) {
			case Side.Black:
				FullMoveUI latestFullMoveUI = moveUITimeline.Current;
				latestFullMoveUI.BlackMoveText.text = latestHalfMove.ToAlgebraicNotation();
				latestFullMoveUI.BlackMoveButton.enabled = true;
				
				break;
			case Side.White:
				GameObject newMoveUIGO = Instantiate(moveUIPrefab, moveHistoryContentParent.transform);
				FullMoveUI newFullMoveUI = newMoveUIGO.GetComponent<FullMoveUI>();
				newFullMoveUI.backgroundImage.color = backgroundColor;
				newFullMoveUI.whiteMoveButtonImage.color = buttonColor;
				newFullMoveUI.blackMoveButtonImage.color = buttonColor;

				if (newFullMoveUI.FullMoveNumber % 2 == 0) newFullMoveUI.SetAlternateColor(moveHistoryAlternateColorDarkenAmount);
				newFullMoveUI.MoveNumberText.text = $"{newFullMoveUI.FullMoveNumber}.";
				newFullMoveUI.WhiteMoveText.text = latestHalfMove.ToAlgebraicNotation();
				newFullMoveUI.BlackMoveText.text = "";
				newFullMoveUI.BlackMoveButton.enabled = false;
				newFullMoveUI.WhiteMoveButton.enabled = true;
				
				moveUITimeline.AddNext(newFullMoveUI);
				break;
		}

		moveHistoryScrollbar.value = 0;
	}

	private void RemoveAlternateHistory() {
		if (!moveUITimeline.IsUpToDate) {
			resultText.gameObject.SetActive(GameManager.Instance.PreviousMoves.Current.CausedCheckmate);
			List<FullMoveUI> divergentFullMoveUIs = moveUITimeline.PopFuture();
			foreach (FullMoveUI divergentFullMoveUI in divergentFullMoveUIs) Destroy(divergentFullMoveUI.gameObject);
		}
	}

	private void ValidateIndicators() {
		Side currentTurnSide = GameManager.Instance.CurrentTurnSide;
		whiteTurnIndicator.enabled = currentTurnSide == Side.White;
		blackTurnIndicator.enabled = currentTurnSide == Side.Black;
	}

	private void UpdateGameStringInputField() => GameStringInputField.text = GameManager.Instance.ExportToFEN();
}