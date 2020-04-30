/*
 * NRF24L01PController.c
 *
 *  Created on: 12/10/2011
 *      Author: MC
 */

#include "NRF24L01PController.h"
#include "HW_IO.h"
#include "common.h"
#include <util/delay.h>

static const int NRF24L01P_TXTIMEOUTMS = 4;		//from datasheet section 6.1.5 pg 23

//Defines:
enum {
	REGBIT_CONFIG_PRIM_RX_BM = 0b00000001,
	REGBIT_CONFIG_PWR_UP_BM = 0b00000010,
	REGBIT_STATUS_TX_DS_BM = 0b00100000,
	REGBIT_FIFOSTATUS_RX_EMPTY_BM = 0b00000001,
};

//Macros:
#define SETBITS(data, mask)		data |= (mask)
#define CLRBITS(data, mask)		data &= ~(mask)
#define GETBYTE(data, byteNo)	*(((uint8_t*)data)+byteNo)

//Variables:
static uint8_t NRF24L01PMode = 0xFF;
static void (*NRF24L01PStartmsTimer)(void) = NULL;
static uint16_t (*NRF24L01PgetmsCounterTicks)(void) = NULL;
static void (*NRF24L01PStopmsTimer)(void) = NULL;

//Helper functions:
static inline BOOL InvalidRegisterAddress(NRF24L01P_RegAdd_t address) {
	return (address&0b11100000);
}

static inline uint8_t SPIGenerateReadRegCmdByte(NRF24L01P_RegAdd_t address) {
	return address;// & 0b00011111; //first 3 bits of address should already be zero!
}

static inline uint8_t SPIGenerateWriteRegCmdByte(NRF24L01P_RegAdd_t address) {
	return address | 0b00100000;
}

static inline BOOL InvalidMode(NRF24L01P_Mode_t mode) {
	return ((mode < MODE_POWERDOWN) || (mode > MODE_RX));
}

void NRF24L01P_Create(void(*StartTimerFuncPtr)(), void(*StopTimerFuncPtr)(), uint16_t(*getCounterTickFuncPtr)()) {

	NRF24L01PStartmsTimer = StartTimerFuncPtr;
	NRF24L01PgetmsCounterTicks = getCounterTickFuncPtr;
	NRF24L01PStopmsTimer = StopTimerFuncPtr;
	IO_NRF_Setup_Pins();
	IO_NRF_Setup_SPI();
	//Doesn't necessarily agree with hardware
	NRF24L01PMode = MODE_POWERDOWN;
	//NRF24L01P_SetMode(MODE_POWERDOWN);
}

void NRF24L01P_Destroy() {
	NRF24L01PMode = 0xFF;
}

void NRF24L01P_SetupInterface(uint8_t chan, uint8_t* id) {
	// PTX, CRC enabled, mask a couple of ints
	NRF24L01P_WriteRegister(NREG_CONFIG, 0b00111001);
	//Disable auto retransmit
	NRF24L01P_WriteRegister(NREG_SETUP_RETR, 0b00000000);
	//Address Width = 5
	NRF24L01P_WriteRegister(NREG_SETUP_AW, 0b00000111);
	//Set RF Channel
	chan &= ~0x80;
	NRF24L01P_WriteRegister(NREG_RF_CH, chan);
	/*RF output power: NREG_RF_SETUP 0b000000XX ->
		00 = -18dBm (7.0mA in TX mode)
		01 = -12dBm	(7.5mA in TX mode)
		10 = -6dBm	(9.0mA in TX mode)
		00 = 0dBm	(11.3mA in TX mode)
	*/
	//Setup RF -> 2Mbps data rate, 0dBm output power
	//NRF24L01P_WriteRegister(NREG_RF_SETUP, 0b00000111);
	//Setup RF -> 1Mbps data rate, 0dBm output power
	//NRF24L01P_WriteRegister(NREG_RF_SETUP, 0b00000011);
	//Setup RF -> 250kbps data rate, 0dBm output power
	NRF24L01P_WriteRegister(NREG_RF_SETUP, 0b00100111);
	//Set the address of RX Pipe 0 to id
	NRF24L01P_WriteRegisterLong(NREG_RX_ADDR_P0, id, 5);
	//Set TX Address to id
	NRF24L01P_WriteRegisterLong(NREG_TX_ADDR, id, 5);
	//Disable auto-ACK
	NRF24L01P_WriteRegister(NREG_EN_AA, 0b00000000);
	//Enable only pipe 0
	NRF24L01P_WriteRegister(NREG_EN_RXADDR, 0b00000001);
	//Enable Pipe 0 and use payload width of 4
	NRF24L01P_WriteRegister(NREG_RX_PW_P0, 4);
	//Clear flags in status:
	NRF24L01P_WriteRegister(NREG_STATUS, 0b01110000);
}

