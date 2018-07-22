using UnityEngine;
using UnityEditor;
using Ardunity;

[CustomEditor(typeof(ImpulseInput))]
public class ImpulseInputEditor : ArdunityObjectEditor
{
    SerializedProperty script;
	SerializedProperty threshold;
	SerializedProperty delayTime;
	SerializedProperty clearTime;
	SerializedProperty impulseMode;
	SerializedProperty invert;
	SerializedProperty OnTriggerShot;

	void OnEnable()
	{
        script = serializedObject.FindProperty("m_Script");
		threshold = serializedObject.FindProperty("threshold");
		delayTime = serializedObject.FindProperty("delayTime");
		clearTime = serializedObject.FindProperty("clearTime");
		impulseMode = serializedObject.FindProperty("impulseMode");
		invert = serializedObject.FindProperty("invert");
		OnTriggerShot = serializedObject.FindProperty("OnTriggerShot");
	}

	public override void OnInspectorGUI()
	{
		this.serializedObject.Update();
		
		ImpulseInput bridge = (ImpulseInput)target;
        
        GUI.enabled = false;
        EditorGUILayout.PropertyField(script, true, new GUILayoutOption[0]);
        GUI.enabled = true;
		EditorGUILayout.PropertyField(threshold, new GUIContent("Threshold"));
		EditorGUILayout.PropertyField(delayTime, new GUIContent("Delay Time(sec)"));
		EditorGUILayout.PropertyField(clearTime, new GUIContent("Clear Time(sec)"));
		EditorGUILayout.PropertyField(impulseMode, new GUIContent("Impulse Mode"));
		EditorGUILayout.PropertyField(invert, new GUIContent("Invert"));

		if(Application.isPlaying)
		{
			EditorGUILayout.Slider("Impulse Value", bridge.Value, 0f, 1f);
			EditorUtility.SetDirty(target);
		}
		
		EditorGUILayout.Separator();
		EditorGUILayout.PropertyField(OnTriggerShot, new GUIContent("OnTriggerShot"));

		this.serializedObject.ApplyModifiedProperties();
	}
	
	static public void AddMenuItem(GenericMenu menu, GenericMenu.MenuFunction2 func)
	{
		string menuName = "Unity/Add Bridge/Input/ImpulseInput";
		
		if(Selection.activeGameObject != null)
			menu.AddItem(new GUIContent(menuName), false, func, typeof(ImpulseInput));
		else
			menu.AddDisabledItem(new GUIContent(menuName));
	}
}
