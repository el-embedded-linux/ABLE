using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Ardunity;

[CustomEditor(typeof(HM10))]
public class HM10Editor : ArdunityObjectEditor
{
    bool foldout = false;
    
    SerializedProperty searchTimeout;
    SerializedProperty serviceUUID;
    SerializedProperty charUUID;
    SerializedProperty OnOpen;
    SerializedProperty OnClose;
    SerializedProperty OnOpenFailed;
    SerializedProperty OnErrorClosed;
    SerializedProperty OnStartSearch;
    SerializedProperty OnStopSearch;

    void OnEnable()
	{
        searchTimeout = serializedObject.FindProperty("searchTimeout");
        serviceUUID = serializedObject.FindProperty("serviceUUID");
        charUUID = serializedObject.FindProperty("charUUID");
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

#if UNITY_STANDALONE_OSX

#if UNITY_EDITOR_OSX
        HM10 socket = (HM10)target;

        if(Application.isPlaying)
        {
            if(socket.isSupport)
            {
                GUILayout.Label(string.Format("Device: {0}", socket.device.name));
                if(socket.IsOpen)
                {
                }
                else
                {
                    if(socket.isSearching)
                    {
                        EditorUtility.SetDirty(target);
                        
                        if(GUILayout.Button("Stop Search"))
                            socket.StopSearch();
                    }
                    else
                    {
                        if(GUILayout.Button("Start Search"))
                            socket.StartSearch();
                    }
                    
                    if(socket.foundDevices.Count > 0)
                    {
                        EditorGUILayout.Foldout(true, "Found Devices");
                        List<string> names = new List<string>();
                        int selection = -1;
                        for(int i=0; i<socket.foundDevices.Count; i++)
                        {
                            names.Add(socket.foundDevices[i].name);
                            if(socket.foundDevices[i].Equals(socket.device))
                                selection = i;
                        }
    
                        int newSelection = GUILayout.SelectionGrid(selection, names.ToArray(), 1);
                        if(selection != newSelection)
                            socket.device = new CommDevice(socket.foundDevices[newSelection]);
                    }
                }
            }
            else
            {
                EditorGUILayout.HelpBox("This machine is not supported BLE", MessageType.Info);
            }
        }
#else
        EditorGUILayout.HelpBox("Inspector is not support on current OS.", MessageType.Warning);
#endif

#elif UNITY_ANDROID
        EditorGUILayout.HelpBox("Inspector is not support on current platform.", MessageType.Warning);
#else
        EditorGUILayout.HelpBox("This component is not supported on current platform.", MessageType.Warning);
#endif
      
        EditorGUILayout.PropertyField(searchTimeout, new GUIContent("searchTimeout"));
        EditorGUILayout.PropertyField(serviceUUID, new GUIContent("Service UUID"));
        EditorGUILayout.PropertyField(charUUID, new GUIContent("Characteristic UUID"));

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
        string menuName = "Unity/Add CommSocket/HM10";
        
        if(Selection.activeGameObject != null )
            menu.AddItem(new GUIContent(menuName), false, func, typeof(HM10));
        else
            menu.AddDisabledItem(new GUIContent(menuName));
    }
}
