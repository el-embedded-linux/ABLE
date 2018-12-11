using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using Ardunity;


[CustomEditor(typeof(ToggleEvent))]
public class ToggleEventEditor : Editor
{
	[MenuItem("ARDUnity/Add Utility/Event/ToggleEvent", true)]
	static bool ValidateMenu()
	{
		if(Selection.activeGameObject == null)
			return false;
		
		if(Selection.activeGameObject.GetComponent<Toggle>() == null)
			return false;
		
		return true;
	}
	[MenuItem("ARDUnity/Add Utility/Event/ToggleEvent", false, 10)]
    static void DoMenu()
    {
        Selection.activeGameObject.AddComponent<ToggleEvent>();
    }
	
	SerializedProperty script;
	SerializedProperty OnChecked;
    SerializedProperty OnUnchecked;
	
	void OnEnable()
	{
        script = serializedObject.FindProperty("m_Script");
		OnChecked = serializedObject.FindProperty("OnChecked");
        OnUnchecked = serializedObject.FindProperty("OnUnchecked");
	}
	
	public override void OnInspectorGUI()
	{
		this.serializedObject.Update();
		
		//ToggleEvent utility = (ToggleEvent)target;
		
        GUI.enabled = false;
        EditorGUILayout.PropertyField(script, true, new GUILayoutOption[0]);
        GUI.enabled = true;
		EditorGUILayout.PropertyField(OnChecked, new GUIContent("OnChecked"));
        EditorGUILayout.PropertyField(OnUnchecked, new GUIContent("OnUnchecked"));
		
		this.serializedObject.ApplyModifiedProperties();
	}
}