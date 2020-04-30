/*
 * Keypad.c
 *
 *  Created on: 23/04/2011
 *      Author: MC
 */

#include "Keypad.h"

extern char s[80];
char lastKey = 0;

void KP_IntMode() {

	KP_INTDIS();
	//Setup Columns
	PORTCFG.MPCMASK = KP_C1_bm|KP_C2_bm|KP_C3_bm;
	KP_PORT.PIN0CTRL = PORT_OPC_PULLUP_gc|PORT_ISC_BOTHEDGES_gc;
	KP_PORT.DIRCLR = KP_C1_bm|KP_C2_bm|KP_C3_bm;

	//Setup Other Pins
#if ((KP_C1_bp == 2)|(KP_C2_bp == 2)|(KP_C3_bp == 2))
	//Interrupt on Column - set rows as outputs
	KP_PORT.DIRSET = KP_R1_bm|KP_R2_bm|KP_R3_bm|KP_R4_bm;
	KP_PORT.OUTCLR = KP_R1_bm|KP_R2_bm|KP_R3_bm|KP_R4_bm;
	KP_PORT.DIRCLR = (1<<2);
#else
	//interrupt on Row - set columns as outputs
	KP_PORT.DIRSET = KP_C1_bm|KP_C2_bm|KP_C3_bm;
	KP_PORT.OUTCLR = KP_C1_bm|KP_C2_bm|KP_C3_bm;
	KP_PORT.DIRCLR = (1<<2);
#endif

	//Setup Interrupt
	KP_PORT.INT0MASK = KP_C1_bm|KP_C2_bm|KP_C3_bm;
	//Clear Interrupt
	KP_PORT.INTFLAGS = (1<<0);
	//Clear and Enable Interrupt
	KP_PORT.INTFLAGS = PORT_INT0IF_bm;
	KP_INTEN();

}

char KP_KeyScan() {
	uint8_t Row, Col;
	char retVal = 0;//KP_KEY_NONE;

	//Disable Interrupt
	KP_INTDIS();
	//Disable Colums Pullups
	//PORTCFG.MPCMASK = KP_C1_bm|KP_C2_bm|KP_C3_bm;
	//KP_PORT.PIN0CTRL = 0;

	//Set ALL keypad pins as inputs
	KP_ALLINPUT();

	//Pulldowns on Columns
	//PORTCFG.MPCMASK = KP_C1_bm|KP_C2_bm|KP_C3_bm;
	//KP_PORT.PIN0CTRL = PORT_OPC_PULLDOWN_gc;

	for (Col = 1; Col <= 3; Col++) {
		for (Row = 1; Row <= 4; Row++) {
			//Setup Row as an output - low
			//USART_printf_P(&USARTC0, "R:%d, C:%d\r\n",Row, Col);
			KP_PORT.DIRSET = KP_RowTobm(Row);
			KP_PORT.OUTCLR = KP_RowTobm(Row);
			if (!(KP_PORT.IN&KP_ColTobm(Col))) {
				//USART_printf_P(&USARTC0, "PASS: 0x%02X\r\n",KP_PORT.IN&KP_ColTobm(Col));
				retVal = KP_RowColToKey(Row,Col);
				KP_PORT.DIRCLR = KP_RowTobm(Row);
				KP_ALLINPUT();
				goto Exit;
			}
			KP_PORT.DIRCLR = KP_RowTobm(Row);
		}
	}

	//USART_printf_P(&USARTC0, "NOKEY\r\n",1);
	Exit:
	//USART_printf_P(&USARTC0, "EXIT\r\n",1);
	//Pulldowns on Columns
	PORTCFG.MPCMASK = KP_C1_bm|KP_C2_bm|KP_C3_bm;
	KP_PORT.PIN0CTRL = 0;

	//PColums Ints
	//PORTCFG.MPCMASK = KP_C1_bm|KP_C2_bm|KP_C3_bm;
	//KP_PORT.PIN0CTRL = PORT_ISC_BOTHEDGES_gc;
	//KP_PORT.DIRCLR = KP_C1_bm|KP_C2_bm|KP_C3_bm;
	lastKey = retVal;

	return retVal;
}

uint8_t KP_KeyStatus(uint8_t Row, uint8_t Col) {
	//uint8_t Row, Col;
	char retVal = 0;//KP_KEY_NONE;

	//Disable Interrupt
	KP_INTDIS();
	//Enable Colums Pullups
	PORTCFG.MPCMASK = KP_C1_bm|KP_C2_bm|KP_C3_bm;
	KP_PORT.PIN0CTRL = PORT_OPC_PULLUP_gc;

	//Set ALL keypad pins as inputs
	KP_ALLINPUT();

	//Setup Row as an output - low
	KP_PORT.DIRSET = KP_RowTobm(Row);
	KP_PORT.OUTCLR = KP_RowTobm(Row);
	if (!(KP_PORT.IN&KP_ColTobm(Col))) {
		retVal = 1;
	}
	KP_ALLINPUT();

	//Pulldowns on Columns
	PORTCFG.MPCMASK = KP_C1_bm|KP_C2_bm|KP_C3_bm;
	KP_PORT.PIN0CTRL = 0;

	return retVal;
}

uint8_t KP_RowTobm(uint8_t Row) {
	switch (Row) {
	case 1:
		return KP_R1_bm;
		break;
	case 2:
		return KP_R2_bm;
		break;
	case 3:
		return KP_R3_bm;
		break;
	case 4:
		return KP_R4_bm;
		break;
	}
	return 0;
}

uint8_t KP_ColTobm(uint8_t Col) {
	switch (Col) {
	case 1:
		return KP_C1_bm;
		break;
	case 2:
		return KP_C2_bm;
		break;
	case 3:
		return KP_C3_bm;
		break;
	}
	return 0;
}

char KP_RowColToKey(uint8_t Row, uint8_t Col) {
	uint8_t RowCol;
	RowCol = (Row<<4) + Col;
	switch (RowCol) {
	case 0x11:
		return '1';//KP_KEY_1;
		break;
	case 0x12:
		return '2';//KP_KEY_2;
		break;
	case 0x13:
		return '3';//KP_KEY_3;
		break;
	case 0x21:
		return '4';//KP_KEY_4;
		break;
	case 0x22:
		return '5';//KP_KEY_5;
		break;
	case 0x23:
		return '6';//KP_KEY_6;
		break;
	case 0x31:
		return '7';//KP_KEY_7;
		break;
	case 0x32:
		return '8';//KP_KEY_8;
		break;
	case 0x33:
		return '9';//KP_KEY_9;
		break;
	case 0x41:
		return '*';//KP_KEY_STAR;
		break;
	case 0x42:
		return '0';//KP_KEY_0;
		break;
	case 0x43:
		return '#';//KP_KEY_HASH;
		break;
	}
	return 0;//KP_KEY_NONE;
}

/*
 *
ISR(PORTA_INT0_vect) {
	if (lastKey == 0)
		Wakeup_Source |= WAKEUP_SOURCE_KP;
	else {
		KP_KeyScan();
		KP_IntMode();
	}
}
 */
