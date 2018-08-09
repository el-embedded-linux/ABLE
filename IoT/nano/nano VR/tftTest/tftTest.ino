#include <Time.h>
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

int distance=12;
int spd = 13;
String today;

void setup() {
  Serial.begin(9600);
  while (!Serial);

  setTime(13,55,0,18,5,18);
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
  
  tft.setCursor(65, 40);
  tft.setTextColor(ILI9340_WHITE);    tft.setTextSize(3);
  tft.println(today);

  tft.setCursor(85, 70);
  tft.setTextColor(ILI9340_WHITE);    tft.setTextSize(3);
  tft.println("Distance");
  tft.println("");

  if(distance <10){
    tft.setCursor(70, 120);
  }
  else if (distance <100){
    tft.setCursor(60, 120);
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
  
  tft.setCursor(65, 40);
  tft.setTextColor(ILI9340_WHITE);    tft.setTextSize(3);
  tft.println("2018-05-17");

  tft.setCursor(110, 70);
  tft.setTextColor(ILI9340_WHITE);    tft.setTextSize(3);
  tft.println("Speed");
  tft.println("");


  //속도 출력
  if(spd <10){
    tft.setCursor(120, 110);
  }
  else if (spd <100){
    tft.setCursor(110, 110);
  }

   tft.setTextColor(ILI9340_WHITE);    tft.setTextSize(8);
   tft.println(spd);

   tft.setCursor(120, 180);
   tft.setTextSize(3);
   tft.println("km/s");

  return micros() - start;
}
