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
	[SerializeField] private Image backgroundImage;
	[SerializeField] private Image whiteMoveButtonImage;
	[SerializeField] private Image blackMoveButtonImage;
	
	[HideInInspector] public int TurnNumber;
	private const float AlternateColorDarkenDifference = 0.1f;

	public void SetAlternateColor() {
		foreach (Image image in new []{ backgroundImage, whiteMoveButtonImage, blackMoveButtonImage }) {
			Color lightColor = image.color;
			image.color = new Color(lightColor.r - AlternateColorDarkenDifference, lightColor.g - AlternateColorDarkenDifference, lightColor.b - AlternateColorDarkenDifference);
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
