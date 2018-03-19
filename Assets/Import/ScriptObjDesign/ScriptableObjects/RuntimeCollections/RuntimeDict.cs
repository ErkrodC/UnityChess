using System.Collections.Generic;
using UnityEngine;

public abstract class RuntimeDict<TKey, TValue> : ScriptableObject {

	public Dictionary<TKey, TValue> Dict = new Dictionary<TKey, TValue>();
	[SerializeField] private List<TKey> keys;
	[SerializeField] private List<TValue> values;

	private void OnEnable() {
		if (keys.Count != values.Count) {
			throw new UnityException($"Number of Keys and Values on {name} must be equalized in the inspector.");
		}

		for (int i = 0; i < keys.Count; i++) {
			Add(keys[i], values[i]);
		}
	}

	public void Add(TKey key, TValue value) {
		if (!Dict.ContainsKey(key)) {
			Dict.Add(key, value);
		}
	}

	public void Remove(TKey key) {
		if (Dict.ContainsKey(key)) {
			Dict.Remove(key);
		}
	}
}