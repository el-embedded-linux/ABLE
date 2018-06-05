using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using Ardunity;


[CustomEditor(typeof(GenericStepper))]
public class GenericStepperEditor : ArdunityObjectEditor
{
	bool foldout = false;

    SerializedProperty script;
	SerializedProperty id;
	SerializedProperty step;
	SerializedProperty gearRatio;
    SerializedProperty driveType;
	SerializedProperty pin1;
	SerializedProperty pin2;
    SerializedProperty pin3;
    SerializedProperty pin4;
	
	void OnEnable()
	{
        script = serializedObject.FindProperty("m_Script");
		id = serializedObject.FindProperty("id");
		step = serializedObject.FindProperty("step");
		gearRatio = serializedObject.FindProperty("gearRatio");
        driveType = serializedObject.FindProperty("driveType");
		pin1 = serializedObject.FindProperty("pin1");
		pin2 = serializedObject.FindProperty("pin2");
        pin3 = serializedObject.FindProperty("pin3");
        pin4 = serializedObject.FindProperty("pin4");
	}
	
	public override void OnInspectorGUI()
	{
		this.serializedObject.Update();
		
		GenericStepper controller = (GenericStepper)target;
		
        GUI.enabled = false;
        EditorGUILayout.PropertyField(script, true, new GUILayoutOption[0]);
        GUI.enabled = true;
		foldout = EditorGUILayout.Foldout(foldout, "Sketch Options");
		if(foldout)
		{
			EditorGUI.indentLevel++;
			EditorGUILayout.PropertyField(id, new GUIContent("id"));
			EditorGUILayout.PropertyField(step, new GUIContent("Step"));
			EditorGUILayout.PropertyField(gearRatio, new GUIContent("Gear Ratio"));
            EditorGUILayout.PropertyField(driveType, new GUIContent("Drive Type"));
			EditorGUILayout.PropertyField(pin1, new GUIContent("Pin1"));
			EditorGUILayout.PropertyField(pin2, new GUIContent("Pin2"));
            EditorGUILayout.PropertyField(pin3, new GUIContent("Pin3"));
			EditorGUILayout.PropertyField(pin4, new GUIContent("Pin4"));
			EditorGUI.indentLevel--;
		}

		GenericStepper.ControlMode newMode = (GenericStepper.ControlMode)EditorGUILayout.EnumPopup("Control By", controller.mode);
		if(newMode != controller.mode)
		{
			controller.mode = newMode;
			if(!Application.isPlaying)
				EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
		}

		EditorGUI.indentLevel++;
		float newRPM = EditorGUILayout.FloatField("RPM", controller.rpm);
		if(newRPM != controller.rpm)
		{
			controller.rpm = newRPM;
			if(!Application.isPlaying)
				EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
		}

		if(controller.mode == GenericStepper.ControlMode.Angle)
		{
			float newAngle = EditorGUILayout.FloatField("Angle", controller.angle);
			if(newAngle != controller.angle)
			{
				controller.angle = newAngle;
				if(!Application.isPlaying)
					EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
			}
		}
		EditorGUI.indentLevel--;

		this.serializedObject.ApplyModifiedProperties();
	}
	
	static public void AddMenuItem(GenericMenu menu, GenericMenu.MenuFunction2 func)
	{
		string menuName = "ARDUINO/Add Controller/Motor/GenericStepper";
		
		if(Selection.activeGameObject != null)
			menu.AddItem(new GUIContent(menuName), false, func, typeof(GenericStepper));
		else
			menu.AddDisabledItem(new GUIContent(menuName));
	}
}
