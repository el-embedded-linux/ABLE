using UnityEngine;
using UnityEditor;
using Ardunity;


[CustomEditor(typeof(AxisInput))]
public class AxisInputEditor : ArdunityObjectEditor
{
    SerializedProperty script;
	SerializedProperty minCenterHorizontal;
	SerializedProperty maxCenterHorizontal;
	SerializedProperty minCenterVertical;
	SerializedProperty maxCenterVertical;
    SerializedProperty invertHorizontal;
	SerializedProperty invertVertical;
    
	void OnEnable()
	{
        script = serializedObject.FindProperty("m_Script");
		minCenterHorizontal = serializedObject.FindProperty("minCenterHorizontal");
		maxCenterHorizontal = serializedObject.FindProperty("maxCenterHorizontal");
		minCenterVertical = serializedObject.FindProperty("minCenterVertical");
		maxCenterVertical = serializedObject.FindProperty("maxCenterVertical");
        invertHorizontal = serializedObject.FindProperty("invertHorizontal");
		invertVertical = serializedObject.FindProperty("invertVertical");
	}
	
	public override void OnInspectorGUI()
	{
		this.serializedObject.Update();
		
		AxisInput bridge = (AxisInput)target;
        
        GUI.enabled = false;
        EditorGUILayout.PropertyField(script, true, new GUILayoutOption[0]);
        GUI.enabled = true;
		EditorGUILayout.PropertyField(minCenterHorizontal, new GUIContent("minCenterHorizontal"));
		EditorGUILayout.PropertyField(maxCenterHorizontal, new GUIContent("maxCenterHorizontal"));
		EditorGUILayout.PropertyField(minCenterVertical, new GUIContent("minCenterVertical"));
		EditorGUILayout.PropertyField(maxCenterVertical, new GUIContent("maxCenterVertical"));
		EditorGUILayout.PropertyField(invertHorizontal, new GUIContent("invertHorizontal"));
        EditorGUILayout.PropertyField(invertVertical, new GUIContent("invertVertical"));

		if(Application.isPlaying && bridge.enabled)
		{
			EditorGUILayout.Vector2Field("Input Axis", bridge.axis);
			EditorUtility.SetDirty(target);
		}

		this.serializedObject.ApplyModifiedProperties();
	}
	
	static public void AddMenuItem(GenericMenu menu, GenericMenu.MenuFunction2 func)
	{
		string menuName = "Unity/Add Bridge/Input/AxisInput";
		
		if(Selection.activeGameObject != null)
			menu.AddItem(new GUIContent(menuName), false, func, typeof(AxisInput));
		else
			menu.AddDisabledItem(new GUIContent(menuName));
	}
}