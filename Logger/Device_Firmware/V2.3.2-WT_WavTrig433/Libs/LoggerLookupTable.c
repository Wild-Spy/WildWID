/*
 * LoggerLookupTable.c
 *
 *  Created on: 28/07/2016
 *      Author: Matthew Cochrane
 */

#include "LoggerLookupTable.h"
#include "MX_SerFlash.h"
#include "MX_FlashLogger.h" //Needed for FLASH_OFFSET_START
#include "SettingsManager.h"

static LLUTStruct_t LLUT_Data[LLUTMAXSIZE];	//!<Array containing list data.  Is a constant size depending on #LISTMAXSIZE.
static uint8_t LLUT_Count = 1;	//!<The number of entries currently stored in the list.

mx_err_t LLUT_AddLogger(uint32_t loggerID, uint8_t* index, LLUTStruct_t** info );

uint8_t LLUT_GetCountFromFlash(void);
mx_err_t LLUT_WriteCountToFlash(uint8_t count);
mx_err_t LLUT_GetLoggerIdFromFlash(uint8_t index, uint32_t* loggerId);
mx_err_t LLUT_WriteLoggerIdToFlash(uint8_t index, uint32_t loggerId);
void LLUT_UpdateLoggerFlags(LLUTStruct_t* info, bool self);

LLUTStruct_t* LLUT_GetDataPtr() {
	LLUT_Data[0].ID = setting_LoggerId;
	LLUT_Data[0].is_ok = 120;
	//bat voltage in LLUT_SetCurrentLoggerVoltage
	LLUT_Data[0].flash_usage = ((double)RecordCount/(double)SerFlash_SIZE)*255;
	LLUT_Data[0].flags |= (1<<0); //isok = yes
	LLUT_UpdateLoggerFlags(&LLUT_Data[0], true);
	
	//TODO[ ]: test!
	for (uint8_t i = 0; i < LLUT_Count; ++i) {
		LLUT_UpdateLoggerFlags(&LLUT_Data[i], false);
	}
	
	return LLUT_Data;
}

/*!	\brief [99.9%] Initialises the list
 *
 *	\return Whether function succeeded.  #mx_err_Success or #mx_err_Overflow.
 */
mx_err_t LLUT_Init() {
	
	uint32_t* tmp_loggerId;
	LLUT_Count = LLUT_GetCountFromFlash();
	
	LLUT_Data[0].ID = setting_LoggerId;
	LLUT_Data[0].bat_volt = 0;
	LLUT_Data[0].is_ok = 0;
	LLUT_Data[0].flash_usage = 0;
	LLUT_Data[0].flags = (1<<0);
	
	if (LLUT_Count == 0xFF || LLUT_Count > LLUTMAXSIZE || LLUT_Count == 0) {
		LLUT_ClearList();
	}
	
	for (uint16_t i = 1; i < LLUT_Count; ++i) {
		if (LLUT_GetLoggerIdFromFlash((uint8_t)i, &tmp_loggerId) != mx_err_Success) {
			return mx_err_Overflow;
		}
		//LLUT_Data[i] = tmp_loggerInfo;
		LLUT_Data[i].ID = tmp_loggerId;
		LLUT_Data[i].bat_volt = 0;
		LLUT_Data[i].is_ok = 0;
		LLUT_Data[i].flash_usage = 0;
		LLUT_Data[i].flags = (1<<3);
	}
	
	return mx_err_Success;
}

void LLUT_DecrementIsOkTimers() {
	for (uint8_t i = 1; i < LLUT_Count; ++i) {
		if (LLUT_Data[i].is_ok > 0) --LLUT_Data[i].is_ok;
	}
}

void LLUT_SetCurrentLoggerVoltage(float voltage) {
	LLUT_Data[0].bat_volt = (uint16_t)(voltage*1000);
}

void LLUT_UpdateLoggerFlags(LLUTStruct_t* info, bool self) {
	if (self) {
		if (info->bat_volt < 11500) { //11.5V
			info->flags |= (1<<1); //low voltage = yes
		} else {
			info->flags &= ~(1<<1); //low voltage = no
		}
		if (info->flash_usage >= 230) { //more than 90% used
			info->flags |= (1<<2); //data storage low = yes
		} else {
			info->flags &= ~(1<<2); //data storage low = no
		}
	}
	if (info->is_ok) {
		info->flags |= (1<<0);
	} else {
		info->flags &= ~(1<<0);
	}
}

mx_err_t LLUT_FindLoggerOrAdd(uint32_t loggerId, uint8_t* index, LLUTStruct_t** info) {
	mx_err_t retVal;
	
	if (LLUT_ShortIdFromLoggerId(loggerId, index, info) == mx_err_Success) {
		retVal = mx_err_Success;
	} else {
		retVal = LLUT_AddLogger(loggerId, index, info);
	}
	
	if (retVal == mx_err_Success) {
		(*info)->is_ok = 120;
		(*info)->flags &= ~(1<<3);
	}
	
	return retVal;
}

