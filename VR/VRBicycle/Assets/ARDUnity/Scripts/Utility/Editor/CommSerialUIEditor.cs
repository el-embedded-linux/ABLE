using UnityEngine;
using UnityEditor;
using Ardunity;


[CustomEditor(typeof(CommSerialUI))]
public class CommSerialUIEditor : Editor
{
	[MenuItem("ARDUnity/Add Utility/UI/CommSerialUI", true)]
	static bool ValidateMenu()
	{
		if(Selection.activeGameObject == null)
			return false;
			
		return true;
	}
	[MenuItem("ARDUnity/Add Utility/UI/CommSerialUI", false, 10)]
    static void DoMenu()
    {
        Selection.activeGameObject.AddComponent<CommSerialUI>();
    }
	
	SerializedProperty script;
	SerializedProperty popupCanvas;
	SerializedProperty settingCommSocket;
	SerializedProperty ok;
	SerializedProperty cancel;
	SerializedProperty commSerial;
	SerializedProperty portList;
	
	void OnEnable()
	{
        script = serializedObject.FindProperty("m_Script");
		popupCanvas = serializedObject.FindProperty("popupCanvas");
		settingCommSocket = serializedObject.FindProperty("settingCommSocket");
		ok = serializedObject.FindProperty("ok");
		cancel = serializedObject.FindProperty("cancel");
		commSerial = serializedObject.FindProperty("commSerial");
		portList = serializedObject.FindProperty("portList");
	}
	
	public override void OnInspectorGUI()
	{
		this.serializedObject.Update();
		
		//CommSerialUI utility = (CommSerialUI)target;
		
        GUI.enabled = false;
        EditorGUILayout.PropertyField(script, true, new GUILayoutOption[0]);
        GUI.enabled = true;
		EditorGUILayout.PropertyField(commSerial, new GUIContent("commSerial"));
		EditorGUILayout.PropertyField(popupCanvas, new GUIContent("popupCanvas"));
		EditorGUILayout.PropertyField(settingCommSocket, new GUIContent("settingCommSocket"));		
		EditorGUILayout.PropertyField(portList, new GUIContent("portList"));
		EditorGUILayout.PropertyField(ok, new GUIContent("ok"));
		EditorGUILayout.PropertyField(cancel, new GUIContent("cancel"));

		this.serializedObject.ApplyModifiedProperties();
	}
}