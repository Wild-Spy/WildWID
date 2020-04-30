/*
 * KeyPad.h
 *
 *  Created on: 23/04/2011
 *      Author: MC
 */

#ifndef KEYPAD_H_
#define KEYPAD_H_

#include <stdlib.h>
#include <avr/io.h>
#include <avr/interrupt.h>
#include "NHDC160100DiZ.h"
#include "MX_USART.h"

//Shouldn't be too hard to modify if inputs span over multiple ports.

//Defines
#define KP_PORT			PORTA

#define	KP_C1_bp		0
#define	KP_C2_bp		1
#define	KP_C3_bp		2
#define	KP_R1_bp		3
#define	KP_R2_bp		4
#define	KP_R3_bp		5
#define	KP_R4_bp		6

#define	KP_C1_bm		(1<<KP_C1_bp)
#define	KP_C2_bm		(1<<KP_C2_bp)
#define	KP_C3_bm		(1<<KP_C3_bp)
#define KP_R1_bm		(1<<KP_R1_bp)
#define	KP_R2_bm		(1<<KP_R2_bp)
#define	KP_R3_bm		(1<<KP_R3_bp)
#define	KP_R4_bm		(1<<KP_R4_bp)

/*typedef enum KP_KEY_enum {
	KP_KEY_NONE = 0,
	KP_KEY_1 = '1',
	KP_KEY_2 = '2',
	KP_KEY_3 = '3',
	KP_KEY_4 = '4',
	KP_KEY_5 = '5',
	KP_KEY_6 = '6',
	KP_KEY_7 = '7',
	KP_KEY_8 = '8',
	KP_KEY_9 = '9',
	KP_KEY_STAR = '*',
	KP_KEY_0 = '0',
	KP_KEY_HASH = '#',
} KP_KEY_t;*/

//Macros
#define KP_INTEN()		KP_PORT.INTCTRL = PORT_INT0LVL_MED_gc
#define KP_INTDIS()		KP_PORT.INTCTRL = PORT_INT0LVL_OFF_gc
#define KP_ALLINPUT()	KP_PORT.DIRCLR = KP_C1_bm|KP_C2_bm|KP_C3_bm|KP_R1_bm|KP_R2_bm|KP_R3_bm|KP_R4_bm


//Function Definitions
void KP_IntMode( void );
char KP_KeyScan( void );
uint8_t KP_RowTobm(uint8_t Row);
uint8_t KP_ColTobm(uint8_t Col);
char KP_RowColToKey(uint8_t Row, uint8_t Col);
uint8_t KP_KeyStatus(uint8_t Row, uint8_t Col);

#endif /* KEYPAD_H_ */
