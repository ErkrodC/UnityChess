using System;
using UnityChess.Application.Service;
using UnityChess.Core.Util;
using UnityChess.DependencyInjection;

namespace UnityChess.Presentation.ViewModel {
	public class MenuVM : IViewModel {
		public string fenString;
		public string gameResult;
		public Side playAsSide;
		public MatchOptions.PlayerType opponentType;

		public Action onStartNewGameClicked;
		public Action<string> onLoadFENClicked;
	}
}
