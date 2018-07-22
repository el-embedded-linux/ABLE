using UnityEngine;
using UnityEditor;
using Ardunity;

[CustomEditor(typeof(RTTTLSong))]
public class RTTTLSongEditor : ArdunityObjectEditor
{
    SerializedProperty script;
	SerializedProperty songAsset;
	SerializedProperty OnEndSong;
	
	int _selection = -1;

	void OnEnable()
	{
        script = serializedObject.FindProperty("m_Script");
		songAsset = serializedObject.FindProperty("songAsset");
		OnEndSong = serializedObject.FindProperty("OnEndSong");
	}

	public override void OnInspectorGUI()
	{
		this.serializedObject.Update();
		
		RTTTLSong bridge = (RTTTLSong)target;

        GUI.enabled = false;
        EditorGUILayout.PropertyField(script, true, new GUILayoutOption[0]);
        GUI.enabled = true;
		EditorGUILayout.PropertyField(songAsset, new GUIContent("songAsset"));

		if(Application.isPlaying)
		{
			EditorGUILayout.BeginHorizontal();
			_selection = EditorGUILayout.Popup("Song", _selection, bridge.songs);
			if(!bridge.isPlaying)
			{
				if(GUILayout.Button("Play"))
				{
					bridge.SelectSong(_selection);
					bridge.Play();
				}
			}
			else
			{
				if(GUILayout.Button("Stop"))
					bridge.Stop();
			}
			EditorGUILayout.EndHorizontal();
			
			EditorUtility.SetDirty(target);
		}
	
		EditorGUILayout.Separator();
		EditorGUILayout.PropertyField(OnEndSong, new GUIContent("OnEndSong"));
		
		this.serializedObject.ApplyModifiedProperties();
	}
	
	static public void AddMenuItem(GenericMenu menu, GenericMenu.MenuFunction2 func)
	{
		string menuName = "Unity/Add Bridge/Output/RTTTLSong";
		
		if(Selection.activeGameObject != null)
			menu.AddItem(new GUIContent(menuName), false, func, typeof(RTTTLSong));
		else
			menu.AddDisabledItem(new GUIContent(menuName));
	}
}
