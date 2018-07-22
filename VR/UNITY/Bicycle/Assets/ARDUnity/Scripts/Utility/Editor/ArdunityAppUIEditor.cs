using UnityEngine;
using UnityEditor;
using Ardunity;


[CustomEditor(typeof(ArdunityAppUI))]
public class ArdunityAppUIEditor : Editor
{
	[MenuItem("ARDUnity/Add Utility/UI/ArdunityAppUI", true)]
	static bool ValidateMenu()
	{
		if(Selection.activeGameObject == null)
			return false;
			
		return true;
	}
	[MenuItem("ARDUnity/Add Utility/UI/ArdunityAppUI", false, 10)]
    static void DoMenu()
    {
        Selection.activeGameObject.AddComponent<ArdunityAppUI>();
    }
	
	SerializedProperty script;
	SerializedProperty ardunityApp;
	SerializedProperty commSocketUI;
	SerializedProperty connect;
	SerializedProperty disconnect;
	SerializedProperty quit;
	SerializedProperty messageCanvas;
	SerializedProperty msgConnecting;
	SerializedProperty msgConnectionFailed;
	SerializedProperty msgLostConnection;
	SerializedProperty okConnectionFailed;
	SerializedProperty okLostConnection;
	
	void OnEnable()
	{
        script = serializedObject.FindProperty("m_Script");
		ardunityApp = serializedObject.FindProperty("ardunityApp");
		commSocketUI = serializedObject.FindProperty("commSocketUI");
		connect = serializedObject.FindProperty("connect");
		disconnect = serializedObject.FindProperty("disconnect");
		quit = serializedObject.FindProperty("quit");
		messageCanvas = serializedObject.FindProperty("messageCanvas");
		msgConnecting = serializedObject.FindProperty("msgConnecting");
		msgConnectionFailed = serializedObject.FindProperty("msgConnectionFailed");
		msgLostConnection = serializedObject.FindProperty("msgLostConnection");
		okConnectionFailed = serializedObject.FindProperty("okConnectionFailed");
		okLostConnection = serializedObject.FindProperty("okLostConnection");
	}
	
	public override void OnInspectorGUI()
	{
		this.serializedObject.Update();
		
		//ArdunityAppUI utility = (ArdunityAppUI)target;
		
        GUI.enabled = false;
        EditorGUILayout.PropertyField(script, true, new GUILayoutOption[0]);
        GUI.enabled = true;
		EditorGUILayout.PropertyField(ardunityApp, new GUIContent("ardunityApp"));
		EditorGUILayout.PropertyField(commSocketUI, new GUIContent("commSocketUI"));
		EditorGUILayout.PropertyField(connect, new GUIContent("connect"));		
		EditorGUILayout.PropertyField(disconnect, new GUIContent("disconnect"));
		EditorGUILayout.PropertyField(quit, new GUIContent("quit"));
		EditorGUILayout.PropertyField(messageCanvas, new GUIContent("messageCanvas"));
		EditorGUILayout.PropertyField(msgConnecting, new GUIContent("msgConnecting"));
		EditorGUILayout.PropertyField(msgConnectionFailed, new GUIContent("msgConnectionFailed"));
		EditorGUILayout.PropertyField(msgLostConnection, new GUIContent("msgLostConnection"));
		EditorGUILayout.PropertyField(okConnectionFailed, new GUIContent("okConnectionFailed"));
		EditorGUILayout.PropertyField(okLostConnection, new GUIContent("okLostConnection"));
		
		this.serializedObject.ApplyModifiedProperties();
	}
}