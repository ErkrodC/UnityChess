using UnityChess.Application;
using UnityChess.Presentation.ViewModel;
using UnityEngine;

namespace UnityChess.Presentation {
	public class Bootstrapper : MonoBehaviour {
		[SerializeField] private BoardVM _boardVM;
		[SerializeField] private MoveHistoryVM _moveHistoryVM;
		[SerializeField] private MenuVM _menuVM;
		[SerializeField] private PromotionVM _promotionVM;

		private BoardMediator _boardMediator;
		private MoveHistoryMediator _moveHistoryMediator;
		private MenuMediator _menuMediator;
		private PromotionMediator _promotionMediator;

		private void Awake() {
			GameManager gameManager = new GameManager();

			_boardMediator = new BoardMediator(gameManager, _boardVM);
			_moveHistoryMediator = new MoveHistoryMediator(gameManager, _moveHistoryVM);
			_menuMediator = new MenuMediator(gameManager, _menuVM);
			_promotionMediator = new PromotionMediator(gameManager, _promotionVM);

			gameManager.StartNewGame();
		}

		// ER TODO here be a good spot to pass calls application layer from Unity Update, say for timers?
	}
}