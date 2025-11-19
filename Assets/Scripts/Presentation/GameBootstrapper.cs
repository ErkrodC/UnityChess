using UnityChess.Application;
using UnityChess.Presentation.ViewModel;
using UnityEngine;

namespace UnityChess.Presentation {
	public class GameBootstrapper : MonoBehaviour {
		[Header("View Models")]
		[SerializeField] private GameViewModel _gameViewModel;

		private GameManager _gameManager;
		private GamePresenter _gamePresenter;

		private void Awake() {
			_gameManager = new GameManager();
			_gamePresenter = new GamePresenter(_gameManager, _gameViewModel);
		}

		private void Start() {
			_gameManager.StartNewGame();
		}

		public void OnNewGameButtonClicked() => _gameManager.StartNewGame();
	}
}