NRF24L01P_Mode_t NRF24L01P_GetMode_Impl(void) {
	return NRF24L01PMode;
}

NRF24L01P_Mode_t (*NRF24L01P_GetMode)(void) = NRF24L01P_GetMode_Impl;

NRF24L01P_RW_Reg_RetVal_t NRF24L01P_ReadRegister_Impl(NRF24L01P_RegAdd_t address, uint8_t* data) {
	//R_REGISTER
	//Command =  000AAAAA;

	if (InvalidRegisterAddress(address)) {
		#ifdef NRF24L01P_USE_RUNTIME_ERRORS
		RUNTIME_ERROR("NRF24L01P_ReadRegister: Invalid Register Address", address);
		#endif
		return NRF24L01P_INVALID_INPUT;
	}
	uint8_t command = SPIGenerateReadRegCmdByte(address);

	//None of these functions can timeout because the SPI is in master mode...
	IO_NRF_CSN(PIN_LOW);
	IO_SPI_TX(command);
	*data = IO_SPI_RX();
	IO_NRF_CSN(PIN_HIGH);
	return NRF24L01P_SUCCESS;
}

NRF24L01P_RW_Reg_RetVal_t (*NRF24L01P_ReadRegister)(NRF24L01P_RegAdd_t address, uint8_t* data) = NRF24L01P_ReadRegister_Impl;

NRF24L01P_RW_Reg_RetVal_t NRF24L01P_WriteRegister_Impl(NRF24L01P_RegAdd_t address, uint8_t data) {
	//W_REGISTER - Executable in power down and standby modes only.
	//Command =  001AAAAA;

	return NRF24L01P_WriteRegisterLong(address, &data, 1);
}

NRF24L01P_RW_Reg_RetVal_t (*NRF24L01P_WriteRegister)(NRF24L01P_RegAdd_t address, uint8_t data) = NRF24L01P_WriteRegister_Impl;

int8_t NRF24L01P_SetMode_Impl(NRF24L01P_Mode_t mode) {
	uint8_t configReg;
	uint8_t pinCEVal;
	int8_t tmpErr = 0;

	if (InvalidMode(mode))
		return NRF24L01P_INVALID_INPUT;

	if ((tmpErr = NRF24L01P_ReadRegister(NREG_CONFIG, &configReg)) < 0) {
		NRF24L01PMode = MODE_UNKNOWN;
		return tmpErr;
	}

	switch (mode) {
	case MODE_RX:
		SETBITS(configReg, REGBIT_CONFIG_PWR_UP_BM|REGBIT_CONFIG_PRIM_RX_BM);
		pinCEVal = PIN_HIGH;
		break;
	case MODE_TX:
		SETBITS(configReg, REGBIT_CONFIG_PWR_UP_BM);
		CLRBITS(configReg, REGBIT_CONFIG_PRIM_RX_BM);
		pinCEVal = PIN_LOW;
		break;
	case MODE_POWERDOWN:
		CLRBITS(configReg, REGBIT_CONFIG_PWR_UP_BM);
		pinCEVal = PIN_LOW;
		break;
	default:
		break;
	}

	if ((tmpErr = NRF24L01P_WriteRegister(NREG_CONFIG, configReg)) < 0) {
		NRF24L01PMode = MODE_UNKNOWN;
		return tmpErr;
	}

	if (pinCEVal < PIN_UNCHANGED)
		IO_NRF_CE(pinCEVal);

	NRF24L01PMode = mode;

	return NRF24L01P_SUCCESS;
}

int8_t (*NRF24L01P_SetMode)(NRF24L01P_Mode_t mode) = NRF24L01P_SetMode_Impl;

