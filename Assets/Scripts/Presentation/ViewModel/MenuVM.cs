using System;
using UnityChess.DependencyInjection;

namespace UnityChess.Presentation.ViewModel {
	[Serializable]
	public class MenuVM : IViewModel {
		public string fenString { get; set; }
		public string gameResult { get; set; }

		public Action onStartNewGameClicked { get; set; }
		public Action<string> onLoadFenClicked { get; set; }
	}
}
