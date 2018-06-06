/*************************************************** 
  This is a example sketch demonstrating graphic drawing
  capabilities of the SSD1351 library for the 1.5" 
  and 1.27" 16-bit Color OLEDs with SSD1351 driver chip

  Pick one up today in the adafruit shop!
  ------> http://www.adafruit.com/products/1431
  ------> http://www.adafruit.com/products/1673
 
  If you're using a 1.27" OLED, change SSD1351HEIGHT in Adafruit_SSD1351.h
 	to 96 instead of 128

  These displays use SPI to communicate, 4 or 5 pins are required to  
  interface
  Adafruit invests time and resources providing this open source code, 
  please support Adafruit and open-source hardware by purchasing 
  products from Adafruit!

  Written by Limor Fried/Ladyada for Adafruit Industries.  
  BSD license, all text above must be included in any redistribution

  The Adafruit GFX Graphics core library is also required
  https://github.com/adafruit/Adafruit-GFX-Library
  Be sure to install it!
 ****************************************************/

// You can use any (4 or) 5 pins 
#define sclk 3
#define mosi 2
#define dc   4
#define cs   6
#define rst  5

// Color definitions
#define	BLACK           0x0000
#define	BLUE            0x001F
#define	RED             0xF800
#define	GREEN           0x07E0
#define CYAN            0x07FF
#define MAGENTA         0xF81F
#define YELLOW          0xFFE0  
#define WHITE           0xFFFF

#define buttonPin 9

#include <Adafruit_GFX.h>
#include <Adafruit_SSD1351.h>
#include <SPI.h>
#include <TimeLib.h>

int i = 15;
int triger = 1;
int buttonState = 0; 
// Option 1: use any pins but a little slower
Adafruit_SSD1351 tft = Adafruit_SSD1351(cs, dc, mosi, sclk, rst);  

// Option 2: must use the hardware SPI pins 
// (for UNO thats sclk = 13 and sid = 11) and pin 10 must be 
// an output. This is much faster - also required if you want
// to use the microSD card (see the image drawing example)
//Adafruit_SSD1351 tft = Adafruit_SSD1351(cs, dc, rst);

void fillpixelbypixel(uint16_t color) {
  for (uint8_t x=0; x < tft.width(); x++) {
    for (uint8_t y=0; y < tft.height(); y++) {
      tft.drawPixel(x, y, color);
    }
  }
  delay(100);
}

void setup(void) {
  Serial.begin(9600);
  pinMode(buttonPin, INPUT_PULLUP);
  Serial.print("hello!");
  tft.begin();

  Serial.println("init");

  // You can optionally rotate the display by running the line below.
  // Note that a value of 0 means no rotation, 1 means 90 clockwise,
  // 2 means 180 degrees clockwise, and 3 means 270 degrees clockwise.
  //tft.setRotation(1);
  // NOTE: The test pattern at the start will NOT be rotated!  The code
  // for rendering the test pattern talks directly to the display and
  // ignores any rotation.

  Serial.println("done");
  setTime(17, 39, 0, 2, 6, 18);
  tft.fillScreen(BLACK);
  speedPrint(i);
  
}

void loop() {
  buttonState = digitalRead(buttonPin);
  if(buttonState == LOW){
    change();
  }
  if(triger>1)  triger=0;
}
void change() {
  if(triger == 0) {
    speedPrint(i);
    triger = 1;
  } else {
    distancePrint();
    triger = 0;
  }
}
void printDigits(int digits){
  if(digits < 10)
    tft.print('0');
  tft.print(digits);
}
void distancePrint() {
  tft.fillScreen(BLACK);
  tft.setCursor(5, 5);
  tft.setTextColor(WHITE);  
  tft.setTextSize(2);
  tft.print(year());
  tft.print("/");
  printDigits(month());
  tft.print("/");
  printDigits(day());
  tft.setCursor(18, 30);
  tft.setTextColor(YELLOW);  
  tft.setTextSize(2);
  tft.println("Distance");
  tft.setCursor(18, 60);
  tft.setTextColor(BLUE);  
  tft.setTextSize(4);
  tft.print("23.2");
  tft.setCursor(53, 100);
  tft.setTextColor(BLUE);  
  tft.setTextSize(2);
  tft.print("km");
  //delay(2000);
}
void speedPrint(int speed){
  tft.fillScreen(BLACK);
  tft.setCursor(5, 5);
  tft.setTextColor(WHITE);  
  tft.setTextSize(2);
  tft.print(year());
  tft.print("/");
  printDigits(month());
  tft.print("/");
  printDigits(day());
  tft.setCursor(35, 30);
  tft.setTextColor(YELLOW);  
  tft.setTextSize(2);
  tft.println("Speed");
  tft.setCursor(25, 60);
  tft.setTextColor(BLUE);  
  tft.setTextSize(5);
  tft.print(speed);
  tft.setCursor(85, 85);
  tft.setTextColor(BLUE);  
  tft.setTextSize(1);
  tft.print("km/h");
  i=i+1;
  if(i>30)  i=15;
  //delay(2000);
}


