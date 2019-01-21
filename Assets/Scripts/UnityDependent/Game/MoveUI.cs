using UnityEngine;
using UnityEngine.UI;

public class MoveUI : MonoBehaviour {
	[Header("Moves")]
	public Text MoveNumberText;
	public Text WhiteMoveText;
	public Text BlackMoveText;
	public Button WhiteMoveButton;
	public Button BlackMoveButton;
	
	[Header("Analysis")]
	public Text WhiteAnalysisText;
	public Text BlackAnalysisText;
	public Image WhiteAnalysisFillImage;
	public Image BlackAnalysisFillImage;

	[Header("Colored Images")]
	public Image backgroundImage;
	public Image whiteMoveButtonImage;
	public Image blackMoveButtonImage;
	
	[HideInInspector] public int TurnNumber;

	public void SetAlternateColor(float darkenAmount) {
		foreach (Image image in new []{ backgroundImage, whiteMoveButtonImage, blackMoveButtonImage }) {
			Color lightColor = image.color;
			image.color = new Color(lightColor.r - darkenAmount, lightColor.g - darkenAmount, lightColor.b - darkenAmount);
		}
	}

	public void ResetBoardToWhiteMove() {
		int turnIndex = (TurnNumber - 1) * 2;
		GameManager.Instance.ResetGameToTurn(turnIndex);
	}

	public void ResetBoardToBlackMove() {
		int turnIndex = (TurnNumber - 1) * 2 + 1;
		GameManager.Instance.ResetGameToTurn(turnIndex);
	}
}
