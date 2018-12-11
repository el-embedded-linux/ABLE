using UnityEngine;
using UnityEditor;
using Ardunity;


[CustomEditor(typeof(InputFilter))]
public class InputFilterEditor : ArdunityObjectEditor
{
    SerializedProperty script;
	SerializedProperty invert;
	SerializedProperty amplify;
	SerializedProperty minValue;
	SerializedProperty maxValue;
	SerializedProperty smooth;
	SerializedProperty sensitivity;
	
	
	void OnEnable()
	{
        script = serializedObject.FindProperty("m_Script");
		invert = serializedObject.FindProperty("invert");
		amplify = serializedObject.FindProperty("amplify");
		minValue = serializedObject.FindProperty("minValue");
		maxValue = serializedObject.FindProperty("maxValue");
		smooth = serializedObject.FindProperty("smooth");
		sensitivity = serializedObject.FindProperty("sensitivity");
	}
	
	public override void OnInspectorGUI()
	{
		this.serializedObject.Update();
		
		InputFilter bridge = (InputFilter)target;

        GUI.enabled = false;
        EditorGUILayout.PropertyField(script, true, new GUILayoutOption[0]);
        GUI.enabled = true;
		EditorGUILayout.PropertyField(invert, new GUIContent("invert"));
		EditorGUILayout.PropertyField(amplify, new GUIContent("amplify"));
		if(bridge.amplify)
		{
			EditorGUI.indentLevel++;
			EditorGUILayout.PropertyField(minValue, new GUIContent("minValue"));
			EditorGUILayout.PropertyField(maxValue, new GUIContent("maxValue"));
			EditorGUI.indentLevel--;
		}		
		EditorGUILayout.PropertyField(smooth, new GUIContent("smooth"));
		if(bridge.smooth)
		{
			EditorGUI.indentLevel++;
			EditorGUILayout.PropertyField(sensitivity, new GUIContent("sensitivity"));
			EditorGUI.indentLevel--;
		}
		
		if(Application.isPlaying)
		{
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Source Curve");
			AnimationCurve curve = new AnimationCurve();
			float[] values = bridge.sourceValues;
			if(values.Length > 0)
			{
				curve.AddKey(0f, 0f);
				curve.AddKey(0.1f, 1f);
				for(int i=0; i<values.Length; i++)
					curve.AddKey(0.1f * (i + 2), values[i]);
				
				for(int i=0; i<curve.length; i++)
					curve.SmoothTangents(i, 0f);
			}
			EditorGUILayout.CurveField(curve, GUILayout.Height(120));
			EditorGUI.indentLevel++;
			EditorGUILayout.LabelField(string.Format("Range: {0:f2} ~ {1:f2}", bridge.minSourceValue, bridge.maxSourceValue));
			EditorGUI.indentLevel--;
			
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Result Curve");
			curve = new AnimationCurve();
			values = bridge.resultValues;
			if(values.Length > 0)
			{
				curve.AddKey(0f, 0f);
				curve.AddKey(0.1f, 1f);
				for(int i=0; i<values.Length; i++)
					curve.AddKey(0.1f * (i + 2), values[i]);
				
				for(int i=0; i<curve.length; i++)
					curve.SmoothTangents(i, 0f);
			}
			EditorGUILayout.CurveField(curve, GUILayout.Height(120));
			EditorGUI.indentLevel++;
			EditorGUILayout.LabelField(string.Format("Value: {0:f2}", bridge.Value));
			EditorGUI.indentLevel--;		
			
			if(GUILayout.Button("Reset"))
				bridge.ResetFilter();
			
			EditorUtility.SetDirty(target);
		}
		
		this.serializedObject.ApplyModifiedProperties();
	}
	
	static public void AddMenuItem(GenericMenu menu, GenericMenu.MenuFunction2 func)
	{
		string menuName = "Unity/Add Bridge/Input/InputFilter";
		
		if(Selection.activeGameObject != null)
			menu.AddItem(new GUIContent(menuName), false, func, typeof(InputFilter));
		else
			menu.AddDisabledItem(new GUIContent(menuName));
	}
}
