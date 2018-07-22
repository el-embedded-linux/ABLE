using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using Ardunity;


[CustomEditor(typeof(MPR121))]
public class MPR121Editor : ArdunityObjectEditor
{
	bool foldout = false;

    SerializedProperty script;
	SerializedProperty id;

	void OnEnable()
	{
        script = serializedObject.FindProperty("m_Script");
		id = serializedObject.FindProperty("id");
	}

	public override void OnInspectorGUI()
	{
		this.serializedObject.Update();
		
		MPR121 controller = (MPR121)target;
        
        GUI.enabled = false;
        EditorGUILayout.PropertyField(script, true, new GUILayoutOption[0]);
        GUI.enabled = true;
		foldout = EditorGUILayout.Foldout(foldout, "Sketch Options");
		if(foldout)
		{
			EditorGUI.indentLevel++;
			EditorGUILayout.PropertyField(id, new GUIContent("id"));
			int oldIndex = Mathf.Clamp(controller.address - 0x5A, 0, 3);
			int newIndex = EditorGUILayout.Popup("Address:", oldIndex, new string[] { "0x5A", "0x5B", "0x5C", "0x5D" });
			if(oldIndex != newIndex)
			{
				controller.address = 0x5A + newIndex;
				if(!Application.isPlaying)
					EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
			}
			EditorGUI.indentLevel--;
		}
		
		controller.enableUpdate = EditorGUILayout.Toggle("Enable update", controller.enableUpdate);

		for(int i=0; i<12; i++)
		{
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField(string.Format("Ch{0:d}", i), GUILayout.Width(50f));
			int index = 0;
			if(controller.GetElectrodeState(i))
				index = 1;
			GUILayout.SelectionGrid(index, new string[] {"FALSE", "TRUE"}, 2);
			EditorGUILayout.EndHorizontal();
		}
			
		if(Application.isPlaying && controller.enableUpdate)
			EditorUtility.SetDirty(target);
		
		this.serializedObject.ApplyModifiedProperties();
	}
	
	static public void AddMenuItem(GenericMenu menu, GenericMenu.MenuFunction2 func)
	{
		string menuName = "ARDUINO/Add Controller/Sensor/MPR121";
		
		if(Selection.activeGameObject != null)
			menu.AddItem(new GUIContent(menuName), false, func, typeof(MPR121));
		else
			menu.AddDisabledItem(new GUIContent(menuName));
	}
}
