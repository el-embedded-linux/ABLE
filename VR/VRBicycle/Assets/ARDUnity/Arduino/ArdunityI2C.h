/*
  ArdunityI2C.h - Ardunity Arduino library
  Copyright (C) 2015 ojh6t3k.  All rights reserved.
*/

#ifndef ArdunityI2C_h
#define ArdunityI2C_h

#include "ArdunityController.h"



class ArdunityI2C : public ArdunityController
{
public:
	ArdunityI2C(int id);
	ArdunityI2C(int id, int addr);

protected:
	uint8_t address;
	
	void OnSetup();
	boolean Write(uint8_t data);
	boolean Write(uint8_t* data, uint8_t length);
	boolean Write(uint8_t reg, uint8_t* data, uint8_t length);
	boolean Write(uint8_t reg, uint8_t data);
	boolean Write(uint8_t addr, uint8_t reg, uint8_t* data, uint8_t length);
	
	boolean Read(uint8_t* buffer, uint8_t length);
	boolean Read(uint8_t reg, uint8_t* buffer, uint8_t length);
	boolean Read(uint8_t addr, uint8_t reg, uint8_t* buffer, uint8_t length);
	void Clear();

private:

};

#endif

