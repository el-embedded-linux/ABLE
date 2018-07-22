using UnityEngine;
using UnityEditor;
using Ardunity;


[CustomEditor(typeof(DeviceRotationReactor))]
public class DeviceRotationReactorEditor : ArdunityObjectEditor
{
    bool foldout = false;
    
    SerializedProperty script;
    SerializedProperty OnActive;
    SerializedProperty OnDeactive;
    
	void OnEnable()
	{
        script = serializedObject.FindProperty("m_Script");
        OnActive = serializedObject.FindProperty("OnActive");
        OnDeactive = serializedObject.FindProperty("OnDeactive");
	}
	
	public override void OnInspectorGUI()
	{
		this.serializedObject.Update();
		
		//DeviceRotationReactor reactor = (DeviceRotationReactor)target;
        
        GUI.enabled = false;
        EditorGUILayout.PropertyField(script, true, new GUILayoutOption[0]);
        GUI.enabled = true;
    
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
		string menuName = "Unity/Add Reactor/Device/DeviceRotationReactor";
		
		if(Selection.activeGameObject != null)
			menu.AddItem(new GUIContent(menuName), false, func, typeof(DeviceRotationReactor));
		else
			menu.AddDisabledItem(new GUIContent(menuName));
	}
}