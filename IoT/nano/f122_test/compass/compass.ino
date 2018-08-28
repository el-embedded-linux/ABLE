//RingLED
#include<Adafruit_NeoPixel.h>
#ifdef __AVR__
  #include <avr/power.h>
#endif
#define PIN 6
//지자기
//GND는 GND, VDD는 3V, SDA핀 A4, SCL핀 A5, 
#include "I2Cdev.h"
#include "MPU9250.h"
int16_t   mx, my, mz;
MPU9250 accelgyro;
I2Cdev   I2C_M;

uint8_t buffer_m[6];

float heading;
float tiltheading;
int head;

int i;
int temp_com;
int pump;
int test;
int randNumber;

//intial num : 5000
#define sample_num_mdate  500

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

    // Mxyz_init_calibrated ();
    get_calibration_Data (); 
    
}
//ToDoList
//1. 값이 갑자기 너무 커지는 경우에는 반응하지 않는다.
//2. 값이 너무 조금 움이는 경우에는 반응하지 않는다.
void loop() {
  
    getCompassDate_calibrated(); 
    getTiltHeading();
    Serial.println("The clockwise angle between the magnetic north and X-Axis:");
    Serial.println(tiltheading); //주석
    Serial.println(" ");
    head = (int)tiltheading;
    if(head<0){//값이 0~360 범위안에 없는 경우
        head=head+360;
    }else if(head>360){
      head= head-360;
    }
    Serial.println(head);
    switch(head){

      case 0 ... 14: i=0; break;
      case 15 ... 29: i=1; break;
      case 30 ... 44: i=2; break;
      case 45 ... 59: i=3; break;
      case 60 ... 74: i=4; break;
      case 75 ... 89: i=5; break;
      case 90 ... 104: i=6; break;
      case 105 ... 119: i=7; break;
      case 120 ... 134: i=8; break;
      case 135 ... 149: i=9; break;
      case 150 ... 164: i=10; break;
      case 165 ... 179: i=11; break;
      case 180 ... 194: i=12; break;
      case 195 ... 209: i=13; break;
      case 210 ... 224: i=14; break;
      case 225 ... 239: i=15; break;
      case 240 ... 254: i=16; break;
      case 255 ... 269: i=17; break;
      case 270 ... 284: i=18; break;
      case 285 ... 299: i=19; break;
      case 300 ... 314: i=20; break;
      case 315 ... 329: i=21; break;
      case 330 ... 344: i=22; break;
      case 345 ... 359: i=23; break;
      default: break;
    }
        colorWipe(temp_com,strip.Color(0, 0, 0), 0); // 전값을 초기화시키고
        colorWipe(i,strip.Color(0, 100, 0), 5); // 초록색으로 해당 픽셀 on
    delay(100);
    temp_com=i;
}

void colorWipe(int i, uint32_t c, uint8_t wait) {
  
    strip.setPixelColor(i, c);
    strip.show();
    delay(wait);
  
}
/*
void getHeading(void)
{
    heading = 180 * atan2(Mxyz[1], Mxyz[0])/PI;
    if (heading < 0)heading += 360;
}
*/

void getTiltHeading(void)
{
    float pitch = asin(-Axyz[0]);
    float roll = asin(Axyz[1] / cos(pitch));

    float xh = Mxyz[0] * cos(pitch) + Mxyz[2] * sin(pitch);
    float yh = Mxyz[0] * sin(roll) * sin(pitch) + Mxyz[1] * cos(roll) - Mxyz[2] * sin(roll) * cos(pitch);
    float zh = -Mxyz[0] * cos(roll) * sin(pitch) + Mxyz[1] * sin(roll) + Mxyz[2] * cos(roll) * cos(pitch);
    tiltheading = 180 * atan2(yh, xh)/PI;
    if (yh < 0) tiltheading += 360;
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

/*
void Mxyz_init_calibrated ()
{

    Serial.println(F("Before using 9DOF,we need to calibrate the compass frist,It will takes about 2 minutes."));
    Serial.print("  ");
    Serial.println(F("During  calibratting ,you should rotate and turn the 9DOF all the time within 2 minutes."));
    Serial.print("  ");
    Serial.println(F("If you are ready ,please sent a command data 'ready' to start sample and calibrate."));
    while (!Serial.find("ready"));
    Serial.println("  ");
    Serial.println("ready");
    Serial.println("Sample starting......");
    Serial.println("waiting ......");

    get_calibration_Data ();

    Serial.println("     ");
    Serial.println("compass calibration parameter ");
    Serial.print(mx_centre);
    Serial.print("     ");
    Serial.print(my_centre);
    Serial.print("     ");
    Serial.println(mz_centre);
    Serial.println("    ");
}
*/

void get_calibration_Data ()
{
    for (int i = 0; i < sample_num_mdate; i++)
    {
        get_one_sample_date_mxyz();
        /*
        Serial.print(mx_sample[2]);
        Serial.print(" ");
        Serial.print(my_sample[2]);                            //you can see the sample data here .
        Serial.print(" ");
        Serial.println(mz_sample[2]);
        */
        if (mx_sample[2] >= mx_sample[1])mx_sample[1] = mx_sample[2];
        if (my_sample[2] >= my_sample[1])my_sample[1] = my_sample[2]; //find max value
        if (mz_sample[2] >= mz_sample[1])mz_sample[1] = mz_sample[2];

        if (mx_sample[2] <= mx_sample[0])mx_sample[0] = mx_sample[2];
        if (my_sample[2] <= my_sample[0])my_sample[0] = my_sample[2]; //find min value
        if (mz_sample[2] <= mz_sample[0])mz_sample[0] = mz_sample[2];

    }

    mx_max = mx_sample[1];
    my_max = my_sample[1];
    mz_max = mz_sample[1];

    mx_min = mx_sample[0];
    my_min = my_sample[0];
    mz_min = mz_sample[0];



    mx_centre = (mx_max + mx_min) / 2;
    my_centre = (my_max + my_min) / 2;
    mz_centre = (mz_max + mz_min) / 2;

}

void get_one_sample_date_mxyz()
{
    getCompass_Data();
    mx_sample[2] = Mxyz[0];
    my_sample[2] = Mxyz[1];
    mz_sample[2] = Mxyz[2];
}
