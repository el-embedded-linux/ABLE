using UnityEngine;
using System.Collections;
using UnityEditor;
using Ardunity;

[CustomEditor(typeof(DeviceCamera))]
public class DeviceCameraEditor : Editor
{
	[MenuItem("ARDUnity/Add Utility/Device/DeviceCamera", true)]
	static bool ValidateMenu()
	{
		if(Selection.activeGameObject == null)
			return false;
			
		return true;
	}
	[MenuItem("ARDUnity/Add Utility/Device/DeviceCamera", false, 10)]
    static void DoMenu()
    {
        Selection.activeGameObject.AddComponent<DeviceCamera>();
    }


	SerializedProperty script;
	SerializedProperty defaultDevice;
	SerializedProperty capWidth;
	SerializedProperty capHeight;
	SerializedProperty capFPS;
	SerializedProperty autoPlay;
	SerializedProperty material;
	SerializedProperty rawImage;

	void OnEnable()
	{
		script = serializedObject.FindProperty("m_Script");
		defaultDevice = serializedObject.FindProperty("defaultDevice");
		capWidth = serializedObject.FindProperty("capWidth");
		capHeight = serializedObject.FindProperty("capHeight");
		capFPS = serializedObject.FindProperty("capFPS");
		autoPlay = serializedObject.FindProperty("autoPlay");
		material = serializedObject.FindProperty("material");
		rawImage = serializedObject.FindProperty("rawImage");
	}
	
	public override void OnInspectorGUI()
	{
		GUI.enabled = false;
        EditorGUILayout.PropertyField(script, true, new GUILayoutOption[0]);
        GUI.enabled = true;

#if UNITY_WEBPLAYER || UNITY_WEBGL
		EditorGUILayout.HelpBox("This component does not work on web player/webGL platform", MessageType.Error);
#else
		this.serializedObject.Update();
		
		DeviceCamera deviceCamera = (DeviceCamera)target;

		EditorGUILayout.PropertyField(defaultDevice, new GUIContent("default Device"));
		EditorGUILayout.PropertyField(capWidth, new GUIContent("capture Width"));
		EditorGUILayout.PropertyField(capHeight, new GUIContent("capture Height"));
		EditorGUILayout.PropertyField(capFPS, new GUIContent("capture FPS"));
		EditorGUILayout.PropertyField(autoPlay, new GUIContent("auto Play"));
		EditorGUILayout.PropertyField(material, new GUIContent("Render Material"));
		EditorGUILayout.PropertyField(rawImage, new GUIContent("Render RawImage"));

		if(Application.isPlaying == true)
		{
			EditorGUILayout.LabelField(string.Format("Device Name: {0}", deviceCamera.deviceName));
			EditorGUILayout.LabelField(string.Format("Image Width: {0:d}", deviceCamera.Width));
			EditorGUILayout.LabelField(string.Format("Image Height: {0:d}", deviceCamera.Height));
			if(GUILayout.Button("Refresh Settings") == true)
				deviceCamera.RefreshSettings();
			if(GUILayout.Button("Change Device") == true)
				deviceCamera.ChangeDevice();
			if(deviceCamera.isPlaying == false)
			{
				if(GUILayout.Button("Play") == true)
					deviceCamera.Play();
			}
			else
			{
				if(GUILayout.Button("Pause") == true)
					deviceCamera.Pause();
			}
			if(GUILayout.Button("Stop") == true)
				deviceCamera.Stop();
		}
		
		this.serializedObject.ApplyModifiedProperties();
#endif
	}
}
