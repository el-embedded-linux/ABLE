using UnityEngine;
using UnityEditor;
using Ardunity;


[CustomEditor(typeof(MPUSeries))]
public class MPUSeriesEditor : ArdunityObjectEditor
{
	bool foldout = false;
	bool foldout2 = false;

	SerializedProperty script;
	SerializedProperty id;
	SerializedProperty model;
	SerializedProperty secondary;
	SerializedProperty forward;
	SerializedProperty up;
	SerializedProperty OnStartCalibration;
	SerializedProperty OnCalibrated;

	void OnEnable()
	{
		script = serializedObject.FindProperty("m_Script");
		id = serializedObject.FindProperty("id");
		model = serializedObject.FindProperty("model");
		secondary = serializedObject.FindProperty("secondary");
		forward = serializedObject.FindProperty("forward");
		up = serializedObject.FindProperty("up");
		OnStartCalibration = serializedObject.FindProperty("OnStartCalibration");
		OnCalibrated = serializedObject.FindProperty("OnCalibrated");
	}

	public override void OnInspectorGUI()
	{
		this.serializedObject.Update();
		
		MPUSeries controller = (MPUSeries)target;
		
		GUI.enabled = false;
        EditorGUILayout.PropertyField(script, true, new GUILayoutOption[0]);
        GUI.enabled = true;
		foldout = EditorGUILayout.Foldout(foldout, "Sketch Options");
		if(foldout == true)
		{
			EditorGUI.indentLevel++;
			EditorGUILayout.PropertyField(id, new GUIContent("id"));
			EditorGUILayout.PropertyField(model, new GUIContent("model"));
			EditorGUILayout.PropertyField(secondary, new GUIContent("secondary"));
			EditorGUILayout.PropertyField(forward, new GUIContent("forward"));
			EditorGUILayout.PropertyField(up, new GUIContent("up"));
			EditorGUI.indentLevel--;
		}

		controller.enableUpdate = EditorGUILayout.Toggle("Enable update", controller.enableUpdate);

		foldout2 = EditorGUILayout.Foldout(foldout2, "Events");
		if(foldout2 == true)
		{
			EditorGUILayout.PropertyField(OnStartCalibration, new GUIContent("OnStartCalibration"));
			EditorGUILayout.PropertyField(OnCalibrated, new GUIContent("OnCalibrated"));
		}

		if(Application.isPlaying)
		{
			if(controller.connected)
			{
				if(controller.calibrating)
				{
					EditorGUILayout.HelpBox("Now MPU is calibrating...\nPlease, lay down the device on the flat surface.", MessageType.Info);
				}
				else
				{
					if(GUILayout.Button("Calibration"))
						controller.Calibration();
				}
			}

			EditorUtility.SetDirty(target);
		}
   
		this.serializedObject.ApplyModifiedProperties();
	}
	
	static public void AddMenuItem(GenericMenu menu, GenericMenu.MenuFunction2 func)
	{
		string menuName = "ARDUINO/Add Controller/AHRS/MPUSeries";
		
		if(Selection.activeGameObject != null)
			menu.AddItem(new GUIContent(menuName), false, func, typeof(MPUSeries));
		else
			menu.AddDisabledItem(new GUIContent(menuName));
	}
}
