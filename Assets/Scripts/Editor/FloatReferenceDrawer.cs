using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(FloatReference))]
public class FloatReferenceDrawer : PropertyDrawer {
	private readonly string[] popupOptions = {
		"Use Constant",
		"Use Variable"
	};

	private GUIStyle popupStyle;

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
		if (popupStyle == null) {
			popupStyle = new GUIStyle(GUI.skin.GetStyle("PaneOptions")) {imagePosition = ImagePosition.ImageOnly};
		}

		label = EditorGUI.BeginProperty(position, label, property);
		position = EditorGUI.PrefixLabel(position, label);

		EditorGUI.BeginChangeCheck();

		SerializedProperty useConstant = property.FindPropertyRelative("UseConstant");
		SerializedProperty constantValue = property.FindPropertyRelative("ConstantValue");
		SerializedProperty variable = property.FindPropertyRelative("Variable");

		Rect buttonRect = new Rect(position);
		buttonRect.yMin += popupStyle.margin.top;
		buttonRect.width = popupStyle.fixedWidth + popupStyle.margin.right;
		position.xMin = buttonRect.xMax;

		int indent = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 0;

		int result = EditorGUI.Popup(buttonRect, useConstant.boolValue ? 0 : 1, popupOptions, popupStyle);

		useConstant.boolValue = result == 0;

		EditorGUI.PropertyField(position, useConstant.boolValue ? constantValue : variable, GUIContent.none);

		if (EditorGUI.EndChangeCheck()) {
			property.serializedObject.ApplyModifiedProperties();
		}

		EditorGUI.indentLevel = indent;
		EditorGUI.EndProperty();
	}
}