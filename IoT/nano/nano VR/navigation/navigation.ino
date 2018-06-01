//RingLED
#include<Adafruit_NeoPixel.h>
#include<math.h>
#ifdef __AVR__
  #include <avr/power.h>
#endif
#define PIN 6
//지자기 센서는 뒤집어서 사용해야 한다.
//GND는 GND, VDD는 3V, SDA핀 A4, SCL핀 A5, 
#include "I2Cdev.h"
#include "MPU9250.h"
int16_t   mx, my, mz;
MPU9250 accelgyro;
I2Cdev   I2C_M;
uint8_t buffer_m[6];

float tiltheading;
int head;
int heading;

int i;
int j;
int temp_com;
int pump;
int test;
int randNumber;

volatile float mx_sample[3];
volatile float my_sample[3];
volatile float mz_sample[3];

static float mx_centre = 0;
static float my_centre = 0;
static float mz_centre = 0;

volatile int mx_max = 0;
volatile int my_max = 0;
volatile int mz_max = 0;

volatile int mx_min = 0;
volatile int my_min = 0;
volatile int mz_min = 0;

Adafruit_NeoPixel strip = Adafruit_NeoPixel(64, PIN, NEO_GRB + NEO_KHZ800);

// IMPORTANT: To reduce NeoPixel burnout risk, add 1000 uF capacitor across
// pixel power leads, add 300 - 500 Ohm resistor on first pixel's data input
// and minimize distance between Arduino and first pixel.  Avoid connecting
// on a live circuit...if you must, connect GND first.
float Mxyz[3];
float Axyz[3];

void setup() {
  // This is for Trinket 5V 16MHz, you can remove these three lines if you are not using a Trinket
  #if defined (__AVR_ATtiny85__)
    if (F_CPU == 16000000) clock_prescale_set(clock_div_1);
  #endif
  // End of trinket special code
  
  strip.begin();
  strip.show(); // Initialize all pixels to 'off'
 
  // join I2C bus (I2Cdev library doesn't do this automatically)
    Wire.begin();
    // it's really up to you depending on your project)
    Serial.begin(9600);

    // initialize device
    Serial.println("Initializing I2C devices...");
    accelgyro.initialize();

    // verify connection
    Serial.println("Testing device connections...");
    Serial.println(accelgyro.testConnection() ? "MPU9250 connection successful" : "MPU9250 connection failed");



    delay(1000);
    Serial.println("     ");

    //  Mxyz_init_calibrated ();
    
    //randomSeed의 매개변수로 0번 채널(A0번 핀)에서 읽은 아날로그 값을 전달
    randomSeed(analogRead(0));
}