/*!	\brief [95%] Returns a pointer to the item in the list with the passed index.
 *
 *	\param [in] Index Index in list
 *	\param [out] RetVal Returned Pointer
 *	\return #mx_err_NotFound or #mx_err_Success
 */
mx_err_t LLUT_LoggerIdFromShortId(uint8_t index, uint32_t* loggerId) {

	//As an example, if we have one item in our list, Count = 1 and
	//the associated index is 0.  If the passed Index to this function
	//is equal to count (eg. if count = 1 and passed index = 1) or is
	//greater than count then the index has no associated data yet and
	//an error should be returned.
	if (index >= LLUT_Count) {
		return mx_err_NotFound;	//not found
	}

	//Only 95% sure that this works
	*loggerId = LLUT_Data[index].ID;
	return mx_err_Success;

}

/*!	\brief [??%] Find first item with specified ID in the list.
 *
 *	Search through the list and return a pointer to the first occurrence
 *	of an entry with the ID being searched for.
 *
 *	\param [in] ID ID being searched for
 *	\param [out] RetVal Pointer to first matching item if found
 *	\return #mx_err_NotFound or #mx_err_Success
 */
mx_err_t LLUT_ShortIdFromLoggerId(uint32_t loggerId, uint8_t* index, LLUTStruct_t** info) {

	int i;

	for (i = 0; i < LLUT_Count; i++) {
		if (loggerId == LLUT_Data[i].ID) {
			*index = i;
			*info = &LLUT_Data[i];
			return mx_err_Success;
		}
	}

	return mx_err_NotFound;	//not found
}

/*!	\brief [100%] Number of items currently in the list.
 *
 *	\return #LLUT_Count
 */
uint8_t LLUT_GetCount() {
	return LLUT_Count;
}

mx_err_t LLUT_ClearList(void) {
	LLUT_Count = 1; //First index represents this logger Id
	LLUT_WriteCountToFlash(LLUT_Count);
	return mx_err_Success;
}

mx_err_t LLUT_AddLogger(uint32_t loggerID, uint8_t* index, LLUTStruct_t** info) {
	//Declaring Data[MAX] means Data[0] -> Data[MAX-1] exist.
	//So when Count == MAX (and beyond) the list is full.
	if (LLUT_Count >= LLUTMAXSIZE) {
		return mx_err_Overflow;
	}

	*index = LLUT_Count;
	*info = &LLUT_Data[LLUT_Count];
	LLUT_Data[LLUT_Count].ID = loggerID;
	LLUT_Data[LLUT_Count].bat_volt = 0;
	LLUT_Data[LLUT_Count].is_ok = 120;
	LLUT_Data[LLUT_Count].flash_usage = 0;
	LLUT_Data[LLUT_Count].flags = (1<<3) | (1<<0);
	LLUT_WriteLoggerIdToFlash(LLUT_Count, loggerID);
	LLUT_Count++;
	LLUT_WriteCountToFlash(LLUT_Count);

	return mx_err_Success;
}

uint8_t LLUT_GetCountFromFlash(void) {
	uint8_t count;
	SerFlash_ReadBytes(0, LLUT_ID_COUNT_LEN_BYTES, (uint8_t*)(&count));
	return count;
}

mx_err_t LLUT_WriteCountToFlash(uint8_t count) {
	SerFlash_WriteBytes(0, LLUT_ID_COUNT_LEN_BYTES, (uint8_t*)(&count));
	return mx_err_Success;
}

mx_err_t LLUT_GetLoggerIdFromFlash(uint8_t index, uint32_t* loggerId) {
	if (LLUT_ID_COUNT_LEN_BYTES+index*LLUTRECORD_SIZE + LLUTRECORD_SIZE+1 < FLASH_OFFSET_START) {
		SerFlash_ReadBytes((uint32_t)(LLUT_ID_COUNT_LEN_BYTES+index*LLUT_LOGGERID_SIZE), LLUT_LOGGERID_SIZE, (uint8_t*)loggerId);
		return mx_err_Success;
	} else {
		return mx_err_Overflow;
	}
}

mx_err_t LLUT_WriteLoggerIdToFlash(uint8_t index, uint32_t loggerId){
	if (LLUT_ID_COUNT_LEN_BYTES+index*LLUTRECORD_SIZE + LLUTRECORD_SIZE <= FLASH_OFFSET_START) {
		SerFlash_WriteBytes((uint32_t)(LLUT_ID_COUNT_LEN_BYTES+index*LLUT_LOGGERID_SIZE), LLUT_LOGGERID_SIZE, (uint8_t*)(&loggerId));
		return mx_err_Success;
	} else {
		return mx_err_Overflow;
	}
}
