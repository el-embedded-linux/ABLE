using UnityEngine;
using UnityEditor;
using Ardunity;


[CustomEditor(typeof(CommWifiUI))]
public class CommWiFiUIEditor : Editor
{
	[MenuItem("ARDUnity/Add Utility/UI/CommWifiUI", true)]
	static bool ValidateMenu()
	{
		if(Selection.activeGameObject == null)
			return false;
			
		return true;
	}
	[MenuItem("ARDUnity/Add Utility/UI/CommWifiUI", false, 10)]
    static void DoMenu()
    {
        Selection.activeGameObject.AddComponent<CommWifiUI>();
    }
	
	SerializedProperty script;
	SerializedProperty popupCanvas;
	SerializedProperty settingCommSocket;
	SerializedProperty ok;
	SerializedProperty cancel;
	SerializedProperty commWifi;
	SerializedProperty ipAddress;
	SerializedProperty port;
	
	void OnEnable()
	{
        script = serializedObject.FindProperty("m_Script");
		popupCanvas = serializedObject.FindProperty("popupCanvas");
		settingCommSocket = serializedObject.FindProperty("settingCommSocket");
		ok = serializedObject.FindProperty("ok");
		cancel = serializedObject.FindProperty("cancel");
		commWifi = serializedObject.FindProperty("commWifi");
		ipAddress = serializedObject.FindProperty("ipAddress");
		port = serializedObject.FindProperty("port");
	}
	
	public override void OnInspectorGUI()
	{
		this.serializedObject.Update();
		
		//CommWifiUI utility = (CommWifiUI)target;
		
        GUI.enabled = false;
        EditorGUILayout.PropertyField(script, true, new GUILayoutOption[0]);
        GUI.enabled = true;
		EditorGUILayout.PropertyField(commWifi, new GUIContent("commWifi"));
		EditorGUILayout.PropertyField(popupCanvas, new GUIContent("popupCanvas"));
		EditorGUILayout.PropertyField(settingCommSocket, new GUIContent("settingCommSocket"));
		EditorGUILayout.PropertyField(ipAddress, new GUIContent("ipAddress"));
		EditorGUILayout.PropertyField(port, new GUIContent("port"));
		EditorGUILayout.PropertyField(ok, new GUIContent("ok"));
		EditorGUILayout.PropertyField(cancel, new GUIContent("cancel"));

		this.serializedObject.ApplyModifiedProperties();
	}
}