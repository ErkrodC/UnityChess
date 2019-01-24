using UnityEngine;
using UnityEngine.UI;

public class FullMoveUI : MonoBehaviour {
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
	public GameObject whiteMoveHighlight;
	public GameObject blackMoveHighlight;

	public int FullMoveNumber => transform.GetSiblingIndex() + 1;
	private int WhiteHalfMoveIndex => (FullMoveNumber - 1) * 2;
	private int BlackHalfMoveIndex => WhiteHalfMoveIndex + 1;

	private void Start() => ValidateMoveHighlights();

	public void SetAlternateColor(float darkenAmount) {
		foreach (Image image in new []{ backgroundImage, whiteMoveButtonImage, blackMoveButtonImage }) {
			Color lightColor = image.color;
			image.color = new Color(lightColor.r - darkenAmount, lightColor.g - darkenAmount, lightColor.b - darkenAmount);
		}
	}

	public void ResetBoardToWhiteMove() => GameManager.Instance.ResetGameToHalfMoveIndex(WhiteHalfMoveIndex);

	public void ResetBoardToBlackMove() => GameManager.Instance.ResetGameToHalfMoveIndex(BlackHalfMoveIndex);

	public void ValidateMoveHighlights() {
		int halfMoveCount = GameManager.Instance.HalfMoveCount;
		whiteMoveHighlight.SetActive(halfMoveCount == WhiteHalfMoveIndex);
		blackMoveHighlight.SetActive(halfMoveCount == BlackHalfMoveIndex);
	}
}
