/*
 * M_USART.c
 *
 *  Created on: 22/01/2010
 *      Author: Matthew Cochrane
 *	   Version: 1.0
 */

#include "MX_USART.h"

extern char s[80];

volatile uint16_t Timer1 = 0;

ISR(TCC1_OVF_vect) {
	cli();
	if (Timer1) Timer1--;
	else {
		TC1_ConfigClockSource(&TCC1, TC_CLKSEL_OFF_gc);
		TCC1.INTCTRLA = TC_OVFINTLVL_OFF_gc;
		TCC1.CNT = 0;
	}
	sei();
}

/* Function Name:	SetupUSART for XMEGA
** Created On:		17/08/2010
** Created By:		Matthew Cochrane
** Description:		Sets up the USART in asynchronous mode to a Baud rate specified by
** 					UBRR and UU2X with 8 data bits no parity and one stop bit.
** Inputs:
**  m_PORT	-	A pointer to the port object on which the USART resides.
**  m_USART	-	A pointer the the USART object to be configured
** 	bsel	-	Used to select the baud rate. (See Note)
** 	bscale	-	Used to select the baud rate. (See Note)
**
** Notes:
** 	Use the following link to calculate bsel and bscale values:
** 		http://www.avrcalc.elektronik-projekt.de/xmega/baud_rate_calculator
*/
void USART_Setup (PORT_t* m_PORT, USART_t* m_USART, int bsel, uint8_t bscale, uint8_t doubleSpeed) {

	m_PORT->DIRSET = PIN3_bm;	// PX3 (TXD0) as output
	m_PORT->DIRCLR = PIN2_bm;	// PX2 (RXD0) as input

	// Set USART Format to 8bit, no parity, 1 stop bit
	m_USART->CTRLC = USART_CHSIZE_8BIT_gc | USART_PMODE_DISABLED_gc ;

	// 9600bps @ 32Mhz as calculated from ProtoTalk Calc
	//int bsel = 3317;
	//uint8_t bscale = -4;

	m_USART->BAUDCTRLA = (uint8_t) bsel;
	m_USART->BAUDCTRLB = (bscale << 4) | (bsel >> 8);

	// Enable both RX and TX.
	m_USART->CTRLB |= USART_RXEN_bm | USART_TXEN_bm;
	if (doubleSpeed)
		m_USART->CTRLB |= USART_CLK2X_bm;

}

void USART_StartTim10ms(uint16_t Time) {
	cli();
	Timer1 = Time;
	//Setup Timer 0 for 10ms Timeout
	TC_SetPeriod(&TCC1, 0x3A97);//(12MHz) //0x9C3F); //(32MHz)
	TCC1.CNT = 0;
	TC1_ConfigClockSource(&TCC1, TC_CLKSEL_DIV8_gc);
	TCC1.INTCTRLA = TC_OVFINTLVL_MED_gc;
	sei();
}

void USART_StopTim10ms() {
	//Stop the timer
	TC1_ConfigClockSource(&TCC1, TC_CLKSEL_OFF_gc);
	TCC1.INTCTRLA = TC_OVFINTLVL_OFF_gc;
	TCC1.CNT = 0;
	//Clear the timer
	Timer1 = 0;
}

/* Function Name:	USART_tx_Byte
** Created On:		17/08/2010
** Created By:		Matthew Cochrane
** Description:		Transmits a single character via the USART.
** Note:			Requires the USART to be initialised!
** Inputs:
** 	data	-	The char to be transmitted
*/
//TODO[ ]: Not Tested with return value implemented!!!
USART_err_t USART_tx_Byte (USART_t* m_USART, char data) {
	if (USART_USB_IN || (m_USART != &USARTC0)) {
		/* Wait for empty transmit buffer */
		while (!(m_USART->STATUS & 0b00100000));
		/* Put data into buffer, sends the data */
		m_USART->DATA = data;
		return USART_err_Success;
	} else {
		return USART_err_USBUnplugged;
	}
}

/* Function Name:	USART_tx_String
** Created On:		17/08/2010
** Created By:		Matthew Cochrane
** Description:		Transmits a null terminated string via the USART not including the null
** 					character terminating it.
** Note:			Requires the USART to be initialised!
** Inputs:
** 	data	-	A pointer to the start of the string to be transmitted.  The string will not
** 				be altered by the function and is cast as const.
*/
//TODO[ ]: Not Tested with return value implemented!!!
USART_err_t USART_tx_String (USART_t* m_USART, const char * data) {
	USART_err_t tmpUSARTErr;

	while (*data++ != '\0') {
		if ((tmpUSARTErr = USART_tx_Byte(m_USART, *(data-1))) < 0) {
			return tmpUSARTErr;
		}
	}

	return USART_err_Success;
}

/* Function Name:	USART_tx_String_P
** Created On:		17/08/2010
** Created By:		Matthew Cochrane
** Description:		Transmits a null terminated string which is located in program memory
** 					via the USART.  It does not transmit the null character which terminates
** 					the string.
** Note:			Requires the USART to be initialised!
** Example:			To transmit a static string via the USART use this function rather than
** 					USART_tx_String as it allows the string to be stored completely in
** 					program memory rather than taking up SRAM unnecessarily.  Use the PSTR
** 					macro when calling this function. eg.
** 						USART_tx_String_P(PSTR("Hello World!!"));
** Inputs:
** 	data	-	A 16-bit pointer to the start of the string to be transmitted in program
** 				memory!  NOT in SRAM!!
*/
//TODO[ ]: Not Tested with return value implemented!!!
USART_err_t USART_tx_String_P (USART_t* m_USART, const char* data) {
	USART_err_t tmpUSARTErr;
	char tmpChar;
	
	tmpChar = pgm_read_byte(data++);
	if (tmpChar) {
		do {
			if ((tmpUSARTErr = USART_tx_Byte(m_USART, tmpChar)) < 0) {
				return tmpUSARTErr;
			}
			tmpChar = pgm_read_byte(data++);
		} while (tmpChar != '\0');
	}
	return USART_err_Success;
}

