/*
 * M_NRF24L01.c
 *
 *  Created on: 09/03/2010
 *      Author: Matty
 */

#include "M_NRF24L01.h"

extern volatile uint8_t TX_Delay;

void NRF24L01_SetupnRF24L01(uint8_t RF_Chan, uint8_t* pipeAddress) {
	RF_Chan &= ~0x80;
	// PTX, CRC enabled, mask a couple of ints
	NRF24L01_W_Reg(NREG_CONFIG, 0b00111001);
	//Disable auto retransmit
	NRF24L01_W_Reg(NREG_SETUP_RETR, 0x00);
	//Address Width = 5
	NRF24L01_W_Reg(NREG_SETUP_AW, 0b00000111);
	//Set RF Channel
	NRF24L01_W_Reg(NREG_RF_CH, RF_Chan);
	/*RF output power: NREG_RF_SETUP 0b000000XX ->
		00 = -18dBm (7.0mA in TX mode)
		01 = -12dBm	(7.5mA in TX mode)
		10 = -6dBm	(9.0mA in TX mode)
		00 = 0dBm	(11.3mA in TX mode)
	*/
	//Setup RF -> 2Mbps data rate, 0dBm output power
	//NRF24L01_W_Reg(NREG_RF_SETUP, 0b00000111);
	//Setup RF -> 1Mbps data rate, 0dBm output power
	//NRF24L01_W_Reg(NREG_RF_SETUP, 0b00000011);
	//Setup RF -> 250kbps data rate, 0dBm output power
	NRF24L01_W_Reg(NREG_RF_SETUP, 0b00100111);
	//uint8_t myData[5];
	//for (uint8_t i = 0; i < 5; i++) myData[i] = 0xE6;
	//Set the address of RX Pipe 0 to pipeAddress
	NRF24L01_W_Reg_Long(NREG_RX_ADDR_P0, (uint8_t*)pipeAddress, 5);
	//Set TX Address to pipeAddress
	NRF24L01_W_Reg_Long(NREG_TX_ADDR, (uint8_t*)pipeAddress, 5);
	//Disable auto-ACK
	NRF24L01_W_Reg(NREG_EN_AA, 0x00);
	//Enable only pipe 0
	NRF24L01_W_Reg(NREG_EN_RXADDR, 0b00000001);
	//Enable Pipe 0 and use payload width of 4
	NRF24L01_W_Reg(NREG_RX_PW_P0, 4);
}

void NRF24L01_SetMode(uint8_t Power, uint8_t RXMode) {
	uint8_t ConfigReg;
	//Power up and put into RX mode
	ConfigReg = NRF24L01_R_Reg(NREG_CONFIG);

	if (Power)
		ConfigReg |= 0b00000010;
	else
		ConfigReg &= 0b11111101;

	if (RXMode) {
		ConfigReg |= 0b00000001;
		NRF24L01_W_Reg(NREG_CONFIG, ConfigReg);
		if (Power) {
			PORTB |= (1<<CE1);
		} else {
			PORTB &= ~(1<<CE1);
		}
	} else {
		ConfigReg &= 0b11111110;
		NRF24L01_W_Reg(NREG_CONFIG, ConfigReg);
		PORTB &= ~(1<<CE1);
	}
}

uint8_t NRF24L01_R_Reg(uint8_t Address) {
	//R_REGISTER
	//Command =  000AAAAA;
	Address &= 0b00011111;
	PORTB &= ~(1<<CSN1);
	NRF24L01_SPI_TX(Address);
	_delay_us(5);
	uint8_t rxval = NRF24L01_SPI_RX();
	PORTB |= (1<<CSN1);
	return rxval;
}

void NRF24L01_R_Reg_Long(uint8_t Address, uint8_t* Data, uint8_t Length) {
	//R_REGISTER
	//Command =  000AAAAA;
	Address &= 0b00011111;
	PORTB &= ~(1<<CSN1);
	NRF24L01_SPI_TX(Address);
	_delay_us(5);
	while (Length--) {
		//TODO[ ]: Test that this works:  Does Data get incremented before of after the assignment?
		*Data++ = NRF24L01_SPI_RX();
	}
	PORTB |= (1<<CSN1);
}

void NRF24L01_W_Reg(uint8_t Address, uint8_t Data) {
	//W_REGISTER - Executable in power down and standby modes only.
	//Command =  001AAAAA;
	Address &= 0b00011111;
	Address |= 0b00100000;
	PORTB &= ~(1<<CSN1);
	NRF24L01_SPI_TX(Address);
	_delay_us(5);
	NRF24L01_SPI_TX(Data);
	PORTB |= (1<<CSN1);
}

