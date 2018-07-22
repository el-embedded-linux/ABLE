/*
  MPR121.h - Ardunity Arduino library
  Copyright (C) 2015 ojh6t3k.  All rights reserved.
*/
#ifndef MPR121_h
#define MPR121_h

#include "ArdunityI2C.h"



class MPR121 : public ArdunityI2C
{
public:
	MPR121(int id, int addr);	

protected:
	void OnSetup();
	void OnStart();
	void OnStop();
	void OnProcess();
	void OnUpdate();
	void OnExecute();
	void OnFlush();

private:
	bool _initialized;
	UINT16 _value;
};

#endif

