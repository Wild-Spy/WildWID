/*
 * MX_SerComms.h
 *
 * Created: 17/08/2011 1:58:10 PM
 *  Author: MC
 */ 


#ifndef MX_SERCOMMS_H_
#define MX_SERCOMMS_H_

#include <avr/io.h>
#include <avr/interrupt.h>
#include <stdio.h>
#include <math.h>
#include <avr/pgmspace.h>
#include <util/crc16.h>
#include "..\WIDLogV2.h"

//Must make sure that '-fshort-enums' flag is set (theres a check box under optimizations) and it
//will only take up 1 byte!
typedef enum _SerCom_err_t {
	SerCom_err_Success			=  0,
	SerCom_err_Timeout			= -1,
	SerCom_err_Abort			= -2,
	SerCom_err_Retry			= -3,
	SerCom_err_Nak				= -4,
	SerCom_err_Unknown			= -20,
	SerCom_err_NoKnownCmd		= -6,
} SerCom_err_t;

typedef SerCom_err_t (*CMD_Handler_t)(USART_t*);
typedef struct _Ser_CMD_Handler {
	uint8_t CMDByte;
	CMD_Handler_t Handler;
} Ser_CMD_Handler;

//Defines:
#define SERCOM_RXSEG_ID		0x01
#define SERCOM_TXSEG_ID		0x02
#define SERCOM_FINSEG_ID	0x03

#define SERCOM_RESP_OK		0x01
#define SERCOM_RESP_RESEND	0x02
#define SERCOM_RESP_ABORT	0x03

#define SERCOM_PC_ACK		0x01
#define SERCOM_PC_NAK		0x02

#define SERCOM_TIMEOUT		500 

//Macros:
#define ByteNo(SrcPtr, byteNo)	*(((uint8_t*)SrcPtr)+byteNo)

//Function Prototypes:
SerCom_err_t Ser_RxSeg(USART_t* m_USART, uint16_t len, uint8_t* DataDest);
SerCom_err_t Ser_TxSeg(USART_t* m_USART, uint16_t len, uint8_t* DataSrc);
SerCom_err_t SerCom_RxAck(USART_t* m_USART);
SerCom_err_t Ser_CMDHandler(USART_t* m_USART);
SerCom_err_t SerCom_FinishSeg(USART_t* m_USART);

#endif /* MX_SERCOMMS_H_ */