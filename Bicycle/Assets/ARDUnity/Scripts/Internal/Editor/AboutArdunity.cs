using UnityEngine;
using UnityEditor;

public class AboutArdunity : EditorWindow
{
	readonly private string _edition = "Deluxe Edition";
    readonly private string _version = "1.0.6";
	readonly private string _releaseURL = "https://sites.google.com/site/ardunitydoc/home/release-note/releasenote-deluxe";

  //  static private Vector2 _windowSize = new Vector2(260, 215);
	static private Vector2 _windowSize = new Vector2(260, 190);
    private Texture2D _logo;
    
	[MenuItem ("ARDUnity/About ARDUnity", false, 100)]
    static void ShowWindow()
	{
		AboutArdunity window = (AboutArdunity)EditorWindow.GetWindow(typeof(AboutArdunity), true, "About ARDUnity");
        window.maxSize = _windowSize;
        window.minSize = _windowSize;
		window.Show();
	}

	[MenuItem ("ARDUnity/PlayMaker Add-on", false, 90)]
	static void PlayMakerAddon()
	{
		string path = Application.dataPath + "/ARDUnity/PlayMakerAddon.unitypackage";
		AssetDatabase.ImportPackage(path, true);
	}

	[MenuItem ("ARDUnity/View User Guide", false, 90)]
	static void ViewUserGuide()
	{
		string url = "file://" + Application.dataPath + "/ARDUnity/UserGuide.pdf";
		Application.OpenURL(url);
	}

	[MenuItem ("ARDUnity/Online Document", false, 90)]
	static void OnlineDocument()
	{
		Application.OpenURL("https://sites.google.com/site/ardunitydoc/");
	}

	[MenuItem ("ARDUnity/Forum", false, 90)]
	static void Forum()
	{
		Application.OpenURL("https://groups.google.com/forum/#!forum/ardunity-forum");
	}
    
    void OnEnable()
	{
		_logo = (Texture2D)EditorGUIUtility.Load("Assets/ARDUnity/Logo.png");
	}
    
    void OnGUI()
	{
        GUILayout.Box(_logo, GUILayout.Width(_windowSize.x - 10), GUILayout.Height((_windowSize.x - 10) * 0.4f));
        GUILayout.Label("Smart Maker. All Right Reserved.");
		GUILayout.Label(_edition);
        GUILayout.Label(string.Format("Version {0}", _version));
        
        if(GUILayout.Button("Release Notes"))
			Application.OpenURL(_releaseURL);
        
    //    if(GUILayout.Button("Ardunity.com"))
    //        Application.OpenURL("http://www.ardunity.com");
    }
}
