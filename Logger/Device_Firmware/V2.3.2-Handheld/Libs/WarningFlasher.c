/*
 * File: WarningFlasher.h
 * Author: Matthew Cochrane
 * HW: XMEGA-A4
 */

#include "WarningFlasher.h"
#include "TC_driver.h"

static uint16_t warningOnSecs = 0;

static void warningFlasherSetLights();

void warningFlasherOn(uint16_t on_time_sec)
{
	if (!warningOnSecs)
	{
		WARNING_FLASHER_PORT.DIRSET = (1<<WARNING_FLASHER_EN_PIN)|(1<<WARNING_FLASHER_PIN1)|(1<<WARNING_FLASHER_PIN2);	//sign enable pin square pin on keyboard connector
		WARNING_FLASHER_PORT.OUTSET = (1<<WARNING_FLASHER_EN_PIN);	//sign enable pin square pin on keyboard connector
		warningOnSecs = on_time_sec;
		warningFlasherSetLights();
	}
	else
	{
		warningOnSecs = on_time_sec;
	}
}

void warningFlasherOff()
{
	WARNING_FLASHER_PORT.OUTCLR = (1<<WARNING_FLASHER_EN_PIN)|(1<<WARNING_FLASHER_PIN1)|(1<<WARNING_FLASHER_PIN2);	//sign enable pin square pin on keyboard connector
	warningOnSecs = 0;
}

//Call this function from a timer ISR or callback.
//It is expected that this function is called once every second.
void warningFlasherUpdate() 
{
	if (warningOnSecs) {
		warningOnSecs--;
		warningFlasherSetLights();
		if (warningOnSecs == 0)
		{
			warningFlasherOff();
		}
	}
}


//private functions...

static inline void warningFlasherSetLights()
{
	if (warningOnSecs&0x01) { //if it's an even number
		//Turn bottom flashers off and top flashers on
		WARNING_FLASHER_PORT.OUTSET = (1<<WARNING_FLASHER_PIN1);
		WARNING_FLASHER_PORT.OUTCLR = (1<<WARNING_FLASHER_PIN2);
	} else {
		//Turn bottom flashers on and top flashers off
		WARNING_FLASHER_PORT.OUTCLR = (1<<WARNING_FLASHER_PIN1);
		WARNING_FLASHER_PORT.OUTSET = (1<<WARNING_FLASHER_PIN2);
	}
}