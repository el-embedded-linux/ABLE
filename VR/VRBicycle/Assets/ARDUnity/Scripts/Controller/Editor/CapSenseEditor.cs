using UnityEngine;
using UnityEditor;
using Ardunity;


[CustomEditor(typeof(CapSense))]
public class CapSenseEditor : ArdunityObjectEditor
{
	bool foldout = false;

	SerializedProperty script;
	SerializedProperty id;
	SerializedProperty send;
	SerializedProperty receive;
	SerializedProperty threshold;

	void OnEnable()
	{
		script = serializedObject.FindProperty("m_Script");
		id = serializedObject.FindProperty("id");
		send = serializedObject.FindProperty("send");
		receive = serializedObject.FindProperty("receive");
		threshold = serializedObject.FindProperty("threshold");
	}

	public override void OnInspectorGUI()
	{
		this.serializedObject.Update();

		CapSense controller = (CapSense)target;

		GUI.enabled = false;
		EditorGUILayout.PropertyField(script, true, new GUILayoutOption[0]);
		GUI.enabled = true;
		foldout = EditorGUILayout.Foldout(foldout, "Sketch Options");
		if(foldout)
		{
			EditorGUI.indentLevel++;
			EditorGUILayout.PropertyField(id, new GUIContent("id"));
			EditorGUILayout.PropertyField(send, new GUIContent("send"));
			EditorGUILayout.PropertyField(receive, new GUIContent("receive"));
			EditorGUI.indentLevel--;
		}

		controller.enableUpdate = EditorGUILayout.Toggle("Enable update", controller.enableUpdate);
		EditorGUILayout.LabelField(string.Format("Raw Value: {0:d}", controller.RawValue));
		EditorGUILayout.PropertyField(threshold, new GUIContent("Threshold"));

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
		string menuName = "ARDUINO/Add Controller/Sensor/CapSense";

		if(Selection.activeGameObject != null)
			menu.AddItem(new GUIContent(menuName), false, func, typeof(CapSense));
		else
			menu.AddDisabledItem(new GUIContent(menuName));
	}
}
