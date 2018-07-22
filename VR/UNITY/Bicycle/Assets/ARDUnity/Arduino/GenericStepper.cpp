/*
  GenericStepper.cpp - Ardunity Arduino library
  Copyright (C) 2015 ojh6t3k.  All rights reserved.
*/

//******************************************************************************
//* Includes
//******************************************************************************
#include "Ardunity.h"
#include "GenericStepper.h"


//******************************************************************************
//* Constructors
//******************************************************************************
GenericStepper::GenericStepper(int id, int step, int drive, int pin1, int pin2, int pin3, int pin4) : ArdunityController(id)
{
	_drive = drive;
	if(_drive == HALF_STEP_DRIVE)
		_stepAngle = 360 / (float)(step * 2);
	else
		_stepAngle = 360 / (float)step;
	_halfStepAngle = _stepAngle * 0.5;
	_phase = 0;
	_pin1 = pin1;
	_pin2 = pin2;
	_pin3 = pin3;
	_pin4 = pin4;
    canFlush = false;
}


//******************************************************************************
//* Override Methods
//******************************************************************************
void GenericStepper::OnSetup()
{
	pinMode(_pin1, OUTPUT);
	pinMode(_pin2, OUTPUT);
	pinMode(_pin3, OUTPUT);
	pinMode(_pin4, OUTPUT);
	
	digitalWrite(_pin1, LOW);
	digitalWrite(_pin2, LOW);
	digitalWrite(_pin3, LOW);
	digitalWrite(_pin4, LOW);
}

void GenericStepper::OnStart()
{
	_delayTime = 0;
	_mode = 0;
	_rpm = 0;
	_angle = 0;
	_errorAngle = 0;
	_direction = true;
	SetStep();
}

void GenericStepper::OnStop()
{
	digitalWrite(_pin1, LOW);
	digitalWrite(_pin2, LOW);
	digitalWrite(_pin3, LOW);
	digitalWrite(_pin4, LOW);
}

void GenericStepper::OnProcess()
{
	if(started)
	{
		if(_delayTime > 0)
		{
			unsigned long curTime = micros();
			if(curTime - _preTime >= _delayTime)
			{
				if(_mode == 1) // Speed mode
				{
					SetStep(_direction);
				}
				else if(_mode == 2) // Angle mode
				{
					if(_errorAngle != 0)
					{
						if(abs(_errorAngle) > _halfStepAngle)
						{
							if(_errorAngle > 0)
							{
								SetStep(false);
								_errorAngle -= _stepAngle;
							}
							else
							{
								SetStep(true);
								_errorAngle += _stepAngle;
							}
						}
						else
							_errorAngle = 0;
					}
				}
				_preTime = curTime;
			}			
		}
	}
}

void GenericStepper::OnUpdate()
{
	ArdunityApp.pop(&_newMode);
	ArdunityApp.pop(&_newRPM);
	ArdunityApp.pop(&_newAngle);

	updated = true;
}

void GenericStepper::OnExecute()
{
	if(_rpm != _newRPM)
	{
		_rpm = _newRPM;
		if(_rpm >= 0)
			_direction = true;
		else
			_direction = false;
		
		if(_rpm == 0)
			_delayTime = 0;
		else
		{
			float temp = _stepAngle / (_rpm * 360);
			if(temp < 0)
				temp *= -1;
			
			_delayTime = (unsigned long)(temp * 60000000);
			if(_delayTime == 0)
				_delayTime = 1;
			_preTime = micros();
		}
	}

	if(_angle != _newAngle)
	{
		_angle -= _errorAngle;
		_errorAngle = _newAngle - _angle;
		_angle = _newAngle;
	}

	if(_mode != _newMode)
	{
		_mode = _newMode;
		if(_mode == 1)
		{

		}
		else if(_mode == 2)
		{
			_angle = 0;
			_errorAngle = _newAngle - _angle;
			_angle = _newAngle;
		}
	}
}

void GenericStepper::OnFlush()
{
}

