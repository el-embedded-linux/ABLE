/*
  HCSR04.cpp - Ardunity Arduino library
  Copyright (C) 2016 ojh6t3k.  All rights reserved.
*/

//******************************************************************************
//* Includes
//******************************************************************************
#include "Ardunity.h"
#include "HCSR04.h"


//******************************************************************************
//* Constructors
//******************************************************************************

HCSR04::HCSR04(int id, int trig, int echo) : ArdunityController(id)
{
	_trig = trig;
    _echo = echo;
    canFlush = true;
}


//******************************************************************************
//* Override Methods
//******************************************************************************
void HCSR04::OnSetup()
{
    pinMode(_trig, OUTPUT);
    pinMode(_echo, INPUT);
}

void HCSR04::OnStart()
{
    
}

void HCSR04::OnStop()
{	
}

void HCSR04::OnProcess()
{
    if(started)
    {
        digitalWrite(_trig, LOW);
        delayMicroseconds(4);
        digitalWrite(_trig, HIGH);
        delayMicroseconds(20);
        digitalWrite(_trig, LOW);
        
        unsigned long duration = pulseIn(_echo, HIGH, 5000);
        float dist = duration * 0.17;
        int newDistance = (int)dist;
        if(_distance != (UINT16)newDistance)
        {
            _distance = (UINT16)newDistance;
            dirty = true;
        }
    }
}

void HCSR04::OnUpdate()
{
}

void HCSR04::OnExecute()
{
}

void HCSR04::OnFlush()
{
	ArdunityApp.push(_distance);
}

