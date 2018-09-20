//TFTLCD
#include <Time.h>
#include <LiquidCrystal.h>
#include <TimeLib.h>
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
Adafruit_ILI9340 tft = Adafruit_ILI9340(_cs, _dc, _rst);

String today;

int chmod=1; //lcd모드를 변경해줌
int bySpeed=18;//test
int distance = 200;//test
int beatsPerMinute = 120;//test
int btLCD_up, btLCD_down;
//Btn
#define BTLCD_UP 5//TFTLCD 위로 버튼
#define BTLCD_DOWN 4//TFTLCD 아래 버튼

//Thread
#include <Thread.h>
#include <ThreadController.h>

#include <Stepper.h>
ThreadController controll = ThreadController();
Thread myThread_tftLCD = Thread();
Thread myThread_btn = Thread();

void btLCDCallback(){
  btLCD_up = digitalRead(BTLCD_UP);
  btLCD_down = digitalRead(BTLCD_DOWN);

  if(btLCD_up == LOW){
    if(chmod>1001){
      chmod=1;
    }else{
    chmod +=1;
    }
  }
  if(btLCD_down == LOW){
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
    heartText();
  }
}

void setup() {
 #if defined (__AVR_ATtiny85__) 
  if (F_CPU == 16000000) clock_prescale_set(clock_div_1); 
  #endif 
  Serial.begin(9600);
  while (!Serial);
  pinMode(BTLCD_UP, INPUT);//tftLCD 버튼 up
  pinMode(BTLCD_DOWN, INPUT);//tftLCD 버튼 down
  setTime(01,37,0,10,8,18);
  date();
  Serial.println(today);
  
  tft.begin();
  tft.setRotation(3);

  //thread
  myThread_tftLCD.onRun(tftLCDCallback);
  myThread_btn.onRun(btLCDCallback);
  myThread_tftLCD.setInterval(1000);//tftLCD의 초기화 주기를 늦추기 위해서

  controll.add(&myThread_tftLCD);
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

unsigned int heartText(){
  tft.fillScreen(ILI9340_BLACK);
  unsigned long start = micros();
  
  tft.setCursor(85, 40);
  tft.setTextColor(ILI9340_WHITE);    tft.setTextSize(3);
  tft.println(today);

  tft.setCursor(85, 70);
  tft.setTextColor(ILI9340_WHITE);    tft.setTextSize(3);
  tft.println("Heart Rate");
  tft.println("");

  //심박도 출력
  if(beatsPerMinute <100){
    tft.setCursor(60, 110);
  }
  else if (beatsPerMinute >=100){
    tft.setCursor(70, 110);
  }

   tft.setTextColor(ILI9340_WHITE);    tft.setTextSize(8);
   tft.println(beatsPerMinute);

   tft.setCursor(120, 180);
   tft.setTextSize(3);
   tft.println("BPM");
   //손가락이 있는지 없는지 감지하는 부분 안넣었다.
   return micros() - start;
}
