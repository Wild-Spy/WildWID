/*
 * xmeg1.h
 *
 *  Created on: 06/08/2010
 *      Author: MC
 */

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
#include "Libs/NHDC160100DiZ.h"
#include "Libs/KeyPad.h"
#include "Libs/GUI.h"
#include "Version.h"

//Defines:
#define WAKEUP_SOURCE_NONE		0
#define WAKEUP_SOURCE_RTC		0b00000001
#define WAKEUP_SOURCE_USART		0b00000010
#define WAKEUP_SOURCE_NORDIC	0b00000100
#define WAKEUP_SOURCE_PWR		0b00001000
#define WAKEUP_SOURCE_KP		0b00010000

#define BBAK_REASON_NONE		0
#define BBAK_REASON_SW_OFF		1
#define BBAK_REASON_LO_BAT		2
#define BBAK_REASON_NO_BAT		3


#define USART_TIMEOUT		15000	//wait 15 seconds
#define TAGLISTUPDATE		20		//in seconds
#define BATVOLTCHECK		10		//in seconds

#define RECCOUNT_ADD		0x00	//a 4 byte unsigned integer (uint32_t)
#define RFIDADD				0x04	//a 5-byte unsigned integer	(uint8_t[5])
#define RFCHANADD			0x09	//a 1-byte unsigned integer	(uint8_t)
#define TIMESAVEADD			0x0A	//a 6-byte structure (struct RTCTime)
#define ADCOFFSETV			0x10	//a 2-byte unsigned integer (uint16_t)

#define RECORD_SIZE			9	//size of a record in bytes

//Macros:
#define SAVESP()		SP_saved = (uint16_t)((SPH<<8)+SPL)
#define SAVESTATE()		SAVESP(); save_PC()

#define ByteNO(SrcPtr, byteNo)	*(((uint8_t*)SrcPtr)+byteNo)

//Pins
#define SMPS_PWR_PIN	1
#define BATV_READ_PIN	0
#define	PWR_GD_PIN		2

//Pins
#define SMPS_ON()		PORTE.OUTCLR = (1<<SMPS_PWR_PIN)
#define SMPS_OFF()		PORTE.OUTSET = (1<<SMPS_PWR_PIN)
#define BATV_READ_ON()	PORTE.OUTSET = (1<<BATV_READ_PIN)
#define BATV_READ_OFF()	PORTE.OUTCLR = (1<<BATV_READ_PIN)

#define RCTOEEPROM()	WriteEEPROM((uint8_t*)&RecordCount,4,RECCOUNT_ADD);
#define RCFROMEEPROM()	ReadEEPROM((uint8_t*)&RecordCount,4, RECCOUNT_ADD);


//Function Prototypes:
void SetupHardware( void );
void bBakStartup( void );
void EnterBatBakMode(uint8_t Reason);
void WakeFromBatBakMode( void );
float CheckBatteryVoltage( void );
void CalibrateADCOffset( void );
int ADCRead(uint8_t SAMP);
void RTCWakeChecks( void );
void ShowMenu( void );
void doRFWork( uint8_t );
void UpdateTagList( void );
uint8_t saveRecord(struct RTCTime*, uint32_t*);
uint8_t loadRecord(uint32_t, struct RTCTime*, uint32_t*);
uint8_t RecordToFlash(struct RTCTime* mTime, uint32_t* ID, uint8_t Flags);
inline void WriteTimeEEPROM(struct RTCTime*, uint16_t);
inline void ReadTimeEEPROM(struct RTCTime*, uint16_t);
void save_PC( void );
uint8_t ReadCalibrationByte( uint8_t index );

#endif /* WIDLOG_H_ */
