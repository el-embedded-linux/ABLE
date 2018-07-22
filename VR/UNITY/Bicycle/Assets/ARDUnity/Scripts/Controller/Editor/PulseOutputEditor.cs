using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using Ardunity;


[CustomEditor(typeof(PulseOutput))]
public class PulseOutputEditor : ArdunityObjectEditor
{
	bool foldout = false;

    SerializedProperty script;
	SerializedProperty id;
	SerializedProperty pin;
	SerializedProperty defaultValue;
	SerializedProperty setTime;
	SerializedProperty delayTime;

	void OnEnable()
	{
        script = serializedObject.FindProperty("m_Script");
		id = serializedObject.FindProperty("id");
		pin = serializedObject.FindProperty("pin");
		defaultValue = serializedObject.FindProperty("defaultValue");
		setTime = serializedObject.FindProperty("setTime");
		delayTime = serializedObject.FindProperty("delayTime");
	}

	public override void OnInspectorGUI()
	{
		this.serializedObject.Update();
		
		PulseOutput controller = (PulseOutput)target;
        
        GUI.enabled = false;
        EditorGUILayout.PropertyField(script, true, new GUILayoutOption[0]);
        GUI.enabled = true;
		foldout = EditorGUILayout.Foldout(foldout, "Sketch Options");
		if(foldout)
		{
			EditorGUI.indentLevel++;
			EditorGUILayout.PropertyField(id, new GUIContent("id"));
			EditorGUILayout.PropertyField(pin, new GUIContent("pin"));
			EditorGUILayout.PropertyField(defaultValue, new GUIContent("defaultValue"));
			EditorGUI.indentLevel--;
		}

		EditorGUILayout.PropertyField(setTime, new GUIContent("Set Time(ms)"));
		EditorGUILayout.PropertyField(delayTime, new GUIContent("Delay Time(ms)"));

		if(Application.isPlaying && controller.connected)
		{
			if(controller.Active)
			{
				if(GUILayout.Button("Deactivate"))
					controller.Active = false;
			}
			else
			{
				if(GUILayout.Button("Activate"))
					controller.Active = true;
				
				if(GUILayout.Button("One Shot"))
					controller.OneShot();
			}
		}
		
		this.serializedObject.ApplyModifiedProperties();
	}
	
	static public void AddMenuItem(GenericMenu menu, GenericMenu.MenuFunction2 func)
	{
		string menuName = "ARDUINO/Add Controller/Basic/PulseOutput";
		
		if(Selection.activeGameObject != null)
			menu.AddItem(new GUIContent(menuName), false, func, typeof(PulseOutput));
		else
			menu.AddDisabledItem(new GUIContent(menuName));
	}
}
