using UnityEngine;
using UnityEditor;
using Ardunity;


[CustomEditor(typeof(ValueOutput))]
public class ValueOutputEditor : ArdunityObjectEditor
{
    SerializedProperty script;
	SerializedProperty value;
	
	void OnEnable()
	{
        script = serializedObject.FindProperty("m_Script");
		value = serializedObject.FindProperty("value");
	}
	
	public override void OnInspectorGUI()
	{
		this.serializedObject.Update();
		
		//ValueOutput bridge = (ValueOutput)target;

        GUI.enabled = false;
        EditorGUILayout.PropertyField(script, true, new GUILayoutOption[0]);
        GUI.enabled = true;
		EditorGUILayout.PropertyField(value, new GUIContent("value"));
		
		this.serializedObject.ApplyModifiedProperties();
	}
	
	static public void AddMenuItem(GenericMenu menu, GenericMenu.MenuFunction2 func)
	{
		string menuName = "Unity/Add Bridge/Output/ValueOutput";
		
		if(Selection.activeGameObject != null)
			menu.AddItem(new GUIContent(menuName), false, func, typeof(ValueOutput));
		else
			menu.AddDisabledItem(new GUIContent(menuName));
	}
}