//******************************************************************************
//* Private Methods
//******************************************************************************
void GenericStepper::SetStep()
{
	if(_drive == WAVE_DRIVE)
	{
		if(_phase > 3)
			_phase = 0;
		else if(_phase < 0)
			_phase = 3;
		
		switch (_phase) 
		{
			case 0:  // 1000
				digitalWrite(_pin1, HIGH);
				digitalWrite(_pin2, LOW);
				digitalWrite(_pin3, LOW);
				digitalWrite(_pin4, LOW);
				break;

			case 1:  // 0100
				digitalWrite(_pin1, LOW);
				digitalWrite(_pin2, HIGH);
				digitalWrite(_pin3, LOW);
				digitalWrite(_pin4, LOW);
				break;

			case 2:  // 0010
				digitalWrite(_pin1, LOW);
				digitalWrite(_pin2, LOW);
				digitalWrite(_pin3, HIGH);
				digitalWrite(_pin4, LOW);
				break;

			case 3:  // 0001
				digitalWrite(_pin1, LOW);
				digitalWrite(_pin2, LOW);
				digitalWrite(_pin3, LOW);
				digitalWrite(_pin4, HIGH);
				break;
		}
	}
	else if(_drive == FULL_STEP_DRIVE)
	{
		if(_phase > 3)
			_phase = 0;
		else if(_phase < 0)
			_phase = 3;
		
		switch (_phase) 
		{
			case 0:  // 1100
				digitalWrite(_pin1, HIGH);
				digitalWrite(_pin2, HIGH);
				digitalWrite(_pin3, LOW);
				digitalWrite(_pin4, LOW);
				break;

			case 1:  // 0110
				digitalWrite(_pin1, LOW);
				digitalWrite(_pin2, HIGH);
				digitalWrite(_pin3, HIGH);
				digitalWrite(_pin4, LOW);
				break;

			case 2:  // 0011
				digitalWrite(_pin1, LOW);
				digitalWrite(_pin2, LOW);
				digitalWrite(_pin3, HIGH);
				digitalWrite(_pin4, HIGH);
				break;

			case 3:  // 1001
				digitalWrite(_pin1, HIGH);
				digitalWrite(_pin2, LOW);
				digitalWrite(_pin3, LOW);
				digitalWrite(_pin4, HIGH);
				break;
		}
	}
	else if(_drive == HALF_STEP_DRIVE)
	{
		if(_phase > 7)
			_phase = 0;
		else if(_phase < 0)
			_phase = 7;
		
		switch (_phase) 
		{
			case 0:  // 1000
				digitalWrite(_pin1, HIGH);
				digitalWrite(_pin2, LOW);
				digitalWrite(_pin3, LOW);
				digitalWrite(_pin4, LOW);
				break;

			case 1:  // 1100
				digitalWrite(_pin1, HIGH);
				digitalWrite(_pin2, HIGH);
				digitalWrite(_pin3, LOW);
				digitalWrite(_pin4, LOW);
				break;

			case 2:  // 0100
				digitalWrite(_pin1, LOW);
				digitalWrite(_pin2, HIGH);
				digitalWrite(_pin3, LOW);
				digitalWrite(_pin4, LOW);
				break;

			case 3:  // 0110
				digitalWrite(_pin1, LOW);
				digitalWrite(_pin2, HIGH);
				digitalWrite(_pin3, HIGH);
				digitalWrite(_pin4, LOW);
				break;
			
			case 4:  // 0010
				digitalWrite(_pin1, LOW);
				digitalWrite(_pin2, LOW);
				digitalWrite(_pin3, HIGH);
				digitalWrite(_pin4, LOW);
				break;

			case 5:  // 0011
				digitalWrite(_pin1, LOW);
				digitalWrite(_pin2, LOW);
				digitalWrite(_pin3, HIGH);
				digitalWrite(_pin4, HIGH);
				break;

			case 6:  // 0001
				digitalWrite(_pin1, LOW);
				digitalWrite(_pin2, LOW);
				digitalWrite(_pin3, LOW);
				digitalWrite(_pin4, HIGH);
				break;

			case 7:  // 1001
				digitalWrite(_pin1, HIGH);
				digitalWrite(_pin2, LOW);
				digitalWrite(_pin3, LOW);
				digitalWrite(_pin4, HIGH);
				break;
		}
	}
}

void GenericStepper::SetStep(boolean direction)
{
	if(direction)
		_phase++;
	else
		_phase--;
	
	SetStep();
}
