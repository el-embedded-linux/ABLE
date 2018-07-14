using UnityEngine;
using UnityEditor;
using Ardunity;


[CustomEditor(typeof(DialSlider))]
public class DialSliderEditor : Editor
{
	[MenuItem("ARDUnity/Add Utility/UI/DialSlider", true)]
	static bool ValidateMenu()
	{
		if(Selection.activeGameObject == null)
			return false;
		
		if(Selection.activeGameObject.GetComponent<RectTransform>() == null)
			return false;
			
		return true;
	}
	[MenuItem("ARDUnity/Add Utility/UI/DialSlider", false, 10)]
    static void DoMenu()
    {
        Selection.activeGameObject.AddComponent<DialSlider>();
    }
	
	SerializedProperty script;
	SerializedProperty knob;
	SerializedProperty minAngle;
	SerializedProperty maxAngle;
	SerializedProperty interactable;
	SerializedProperty spring;
	SerializedProperty OnDragStart;
	SerializedProperty OnDragEnd;
	
	void OnEnable()
	{
        script = serializedObject.FindProperty("m_Script");
		knob = serializedObject.FindProperty("knob");
		minAngle = serializedObject.FindProperty("minAngle");
		maxAngle = serializedObject.FindProperty("maxAngle");
		interactable = serializedObject.FindProperty("interactable");
		spring = serializedObject.FindProperty("spring");
		OnDragStart = serializedObject.FindProperty("OnDragStart");
		OnDragEnd = serializedObject.FindProperty("OnDragEnd");
	}
	
	public override void OnInspectorGUI()
	{
		this.serializedObject.Update();
		
		DialSlider utility = (DialSlider)target;
		
        GUI.enabled = false;
        EditorGUILayout.PropertyField(script, true, new GUILayoutOption[0]);
        GUI.enabled = true;
		EditorGUILayout.PropertyField(knob, new GUIContent("knob"));
		EditorGUILayout.PropertyField(minAngle, new GUIContent("minAngle"));		
		EditorGUILayout.PropertyField(maxAngle, new GUIContent("maxAngle"));
		EditorGUILayout.PropertyField(interactable, new GUIContent("interactable"));
		EditorGUILayout.PropertyField(spring, new GUIContent("spring"));
		
		utility.angle = EditorGUILayout.Slider("Angle", utility.angle, utility.minAngle, utility.maxAngle);
		
		EditorGUILayout.Separator();
		EditorGUILayout.PropertyField(OnDragStart, new GUIContent("OnDragStart"));
		EditorGUILayout.PropertyField(OnDragEnd, new GUIContent("OnDragEnd"));

		if(Application.isPlaying && utility.interactable)
			EditorUtility.SetDirty(target);
		
		this.serializedObject.ApplyModifiedProperties();
	}
}