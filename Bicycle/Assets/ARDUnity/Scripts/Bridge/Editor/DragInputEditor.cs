using UnityEngine;
using UnityEditor;
using Ardunity;


[CustomEditor(typeof(DragInput))]
public class DragInputEditor : ArdunityObjectEditor
{
    SerializedProperty script;
	SerializedProperty minValue;
	SerializedProperty maxValue;
	SerializedProperty invert;
	SerializedProperty deltaMultiplier;
	SerializedProperty forceMultiplier;
	SerializedProperty OnDragStart;
	SerializedProperty OnDragEnd;
	
	
	void OnEnable()
	{
        script = serializedObject.FindProperty("m_Script");
		minValue = serializedObject.FindProperty("minValue");
		maxValue = serializedObject.FindProperty("maxValue");
		invert = serializedObject.FindProperty("invert");
		deltaMultiplier = serializedObject.FindProperty("deltaMultiplier");
		forceMultiplier = serializedObject.FindProperty("forceMultiplier");
		OnDragStart = serializedObject.FindProperty("OnDragStart");
		OnDragEnd = serializedObject.FindProperty("OnDragEnd");
	}
	
	public override void OnInspectorGUI()
	{
		this.serializedObject.Update();
		
		DragInput bridge = (DragInput)target;

        GUI.enabled = false;
        EditorGUILayout.PropertyField(script, true, new GUILayoutOption[0]);
        GUI.enabled = true;
		EditorGUILayout.PropertyField(minValue, new GUIContent("minValue"));
		EditorGUILayout.PropertyField(maxValue, new GUIContent("maxValue"));
		EditorGUILayout.PropertyField(invert, new GUIContent("invert"));
		EditorGUILayout.PropertyField(deltaMultiplier, new GUIContent("deltaMultiplier"));
		EditorGUILayout.PropertyField(forceMultiplier, new GUIContent("forceMultiplier"));
		
		if(Application.isPlaying)
		{
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("DragData");			
			EditorGUI.indentLevel++;
			DragData dragData = bridge.dragData;
			EditorGUILayout.Toggle("isDrag", dragData.isDrag);
			EditorGUILayout.FloatField("delta", dragData.delta);
			EditorGUILayout.FloatField("force", dragData.force);
			EditorGUI.indentLevel--;
			EditorGUILayout.FloatField("Value", bridge.Value);
			
			EditorUtility.SetDirty(target);
		}
		
		EditorGUILayout.Separator();
		EditorGUILayout.PropertyField(OnDragStart, new GUIContent("OnDragStart"));
		EditorGUILayout.PropertyField(OnDragEnd, new GUIContent("OnDragEnd"));
		
		this.serializedObject.ApplyModifiedProperties();
	}
	
	static public void AddMenuItem(GenericMenu menu, GenericMenu.MenuFunction2 func)
	{
		string menuName = "Unity/Add Bridge/Input/DragInput";
		
		if(Selection.activeGameObject != null)
			menu.AddItem(new GUIContent(menuName), false, func, typeof(DragInput));
		else
			menu.AddDisabledItem(new GUIContent(menuName));
	}
}
