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

	[HideInInspector] public int TurnNumber;

	public void ResetBoardToWhiteMove() {
		
	}

	public void ResetBoardToBlackMove() {
		
	}
}
