/*
  ArdunityController.h - Ardunity Arduino library
  Copyright (C) 2015 ojh6t3k.  All rights reserved.
*/

#ifndef ArdunityController_h
#define ArdunityController_h

#include "Arduino.h"

extern "C" {
typedef unsigned char UINT8;
typedef char INT8;
typedef unsigned short UINT16;
typedef short INT16;
typedef unsigned long UINT32;
typedef long INT32;
typedef float FLOAT32;
typedef char* STRING;
}


class ArdunityController
{
public:
	ArdunityController* nextController;

	ArdunityController(int id);

	void setup();
	void start();
	void stop();
	void process();
	boolean update(byte id);
	void execute();
	void flush();

protected:
    boolean canFlush;
    boolean started;
	boolean updated;
	boolean dirty;
	
	virtual void OnSetup() {}
	virtual void OnStart() {}
	virtual void OnStop() {}
	virtual void OnProcess() {}
	virtual void OnUpdate() {}
	virtual void OnExecute() {}
	virtual void OnFlush() {}

private:
	UINT8 _id;
	UINT8 _enableUpdate;
};

#endif

