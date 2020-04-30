/*
 * MX_DateTime.c
 *
 *  Source Created on: 27/08/2010
 *         Created on: 22/07/2016
 *             Author: MC
 */

#include "MX_DateTime.h"
#include <avr/pgmspace.h>
#include <avr/interrupt.h>
#include <time.h>
#include <stdio.h>

uint8_t IntToBCD (uint8_t val) {
	if (val > 99) return 99;
	uint8_t a = val/10;
	return (a<<4)|(val-(a*10));
}

uint8_t BCDToInt (uint8_t val) {
	if (val > 0x99) return 99;
	return (val&0x0F)+10*(val>>4);
}

void CopyTime (RTC_Time_t* SrcTime, RTC_Time_t* DstTime) {
	cli();
	for (uint8_t i = 0; i < 6; i++)
		*(((uint8_t*)DstTime)+i) = *(((uint8_t*)SrcTime)+i);
	sei();
}

void ClearTime (RTC_Time_t* Time1) {
	cli();
	Time1->Year = 0;
	Time1->Month = 0;
	Time1->Day = 0;
	Time1->Hour = 0;
	Time1->Minute = 0;
	Time1->Second = 0;
	sei();
}

uint8_t TimeBetweenTimes(RTC_Time_t* time, RTC_Time_t* start, RTC_Time_t* stop) {
	
	// time >= start and time <= stop then return True -> ie if time == start or time == stop then it is 'between'.
	// time < start or time > stop then return False
	
	// time > 0 and time < 3 means that time is > start but < stop -> this might be quite useful
	
	// time > start and time < stop then 1
	// time == start then 2
	// time == stop  then 3
	// time == start && time == stop then 4
	// time < start then 0
	// time > stop then 0
	
	//If start > stop then the output is undefined.
	
	uint8_t a = FirstTimeGreaterOrEqualTo(time, start);
	uint8_t b = FirstTimeGreaterOrEqualTo(stop, time);
	
	if (a == 1 && b == 1) return 1;
	else if (a == 2 && b == 1) return 2;
	else if (a == 1 && b == 1) return 3;
	else if (a == 2 && b == 2) return 4;
	else return 0;
	
	//if time > start, a = 1	Y
	//if time == start, a = 2	Y
	//if time < start, a = 0	N
	//if time < stop, b = 1		Y
	//if stop == time, b = 2	Y
	//if time > stop, b = 0		N
	//if a > 0 and b > 0 then return true else return false
	
	//if a or b == 2 then we are on a boundary
	//if a == 2, then time == start
	//if b == 2, then time == stop
	//if a == b == 2, then time == start == stop
}

uint8_t FirstTimeGreaterOrEqualTo (RTC_Time_t* Time1, RTC_Time_t* Time2) {
	//It is assumed that times are VALID!

	//if Time1 >= Time2 then TRUE
	//if Time1 < Time2  then FALSE

	//if Time1 > Time2 then 1
	//if Time2 > Time1 then 0
	//if Time1 = Time2 then 2

	if (Time2->Year > Time1->Year) {
		return 0;
	} else if (Time2->Year < Time1->Year) {
		return 1;
	} else {
		if (Time2->Month > Time1->Month) {
			return 0;
		} else if (Time2->Month < Time1->Month) {
			return 1;
		} else {
			if (Time2->Day > Time1->Day) {
				return 0;
			} else if (Time2->Day < Time1->Day) {
				return 1;
			} else {
				if (Time2->Hour > Time1->Hour) {
					return 0;
				} else if (Time2->Hour < Time1->Hour) {
					return 1;
				} else {
					if (Time2->Minute > Time1->Minute) {
						return 0;
					} else if (Time2->Minute < Time1->Minute) {
						return 1;
					} else {
						if (Time2->Second > Time1->Second) {
							return 0;
						} else if (Time2->Second < Time1->Second) {
							return 1;
						} else {
							return 2;
						}
					}
				}
			}
		}
	}

}

uint8_t ValidDateTime(RTC_Time_t* DT) {
	uint8_t days;
	if (DT->Second > 59 || DT->Minute > 59 || DT->Hour > 23)
		return 0;

	if (DT->Day == 0 || DT->Month == 0 || DT->Month > 12)
		return 0;

	days = DaysInMonth(DT->Month, DT->Year);

	if (DT->Day > days)
		return 0;

	//If we got here.. it's a valid date!
	return 1;
}

