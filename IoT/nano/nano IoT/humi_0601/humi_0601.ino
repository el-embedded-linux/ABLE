#include <Adafruit_NeoPixel.h>

#define ADAMATRIX 10  // 매트릭스 LED
#define BARWING 7
//#define LEFTPIN   9   // 바LED 좌측
//#define RIGHTPIN  8   // 바LED 우측
#define BTL 12        // 좌측 버튼
#define BTR 11        // 우측 버튼
#define NUMPIXELS 64  // 매트릭스 LED개수
#define BARPIXELS 28  // 바 LED개수
#define PRESSURE A0

// LED 개수, 아두이노 디지털 핀, 네오픽셀 컬러 타입 + 네오픽셀 클럭
Adafruit_NeoPixel matrix = Adafruit_NeoPixel(NUMPIXELS, ADAMATRIX, NEO_GRB + NEO_KHZ800);
Adafruit_NeoPixel bar = Adafruit_NeoPixel(BARPIXELS, BARWING, NEO_GRB + NEO_KHZ800);

int btLeft, btRight;  // 좌우 버튼
int c = 0;            // 버튼 동시 제어 변수
int presRead;         // 압력센서
int presPoint;

int humi[6][64]={
  { 0, 0, 0, 1, 0, 0, 0, 0,
    0, 0, 1, 1, 0, 0, 0, 0,
    0, 1, 1, 0, 0, 0, 0, 0,
    1, 1, 1, 1, 1, 1, 1, 1,
    1, 1, 1, 1, 1, 1, 1, 1,
    0, 1, 1, 0, 0, 0, 0, 0,
    0, 0, 1, 1, 0, 0, 0, 0,
    0, 0, 0, 1, 0, 0, 0, 0
  }, 
  { 0, 0, 0, 0, 1, 0, 0, 0,
    0, 0, 0, 0, 1, 1, 0, 0,
    0, 0, 0, 0, 0, 1, 1, 0,
    1, 1, 1, 1, 1, 1, 1, 1,
    1, 1, 1, 1, 1, 1, 1, 1,
    0, 0, 0, 0, 0, 1, 1, 0,
    0, 0, 0, 0, 1, 1, 0, 0,
    0, 0, 0, 0, 1, 0, 0, 0
  },
  { 1, 1, 1, 1, 1, 1, 1, 1,
    1, 1, 0, 0, 0, 0, 1, 1,
    1, 0, 1, 0, 0, 1, 0, 1,
    1, 0, 0, 1, 1, 0, 0, 1,
    1, 0, 0, 1, 1, 0, 0, 1,
    1, 0, 1, 0, 0, 1, 0, 1,
    1, 1, 0, 0, 0, 0, 1, 1,
    1, 1, 1, 1, 1, 1, 1, 1
  },
  { 0, 0, 0, 1, 1, 0, 0, 0,
    0, 0, 1, 0, 0, 1, 0, 0,
    0, 0, 1, 0, 0, 1, 0, 0,
    0, 0, 1, 0, 0, 1, 0, 0,
    0, 1, 0, 0, 0, 0, 1, 0,
    0, 1, 0, 0, 0, 0, 1, 0,
    1, 0, 0, 0, 0, 0, 0, 1,
    1, 1, 1, 1, 1, 1, 1, 1
  },
  { 0, 0, 0, 0, 0, 0, 0, 0,
    0, 0, 0, 0, 0, 0, 0, 0,
    0, 0, 0, 0, 0, 0, 0, 0,
    0, 0, 0, 0, 0, 0, 0, 0,
    0, 0, 0, 0, 0, 0, 0, 0,
    0, 0, 0, 0, 0, 0, 0, 0,
    0, 0, 0, 0, 0, 0, 0, 0,
    0, 0, 0, 0, 0, 0, 0, 0
  },
  { 1, 1, 1, 1, 1, 1, 1, 1,
    1, 1, 1, 1, 1, 1, 1, 1,
    1, 1, 1, 1, 1, 1, 1, 1,
    1, 1, 1, 1, 1, 1, 1, 1,
    1, 1, 1, 1, 1, 1, 1, 1,
    1, 1, 1, 1, 1, 1, 1, 1,
    1, 1, 1, 1, 1, 1, 1, 1,
    1, 1, 1, 1, 1, 1, 1, 1
  }
};

