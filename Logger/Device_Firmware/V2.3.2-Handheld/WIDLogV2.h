/*
 * WIDLogV2.h
 *
 *  Created on: 06/08/2010
 *      Author: Matthew Cochrane
 */
/* This file has been prepared for Doxygen automatic documentation generation.*/
/*! \file *********************************************************************
 *
 * \brief  WID Logger main source code header file.
 *
 *      This file contains the macros function definitions and defines that are
 *		to be used in the WIDLogV2.c file.
 *
 * \par Documentation
 *      No further documentation as of yet
 *
 * \author
 *      Matthew Cochrane \n
 *      Wild Spy
 *
 * $Revision: 0 $
 * $Date: 01/10/2011 $  \n
 *
 *****************************************************************************/

#ifndef WIDLOG_H_
#define WIDLOG_H_

#define F_CPU 12000000UL

#include <stddef.h>
#include <stdlib.h>
#include <avr/io.h>
#include <avr/sleep.h>
#include <util/delay.h>
#include <util/crc16.h>
#include <avr/pgmspace.h>
#include <avr/interrupt.h>
#include "Libs/clksys_driver.h"
#include "Libs/MX_NRF24L01.h"
#include "Libs/MX_USART.h"
#include "Libs/MX_SerFlash.h"
#include "Libs/MX_RTC.h"
#include "Libs/eeprom_driver.h"
#include "Libs/TC_driver.h"
#include "Libs/M_List.h"
#include "Libs/MX_GM862.h"
#include "Libs/CC2500.h"
#include "Libs/CC110L.h"
//#include "Libs/CC110LB.h"
#include "Libs/SettingsManager.h"
#include "Libs/WarningFlasher.h"
#include "Libs/InterLoggerRx.h"
#include "Libs/MX_Iridium9603.h"
#include "Strings.h"
#include "Libs/M_CountList.h"
#include "Libs/ErrorCorrection.h"
#include "Libs/cc110LTester.h"
#include "Libs/WakeupSource.h"
#include "Libs/wdt_driver.h"
#include "Libs/MX_SendData.h"
#include "Libs/MX_FlashLogger.h"
#include "Libs/MX_SerComms.h"
#include "Libs/PowerMacros.h"
#include "Libs/LoggerLookupTable.h"

//Defines:

#define RF_DISP_NONE					0
#define RF_DISP_SINGLETAGMODE			1
#define RF_DISP_TAGID					2
#define RF_DISP_TAGIDTIME				3
#define RF_DISP_TAGIDTIMEADD			4

enum SysClk_Speed {
	CLK_SPD_32MHz = 0,
	CLK_SPD_12MHz_SETUP,
	CLK_SPD_12MHz_SWITCH
};
//#define CLK_SPD_32MHz					1
//#define CLK_SPD_12MHz_SETUP			2
//#define CLK_SPD_12MHz_SWITCH			3

#define USART_TIMEOUT					15000	//wait 15 seconds
#define TAGLISTUPDATE					30		//in seconds
#define RFCALUPDATE						5		//in seconds
#define BATVOLTCHECK					10		//in seconds
#define ISOKTIMERUPDATE					60      //in seconds
#define TXSTATUSUPDATE					3600    //in seconds

// #define RECCOUNT_ADD				0x00	//a 4 byte unsigned integer (uint32_t)
// #define RFID_ADD					0x04	//a 5-byte unsigned integer	(uint8_t[5])
// #define RFCHAN_ADD					0x09	//a 1-byte unsigned integer	(uint8_t)
// #define TIMESAVE_ADD				0x0A	//a 6-byte structure (RTC_Time_t)				*****
// #define ADCOFFSETV_ADD				0x10	//a 2-byte unsigned integer (uint16_t)
// #define SELFILTER_ADD				0x12	//a 1-byte unsigned integer	(uint8_t)			*****
// #define LOGMODE_ADD					0x13	//a 1-byte unsigned integer	(uint8_t)
// #define FILTER0_ADD					0x14	//char Filter[8]
// #define NEXTGPRSSEND_ADD			0x1C	//a 6-byte structure (RTC_Time_t)
// #define SENDGPRSPER_ADD				0x22	//a 6-byte structure (RTC_Time_t)
// #define LOGGERNAME_ADD				0x28	//char LoggerName[20]
// #define EMAILTOADD_ADD				0x3C	//char EmailToAddress[60]
// #define GPRSAPN_ADD					0x78	//char GPRS_APN[30];
// #define GPRSUSERID_ADD				0x96	//char GPRS_UserId[30];
// #define GPRSPASSW_ADD				0xB4	//char GPRS_Passw[30];

//#define INTER_LOGGER_CC110L_INSTALLED
#define INTER_LOGGER_CC110L_DISABLE
#define HAS_IRIDIUM_MODULE

#define WDT_ENABLED	//comment out to DISABLE the watchdog timer (for finding what caused the reset)

#define WDT_DISABLE()		do {WDT_Disable();} while (0)
#define WDT_ENABLE()  		do {WDT_EnableAndSetTimeout(WDT_PER_8KCLK_gc);} while (0)
#define WDT_RESET()			do {WDT_Reset();} while (0)

