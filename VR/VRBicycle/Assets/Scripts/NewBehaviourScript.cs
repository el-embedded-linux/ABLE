using UnityEngine;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using UnityEngine.UI;
using System.IO.Ports;
using UnityEngine.SceneManagement;

public class NewBehaviourScript : MonoBehaviour
{
    SerialPort sp = new SerialPort("COM5", 9600);
    Stopwatch sw = new Stopwatch();

    public static string curSpeed;
    public static string totalDist;
    //float radius = 20; // 바퀴당 이동 거리를 확인 하기 위해 자전거 바퀴의 반지름을 입력해 줍니다.(Cm 단위)
    float circle = (float)(2 * 20 * 3.14)/5;  // 자전거 바퀴의 둘레를 계산(단위를 m로 바꿔주기 위해 100을 나눕니다.)

    public static float bySpeed = 0; // 자전거의 속도  
    float ckTime = 0;  // 리드스위치가 
    float uckTime = 0; // Unckecked
    float cycleTime = 0;  // 리드스위치가 인식이 안됬을 시간 부터 인식됬을 때까지의 시간
    float distance = 0; // 자전거의 누적 이동 거리
    float lcdDis = 0; // 자전거의 이동 거리를 LCD출력에 맞게 바꿔즌 값.(단위 수정 or 소숫점 제거)

    float bicdis = 0;
   
    int count = 0;  // 리드스위치의 노이즈를 제거하기 위해 카운트를 넣어줍니다.
    int endCount = 0; //리드스위치 false일때 속도 체크
    bool temp = false;  // 리드 스위치가 닫혔는지 확인하는 변수
    bool value;
    
    int ct=0;
    private Menu menu2;
    public static int menu = 0;


    void Start()
    {
        sw.Start();
        sp.Open();
        sp.ReadTimeout = 1;

    }
    void Update()
    {
        if (sp.IsOpen)
        {
            try
            {
                int a = sp.ReadByte();
                if (a == 3)
                {

                    ButtonOnClick();
                }
                else if (a == 1)
                {
                    // Debug.Log("1");
                    DoSelect(a);
                }
                else if (a == 2)
                {
                    // Debug.Log("2");
                    DoSelect(a);
                }

                if (a == 4)
                {
                    
                    value=true;
                }
                else if (a == 5)
                {
                    // Debug.Log("1");
                    value = false;
                    ct++;
                    
                }


                if (value == true && temp == false)
                {  // 리드 스위치가 열릴 때(닫힘 -> 열림)
                    ckTime = sw.ElapsedMilliseconds;  // 시간을 확인해서 저장합니다.
                    temp = true;  // temp값을 1로 바꿔줍니다.(리드스위치가 열려있는 상태값 저장)
                    if (bySpeed == 0) //한바퀴에 속도가 나오지 않음
                    {
                        bySpeed = 5;
                    }
                }

                else if (value == false && temp == true && count > 5)
                {  // 리드 스위치가 닫히고(열림 -> 닫힘), 노이즈 방지 카운트가 5이상일때
                    uckTime = sw.ElapsedMilliseconds;   // 시간을 확인해서 저장합니다.

                    bicdis += 0.0028f;
                    totalDist = bicdis.ToString("f2");

                    cycleTime = (uckTime - ckTime) / 1000;
                    // 열릴 때 시각과 닫힐 때 시각의 차를 이용하여 바퀴가 한바퀴 돌때 걸린 시간을 계산합니다.
                    bySpeed = (float)((circle / cycleTime) * 3.6); // 바퀴가 한바퀴 돌때의 거리와 시간을 가지고 속도를 구해줍니다.(단위는 Km/h입니다.)
                    temp = false;
                    count = 0;
                    distance += circle;  // 한바퀴 돌았으면 이동거리를 누적 이동거리에 더해줍니다.
                    {

                    }

                }

                if (value == true)
                {  // 리드 스위치가 열려있으면 카운트를 1씩 증가 시켜 줍니다.
                    count++;
                    endCount = 0;
                    if (count > 25)
                    { // 카운트가 50이 넘어가면(자전거가 멈췄을 때) 속도를 0으로 바꿔줍니다.
                        bySpeed = 0;
                    }
                }

                if (!value)
                { //리드 스위치가 닫혀있을때
                    endCount++;
                    if (endCount > 25)
                    {
                        bySpeed = 0;
                    }
                }


                string realspeed = GetN2(bySpeed);
                curSpeed = realspeed + "km/h";
                Debug.Log("Speed: " + realspeed + "km/h");

            }
            catch (System.Exception) { }

        }



    }

    private string GetN2(float A)
    {
        string result = string.Empty;

        if (A == (int)A)
            result = A.ToString();
        else
            result = A.ToString("N1");

        return result;
    }
    void Awake()
    {
        menu2 = FindObjectOfType<Menu>();
    }

        public void ButtonOnClick()
    {
        menu2.gameObject.SetActive(true);
        menu2.SetShowFlag();
    }

    public void DoSelect(int a)
    {
        RealTimeDB realTimeDB = gameObject.AddComponent<RealTimeDB>();
        ContinueBtn cb = gameObject.AddComponent<ContinueBtn>();
        if (a == 1)
        {
            GameObject gameObject2;
            gameObject2 = GameObject.Find("Outline_m");
            if (menu == 0)                               // UI가 맨위에 있을 때는
            {
                gameObject2.transform.Translate(0, -70, 0);         // UI Y좌표를 -70
                menu = 1;
            }
            else if (menu == 1)
            {
                gameObject2.transform.Translate(0, -60, 0);
                menu = 2;
            }
            else if (menu == 2)
            {
                gameObject2.transform.Translate(0, -65, 0);
                menu = 3;
            }
            else if (menu == 3)                         // UI가 맨 아래에 있을 때는
            {
                gameObject2.transform.Translate(0, 195, 0);         // UI 좌표를 내린만큼 다시 올림
                menu = 0;
            }
        }
        else if (a == 2)
        {

            GameObject gameObject2;
            gameObject2 = GameObject.Find("Outline_m");

            if (menu == 0)
            {
                menu = 0;
                realTimeDB.InitDatabase();                              // FIrebase에 저장
            }
            else if (menu == 1)
            {
                menu = 0;
                gameObject2.transform.Translate(0, 60, 0);
                cb.ButtonOnClick();                                     // 메뉴 버튼 다시 내리기
            }
            else if (menu == 2)
            {
                menu = 0;
                gameObject2.transform.Translate(0, 125, 0);
                SceneManager.LoadScene("03_Mapselect");                 // 맵 선택 씬으로 이동
            }
            else if (menu == 3)
            {
                menu = 0;
                gameObject2.transform.Translate(0, 195, 0);
                SceneManager.LoadScene("02_Menu");                      // 메뉴 선택 씬으로 이동
            }
        }
    }



}
