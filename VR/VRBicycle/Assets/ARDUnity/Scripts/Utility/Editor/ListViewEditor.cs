using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using Ardunity;


[CustomEditor(typeof(ListView))]
public class ListViewEditor : Editor
{
	[MenuItem("ARDUnity/Add Utility/UI/ListView", true)]
	static bool ValidateMenu()
	{
		if(Selection.activeGameObject == null)
			return false;
		
		if(Selection.activeGameObject.GetComponent<RectTransform>() == null)
			return false;
			
		return true;
	}
	[MenuItem("ARDUnity/Add Utility/UI/ListView", false, 10)]
    static void DoMenu()
    {
        Selection.activeGameObject.AddComponent<ListView>();
    }
	
	SerializedProperty script;
	SerializedProperty itemRoot;
	SerializedProperty OnSelectionChanged;
	
	void OnEnable()
	{
        script = serializedObject.FindProperty("m_Script");
		itemRoot = serializedObject.FindProperty("itemRoot");
		OnSelectionChanged = serializedObject.FindProperty("OnSelectionChanged");
	}
	
	public override void OnInspectorGUI()
	{
		this.serializedObject.Update();
		
//		ListView utility = (ListView)target;
		
        GUI.enabled = false;
        EditorGUILayout.PropertyField(script, true, new GUILayoutOption[0]);
        GUI.enabled = true;
		EditorGUILayout.PropertyField(itemRoot, new GUIContent("itemRoot"));
	
		EditorGUILayout.Separator();
		EditorGUILayout.PropertyField(OnSelectionChanged, new GUIContent("OnSelectionChanged"));
		
		this.serializedObject.ApplyModifiedProperties();
	}
}