uint32_t TimeToLinuxTimestamp(RTC_Time_t* DateTime) {
	
}

void AddTimes (RTC_Time_t* DateTime, RTC_Time_t* AddTime) {
	//DateTime = DateTime + AddTime
	//AddTime can be a specific number of days month minutes etc.
	//DateTime MUST BE A PROPER DATETIME!
	//Double overflows are supported
	//Overflows added from small time through to large time
	//(ie. secs then mins then hours then days etc.)

	uint8_t a;
	uint8_t YearsAddedInMonth = 0;

	DateTime->Second += AddTime->Second;
	while (DateTime->Second > 59) {
		DateTime->Second -= 60;
		DateTime->Minute++;
	}
	
	DateTime->Minute += AddTime->Minute;
	while (DateTime->Minute > 59) {
		DateTime->Minute -= 60;
		DateTime->Hour++;
	}
	
	DateTime->Hour += AddTime->Hour;
	while (DateTime->Hour > 23) {
		DateTime->Hour -= 24;
		DateTime->Day++;
	}
	
	DateTime->Day += AddTime->Day;
	while (DateTime->Day > (a = DaysInMonth(DateTime->Month, DateTime->Year))) {
		DateTime->Day -= a;
		DateTime->Month++;
		if (DateTime->Month > 12) {
			DateTime->Month -= 12;
			DateTime->Year++;
			YearsAddedInMonth++;
		}
	}
	
	DateTime->Month += (AddTime->Month - YearsAddedInMonth);
	while (DateTime->Month > 12) {
		DateTime->Month -= 12;
		DateTime->Year++;
	}
	
	DateTime->Year += AddTime->Year;
	//while (DateTime->Year > 99) {
	//	DateTime->Year -= 100;
	//}
}

uint8_t DaysInMonth(uint8_t Month, uint8_t Year) {
	switch (Month) {
		//31 day months
		case 1://Jan
		case 3://Mar
		case 5://May
		case 7://July
		case 8://Aug
		case 10://Oct
		case 12://Dec
			return 31;
			break;
		//30 day months
		case 4://April
		case 6://June
		case 9://Sep
		case 11://Nov
			return 30;
			break;
		//The dreaded February!!!
		case 2://Feb
			//Is it a leap year??
			if(((Year + 2000) % 4 == 0 && (Year + 2000) % 100 != 0) || (Year + 2000) % 400 == 0)
				return 29;//Yes
			else
				return 28;//No
			break;
		default:
			return 0;
		}
}

void sPrintDateTime (char* __s, RTC_Time_t* pTime, char* lbl) {
	if (lbl[0] == '\0')
		sprintf_P(__s,PSTR("%02d-%02d-%02d %02d:%02d:%02d")
						, pTime->Day
						, pTime->Month
						, pTime->Year
						, pTime->Hour
						, pTime->Minute
						, pTime->Second);
	else
		sprintf_P(__s,PSTR("%s: %02d-%02d-%02d %02d:%02d:%02d\r\n")
					, lbl
					, pTime->Day
					, pTime->Month
					, pTime->Year
					, pTime->Hour
					, pTime->Minute
					, pTime->Second);
	//USART_tx_String(s);
}

void sPrintDate (char* __s, RTC_Time_t* pTime) {
	sprintf_P(__s,PSTR("%02d-%02d-%02d")
			, pTime->Day
			, pTime->Month
			, pTime->Year);
}

void sPrintTime (char* __s, RTC_Time_t* pTime) {
	sprintf_P(__s,PSTR("%02d:%02d:%02d")
			, pTime->Hour
			, pTime->Minute
			, pTime->Second);
}

