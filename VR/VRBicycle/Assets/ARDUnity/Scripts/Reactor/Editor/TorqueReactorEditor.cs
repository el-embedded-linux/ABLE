using UnityEngine;
using UnityEditor;
using Ardunity;


[CustomEditor(typeof(TorqueReactor))]
public class TorqueReactorEditor : ArdunityObjectEditor
{
    SerializedProperty script;
	SerializedProperty rigidBody;
	SerializedProperty axis;
	SerializedProperty torque;
	SerializedProperty forceMode;
	SerializedProperty oneShotOnly;
	
	void OnEnable()
	{
        script = serializedObject.FindProperty("m_Script");
		rigidBody = serializedObject.FindProperty("rigidBody");
		axis = serializedObject.FindProperty("axis");
		torque = serializedObject.FindProperty("torque");
		forceMode = serializedObject.FindProperty("forceMode");
		oneShotOnly = serializedObject.FindProperty("oneShotOnly");
	}
	
	public override void OnInspectorGUI()
	{
		this.serializedObject.Update();
		
		TorqueReactor reactor = (TorqueReactor)target;
        
        GUI.enabled = false;
        EditorGUILayout.PropertyField(script, true, new GUILayoutOption[0]);
        GUI.enabled = true;
		EditorGUILayout.PropertyField(rigidBody, new GUIContent("rigidBody"));
		EditorGUILayout.PropertyField(axis, new GUIContent("axis"));
		EditorGUILayout.PropertyField(torque, new GUIContent("torque"));
		EditorGUILayout.PropertyField(forceMode, new GUIContent("forceMode"));
		EditorGUILayout.PropertyField(oneShotOnly, new GUIContent("oneShotOnly"));
		
        if(Application.isPlaying && reactor.oneShotOnly)
        {
            if(GUILayout.Button("Reset"))
                reactor.ResetOneShot();
        }

		this.serializedObject.ApplyModifiedProperties();
	}
	
	static public void AddMenuItem(GenericMenu menu, GenericMenu.MenuFunction2 func)
	{
		string menuName = "Unity/Add Reactor/Physics/TorqueReactor";
		
		if(Selection.activeGameObject != null)
			menu.AddItem(new GUIContent(menuName), false, func, typeof(TorqueReactor));
		else
			menu.AddDisabledItem(new GUIContent(menuName));
	}
}