//Uncomment the define below to use the secondary 433 (inter-logger) chip as the main WID pickup chip.
//This will disable inter-logger comms.
//#define _433_PICKUP_MODE
//Uncomment below if the logger is a dual 433MHZ Logger
//#define _DUAL_433_LOGGER
//#define SHOW_INTERLOGGER_DEBUG

//General Macros:
#define SAVESP()					SP_saved = (uint16_t)((SPH<<8)+SPL)
#define SAVESTATE()					do {SAVESP(); save_PC();} while (0)
#define LOGGINGISENABLED			(setting_LoggingEnabled > 1)

#define ByteNo(SrcPtr, byteNo)		*(((uint8_t*)SrcPtr)+byteNo)

//Pin Macros:


//EEPROM Variable Save Macros:
// #define RCTOEEPROM()				WriteEEPROM((uint8_t*)&RecordCount, 4, RECCOUNT_ADD)
// #define RCFROMEEPROM()				ReadEEPROM((uint8_t*)&RecordCount, 4, RECCOUNT_ADD)
// 
// #define RFCHANTOEEPROM()			WriteEEPROM((uint8_t*)&myRF_Chan, 1, RFCHAN_ADD)
// #define RFCHANFROMEEPROM()			ReadEEPROM((uint8_t*)&myRF_Chan, 1, RFCHAN_ADD)		
// 
// #define RFIDTOEEPROM()				WriteEEPROM((uint8_t*)myRF_ID, 5, RFID_ADD)
// #define RFIDFROMEEPROM()			ReadEEPROM((uint8_t*)myRF_ID, 5, RFID_ADD)
									
// #define LOGMODETOEEPROM()			WriteEEPROM((uint8_t*)&LoggingEnabled, 1, LOGMODE_ADD)
// #define LOGMODEFROMEEPROM()			ReadEEPROM((uint8_t*)&LoggingEnabled, 1, LOGMODE_ADD)

// #define WRITEFILTEREEPROM() 		WriteEEPROM((uint8_t*)Filter, 8, FILTER0_ADD)
// #define READFILTEREEPROM()  		ReadEEPROM((uint8_t*)Filter, 8,  FILTER0_ADD)

// #define GPRSSENDNEXTTOEEPROM()		WriteTimeEEPROM(&Send_Data_GPRS_Next, NEXTGPRSSEND_ADD)
// #define GPRSSENDNEXTFROMEEPROM()	ReadTimeEEPROM(&Send_Data_GPRS_Next, NEXTGPRSSEND_ADD)

// #define GPRSSENDPERTOEEPROM()		WriteTimeEEPROM(&Send_Data_GPRS_Period, SENDGPRSPER_ADD)
// #define GPRSSENDPERFROMEEPROM()		ReadTimeEEPROM(&Send_Data_GPRS_Period, SENDGPRSPER_ADD)

// #define EMAILTOADDTOEEPROM()		WriteEEPROM((uint8_t*)EmailToAddress, 60, EMAILTOADD_ADD)
// #define EMAILTOADDFROMEEPROM()		ReadEEPROM((uint8_t*)EmailToAddress, 60, EMAILTOADD_ADD)
// 
// #define LOGGERNAMETOEEPROM()		WriteEEPROM((uint8_t*)LoggerName, 20, LOGGERNAME_ADD)
// #define LOGGERNAMEFROMEEPROM()		ReadEEPROM((uint8_t*)LoggerName, 20, LOGGERNAME_ADD)

// #define GPRSSETINGSTOEEPROM()		WriteEEPROM((uint8_t*)GPRS_APN, 30, GPRSAPN_ADD); WriteEEPROM((uint8_t*)GPRS_UserId, 30, GPRSUSERID_ADD); WriteEEPROM((uint8_t*)GPRS_Passw, 30, GPRSPASSW_ADD)
// #define GPRSSETINGSFROMEEPROM()		ReadEEPROM((uint8_t*)GPRS_APN, 30, GPRSAPN_ADD); ReadEEPROM((uint8_t*)GPRS_UserId, 30, GPRSUSERID_ADD); ReadEEPROM((uint8_t*)GPRS_Passw, 30, GPRSPASSW_ADD)

//Function Prototypes:
void MainLoop_Active_Mode();
void MainLoop_BatBackup_NoBat_Mode();
void MainLoop_BatBackup_LoBat_Mode();
void SetupHardware( void );
uint8_t EnterBatBakMode(uint8_t Reason);
uint8_t WakeFromBatBakMode( void );
uint8_t BatVal(float batVolt);
float CheckBatteryVoltage( void );
void CalibrateADCOffset( void );
int ADCRead(uint8_t SAMP);
void RTCWakeChecks( void );
void doRFWorkCC2500( uint8_t );
void doRFWorkCC110L();
void doRFWorkCC110L_WID(uint8_t dispLvl);
void UpdateTagList( void );
void save_PC( void );
uint8_t ReadCalibrationByte( uint8_t index );
void FilterStr2Mask(char* filtStr);
void SetClkSpeed(uint8_t speed);

void SendEmailChecks();

void clearWakeupSource(uint8_t src);
#endif /* WIDLOG_H_ */
