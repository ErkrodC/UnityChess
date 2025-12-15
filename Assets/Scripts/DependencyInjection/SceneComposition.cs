using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UnityChess.DependencyInjection {
	[CreateAssetMenu(fileName = "SceneComposition", menuName = "ScriptableObjects/SceneComposition")]
	public class SceneComposition : ScriptableObject {
		[Header("Mediators")]
		public List<MonoScript> mediatorScripts = new();

		[Header("Views")]
		public List<MonoScript> viewScripts = new();
	}
}