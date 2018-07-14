using UnityEngine;
using UnityEditor;
using Ardunity;


[CustomEditor(typeof(GenericMotor))]
public class GenericMotorEditor : ArdunityObjectEditor
{
	bool foldout = false;

    SerializedProperty script;
	SerializedProperty id;
    SerializedProperty controlType;
	SerializedProperty pin1;
	SerializedProperty pin2;
    SerializedProperty pin3;
    SerializedProperty punchValue;
	
	void OnEnable()
	{
        script = serializedObject.FindProperty("m_Script");
		id = serializedObject.FindProperty("id");
        controlType = serializedObject.FindProperty("controlType");
		pin1 = serializedObject.FindProperty("pin1");
		pin2 = serializedObject.FindProperty("pin2");
        pin3 = serializedObject.FindProperty("pin3");
        punchValue = serializedObject.FindProperty("punchValue");
	}
	
	public override void OnInspectorGUI()
	{
		this.serializedObject.Update();
		
		GenericMotor controller = (GenericMotor)target;
		
        GUI.enabled = false;
        EditorGUILayout.PropertyField(script, true, new GUILayoutOption[0]);
        GUI.enabled = true;
		foldout = EditorGUILayout.Foldout(foldout, "Sketch Options");
		if(foldout)
		{
			EditorGUI.indentLevel++;
			EditorGUILayout.PropertyField(id, new GUIContent("id"));
            EditorGUILayout.PropertyField(controlType, new GUIContent("controlType"));
            if(controller.controlType == GenericMotor.ControlType.OnePWM_OneDir)
            {
                EditorGUILayout.PropertyField(pin1, new GUIContent("dir Pin"));
			    EditorGUILayout.PropertyField(pin2, new GUIContent("pwm Pin(~_)"));
            }
            else if(controller.controlType == GenericMotor.ControlType.TwoPWM)
            {
                EditorGUILayout.PropertyField(pin1, new GUIContent("F pwm Pin(~_)"));
			    EditorGUILayout.PropertyField(pin2, new GUIContent("B pwm Pin(~_)"));
            }
            else
            {
                EditorGUILayout.PropertyField(pin1, new GUIContent("pwm Pin(~_)"));
			    EditorGUILayout.PropertyField(pin2, new GUIContent("F dir Pin"));
                EditorGUILayout.PropertyField(pin3, new GUIContent("B dir Pin"));
            }
			EditorGUI.indentLevel--;
		}
        
        EditorGUILayout.PropertyField(punchValue, new GUIContent("punchValue"));
		controller.Value = EditorGUILayout.Slider("Value", controller.Value, -1f, 1f);
        if(GUILayout.Button("Stop"))
            controller.Value = 0f;

		this.serializedObject.ApplyModifiedProperties();
	}
	
	static public void AddMenuItem(GenericMenu menu, GenericMenu.MenuFunction2 func)
	{
		string menuName = "ARDUINO/Add Controller/Motor/GenericMotor";
		
		if(Selection.activeGameObject != null)
			menu.AddItem(new GUIContent(menuName), false, func, typeof(GenericMotor));
		else
			menu.AddDisabledItem(new GUIContent(menuName));
	}
}