void NRF24L01P_FlushTxFIFO() {
	//FLUSH_TX
	//Command =  11100001;
	IO_NRF_CSN(PIN_LOW);
	IO_SPI_TX(0b11100001);
	IO_NRF_CSN(PIN_HIGH);
}

void NRF24L01P_FlushRxFIFO() {
	//FLUSH_RX
	//Command =  11100010;
	//Should not be executed during transmission of ACK packet, that is, ACK
	//packet will not be completed.
	IO_NRF_CSN(PIN_LOW);
	IO_SPI_TX(0b11100010);
	IO_NRF_CSN(PIN_HIGH);
}

NRF24L01P_TxRx_RetVal_t NRF24L01P_TxPayload(uint32_t data) {
	//W_TX_PAYLOAD
	//Command =  10100000;
	uint8_t statReg = 0x00;
	uint16_t startTime;
	BOOL TimedOut = FALSE;
	uint8_t i;

	if (NRF24L01P_GetMode() != MODE_TX)
		return NRF_TxRx_ret_wrong_mode;

	IO_NRF_CSN(PIN_LOW);
	IO_SPI_TX(0b10100000);
	for (i = 0; i < 4; i++)
		IO_SPI_TX(GETBYTE(&data, i));
	IO_NRF_CSN(PIN_HIGH);

	IO_NRF_CE(PIN_HIGH);
	_delay_us(15);
	IO_NRF_CE(PIN_LOW);
	_delay_us(12);
	
	CPUSleep_Time(CPUSLEEP_0m5s);
	
	NRF24L01PStartmsTimer();
	startTime = NRF24L01PgetmsCounterTicks();

	do {
		NRF24L01P_ReadRegister(NREG_STATUS, &statReg);
		TimedOut = (NRF24L01PgetmsCounterTicks() - startTime) >= NRF24L01P_TXTIMEOUTMS;
	} while (!(statReg & REGBIT_STATUS_TX_DS_BM) && (!TimedOut));
	
	NRF24L01PStopmsTimer();
	
	if (TimedOut) return NRF_TxRx_ret_timeout;

	NRF24L01P_WriteRegister(NREG_STATUS, 0b01110000);

	return NRF_TxRx_ret_success;
}

NRF24L01P_TxRx_RetVal_t NRF24L01P_RxPayload(uint32_t* data) {
	uint8_t FIFOStat = 0xFF;
	uint8_t i;

	if (NRF24L01P_GetMode() != MODE_RX)
		return NRF_TxRx_ret_wrong_mode;

	NRF24L01P_ReadRegister(NREG_FIFOSTATUS, &FIFOStat);
	if (FIFOStat & REGBIT_FIFOSTATUS_RX_EMPTY_BM)
		return NRF_TxRx_ret_rx_fifo_empty;
	IO_NRF_CSN(PIN_LOW);
	IO_SPI_TX(0b01100001);
	for (i = 0; i < 4; i++) {
		GETBYTE(data, i) = IO_SPI_RX();
	}
	IO_NRF_CSN(PIN_HIGH);

	return NRF_TxRx_ret_success;
}

NRF24L01P_RW_Reg_RetVal_t NRF24L01P_WriteRegisterLong_Impl(NRF24L01P_RegAdd_t address, uint8_t* data, uint8_t length) {
	//W_REGISTER - Executable in power down and standby modes only.
	//Command =  001AAAAA;

	if (InvalidRegisterAddress(address)) {
		#ifdef NRF24L01P_USE_RUNTIME_ERRORS
		RUNTIME_ERROR("NRF24L01P_WriteRegister: Invalid Register Address", address);
		#endif
		return NRF24L01P_INVALID_INPUT;
	}

	uint8_t command = SPIGenerateWriteRegCmdByte(address);

	IO_NRF_CSN(PIN_LOW);
	IO_SPI_TX(command);
	while (length--) {
		IO_SPI_TX(*data++);
	}
	IO_NRF_CSN(PIN_HIGH);
	return NRF24L01P_SUCCESS;

}

NRF24L01P_RW_Reg_RetVal_t (*NRF24L01P_WriteRegisterLong)(NRF24L01P_RegAdd_t address, uint8_t* data, uint8_t length) = NRF24L01P_WriteRegisterLong_Impl;
