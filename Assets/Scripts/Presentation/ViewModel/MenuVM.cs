using System;
using UnityChess.DependencyInjection;

namespace UnityChess.Presentation.ViewModel {
	public class MenuVM : IViewModel {
		public string fenString;
		public string gameResult;

		public Action onStartNewGameClicked;
		public Action<string> onLoadFENClicked;
	}
}
