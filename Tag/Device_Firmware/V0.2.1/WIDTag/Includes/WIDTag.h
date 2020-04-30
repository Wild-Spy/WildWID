/*
 * WIDTag.h
 *
 *  Created on: 05/09/2010
 *      Author: Matthew Cochrane
 */

#ifndef WIDTAG_H_
#define WIDTAG_H_

//Includes
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <avr/io.h>
#include <avr/eeprom.h>
#include <avr/interrupt.h>
#include <avr/sleep.h>
#include <avr/wdt.h>
#include <util/delay.h>
#include "M_PRTC.h"
#include "M_USART.h"
#include "M_NRF24L01.h"

//Define
#define CPU8MHZ 1
//#define CPU1MHZ 1
#ifdef CPU8MHZ
	#define UBRRVAL 		0x0067 	//for 8MHz (Double Speed)
	#define TIMEOUT			253		//aprox 8 seconds
	#define NORWAITTIMPRE 	(1<<CS02)
	#define NORWAITTIMOCA 	0x4E
	#define T0WAIT10MSOCA	0x4D
#endif
#ifdef CPU1MHZ
	#define UBRRVAL 		0x000C //for 1MHz (Double Speed)
	#define TIMEOUT			32	//aprox 8 seconds
	#define NORWAITTIMPRE 	(1<<CS01)|(1<<CS00)
	#define NORWAITTIMOCA 	0x26
	#define T0WAIT10MSOCA	0x13
#endif

#define WAKEUP_SOURCE_NONE 0
#define WAKEUP_SOURCE_UART 1
#define WAKEUP_SOURCE_RTC 2

//Addresses V0.2 and after:
#define DEVIDADD	0x10	//a 4-byte integer (uint32_t)
#define DEVARMEDADD	0x17	//a 1-byte integer (uint8_t)
#define RFIDADD		0x30	//a 5-byte unsigned integer	(uint8_t[5])
#define RFCHANADD	0x36	//a 1-byte unsigned integer	(uint8_t)
#define DTIMEADD	0x52	//a 5-byte compressed structure (zipTime(struct RTCTime))
#define TXPERADD	0x58	//a 1-byte unsigned integer	(uint8_t)

/*OLD addresses (pre V0.2):
#define DEVIDADD	0x00	//a 4-byte integer (uint32_t)
#define DEVARMEDADD	0x07	//a 1-byte integer (uint8_t)
#define RFIDADD		0x20	//a 5-byte unsigned integer	(uint8_t[5])
#define RFCHANADD	0x25	//a 1-byte unsigned integer	(uint8_t)
#define DTIMEADD	0x42	//a 5-byte compressed structure (zipTime(struct RTCTime))
#define TXPERADD	0x47	//a 1-byte unsigned integer	(uint8_t)*/

#define ARM_OFF 	0
#define ARM_ON		1
#define ARM_DEBUG	2
#define ARM_SHELF	3

//Macros
#define ByteNO(SrcPtr, byteNo)	*(((uint8_t*)SrcPtr)+byteNo)

//Function Prototypes
void ShowMenu(void);
uint16_t GetBatVoltage(void);
void ADC_Setup(void);
uint16_t ADC_Read();
void ADC_Stop();
static inline void delay_ms(uint16_t);
//inline void WriteTimeEEPROM(struct RTCTime*, uint16_t);
//inline void ReadTimeEEPROM(struct RTCTime*, uint16_t);
void ReadEEPROM(uint8_t*, uint8_t, uint16_t);
void WriteEEPROM(uint8_t*, uint8_t, uint16_t);
void StartTim10ms(uint16_t);
void StopTim10ms(void);
void HardwareSetup(void);
void ReadAllVarsFromEEPROM(void);
void DevArmSetup();
void TXPayload(void);
void CPUSleep_1m5s(void);

#endif /* WIDTAG_H_ */
