using UnityEngine;
using UnityEditor;
using Ardunity;


[CustomEditor(typeof(DeviceRollReactor))]
public class DeviceRollReactorEditor : ArdunityObjectEditor
{
    bool foldout = false;
    
    SerializedProperty script;
    SerializedProperty invert;
    SerializedProperty OnActive;
    SerializedProperty OnDeactive;
    
	void OnEnable()
	{
        script = serializedObject.FindProperty("m_Script");
        invert = serializedObject.FindProperty("invert");
        OnActive = serializedObject.FindProperty("OnActive");
        OnDeactive = serializedObject.FindProperty("OnDeactive");
	}
	
	public override void OnInspectorGUI()
	{
		this.serializedObject.Update();
		
		//DeviceRollReactor reactor = (DeviceRollReactor)target;
        
        GUI.enabled = false;
        EditorGUILayout.PropertyField(script, true, new GUILayoutOption[0]);
        GUI.enabled = true;
        EditorGUILayout.PropertyField(invert, new GUIContent("invert"));
        
        foldout = EditorGUILayout.Foldout(foldout, "Events");
		if(foldout)
		{
			EditorGUI.indentLevel++;
			EditorGUILayout.PropertyField(OnActive, new GUIContent("OnActive"));
			EditorGUILayout.PropertyField(OnDeactive, new GUIContent("OnDeactive"));
			EditorGUI.indentLevel--;
		}

		this.serializedObject.ApplyModifiedProperties();
	}
	
	static public void AddMenuItem(GenericMenu menu, GenericMenu.MenuFunction2 func)
	{
		string menuName = "Unity/Add Reactor/Device/DeviceRollReactor";
		
		if(Selection.activeGameObject != null)
			menu.AddItem(new GUIContent(menuName), false, func, typeof(DeviceRollReactor));
		else
			menu.AddDisabledItem(new GUIContent(menuName));
	}
}