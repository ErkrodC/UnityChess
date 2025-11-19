using System;
using UnityEngine;

namespace UnityChess.Presentation.ViewModel {
	[Serializable]
	public class MenuVM {
		[field: SerializeField] public string fenString { get; set; }
		[field: SerializeField] public string gameResult { get; set; }

		public Action onStartNewGameClicked { get; set; }
		public Action<string> onLoadFenClicked { get; set; }
	}
}
