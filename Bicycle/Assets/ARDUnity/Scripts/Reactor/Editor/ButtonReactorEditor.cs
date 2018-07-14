using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using Ardunity;


[CustomEditor(typeof(ButtonReactor))]
public class ButtonReactorEditor : ArdunityObjectEditor
{
    SerializedProperty script;
    SerializedProperty checkEdge;
    
	void OnEnable()
	{
        script = serializedObject.FindProperty("m_Script");
        checkEdge = serializedObject.FindProperty("checkEdge");
	}
	
	public override void OnInspectorGUI()
	{
		this.serializedObject.Update();
		
		//ButtonReactor reactor = (ButtonReactor)target;
        
        GUI.enabled = false;
        EditorGUILayout.PropertyField(script, true, new GUILayoutOption[0]);
        GUI.enabled = true;
        EditorGUILayout.PropertyField(checkEdge, new GUIContent("checkEdge"));

		this.serializedObject.ApplyModifiedProperties();
	}
	
	static public void AddMenuItem(GenericMenu menu, GenericMenu.MenuFunction2 func)
	{
		string menuName = "Unity/Add Reactor/UI/ButtonReactor";
		
		if(Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<Button>() != null)
			menu.AddItem(new GUIContent(menuName), false, func, typeof(ButtonReactor));
		else
			menu.AddDisabledItem(new GUIContent(menuName));
	}
}