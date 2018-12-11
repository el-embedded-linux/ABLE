using UnityEngine;
using UnityEditor;
using Ardunity;


[CustomEditor(typeof(CommBluetoothUI))]
public class CommBluetoothUIEditor : Editor
{
	[MenuItem("ARDUnity/Add Utility/UI/CommBluetoothUI", true)]
	static bool ValidateMenu()
	{
		if(Selection.activeGameObject == null)
			return false;
			
		return true;
	}
	[MenuItem("ARDUnity/Add Utility/UI/CommBluetoothUI", false, 10)]
    static void DoMenu()
    {
        Selection.activeGameObject.AddComponent<CommBluetoothUI>();
    }
	
	SerializedProperty script;
	SerializedProperty popupCanvas;
	SerializedProperty settingCommSocket;
	SerializedProperty ok;
	SerializedProperty cancel;
	SerializedProperty commBluetooth;
	SerializedProperty deviceList;
	SerializedProperty deviceItem;
	
	void OnEnable()
	{
        script = serializedObject.FindProperty("m_Script");
		popupCanvas = serializedObject.FindProperty("popupCanvas");
		settingCommSocket = serializedObject.FindProperty("settingCommSocket");
		ok = serializedObject.FindProperty("ok");
		cancel = serializedObject.FindProperty("cancel");
		commBluetooth = serializedObject.FindProperty("commBluetooth");
		deviceList = serializedObject.FindProperty("deviceList");
		deviceItem = serializedObject.FindProperty("deviceItem");
	}
	
	public override void OnInspectorGUI()
	{
		this.serializedObject.Update();
		
		//CommBluetoothUI utility = (CommBluetoothUI)target;
		
        GUI.enabled = false;
        EditorGUILayout.PropertyField(script, true, new GUILayoutOption[0]);
        GUI.enabled = true;
		EditorGUILayout.PropertyField(commBluetooth, new GUIContent("commBluetooth"));
		EditorGUILayout.PropertyField(popupCanvas, new GUIContent("popupCanvas"));
		EditorGUILayout.PropertyField(settingCommSocket, new GUIContent("settingCommSocket"));		
		EditorGUILayout.PropertyField(deviceList, new GUIContent("deviceList"));
		EditorGUILayout.PropertyField(deviceItem, new GUIContent("deviceItem"));
		EditorGUILayout.PropertyField(ok, new GUIContent("ok"));
		EditorGUILayout.PropertyField(cancel, new GUIContent("cancel"));

		this.serializedObject.ApplyModifiedProperties();
	}
}