void NRF24L01_W_Reg_Long(uint8_t Address, uint8_t* Data, uint8_t Length) {
	//W_REGISTER - Executable in power down and standby modes only.
	//Command =  001AAAAA;
	Address &= 0b00011111;
	Address |= 0b00100000;
	PORTB &= ~(1<<CSN1);
	NRF24L01_SPI_TX(Address);
	_delay_us(5);
	while (Length--) {
		NRF24L01_SPI_TX(*Data++);
	}
	PORTB |= (1<<CSN1);
}

void NRF24L01_Flush_RX_FIFO() {
	//FLUSH_RX
	//Command =  11100010;
	PORTB &= ~(1<<CSN1);
	NRF24L01_SPI_TX(0b11100010);
	PORTB |= (1<<CSN1);
}

void NRF24L01_Flush_TX_FIFO() {
	//FLUSH_TX
	//Command =  11100001;
	PORTB &= ~(1<<CSN1);
	NRF24L01_SPI_TX(0b11100001);
	PORTB |= (1<<CSN1);
}

/*uint8_t NRF24L01_R_RX_PAYLOAD(uint32_t* rxData) {
	//R_RX_PAYLOAD
	//Command =  01100001;

	//if RX_EMPTY bit in FIFO_STATUS is 0 (ie. there is data in the FIFO)
	if (!(NRF24L01_R_Reg(NREG_FIFOSTATUS) & 0x01)) {

		//disable the receiver
		PORTB &= ~(1<<CE1);

		PORTB &= ~(1<<CSN1);
		NRF24L01_SPI_TX(0b01100001);
		*(((uint8_t*)rxData)+0) = NRF24L01_SPI_RX();
		*(((uint8_t*)rxData)+1) = NRF24L01_SPI_RX();
		*(((uint8_t*)rxData)+2) = NRF24L01_SPI_RX();
		*(((uint8_t*)rxData)+3) = NRF24L01_SPI_RX();
		PORTB |= (1<<CSN1);

		//NRF24L01_Flush_RX_FIFO();
		NRF24L01_W_Reg(NREG_STATUS, 0x40);

		//enable the receiver
		PORTB |= (1<<CE1);

		return 0;
	} else
		return 1;

}*/

uint8_t NRF24L01_TX_PAYLOAD(uint32_t* Data) {
	//W_TX_PAYLOAD
	//Command =  10100000;
	PORTB &= ~(1<<CSN1);
	NRF24L01_SPI_TX(0b10100000);
	//TODO[ ]: Test this works...
	NRF24L01_SPI_TX(*(((uint8_t*)Data)+0));
	NRF24L01_SPI_TX(*(((uint8_t*)Data)+1));
	NRF24L01_SPI_TX(*(((uint8_t*)Data)+2));
	NRF24L01_SPI_TX(*(((uint8_t*)Data)+3));
	PORTB |= (1<<CSN1);

	//pulse CE
	PORTB |= (1<<CE1);
	_delay_us(15);
	PORTB &= ~(1<<CE1);
	_delay_us(12);
	//wait for tx bit in status reg to be set then clear it.
	TX_Delay = 2;
	while((!(NRF24L01_R_Reg(NREG_STATUS) & 0b00100000)) && (TX_Delay));
	NRF24L01_W_Reg(NREG_STATUS,0x70);
	return TX_Delay;
}

void NRF24L01_SPI_TX(uint8_t Data) {
	SPDR = Data;
	while(!(SPSR & (1<<SPIF)));
}

uint8_t NRF24L01_SPI_RX() {
	SPDR = 0x00;
	while(!(SPSR & (1<<SPIF)));
	return SPDR;
}

void NRF24L01_SPI_Setup() {

	//The Actual SPI as a master
	// Set MOSI, SCK and !SS (2) and CE (1) as outputs, all others as inputs
	//(o)CSN1=7, (o)CE1=6, (o)SCK=5, (i)MISO=4, (o)MOSI=3, (o)CSN0=2, (o)CE0=1
	DDRB = (1<<CSN1)|(1<<CE1)|(1<<NRF_SCK)|(1<<NRF_MOSI)|(1<<2)|(1<<1);
	PORTB |= (1<<CSN1);//|(1<<2);
	// Enable SPI, Master, set clock rate fck/2
	SPCR = (1<<SPE)|(1<<MSTR);
	SPSR = (1<<SPI2X);

}
