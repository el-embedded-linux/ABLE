/*
  AnalogInput.cpp - Ardunity Arduino library
  Copyright (C) 2015 ojh6t3k.  All rights reserved.
*/

//******************************************************************************
//* Includes
//******************************************************************************
#include "Ardunity.h"
#include "AnalogInput.h"


//******************************************************************************
//* Constructors
//******************************************************************************

AnalogInput::AnalogInput(int id, int pin) : ArdunityController(id)
{
	_pin = pin;
    canFlush = true;
}


//******************************************************************************
//* Override Methods
//******************************************************************************
void AnalogInput::OnSetup()
{
}

void AnalogInput::OnStart()
{
}

void AnalogInput::OnStop()
{	
}

void AnalogInput::OnProcess()
{
    if(started)
    {
        FLOAT32 newValue = (FLOAT32)analogRead(_pin) / (FLOAT32)ArdunityApp.maxADC;
        if(_value != newValue)
        {
            _value = newValue;
            dirty = true;
        }
    }
}

void AnalogInput::OnUpdate()
{
}

void AnalogInput::OnExecute()
{
}

void AnalogInput::OnFlush()
{
	ArdunityApp.push(_value);
}
