using UnityEngine;
using UnityEditor;
using Ardunity;


[CustomEditor(typeof(ToneOutput))]
public class ToneOutputEditor : ArdunityObjectEditor
{
    SerializedProperty script;
	SerializedProperty toneFrequency;
	
	void OnEnable()
	{
        script = serializedObject.FindProperty("m_Script");
		toneFrequency = serializedObject.FindProperty("toneFrequency");
	}
	
	public override void OnInspectorGUI()
	{
		this.serializedObject.Update();
		
		//Bool2ToneFrequency bridge = (Bool2ToneFrequency)target;
        
        GUI.enabled = false;
        EditorGUILayout.PropertyField(script, true, new GUILayoutOption[0]);
        GUI.enabled = true;
		EditorGUILayout.PropertyField(toneFrequency, new GUIContent("toneFrequency"));
		
		this.serializedObject.ApplyModifiedProperties();
	}
	
	static public void AddMenuItem(GenericMenu menu, GenericMenu.MenuFunction2 func)
	{
		string menuName = "Unity/Add Bridge/Output/ToneOutput";
		
		if(Selection.activeGameObject != null)
			menu.AddItem(new GUIContent(menuName), false, func, typeof(ToneOutput));
		else
			menu.AddDisabledItem(new GUIContent(menuName));
	}
}
