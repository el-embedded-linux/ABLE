using UnityEngine;
using UnityEditor;
using Ardunity;
using System.Collections.Generic;


[CustomEditor(typeof(CurveOutput))]
public class CurveOutputEditor : ArdunityObjectEditor
{
    bool foldout = false;
    bool foldout2 = false;
    bool foldout3 = false;
    
    SerializedProperty script;
    SerializedProperty outputName;
	SerializedProperty startCurve;
	SerializedProperty loopCurve;
	SerializedProperty endCurve;
	SerializedProperty multiplier;
	SerializedProperty speed;
	SerializedProperty loop;
    SerializedProperty OnStart;
    SerializedProperty OnStop;
	
	void OnEnable()
	{
        script = serializedObject.FindProperty("m_Script");
        outputName = serializedObject.FindProperty("outputName");
		startCurve = serializedObject.FindProperty("startCurve");
		loopCurve = serializedObject.FindProperty("loopCurve");
		endCurve = serializedObject.FindProperty("endCurve");
		multiplier = serializedObject.FindProperty("multiplier");
		speed = serializedObject.FindProperty("speed");
		loop = serializedObject.FindProperty("loop");
        OnStart = serializedObject.FindProperty("OnStart");
        OnStop = serializedObject.FindProperty("OnStop");
	}
	
	public override void OnInspectorGUI()
	{
		this.serializedObject.Update();
		
		CurveOutput bridge = (CurveOutput)target;

        GUI.enabled = false;
        EditorGUILayout.PropertyField(script, true, new GUILayoutOption[0]);
        GUI.enabled = true;
        EditorGUILayout.PropertyField(outputName, new GUIContent("outputName"));
        foldout = EditorGUILayout.Foldout(foldout, "Curves");
        if (foldout)
        {
            EditorGUILayout.PropertyField(startCurve, new GUIContent("start"));
            EditorGUILayout.PropertyField(loopCurve, new GUIContent("loop"));
            EditorGUILayout.PropertyField(endCurve, new GUIContent("end"));
        }
		
        foldout2 = EditorGUILayout.Foldout(foldout2, "Options");
        if (foldout2)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(multiplier, new GUIContent("multiplier"));
            EditorGUILayout.PropertyField(speed, new GUIContent("speed"));
            EditorGUILayout.PropertyField(loop, new GUIContent("loop"));
            EditorGUI.indentLevel--;
        }
		
		if(Application.isPlaying)
		{
			if(bridge.isPlaying)
			{
				if(GUILayout.Button("Stop"))
					bridge.Stop();
			}
			else
			{
				if(GUILayout.Button("Play"))
					bridge.Play();
			}
            
            Keyframe[] values = bridge.historyValues;                
            AnimationCurve curve = new AnimationCurve();
            for(int i=0; i<values.Length; i++)
            {
                Keyframe key = new Keyframe();
                key.time = values[i].time - values[0].time;
                key.value = values[i].value;
                if(i > 1)
                    key.inTangent = (values[i].value - values[i-2].value) / 0.1f;
                if(i < values.Length - 2)
                    key.outTangent = (values[i+2].value - values[i].value) / 0.1f;
                curve.AddKey(key);
            }
            EditorGUILayout.CurveField(curve, GUILayout.Height(120));
            EditorGUILayout.LabelField(string.Format("Value: {0:f2}", bridge.Value));
            
            EditorUtility.SetDirty(target);
		}
        
        foldout3 = EditorGUILayout.Foldout(foldout3, "Events");
        if (foldout3)
        {
            EditorGUILayout.PropertyField(OnStart, new GUIContent("OnStart"));
            EditorGUILayout.PropertyField(OnStop, new GUIContent("OnStop"));
        }
		
		this.serializedObject.ApplyModifiedProperties();
	}
	
	static public void AddMenuItem(GenericMenu menu, GenericMenu.MenuFunction2 func)
	{
		string menuName = "Unity/Add Bridge/Output/CurveOutput";
		
		if(Selection.activeGameObject != null)
			menu.AddItem(new GUIContent(menuName), false, func, typeof(CurveOutput));
		else
			menu.AddDisabledItem(new GUIContent(menuName));
	}
}
