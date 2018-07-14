/*
  AnalogOutput.cpp - Ardunity Arduino library
  Copyright (C) 2015 ojh6t3k.  All rights reserved.
*/

//******************************************************************************
//* Includes
//******************************************************************************
#include "Ardunity.h"
#include "AnalogOutput.h"


//******************************************************************************
//* Constructors
//******************************************************************************

AnalogOutput::AnalogOutput(int id, int pin, float defaultValue, boolean resetOnStop) : ArdunityController(id)
{
	_pin = pin;
	_defaultValue = defaultValue;
	_resetOnStop = resetOnStop;
    canFlush = false;
}

//******************************************************************************
//* Override Methods
//******************************************************************************
void AnalogOutput::OnSetup()
{
	pinMode(_pin, OUTPUT);
	_value = _defaultValue;
	OnExecute();
}

void AnalogOutput::OnStart()
{
}

void AnalogOutput::OnStop()
{
	if(_resetOnStop)
	{
		_value = _defaultValue;
		OnExecute();
	}
}

void AnalogOutput::OnProcess()
{	
}

void AnalogOutput::OnUpdate()
{
	FLOAT32 newValue = _value;
	ArdunityApp.pop(&newValue);
	if(_value != newValue)
	{
		_value = newValue;
		updated = true;
	}		
}

void AnalogOutput::OnExecute()
{
	analogWrite(_pin, (int)(_value * ArdunityApp.maxPWM));
}

void AnalogOutput::OnFlush()
{
}

//******************************************************************************
//* Private Methods
//******************************************************************************

