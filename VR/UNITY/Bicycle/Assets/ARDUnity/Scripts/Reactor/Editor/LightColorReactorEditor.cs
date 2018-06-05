using UnityEngine;
using UnityEditor;
using Ardunity;


[CustomEditor(typeof(LightColorReactor))]
public class LightColorReactorEditor : ArdunityObjectEditor
{
    SerializedProperty script;
    
	void OnEnable()
	{
        script = serializedObject.FindProperty("m_Script");
	}
	
	public override void OnInspectorGUI()
	{
		this.serializedObject.Update();
		
		//LightColorReactor reactor = (LightColorReactor)target;
        
        GUI.enabled = false;
        EditorGUILayout.PropertyField(script, true, new GUILayoutOption[0]);
        GUI.enabled = true;
        
		this.serializedObject.ApplyModifiedProperties();
	}
	
	static public void AddMenuItem(GenericMenu menu, GenericMenu.MenuFunction2 func)
	{
		string menuName = "Unity/Add Reactor/Effect/LightColorReactor";
		
		if(Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<Light>() != null)
			menu.AddItem(new GUIContent(menuName), false, func, typeof(LightColorReactor));
		else
			menu.AddDisabledItem(new GUIContent(menuName));
	}
}