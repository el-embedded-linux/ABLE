using UnityEngine;
using UnityEditor;
using Ardunity;


[CustomEditor(typeof(ColorInput))]
public class ColorInputEditor : ArdunityObjectEditor
{
    SerializedProperty script;
    
	void OnEnable()
	{
        script = serializedObject.FindProperty("m_Script");
	}
	
	public override void OnInspectorGUI()
	{
		this.serializedObject.Update();
		
		ColorInput bridge = (ColorInput)target;
		
        GUI.enabled = false;
        EditorGUILayout.PropertyField(script, true, new GUILayoutOption[0]);
        GUI.enabled = true;
		EditorGUILayout.ColorField("Color", bridge.color);
		
		if(Application.isPlaying)
			EditorUtility.SetDirty(target);

		this.serializedObject.ApplyModifiedProperties();
	}
	
	static public void AddMenuItem(GenericMenu menu, GenericMenu.MenuFunction2 func)
	{
		string menuName = "Unity/Add Bridge/Input/ColorInput";
		
		if(Selection.activeGameObject != null)
			menu.AddItem(new GUIContent(menuName), false, func, typeof(ColorInput));
		else
			menu.AddDisabledItem(new GUIContent(menuName));
	}
}