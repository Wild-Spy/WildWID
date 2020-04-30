/*
 * M_NRF24L01.h
 *
 *  Created on: 09/03/2010
 *      Author: Matty
 */

#ifndef M_NRF24L01_H_
#define M_NRF24L01_H_

#include <stdlib.h>
#include <avr/io.h>
#include <util/delay.h>


//Pins:
#define CSN1 		2
#define CE1 		1
#define NRF_MISO 	4
#define NRF_MOSI 	3
#define NRF_SCK 	5


//nRF24L01 Registers:
#define NREG_CONFIG		0x00
#define NREG_EN_AA		0x01
#define NREG_EN_RXADDR	0x02
#define NREG_SETUP_AW	0x03
#define NREG_SETUP_RETR	0x04
#define NREG_RF_CH		0x05
#define NREG_RF_SETUP	0x06
#define NREG_STATUS		0x07
#define NREG_OBSERVE_TX	0x08
#define NREG_RPD		0x09
#define NREG_RX_ADDR_P0	0x0A	//5 bytes
#define NREG_RX_ADDR_P1	0x0B	//5 bytes
#define NREG_RX_ADDR_P2	0x0C
#define NREG_RX_ADDR_P3	0x0D
#define NREG_RX_ADDR_P4	0x0E
#define NREG_RX_ADDR_P5	0x0F
#define NREG_TX_ADDR	0x10	//5 bytes
#define NREG_RX_PW_P0	0x11
#define NREG_RX_PW_P1	0x12
#define NREG_RX_PW_P2	0x13
#define NREG_RX_PW_P3	0x14
#define NREG_RX_PW_P4	0x15
#define NREG_RX_PW_P5	0x16
#define NREG_FIFOSTATUS 0x17

//Macros:
#define NRF_POWERDOWN	NRF24L01_SetMode(0,1)
#define NRF_RXMODE		NRF24L01_SetMode(1,1)
#define NRF_TXMODE		NRF24L01_SetMode(1,0)

//Function Prototypes
void NRF24L01_SPI_Setup( );
void NRF24L01_SPI_TX(uint8_t);
uint8_t NRF24L01_SPI_RX( );
void NRF24L01_W_Reg(uint8_t , uint8_t);
void NRF24L01_W_Reg_Long(uint8_t , uint8_t* , uint8_t);
uint8_t NRF24L01_R_Reg(uint8_t);
void NRF24L01_R_Reg_Long(uint8_t , uint8_t* , uint8_t);
uint8_t NRF24L01_TX_PAYLOAD(uint32_t*);
//uint8_t NRF24L01_R_RX_PAYLOAD(uint32_t*);
void NRF24L01_SetupnRF24L01(uint8_t, uint8_t*);
void NRF24L01_SetMode(uint8_t , uint8_t);
void NRF24L01_Flush_RX_FIFO( );
void NRF24L01_Flush_TX_FIFO( );


#endif /* M_NRF24L01_H_ */
