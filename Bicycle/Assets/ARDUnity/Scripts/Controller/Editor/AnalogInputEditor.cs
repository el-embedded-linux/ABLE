using UnityEngine;
using UnityEditor;
using Ardunity;


[CustomEditor(typeof(AnalogInput))]
public class AnalogInputEditor : ArdunityObjectEditor
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
		
		AnalogInput controller = (AnalogInput)target;
        
        GUI.enabled = false;
        EditorGUILayout.PropertyField(script, true, new GUILayoutOption[0]);
        GUI.enabled = true;
		foldout = EditorGUILayout.Foldout(foldout, "Sketch Options");
		if(foldout)
		{
			EditorGUI.indentLevel++;
			EditorGUILayout.PropertyField(id, new GUIContent("id"));
			EditorGUILayout.PropertyField(pin, new GUIContent("pin(A_)"));
			EditorGUI.indentLevel--;
		}
		
		controller.enableUpdate = EditorGUILayout.Toggle("Enable update", controller.enableUpdate);

		EditorGUILayout.Slider("Value", controller.Value, 0f, 1f);
			
		if(Application.isPlaying && controller.enableUpdate)
			EditorUtility.SetDirty(target);
		
		this.serializedObject.ApplyModifiedProperties();
	}
	
	static public void AddMenuItem(GenericMenu menu, GenericMenu.MenuFunction2 func)
	{
		string menuName = "ARDUINO/Add Controller/Basic/AnalogInput";
		
		if(Selection.activeGameObject != null)
			menu.AddItem(new GUIContent(menuName), false, func, typeof(AnalogInput));
		else
			menu.AddDisabledItem(new GUIContent(menuName));
	}
}
