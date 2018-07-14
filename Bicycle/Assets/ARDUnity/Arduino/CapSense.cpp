/*
  CapSense.cpp - Ardunity Arduino library
  Copyright (C) 2016 ojh6t3k.  All rights reserved.
*/

/*
 This code was implemented with reference to the "CapacitiveSensor" library.
 https://github.com/PaulStoffregen/CapacitiveSensor
 http://www.pjrc.com/teensy/td_libs_CapacitiveSensor.html
 http://playground.arduino.cc/Main/CapacitiveSensor
 Thanks for Paul Bagder
 */


//******************************************************************************
//* Includes
//******************************************************************************
#include "Ardunity.h"
#include "CapSense.h"

#define SAMPLE_NUM  5
#define TIMEOUT_NUM 5


//******************************************************************************
//* Constructors
//******************************************************************************

CapSense::CapSense(int id, int send, int receive) : ArdunityController(id)
{
	_send = send;
    _receive = receive;
    canFlush = true;
    
    error = 1;
    loopTimingFactor = 310;		// determined empirically -  a hack
    
    CS_Timeout_Millis = (TIMEOUT_NUM * (float)loopTimingFactor * (float)F_CPU) / 16000000;
    CS_AutocaL_Millis = 20000;
    
    sBit = PIN_TO_BITMASK(_send);					// get send pin's ports and bitmask
    sReg = PIN_TO_BASEREG(_send);					// get pointer to output register
    
    rBit = PIN_TO_BITMASK(_receive);				// get receive pin's ports and bitmask
    rReg = PIN_TO_BASEREG(_receive);
    
#ifdef NUM_DIGITAL_PINS
    if(_send >= NUM_DIGITAL_PINS)
        error = -1;
    if(_receive >= NUM_DIGITAL_PINS)
        error = -1;
#endif
}


//******************************************************************************
//* Override Methods
//******************************************************************************
void CapSense::OnSetup()
{
    pinMode(_send, OUTPUT);
    pinMode(_receive, INPUT);
    digitalWrite(_send, LOW);
}

void CapSense::OnStart()
{
    leastTotal = 0x0FFFFFFFL;   // input large value for autocalibrate begin
    lastCal = millis();         // set millis for start
}

void CapSense::OnStop()
{	
}

void CapSense::OnProcess()
{
    if(started)
    {
        if(error < 0)
            return;
        
        unsigned long  total = 0;
        for(int i=0; i<SAMPLE_NUM; i++)
        {
            noInterrupts();
            DIRECT_WRITE_LOW(sReg, sBit);	// sendPin Register low
            DIRECT_MODE_INPUT(rReg, rBit);	// receivePin to input (pullups are off)
            DIRECT_MODE_OUTPUT(rReg, rBit); // receivePin to OUTPUT
            DIRECT_WRITE_LOW(rReg, rBit);	// pin is now LOW AND OUTPUT
            delayMicroseconds(10);
            DIRECT_MODE_INPUT(rReg, rBit);	// receivePin to input (pullups are off)
            DIRECT_WRITE_HIGH(sReg, sBit);	// sendPin High
            interrupts();
            
            while(!DIRECT_READ(rReg, rBit) && (total < CS_Timeout_Millis))
            {
                // while receive pin is LOW AND total is positive value
                total++;
            }
            
            if (total > CS_Timeout_Millis)
            {
                //  total variable over timeout
                return;
            }
            
            // set receive pin HIGH briefly to charge up fully - because the while loop above will exit when pin is ~ 2.5V
            noInterrupts();
            DIRECT_WRITE_HIGH(rReg, rBit);
            DIRECT_MODE_OUTPUT(rReg, rBit);  // receivePin to OUTPUT - pin is now HIGH AND OUTPUT
            DIRECT_WRITE_HIGH(rReg, rBit);
            DIRECT_MODE_INPUT(rReg, rBit);	// receivePin to INPUT (pullup is off)
            DIRECT_WRITE_LOW(sReg, sBit);	// sendPin LOW
            interrupts();
            
#ifdef FIVE_VOLT_TOLERANCE_WORKAROUND
            DIRECT_MODE_OUTPUT(rReg, rBit);
            DIRECT_WRITE_LOW(rReg, rBit);
            delayMicroseconds(10);
            DIRECT_MODE_INPUT(rReg, rBit);	// receivePin to INPUT (pullup is off)
#else
            while(DIRECT_READ(rReg, rBit) && (total < CS_Timeout_Millis))
            {
                // while receive pin is HIGH  AND total is less than timeout
                total++;
            }
#endif
            
            if(total >= CS_Timeout_Millis)
            {
                // total variable over timeout
                return;
            }
        }
        
        if((millis() - lastCal > CS_AutocaL_Millis) && abs(total - leastTotal) < (int)(.10 * (float)leastTotal))
        {
            leastTotal = 0x0FFFFFFFL;          // reset for "autocalibrate"
            lastCal = millis();
        }
        
        if (total < leastTotal)
            leastTotal = total;
        
        UINT16 newValue = (UINT16)(total - leastTotal);
        if(_value != newValue)
        {
            _value = newValue;
            dirty = true;
        }
    }
}

void CapSense::OnUpdate()
{
}

void CapSense::OnExecute()
{
}

void CapSense::OnFlush()
{
	ArdunityApp.push(_value);
}

