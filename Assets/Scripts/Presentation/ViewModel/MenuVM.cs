using System;
using UnityChess.Application.Service;
using UnityChess.Core.Util;
using UnityChess.DependencyInjection;

namespace UnityChess.Presentation.ViewModel {
	public class MenuVM : IViewModel {
		public event Action<PieceSetManifest> manifestReady;

		public string fenString;
		public string gameResult;
		public Side playAsSide;
		public MatchOptions.PlayerType opponentType;
		public PieceSetManifest pieceSetManifest;

		public Action onStartNewGameClicked;
		public Action<string> onLoadFENClicked;
		public Action<string> onActivePieceSetKeyChanged;

		public void NotifyPieceSetManifestReady() => manifestReady?.Invoke(pieceSetManifest);
	}
}
