using UnityEngine;
using UnityEditor;
using Ardunity;


[CustomEditor(typeof(OutputCombine))]
public class OutputCombineEditor : ArdunityObjectEditor
{
    SerializedProperty script;
	SerializedProperty combineMode;

	void OnEnable()
	{
        script = serializedObject.FindProperty("m_Script");
		combineMode = serializedObject.FindProperty("combineMode");
	}

	public override void OnInspectorGUI()
	{
		this.serializedObject.Update();
		
		OutputCombine bridge = (OutputCombine)target;
        
        GUI.enabled = false;
        EditorGUILayout.PropertyField(script, true, new GUILayoutOption[0]);
        GUI.enabled = true;
		EditorGUILayout.PropertyField(combineMode, new GUIContent("combineMode"));
		
        if(Application.isPlaying)
		{
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
        
		this.serializedObject.ApplyModifiedProperties();
	}
	
	static public void AddMenuItem(GenericMenu menu, GenericMenu.MenuFunction2 func)
	{
		string menuName = "Unity/Add Bridge/Output/OutputCombine";
		
		if(Selection.activeGameObject != null)
			menu.AddItem(new GUIContent(menuName), false, func, typeof(OutputCombine));
		else
			menu.AddDisabledItem(new GUIContent(menuName));
	}
}