int matrixColor[6][3] = {
  {0, 20, 0},
  {0, 20, 0},
  {20, 0, 0},
  {20, 0, 0},
  {0, 0, 0},
  {0, 20, 0}
};

int barShape[3][28] = {
  { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
  { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 },
  { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 }
};

int barColor[3][3] = {
  {0, 20, 0}, {0, 20, 0}, {20, 0, 0}
};

void setup() {
  #if defined (__AVR_ATtiny85__) 
  if (F_CPU == 16000000) clock_prescale_set(clock_div_1); 
  #endif 
  Serial.begin(9600);
  matrix.begin();
  bar.begin();
  pinMode(ADAMATRIX, OUTPUT); // 매트릭스 핀
  pinMode(BARWING, OUTPUT);   // 바 핀
  pinMode(BTL, INPUT); // 좌측 버튼
  pinMode(BTR, INPUT); // 우측 버튼

}

void loop() {
  btLeft = digitalRead(BTL);
  btRight = digitalRead(BTR);
 
// 매트릭스 LED + 바 LED
  showMatrix(btLeft, btRight, c);
//  showPres(presRead, PRESSURE, presPoint);
}
/*
void showPres(int presRead1, int PRESSURE1, int presPoint1)
{
  presRead1 = analogRead(PRESSURE1);
  presPoint1 = map(presRead1, 0, 1024, 0,255);
  Serial.println(presPoint1);
  delay(100);
  if(presPoint1 >= 15) {
    for(int i=0;i<64;i++) {
      matrix.setPixelColor(i, matrix.Color(humi[2][i]*matrixColor[2][0], humi[2][i]*matrixColor[2][1], humi[2][i]*matrixColor[2][2]));
    }
    for(int i=0;i<28;i++) {
      bar.setPixelColor(i,bar.Color(barShape[2][i]*barColor[2][0], barShape[2][i]*barColor[2][1], barShape[2][i]*barColor[2][2]));
    }
  } else {
    for(int i=0;i<64;i++) {
      matrix.setPixelColor(i, matrix.Color(0,0,0));
    }
    for(int i=0;i<28;i++) {
      bar.setPixelColor(i,bar.Color(0,0,0));
    }
  }
}

*/

void showMatrix(int btLeft1, int btRight1, int c1)
{
  if(btLeft1 == HIGH && c1 == 0) {
    for(int i=0;i<64;i++) {
      matrix.setPixelColor(i, matrix.Color(humi[0][i]*matrixColor[0][0], humi[0][i]*matrixColor[0][1], humi[0][i]*matrixColor[0][2]));
    }
    for(int i=0;i<28;i++) {
      bar.setPixelColor(i,bar.Color(barShape[0][i]*barColor[0][0], barShape[0][i]*barColor[0][1], barShape[0][i]*barColor[0][2]));
    }
    c = 1;
  }  if(btLeft1 == HIGH && c1 == 1) {
    for(int i=0;i<64;i++) {
      matrix.setPixelColor(i, matrix.Color(0,0,0));
    }
    for(int i=0;i<28;i++) {
      bar.setPixelColor(i,bar.Color(0,0,0));
    }
    c = 0;
  }
  if(btRight1 == HIGH && c1 == 0) {
    for(int i=0;i<64;i++) {
      matrix.setPixelColor(i, matrix.Color(humi[1][i]*matrixColor[1][0], humi[1][i]*matrixColor[1][1], humi[1][i]*matrixColor[1][2]));
    }
    for(int i=0;i<28;i++) {
      bar.setPixelColor(i,bar.Color(barShape[1][i]*barColor[0][0], barShape[1][i]*barColor[0][1], barShape[1][i]*barColor[0][2]));
    }
    c = 1;
  }  if(btRight1 == HIGH && c1 == 1) {
    for(int i=0;i<64;i++) {
      matrix.setPixelColor(i, matrix.Color(0,0,0));
    }
    for(int i=0;i<28;i++) {
      bar.setPixelColor(i,bar.Color(0,0,0));
    }
    c = 0;
  }
  matrix.show();
  bar.show();
}
  
