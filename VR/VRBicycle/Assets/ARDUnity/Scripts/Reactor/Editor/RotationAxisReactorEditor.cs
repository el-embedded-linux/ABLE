using UnityEngine;
using UnityEditor;
using Ardunity;


[CustomEditor(typeof(RotationAxisReactor))]
public class RotationAxisReactorEditor : ArdunityObjectEditor
{
    private bool _useGizmo = true;
    
    SerializedProperty script;
	SerializedProperty upAxis;
	SerializedProperty forwardAxis;
	SerializedProperty invert;
	
	void OnEnable()
	{
        script = serializedObject.FindProperty("m_Script");
		upAxis = serializedObject.FindProperty("upAxis");
		forwardAxis = serializedObject.FindProperty("forwardAxis");
		invert = serializedObject.FindProperty("invert");
	}
	
	public override void OnInspectorGUI()
	{
		this.serializedObject.Update();
		
		//RotationAxisReactor reactor = (RotationAxisReactor)target;
        
        GUI.enabled = false;
        EditorGUILayout.PropertyField(script, true, new GUILayoutOption[0]);
        GUI.enabled = true;
		EditorGUILayout.PropertyField(upAxis, new GUIContent("upAxis"));
		EditorGUILayout.PropertyField(forwardAxis, new GUIContent("forwardAxis"));		
		EditorGUILayout.PropertyField(invert, new GUIContent("invert"));
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
        
        RotationAxisReactor reactor = (RotationAxisReactor)target;
        
        float size = HandleUtility.GetHandleSize(reactor.transform.position) * 1.5f;
        
        Handles.color = Color.yellow;
        Vector3 dir = Vector3.right;
        if(reactor.upAxis == Axis.X)
            dir = reactor.transform.right;
        if(reactor.upAxis == Axis.Y)
            dir = reactor.transform.up;
        if(reactor.upAxis == Axis.Z)
            dir = reactor.transform.forward;
        reactor.transform.rotation = Handles.Disc(reactor.transform.rotation, reactor.transform.position, dir, size, false, 1f);        
        
        Handles.color = Color.cyan;
        Quaternion q = Quaternion.identity;
        if(reactor.forwardAxis == Axis.X)
            q = Quaternion.FromToRotation(Vector3.forward, Vector3.right);
        if(reactor.forwardAxis == Axis.Y)
            q = Quaternion.FromToRotation(Vector3.forward, Vector3.up);
        if(reactor.forwardAxis == Axis.Z)
            q = Quaternion.FromToRotation(Vector3.forward, Vector3.forward);
#if UNITY_5_5_OR_NEWER
		Handles.ArrowHandleCap(0, reactor.transform.position, reactor.transform.rotation * q, size, EventType.Repaint);
#else
		Handles.ArrowCap(0, reactor.transform.position, reactor.transform.rotation * q, size);
#endif
    }
	
	static public void AddMenuItem(GenericMenu menu, GenericMenu.MenuFunction2 func)
	{
		string menuName = "Unity/Add Reactor/Transform/RotationAxisReactor";
		
		if(Selection.activeGameObject != null)
			menu.AddItem(new GUIContent(menuName), false, func, typeof(RotationAxisReactor));
		else
			menu.AddDisabledItem(new GUIContent(menuName));
	}
}