/*
  DigitalInput.cpp - Ardunity Arduino library
  Copyright (C) 2015 ojh6t3k.  All rights reserved.
*/

//******************************************************************************
//* Includes
//******************************************************************************
#include "Ardunity.h"
#include "DigitalInput.h"


//******************************************************************************
//* Constructors
//******************************************************************************

DigitalInput::DigitalInput(int id, int pin, boolean pullup) : ArdunityController(id)
{
	_pin = pin;
	_pullup = pullup;
    canFlush = true;
}

//******************************************************************************
//* Override Methods
//******************************************************************************
void DigitalInput::OnSetup()
{
	if(_pullup == false)
		pinMode(_pin, INPUT);
	else
		pinMode(_pin, INPUT_PULLUP);
}

void DigitalInput::OnStart()
{
}

void DigitalInput::OnStop()
{
}

void DigitalInput::OnProcess()
{
    if(started)
    {
        int value = digitalRead(_pin);
        if(value == 0)
        {
            if(_value == 1)
            {
                _value = 0;
                dirty = true;
            }
        }
        else
        {
            if(_value == 0)
            {
                _value = 1;
                dirty = true;
            }
        }
    }
}

void DigitalInput::OnUpdate()
{
}

void DigitalInput::OnExecute()
{
}

void DigitalInput::OnFlush()
{
	ArdunityApp.push(_value);
}

//******************************************************************************
//* Private Methods
//******************************************************************************

