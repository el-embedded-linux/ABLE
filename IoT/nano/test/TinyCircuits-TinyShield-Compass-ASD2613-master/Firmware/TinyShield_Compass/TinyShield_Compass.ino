//-------------------------------------------------------------------------------
//  TinyCircuits Compass TinyShield Example Sketch
//  Using Honeywell HMC5883 in I2C mode
//
//  Created 2/16/2014
//  by Ken Burns, TinyCircuits http://Tiny-Circuits.com
//
//  This example code is in the public domain.
//
//-------------------------------------------------------------------------------

#include <Wire.h>

#define HMC5883_I2CADDR     0x1E

int CompassX;
int CompassY;
int CompassZ;

void setup()
{
  Wire.begin();
  Serial.begin(9600);
  HMC5883nit();
}


void loop()
{
  HMC5883ReadCompass();

  // Print out the compass data
  Serial.print("x: ");
  Serial.print(CompassX);
  Serial.print(", y: ");
  Serial.print(CompassY);
  Serial.print(", z:");
  Serial.println(CompassZ);

  delay(1000);
}


void HMC5883nit()
{
  //Put the HMC5883 into operating mode
  Wire.beginTransmission(HMC5883_I2CADDR);
  Wire.write(0x02);     // Mode register
  Wire.write(0x00);     // Continuous measurement mode
  Wire.endTransmission();
}


void HMC5883ReadCompass()
{
  uint8_t ReadBuff[6];

  // Read the 6 data bytes from the HMC5883
  Wire.beginTransmission(HMC5883_I2CADDR);
  Wire.write(0x03);
  Wire.endTransmission();
  Wire.requestFrom(HMC5883_I2CADDR, 6);

  for (int i = 0; i < 6; i++)
  {
    ReadBuff[i] = Wire.read();
  }

  CompassX = ReadBuff[0] << 8;
  CompassX |= ReadBuff[1];

  CompassY = ReadBuff[4] << 8;
  CompassY |= ReadBuff[5];

  CompassZ = ReadBuff[2] << 8;
  CompassZ |= ReadBuff[3];
}



