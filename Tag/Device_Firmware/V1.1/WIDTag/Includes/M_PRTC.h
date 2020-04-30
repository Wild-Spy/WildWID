/*
 * M_PRTC.h
 *
 *  Created on: 04/09/2010
 *      Author: MC
 */

#ifndef M_PRTC_H_
#define M_PRTC_H_

#include <stdlib.h>
#include <avr/io.h>
#include <avr/pgmspace.h>
#include <stdio.h>

struct RTCTime {
	uint8_t Second;
	uint8_t Minute;
	uint8_t Hour;

	uint8_t Day;
	uint8_t Month;
	int Year;
};


//Function Prototypes:
void RTC_Setup();
uint16_t RTC_GetCNT();
uint8_t IntToBCD(uint8_t);
uint8_t BCDToInt(uint8_t);
void CopyTime (struct RTCTime*, struct RTCTime*);
void AddTimes(struct RTCTime*, struct RTCTime*);
void ClearTime(struct RTCTime*);
uint8_t CompareTimes (struct RTCTime* , struct RTCTime* );
//uint8_t FirstTimeGreater (struct RTCTime* , struct RTCTime* );
uint8_t ValidDateTime(struct RTCTime*);
uint8_t DaysInMonth(uint8_t , uint8_t );
int str2Time(char*, struct RTCTime*);
void sPrintTime(char*, struct RTCTime* , char*);
void ZipTime(struct RTCTime*, uint8_t*);
void UnzipTime(struct RTCTime*, uint8_t*);

#endif /* M_PRTC_H_ */
