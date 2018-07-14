using UnityEngine;
using Ardunity;
using System.Diagnostics;
using Debug = UnityEngine.Debug;
using UnityEngine.UI;

public class NewBehaviourScript : MonoBehaviour
{
    Stopwatch sw = new Stopwatch();

    public static string curSpeed;
    public static string totalDist;
    //float radius = 20; // 바퀴당 이동 거리를 확인 하기 위해 자전거 바퀴의 반지름을 입력해 줍니다.(Cm 단위)
    float circle = (float)(2 * 35 * 3.14) / 100;  // 자전거 바퀴의 둘레를 계산(단위를 m로 바꿔주기 위해 100을 나눕니다.)

    public static float bySpeed = 0; // 자전거의 속도
    float ckTime = 0;  // 리드스위치가 
    float uckTime = 0; // Unckecked
    float cycleTime = 0;  // 리드스위치가 인식이 안됬을 시간 부터 인식됬을 때까지의 시간
    float distance = 0; // 자전거의 누적 이동 거리
    float lcdDis = 0; // 자전거의 이동 거리를 LCD출력에 맞게 바꿔즌 값.(단위 수정 or 소숫점 제거)
    float bicdis = 0;

    int count = 0;  // 리드스위치의 노이즈를 제거하기 위해 카운트를 넣어줍니다.
    bool temp = false;  // 리드 스위치가 닫혔는지 확인하는 변수

    public DigitalInput digitalInput;

    void Start()
    {
        sw.Start();
        digitalInput.OnValueChanged.AddListener(OnDigitalInputChanged);

    }
    void Update()
    {
        
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

    void OnDigitalInputChanged(bool value)
    {

        if (value == true && temp == false)
        {  // 리드 스위치가 열릴 때(닫힘 -> 열림)
            ckTime = sw.ElapsedMilliseconds;  // 시간을 확인해서 저장합니다.
            temp = true;  // temp값을 1로 바꿔줍니다.(리드스위치가 열려있는 상태값 저장)
        }

        else if (value == false && temp == true && count > 5)
        {  // 리드 스위치가 닫히고(열림 -> 닫힘), 노이즈 방지 카운트가 5이상일때
            uckTime = sw.ElapsedMilliseconds;   // 시간을 확인해서 저장합니다.

            bicdis += 0.0028f;
            totalDist = bicdis.ToString("f2") + "km";

            cycleTime = (uckTime - ckTime) / 1000;
            // 열릴 때 시각과 닫힐 때 시각의 차를 이용하여 바퀴가 한바퀴 돌때 걸린 시간을 계산합니다.
            bySpeed = (float)((circle / cycleTime) * 3.6); // 바퀴가 한바퀴 돌때의 거리와 시간을 가지고 속도를 구해줍니다.(단위는 Km/h입니다.)
            temp = false;
            count = 0;
            distance += circle;  // 한바퀴 돌았으면 이동거리를 누적 이동거리에 더해줍니다.
        }

        if (value == true)
        {  // 리드 스위치가 열려있으면 카운트를 1씩 증가 시켜 줍니다.
            count++;
            if (count > 150)
            { // 카운트가 150이 넘어가면(자전거가 멈췄을 때) 속도를 0으로 바꿔줍니다.
                bySpeed = 0;
            }
        }


        string realspeed = GetN2(bySpeed);
        curSpeed = realspeed + "km/h";
        Debug.Log("Speed: " + realspeed + "km/h");


    }

}
