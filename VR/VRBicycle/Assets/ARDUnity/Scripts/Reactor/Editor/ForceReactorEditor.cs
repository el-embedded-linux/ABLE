using UnityEngine;
using UnityEditor;
using Ardunity;


[CustomEditor(typeof(ForceReactor))]
public class ForceReactorEditor : ArdunityObjectEditor
{
    SerializedProperty script;
	SerializedProperty rigidBody;
	SerializedProperty position;
	SerializedProperty direction;
	SerializedProperty force;
	SerializedProperty forceMode;
	SerializedProperty oneShotOnly;
	
	void OnEnable()
	{
        script = serializedObject.FindProperty("m_Script");
		rigidBody = serializedObject.FindProperty("rigidBody");
		position = serializedObject.FindProperty("position");
		direction = serializedObject.FindProperty("direction");
		force = serializedObject.FindProperty("force");
		forceMode = serializedObject.FindProperty("forceMode");
		oneShotOnly = serializedObject.FindProperty("oneShotOnly");
	}
	
	public override void OnInspectorGUI()
	{
		this.serializedObject.Update();
		
		ForceReactor reactor = (ForceReactor)target;

        GUI.enabled = false;
        EditorGUILayout.PropertyField(script, true, new GUILayoutOption[0]);
        GUI.enabled = true;
		EditorGUILayout.PropertyField(rigidBody, new GUIContent("rigidBody"));
		EditorGUILayout.PropertyField(position, new GUIContent("position"));
		EditorGUILayout.PropertyField(direction, new GUIContent("direction"));
		EditorGUILayout.PropertyField(force, new GUIContent("force"));
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
		string menuName = "Unity/Add Reactor/Physics/ForceReactor";
		
		if(Selection.activeGameObject != null)
			menu.AddItem(new GUIContent(menuName), false, func, typeof(ForceReactor));
		else
			menu.AddDisabledItem(new GUIContent(menuName));
	}
}