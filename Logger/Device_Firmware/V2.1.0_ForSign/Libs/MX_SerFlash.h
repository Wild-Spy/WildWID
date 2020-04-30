/*
 * MX_SerFlash.h
 *
 *  Created on: 24/08/2010
 *      Author: MC
 */

#ifndef MX_SerFlash_H_
#define MX_SerFlash_H_

#include <stdlib.h>
#include <avr/io.h>

#define SF_VER_ATMEL		1
#define SF_VER_GENERIC		2

#define SF_VER				SF_VER_GENERIC



//Properties:
#if SF_VER == SF_VER_ATMEL
//size of device in bytes, ie MAXADDRESS+1
#define SerFlash_SIZE			0x400000L
#define SerFlash_MAXADDRESS		SerFlash_SIZE-1
#define SerFlash_SECTORSIZE		0x10000L
#define SerFlash_PAGESIZE		0x100
//#define SerFlash_FAST_RD		1
#elif SF_VER == SF_VER_GENERIC
//size of device in bytes, ie MAXADDRESS+1
#define SerFlash_SIZE			0x800000L //0x200000L
#define SerFlash_MAXADDRESS		SerFlash_SIZE-1
#define SerFlash_SECTORSIZE		0x10000L
#define SerFlash_PAGESIZE		0x100
//#define SerFlash_FAST_RD		1
#endif

//Devices:
#define SerFlash_PORT	PORTD
#define SerFlash_SPI	SPID

//Pins:
//#define SerFlash_WPN	0
//#define SerFlash_HOLDN	3
#define SerFlash_CSN 	1
#define SerFlash_MISO 	6
#define SerFlash_MOSI 	5
#define SerFlash_SCK 	7

//Opcodes:
//		MACRO						OPCODE  FREQ	Add Bytes	Dummy Bytes		Data Bytes
#define SerFlash_OP_WrtEn		0x06  //75Mhz	0			0				0
#define SerFlash_OP_WrtDis		0x04  //75Mhz	0			0				0
#define SerFlash_OP_RdManDevID	0x9F  //75Mhz	0			0				1-20
#define SerFlash_OP_RdStatus	0x05  //33Mhz	0			0				1+
#define SerFlash_OP_WrtStatus	0x01  //75Mhz	0			0				1
#define SerFlash_OP_ReadAryS	0x03  //33Mhz	3			0				1+
#define SerFlash_OP_ReadAryF	0x0B  //75Mhz	3			1				1+
#define SerFlash_OP_BPPrgm		0x02  //75Mhz	3			0				1+
#define SerFlash_OP_BlkErs64k	0xD8  //75Mhz	3			0				0
#define SerFlash_OP_ChpErs		0xC7  //75Mhz	0			0				0
#define SerFlash_OP_DPwrDwn		0xB9  //75Mhz	0			0				0
#define SerFlash_OP_ResDPwrDwn	0xAB  //75Mhz	0			0				0
#if SF_VER == SF_VER_ATMEL
#define SerFlash_OP_ChpErs1		0x60  //66Mhz	0			0				0
#define SerFlash_OP_BlkErs4k	0x20  //66Mhz	3			0				0
#define SerFlash_OP_BlkErs32k	0x52  //66Mhz	3			0				0
#define SerFlash_OP_ProtSect	0x36  //66Mhz	3			0				0
#define SerFlash_OP_UnProtSect	0x39  //66Mhz	3			0				0
//global protect/unprotect
#define SerFlash_OP_RdProtReg	0x3C  //66Mhz	3			0				1+
#define SerFlash_OP_ProgOTP		0x9B  //66Mhz	3			0				1+
#define SerFlash_OP_RdOTP		0x77  //66Mhz	3			2				1+
#endif

#if SF_VER == SF_VER_GENERIC
//Protection:
#define SerFlash_PROT_MASK		0b00011100
#define SerFlash_PROT_NONE		0b00000000
#define SerFlash_PROT_U32		0b00000100
#define SerFlash_PROT_U16		0b00001000
#define SerFlash_PROT_U8		0b00001100
#define SerFlash_PROT_U4		0b00010000
#define SerFlash_PROT_U2		0b00010100
#define SerFlash_PROT_ALL		0b00011100
#endif


//Macros:
#define SerFlash_WaitRDY()	if (SerFlash_StPollStatusReg()&0x01){while(SerFlash_PollStatusReg()&0x01);}SerFlash_FinPollStatusReg()

//Function Prototypes:
void SerFlash_ReadBytes(uint32_t Address, uint16_t Length, uint8_t* Data);
void SerFlash_WriteBytes(uint32_t Address, uint16_t Length, uint8_t* Data);
void SerFlash_BlockErase(uint32_t Address, uint8_t Size);
void SerFlash_ChipErase(uint8_t Saftey);
void SerFlash_WriteEn();
void SerFlash_WriteDis();
void SerFlash_DeepPwrDwn(uint8_t Value);
uint8_t SerFlash_RdSectProt(uint32_t Address);
uint8_t SerFlash_RdStatusReg();
void SerFlash_SPI_TX(uint8_t Data);
uint8_t SerFlash_SPI_RX();
void SerFlash_SPI_Setup();
#if SF_VER == SF_VER_ATMEL
void SerFlash_ProtSect(uint32_t Address, uint8_t Value);
#elif SF_VER == SF_VER_GENERIC
void SerFlash_SetProt(uint8_t protVal);
#endif

uint8_t inline SerFlash_StPollStatusReg() {
	SerFlash_PORT.OUTCLR = (1<< SerFlash_CSN);
	SerFlash_SPI_TX(SerFlash_OP_RdStatus);
	return SerFlash_SPI_RX();
}

uint8_t inline SerFlash_PollStatusReg() {
	return SerFlash_SPI_RX();
}

void inline SerFlash_FinPollStatusReg() {
	SerFlash_PORT.OUTSET = (1<<SerFlash_CSN);
}

#endif /* MX_SerFlash_H_ */
