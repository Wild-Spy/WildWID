/*
 * File: WarningFlasher.h
 * Author: Matthew Cochrane
 * HW: XMEGA-A4
 */

#ifndef __WARNING_FLASHER_H__
#define __WARNING_FLASHER_H__

#include <stdint.h>

#define WARNING_FLASHER_PORT	PORTA
#define WARNING_FLASHER_EN_PIN	0
#define WARNING_FLASHER_PIN1	1
#define WARNING_FLASHER_PIN2	5

void warningFlasherOn(uint16_t on_time_sec);
void warningFlasherOff();

//Call this function from a timer ISR or callback.
//It is expected that this function is called once every second.
void warningFlasherUpdate();

#endif // __WARNING_FLASHER_H__