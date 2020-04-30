/*
 * HW_IO_ATMEGA88.c
 *
 * Created: 15/10/2011 10:47:55 PM
 *  Author: MC
 */ 

#include "HW_IO.h"
#include <avr/io.h>

void IO_SPI_TX(SPIData_t data) {
	SPDR = data;
	while(!(SPSR & (1<<SPIF)));
}

SPIData_t IO_SPI_RX(void) {
	SPDR = 0x00;
	while(!(SPSR & (1<<SPIF)));
	return SPDR;
}

void IO_NRF_CSN(pinState_t data) {
	if (data == PIN_HIGH)
		NRF_PORT |= (1<<NRF_CSN);
	else if (data == PIN_LOW)
		NRF_PORT &= ~(1<<NRF_CSN);
}

void IO_NRF_CE(pinState_t data) {
	if (data == PIN_HIGH)
		NRF_PORT |= (1<<NRF_CE);
	else if (data == PIN_LOW)
		NRF_PORT &= ~(1<<NRF_CE);
}

void IO_NRF_Setup_Pins(void) {
	//The Actual SPI as a master
	NRF_DDR |= (1<<NRF_CE);
	NRF_PORT &= ~((1<<NRF_CE));
}

void IO_NRF_Setup_SPI(void) {
	//The Actual SPI as a master
	NRF_DDR |= (1<<NRF_SCK)|(1<<NRF_MOSI)|(1<<NRF_CSN)|(1<<NRF_CE);
	NRF_DDR &= ~((1<<NRF_MISO));
	NRF_PORT |= (1<<NRF_CSN);//|(1<<2);
	NRF_PORT &= ~((1<<NRF_CE)|(1<<NRF_SCK)|(1<<NRF_MOSI)|(1<<NRF_MISO));
	// Enable SPI, Master, set clock rate fck/2
	SPCR = (1<<SPE)|(1<<MSTR);
	SPSR = (1<<SPI2X);
}