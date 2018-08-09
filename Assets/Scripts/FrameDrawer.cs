using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(Frame))]
public class FrameDrawer : PropertyDrawer {

	int heightSize = 4;
	int spriteSize = 60;
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
		EditorGUI.BeginProperty(position, label, property);
		
		position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

		var indent = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 0;

		SerializedProperty spriteProp = property.FindPropertyRelative("sprite");
		SerializedProperty hitBoxProp = property.FindPropertyRelative("hitBoxes");
		SerializedProperty hurtBoxProp = property.FindPropertyRelative("hurtBoxes");

		Sprite spriteTexture = (Sprite)spriteProp.objectReferenceValue;

		int hitBoxArraySize = hitBoxProp.arraySize;

		Rect hitBoxFieldRect = new Rect(position.x + 50, position.y, 16, 16);

		EditorGUI.BeginChangeCheck();
		int newSize = EditorGUI.IntField(hitBoxFieldRect, "HitBoxes", hitBoxArraySize);
		if(newSize < 0) newSize = 0;
		if(EditorGUI.EndChangeCheck()){
			hitBoxProp.arraySize = newSize;
			hitBoxArraySize = newSize;
		}
		
		Rect hitBoxRect = new Rect(position.x + 50, position.y + 32, 64, 16);
		Rect hurtBoxRect = new Rect(position.x + 50, position.y + 30, 50, position.height);
		Rect spriteRect = new Rect(position.x - 100, position.y, 100, position.height);

		EditorGUI.DrawTextureTransparent(spriteRect, spriteTexture.texture, ScaleMode.ScaleToFit);
		//EditorGUI.PropertyField(hitBoxRect, property.FindPropertyRelative("hitBoxes"));
		for(int i = 0; i< hitBoxArraySize; i++) {
			Rect hitBox = (Rect)hitBoxProp.GetArrayElementAtIndex(i).rectValue;
			EditorGUI.RectField(hitBoxRect, hitBox);
			hitBox.xMin = spriteRect.xMax;
			hitBox.yMin = spriteRect.yMax;
			EditorGUI.DrawRect(hitBox, new Color(1f, 0f, 0f, 0.3f));
			hitBoxRect = new Rect(hitBoxRect.xMin, hitBoxRect.yMin + 32, 128, heightSize);
		}
		if(hitBoxArraySize == 0) EditorGUI.DrawRect(spriteRect, Color.blue);
		//EditorGUI.PropertyField(hurtBoxRect, property.FindPropertyRelative("hurtBoxes"));

		EditorGUI.indentLevel = indent;

		EditorGUI.EndProperty();
	}

	public override float GetPropertyHeight (SerializedProperty property, GUIContent label) {
		
		return base.GetPropertyHeight (property, label) * 8;
	}
}
