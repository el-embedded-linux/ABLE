using UnityEngine;
using UnityEditor;
using Ardunity;


[CustomEditor(typeof(DigitalInput))]
public class DigitalInputEditor : ArdunityObjectEditor
{
	bool foldout = false;

    SerializedProperty script;
	SerializedProperty id;
	SerializedProperty pin;
	SerializedProperty pullup;

	void OnEnable()
	{
        script = serializedObject.FindProperty("m_Script");
		id = serializedObject.FindProperty("id");
		pin = serializedObject.FindProperty("pin");
		pullup = serializedObject.FindProperty("pullup");
	}

	public override void OnInspectorGUI()
	{
		this.serializedObject.Update();
		
		DigitalInput controller = (DigitalInput)target;

        GUI.enabled = false;
        EditorGUILayout.PropertyField(script, true, new GUILayoutOption[0]);
        GUI.enabled = true;
		foldout = EditorGUILayout.Foldout(foldout, "Sketch Options");
		if(foldout)
		{
			EditorGUI.indentLevel++;
			EditorGUILayout.PropertyField(id, new GUIContent("id"));
			EditorGUILayout.PropertyField(pin, new GUIContent("pin"));
			EditorGUILayout.PropertyField(pullup, new GUIContent("pullup"));
			EditorGUI.indentLevel--;
		}
		
		controller.enableUpdate = EditorGUILayout.Toggle("Enable update", controller.enableUpdate);

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Value", GUILayout.Width(80f));
		int index = 0;
		if(controller.Value)
			index = 1;
		GUILayout.SelectionGrid(index, new string[] {"FALSE", "TRUE"}, 2);
		EditorGUILayout.EndHorizontal();
			
		if(Application.isPlaying && controller.enableUpdate)
			EditorUtility.SetDirty(target);
		
		this.serializedObject.ApplyModifiedProperties();
	}
	
	static public void AddMenuItem(GenericMenu menu, GenericMenu.MenuFunction2 func)
	{
		string menuName = "ARDUINO/Add Controller/Basic/DigitalInput";
		
		if(Selection.activeGameObject != null)
			menu.AddItem(new GUIContent(menuName), false, func, typeof(DigitalInput));
		else
			menu.AddDisabledItem(new GUIContent(menuName));
	}
}
