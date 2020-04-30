/*
 * IO.h
 *
 *  Created on: 12/10/2011
 *      Author: MC
 */

#ifndef IO_H_
#define IO_H_

#include <stdint.h>

//Typedefs:
typedef uint8_t SPIData_t;
typedef enum pinstate {
	PIN_LOW = 0,
	PIN_HIGH = 1,
	PIN_UNCHANGED = 2,
} pinState_t;

//Defines:
#define NRF_PORT	PORTB
#define NRF_DDR		DDRB
#define NRF_CE 		1		//O
#define NRF_CSN 	2		//O
#define NRF_MOSI 	3		//O
#define NRF_MISO 	4		//I
#define NRF_SCK 	5		//O

//Function Prototypes:
void IO_SPI_TX(SPIData_t data);
SPIData_t IO_SPI_RX(void);
void IO_NRF_CSN(pinState_t data);
void IO_NRF_CE(pinState_t data);
void IO_NRF_Setup_Pins(void);
void IO_NRF_Setup_SPI(void);

#endif /* IO_H_ */
