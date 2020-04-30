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

struct RTCTime {
	uint8_t Second;
	uint8_t Minute;
	uint8_t Hour;

	uint8_t Day;
	uint8_t Month;
	uint8_t Year;
};

#define RTC_SYNCWAIT	while(RTC.STATUS & 0x01)

//Function Prototypes:
void RTC_Setup();
uint16_t RTC_GetCNT();
uint8_t IntToBCD(uint8_t);
uint8_t BCDToInt(uint8_t);
void CopyTime (struct RTCTime*, struct RTCTime*);
void AddTimes(struct RTCTime*, struct RTCTime*);
void ClearTime(struct RTCTime*);
uint8_t CompareTimes (struct RTCTime* , struct RTCTime* );
uint8_t FirstTimeGreater (struct RTCTime* , struct RTCTime* );
uint8_t ValidDateTime(struct RTCTime*);
uint8_t DaysInMonth(uint8_t , uint8_t );
int str2Time(char*, struct RTCTime*);
void sPrintDateTime(char*, struct RTCTime* , char*);
void sPrintDate(char* __s, struct RTCTime* pTime);
void sPrintTime(char* __s, struct RTCTime* pTime);
void ZipTime(struct RTCTime*, uint8_t*, uint8_t);
uint8_t UnzipTime(struct RTCTime*, uint8_t* );

#endif /* MX_RTC_H_ */
