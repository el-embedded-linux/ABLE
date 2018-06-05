using UnityEngine;
using UnityEditor;
using Ardunity;


[CustomEditor(typeof(LightIntensityReactor))]
public class LightIntensityReactorEditor : ArdunityObjectEditor
{
    SerializedProperty script;
	SerializedProperty maxIntensity;
	SerializedProperty cutoffIntensity;
	
	void OnEnable()
	{
        script = serializedObject.FindProperty("m_Script");
		maxIntensity = serializedObject.FindProperty("maxIntensity");
		cutoffIntensity = serializedObject.FindProperty("cutoffIntensity");
	}
	
	public override void OnInspectorGUI()
	{
		this.serializedObject.Update();
		
		//LightIntensityReactor reactor = (LightIntensityReactor)target;
        
        GUI.enabled = false;
        EditorGUILayout.PropertyField(script, true, new GUILayoutOption[0]);
        GUI.enabled = true;
		EditorGUILayout.PropertyField(maxIntensity);
		EditorGUILayout.PropertyField(cutoffIntensity);

		this.serializedObject.ApplyModifiedProperties();
	}
	
	static public void AddMenuItem(GenericMenu menu, GenericMenu.MenuFunction2 func)
	{
		string menuName = "Unity/Add Reactor/Effect/LightIntensityReactor";
		
		if(Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<Light>() != null)
			menu.AddItem(new GUIContent(menuName), false, func, typeof(LightIntensityReactor));
		else
			menu.AddDisabledItem(new GUIContent(menuName));
	}
}