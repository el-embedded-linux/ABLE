/*
  PulseOutput.cpp - Ardunity Arduino library
  Copyright (C) 2015 ojh6t3k.  All rights reserved.
*/

//******************************************************************************
//* Includes
//******************************************************************************
#include "Ardunity.h"
#include "PulseOutput.h"


//******************************************************************************
//* Constructors
//******************************************************************************

PulseOutput::PulseOutput(int id, int pin, int defaultValue) : ArdunityController(id)
{
	_pin = pin;
	_defaultValue = defaultValue;
	if(_defaultValue == HIGH)
		_setValue = LOW;
	else
		_setValue = HIGH;	
	_value = _defaultValue;
	_run = false;
	_loop2 = false;
	_setTime2 = 0;
	_delayTime2 = 0;	
    canFlush = false;	
}

//******************************************************************************
//* Override Methods
//******************************************************************************
void PulseOutput::OnSetup()
{
	digitalWrite(_pin, LOW); // disable PWM
	pinMode(_pin, OUTPUT);
	_value = _defaultValue;
	digitalWrite(_pin, _value);
}

void PulseOutput::OnStart()
{
}

void PulseOutput::OnStop()
{
	_loop2 = false;
}

void PulseOutput::OnProcess()
{
	if(_run)
	{
		unsigned long time = millis();

		if(_value == _defaultValue)
		{
			if(time - _preTime >= _delayTime2)
			{
				if(_loop2)
				{
					_value = _setValue;
					digitalWrite(_pin, _value);
					_preTime = time;
				}
				else
					_run = false;
			}
		}
		else
		{
			if(time - _preTime >= _setTime2)
			{
				_value = _defaultValue;
				digitalWrite(_pin, _value);

				if(_loop2)
					_preTime = time;
				else
					_run = false;
			}
		}
	}
}

void PulseOutput::OnUpdate()
{
	ArdunityApp.pop(&_loop);
	ArdunityApp.pop(&_setTime);
	ArdunityApp.pop(&_delayTime);

	updated = true;
}

void PulseOutput::OnExecute()
{
	if(_loop == 0)
		_loop2 = false;
	else
		_loop2 = true;
	
	_setTime2 = _setTime;
	_delayTime2 = _delayTime;

	if(!_run)
	{
		_value = _setValue;
		digitalWrite(_pin, _value);
		_preTime = millis();
	}	
	
	_run = true;
}

void PulseOutput::OnFlush()
{
}

//******************************************************************************
//* Private Methods
//******************************************************************************

