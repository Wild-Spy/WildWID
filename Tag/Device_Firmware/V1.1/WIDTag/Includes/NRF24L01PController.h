/*
 * NRF24L01PController.h
 *
 *  Created on: 12/10/2011
 *      Author: MC
 */

#ifndef NRF24L01PCONTROLLER_H_
#define NRF24L01PCONTROLLER_H_

#include <stdint.h>
#include "RuntimeError.h"

//Enums:
enum {
	NRF24L01P_SUCCESS 			=  0,
	NRF24L01P_INVALID_INPUT 	= -1,
	NRF24L01P_TIMEOUT			= -2,
	NRF24L01P_WRONG_STATE		= -3,
	NRF24L01P_DATA_UNAVAILABLE	= -4,
};

typedef enum NRF24L01P_ReadWriteRegRetVal {
	NRF_rw_ret_success			= NRF24L01P_SUCCESS,
	NRF_rw_ret_invalid_input	= NRF24L01P_INVALID_INPUT,
} NRF24L01P_RW_Reg_RetVal_t;

typedef enum NRF24L01P_TxRxPayloadRetVal {
	NRF_TxRx_ret_success			= NRF24L01P_SUCCESS,
	NRF_TxRx_ret_invalid_input		= NRF24L01P_INVALID_INPUT,
	NRF_TxRx_ret_timeout			= NRF24L01P_TIMEOUT,
	NRF_TxRx_ret_wrong_mode			= NRF24L01P_WRONG_STATE,
	NRF_TxRx_ret_rx_fifo_empty		= NRF24L01P_DATA_UNAVAILABLE,
} NRF24L01P_TxRx_RetVal_t;

typedef enum NRF24L01P_RegAdd {
	NREG_CONFIG		= 0x00,
	NREG_EN_AA		= 0x01,
	NREG_EN_RXADDR	= 0x02,
	NREG_SETUP_AW	= 0x03,
	NREG_SETUP_RETR	= 0x04,
	NREG_RF_CH		= 0x05,
	NREG_RF_SETUP	= 0x06,
	NREG_STATUS		= 0x07,
	NREG_OBSERVE_TX	= 0x08,
	NREG_RPD		= 0x09,
	NREG_RX_ADDR_P0	= 0x0A,	//5 bytes
	NREG_RX_ADDR_P1	= 0x0B,	//5 bytes
	NREG_RX_ADDR_P2	= 0x0C,
	NREG_RX_ADDR_P3	= 0x0D,
	NREG_RX_ADDR_P4	= 0x0E,
	NREG_RX_ADDR_P5	= 0x0F,
	NREG_TX_ADDR	= 0x10,	//5 bytes
	NREG_RX_PW_P0	= 0x11,
	NREG_RX_PW_P1	= 0x12,
	NREG_RX_PW_P2	= 0x13,
	NREG_RX_PW_P3	= 0x14,
	NREG_RX_PW_P4	= 0x15,
	NREG_RX_PW_P5	= 0x16,
	NREG_FIFOSTATUS = 0x17,
	NREG_DYNPD		= 0x1C,
} NRF24L01P_RegAdd_t;

typedef enum NRF24L01P_Mode {
	MODE_POWERDOWN,
	MODE_TX,
	MODE_RX,
	MODE_UNKNOWN,
} NRF24L01P_Mode_t;

//Defines:
//#define NRF24L01P_USE_RUNTIME_ERRORS	1

//Function Prototypes:
void NRF24L01P_Create(void(*StartTimerFuncPtr)(), void(*StopTimerFuncPtr)(), uint16_t(*getCounterTickFuncPtr)());
void NRF24L01P_Destroy(void);
extern NRF24L01P_Mode_t (*NRF24L01P_GetMode)(void);
extern NRF24L01P_RW_Reg_RetVal_t (*NRF24L01P_ReadRegister)(NRF24L01P_RegAdd_t address, uint8_t* data);
extern NRF24L01P_RW_Reg_RetVal_t (*NRF24L01P_WriteRegister)(NRF24L01P_RegAdd_t address, uint8_t data);
extern int8_t (*NRF24L01P_SetMode)(NRF24L01P_Mode_t mode);
void NRF24L01P_FlushTxFIFO(void);
void NRF24L01P_FlushRxFIFO(void);
NRF24L01P_TxRx_RetVal_t NRF24L01P_TxPayload(uint32_t data);
NRF24L01P_TxRx_RetVal_t NRF24L01P_RxPayload(uint32_t* data);
extern NRF24L01P_RW_Reg_RetVal_t (*NRF24L01P_WriteRegisterLong)(NRF24L01P_RegAdd_t address, uint8_t* data, uint8_t length);
void NRF24L01P_SetupInterface(uint8_t chan, uint8_t* id);

#define CPUSLEEP_0m5s	0x0F	//0.4883ms with prescalar of 1
void CPUSleep_Time(uint8_t OCR2A_set);

#endif /* NRF24L01PCONTROLLER_H_ */
