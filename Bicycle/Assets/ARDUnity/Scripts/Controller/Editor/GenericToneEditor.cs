using UnityEngine;
using UnityEditor;
using Ardunity;


[CustomEditor(typeof(GenericTone))]
public class GenericToneEditor : ArdunityObjectEditor
{
	bool foldout = false;
    
    SerializedProperty script;
	SerializedProperty id;
	SerializedProperty pin;
	
	void OnEnable()
	{
        script = serializedObject.FindProperty("m_Script");
		id = serializedObject.FindProperty("id");
		pin = serializedObject.FindProperty("pin");
	}
	
	public override void OnInspectorGUI()
	{
		this.serializedObject.Update();
		
		GenericTone controller = (GenericTone)target;
	    
        GUI.enabled = false;
        EditorGUILayout.PropertyField(script, true, new GUILayoutOption[0]);
        GUI.enabled = true;
		foldout = EditorGUILayout.Foldout(foldout, "Sketch Options");
		if(foldout)
		{
			EditorGUI.indentLevel++;
			EditorGUILayout.PropertyField(id, new GUIContent("id"));
			EditorGUILayout.PropertyField(pin, new GUIContent("pin"));
			EditorGUI.indentLevel--;
		}

		controller.toneFrequency = (ToneFrequency)EditorGUILayout.EnumPopup("Tone Frequency", controller.toneFrequency);
		if(GUILayout.Button("Mute"))
			controller.toneFrequency = ToneFrequency.MUTE;

		this.serializedObject.ApplyModifiedProperties();
	}
	
	static public void AddMenuItem(GenericMenu menu, GenericMenu.MenuFunction2 func)
	{
		string menuName = "ARDUINO/Add Controller/Sound/GenericTone";
		
		if(Selection.activeGameObject != null)
			menu.AddItem(new GUIContent(menuName), false, func, typeof(GenericTone));
		else
			menu.AddDisabledItem(new GUIContent(menuName));
	}
}
