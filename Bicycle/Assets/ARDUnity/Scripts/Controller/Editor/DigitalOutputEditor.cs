using UnityEngine;
using UnityEditor;
using Ardunity;


[CustomEditor(typeof(DigitalOutput))]
public class DigitalOutputEditor : ArdunityObjectEditor
{
	bool foldout = false;

    SerializedProperty script;
	SerializedProperty id;
	SerializedProperty pin;
	SerializedProperty defaultValue;
	SerializedProperty resetOnStop;
	SerializedProperty Value;

	void OnEnable()
	{
        script = serializedObject.FindProperty("m_Script");
		id = serializedObject.FindProperty("id");
		pin = serializedObject.FindProperty("pin");
		defaultValue = serializedObject.FindProperty("defaultValue");
		resetOnStop = serializedObject.FindProperty("resetOnStop");
		Value = serializedObject.FindProperty("Value");
	}

	public override void OnInspectorGUI()
	{
		this.serializedObject.Update();
		
//		DigitalOutput controller = (DigitalOutput)target;
        
        GUI.enabled = false;
        EditorGUILayout.PropertyField(script, true, new GUILayoutOption[0]);
        GUI.enabled = true;
		foldout = EditorGUILayout.Foldout(foldout, "Sketch Options");
		if(foldout)
		{
			EditorGUI.indentLevel++;
			EditorGUILayout.PropertyField(id, new GUIContent("id"));
			EditorGUILayout.PropertyField(pin, new GUIContent("pin"));
			EditorGUILayout.PropertyField(defaultValue, new GUIContent("defaultValue"));
			EditorGUILayout.PropertyField(resetOnStop, new GUIContent("resetOnStop"));
			EditorGUI.indentLevel--;
		}

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Value", GUILayout.Width(80f));
		int index = 0;
		if(Value.boolValue == true)
			index = 1;
		int newIndex = GUILayout.SelectionGrid(index, new string[] {"LOW", "HIGH"}, 2);
		if(index != newIndex)
		{
			if(newIndex == 0)
				Value.boolValue = false;
			else
				Value.boolValue = true;
		}
		EditorGUILayout.EndHorizontal();
		
		this.serializedObject.ApplyModifiedProperties();
	}
	
	static public void AddMenuItem(GenericMenu menu, GenericMenu.MenuFunction2 func)
	{
		string menuName = "ARDUINO/Add Controller/Basic/DigitalOutput";
		
		if(Selection.activeGameObject != null)
			menu.AddItem(new GUIContent(menuName), false, func, typeof(DigitalOutput));
		else
			menu.AddDisabledItem(new GUIContent(menuName));
	}
}
