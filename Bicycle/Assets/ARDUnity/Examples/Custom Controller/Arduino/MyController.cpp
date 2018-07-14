//******************************************************************************
//* Includes
//******************************************************************************
#include "Ardunity.h"
#include "MyController.h"


//******************************************************************************
//* Constructors
//******************************************************************************
MyController::MyController(int id) : ArdunityController(id)
{
    // Initialize this class
    _rxSTRING = _stringBuffer;
    canFlush = true; // If send data to Unity, must set true.
    // canFlush = false; // If do not send data to Unity, must set false.
}


//******************************************************************************
//* Override Methods
//******************************************************************************
void MyController::OnSetup()
{
    // Arduino setup
}

void MyController::OnStart()
{
    // When connected to Unity
}

void MyController::OnStop()
{
    // When disconnected to Unity
}

void MyController::OnProcess()
{
    // Arduino loop    
    if(started)
    {
        // When connected to Unity
    }
    else
    {
        // When disconnected to Unity

    }
}

void MyController::OnUpdate()
{
    // When you receive data from Unity
    // 'Pop' means that Arduino get data from Unity
    // The order is important (It must be same with push order in Unity)
    ArdunityApp.pop(&_rxUINT8);
    ArdunityApp.pop(&_rxINT8);
    ArdunityApp.pop(&_rxUINT16);
    ArdunityApp.pop(&_rxINT16);
    ArdunityApp.pop(&_rxUINT32);
    ArdunityApp.pop(&_rxINT32);
    ArdunityApp.pop(&_rxFLOAT32);
    ArdunityApp.pop(_rxSTRING, 30);

    updated = true; // It must be set true to run OnExecute
}

void MyController::OnExecute()
{
    // Do something after receiving data
    _txUINT8 = _rxUINT8;
    _txINT8 = _rxINT8;
    _txUINT16 = _rxUINT16;
    _txINT16 = _rxINT16;
    _txUINT32 = _rxUINT32;
    _txINT32 = _rxINT32;
    _txFLOAT32 = _rxFLOAT32;
    _txSTRING = _rxSTRING;

    dirty = true; // It must be set true to run OnFlush
}

void MyController::OnFlush()
{
    // When you send data to Unity
    // 'Push' means that Arduino throw data to Unity
    // The order is important (It must be same with pop order in Unity)
	ArdunityApp.push(_txUINT8);
    ArdunityApp.push(_txINT8);
    ArdunityApp.push(_txUINT16);
    ArdunityApp.push(_txINT16);
    ArdunityApp.push(_txUINT32);
    ArdunityApp.push(_txINT32);
    ArdunityApp.push(_txFLOAT32);
    ArdunityApp.push(_txSTRING);
}
