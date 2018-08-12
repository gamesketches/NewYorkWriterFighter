using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(Frame))]
public class FrameDrawer : PropertyDrawer {

	int heightSize = 4;
	int spriteSize = 120;
	int ppu = 50;
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
		EditorGUI.BeginProperty(position, label, property);
		
		position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

		var indent = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 0;

		SerializedProperty spriteProp = property.FindPropertyRelative("sprite");
		SerializedProperty hitBoxProp = property.FindPropertyRelative("hitBoxes");
		SerializedProperty hurtBoxProp = property.FindPropertyRelative("hurtBoxes");

		Sprite spriteTexture = (Sprite)spriteProp.objectReferenceValue;

		Rect hitBoxLabelRect = new Rect(position.x + 30, position.y + 20, 60, 16);
		Rect hurtBoxLabelRect = new Rect(position.x + 110, position.y + 20, 60, 16);

		Rect hitBoxInputRect = new Rect(position.x + 30, position.y + 40, 70, 16);
		Rect hurtBoxInputRect = new Rect(position.x + 110, position.y + 40, 70, 16);
		Rect spriteRect = new Rect(position.x - 100, position.y, spriteSize, spriteSize);
		Rect spriteObjRect = new Rect(position.x + 30, position.y, position.width - 30, 16);

		EditorGUIUtility.labelWidth = 40;
		EditorGUI.PropertyField(spriteObjRect, spriteProp);
		EditorGUIUtility.labelWidth = 120;
		EditorGUI.DrawTextureTransparent(spriteRect, spriteTexture.texture, ScaleMode.ScaleToFit);
		DrawBoxInput("Hit", hitBoxLabelRect, hitBoxProp, hitBoxInputRect, spriteRect, new Color(1f, 0f, 0f, 0.3f));
		DrawBoxInput("Hurt", hurtBoxLabelRect, hurtBoxProp, hurtBoxInputRect, spriteRect, new Color(0f, 1f, 0f, 0.3f));

		EditorGUI.indentLevel = indent;

		EditorGUI.EndProperty();
	}

	public override float GetPropertyHeight (SerializedProperty property, GUIContent label) {
		
		return base.GetPropertyHeight (property, label) * 8;
	}

	void DrawBoxInput(string label, Rect labelRect, SerializedProperty boxes, Rect inputRect, Rect spriteRect, Color boxColor) {
		int arraySize = boxes.arraySize;
		
		EditorGUI.BeginChangeCheck();
		EditorGUIUtility.labelWidth = 30;
		int newSize = EditorGUI.IntField(labelRect, label, arraySize);
		
		if(newSize < 0) newSize = 0;
		if(EditorGUI.EndChangeCheck()) {
			boxes.arraySize = newSize;
			arraySize = newSize;
		}
		EditorGUIUtility.labelWidth = 120;

		for(int i = 0; i < arraySize; i++) {
			Rect box = (Rect)boxes.GetArrayElementAtIndex(i).rectValue;
			EditorGUI.BeginChangeCheck();
			Rect newRect = EditorGUI.RectField(inputRect, box);
			if(EditorGUI.EndChangeCheck()) {
				boxes.GetArrayElementAtIndex(i).rectValue = newRect;
			}
			EditorGUI.DrawRect(AdjustRectToSpriteDisplay(spriteRect, newRect), boxColor);
			inputRect = new Rect(inputRect.x, inputRect.y + 35, 64, heightSize);
		}
	}

	Rect AdjustRectToSpriteDisplay(Rect spriteRect, Rect boxRect) {
			float ppuScaling = ppu / 2;
			return new Rect((boxRect.center.x  * ppuScaling) + spriteRect.center.x - (boxRect.width * ppuScaling), (-boxRect.center.y * ppuScaling) + spriteRect.center.y,
									boxRect.width * ppuScaling, boxRect.height * ppuScaling);
	}

}
