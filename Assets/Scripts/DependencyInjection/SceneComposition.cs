using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityChess.DependencyInjection {
	[CreateAssetMenu(fileName = "SceneComposition", menuName = "ScriptableObjects/SceneComposition")]
	public class SceneComposition : ScriptableObject {
		[Serializable]
		public class TypeReference {
			public string guid;           // Script GUID (most robust)
			public bool isIncluded;       // Checkbox state
			
			// Cached for editor display (regenerated on inspector draw)
			[NonSerialized] public string typeName;
			[NonSerialized] public Type resolvedType;
		}
		
		[Header("Included Types")]
		public List<TypeReference> mediators = new();
		public List<TypeReference> views = new();
	}
}