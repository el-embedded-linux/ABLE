using UnityEngine;
using UnityEditor;
using Ardunity;

[CustomEditor(typeof(FloatProbe))]
public class FloatProbeEditor : ArdunityObjectEditor
{
    SerializedProperty script;

	void OnEnable()
	{
        script = serializedObject.FindProperty("m_Script");
	}

	public override void OnInspectorGUI()
	{
		this.serializedObject.Update();
		
		//FloatProbe bridge = (FloatProbe)target;

        GUI.enabled = false;
        EditorGUILayout.PropertyField(script, true, new GUILayoutOption[0]);
        GUI.enabled = true;
		
		this.serializedObject.ApplyModifiedProperties();
	}
	
	static public void AddMenuItem(GenericMenu menu, GenericMenu.MenuFunction2 func)
	{
		string menuName = "Unity/Add Bridge/Probe/FloatProbe";
		
		if(Selection.activeGameObject != null)
			menu.AddItem(new GUIContent(menuName), false, func, typeof(FloatProbe));
		else
			menu.AddDisabledItem(new GUIContent(menuName));
	}
}
