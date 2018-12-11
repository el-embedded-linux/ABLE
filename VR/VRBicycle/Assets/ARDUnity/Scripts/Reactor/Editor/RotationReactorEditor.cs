using UnityEngine;
using UnityEditor;
using Ardunity;


[CustomEditor(typeof(RotationReactor))]
public class RotationReactorEditor : ArdunityObjectEditor
{
	SerializedProperty script;
	SerializedProperty smoothFollow;
	
	void OnEnable()
	{
		script = serializedObject.FindProperty("m_Script");
		smoothFollow = serializedObject.FindProperty("smoothFollow");
	}
	
	public override void OnInspectorGUI()
	{
		this.serializedObject.Update();
		
		//RotationReactor reactor = (RotationReactor)target;
		
		GUI.enabled = false;
        EditorGUILayout.PropertyField(script, true, new GUILayoutOption[0]);
        GUI.enabled = true;

		EditorGUILayout.PropertyField(smoothFollow, new GUIContent("smoothFollow"));

		this.serializedObject.ApplyModifiedProperties();
	}
	
	static public void AddMenuItem(GenericMenu menu, GenericMenu.MenuFunction2 func)
	{
		string menuName = "Unity/Add Reactor/Transform/RotationReactor";
		
		if(Selection.activeGameObject != null)
			menu.AddItem(new GUIContent(menuName), false, func, typeof(RotationReactor));
		else
			menu.AddDisabledItem(new GUIContent(menuName));
	}
}