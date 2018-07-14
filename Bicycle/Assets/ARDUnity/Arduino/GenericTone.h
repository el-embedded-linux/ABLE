/*
  GenericTone.h - Ardunity Arduino library
  Copyright (C) 2015 ojh6t3k.  All rights reserved.
*/

#ifndef GenericTone_h
#define GenericTone_h

#include "ArdunityController.h"


class GenericTone : public ArdunityController
{
public:
	GenericTone(int id, int pin);

protected:
	void OnSetup();
	void OnStart();
	void OnStop();
	void OnProcess();
	void OnUpdate();
	void OnExecute();
	void OnFlush();

private:
    int _pin;
	UINT16 _frequency;
};

#endif

