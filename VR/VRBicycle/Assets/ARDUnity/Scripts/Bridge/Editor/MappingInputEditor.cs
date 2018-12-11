using UnityEngine;
using UnityEditor;
using Ardunity;


[CustomEditor(typeof(MappingInput))]
public class MappingInputEditor : ArdunityObjectEditor
{
    SerializedProperty script;
	SerializedProperty sourceName;
    SerializedProperty resultName;
	SerializedProperty mapCurve;
	
	
	void OnEnable()
	{
        script = serializedObject.FindProperty("m_Script");
		sourceName = serializedObject.FindProperty("sourceName");
        resultName = serializedObject.FindProperty("resultName");
		mapCurve = serializedObject.FindProperty("mapCurve");
	}
	
	public override void OnInspectorGUI()
	{
		this.serializedObject.Update();
		
		//AnalogMap bridge = (AnalogMap)target;

        GUI.enabled = false;
        EditorGUILayout.PropertyField(script, true, new GUILayoutOption[0]);
        GUI.enabled = true;
		EditorGUILayout.PropertyField(sourceName, new GUIContent("sourceName"));
        EditorGUILayout.PropertyField(resultName, new GUIContent("resultName"));
		EditorGUILayout.PropertyField(mapCurve, new GUIContent("mapCurve"));
		
		this.serializedObject.ApplyModifiedProperties();
	}
	
	static public void AddMenuItem(GenericMenu menu, GenericMenu.MenuFunction2 func)
	{
		string menuName = "Unity/Add Bridge/Input/MappingInput";
		
		if(Selection.activeGameObject != null)
			menu.AddItem(new GUIContent(menuName), false, func, typeof(MappingInput));
		else
			menu.AddDisabledItem(new GUIContent(menuName));
	}
}
