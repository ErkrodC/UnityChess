using System;
using UnityEngine;

[CreateAssetMenu, Serializable]
public class FloatVariable : ScriptableObject {
#if UNITY_EDITOR
	[Multiline] public string DeveloperDescription = "";
#endif

	public float Value;

	public void SetValue(float value) {
		Value = value;
	}

	public void SetValue(FloatVariable value) {
		Value = value.Value;
	}

	public void ApplyChange(float amount) {
		Value += amount;
	}

	public void ApplyChange(FloatVariable amount) {
		Value += amount.Value;
	}
}