using UnityChess;
using UnityEngine;

public class PromotionUI : MonoBehaviour {
	[SerializeField] private GameObject promotionUI;
	
	private bool userHasMadeSelection;
	private ElectedPiece userChoice = ElectedPiece.None;

	public void ActivateUI() => promotionUI.gameObject.SetActive(true);
	public void DeactivateUI() => promotionUI.gameObject.SetActive(false);

	public ElectedPiece GetUserSelection() {
		while (!userHasMadeSelection) { }
		
		userHasMadeSelection = false;
		return userChoice;
	}

	public void OnElectionButton(int choice) {
		userChoice = (ElectedPiece) choice;
		userHasMadeSelection = true;
	}
}
