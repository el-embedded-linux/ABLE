using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using Ardunity;


[CustomEditor(typeof(DropdownReactor))]
public class DropdownReactorEditor : ArdunityObjectEditor
{
    SerializedProperty script;
    
	void OnEnable()
	{
        script = serializedObject.FindProperty("m_Script");
	}
	
	public override void OnInspectorGUI()
	{
		this.serializedObject.Update();
		
		//DropdownReactor reactor = (DropdownReactor)target;
        
        GUI.enabled = false;
        EditorGUILayout.PropertyField(script, true, new GUILayoutOption[0]);
        GUI.enabled = true;

		this.serializedObject.ApplyModifiedProperties();
	}
	
	static public void AddMenuItem(GenericMenu menu, GenericMenu.MenuFunction2 func)
	{
		string menuName = "Unity/Add Reactor/UI/DropdownReactor";
		
		if(Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<Dropdown>() != null)
			menu.AddItem(new GUIContent(menuName), false, func, typeof(DropdownReactor));
		else
			menu.AddDisabledItem(new GUIContent(menuName));
	}
}