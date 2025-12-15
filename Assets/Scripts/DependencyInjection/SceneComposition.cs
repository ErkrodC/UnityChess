using System.Collections.Generic;
using UnityEngine;

namespace UnityChess.DependencyInjection {
	[CreateAssetMenu(fileName = "SceneComposition", menuName = "ScriptableObjects/SceneComposition")]
	public class SceneComposition : ScriptableObject {
		public List<string> includedMediatorGUIDs = new();
		public List<string> includedViewGUIDs = new();
	}
}