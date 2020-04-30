/*
 * MX_RTC.c
 *
 *  Created on: 27/08/2010
 *      Author: MC
 */

#include "MX_RTC.h"
#include <avr/io.h>
#include <avr/pgmspace.h>
#include <avr/interrupt.h>

#define RTC_SYNCWAIT	while(RTC.STATUS & 0x01)

//volatile RTC_Time_t RTC_c_Time;
//RTC_Time_t RTC_Period;

extern RTC_Time_t RTC_c_Time, RTC_Period;
//To go in main.c...
/*RTC_Time_t RTC_c_Time;
RTC_Time_t RTC_Period;


ISR(RTC_OVF_vect) {
	AddTimes(&RTC_c_Time, &RTC_Period);
}*/

//1 second overflow
void RTC_Setup() {

	ClearTime(&RTC_c_Time);
	RTC_c_Time.Year = 10;
	RTC_c_Time.Month = 1;
	RTC_c_Time.Day = 1;
	ClearTime(&RTC_Period);
	RTC_Period.Second = 1;

	//Initial value of PER is already 0xFFFF
	RTC.PER = 0x7FFF;
	//Initial value of CNT is already 0x0000
	//RTC.CNT = 0x0000;
	//RTC.INTFLAGS = RTC_OVFIF_bm;
	//enable all levels of interrupts
	PMIC.CTRL = PMIC_HILVLEN_bm|PMIC_MEDLVLEN_bm|PMIC_LOLVLEN_bm;
	RTC.INTCTRL = RTC_OVFINTLVL_HI_gc;
	RTC.CTRL = RTC_PRESCALER_DIV1_gc;
	//start the RTC - no prescaling -> 1s overflow;

}

uint16_t RTC_GetCNT() {
	RTC_SYNCWAIT;
	return RTC.CNT;
}