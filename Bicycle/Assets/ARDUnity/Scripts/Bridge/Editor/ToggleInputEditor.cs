using UnityEngine;
using UnityEditor;
using Ardunity;

[CustomEditor(typeof(ToggleInput))]
public class ToggleInputEditor : ArdunityObjectEditor
{
    SerializedProperty script;
	SerializedProperty checkEdge;

	void OnEnable()
	{
        script = serializedObject.FindProperty("m_Script");
		checkEdge = serializedObject.FindProperty("checkEdge");
	}

	public override void OnInspectorGUI()
	{
		this.serializedObject.Update();
		
		ToggleInput bridge = (ToggleInput)target;

        GUI.enabled = false;
        EditorGUILayout.PropertyField(script, true, new GUILayoutOption[0]);
        GUI.enabled = true;
		EditorGUILayout.PropertyField(checkEdge, new GUIContent("checkEdge"));

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Value", GUILayout.Width(80f));
		int index = 0;
		if(bridge.Value == true)
			index = 1;
		GUILayout.SelectionGrid(index, new string[] {"FALSE", "TRUE"}, 2);
		EditorGUILayout.EndHorizontal();
	
		if(Application.isPlaying == true)
			EditorUtility.SetDirty(target);
		
		this.serializedObject.ApplyModifiedProperties();
	}
	
	static public void AddMenuItem(GenericMenu menu, GenericMenu.MenuFunction2 func)
	{
		string menuName = "Unity/Add Bridge/Input/ToggleInput";
		
		if(Selection.activeGameObject != null)
			menu.AddItem(new GUIContent(menuName), false, func, typeof(ToggleInput));
		else
			menu.AddDisabledItem(new GUIContent(menuName));
	}
}
