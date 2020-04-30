/*
 * MX_DateTime.h
 *
 *  Source Created on: 27/08/2010
 *         Created on: 22/07/2016
 *             Author: MC
 */

#ifndef MX_DATETIME_H_
#define MX_DATETIME_H_

#include <stdint.h>
#include <stdlib.h>

#define SET_RTC_TIME_T(t, day,month,year,hour,minute,second) do {(t).Year = year; (t).Month = month; (t).Day = day; (t).Hour = hour; (t).Minute = minute; (t).Second = second;} while (0)

typedef struct RTCTime {
	uint8_t Second;
	uint8_t Minute;
	uint8_t Hour;

	uint8_t Day;
	uint8_t Month;
	uint8_t Year;
} RTC_Time_t;

//Function Prototypes:
uint8_t IntToBCD(uint8_t val);
uint8_t BCDToInt(uint8_t val);
void CopyTime(RTC_Time_t* SrcTime, RTC_Time_t* DstTime);
void AddTimes(RTC_Time_t* DateTime, RTC_Time_t* AddTime);
void SubtractSecondsFromTime(RTC_Time_t* time, uint32_t secs);
void ClearTime(RTC_Time_t*);
uint8_t FirstTimeGreaterOrEqualTo(RTC_Time_t* Time1, RTC_Time_t* Time2);
uint8_t TimeBetweenTimes(RTC_Time_t* time, RTC_Time_t* start, RTC_Time_t* stop);
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

void AddSecondsToEpoch(RTC_Time_t* epoch, uint32_t secs, RTC_Time_t* destTime);

#endif /* MX_DATETIME_H_ */
