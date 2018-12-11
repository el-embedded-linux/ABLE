/*
  GenericServo.cpp - Ardunity Arduino library
  Copyright (C) 2015 ojh6t3k.  All rights reserved.
*/

//******************************************************************************
//* Includes
//******************************************************************************
#include "Ardunity.h"
#include "GenericServo.h"


//******************************************************************************
//* Constructors
//******************************************************************************

GenericServo::GenericServo(int id, int pin, boolean smooth) : ArdunityController(id)
{
	_pin = pin;
	_smooth = smooth;
	_first = false;
    canFlush = false;
}

//******************************************************************************
//* Override Methods
//******************************************************************************
void GenericServo::OnSetup()
{
}

void GenericServo::OnStart()
{
	_servo.attach(_pin);
	_first = true;
}

void GenericServo::OnStop()
{
	_servo.detach();
}

void GenericServo::OnProcess()
{
	if(started && _smooth)
	{
		if(_curAngle != _endAngle)
		{
			float t = (float)(_endTime - millis()) / (float)(_endTime - _startTime);
			if(t <= 0)
				_curAngle = _endAngle;
			else
			{
				float a = (float)(_endAngle - _startAngle) * (1 - t);
				_curAngle = _startAngle + (int)a;
			}

			_servo.write(_curAngle);
		}
	}
}

void GenericServo::OnUpdate()
{
	UINT8 newAngle;
	ArdunityApp.pop(&newAngle);
	if(_angle != newAngle)
	{
		_angle = newAngle;
		updated = true;
	}
}

void GenericServo::OnExecute()
{
	if(_smooth)
	{
		if(_first)
		{
			_first = false;
			_curAngle = (int)_angle;
			_startAngle = _curAngle;
			_endAngle = _startAngle;
			_servo.write(_curAngle);
			_endTime = millis();
		}
		else
		{
			_startAngle = _curAngle;
			_endAngle = (int)_angle;
			_startTime = millis();
			_endTime = (_startTime - _endTime) + _startTime;
			if(_endTime <= _startTime)
			{
				// Timer overflow
				_curAngle = _endAngle;
				_startAngle = _endAngle;
				_servo.write(_curAngle);
				_endTime = _startTime;
			}
		}
	}
	else
	{
		_servo.write(_angle);
	}
}

void GenericServo::OnFlush()
{
}

//******************************************************************************
//* Private Methods
//******************************************************************************

