#ifndef MyController_h
#define MyController_h

#include "ArdunityController.h"


class MyController : public ArdunityController
{
public:
	MyController(int id);

protected:
	void OnSetup();
	void OnStart();
	void OnStop();
	void OnProcess();
	void OnUpdate();
	void OnExecute();
	void OnFlush();

private:
	UINT8 _rxUINT8;
	INT8 _rxINT8;
	UINT16 _rxUINT16;
	INT16 _rxINT16;
	UINT32 _rxUINT32;
	INT32 _rxINT32;
	FLOAT32 _rxFLOAT32;
	STRING _rxSTRING;

	UINT8 _txUINT8;
	INT8 _txINT8;
	UINT16 _txUINT16;
	INT16 _txINT16;
	UINT32 _txUINT32;
	INT32 _txINT32;
	FLOAT32 _txFLOAT32;
	STRING _txSTRING;

	// STRING should be required to assign array.
	char _stringBuffer[30];
};

#endif

