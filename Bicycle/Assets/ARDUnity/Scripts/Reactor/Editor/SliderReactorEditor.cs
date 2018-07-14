using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using Ardunity;


[CustomEditor(typeof(SliderReactor))]
public class SliderReactorEditor : ArdunityObjectEditor
{
    SerializedProperty script;
    
	void OnEnable()
	{
        script = serializedObject.FindProperty("m_Script");
	}
	
	public override void OnInspectorGUI()
	{
		this.serializedObject.Update();
		
		//SliderReactor reactor = (SliderReactor)target;
        
        GUI.enabled = false;
        EditorGUILayout.PropertyField(script, true, new GUILayoutOption[0]);
        GUI.enabled = true;

		this.serializedObject.ApplyModifiedProperties();
	}
	
	static public void AddMenuItem(GenericMenu menu, GenericMenu.MenuFunction2 func)
	{
		string menuName = "Unity/Add Reactor/UI/SliderReactor";
		
		if(Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<Slider>() != null)
			menu.AddItem(new GUIContent(menuName), false, func, typeof(SliderReactor));
		else
			menu.AddDisabledItem(new GUIContent(menuName));
	}
}