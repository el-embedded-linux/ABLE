#include <Adafruit_NeoPixel.h>
#include <LiquidCrystal.h>
#include <Time.h>
#include <TimeLib.h>
#include "SPI.h"
#include "Adafruit_GFX.h"
#include "Adafruit_ILI9340.h"

#define BTLCD_UP 5//TFTLCD 위로 버튼
#define BTLCD_DOWN 4//TFTLCD 아래 버튼

//TFT LCD
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
Adafruit_ILI9340 tft = Adafruit_ILI9340(_cs, _dc, _rst);

int distance=12;
int spd = 13;
String today;

float radius = 20;
float circle = (2*radius*3.14)/100;

float bySpeed = 0; // 자전거의 속도
float ckTime = 0;  // 리드스위치가 
float uckTime = 0; // Unckecked
float cycleTime = 0;  // 리드스위치가 인식이 안됬을 시간 부터 인식됬을 때까지의 시간
float byDistance = 0; // 자전거의 누적 이동 거리
boolean check;
boolean temp = 0; //리드 스위치가 닫혔는지 확인하는 변수
int count = 0;//리드스위치의 노이즈를 제거하기 위해 카운트를 넣어줍니다.


int chmod=1; //lcd모드를 변경해줌

ThreadController controll = ThreadController();
Thread myThread_reed = Thread();
Thread myThread_tftLCD = Thread();
Thread myThread_bpm = Thread();
Thread myThread_btn = Thread();

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

void btLCDCallback(){
    BTLCD_UP = digitalRead(BTLCD_UP);
    BTLCD_DOWN = digitalRead(BTLCD_DOWN);
  if(btLCD_UP == LOW){
    if(chmod>1001){
      chmod=1;
    }else{
    chmod +=1;
    }
  }
  if(BTLCD_DOWN == LOW){
    if(chmod>1001){
      chmod=1;
    }else{
    chmod -=1;
    }
  }
}

void tftLCDCallback(){
  if(chmod%3==0){
    distanceText();
  }else if(chmod%3==1){
    speedText();
  }else if(chmod%3==2){
    BPMText();
  }
}

void setup() {
 #if defined (__AVR_ATtiny85__) 
  if (F_CPU == 16000000) clock_prescale_set(clock_div_1); 
  #endif 
  Serial.begin(9600);
  while (!Serial);
  pinMode(BTLCD, INPUT);//LCD버튼
  setTime(01,37,0,10,8,18);
  matrix.begin();
  bar.begin();
  date();
  Serial.println(today);
  
  tft.begin();
  tft.setRotation(3);

  //thread
  myThread_btLCD.onRun(btLCDCallback);
  myThread_reed.onRun(reedCallback);
  myThread_tftLCD.onRun(tftLCDCallback);
  myThread_btn.onRun(btnCallback);
  myThread_tftLCD.setInterval(1000);//tftLCD의 초기화 주기를 늦추기 위해서


  controll.add(&myThread_reed);
  controll.add(&myThread_tftLCD);
  controll.add(&myThread_bpm);
  controll.add(&myThread_btn);

}

void loop(void) {
  controll.run();
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
