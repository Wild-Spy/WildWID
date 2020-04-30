/*
 * LoggerLookupTable.h
 *
 *  Created on: 28/07/2016
 *      Author: Matthew Cochrane
 */

//Usage of this module:
/* Call LLUT_Init() on system setup
 * Call LLUT_FindLoggerOrAdd(...) when you want to update a logger, use this regardless of
 *                                whether the logger exists in the table or not yet.
 *								  This function modifies the loggers flags and is_ok timer.
 * Call LLUT_ShortIdFromLoggerId(...) if you want to lookup a logger without modifying its 
 *								      flags or is_ok timer.
 * Call LLUT_GetDataPtr() to get a pointer to the start of the table in memory.  Increment
 *						  over LLUT_GetCount() items to service each element in the table.
 *
 *
 * Call LLUT_DecrementIsOkTimers() once every minute to decrement all is_ok timers in the
 *							       table.
 * Call LLUT_SetCurrentLoggerVoltage(...) to set the current logger's voltage in the table.
 */

#ifndef __LOGGERLOOKUPTABLE_H_
#define __LOGGERLOOKUPTABLE_H_

#include <stdlib.h>
#include <string.h>
#include <avr/io.h>
#include "MX_RTC.h"
#include "../Strings.h"
#include "MX_ErrorEnum.h"

#define LLUTMAXSIZE		40		//!<Maximum number of entries in the list.
#define LLUTRECORD_SIZE	sizeof(LLUTStruct_t)
#define LLUTBYTESIZE	LLUTMAXSIZE*LLUTRECORD_SIZE

#define LLUT_ID_COUNT_LEN_BYTES 1
#define LLUT_LOGGERID_SIZE	4 //sizeof(uint32_t)
//#define FLASH_OFFSET_START	(LLUT_ID_COUNT_LEN_BYTES + LLUTMAXSIZE*LLUT_LOGGERID_SIZE)

typedef struct _LLUTStruct_t {
	uint32_t ID;
	uint8_t flags;	//0b76543210
					//  0 - isok (see the is_ok flag)
					//  1 - low voltage
					//  2 - data storage low
					//  3 - haven't received update yet
	uint16_t bat_volt; //voltage * 1000 (ie 12.340V = 12340)
	uint8_t flash_usage; //255 = 100%, 0 = 0%
	uint8_t is_ok; //a timer that keeps being decremented, each tick is 1 minute, set to 120 when we receive an update
				   //so we have 2 hours to receive the next update, if we don't receive it we flag an error 'is_ok=false'.
} LLUTStruct_t;

//Function Prototypes:
mx_err_t LLUT_Init(void);

//This function updates the logger
mx_err_t LLUT_FindLoggerOrAdd(uint32_t loggerId, uint8_t* index, LLUTStruct_t** info);
uint8_t LLUT_GetCount(void);
mx_err_t LLUT_ClearList(void);

mx_err_t LLUT_LoggerIdFromShortId(uint8_t index, uint32_t* loggerId);
mx_err_t LLUT_ShortIdFromLoggerId(uint32_t loggerId, uint8_t* index, LLUTStruct_t** info);

void LLUT_DecrementIsOkTimers();
void LLUT_SetCurrentLoggerVoltage(float voltage);

LLUTStruct_t* LLUT_GetDataPtr();

//Private functions
#ifdef UNIT_TEST
mx_err_t LLUT_AddLogger(uint32_t loggerID, uint8_t* index, LLUTStruct_t** info );

uint8_t LLUT_GetCountFromFlash(void);
mx_err_t LLUT_WriteCountToFlash(uint8_t count);
mx_err_t LLUT_GetLoggerIdFromFlash(uint8_t index, uint32_t* loggerId);
mx_err_t LLUT_WriteLoggerIdToFlash(uint8_t index, uint32_t loggerId);
#endif

#endif