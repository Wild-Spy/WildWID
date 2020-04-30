/*
 * MX_FlashLogger.h
 *
 *  Created on: 22/07/2016
 *      Author: Matthew Cochrane
 */
/* This file has been prepared for Doxygen automatic documentation generation.*/
/*! \file *********************************************************************
 *
 * \brief  List 'class' implementation header file.
 *
 * \author
 *      Matthew Cochrane \n
 *      Wild Spy
 *
 * $Revision: 1 $
 * $Date: 03/10/2011 $  \n
 *
 *****************************************************************************/

#ifndef MX_FLSAHLOGGER_H_
#define MX_FLSAHLOGGER_H_

#include <stdint.h>
#include "MX_DateTime.h"
//#include "LoggerLookupTable.h"

#define FLASH_OFFSET_START				0x00000401L
//Saved to Flash when certain events occur (flags = 0x02 = 0b10)
//C -> Command Nibble, D -> Data Nibble
//										0xCCDDDDDD
#define SYS_MSG_PWR_ON					0x00000000L
#define SYS_MSG_WAKE_NOTIME				0x01000000L
#define SYS_MSG_WAKE_UD_TIME			0x02000000L
#define SYS_MSG_SEND_EMAIL_FAIL			0x03000000L //Data = number of attempts
#define SYS_MSG_SENT_EMAIL				0x04000000L	//Data = number of attempts
#define SYS_MSG_SEND_EMAIL_ATTEMPT		0x05000000L
#define SYS_MSG_UPDATED_TIME			0x06000000L //Data = source
#define SYS_MSG_PING					0x07000000L

#define SYS_MSG_UPDATE_TIME_SRC_IRIDIUM		0x01
#define SYS_MSG_UPDATE_TIME_SRC_PC			0x02
#define SYS_MSG_UPDATE_TIME_SRC_INTERLOGGER	0x03

#define FLASHRECORD_SIZE					12 // Changed to 12 29/7/2016 - added loggerShortId 11 //Changed to 11 12/5/2016	//size of a record in bytes
#define SAVE_SYS_MSG(NOW_Ptr, CMD, DATA)	RecordToFlash(NOW_Ptr, CMD + (DATA), 0x02, 0x00, 0x00, 0x00)

uint8_t loadRecord(uint32_t RecordNo, RTC_Time_t* mTime, uint32_t* ID, uint8_t* RSSI, uint8_t* Activity, uint8_t* loggerShortId);
uint8_t saveRecord(RTC_Time_t* mTime, uint32_t ID, uint8_t RSSI, uint8_t Activity, uint8_t loggerShortId);
uint32_t FindRecordCount();

uint8_t RecordToFlash(RTC_Time_t* mTime, uint32_t ID, uint8_t Flags, uint8_t RSSI, uint8_t Activity, uint8_t loggerShortId);


#endif /* MX_FLSAHLOGGER_H_ */
