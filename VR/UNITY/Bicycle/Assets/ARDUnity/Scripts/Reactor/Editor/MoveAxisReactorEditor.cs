using UnityEngine;
using UnityEditor;
using Ardunity;


[CustomEditor(typeof(MoveAxisReactor))]
public class MoveAxisReactorEditor : ArdunityObjectEditor
{
    private bool _useGizmo = true;
    
    SerializedProperty script;
	SerializedProperty moveAxis;
	SerializedProperty invert;
    SerializedProperty scaler;
	
	void OnEnable()
	{
        script = serializedObject.FindProperty("m_Script");
		moveAxis = serializedObject.FindProperty("moveAxis");
		invert = serializedObject.FindProperty("invert");
        scaler = serializedObject.FindProperty("scaler");
	}
	
	public override void OnInspectorGUI()
	{
		this.serializedObject.Update();
		
		//MoveAxisReactor reactor = (MoveAxisReactor)target;
        
        GUI.enabled = false;
        EditorGUILayout.PropertyField(script, true, new GUILayoutOption[0]);
        GUI.enabled = true;
		EditorGUILayout.PropertyField(moveAxis, new GUIContent("moveAxis"));		
		EditorGUILayout.PropertyField(invert, new GUIContent("invert"));
        EditorGUILayout.PropertyField(scaler, new GUIContent("scaler"));
        bool useGizmo = EditorGUILayout.Toggle("Use Gizmo", _useGizmo);
        if(useGizmo != _useGizmo)
        {
            _useGizmo = useGizmo;
            SceneView.RepaintAll();
        }

		this.serializedObject.ApplyModifiedProperties();
	}
    
    void OnSceneGUI()
    {
        if(!_useGizmo)
            return;
        
        MoveAxisReactor reactor = (MoveAxisReactor)target;
    
        Handles.color = Color.magenta;
        Vector3 dir = Vector3.right;
        if(reactor.moveAxis == Axis.X)
            dir = reactor.transform.right;
        if(reactor.moveAxis == Axis.Y)
            dir = reactor.transform.up;
        if(reactor.moveAxis == Axis.Z)
            dir = reactor.transform.forward;
        reactor.transform.position = Handles.Slider(reactor.transform.position, dir);
    }
	
	static public void AddMenuItem(GenericMenu menu, GenericMenu.MenuFunction2 func)
	{
		string menuName = "Unity/Add Reactor/Transform/MoveAxisReactor";
		
		if(Selection.activeGameObject != null)
			menu.AddItem(new GUIContent(menuName), false, func, typeof(MoveAxisReactor));
		else
			menu.AddDisabledItem(new GUIContent(menuName));
	}
}