using UnityEngine;
using UnityEditor;
using Ardunity;


[CustomEditor(typeof(HM10UI))]
public class HM10UIEditor : Editor
{
	[MenuItem("ARDUnity/Add Utility/UI/HM10UI", true)]
	static bool ValidateMenu()
	{
		if(Selection.activeGameObject == null)
			return false;
			
		return true;
	}
	[MenuItem("ARDUnity/Add Utility/UI/HM10UI", false, 10)]
    static void DoMenu()
    {
        Selection.activeGameObject.AddComponent<HM10UI>();
    }
	
	
	SerializedProperty popupCanvas;
	SerializedProperty settingCommSocket;
	SerializedProperty ok;
	SerializedProperty cancel;
	SerializedProperty hm10;
	SerializedProperty deviceList;
	SerializedProperty deviceItem;
	
	void OnEnable()
	{
		popupCanvas = serializedObject.FindProperty("popupCanvas");
		settingCommSocket = serializedObject.FindProperty("settingCommSocket");
		ok = serializedObject.FindProperty("ok");
		cancel = serializedObject.FindProperty("cancel");
		hm10 = serializedObject.FindProperty("hm10");
		deviceList = serializedObject.FindProperty("deviceList");
		deviceItem = serializedObject.FindProperty("deviceItem");
	}
	
	public override void OnInspectorGUI()
	{
		this.serializedObject.Update();
		
		//HM10UI utility = (HM10UI)target;
		
		EditorGUILayout.PropertyField(hm10, new GUIContent("HM-10"));
		EditorGUILayout.PropertyField(popupCanvas, new GUIContent("popupCanvas"));
		EditorGUILayout.PropertyField(settingCommSocket, new GUIContent("settingCommSocket"));
		EditorGUILayout.PropertyField(deviceList, new GUIContent("deviceList"));
		EditorGUILayout.PropertyField(deviceItem, new GUIContent("deviceItem"));
		EditorGUILayout.PropertyField(ok, new GUIContent("ok"));
		EditorGUILayout.PropertyField(cancel, new GUIContent("cancel"));

		this.serializedObject.ApplyModifiedProperties();
	}
}