/* Function Name:	USART_rx_Byte
** Created On:		17/08/2010
** Created By:		Matthew Cochrane
** Description:		Waits for and receives a single character from the USART.  Will time out in
** 					'timeout' milliseconds.
** Note:			Requires the USART to be initialised!
** Outputs:
** 	Returns	-	An integer with the value of -1 if the function timed out and with the value of
** 				the character it received otherwise.
**
*/
USART_err_t USART_rx_Byte (USART_t* m_USART, uint16_t timeout_ms, uint8_t* data) {

	uint8_t TO = 0;

	if (timeout_ms) {
		USART_StartTim10ms(timeout_ms/10);
	}

	// Wait for data to be received
	//TODO[ ]: Make this more efficient
	while ((!(m_USART->STATUS & 0b10000000))&&(USART_USB_IN || (m_USART != &USARTC0))) {
		if (timeout_ms && AtomicRead16(Timer1) == 0) {
			TO = 1;
			break;
		}
	}

	if (timeout_ms) {
		USART_StopTim10ms();
	}

	if (TO) return USART_err_Timeout;
	if (!USART_USB_IN && (m_USART == &USARTC0)) return USART_err_USBUnplugged;

	// Get and return received data from buffer
	*data = m_USART->DATA;
	return USART_err_Success;
}

/* Function Name:	USART_rx_Byte_nb
** Created On:		01/09/2010
** Created By:		Matthew Cochrane
** Description:		Non blocking version of USART_rx_Byte.  
**
** Note:			Requires the USART to be initialised!
** Outputs:
** 	Returns	-	
**
*/
USART_err_t USART_rx_Byte_nb (USART_t* m_USART, uint8_t* data) {

	// Wait for data to be received
	if ((m_USART->STATUS & 0b10000000)) {
	// Get and return received data from buffer
		*data = m_USART->DATA;
		return USART_err_Success;
	} else if (USART_USB_IN && (m_USART == &USARTC0)) {
		return USART_err_USBUnplugged;
	} else
		return USART_err_NoData;
}

/* Function Name:	USART_rx_String
** Created On:		17/08/2010
** Created By:		Matthew Cochrane
** Description:		Receives a line (terminated by a '\r' character) from the USART.  This
** 					function never times out.
** Note:			Requires the USART to be initialised!
** Outputs:
** 	data	-	A pointer to the string received by the USART.
*/
USART_err_t USART_rx_String (USART_t* m_USART, char* data, uint16_t timeout_ms, uint8_t* len) {
	if (USART_USB_IN || (m_USART != &USARTC0)) {
		uint8_t tmpByte;
		USART_err_t tmpUSARTErr;
		uint8_t count = 0;

		tmpUSARTErr = USART_rx_Byte(m_USART, timeout_ms, &tmpByte);
		if ((tmpUSARTErr == USART_err_Success) && ((char)tmpByte != '\n')) {
			count++;
			do {
				*data++ = tmpByte;
				tmpUSARTErr = USART_rx_Byte(m_USART, timeout_ms, &tmpByte);
				count++;
			} while ((tmpUSARTErr == USART_err_Success) && (tmpByte != '\n'));
		}
		if (tmpUSARTErr < 0) {
			*data++ = '\0';
			return tmpUSARTErr;
		} else {
			*data++ = tmpByte;
			*data++ = '\0';
			//return count;
			*len = count;
			return tmpUSARTErr;
		}
	} else
		return mx_err_USBUnplugged;
}

/* Function Name:	USART_rx_String_F
** Created On:		17/08/2010
** Created By:		Matthew Cochrane
** Description:		Receive a string from the USART of a fixed length.
** Note:			Requires the USART to be initialised!
** Outputs:
** 	data	-	A pointer to the string received by the USART.
**  return	-	The number of characters that weren't received (ie. if there were 2 characters
**  			to go when it timed out then it returns 2.  if no timeout occurs it returns 0)
*/
USART_err_t USART_rx_String_F (USART_t* m_USART, char* data, uint16_t timeout_ms, uint8_t* Len, uint8_t Flags) {

	if (USART_USB_IN || (m_USART != &USARTC0)) {
		uint8_t tmpByte;
		USART_err_t tmpUSARTErr;

		do {
			tmpUSARTErr = USART_rx_Byte(m_USART, timeout_ms, &tmpByte);
			if (tmpUSARTErr < 0) break;
			if (Flags & USART_FF_ECHOCHARS)
				USART_tx_Byte(m_USART, tmpByte);
			*data++ = tmpByte;
			if (Flags&USART_FF_ENSABORT)
				if (tmpByte == 's')
					break;
		} while (--(*Len));
		*data++ = '\0';
		return tmpUSARTErr;
	} else {
		return USART_err_USBUnplugged;	
	}		
	
}


