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
  //setTime(13,55,0,18,5,18);
  tft.begin();
  tft.setRotation(3);
  tft.fillScreen(ILI9340_BLACK);

  randomSeed(analogRead(A0));

  delay(1000);
}

void loop(void) {
    distance = random(10,20);
    distanceText();
}

unsigned long distanceText() {
  //tft.fillScreen(ILI9340_BLACK);
  unsigned long start = micros();
  
  tft.setCursor(65, 40);
  tft.setTextColor(ILI9340_WHITE);    tft.setTextSize(3);
  tft.println("2018-01-01");

  tft.setCursor(85, 70);
  tft.setTextColor(ILI9340_WHITE);    tft.setTextSize(3);
  tft.println("Distance");
  tft.println("");

  //tft.fillRect(0, 120, 320, 120, 000000);

  if(distance <10){
    tft.setCursor(70, 120);
  }
  else if (distance <100){
    tft.setCursor(60, 120);
  }
  else if (distance <1000){
    tft.setCursor(40, 120);
  }

   tft.setTextColor(ILI9340_WHITE, ILI9340_BLACK);    tft.setTextSize(8);
   tft.print(distance);
   tft.println("km");

  return micros() - start;
}
