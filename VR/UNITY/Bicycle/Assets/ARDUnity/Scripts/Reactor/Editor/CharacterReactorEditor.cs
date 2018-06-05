using UnityEngine;
using UnityEditor;
using Ardunity;


[CustomEditor(typeof(CharacterReactor))]
public class CharacterReactorEditor : ArdunityObjectEditor
{
	SerializedProperty script;
	SerializedProperty vrCamera;
	SerializedProperty weapon;
	SerializedProperty moveScale;
	SerializedProperty aheadScale;
	SerializedProperty jumpForce;
    SerializedProperty usePhysics;
	
	void OnEnable()
	{
		script = serializedObject.FindProperty("m_Script");
		vrCamera = serializedObject.FindProperty("vrCamera");
		weapon = serializedObject.FindProperty("weapon");
		moveScale = serializedObject.FindProperty("moveScale");
		aheadScale = serializedObject.FindProperty("aheadScale");
		jumpForce = serializedObject.FindProperty("jumpForce");
		usePhysics = serializedObject.FindProperty("usePhysics");
	}
	
	public override void OnInspectorGUI()
	{
		this.serializedObject.Update();
		
		CharacterReactor reactor = (CharacterReactor)target;
		
		GUI.enabled = false;
        EditorGUILayout.PropertyField(script, true, new GUILayoutOption[0]);
        GUI.enabled = true;

		EditorGUILayout.PropertyField(vrCamera, new GUIContent("VR Camera"));
		EditorGUILayout.PropertyField(weapon, new GUIContent("Weapon"));
		EditorGUILayout.PropertyField(moveScale, new GUIContent("Move Scale"));
		EditorGUILayout.PropertyField(aheadScale, new GUIContent("Ahead Scale"));
		EditorGUILayout.PropertyField(jumpForce, new GUIContent("Jump Force"));
		reactor.groundTag = EditorGUILayout.TagField("Ground Tag", reactor.groundTag);
		EditorGUILayout.PropertyField(usePhysics, new GUIContent("Use Physics"));

		this.serializedObject.ApplyModifiedProperties();
	}
	
	static public void AddMenuItem(GenericMenu menu, GenericMenu.MenuFunction2 func)
	{
		string menuName = "Unity/Add Reactor/Miscellaneous/CharacterReactor";
		
		if(Selection.activeGameObject != null)
			menu.AddItem(new GUIContent(menuName), false, func, typeof(CharacterReactor));
		else
			menu.AddDisabledItem(new GUIContent(menuName));
	}
}