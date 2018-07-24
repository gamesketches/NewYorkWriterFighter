using UnityEngine;
using UnityEditor;

//[CustomPropertyDrawer(typeof(Frame))]
public class FrameDrawer : PropertyDrawer {

	int heightSize = 4;
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
		EditorGUI.BeginProperty(position, label, property);
		
		position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

		var indent = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 0;
		
		Rect hitBoxRect = new Rect(position.x + 100, position.y, 50, position.height);
		Rect hurtBoxRect = new Rect(position.x + 100, position.y + 30, 50, position.height);
		Rect spriteRect = new Rect(position.x, position.y, 100, position.height);

		GUIContent spriteImage = new GUIContent(property.FindPropertyRelative("sprite").objectReferenceValue as Texture2D);
		EditorGUI.ObjectField(spriteRect, property.FindPropertyRelative("sprite"), spriteImage);
		EditorGUI.PropertyField(hitBoxRect, property.FindPropertyRelative("hitBoxes"));
		EditorGUI.PropertyField(hurtBoxRect, property.FindPropertyRelative("hurtBoxes"));

		EditorGUI.indentLevel = indent;

		EditorGUI.EndProperty();
	}

	public override float GetPropertyHeight (SerializedProperty property, GUIContent label) {
		
		return base.GetPropertyHeight (property, label) * heightSize;
	}
}
