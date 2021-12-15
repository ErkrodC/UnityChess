using UnityChess;
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

	private static int startingSideOffset => GameManager.Instance.StartingSide switch {
		Side.White => 0,
		_ => -1
	};

	private int WhiteHalfMoveIndex => transform.GetSiblingIndex() * 2 + startingSideOffset;
	private int BlackHalfMoveIndex => transform.GetSiblingIndex() * 2 + 1 + startingSideOffset;

	private void Start() {
		ValidateMoveHighlights();

		GameManager.MoveExecutedEvent += ValidateMoveHighlights;
		GameManager.GameResetToHalfMoveEvent += ValidateMoveHighlights;
	}

	private void OnDestroy() {
		GameManager.MoveExecutedEvent -= ValidateMoveHighlights;
		GameManager.GameResetToHalfMoveEvent -= ValidateMoveHighlights;
	}

	public void SetAlternateColor(float darkenAmount) {
		foreach (Image image in new []{ backgroundImage, whiteMoveButtonImage, blackMoveButtonImage }) {
			Color lightColor = image.color;
			image.color = new Color(lightColor.r - darkenAmount, lightColor.g - darkenAmount, lightColor.b - darkenAmount);
		}
	}

	public void ResetBoardToWhiteMove() => GameManager.Instance.ResetGameToHalfMoveIndex(WhiteHalfMoveIndex);

	public void ResetBoardToBlackMove() => GameManager.Instance.ResetGameToHalfMoveIndex(BlackHalfMoveIndex);

	private void ValidateMoveHighlights() {
		int latestHalfMoveIndex = GameManager.Instance.LatestHalfMoveIndex;
		whiteMoveHighlight.SetActive(latestHalfMoveIndex == WhiteHalfMoveIndex);
		blackMoveHighlight.SetActive(latestHalfMoveIndex == BlackHalfMoveIndex);
	}
}
