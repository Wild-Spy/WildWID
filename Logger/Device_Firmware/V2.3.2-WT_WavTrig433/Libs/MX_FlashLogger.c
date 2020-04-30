/*
 * MX_FlashLogger.c
 *
 *  Created on: 22/07/2016
 *      Author: Matthew Cochrane
 */
/* This file has been prepared for Doxygen automatic documentation generation.*/

#include "MX_FlashLogger.h"
#include "MX_SerFlash.h"
#include "SettingsManager.h"

/*!	\brief Saves a tag pickup.
 *
 *	Doesn't necessarily save that event to flash.  Only the first and last events are
 *	written to flash.  If the tag is not already in the list #MList_Data then it will be 
 *	saved to flash with a start condition and added to the list.  If the tag is already in
 *	the list then update the time of the list entry to the new time but do not save it
 *	to the serial flash.
 *
 *	\param mTime The time of the tag pickup event
 *	\param ID The ID of the tag that was picked up
 *	\return 0 for success, 1 for full buffer, 2 for Flash write failure
 */
uint8_t saveRecord(RTC_Time_t* mTime, uint32_t ID, uint8_t RSSI, uint8_t Activity, uint8_t loggerShortId) {
	//ListStruct_t* ListItem;

	//if in current list of tags...
	//if (mList_ItemWithID(ID, &ListItem) == 0) {
		//Update the latest time
	//	CopyTime(mTime, &(ListItem->LastTime));
	//} else {
		//if not in the current list of tags...
		if (RecordToFlash(mTime, ID, 0x03, RSSI, Activity, loggerShortId) == 0) {
			return 2;
		}
	//	uint32_t Addy = RecordCount - 1;
	//	if (mList_AddItem(ID, Addy, mTime) != 0) {
			//we're fucked!! - out of space in the list.
			//well, not really, it will just log every event rather than just start and stop now
			//because it won't actually get added to the list.
			//(1)USART_tx_String_P(&USARTC0, PSTR("LIST FULL!!! we're boned!\r\n"));
	//		return 1;
	//	}
	//}
	return 0;
}

/*!	\brief Reads a record from the serial flash.
 *
 *	The record at record number RecordNo is read from the serial flash and returned through the two pointers
 *	mTime and ID.
 *
 *	\param [in] RecordNo
 *	\param [out] mTime
 *	\param [out] ID
 *	\return 0xFF if the function failed, the entries flag value otherwise
 */
uint8_t loadRecord(uint32_t RecordNo, RTC_Time_t* mTime, uint32_t* ID, uint8_t* RSSI, uint8_t* Activity, uint8_t* loggerShortId) {
	uint8_t tZip[5];

	if (FLASH_OFFSET_START+RecordNo*FLASHRECORD_SIZE + FLASHRECORD_SIZE+1 < SerFlash_MAXADDRESS) {
		ClearTime(mTime);
		SerFlash_ReadBytes(FLASH_OFFSET_START+RecordNo*FLASHRECORD_SIZE, 5, tZip);
		SerFlash_ReadBytes(FLASH_OFFSET_START+RecordNo*FLASHRECORD_SIZE+5, 4, (uint8_t*)ID);
		SerFlash_ReadBytes(FLASH_OFFSET_START+RecordNo*FLASHRECORD_SIZE+9, 1, (uint8_t*)RSSI);
		SerFlash_ReadBytes(FLASH_OFFSET_START+RecordNo*FLASHRECORD_SIZE+10, 1, (uint8_t*)Activity);
		SerFlash_ReadBytes(FLASH_OFFSET_START+RecordNo*FLASHRECORD_SIZE+11, 1, (uint8_t*)loggerShortId);

		return UnzipTime(mTime, tZip);
	} else {
		return 0xFF;
	}
}

/*!	\brief Saves the passed information to flash.
 * 
 *	The information is saved to the next free entry in the data flash which is calculated from
 *	the value of RecordCount.
 *	
 *	\param mTime the time that the event occurred
 *	\param ID The ID of the tag
 *	\param Flags 2-bit flag value, usually represents whether this is to be a start, stop or system message entry
 *	\return 1 if success, 0 if failure
*/
uint8_t RecordToFlash(RTC_Time_t* mTime, uint32_t ID, uint8_t Flags, uint8_t RSSI, uint8_t Activity, uint8_t loggerShortId) {
	uint8_t tZip[5];
	uint32_t recordStartAddress = FLASH_OFFSET_START+RecordCount*FLASHRECORD_SIZE;

	if (recordStartAddress + FLASHRECORD_SIZE <= SerFlash_MAXADDRESS) {
		//RTC_Time_t tmpTime1, tmpTimeAdd;

		Flags &= 0b00000011;
		ZipTime(mTime, tZip, Flags);
		SerFlash_WriteBytes((uint32_t)(recordStartAddress),    5, tZip);
		SerFlash_WriteBytes((uint32_t)(recordStartAddress+5),  4, (uint8_t*)(&ID));
		SerFlash_WriteBytes((uint32_t)(recordStartAddress+9),  1, &RSSI);
		SerFlash_WriteBytes((uint32_t)(recordStartAddress+10), 1, &Activity);
		SerFlash_WriteBytes((uint32_t)(recordStartAddress+11), 1, &loggerShortId);
		RecordCount++;

		/*//Save recordCount to eeprom once every 100 writes or once per day.
		//TODO[ ]: make sure this works!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!1
		ClearTime(&tmpTimeAdd);
		tmpTimeAdd.Day = 1;
		CopyTime(&RecCount_LastSave, &tmpTime1);
		AddTimes(&tmpTime1, &tmpTimeAdd);
		if (FirstTimeGreater(&RTC_c_Time, &tmpTime1) || ((RecordCount % 100) == 0)) {
			RCTOEEPROM();
			CopyTime(&RTC_c_Time, &RecCount_LastSave);
		}*/

		//TODO[ ]: Don't write RecordCount to EEPROM EVERY time!! maybe once per 100?
		return 1;
	} else
		return 0;
}


/*!	\brief Count the number of stored records in the flash memory
 *
 *	Search through the external flash memory and count how many records
 *	are stored there in total.  It does this by counting until it finds
 *	an empty record.
 *
 *	\return Record count on serial flash
 */
uint32_t FindRecordCount() {
	uint32_t cnt = 0;
	uint8_t i;
	uint8_t FFCount;
	uint8_t tmpData[FLASHRECORD_SIZE];
	
	//return (uint16_t)1000;
	
	//return (SerFlash_MAXADDRESS/FLASHRECORD_SIZE);
	
	//TODO[ ]: Can improve the efficiency of this function
	for (cnt = 0; FLASH_OFFSET_START+cnt*FLASHRECORD_SIZE <= SerFlash_MAXADDRESS-FLASHRECORD_SIZE; cnt++) {
		SerFlash_ReadBytes(FLASH_OFFSET_START+cnt*FLASHRECORD_SIZE, FLASHRECORD_SIZE, tmpData);
		
		FFCount = 0;
		for (i = 0; i < FLASHRECORD_SIZE; i++) {
			if (tmpData[i] == 0xFF) {
				FFCount++;
			}
		}			
		if 	(FFCount == FLASHRECORD_SIZE) {
			//How it works:
			//RC		1 2 3 4 5
			//hasData	1 1 1 1 1 0
			//cnt		0 1 2 3 4 5
			//return val = 5
			return cnt;
		}
	}
	
	return ((SerFlash_MAXADDRESS-FLASH_OFFSET_START)/FLASHRECORD_SIZE);
	
}