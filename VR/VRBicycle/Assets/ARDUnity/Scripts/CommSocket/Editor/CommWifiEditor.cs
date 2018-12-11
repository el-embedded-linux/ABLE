using UnityEngine;
using UnityEditor;
using Ardunity;

[CustomEditor(typeof(CommWifi))]
public class CommWifiEditor : ArdunityObjectEditor
{
    bool foldout = false;
    
    SerializedProperty OnOpen;
    SerializedProperty OnClose;
    SerializedProperty OnOpenFailed;
    SerializedProperty OnErrorClosed;

    void OnEnable()
	{
        OnOpen = serializedObject.FindProperty("OnOpen");
        OnClose = serializedObject.FindProperty("OnClose");
        OnOpenFailed = serializedObject.FindProperty("OnOpenFailed");
        OnErrorClosed = serializedObject.FindProperty("OnErrorClosed");
    }
	
	public override void OnInspectorGUI()
	{
        this.serializedObject.Update();

        CommWifi socket = (CommWifi)target;
        
        if(socket.device.address.Length == 0)
            socket.device.address = "192.168.240.1"; // Arduino Yun default IP
        socket.device.address = EditorGUILayout.DelayedTextField("IP Address", socket.device.address);
        if(socket.device.args.Count == 0)
            socket.device.args.Add("5555"); // Arduino Yun Bridge Port
        int port = EditorGUILayout.DelayedIntField("Port", int.Parse(socket.device.args[0]));
        socket.device.args[0] = port.ToString();
 
        foldout = EditorGUILayout.Foldout(foldout, "Events");
        if (foldout)
        {
            EditorGUILayout.PropertyField(OnOpen, new GUIContent("OnOpen"));
            EditorGUILayout.PropertyField(OnClose, new GUIContent("OnClose"));
            EditorGUILayout.PropertyField(OnOpenFailed, new GUIContent("OnOpenFailed"));
            EditorGUILayout.PropertyField(OnErrorClosed, new GUIContent("OnErrorClosed"));
        }

        this.serializedObject.ApplyModifiedProperties();
	}
    
    static public void AddMenuItem(GenericMenu menu, GenericMenu.MenuFunction2 func)
    {
        string menuName = "Unity/Add CommSocket/CommWifi";
        
        if(Selection.activeGameObject != null)
            menu.AddItem(new GUIContent(menuName), false, func, typeof(CommWifi));
        else
            menu.AddDisabledItem(new GUIContent(menuName));
    }
}
