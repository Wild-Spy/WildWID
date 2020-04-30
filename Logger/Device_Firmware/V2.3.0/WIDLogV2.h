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
#include "Libs/CC110LB.h"
#include "Libs/SettingsManager.h"
#include "Libs/WarningFlasher.h"
#include "Libs/InterLoggerRx.h"
#include "Libs/MX_Iridium9603.h"
#include "Strings.h"
#include "Libs/M_CountList.h"
#include "Libs/ErrorCorrection.h"
#include "Libs/cc110LTester.h"
#include "Libs/WakeupSource.h"

//Defines:
#define BBAK_REASON_NONE				0
#define BBAK_REASON_SOFT_OFF			1
#define BBAK_REASON_LO_BAT				2
#define BBAK_REASON_NO_BAT				3

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

#define SYS_MSG_UPDATE_TIME_SRC_GSM		0x01
#define SYS_MSG_UPDATE_TIME_SRC_PC		0x02

#define USART_TIMEOUT					15000	//wait 15 seconds
#define TAGLISTUPDATE					30		//in seconds
#define RFCALUPDATE						5		//in seconds
#define BATVOLTCHECK					10		//in seconds
#define SEND_DATA_GPRS_RETRIES_MAX		3

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

#define FLASHRECORD_SIZE				11 //Changed to 11 12/5/2016	//size of a record in bytes
#define INTER_LOGGER_CC110L_INSTALLED 1
//Uncomment the define below to use the secondary 433 (inter-logger) chip as the main WID pickup chip.
//This will disable inter-logger comms.
//#define _433_PICKUP_MODE
//Uncomment below if the logger is a dual 433MHZ Logger
//#define _DUAL_433_LOGGER



//Pins
#define SMPS_PWR_PORT				PORTE
#define SMPS_PWR_PIN				1
#define BATV_READ_EN_PORT			PORTB
#define BATV_READ_EN_PIN			0
#define PWR_GD_PORT					PORTB
#define	PWR_GD_PIN					2
#define MCU_STAT_PORT				PORTB
#define MCU_STAT_PIN				3
#define UNIT_PWR_SW_PORT			PORTD
#define UNIT_PWR_SW_PIN				0

//General Macros:
#define SAVESP()					SP_saved = (uint16_t)((SPH<<8)+SPL)
#define SAVESTATE()					do {SAVESP(); save_PC();} while (0)
#define LOGGINGISENABLED			(setting_LoggingEnabled > 1)
#define PWR_GOOD					(PWR_GD_PORT.IN&(1<<PWR_GD_PIN))
#define UNIT_PWR					(UNIT_PWR_SW_PORT.IN&(1<<UNIT_PWR_SW_PIN))
#define PWR_GOOD_INT_EN()			do {PWR_GD_PORT.INTFLAGS |= PORT_INT0IF_bm; PWR_GD_PORT.INTCTRL &= ~(PORT_INT0LVL_gm); PWR_GD_PORT.INTCTRL |= PORT_INT0LVL_MED_gc; if (!PWR_GOOD) {EnterBatBakMode(BBAK_REASON_NO_BAT);}} while (0)	//have to clear the bits first
#define PWR_GOOD_INT_DIS()			do {PWR_GD_PORT.INTCTRL &= ~(PORT_INT0LVL_gm);} while (0)
#define SAVE_SYS_MSG(CMD, DATA)		RecordToFlash(&RTC_c_Time, CMD + (DATA), 0x02, 0x00, 0x00)

#define ByteNo(SrcPtr, byteNo)		*(((uint8_t*)SrcPtr)+byteNo)

//Pin Macros:
#define PIN_SET(PinName)			PinName##_PORT.OUTSET = (1<<PinName##_PIN)
#define PIN_CLR(PinName)			PinName##_PORT.OUTCLR = (1<<PinName##_PIN)
#define SMPS_ON()					PIN_CLR(SMPS_PWR)//SMPS_PWR_PORT.OUTCLR = (1<<SMPS_PWR_PIN)
#define SMPS_OFF()					PIN_SET(SMPS_PWR)//SMPS_PWR_PORT.OUTSET = (1<<SMPS_PWR_PIN)
#define BATV_READ_ON()				PIN_SET(BATV_READ_EN)//BATV_READ_EN_PORT.OUTSET = (1<<BATV_READ_EN_PIN)
#define BATV_READ_OFF()				PIN_CLR(BATV_READ_EN)//BATV_READ_EN_PORT.OUTCLR = (1<<BATV_READ_EN_PIN)
#define MCU_STAT_LED_ON()			PIN_CLR(MCU_STAT)//MCU_STAT_PORT.OUTCLR = (1<<MCU_STAT_PIN)
#define MCU_STAT_LED_OFF()			PIN_SET(MCU_STAT)//MCU_STAT_PORT.OUTSET = (1<<MCU_STAT_PIN)

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
uint8_t saveRecord(RTC_Time_t* mTime, uint32_t ID, uint8_t RSSI, uint8_t Activity);
uint8_t loadRecord(uint32_t, RTC_Time_t*, uint32_t*, uint8_t*, uint8_t*);
uint8_t RecordToFlash(RTC_Time_t* mTime, uint32_t ID, uint8_t Flags, uint8_t RSSI, uint8_t Activity);
void save_PC( void );
uint8_t ReadCalibrationByte( uint8_t index );
void FilterStr2Mask(char* filtStr);
void SetClkSpeed(uint8_t speed);
uint32_t FindRecordCount();
void ReelInTheYears();

inline void WriteTimeEEPROM(RTC_Time_t* mTime, uint16_t Address) {
	WriteEEPROM((uint8_t*)mTime, 6, Address);
}
inline void ReadTimeEEPROM(RTC_Time_t* mTime, uint16_t Address) {
	ReadEEPROM((uint8_t*)mTime, 6, Address);
}
void SendEmailChecks();

void clearWakeupSource(uint8_t src);
#endif /* WIDLOG_H_ */
