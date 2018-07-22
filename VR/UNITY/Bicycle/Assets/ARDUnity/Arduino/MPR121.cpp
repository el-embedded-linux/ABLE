/*
  MPR121.cpp - Ardunity Arduino library
  Copyright (C) 2015 ojh6t3k.  All rights reserved.
*/

//******************************************************************************
//* Includes
//******************************************************************************
#include "Ardunity.h"
#include "MPR121.h"

// MPR121 Register Defines
#define MHD_R	    0x2B
#define NHD_R	    0x2C
#define	NCL_R 	    0x2D
#define	FDL_R	    0x2E
#define	MHD_F	    0x2F
#define	NHD_F	    0x30
#define	NCL_F	    0x31
#define	FDL_F	    0x32
#define	ELE0_T	    0x41
#define	ELE0_R	    0x42
#define	ELE1_T	    0x43
#define	ELE1_R	    0x44
#define	ELE2_T	    0x45
#define	ELE2_R	    0x46
#define	ELE3_T	    0x47
#define	ELE3_R	    0x48
#define	ELE4_T	    0x49
#define	ELE4_R	    0x4A
#define	ELE5_T	    0x4B
#define	ELE5_R	    0x4C
#define	ELE6_T  	0x4D
#define	ELE6_R	    0x4E
#define	ELE7_T	    0x4F
#define	ELE7_R	    0x50
#define	ELE8_T	    0x51
#define	ELE8_R	    0x52
#define	ELE9_T	    0x53
#define	ELE9_R	    0x54
#define	ELE10_T	    0x55
#define	ELE10_R	    0x56
#define	ELE11_T	    0x57
#define	ELE11_R	    0x58
#define	FIL_CFG	    0x5D
#define	ELE_CFG	    0x5E
#define GPIO_CTRL0	0x73
#define	GPIO_CTRL1	0x74
#define GPIO_DATA	0x75
#define	GPIO_DIR	0x76
#define	GPIO_EN		0x77
#define	GPIO_SET	0x78
#define	GPIO_CLEAR	0x79
#define	GPIO_TOGGLE	0x7A
#define	ATO_CFG0	0x7B
#define	ATO_CFGU	0x7D
#define	ATO_CFGL	0x7E
#define	ATO_CFGT	0x7F

// Global Constants
#define TOU_THRESH	0x06
#define	REL_THRESH	0x0A

//******************************************************************************
//* Constructors
//******************************************************************************

MPR121::MPR121(int id, int addr) : ArdunityI2C(id)
{
	address = addr;
    canFlush = true;
}

//******************************************************************************
//* Override Methods
//******************************************************************************
void MPR121::OnSetup()
{
    ArdunityI2C::OnSetup();

    _initialized = true;

    if(!Write(ELE_CFG, 0x00)) _initialized = false;

    // Section A - Controls filtering when data is > baseline.
    if(!Write(MHD_R, 0x01)) _initialized = false;
    if(!Write(NHD_R, 0x01)) _initialized = false;
    if(!Write(NCL_R, 0x00)) _initialized = false;
    if(!Write(FDL_R, 0x00)) _initialized = false;

    // Section B - Controls filtering when data is < baseline.
    if(!Write(MHD_F, 0x01)) _initialized = false;
    if(!Write(NHD_F, 0x01)) _initialized = false;
    if(!Write(NCL_F, 0xFF)) _initialized = false;
    if(!Write(FDL_F, 0x02)) _initialized = false;
  
    // Section C - Sets touch and release thresholds for each electrode
    if(!Write(ELE0_T, TOU_THRESH)) _initialized = false;
    if(!Write(ELE0_R, REL_THRESH)) _initialized = false;
    
    if(!Write(ELE1_T, TOU_THRESH)) _initialized = false;
    if(!Write(ELE1_R, REL_THRESH)) _initialized = false;
    
    if(!Write(ELE2_T, TOU_THRESH)) _initialized = false;
    if(!Write(ELE2_R, REL_THRESH)) _initialized = false;
    
    if(!Write(ELE3_T, TOU_THRESH)) _initialized = false;
    if(!Write(ELE3_R, REL_THRESH)) _initialized = false;
    
    if(!Write(ELE4_T, TOU_THRESH)) _initialized = false;
    if(!Write(ELE4_R, REL_THRESH)) _initialized = false;
    
    if(!Write(ELE5_T, TOU_THRESH)) _initialized = false;
    if(!Write(ELE5_R, REL_THRESH)) _initialized = false;
    
    if(!Write(ELE6_T, TOU_THRESH)) _initialized = false;
    if(!Write(ELE6_R, REL_THRESH)) _initialized = false;
    
    if(!Write(ELE7_T, TOU_THRESH)) _initialized = false;
    if(!Write(ELE7_R, REL_THRESH)) _initialized = false;
    
    if(!Write(ELE8_T, TOU_THRESH)) _initialized = false;
    if(!Write(ELE8_R, REL_THRESH)) _initialized = false;
    
    if(!Write(ELE9_T, TOU_THRESH)) _initialized = false;
    if(!Write(ELE9_R, REL_THRESH)) _initialized = false;
    
    if(!Write(ELE10_T, TOU_THRESH)) _initialized = false;
    if(!Write(ELE10_R, REL_THRESH)) _initialized = false;
    
    if(!Write(ELE11_T, TOU_THRESH)) _initialized = false;
    if(!Write(ELE11_R, REL_THRESH)) _initialized = false;
  
    // Section D
    // Set the Filter Configuration
    // Set ESI2
    if(!Write(FIL_CFG, 0x04)) _initialized = false;
  
    // Section E
    // Electrode Configuration
    // Set ELE_CFG to 0x00 to return to standby mode
    if(!Write(ELE_CFG, 0x0C)) _initialized = false;  // Enables all 12 Electrodes
  
  
    // Section F
    // Enable Auto Config and auto Reconfig
    // set_register(0x5A, ATO_CFG0, 0x0B);
    // set_register(0x5A, ATO_CFGU, 0xC9);  // USL = (Vdd-0.7)/vdd*256 = 0xC9 @3.3V   set_register(0x5A, ATO_CFGL, 0x82);  // LSL = 0.65*USL = 0x82 @3.3V
    // set_register(0x5A, ATO_CFGT, 0xB5);  // Target = 0.9*USL = 0xB5 @3.3V
    
    if(!Write(ELE_CFG, 0x0C)) _initialized = false;

	_value = 0;
}

void MPR121::OnStart()
{
}

void MPR121::OnStop()
{
}

void MPR121::OnProcess()
{
    if(started && _initialized)
    {
        uint8_t data[2];
        if(Read(data, 2))
        {
            UINT16 newValue = (UINT16)((data[1] << 8) | data[0]);
            if(newValue != _value)
            {
                _value = newValue;
                dirty = true;
            }
        }        
    }
}

void MPR121::OnUpdate()
{
}

void MPR121::OnExecute()
{
}

void MPR121::OnFlush()
{
    ArdunityApp.push(_value);
}

//******************************************************************************
//* Private Methods
//******************************************************************************

