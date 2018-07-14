using UnityEngine;
using UnityEditor;
using Ardunity;

[CustomEditor(typeof(CommBluetooth))]
public class CommBluetoothEditor : ArdunityObjectEditor
{
    bool foldout = false;
    
    SerializedProperty searchTimeout;
    SerializedProperty OnOpen;
    SerializedProperty OnClose;
    SerializedProperty OnOpenFailed;
    SerializedProperty OnErrorClosed;
    SerializedProperty OnStartSearch;
    SerializedProperty OnStopSearch;

    void OnEnable()
	{
        searchTimeout = serializedObject.FindProperty("searchTimeout");
        OnOpen = serializedObject.FindProperty("OnOpen");
        OnClose = serializedObject.FindProperty("OnClose");
        OnOpenFailed = serializedObject.FindProperty("OnOpenFailed");
        OnErrorClosed = serializedObject.FindProperty("OnErrorClosed");
        OnStartSearch = serializedObject.FindProperty("OnStartSearch");
        OnStopSearch = serializedObject.FindProperty("OnStopSearch");
    }
	
	public override void OnInspectorGUI()
	{
        this.serializedObject.Update();

      //  CommBluetooth socket = (CommBluetooth)target;
        
        EditorGUILayout.HelpBox("This component works only with Android platform.", MessageType.Info);
      
        EditorGUILayout.PropertyField(searchTimeout, new GUIContent("searchTimeout"));

        foldout = EditorGUILayout.Foldout(foldout, "Events");
        if (foldout)
        {
            EditorGUILayout.PropertyField(OnOpen, new GUIContent("OnOpen"));
            EditorGUILayout.PropertyField(OnClose, new GUIContent("OnClose"));
            EditorGUILayout.PropertyField(OnOpenFailed, new GUIContent("OnOpenFailed"));
            EditorGUILayout.PropertyField(OnErrorClosed, new GUIContent("OnErrorClosed"));
            EditorGUILayout.PropertyField(OnStartSearch, new GUIContent("OnStartSearch"));
            EditorGUILayout.PropertyField(OnStopSearch, new GUIContent("OnStopSearch"));
        }
        
        this.serializedObject.ApplyModifiedProperties();
	}
    
    static public void AddMenuItem(GenericMenu menu, GenericMenu.MenuFunction2 func)
    {
        string menuName = "Unity/Add CommSocket/CommBluetooth";
        
        if(Selection.activeGameObject != null)
            menu.AddItem(new GUIContent(menuName), false, func, typeof(CommBluetooth));
        else
            menu.AddDisabledItem(new GUIContent(menuName));
    }
}
