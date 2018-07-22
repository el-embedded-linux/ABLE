using UnityEngine;
using UnityEditor;
using Ardunity;


[CustomEditor(typeof(ExplosionReactor))]
public class ExplosionReactorEditor : ArdunityObjectEditor
{
    private bool _useGizmo = true;
    
    SerializedProperty script;
    SerializedProperty effectRadius;
	SerializedProperty explosionForce;
	SerializedProperty oneShotOnly;
	SerializedProperty layerMask;
	
	void OnEnable()
	{
        script = serializedObject.FindProperty("m_Script");
        effectRadius = serializedObject.FindProperty("effectRadius");
		explosionForce = serializedObject.FindProperty("explosionForce");
		oneShotOnly = serializedObject.FindProperty("oneShotOnly");
		layerMask = serializedObject.FindProperty("layerMask");
	}
	
	public override void OnInspectorGUI()
	{
		this.serializedObject.Update();
		
		ExplosionReactor reactor = (ExplosionReactor)target;
        
        GUI.enabled = false;
        EditorGUILayout.PropertyField(script, true, new GUILayoutOption[0]);
        GUI.enabled = true;
        EditorGUILayout.PropertyField(effectRadius, new GUIContent("effectRadius"));
		EditorGUILayout.PropertyField(explosionForce, new GUIContent("explosionForce"));
		EditorGUILayout.PropertyField(oneShotOnly, new GUIContent("oneShotOnly"));
		EditorGUILayout.PropertyField(layerMask, new GUIContent("layerMask"));
        bool useGizmo = EditorGUILayout.Toggle("Use Gizmo", _useGizmo);
        if(useGizmo != _useGizmo)
        {
            _useGizmo = useGizmo;
            SceneView.RepaintAll();
        }
		
        if(Application.isPlaying && reactor.oneShotOnly)
        {
            if(GUILayout.Button("Reset"))
                reactor.ResetOneShot();
        }
        
		this.serializedObject.ApplyModifiedProperties();
	}
    
    void OnSceneGUI()
    {
        if(!_useGizmo)
            return;
        
        ExplosionReactor reactor = (ExplosionReactor)target;
    
        Handles.color = Color.yellow;
        reactor.effectRadius = Handles.RadiusHandle(Quaternion.identity, reactor.transform.position, reactor.effectRadius);
    }
	
	static public void AddMenuItem(GenericMenu menu, GenericMenu.MenuFunction2 func)
	{
		string menuName = "Unity/Add Reactor/Physics/ExplosionReactor";
		
		if(Selection.activeGameObject != null)
			menu.AddItem(new GUIContent(menuName), false, func, typeof(ExplosionReactor));
		else
			menu.AddDisabledItem(new GUIContent(menuName));
	}
}