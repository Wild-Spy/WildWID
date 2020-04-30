/*
 * M_NRF24L01.c
 *
 *  Created on: 09/03/2010
 *      Author: Matty
 */

#include "MX_NRF24L01.h"

extern char s[80];
SPI_t* my_SPI;

void NRF24L01_SetupnRF24L01(uint8_t RF_Chan, uint8_t* pipeAddress) {
	RF_Chan &= ~0x80;
	// PTX, CRC enabled, mask a couple of ints
	NRF24L01_W_Reg(NREG_CONFIG, 0b00111001);
	//Disable auto retransmit
	NRF24L01_W_Reg(NREG_SETUP_RETR, 0x00);
	//Address Width = 5
	NRF24L01_W_Reg(NREG_SETUP_AW, 0b00000011);
	//Set RF Channel
	NRF24L01_W_Reg(NREG_RF_CH, RF_Chan);
	//Setup RF -> 1Mbps data rate, 0dBm output power
	//NRF24L01_W_Reg(NREG_RF_SETUP, 0b00000111);
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
			NRF_PORT.OUTSET = (1<<NRF_CE);
		} else {
			NRF_PORT.OUTCLR = (1<<NRF_CE);
		}
	} else {
		ConfigReg &= 0b11111110;
		NRF24L01_W_Reg(NREG_CONFIG, ConfigReg);
		NRF_PORT.OUTCLR = (1<<NRF_CE);
	}
}

uint8_t NRF24L01_R_Reg(uint8_t Address) {
	//R_REGISTER
	//Command =  000AAAAA;
	Address &= 0b00011111;
	NRF_PORT.OUTCLR = (1<<NRF_CSN);
	NRF24L01_SPI_TX(Address);
	_delay_us(5);
	uint8_t rxval = NRF24L01_SPI_RX(my_SPI);
	NRF_PORT.OUTSET = (1<<NRF_CSN);
	return rxval;
}

void NRF24L01_R_Reg_Long(uint8_t Address, uint8_t* Data, uint8_t Length) {
	//R_REGISTER
	//Command =  000AAAAA;
	Address &= 0b00011111;
	NRF_PORT.OUTCLR = (1<<NRF_CSN);
	NRF24L01_SPI_TX(Address);
	_delay_us(5);
	while (Length--) {
		//TODO[ ]: Test that this works:  Does Data get incremented before of after the assignment?
		*Data++ = NRF24L01_SPI_RX(my_SPI);
	}
	NRF_PORT.OUTSET = (1<<NRF_CSN);
}

void NRF24L01_W_Reg(uint8_t Address, uint8_t Data) {
	//W_REGISTER - Executable in power down and standby modes only.
	//Command =  001AAAAA;
	Address &= 0b00011111;
	Address |= 0b00100000;
	NRF_PORT.OUTCLR = (1<<NRF_CSN);
	NRF24L01_SPI_TX(Address);
	_delay_us(5);
	NRF24L01_SPI_TX(Data);
	NRF_PORT.OUTSET = (1<<NRF_CSN);
}

void NRF24L01_W_Reg_Long(uint8_t Address, uint8_t* Data, uint8_t Length) {
	//W_REGISTER - Executable in power down and standby modes only.
	//Command =  001AAAAA;
	Address &= 0b00011111;
	Address |= 0b00100000;
	NRF_PORT.OUTCLR = (1<<NRF_CSN);
	NRF24L01_SPI_TX(Address);
	_delay_us(5);
	while (Length--) {
		NRF24L01_SPI_TX(*Data++);
	}
	NRF_PORT.OUTSET = (1<<NRF_CSN);
}

void NRF24L01_Flush_RX_FIFO() {
	//FLUSH_RX
	//Command =  11100010;
	NRF_PORT.OUTCLR = (1<<NRF_CSN);
	NRF24L01_SPI_TX(0b11100010);
	NRF_PORT.OUTSET = (1<<NRF_CSN);
}

void NRF24L01_Flush_TX_FIFO() {
	//FLUSH_TX
	//Command =  11100001;
	NRF_PORT.OUTCLR = (1<<NRF_CSN);
	NRF24L01_SPI_TX(0b11100001);
	NRF_PORT.OUTSET = (1<<NRF_CSN);
}

uint8_t NRF24L01_R_RX_PAYLOAD(uint32_t* rxData) {
	//R_RX_PAYLOAD
	//Command =  01100001;

	if (NRF24L01_R_Reg(NREG_STATUS) & 0x40) {

		//disable the receiver
		//NRF_PORT.OUTCLR = (1<<NRF_CE);

		NRF_PORT.OUTCLR = (1<<NRF_CSN);
		NRF24L01_SPI_TX(0b01100001);
		*(((uint8_t*)rxData)+0) = NRF24L01_SPI_RX();
		*(((uint8_t*)rxData)+1) = NRF24L01_SPI_RX();
		*(((uint8_t*)rxData)+2) = NRF24L01_SPI_RX();
		*(((uint8_t*)rxData)+3) = NRF24L01_SPI_RX();
		NRF_PORT.OUTSET = (1<<NRF_CSN);

		//enable the receiver
		//NRF_PORT.OUTSET = (1<<NRF_CE);

		return 0;
	} else
		return 1;

}

