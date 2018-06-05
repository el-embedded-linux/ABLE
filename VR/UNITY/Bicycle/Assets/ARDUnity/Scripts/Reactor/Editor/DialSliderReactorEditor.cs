using UnityEngine;
using UnityEditor;
using Ardunity;


[CustomEditor(typeof(DialSliderReactor))]
public class DialSliderReactorEditor : ArdunityObjectEditor
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
		
		//DialSliderReactor reactor = (DialSliderReactor)target;
		
        GUI.enabled = false;
        EditorGUILayout.PropertyField(script, true, new GUILayoutOption[0]);
        GUI.enabled = true;
		EditorGUILayout.PropertyField(invert, new GUIContent("Invert"));

		this.serializedObject.ApplyModifiedProperties();
	}
	
	static public void AddMenuItem(GenericMenu menu, GenericMenu.MenuFunction2 func)
	{
		string menuName = "Unity/Add Reactor/UI/DialSliderReactor";
		
		if(Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<DialSlider>() != null)
			menu.AddItem(new GUIContent(menuName), false, func, typeof(DialSliderReactor));
		else
			menu.AddDisabledItem(new GUIContent(menuName));
	}
}