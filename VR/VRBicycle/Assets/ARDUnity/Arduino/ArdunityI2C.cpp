/*
  ArdunityI2C.cpp - Ardunity Arduino library
  Copyright (C) 2015 ojh6t3k.  All rights reserved.
*/

//******************************************************************************
//* Includes
//******************************************************************************
#include <Wire.h>
#include "ArdunityI2C.h"


static boolean initialized = false;

//******************************************************************************
//* Constructors
//******************************************************************************
ArdunityI2C::ArdunityI2C(int id) : ArdunityController(id)
{
}

ArdunityI2C::ArdunityI2C(int id, int addr) : ArdunityController(id)
{
	address = (uint8_t)addr;
}

//******************************************************************************
//* Public Methods
//******************************************************************************


//******************************************************************************
//* Override Methods
//******************************************************************************
void ArdunityI2C::OnSetup()
{
	if(!initialized)
	{
		Wire.begin();
		initialized = true;
	}
}


//******************************************************************************
//* Protected Methods
//******************************************************************************
boolean ArdunityI2C::Write(uint8_t* data, uint8_t length)
{
	Wire.beginTransmission(address);
	for(uint8_t i=0; i<length; i++)
		Wire.write(data[i]);
	if(Wire.endTransmission() > 0)
		return false;
	
	return true;
}

boolean ArdunityI2C::Write(uint8_t data)
{
	return Write(&data, 1);
}

boolean ArdunityI2C::Write(uint8_t reg, uint8_t* data, uint8_t length)
{
	Wire.beginTransmission(address);
	Wire.write(reg);
	for(uint8_t i=0; i<length; i++)
		Wire.write(data[i]);
	if(Wire.endTransmission() > 0)
		return false;
	
	return true;
}

boolean ArdunityI2C::Write(uint8_t reg, uint8_t data)
{
	Wire.beginTransmission(address);
	Wire.write(reg);
	Wire.write(data);
	if(Wire.endTransmission() > 0)
		return false;
	
	return true;
}

boolean ArdunityI2C::Write(uint8_t addr, uint8_t reg, uint8_t* data, uint8_t length)
{
	Wire.beginTransmission(addr);
	Wire.write(reg);
	for(uint8_t i=0; i<length; i++)
		Wire.write(data[i]);
	if(Wire.endTransmission() > 0)
		return false;
	
	return true;
}

boolean ArdunityI2C::Read(uint8_t reg, uint8_t* buffer, uint8_t length)
{
	if(!Write(reg))
		return false;
	
	return Read(buffer, length);
}

boolean ArdunityI2C::Read(uint8_t* buffer, uint8_t length)
{
	Clear();
	
	Wire.requestFrom(address, length);
	
	unsigned long time = millis();
	int num = Wire.available();
	while(millis() - time < 2)
	{		
		if(num >= length)
		{
			for(uint8_t i=0; i<length; i++)
				buffer[i] = Wire.read();
			
			return true;
		}
		
		if(num != Wire.available())
		{
			num = Wire.available();
			time = millis();
		}
	}
	
	return false;
}

boolean ArdunityI2C::Read(uint8_t addr, uint8_t reg, uint8_t* buffer, uint8_t length)
{
	Wire.beginTransmission(addr);
	Wire.write(reg);
	if(Wire.endTransmission() > 0)
		return false;
	
	return Read(buffer, length);
}

void ArdunityI2C::Clear()
{
	uint8_t length = Wire.available();
	for(uint8_t i=0; i<length; i++)
		Wire.read();
}

//******************************************************************************
//* Private Methods
//******************************************************************************