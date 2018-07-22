using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(Frame))]
public class FrameDrawer : PropertyDrawer {

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
		EditorGUI.BeginProperty(position, label, property);
		
		position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

		var indent = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 0;
		
		//Rect spriteRect = new Rect(position.x, position.y, 30, position.height
		Rect spriteRect = new Rect(position.x, position.y, 30, position.height);
		Rect hitBoxRect = new Rect(position.x, position.y + 30, 50, position.height);
		Rect hurtBoxRect = new Rect(position.x, position.y + 90, position.width, position.height);

		EditorGUI.PropertyField(spriteRect, property.FindPropertyRelative("sprite"));
		EditorGUI.PropertyField(hitBoxRect, property.FindPropertyRelative("hitBoxes"));
		EditorGUI.PropertyField(hurtBoxRect, property.FindPropertyRelative("hurtBoxes"));

		EditorGUI.indentLevel = indent;

		EditorGUI.EndProperty();
	}
}
