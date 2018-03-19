using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameEvent))]
public class EventEditor : Editor {
	public override void OnInspectorGUI() {
		base.OnInspectorGUI();

		GUI.enabled = Application.isPlaying;

		GameEvent ge = target as GameEvent;
		if (GUILayout.Button("Raise")) {
			if (ge != null) ge.Raise();
		}
	}
}