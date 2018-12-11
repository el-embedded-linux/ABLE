using UnityEngine;
using UnityEditor;
using Ardunity;

[CustomEditor(typeof(BitFlagInput))]
public class BitFlagInputEditor : ArdunityObjectEditor
{
	bool foldout = false;

    SerializedProperty script;
	SerializedProperty bitCombine;

	void OnEnable()
	{
        script = serializedObject.FindProperty("m_Script");
		bitCombine = serializedObject.FindProperty("bitCombine");
	}

	public override void OnInspectorGUI()
	{
		this.serializedObject.Update();
		
		BitFlagInput bridge = (BitFlagInput)target;

        GUI.enabled = false;
        EditorGUILayout.PropertyField(script, true, new GUILayoutOption[0]);
        GUI.enabled = true;

		foldout = EditorGUILayout.Foldout(foldout, "Mask: " + BitFlagInput.ToMaskString(bridge.bitMask));
		if(foldout)
		{
			EditorGUI.indentLevel++;
			for(int i=0; i<16; i++)
			{
				bool bit = false;
				if((bridge.bitMask & (1 << i)) > 0)
					bit = true;
				
				bit = EditorGUILayout.Toggle(string.Format("Bit{0:d}", i), bit);

				if(bit)
					bridge.bitMask |= (1 << i);
				else
					bridge.bitMask &= ~(1 << i);
			}
			EditorGUI.indentLevel--;
		}

		EditorGUILayout.PropertyField(bitCombine, new GUIContent("Bit Combine"));

		EditorGUILayout.BeginHorizontal();
		EditorGUILayout.LabelField("Value", GUILayout.Width(80f));
		int index = 0;
		if(bridge.Value)
			index = 1;
		GUILayout.SelectionGrid(index, new string[] {"FALSE", "TRUE"}, 2);
		EditorGUILayout.EndHorizontal();
			
		if(Application.isPlaying)
			EditorUtility.SetDirty(target);
		
		this.serializedObject.ApplyModifiedProperties();
	}
	
	static public void AddMenuItem(GenericMenu menu, GenericMenu.MenuFunction2 func)
	{
		string menuName = "Unity/Add Bridge/Input/BitFlagInput";
		
		if(Selection.activeGameObject != null)
			menu.AddItem(new GUIContent(menuName), false, func, typeof(BitFlagInput));
		else
			menu.AddDisabledItem(new GUIContent(menuName));
	}
}
