using UnityChess.Application;
using UnityChess.Presentation.ViewModel;
using UnityEngine;

namespace UnityChess.Presentation {
	public class GameBootstrapper : MonoBehaviour {
		[Header("View Models")]
		[SerializeField] private GameViewModel _gameViewModel;

		private GamePresenter _gamePresenter;

		private void Awake() {
			_gamePresenter = new GamePresenter(new GameManager(), _gameViewModel);
		}
	}
}