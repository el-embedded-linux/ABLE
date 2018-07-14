using UnityEngine;
using UnityEditor;
using Ardunity;


[CustomEditor(typeof(GenericServo))]
public class GenericServoEditor : ArdunityObjectEditor
{	
	bool foldout = false;
    
    SerializedProperty script;
	SerializedProperty id;
	SerializedProperty pin;
	SerializedProperty smooth;
	SerializedProperty calibratedAngle;
	SerializedProperty minAngle;
	SerializedProperty maxAngle;
	SerializedProperty angle;
	SerializedProperty handleObject;
	
	void OnEnable()
	{
        script = serializedObject.FindProperty("m_Script");
		id = serializedObject.FindProperty("id");
		pin = serializedObject.FindProperty("pin");
		smooth = serializedObject.FindProperty("smooth");
		calibratedAngle = serializedObject.FindProperty("calibratedAngle");
		minAngle = serializedObject.FindProperty("minAngle");
		maxAngle = serializedObject.FindProperty("maxAngle");
		angle = serializedObject.FindProperty("angle");
		handleObject = serializedObject.FindProperty("handleObject");
	}
	
	public override void OnInspectorGUI()
	{
		this.serializedObject.Update();
		
//		GenericServo controller = (GenericServo)target;
		
        GUI.enabled = false;
        EditorGUILayout.PropertyField(script, true, new GUILayoutOption[0]);
        GUI.enabled = true;
		foldout = EditorGUILayout.Foldout(foldout, "Sketch Options");
		if(foldout)
		{
			EditorGUI.indentLevel++;
			EditorGUILayout.PropertyField(id, new GUIContent("id"));
			EditorGUILayout.PropertyField(pin, new GUIContent("pin"));
			EditorGUILayout.PropertyField(smooth, new GUIContent("Smooth"));
			EditorGUI.indentLevel--;
		}

		EditorGUILayout.PropertyField(calibratedAngle, new GUIContent("Calibrated Angle"));
		EditorGUILayout.PropertyField(minAngle, new GUIContent("Min Angle"));
		EditorGUILayout.PropertyField(maxAngle, new GUIContent("Max Angle"));
		EditorGUILayout.PropertyField(angle, new GUIContent("Angle"));
		EditorGUILayout.PropertyField(handleObject, new GUIContent("Handle"));

		this.serializedObject.ApplyModifiedProperties();
	}
	
	static public void AddMenuItem(GenericMenu menu, GenericMenu.MenuFunction2 func)
	{
		string menuName = "ARDUINO/Add Controller/Motor/GenericServo";
		
		if(Selection.activeGameObject != null)
			menu.AddItem(new GUIContent(menuName), false, func, typeof(GenericServo));
		else
			menu.AddDisabledItem(new GUIContent(menuName));
	}
}
