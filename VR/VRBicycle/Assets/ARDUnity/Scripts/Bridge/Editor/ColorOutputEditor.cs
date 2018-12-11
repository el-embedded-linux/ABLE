using UnityEngine;
using UnityEditor;
using Ardunity;


[CustomEditor(typeof(ColorOutput))]
public class ColorOutputEditor : ArdunityObjectEditor
{
    SerializedProperty script;
	SerializedProperty color;
    
	void OnEnable()
	{
        script = serializedObject.FindProperty("m_Script");
		color = serializedObject.FindProperty("color");
	}
	
	public override void OnInspectorGUI()
	{
		this.serializedObject.Update();
		
//		ColorOutput bridge = (ColorOutput)target;
		
        GUI.enabled = false;
        EditorGUILayout.PropertyField(script, true, new GUILayoutOption[0]);
        GUI.enabled = true;

		EditorGUILayout.PropertyField(color, new GUIContent("Color"));

		this.serializedObject.ApplyModifiedProperties();
	}
	
	static public void AddMenuItem(GenericMenu menu, GenericMenu.MenuFunction2 func)
	{
		string menuName = "Unity/Add Bridge/Output/ColorOutput";
		
		if(Selection.activeGameObject != null)
			menu.AddItem(new GUIContent(menuName), false, func, typeof(ColorOutput));
		else
			menu.AddDisabledItem(new GUIContent(menuName));
	}
}