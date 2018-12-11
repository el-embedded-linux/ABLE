using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using Ardunity;


[CustomEditor(typeof(SliderSnap))]
public class SliderSnapEditor : Editor
{
	[MenuItem("ARDUnity/Add Utility/UI/SliderSnap", true)]
	static bool ValidateMenu()
	{
		if(Selection.activeGameObject == null)
			return false;
		
		if(Selection.activeGameObject.GetComponent<Slider>() == null)
			return false;
			
		return true;
	}
	[MenuItem("ARDUnity/Add Utility/UI/SliderSnap", false, 10)]
    static void DoMenu()
    {
        Selection.activeGameObject.AddComponent<SliderSnap>();
    }
	
	SerializedProperty script;
	SerializedProperty snapValue;
	
	void OnEnable()
	{
        script = serializedObject.FindProperty("m_Script");
		snapValue = serializedObject.FindProperty("snapValue");
	}
	
	public override void OnInspectorGUI()
	{
		this.serializedObject.Update();
		
		//SliderSnap utility = (SliderSnap)target;
		
        GUI.enabled = false;
        EditorGUILayout.PropertyField(script, true, new GUILayoutOption[0]);
        GUI.enabled = true;
		EditorGUILayout.PropertyField(snapValue, new GUIContent("snapValue"));
		
		this.serializedObject.ApplyModifiedProperties();
	}
}