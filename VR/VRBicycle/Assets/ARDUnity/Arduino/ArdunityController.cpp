/*
  ArdunityController.cpp - Ardunity Arduino library
  Copyright (C) 2015 ojh6t3k.  All rights reserved.
*/

//******************************************************************************
//* Includes
//******************************************************************************
#include "ArdunityController.h"
#include "Ardunity.h"


//******************************************************************************
//* Constructors
//******************************************************************************

ArdunityController::ArdunityController(int id)
{
	_id = (UINT8)id;
    started = false;
	updated = false;
	dirty = false;
    _enableUpdate = 1;	
	nextController = 0;
}

//******************************************************************************
//* Public Methods
//******************************************************************************
void ArdunityController::setup()
{
	OnSetup();
}

void ArdunityController::start()
{
    started = true;
	updated = false;
	dirty = true;
	OnStart();
}

void ArdunityController::stop()
{
    started = false;
	OnStop();
}

void ArdunityController::process()
{
	OnProcess();
}

boolean ArdunityController::update(byte id)
{
	if(_id == (UINT8)id)
	{
		OnUpdate();
		ArdunityApp.pop(&_enableUpdate);
		return true;
	}

	return false;
}

void ArdunityController::execute()
{
	if(updated)
	{
		OnExecute();		
		updated = false;
	}
}

void ArdunityController::flush()
{
    started = true;
	if(canFlush && (_enableUpdate == 1) && dirty)
	{
		ArdunityApp.select(_id);
		OnFlush();
		dirty = false;
		ArdunityApp.flush();
	}
}

