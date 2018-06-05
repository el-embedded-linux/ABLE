/*
  DHT11.cpp - Ardunity Arduino library
  Copyright (C) 2016 ojh6t3k.  All rights reserved.
*/

//******************************************************************************
//* Includes
//******************************************************************************
#include "Ardunity.h"
#include "DHT11.h"

// Return values:
#define DHT11_OK   0
#define DHT11_ERROR_CHECKSUM    1
#define DHT11_ERROR_TIMEOUT     2

//******************************************************************************
//* Constructors
//******************************************************************************

DHT11::DHT11(int id, int pin) : ArdunityController(id)
{
	_pin = pin;
    canFlush = true;
}


//******************************************************************************
//* Override Methods
//******************************************************************************
void DHT11::OnSetup()
{
}

void DHT11::OnStart()
{
    _preTime = millis();
}

void DHT11::OnStop()
{	
}

void DHT11::OnProcess()
{
    if(started)
    {
        unsigned long curTime = millis();
        if((curTime - _preTime) >= 1000) // per 100 msec
        {
            if(read() == DHT11_OK) // Successful read
                dirty = true;

            _preTime = curTime;
        }
    }
}

void DHT11::OnUpdate()
{
}

void DHT11::OnExecute()
{
}

void DHT11::OnFlush()
{
	ArdunityApp.push(_humidity);
    ArdunityApp.push(_temperature);
}

int DHT11::read()
{
    // BUFFER TO RECEIVE
	uint8_t bits[5];
	uint8_t cnt = 7;
	uint8_t idx = 0;

	// EMPTY BUFFER
	for (int i=0; i< 5; i++)
        bits[i] = 0;

	// REQUEST SAMPLE
	pinMode(_pin, OUTPUT);
	digitalWrite(_pin, LOW);
	delay(18);
	digitalWrite(_pin, HIGH);
	delayMicroseconds(40);
	pinMode(_pin, INPUT);

	// ACKNOWLEDGE or TIMEOUT
	unsigned int loopCnt = 10000;
	while(digitalRead(_pin) == LOW)
		if (loopCnt-- == 0) return DHT11_ERROR_TIMEOUT;

	loopCnt = 10000;
	while(digitalRead(_pin) == HIGH)
		if (loopCnt-- == 0) return DHT11_ERROR_TIMEOUT;

	// READ OUTPUT - 40 BITS => 5 BYTES or TIMEOUT
	for (int i=0; i<40; i++)
	{
		loopCnt = 10000;
		while(digitalRead(_pin) == LOW)
			if (loopCnt-- == 0) return DHT11_ERROR_TIMEOUT;

		unsigned long t = micros();

		loopCnt = 10000;
		while(digitalRead(_pin) == HIGH)
			if (loopCnt-- == 0) return DHT11_ERROR_TIMEOUT;

		if ((micros() - t) > 40) bits[idx] |= (1 << cnt);
		if (cnt == 0)   // next byte?
		{
			cnt = 7;    // restart at MSB
			idx++;      // next byte!
		}
		else cnt--;
	}

    uint8_t sum = bits[0] + bits[2];
	if (bits[4] != sum)
        return DHT11_ERROR_CHECKSUM;

	// WRITE TO RIGHT VARS
    // as bits[1] and bits[3] are allways zero they are omitted in formulas.
    _humidity    = (UINT8)bits[0]; 
	_temperature = (UINT8)bits[2];	
    
	return DHT11_OK;
}
