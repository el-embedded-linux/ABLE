#include <Time.h>
#include <TimeLib.h>
#include <LiquidCrystal.h>
#include <Adafruit_NeoPixel.h>
#include "SPI.h"
#include "Adafruit_GFX.h"
#include "Adafruit_ILI9340.h"

#if defined(__SAM3X8E__)
    #undef __FlashStringHelper::F(string_literal)
    #define F(string_literal) string_literal
#endif

#define _sclk 13
#define _miso 12
#define _mosi 11
#define _cs 10
#define _dc 9
#define _rst 8

#define BTLCD 5      //LCD버튼


Adafruit_ILI9340 tft = Adafruit_ILI9340(_cs, _dc, _rst);

String today;

float radius = 20; // 바퀴당 이동 거리를 확인 하기 위해 자전거 바퀴의 반지름을 입력해 줍니다.(Cm 단위)
float circle = (2 * radius * 3.14) / 100;  // 자전거 바퀴의 둘레를 계산(단위를 m로 바꿔주기 위해 100을 나눕니다.)

float bySpeed = 0; // 자전거의 속도
float ckTime = 0;  // 리드스위치가 
float uckTime = 0; // Unckecked
float cycleTime = 0;  // 리드스위치가 인식이 안됬을 시간 부터 인식됬을 때까지의 시간
float distance = 0; // 자전거의 누적 이동 거리
float lcdDis = 0; // 자전거의 이동 거리를 LCD출력에 맞게 바꿔즌 값.(단위 수정 or 소숫점 제거)

int count = 0;  // 리드스위치의 노이즈를 제거하기 위해 카운트를 넣어줍니다.
boolean temp = 0;  // 리드 스위치가 닫혔는지 확인하는 변수

// LED 개수, 아두이노 디지털 핀, 네오픽셀 컬러 타입 + 네오픽셀 클럭
Adafruit_NeoPixel matrix = Adafruit_NeoPixel(NUMPIXELS, ADAMATRIX, NEO_GRB + NEO_KHZ800);
Adafruit_NeoPixel bar = Adafruit_NeoPixel(BARPIXELS, BARWING, NEO_GRB + NEO_KHZ800);

int btLeft, btRight, btLCD;  // 좌우 버튼
int c = 0;            // 버튼 동시 제어 변수
int presRead;         // 압력센서
int presPoint;

void setup() {
  Serial.begin(9600);
  while (!Serial);

  setTime(01,37,0,10,8,18);
  date();
  Serial.println(today);
  
  tft.begin();
  tft.setRotation(3);

  Serial.print(F("Text                     "));
  Serial.println(distanceText());
  delay(3000);

  Serial.println(F("Done!"));
}

void loop(void) {
  //걍 테스트용으로 번갈아가면서 출력하게 했음
  boolean check = digitalRead(A1); // 리드스위치의 상태를 확인합니다.(SIG=A0)
  Serial.print(check);
  if(check == 1 && temp == 0){  // 리드 스위치가 열릴 때(닫힘 -> 열림)
    ckTime = millis();  // 시간을 확인해서 저장합니다.
    temp = 1;  // temp값을 1로 바꿔줍니다.(리드스위치가 열려있는 상태값 저장)
  }
  
  else if(check == 0 && temp == 1 && count > 5){  // 리드 스위치가 닫히고(열림 -> 닫힘), 노이즈 방지 카운트가 5이상일때
    uckTime = millis();  // 시간을 확인해서 저장합니다.
      
    cycleTime = (uckTime - ckTime) / 1000;
    // 열릴 때 시각과 닫힐 때 시각의 차를 이용하여 바퀴가 한바퀴 돌때 걸린 시간을 계산합니다.
    bySpeed = (circle / cycleTime) * 3.6; // 바퀴가 한바퀴 돌때의 거리와 시간을 가지고 속도를 구해줍니다.(단위는 Km/h입니다.)
    temp = 0; 
    count = 0; 
    distance += circle;  // 한바퀴 돌았으면 이동거리를 누적 이동거리에 더해줍니다.
  }
  
  if(check == 1){  // 리드 스위치가 열려있으면 카운트를 1씩 증가 시켜 줍니다.
    count++;
    if(count > 150){ // 카운트가 150이 넘어가면(자전거가 멈췄을 때) 속도를 0으로 바꿔줍니다.
      bySpeed = 0;
    }
  }
  distanceText();
  delay(1000);
  speedText();
  delay(1000);
}

void date(){
  today=year();
  today = today+"-";
  today = today+month();
  today = today+"-";
  today = today+day();
}

unsigned long testFillScreen() {
  unsigned long start = micros();
  tft.fillScreen(ILI9340_BLACK);
  tft.fillScreen(ILI9340_RED);
  return micros() - start;
}

unsigned long distanceText() {
  tft.fillScreen(ILI9340_BLACK);
  unsigned long start = micros();
  
  tft.setCursor(85, 40);
  tft.setTextColor(ILI9340_WHITE);    tft.setTextSize(3);
  tft.println(today);

  tft.setCursor(85, 70);
  tft.setTextColor(ILI9340_WHITE);    tft.setTextSize(3);
  tft.println("Distance");
  tft.println("");

  if(distance <10){
    tft.setCursor(20, 120);
  }
  else if (distance <100){
    tft.setCursor(10, 120);
  }
  else if (distance <1000){
    tft.setCursor(40, 120);
  }

   tft.setTextColor(ILI9340_WHITE);    tft.setTextSize(8);
   tft.print(distance);
   tft.println("km");

  return micros() - start;
}



unsigned long speedText() {
  tft.fillScreen(ILI9340_BLACK);
  unsigned long start = micros();
  
  tft.setCursor(85, 40);
  tft.setTextColor(ILI9340_WHITE);    tft.setTextSize(3);
  tft.println(today);

  tft.setCursor(110, 70);
  tft.setTextColor(ILI9340_WHITE);    tft.setTextSize(3);
  tft.println("Speed");
  tft.println("");


  //속도 출력
  if(bySpeed <10){
    tft.setCursor(50, 110);
  }
  else if (bySpeed <100){
    tft.setCursor(60, 110);
  }

   tft.setTextColor(ILI9340_WHITE);    tft.setTextSize(8);
   tft.println(bySpeed);

   tft.setCursor(120, 180);
   tft.setTextSize(3);
   tft.println("km/s");

  return micros() - start;
}
