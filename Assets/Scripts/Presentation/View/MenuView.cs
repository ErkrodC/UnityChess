using System;
using System.Linq;
using UnityChess.Application.Service;
using UnityChess.Core.Util;
using UnityChess.Presentation.ViewModel;
using UnityEngine.UIElements;

namespace UnityChess.Presentation.View {
	public class MenuView : BaseView<MenuVM> {
		public override void Initialize(MenuVM vm) {
			{ // default values
				vm.playAsSide = Side.White;
				vm.opponentType = MatchOptions.PlayerType.Human;
			}

			{ // play as dropdown
				DropdownField playAsDropdown = _root.Q<DropdownField>("play-as-dropdown");
				playAsDropdown.choices = Enum.GetNames(typeof(Side)).Where(s => s != nameof(Side.None)).ToList();
				playAsDropdown.value = nameof(Side.White);
				playAsDropdown.RegisterValueChangedCallback(evt => vm.playAsSide = Enum.Parse<Side>(evt.newValue));
			}

			{ // opponent type dropdown
				DropdownField playAgainstDropdown = _root.Q<DropdownField>("play-against-dropdown");
				playAgainstDropdown.choices = Enum.GetNames(typeof(MatchOptions.PlayerType)).ToList();
				playAgainstDropdown.value = nameof(MatchOptions.PlayerType.Human);
				playAgainstDropdown.RegisterValueChangedCallback(evt => vm.opponentType
					= Enum.Parse<MatchOptions.PlayerType>(evt.newValue));
			}

			{ // play button
				Button playButton = _root.Q<Button>("play-button");
				playButton.RegisterCallback<ClickEvent>(_ => vm.onStartNewGameClicked?.Invoke());
			}

			{ // piece set dropdown
				DropdownField pieceSetDropdown = _root.Q<DropdownField>("piece-set-dropdown");
				pieceSetDropdown.choices = vm.pieceSetManifest.entries
					.Select(entry => entry.displayName)
					.ToList();
				pieceSetDropdown.value = pieceSetDropdown.choices.FirstOrDefault();
				pieceSetDropdown.RegisterValueChangedCallback(evt => {
					PieceSetManifest.Entry entry = vm.pieceSetManifest.entries
						.FirstOrDefault(e => e.displayName == evt.newValue);
					if (entry != null) {
						vm.onActivePieceSetKeyChanged?.Invoke(entry.addressablesKey);
					}
				});
			}
		}
	}
}