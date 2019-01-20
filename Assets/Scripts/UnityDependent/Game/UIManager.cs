using UnityChess;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviourSingleton<UIManager> {
	[SerializeField] private GameObject promotionUI;
	[SerializeField] private GameObject gameEndUI;
	[SerializeField] private Text resultText;
	[SerializeField] private GameObject moveHistoryContentParent;
	[SerializeField] private Image whiteTurnIndicator;
	[SerializeField] private Image blackTurnIndicator;
	
	private bool userHasMadePromotionPieceChoice;
	private ElectedPiece userPromotionPieceChoice = ElectedPiece.None;

	public void OnNewGameStarted() {
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
}