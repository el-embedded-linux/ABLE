using UnityEngine;
using UnityEditor;
using Ardunity;


[CustomEditor(typeof(HCSR04))]
public class HCSR04Editor : ArdunityObjectEditor
{
	bool foldout = false;

	SerializedProperty script;
	SerializedProperty id;
	SerializedProperty trig;
	SerializedProperty echo;

	void OnEnable()
	{
		script = serializedObject.FindProperty("m_Script");
		id = serializedObject.FindProperty("id");
		trig = serializedObject.FindProperty("trig");
		echo = serializedObject.FindProperty("echo");
	}

	public override void OnInspectorGUI()
	{
		this.serializedObject.Update();

		HCSR04 controller = (HCSR04)target;

		GUI.enabled = false;
		EditorGUILayout.PropertyField(script, true, new GUILayoutOption[0]);
		GUI.enabled = true;
		foldout = EditorGUILayout.Foldout(foldout, "Sketch Options");
		if(foldout)
		{
			EditorGUI.indentLevel++;
			EditorGUILayout.PropertyField(id, new GUIContent("id"));
			EditorGUILayout.PropertyField(trig, new GUIContent("trig"));
			EditorGUILayout.PropertyField(echo, new GUIContent("echo"));
			EditorGUI.indentLevel--;
		}

		controller.enableUpdate = EditorGUILayout.Toggle("Enable update", controller.enableUpdate);

		EditorGUILayout.LabelField(string.Format("Distance: {0:f1}cm", controller.distance));

		if(Application.isPlaying && controller.enableUpdate)
			EditorUtility.SetDirty(target);

		this.serializedObject.ApplyModifiedProperties();
	}

	static public void AddMenuItem(GenericMenu menu, GenericMenu.MenuFunction2 func)
	{
		string menuName = "ARDUINO/Add Controller/Sensor/HCSR04";

		if(Selection.activeGameObject != null)
			menu.AddItem(new GUIContent(menuName), false, func, typeof(HCSR04));
		else
			menu.AddDisabledItem(new GUIContent(menuName));
	}
}
