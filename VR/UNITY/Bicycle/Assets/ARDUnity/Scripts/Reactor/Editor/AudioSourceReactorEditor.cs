using UnityEngine;
using UnityEditor;
using Ardunity;


[CustomEditor(typeof(AudioSourceReactor))]
public class AudioSourceReactorEditor : ArdunityObjectEditor
{
    SerializedProperty script;
    
	void OnEnable()
	{
        script = serializedObject.FindProperty("m_Script");
	}
	
	public override void OnInspectorGUI()
	{
		this.serializedObject.Update();
		
		//AudioSourceReactor reactor = (AudioSourceReactor)target;
        
        GUI.enabled = false;
        EditorGUILayout.PropertyField(script, true, new GUILayoutOption[0]);
        GUI.enabled = true;

		this.serializedObject.ApplyModifiedProperties();
	}
	
	static public void AddMenuItem(GenericMenu menu, GenericMenu.MenuFunction2 func)
	{
		string menuName = "Unity/Add Reactor/Effect/AudioSourceReactor";
		
		if(Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<AudioSource>() != null)
			menu.AddItem(new GUIContent(menuName), false, func, typeof(AudioSourceReactor));
		else
			menu.AddDisabledItem(new GUIContent(menuName));
	}
}