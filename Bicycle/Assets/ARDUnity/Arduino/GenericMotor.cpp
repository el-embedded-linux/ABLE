/*
  GenericMotor.cpp - Ardunity Arduino library
  Copyright (C) 2015 ojh6t3k.  All rights reserved.
*/

//******************************************************************************
//* Includes
//******************************************************************************
#include "Ardunity.h"
#include "GenericMotor.h"


//******************************************************************************
//* Constructors
//******************************************************************************

GenericMotor::GenericMotor(int id, int pin1, int pin2, int pin3, int type) : ArdunityController(id)
{
	_pin1 = pin1;
	_pin2 = pin2;
    _pin3 = pin3;
    _type = type;
    canFlush = false;
}

//******************************************************************************
//* Override Methods
//******************************************************************************
void GenericMotor::OnSetup()
{
    if(_type == OnePWM_OneDir)
    {
        digitalWrite(_pin1, LOW); // disable PWM
	    pinMode(_pin1, OUTPUT);
        pinMode(_pin2, OUTPUT);
    }
    else if(_type == TwoPWM)
    {
        digitalWrite(_pin1, LOW); // disable PWM
	    pinMode(_pin1, OUTPUT);
        digitalWrite(_pin2, LOW); // disable PWM
	    pinMode(_pin2, OUTPUT);
    }
    else
    {
        digitalWrite(_pin1, LOW); // disable PWM
	    pinMode(_pin1, OUTPUT);
        pinMode(_pin2, OUTPUT);
	    pinMode(_pin3, OUTPUT);
    }

	_value = 0;
	OnExecute();
}

void GenericMotor::OnStart()
{
}

void GenericMotor::OnStop()
{
	_value = 0;
	OnExecute();
}

void GenericMotor::OnProcess()
{	
}

void GenericMotor::OnUpdate()
{
	FLOAT32 newValue = _value;
	ArdunityApp.pop(&newValue);
	if(_value != newValue)
	{
		_value = newValue;
		updated = true;
	}		
}

void GenericMotor::OnExecute()
{
    if(_type == OnePWM_OneDir)
    {
        float speed = 0;
        if(_value > 0)
        {
            digitalWrite(_pin1, LOW);
            speed = _value;
        }
        else
        {
            digitalWrite(_pin1, HIGH);
            speed = -_value;
        }

        analogWrite(_pin2, (int)(speed * ArdunityApp.maxPWM));
    }
    else if(_type == TwoPWM)
    {
        if(_value == 0)
        {
            digitalWrite(_pin1, LOW);
            digitalWrite(_pin2, LOW);
        }
        else if(_value > 0)
        {
            analogWrite(_pin1, (int)(_value * ArdunityApp.maxPWM));
            digitalWrite(_pin2, LOW);
        }
        else
        {
            digitalWrite(_pin1, LOW);
            analogWrite(_pin2, (int)(-_value * ArdunityApp.maxPWM));
        }
    }
    else
    {
        float speed = 0;
        if(_value > 0)
        {
            digitalWrite(_pin2, HIGH);
            digitalWrite(_pin3, LOW);
            speed = _value;
        }
        else if(_value < 0)
        {
            digitalWrite(_pin2, LOW);
            digitalWrite(_pin3, HIGH);
            speed = -_value;
        }

        analogWrite(_pin1, (int)(speed * ArdunityApp.maxPWM));
    }
}

void GenericMotor::OnFlush()
{
}

//******************************************************************************
//* Private Methods
//******************************************************************************

