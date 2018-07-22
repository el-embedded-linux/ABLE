using UnityEngine;
using UnityEditor;
using Ardunity;


[CustomEditor(typeof(DHT11))]
public class DHT11Editor : ArdunityObjectEditor
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
		
		DHT11 controller = (DHT11)target;
        
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
		
		controller.enableUpdate = EditorGUILayout.Toggle("Enable update", controller.enableUpdate);

		EditorGUILayout.LabelField(string.Format("Humidity: {0:d}", controller.humidity));
		EditorGUILayout.LabelField(string.Format("Temperature: {0:d}", controller.temperature));
			
		if(Application.isPlaying && controller.enableUpdate)
			EditorUtility.SetDirty(target);
		
		this.serializedObject.ApplyModifiedProperties();
	}
	
	static public void AddMenuItem(GenericMenu menu, GenericMenu.MenuFunction2 func)
	{
		string menuName = "ARDUINO/Add Controller/Sensor/DHT11";
		
		if(Selection.activeGameObject != null)
			menu.AddItem(new GUIContent(menuName), false, func, typeof(DHT11));
		else
			menu.AddDisabledItem(new GUIContent(menuName));
	}
}
