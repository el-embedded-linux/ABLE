using UnityEngine;
using UnityEditor;
using Ardunity;
using UnityEngine.UI;


[CustomEditor(typeof(ListItem))]
public class ListItemEditor : Editor
{
	[MenuItem("ARDUnity/Add Utility/UI/ListItem", true)]
	static bool ValidateMenu()
	{
		if(Selection.activeGameObject == null)
			return false;
		
		if(Selection.activeGameObject.GetComponent<Toggle>() == null)
			return false;
			
		return true;
	}
	[MenuItem("ARDUnity/Add Utility/UI/ListItem", false, 10)]
    static void DoMenu()
    {
        Selection.activeGameObject.AddComponent<ListItem>();
    }
	
	SerializedProperty script;
	SerializedProperty image;
	SerializedProperty textList;
	
	void OnEnable()
	{
        script = serializedObject.FindProperty("m_Script");
		image = serializedObject.FindProperty("image");
		textList = serializedObject.FindProperty("textList");
	}
	
	public override void OnInspectorGUI()
	{
		this.serializedObject.Update();
		
//		ListItem utility = (ListItem)target;
		
        GUI.enabled = false;
        EditorGUILayout.PropertyField(script, true, new GUILayoutOption[0]);
        GUI.enabled = true;
		EditorGUILayout.PropertyField(image, new GUIContent("image"));
		EditorGUILayout.PropertyField(textList, new GUIContent("textList"), true);
	
		this.serializedObject.ApplyModifiedProperties();
	}
}
