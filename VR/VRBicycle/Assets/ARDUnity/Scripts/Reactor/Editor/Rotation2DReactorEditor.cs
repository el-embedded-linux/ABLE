using UnityEngine;
using UnityEditor;
using Ardunity;


[CustomEditor(typeof(Rotation2DReactor))]
public class Rotation2DReactorEditor : ArdunityObjectEditor
{
    SerializedProperty script;
    SerializedProperty invert;
    
	void OnEnable()
	{
        script = serializedObject.FindProperty("m_Script");
        invert = serializedObject.FindProperty("invert");
	}
	
	public override void OnInspectorGUI()
	{
		this.serializedObject.Update();
		
		//Rotation2DReactor reactor = (Rotation2DReactor)target;
        
        GUI.enabled = false;
        EditorGUILayout.PropertyField(script, true, new GUILayoutOption[0]);
        GUI.enabled = true;
        EditorGUILayout.PropertyField(invert, new GUIContent("invert"));

		this.serializedObject.ApplyModifiedProperties();
	}
	
	static public void AddMenuItem(GenericMenu menu, GenericMenu.MenuFunction2 func)
	{
		string menuName = "Unity/Add Reactor/Transform/Rotation2DReactor";
		
		if(Selection.activeGameObject != null)
			menu.AddItem(new GUIContent(menuName), false, func, typeof(Rotation2DReactor));
		else
			menu.AddDisabledItem(new GUIContent(menuName));
	}
}