using UnityEngine;
using UnityEditor;
using Ardunity;


[CustomEditor(typeof(ColliderReactor))]
public class ColliderReactorEditor : ArdunityObjectEditor
{
    SerializedProperty script;
    
	void OnEnable()
	{
        script = serializedObject.FindProperty("m_Script");
	}
	
	public override void OnInspectorGUI()
	{
		this.serializedObject.Update();
		
		//ColliderReactor reactor = (ColliderReactor)target;
        
        GUI.enabled = false;
        EditorGUILayout.PropertyField(script, true, new GUILayoutOption[0]);
        GUI.enabled = true;

		this.serializedObject.ApplyModifiedProperties();
	}
	
	static public void AddMenuItem(GenericMenu menu, GenericMenu.MenuFunction2 func)
	{
		string menuName = "Unity/Add Reactor/Physics/ColliderReactor";
		
		GameObject go = Selection.activeGameObject;
		if(go != null && (go.GetComponent<Collider>() != null || go.GetComponent<Collider2D>() != null))
			menu.AddItem(new GUIContent(menuName), false, func, typeof(ColliderReactor));				
		else
			menu.AddDisabledItem(new GUIContent(menuName));
	}
}