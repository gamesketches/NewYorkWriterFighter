using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Attack))]
[CanEditMultipleObjects]
public class AttackEditor : Editor {

	SerializedProperty hitBoxes;
	// Use this for initialization
	void Start () {
		
	}
	
	void OnEnable () {
	}

	public override void OnInspectorGUI() {
		serializedObject.Update();
		hitBoxes = serializedObject.GetIterator();//FindProperty("hitBoxes");
		int arraySize = hitBoxes.arraySize;//serializedObject.FindProperty("hitBoxes.Array.size").intValue;
		EditorGUILayout.IntField("Hit Box Array Size" + arraySize, arraySize);
		//EditorGUILayout.PropertyField(hitBoxes, new GUIContent("Hit Boxes"), true);
		serializedObject.ApplyModifiedProperties();
	}
}
