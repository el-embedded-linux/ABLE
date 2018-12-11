/*
  DigitalOutput.cpp - Ardunity Arduino library
  Copyright (C) 2015 ojh6t3k.  All rights reserved.
*/

//******************************************************************************
//* Includes
//******************************************************************************
#include "Ardunity.h"
#include "DigitalOutput.h"


//******************************************************************************
//* Constructors
//******************************************************************************

DigitalOutput::DigitalOutput(int id, int pin, int defaultValue, boolean resetOnStop) : ArdunityController(id)
{
	_pin = pin;
	_defaultValue = defaultValue;
	_resetOnStop = resetOnStop;
    canFlush = false;
}

//******************************************************************************
//* Override Methods
//******************************************************************************
void DigitalOutput::OnSetup()
{
	digitalWrite(_pin, LOW); // disable PWM
	pinMode(_pin, OUTPUT);
	_value = _defaultValue;
	OnExecute();
}

void DigitalOutput::OnStart()
{
}

void DigitalOutput::OnStop()
{
	if(_resetOnStop)
	{
		_value = _defaultValue;
		OnExecute();
	}
}

void DigitalOutput::OnProcess()
{	
}

void DigitalOutput::OnUpdate()
{
	UINT8 newValue = _value;	
	ArdunityApp.pop(&newValue);
	if(newValue != _value)
	{
		_value = newValue;
		updated = true;
	}
}

void DigitalOutput::OnExecute()
{
	if(_value == 0)
		digitalWrite(_pin, LOW);
	else
		digitalWrite(_pin, HIGH);
}

void DigitalOutput::OnFlush()
{
}

//******************************************************************************
//* Private Methods
//******************************************************************************

