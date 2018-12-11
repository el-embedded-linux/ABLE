using UnityEngine;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using UnityEngine.UI;
using System.IO.Ports;
using UnityEngine.SceneManagement;
using Ardunity;

public class mapsin : MonoBehaviour {
Stopwatch sw = new Stopwatch();

	public static int menu = 0;
    public static string curSpeed;
    public static string totalDist;
    //float radius = 20; // ??? ?? ??? ?? ?? ?? ??? ??? ???? ??? ???.(Cm ??)
    float circle = (float)(2 * 35 * 3.14) / 100;  // ??? ??? ??? ??(??? m? ???? ?? 100? ????.)

    public static float bySpeed = 0; // ???? ??
    float ckTime = 0;  // ?????? 
    float uckTime = 0; // Unckecked
    float cycleTime = 0;  // ?????? ??? ??? ?? ?? ???? ???? ??
    float distance = 0; // ???? ?? ?? ??
    float lcdDis = 0; // ???? ?? ??? LCD??? ?? ??? ?.(?? ?? or ??? ??)
    float bicdis = 0;

    int count = 0;  // ?????? ???? ???? ?? ???? ?????.
    bool temp = false;  // ?? ???? ???? ???? ??

	public DigitalInput digitalInput;

    void Start()
    {

    }
    void Update()
    {
        
    }public void MoveButton(){

		Debug.Log ("4");  
		GameObject gameObject2;  
		gameObject2 = GameObject.Find("Outline_m");  
		if (menu == 0)                               // UI? ??? ?? ??  
		{  
			gameObject2.transform.Translate(0, (float)-0.18, 0);         // UI Y??? -70  
			menu = 1;  
		}  
		else if (menu == 1)  
		{  
			gameObject2.transform.Translate(0, (float)-0.18, 0);  
			menu = 2;  
		}  
		else if (menu == 2)  
		{  
			gameObject2.transform.Translate(0, (float)-0.18, 0);  
			menu = 3;  
		}  
		else if (menu == 3)      	                     // UI? ? ??? ?? ?? 
		{  
			gameObject2.transform.Translate(0, (float)0.54, 0);         // UI ??? ???? ?? ??
			menu = 0;  
		}  
	}  

	public void SelectButton(){
		Debug.Log ("5");
		GameObject gameObject2;
		gameObject2 = GameObject.Find("Outline_m");
		RealTimeDB realTimeDB = gameObject.AddComponent<RealTimeDB>();
		ContinueBtn cb = gameObject.AddComponent<ContinueBtn>();
		if (menu == 0)  
		{  
			menu = 0;  
			realTimeDB.InitDatabase();                              // FIrebase? ??  
		}  
		else if (menu == 1)  
		{  
			menu = 0;    
			gameObject2.transform.Translate(0, (float)0.18, 0);  
			cb.ButtonOnClick();                                     // ?? ?? ?? ???  
		}  
		else if (menu == 2)  
		{  
			menu = 0;        
			gameObject2.transform.Translate(0, (float)0.36, 0);  
			SceneManager.LoadScene("03_Mapselect");                 // ? ?? ??? ??  
		}  
		else if (menu == 3)      
		{  
			menu = 0;  
			gameObject2.transform.Translate(0, (float)0.54, 0);    
			SceneManager.LoadScene("02_Menu");                      // ?? ?? ??? ??  
		}  
	}    


	/// <summary>
	/// ///////////
	/// </summary>
	public void MoveGameButton(){

		Debug.Log ("3");  
		GameObject gameObject2;  
		gameObject2 = GameObject.Find("Outline_m");  
		if (menu == 0)                               // UI? ??? ?? ??  
		{  
			gameObject2.transform.Translate(0, (float)-0.135 ,0);         // UI Y??? -70  
			menu = 1;
		}  
		else if (menu == 1)  
		{  
			gameObject2.transform.Translate(0, (float)-0.135, 0);  
			menu = 2;  
		}  
		else if (menu == 2)  
		{  
			gameObject2.transform.Translate(0, (float)0.27, 0);  
			menu = 0;  
		} 
	}  

	public void SelectGameButton(){
		Debug.Log ("4");
		GameObject gameObject2;
		gameObject2 = GameObject.Find("Outline_m");
		RealTimeDB realTimeDB = gameObject.AddComponent<RealTimeDB>();
		ContinueBtn cb = gameObject.AddComponent<ContinueBtn>();
		if (menu == 0)  
		{  
			menu = 0;
			cb.ButtonOnClick();                                     // ?? ?? ?? ???  
		}  
		else if (menu == 1)  
		{  
			menu = 0;        
			gameObject2.transform.Translate(0, (float)0.18, 0);  
			SceneManager.LoadScene("03_Mapselect");                                  // ?? ?? ?? ???  
		}  
		else if (menu == 2)  
		{  
			menu = 0;  
			gameObject2.transform.Translate(0, (float)0.153, 0);    
			SceneManager.LoadScene("02_Menu");                // ? ?? ??? ??  
		} 
	}    


}
