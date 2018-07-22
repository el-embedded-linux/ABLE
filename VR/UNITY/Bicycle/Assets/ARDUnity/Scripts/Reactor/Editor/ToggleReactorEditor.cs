using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using Ardunity;


[CustomEditor(typeof(ToggleReactor))]
public class ToggleReactorEditor : ArdunityObjectEditor
{
    SerializedProperty script;
    
	void OnEnable()
	{
        script = serializedObject.FindProperty("m_Script");
	}
	
	public override void OnInspectorGUI()
	{
		this.serializedObject.Update();
		
		//ToggleReactor reactor = (ToggleReactor)target;
        
        GUI.enabled = false;
        EditorGUILayout.PropertyField(script, true, new GUILayoutOption[0]);
        GUI.enabled = true;

		this.serializedObject.ApplyModifiedProperties();
	}
	
	static public void AddMenuItem(GenericMenu menu, GenericMenu.MenuFunction2 func)
	{
		string menuName = "Unity/Add Reactor/UI/ToggleReactor";
		
		if(Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<Toggle>() != null)
			menu.AddItem(new GUIContent(menuName), false, func, typeof(ToggleReactor));
		else
			menu.AddDisabledItem(new GUIContent(menuName));
	}
}