void loop() {
randNumber = random(360);//randNumber 변수에 0 ~ 299 값 중 랜덤한 값을 저장
switch(randNumber){
  case 0 ... 22:j=1;break;
  case 23 ... 67:j=2;break;
  case 68 ... 112:j=3;break;
  case 113 ... 157:j=4;break;
  case 158 ... 202:j=5;break;
  case 203 ... 247:j=6;break;
  case 248 ... 292:j=7;break;
  case 293 ... 337:j=8;break;
  case 338 ... 360:j=1;break;
}
  while(true){
    getCompassDate_calibrated();
    getTiltHeading();
    Serial.print("가야하는 방향: ");
    switch(j){
      case 1:Serial.println("북");break;
      case 2:Serial.println("동북");break;
      case 3:Serial.println("동");break;
      case 4:Serial.println("남동");break;
      case 5:Serial.println("남");break;
      case 6:Serial.println("서남");break;
      case 7:Serial.println("서");break;
      case 8:Serial.println("북서");break;
    }
    Serial.print("각도: ");
    Serial.println(randNumber);
    Serial.print("내가 바라보고 있는 각도: ");
    head = (int)tiltheading;
    if(head<-360){//값이 0~360 범위안에 없는 경우
        head=head+360;
    }else if(head>360){
      head= head-360;
    }
    heading = head-randNumber;
    Serial.print("각도의 차이: ");
    Serial.println(heading);
    switch(heading){
      case -7 ... -1: i=0; break;
      case -22 ... -8: i=1; break;
      case -37 ... -23: i=2; break;
      case -52 ... -38: i=3; break;
      case -67 ... -53: i=4; break;
      case -82 ... -68: i=5; break;
      case -97 ... -83: i=6; break;
      case -112 ... -98: i=7; break;
      case -127 ... -113: i=8; break;
      case -142 ... -128: i=9; break;
      case -157 ... -143: i=10; break;
      case -172 ... -158: i=11; break;
      case -187 ... -173: i=12; break;
      case -202 ... -188: i=13; break;
      case -217 ... -203: i=14; break;
      case -232 ... -218: i=15; break;
      case -247 ... -233: i=16; break;
      case -262 ... -248: i=17; break;
      case -277 ... -263: i=18; break;
      case -292 ... -278: i=19; break;
      case -307 ... -293: i=20; break;
      case -322 ... -308: i=21; break;
      case -337 ... -323: i=22; break;
      case -352 ... -338: i=23; break;
      case -360 ... -353: i=0; break;
 /*---------------------------------------------*/
      case 0 ... 7: i=0; break;
      case 8 ... 22: i=23; break;
      case 23 ... 37: i=22; break;
      case 38 ... 52: i=21; break;
      case 53 ... 67: i=20; break;
      case 68 ... 82: i=19; break;
      case 83 ... 97: i=18; break;
      case 98 ... 112: i=17; break;
      case 113 ... 127: i=16; break;
      case 128 ... 142: i=15; break;
      case 143 ... 157: i=14; break;
      case 158 ... 172: i=13; break;
      case 173 ... 187: i=12; break;
      case 188 ... 202: i=11; break;
      case 203 ... 217: i=10; break;
      case 218 ... 232: i=9; break;
      case 233 ... 247: i=8; break;
      case 248 ... 262: i=7; break;
      case 263 ... 277: i=6; break;
      case 278 ... 292: i=5; break;
      case 293 ... 307: i=4; break;
      case 308 ... 322: i=3; break;
      case 323 ... 337: i=2; break;
      case 338 ... 352: i=1; break;
      case 353 ... 360: i=0; break;
      default: break;
    }

        colorWipe(temp_com,strip.Color(0, 0, 0), 0); // 전값을 초기화시키고
        colorWipe(i,strip.Color(0, 100, 0), 5); // 초록색으로 해당 픽셀 on

    delay(1000);
    temp_com=i;
    //test가 1000이 될 때마다 한 번씩 렌덤하게 각도를 받아온다.
    test+=1;
    Serial.print("타이머(1000이면 좌표 변경): ");
    Serial.println(test);
    Serial.println("");
    if(test==1000){
      break;
    }
  }
}

void colorWipe(int i, uint32_t c, uint8_t wait) {
  
    strip.setPixelColor(i, c);
    strip.show();
    delay(wait);
  
}

void getTiltHeading(void)
{
    float pitch = asin(-Axyz[0]);
    float roll = asin(Axyz[1] / cos(pitch));

    float xh = Mxyz[0] * cos(pitch) + Mxyz[2] * sin(pitch);
    float yh = Mxyz[0] * sin(roll) * sin(pitch) + Mxyz[1] * cos(roll) - Mxyz[2] * sin(roll) * cos(pitch);
    float zh = -Mxyz[0] * cos(roll) * sin(pitch) + Mxyz[1] * sin(roll) + Mxyz[2] * cos(roll) * cos(pitch);
    tiltheading = 180 * atan2(yh, xh)/PI;
    if (yh < 0)tiltheading += 360;
}

void getCompass_Data(void)
{
    I2C_M.writeByte(MPU9150_RA_MAG_ADDRESS, 0x0A, 0x01); //enable the magnetometer
    delay(10);
    I2C_M.readBytes(MPU9150_RA_MAG_ADDRESS, MPU9150_RA_MAG_XOUT_L, 6, buffer_m);

    mx = ((int16_t)(buffer_m[1]) << 8) | buffer_m[0] ;
    my = ((int16_t)(buffer_m[3]) << 8) | buffer_m[2] ;
    mz = ((int16_t)(buffer_m[5]) << 8) | buffer_m[4] ;

    Mxyz[0] = (double) mx * 1200 / 4096;
    Mxyz[1] = (double) my * 1200 / 4096;
    Mxyz[2] = (double) mz * 1200 / 4096;
}

void getCompassDate_calibrated ()
{
    getCompass_Data();
    Mxyz[0] = Mxyz[0] - mx_centre;
    Mxyz[1] = Mxyz[1] - my_centre;
    Mxyz[2] = Mxyz[2] - mz_centre;
}
