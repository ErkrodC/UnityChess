using System;
using System.Linq;
using UnityChess.Application.Service;
using UnityChess.Core.Util;
using UnityChess.Presentation.ViewModel;
using UnityEngine.UIElements;

namespace UnityChess.Presentation.View {
	public class MenuView : BaseView<MenuVM> {
		public override void Initialize(MenuVM vm) {
			vm.playAsSide = Side.White;
			vm.opponentType = MatchOptions.PlayerType.Human;

			DropdownField playAsDropDown = _root.Q<DropdownField>("play-as-dropdown");
			playAsDropDown.choices = Enum.GetNames(typeof(Side)).Where(s => s != nameof(Side.None)).ToList();
			playAsDropDown.value = nameof(Side.White);
			playAsDropDown.RegisterValueChangedCallback(evt => vm.playAsSide = Enum.Parse<Side>(evt.newValue));

			DropdownField playAgainstDropDown = _root.Q<DropdownField>("play-against-dropdown");
			playAgainstDropDown.choices = Enum.GetNames(typeof(MatchOptions.PlayerType)).ToList();
			playAgainstDropDown.value = nameof(MatchOptions.PlayerType.Human);
			playAgainstDropDown.RegisterValueChangedCallback(evt => vm.opponentType
				= Enum.Parse<MatchOptions.PlayerType>(evt.newValue));

			Button playButton = _root.Q<Button>("play-button");
			playButton.RegisterCallback<ClickEvent>(evt => vm.onStartNewGameClicked?.Invoke());
		}
	}
}