void NRF24L01_TX_PAYLOAD(uint32_t* Data) {
	//W_TX_PAYLOAD
	//Command =  10100000;
	NRF_PORT.OUTCLR = (1<<NRF_CSN);
	NRF24L01_SPI_TX(0b10100000);
	//TODO[x]: Test this works...
	NRF24L01_SPI_TX(*(((uint8_t*)Data)+0));
	NRF24L01_SPI_TX(*(((uint8_t*)Data)+1));
	NRF24L01_SPI_TX(*(((uint8_t*)Data)+2));
	NRF24L01_SPI_TX(*(((uint8_t*)Data)+3));
	NRF_PORT.OUTSET = (1<<NRF_CSN);

	//pulse CE
	NRF_PORT.OUTSET = (1<<NRF_CE);
	_delay_us(12);
	NRF_PORT.OUTCLR = (1<<NRF_CE);
	//wait for tx bit in status reg to be set then clear it.
	while(!(NRF24L01_R_Reg(NREG_STATUS) & 0b00100000)) {
		/*sprintf_P(s, PSTR("Config: 0x%02X\r\n"), NRF24L01_R_Reg(NREG_CONFIG));
		USART_tx_String(s);
		sprintf_P(s, PSTR("Status: 0x%02X\r\n"), NRF24L01_R_Reg(NREG_STATUS));
		USART_tx_String(s);
		sprintf_P(s, PSTR("FIFO: 0x%02X\r\n"), NRF24L01_R_Reg(NREG_FIFOSTATUS));
		USART_tx_String(s);*/
	}

	//uint8_t i = 0;
	//while((NRF24L01_R_Reg(NREG_STATUS) & 0b00100000)) {
		NRF24L01_W_Reg(NREG_STATUS,0x70);
		//i++;
	//}
	//if (i > 0) {
	//	sprintf_P(s, PSTR("Took %u times to clear to 0x%02X!\r\n"), i, NRF24L01_R_Reg(NREG_STATUS));
	//	USART_tx_String(s);
	//}


}

void NRF24L01_TX_PAYLOAD1(uint32_t* Data) {
	//W_TX_PAYLOAD
	//Command =  10100000;
	NRF_PORT.OUTCLR = (1<<NRF_CSN);
	NRF24L01_SPI_TX(0b10100000);
	//TODO[x]: Test this works...
	NRF24L01_SPI_TX(*(((uint8_t*)Data)+0));
	NRF24L01_SPI_TX(*(((uint8_t*)Data)+1));
	NRF24L01_SPI_TX(*(((uint8_t*)Data)+2));
	NRF24L01_SPI_TX(*(((uint8_t*)Data)+3));
	NRF_PORT.OUTSET = (1<<NRF_CSN);

	//pulse CE
	NRF_PORT.OUTSET = (1<<NRF_CE);
	_delay_us(12);
	NRF_PORT.OUTCLR = (1<<NRF_CE);
	//wait for tx bit in status reg to be set then clear it.
	while(!(NRF24L01_R_Reg(NREG_STATUS) & 0b00100000)) {
		sprintf_P(s, PSTR("Config: 0x%02X\r\n"), NRF24L01_R_Reg(NREG_CONFIG));
		USART_tx_String(&USARTC0, s);
		sprintf_P(s, PSTR("Status: 0x%02X\r\n"), NRF24L01_R_Reg(NREG_STATUS));
		USART_tx_String(&USARTC0, s);
		sprintf_P(s, PSTR("FIFO: 0x%02X\r\n"), NRF24L01_R_Reg(NREG_FIFOSTATUS));
		USART_tx_String(&USARTC0, s);
		_delay_ms(50);
	}

	//uint8_t i = 0;
	//while((NRF24L01_R_Reg(NREG_STATUS) & 0b00100000)) {
		NRF24L01_W_Reg(NREG_STATUS,0x70);
		//i++;
	//}
	//if (i > 0) {
	//	sprintf_P(s, PSTR("Took %u times to clear to 0x%02X!\r\n"), i, NRF24L01_R_Reg(NREG_STATUS));
	//	USART_tx_String(s);
	//}


}

void NRF24L01_SPI_TX(uint8_t Data) {
	NRF_SPI.DATA = Data;
	while(!(NRF_SPI.STATUS & (1<<SPI_IF_bp)));
}

uint8_t NRF24L01_SPI_RX() {

	NRF_SPI.DATA = 0x00;
	while(!(NRF_SPI.STATUS & (1<<SPI_IF_bp)));
	return NRF_SPI.DATA;
}

void NRF24L01_SPI_Setup() {

	// PX7 (SCK), PX5 (MOSI), PX4 (CSN), PX3 (CE) as outputs
	NRF_PORT.DIRSET = (1<<NRF_SCK)|(1<<NRF_MOSI)|(1<<NRF_CSN)|(1<<NRF_CE);
	// PX6 (MISO), PX2 (IRQ) as inputs
	NRF_PORT.DIRCLR = (1<<NRF_IRQ)|(1<<NRF_MISO);
	// PX4 (CSN) enable pull-up. (TODO[ ]: VERIFY THAT THIS IS HOW YOU DO THIS IN AN XMEGA!!)
	PORTCFG_MPCMASK = (1<<NRF_CSN)|(1<<NRF_IRQ);
	//pull up configuration
	NRF_PORT.PIN0CTRL = 0b00011000;

	NRF_SPI.CTRL = SPI_ENABLE_bm|SPI_MASTER_bm|SPI_MODE_0_gc|SPI_PRESCALER_DIV4_gc;

}
