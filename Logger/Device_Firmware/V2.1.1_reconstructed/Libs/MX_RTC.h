/*
 * MX_RTC.h
 *
 *  Created on: 27/08/2010
 *      Author: MC
 */

#ifndef MX_RTC_H_
#define MX_RTC_H_

#include <stdlib.h>
#include <avr/io.h>
#include <avr/pgmspace.h>
#include <stdio.h>
#include <avr/interrupt.h>

typedef struct RTCTime {
	uint8_t Second;
	uint8_t Minute;
	uint8_t Hour;

	uint8_t Day;
	uint8_t Month;
	uint8_t Year;
} RTC_Time_t;

#define RTC_SYNCWAIT	while(RTC.STATUS & 0x01)

//Function Prototypes:
void RTC_Setup();
uint16_t RTC_GetCNT();
uint8_t IntToBCD(uint8_t val);
uint8_t BCDToInt(uint8_t val);
void CopyTime(RTC_Time_t* SrcTime, RTC_Time_t* DstTime);
void AddTimes(RTC_Time_t* DateTime, RTC_Time_t* AddTime);
void ClearTime(RTC_Time_t*);
uint8_t FirstTimeGreaterOrEqualTo(RTC_Time_t* Time1, RTC_Time_t* Time2);
uint8_t ValidDateTime(RTC_Time_t* DT);
uint8_t DaysInMonth(uint8_t Month, uint8_t Year);
int str2Time(char* __s, RTC_Time_t* destTime);
void sPrintDateTime(char* __s, RTC_Time_t* pTime, char* lbl);
void sPrintDate(char* __s, RTC_Time_t* pTime);
void sPrintTime(char* __s, RTC_Time_t* pTime);
void ZipTime(RTC_Time_t* rawTime, uint8_t* zipTime, uint8_t Flags);
uint8_t UnzipTime(RTC_Time_t* rawTime, uint8_t* zipTime);
void AddToTime(RTC_Time_t* dateTime, uint8_t Yrs, uint8_t Months, uint8_t Days, uint8_t Hrs, uint8_t Mins, uint8_t Secs);
void TimeAfterTime(RTC_Time_t* startTime, RTC_Time_t* destTime, uint8_t Yrs, uint8_t Months, uint8_t Days, uint8_t Hrs, uint8_t Mins, uint8_t Secs);
#endif /* MX_RTC_H_ */