int str2Time(char* __s, RTC_Time_t* destTime) {
	//Format of string: DDMMYYHHMMSS - that's 12 characters.
	int i;

	//check that it's 'valid'
	for(i = 0; i < 12; i++) {
		if (__s[i] < '0' || __s[i] > '9')
			return -1;
	}

	destTime->Day = BCDToInt((uint8_t)(((__s[0]-'0')<<4)+(__s[1]-'0')));
	destTime->Month = BCDToInt((uint8_t)(((__s[2]-'0')<<4)+(__s[3]-'0')));
	destTime->Year = BCDToInt((uint8_t)(((__s[4]-'0')<<4)+(__s[5]-'0')));
	destTime->Hour = BCDToInt((uint8_t)(((__s[6]-'0')<<4)+(__s[7]-'0')));
	destTime->Minute = BCDToInt((uint8_t)(((__s[8]-'0')<<4)+(__s[9]-'0')));
	destTime->Second = BCDToInt((uint8_t)(((__s[10]-'0')<<4)+(__s[11]-'0')));

	if (ValidDateTime(destTime))
		return 0;
	else
		return -3;
}

void AddToTime(RTC_Time_t* dateTime, uint8_t Yrs, uint8_t Months, uint8_t Days, uint8_t Hrs, uint8_t Mins, uint8_t Secs) {
	RTC_Time_t tmpTime;

	ClearTime(&tmpTime);
	tmpTime.Year = Yrs;
	tmpTime.Month = Months;
	tmpTime.Day = Days;
	tmpTime.Hour = Hrs;
	tmpTime.Minute = Mins;
	tmpTime.Second = Secs;
	AddTimes(dateTime, &tmpTime);
}

void TimeAfterTime(RTC_Time_t* startTime, RTC_Time_t* destTime, uint8_t Yrs, uint8_t Months, uint8_t Days, uint8_t Hrs, uint8_t Mins, uint8_t Secs) {
	RTC_Time_t tmpTime;

	CopyTime(startTime, destTime);
	ClearTime(&tmpTime);
	tmpTime.Year = Yrs;
	tmpTime.Month = Months;
	tmpTime.Day = Days;
	tmpTime.Hour = Hrs;
	tmpTime.Minute = Mins;
	tmpTime.Second = Secs;
	AddTimes(destTime, &tmpTime);
}

void AddSecondsToEpoch(RTC_Time_t* epoch, uint32_t secs, RTC_Time_t* destTime) {
	struct tm t;
	time_t t_of_day;
	t.tm_year = epoch->Year+2000-1900;
	t.tm_mon = epoch->Month-1;
	t.tm_mday = epoch->Day;
	t.tm_hour = epoch->Hour;
	t.tm_min = epoch->Minute;
	t.tm_sec = epoch->Second;
	t.tm_isdst = -1; //Is DST on? 1=yes, 0=no, -1=unknown
	t_of_day = mk_gmtime(&t);
	
	t_of_day += secs;
	
	struct tm ts;
	ts = *gmtime(&t_of_day);
	
	destTime->Year = ts.tm_year - 2000 + 1900;
	destTime->Month = ts.tm_mon + 1;
	destTime->Day = ts.tm_mday;
	destTime->Hour = ts.tm_hour;
	destTime->Minute = ts.tm_min;
	destTime->Second = ts.tm_sec;
}

void SubtractSecondsFromTime(RTC_Time_t* time, uint32_t secs) {
	struct tm t;
	time_t t_of_day;
	t.tm_year = time->Year+2000-1900;
	t.tm_mon = time->Month-1;
	t.tm_mday = time->Day;
	t.tm_hour = time->Hour;
	t.tm_min = time->Minute;
	t.tm_sec = time->Second;
	t.tm_isdst = -1; //Is DST on? 1=yes, 0=no, -1=unknown
	t_of_day = mk_gmtime(&t);
	
	t_of_day -= secs;
	
	struct tm ts;
	ts = *gmtime(&t_of_day);
	
	time->Year = ts.tm_year - 2000 + 1900;
	time->Month = ts.tm_mon + 1;
	time->Day = ts.tm_mday;
	time->Hour = ts.tm_hour;
	time->Minute = ts.tm_min;
	time->Second = ts.tm_sec;
}


