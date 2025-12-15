// This file is auto-generated. Do not modify manually.
using UnityChess.DependencyInjection;
using UnityEngine;
using UnityEngine.UIElements;
using UnityChess.Application;
using UnityChess.Presentation;
using UnityChess.Presentation.View;
using UnityChess.Presentation.ViewModel;

namespace UnityChess.Presentation {
	public partial class Bootstrapper {
		private void InstallMain(ServiceRegistry registry, GameObject viewRoot, UIDocument uiDocument) {
			// Register Managers
			registry.RegisterSingleton<GameManager>(() => new GameManager());

			// Register ViewModels
			registry.RegisterSingleton<BoardVM>(() => new BoardVM());
			registry.RegisterSingleton<MenuVM>(() => new MenuVM());
			registry.RegisterSingleton<MoveHistoryVM>(() => new MoveHistoryVM());
			registry.RegisterSingleton<PromotionVM>(() => new PromotionVM());

			// Register Mediators
			registry.RegisterSingleton<BoardMediator>(() => new BoardMediator(registry.Resolve<GameManager>(), registry.Resolve<BoardVM>()));
			registry.RegisterSingleton<MenuMediator>(() => new MenuMediator(registry.Resolve<GameManager>(), registry.Resolve<MenuVM>()));
			registry.RegisterSingleton<MoveHistoryMediator>(() => new MoveHistoryMediator(registry.Resolve<GameManager>(), registry.Resolve<MoveHistoryVM>()));
			registry.RegisterSingleton<PromotionMediator>(() => new PromotionMediator(registry.Resolve<GameManager>(), registry.Resolve<PromotionVM>()));

			// Instantiate Mediators
			registry.Resolve<BoardMediator>();
			registry.Resolve<MenuMediator>();
			registry.Resolve<MoveHistoryMediator>();
			registry.Resolve<PromotionMediator>();

			// Initialize Views
			{
				BoardView view = viewRoot.GetComponent<BoardView>();
				if (view == null) { view = viewRoot.AddComponent<BoardView>(); }
				BoardVM vm = registry.Resolve<BoardVM>();
				view.Initialize(vm, uiDocument);
			}
			{
				MoveHistoryView view = viewRoot.GetComponent<MoveHistoryView>();
				if (view == null) { view = viewRoot.AddComponent<MoveHistoryView>(); }
				MoveHistoryVM vm = registry.Resolve<MoveHistoryVM>();
				view.Initialize(vm, uiDocument);
			}
			{
				PromotionView view = viewRoot.GetComponent<PromotionView>();
				if (view == null) { view = viewRoot.AddComponent<PromotionView>(); }
				PromotionVM vm = registry.Resolve<PromotionVM>();
				view.Initialize(vm, uiDocument);
			}
		}
	}
}
