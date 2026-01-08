using System;
using System.Collections.Generic;
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

				if (vm.pieceSetManifest == null) {
					pieceSetDropdown.SetEnabled(false);
					vm.manifestReady += OnManifestReady;
				} else {
					PopulateDropdownChoices(vm.pieceSetManifest);
				}

				pieceSetDropdown.RegisterValueChangedCallback(evt => {
					if (vm.pieceSetManifest == null) { return; }

					PieceSetManifest.Entry entry = vm.pieceSetManifest.entries
						.FirstOrDefault(e => e.displayName == evt.newValue);
					if (entry != null) {
						vm.onActivePieceSetKeyChanged?.Invoke(entry.key);
					}
				});

				void OnManifestReady(PieceSetManifest manifest) {
					PopulateDropdownChoices(vm.pieceSetManifest);
					vm.manifestReady -= OnManifestReady;
				}

				void PopulateDropdownChoices(PieceSetManifest manifest) {
					if (manifest?.entries == null || manifest.entries.Count == 0) {
						pieceSetDropdown.choices = new List<string>();
						pieceSetDropdown.value = string.Empty;
						pieceSetDropdown.SetEnabled(false);
					} else {
						pieceSetDropdown.choices = manifest.entries
							.Select(entry => entry.displayName)
							.ToList();
						pieceSetDropdown.value = pieceSetDropdown.choices.FirstOrDefault();
						pieceSetDropdown.SetEnabled(true);
					}
				}
			}
		}
	}
}