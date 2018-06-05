using UnityEngine;
using UnityEditor;
using Ardunity;

[CustomEditor(typeof(TriggerInput))]
public class TriggerInputEditor : ArdunityObjectEditor
{
    SerializedProperty script;
	SerializedProperty checkEdge;
	SerializedProperty OnTrigger;

	void OnEnable()
	{
        script = serializedObject.FindProperty("m_Script");
		checkEdge = serializedObject.FindProperty("checkEdge");
		OnTrigger = serializedObject.FindProperty("OnTrigger");
	}

	public override void OnInspectorGUI()
	{
		this.serializedObject.Update();
		
		//TriggerInput bridge = (TriggerInput)target;
        
        GUI.enabled = false;
        EditorGUILayout.PropertyField(script, true, new GUILayoutOption[0]);
        GUI.enabled = true;
		EditorGUILayout.PropertyField(checkEdge, new GUIContent("checkEdge"));
		
		EditorGUILayout.Separator();
		EditorGUILayout.PropertyField(OnTrigger, new GUIContent("OnTrigger"));

		this.serializedObject.ApplyModifiedProperties();
	}
	
	static public void AddMenuItem(GenericMenu menu, GenericMenu.MenuFunction2 func)
	{
		string menuName = "Unity/Add Bridge/Input/TriggerInput";
		
		if(Selection.activeGameObject != null)
			menu.AddItem(new GUIContent(menuName), false, func, typeof(TriggerInput));
		else
			menu.AddDisabledItem(new GUIContent(menuName));
	}
}