void ZipTime(RTC_Time_t* rawTime, uint8_t* zipTime, uint8_t Flags) {
	//using compression method B:
	//							Byte 1
	//Bit 7	Bit 6	Bit 5	Bit 4	Bit 3	Bit 2	Bit 1	Bit 0
	//Yr1	Yr0		Sec5	Sec4	Sec3	Sec2	Sec1	Sec0
	//							Byte 2
	//Bit 7	Bit 6	Bit 5	Bit 4	Bit 3	Bit 2	Bit 1	Bit 0
	//Yr3	Yr2		Min5	Min4	Min3	Min2	Min1	Min0
	//							Byte 3
	//Bit 7	Bit 6	Bit 5	Bit 4	Bit 3	Bit 2	Bit 1	Bit 0
	//Yr6	Yr5		Yr4		Hr4		Hr3		Hr2		Hr1		Hr0
	//							Byte 4
	//Bit 7	Bit 6	Bit 5	Bit 4	Bit 3	Bit 2	Bit 1	Bit 0
	//Yr9	Yr8		Yr7		Day4	Day3	Day2	Day1	Day0
	//							Byte 5
	//Bit 7	Bit 6	Bit 5	Bit 4	Bit 3	Bit 2	Bit 1	Bit 0
	//FLG1	FLG0	Yr11	Yr10	Mon3	Mon2	Mon1	Mon0

	uint16_t Year = rawTime->Year + 2000;

	zipTime[0] = (rawTime->Second&0x3F)|((((uint8_t)Year&0x0003))<<6);
	zipTime[1] = (rawTime->Minute&0x3F)|((((uint8_t)Year&0x000C))<<4);
	zipTime[2] = (rawTime->Hour&0x1F)|((((uint8_t)Year&0x0070))<<1);
	zipTime[3] = (rawTime->Day&0x1F)|((uint8_t)((Year&0x0380)>>2));
	zipTime[4] = (rawTime->Month&0x0F)|((uint8_t)((Year&0x0C00)>>6))|((Flags&0x03)<<6);

}

uint8_t UnzipTime(RTC_Time_t* rawTime, uint8_t* zipTime) {
	//using compression method B:
	//							Byte 1
	//	Bit 7	Bit 6	Bit 5	Bit 4	Bit 3	Bit 2	Bit 1	Bit 0
	//	Yr1		Yr0		Sec5	Sec4	Sec3	Sec2	Sec1	Sec0
	//							Byte 2
	//	Bit 7	Bit 6	Bit 5	Bit 4	Bit 3	Bit 2	Bit 1	Bit 0
	//	Yr3		Yr2		Min5	Min4	Min3	Min2	Min1	Min0
	//							Byte 3
	//	Bit 7	Bit 6	Bit 5	Bit 4	Bit 3	Bit 2	Bit 1	Bit 0
	//	Yr6		Yr5		Yr4		Hr4		Hr3		Hr2		Hr1		Hr0
	//							Byte 4
	//	Bit 7	Bit 6	Bit 5	Bit 4	Bit 3	Bit 2	Bit 1	Bit 0
	//	Yr9		Yr8		Yr7		Day4	Day3	Day2	Day1	Day0
	//							Byte 5
	//	Bit 7	Bit 6	Bit 5	Bit 4	Bit 3	Bit 2	Bit 1	Bit 0
	//	FLG1	FLG0	Yr11	Yr10	Mon3	Mon2	Mon1	Mon0
	
	//Extra bytes to add?
	//							Byte 6
	//	Bit 7	Bit 6	Bit 5	Bit 4	Bit 3	Bit 2	Bit 1	Bit 0
	//	Act7	Act6	Act5	Act4	Act3	Act2	Act1	Act0
	//							Byte 7
	//	Bit 7	Bit 6	Bit 5	Bit 4	Bit 3	Bit 2	Bit 1	Bit 0
	//	IAct7	IAct6	IAct5	IAct4	IAct3	IAct2	IAct1	IAct0

	uint16_t Year;

	rawTime->Second = zipTime[0]&0x3F;
	rawTime->Minute = zipTime[1]&0x3F;
	rawTime->Hour = zipTime[2]&0x1F;
	rawTime->Day = zipTime[3]&0x1F;
	rawTime->Month = zipTime[4]&0x0F;
	Year = (((uint16_t)zipTime[0]&0xC0)>>6)|(((uint16_t)zipTime[1]&0xC0)>>4)|
			(((uint16_t)zipTime[2]&0xE0)>>1)|(((uint16_t)zipTime[3]&0xE0)<<2)|(((uint16_t)zipTime[4]&0x30)<<6);

	rawTime->Year = Year - 2000;
	

	//Return FLG (FLG0 in  bit 0, FLG1 in bit 1)
	return zipTime[4]>>6;

}
