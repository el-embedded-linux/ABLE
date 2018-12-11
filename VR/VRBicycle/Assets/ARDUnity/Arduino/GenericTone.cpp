/*
  GenericTone.cpp - Ardunity Arduino library
  Copyright (C) 2015 ojh6t3k.  All rights reserved.
*/

//******************************************************************************
//* Includes
//******************************************************************************
#include "Ardunity.h"
#include "GenericTone.h"


//******************************************************************************
//* Constructors
//******************************************************************************

GenericTone::GenericTone(int id, int pin) : ArdunityController(id)
{
	_pin = pin;
    canFlush = false;
}

//******************************************************************************
//* Override Methods
//******************************************************************************
void GenericTone::OnSetup()
{
}

void GenericTone::OnStart()
{
}

void GenericTone::OnStop()
{
	noTone(_pin);
}

void GenericTone::OnProcess()
{
}

void GenericTone::OnUpdate()
{
	UINT16 newFrequency = _frequency;
	ArdunityApp.pop(&newFrequency);
	if(newFrequency != _frequency)
	{
		_frequency = newFrequency;
		updated = true;
	}
}

void GenericTone::OnExecute()
{
	if(_frequency == 0)
		noTone(_pin);
	else
		tone(_pin, _frequency);
}

void GenericTone::OnFlush()
{
}
