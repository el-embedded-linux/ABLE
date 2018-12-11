using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using Ardunity;


[CustomEditor(typeof(TextReactor))]
public class TextReactorEditor : ArdunityObjectEditor
{
    SerializedProperty script;
	SerializedProperty format;
	
	void OnEnable()
	{
        script = serializedObject.FindProperty("m_Script");
		format = serializedObject.FindProperty("format");
	}
	
	public override void OnInspectorGUI()
	{
		this.serializedObject.Update();
		
		//TextReactor reactor = (TextReactor)target;

        GUI.enabled = false;
        EditorGUILayout.PropertyField(script, true, new GUILayoutOption[0]);
        GUI.enabled = true;
		EditorGUILayout.PropertyField(format, new GUIContent("format"));

		this.serializedObject.ApplyModifiedProperties();
	}
	
	static public void AddMenuItem(GenericMenu menu, GenericMenu.MenuFunction2 func)
	{
		string menuName = "Unity/Add Reactor/UI/TextReactor";
		
		if(Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<Text>() != null)
			menu.AddItem(new GUIContent(menuName), false, func, typeof(TextReactor));
		else
			menu.AddDisabledItem(new GUIContent(menuName));
	}
}