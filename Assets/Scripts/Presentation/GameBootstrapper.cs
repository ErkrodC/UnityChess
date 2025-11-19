using UnityChess.Application;
using UnityChess.Presentation.ViewModel;
using UnityEngine;

namespace UnityChess.Presentation {
	public class GameBootstrapper : MonoBehaviour {
		[Header("View Models")]
		[SerializeField] private GameVM _gameVM;

		private GamePresenter _gamePresenter;

		private void Awake() {
			_gamePresenter = new GamePresenter(new GameManager(), _gameVM);
		}
	}
}