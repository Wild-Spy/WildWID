/*
 * M_USART.c
 *
 *  Created on: 22/01/2010
 *      Author: Matty
 */

#include "MX_USART.h"

volatile uint16_t Timer1 = 0;

ISR(TCC1_OVF_vect) {
	USART_UpdateTimers();
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
void USART_Setup (PORT_t* m_PORT, USART_t* m_USART, int bsel, uint8_t bscale) {

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

}

/* Function Name:	USART_tx_Byte
** Created On:		17/08/2010
** Created By:		Matthew Cochrane
** Description:		Transmits a single character via the USART.
** Note:			Requires the USART to be initialised!
** Inputs:
** 	data	-	The char to be transmitted
*/
void USART_tx_Byte (USART_t* m_USART, char data) {
	/* Wait for empty transmit buffer */
	if (USART_USB_IN) {
		while (!(m_USART->STATUS & 0b00100000));
		/* Put data into buffer, sends the data */
		m_USART->DATA = data;
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
void USART_tx_String (USART_t* m_USART, const char * data) {
	if (USART_USB_IN) {
		while (*data++ != '\0') {
			USART_tx_Byte(m_USART, *(data-1));
		}
	}
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
void USART_tx_String_P (USART_t* m_USART, const char * data) {
	if (USART_USB_IN) {
		char tmpChar;
		tmpChar = pgm_read_byte(data++);
		if (tmpChar) {
			do {
				USART_tx_Byte(m_USART, tmpChar);
				tmpChar = pgm_read_byte(data++);
			} while (tmpChar != '\0');
		}
	}
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
int USART_rx_Byte (USART_t* m_USART, uint16_t timeout) {

	uint8_t TO = 0;

	if (timeout) {
		USART_StartTim10ms(timeout/10);
	}

	// Wait for data to be received
	while ((!(m_USART->STATUS & 0b10000000))&&USART_USB_IN) {
		if (timeout && AtomicRead16(Timer1) == 0) {
			TO = 1;
			break;
		}
	}

	if (timeout) {
		USART_StopTim10ms();
	}

	if (TO) return -1;
	if (!USART_USB_IN) return -2;

	// Get and return received data from buffer
	return (uint16_t)m_USART->DATA;
}

/* Function Name:	USART_rx_Byte_nb
** Created On:		01/09/2010
** Created By:		Matthew Cochrane
** Description:		Non blocking version of USART_rx_Byte.  If a byte is available
** 					to receive then the function will return it in the lower byte
** 					of the integer it returns.  Otherwise it will return -1.
**
** Note:			Requires the USART to be initialised!
** Outputs:
** 	Returns	-	The character received from the USART or -1 if no character is available.
**
*/
int USART_rx_Byte_nb (USART_t* m_USART) {

	// Wait for data to be received
	if ((m_USART->STATUS & 0b10000000))
	// Get and return received data from buffer
		return (int)m_USART->DATA;
	else if (USART_USB_IN)
		return -2;
	else
		return -1;
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
int USART_rx_String (USART_t* m_USART, char * data, uint16_t timeout) {
	if (USART_USB_IN) {
		int tmpInt;
		uint8_t count = 0;

		tmpInt = USART_rx_Byte(m_USART, timeout);
		if ((tmpInt != -1) && ((char)tmpInt != '\n')) {
			count++;
			do {
				*data++ = (char)tmpInt;
				tmpInt = USART_rx_Byte(m_USART, timeout);
				count++;
			} while ((tmpInt >= 0) && ((char)tmpInt != '\n'));
		}
		if (tmpInt < 0) {
			*data++ = '\0';
			return -1;
		} else {
			*data++ = (char)tmpInt;
			*data++ = '\0';
			return count;
		}
	} else
		return -2;
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
uint8_t USART_rx_String_F (USART_t* m_USART, char * data, uint16_t timeout, uint8_t Len, uint8_t Flags) {

	if (USART_USB_IN) {
		int tmpInt;

		do {
			tmpInt = USART_rx_Byte(m_USART, timeout);
			if (tmpInt < 0) break;
			if (Flags & USART_FF_ECHOCHARS)
				USART_tx_Byte(m_USART, (char)tmpInt);
			*data++ = (char)tmpInt;
		} while (--Len);
	}
	*data++ = '\0';
	return Len;
}

inline void USART_UpdateTimers () {
	cli();
	if (Timer1) Timer1--;
	else {
		TC1_ConfigClockSource(&TCC1, TC_CLKSEL_OFF_gc);
		TCC1.INTCTRLA = TC_OVFINTLVL_OFF_gc;
	}
	sei();
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
	//Clear the timer
	Timer1 = 0;
}

int USART_RX_Int(USART_t* m_USART, uint8_t Digits, uint8_t Flags) {

	//if digits == 0 then finish with a new line

	if (USART_USB_IN) {

		uint8_t i;
		char tmpByte;
		char tmpStr[10];
		int retVal = 0;

		if (Flags & USART_FF_SHOWFORMAT) {
			USART_tx_Byte(m_USART, '(');
			for (i = 0; i < Digits; i++)
				USART_tx_Byte(m_USART, '#');
			USART_tx_Byte(m_USART, ')');
		}

		if (Digits)
			i = Digits;
		else {
			i = USART_rx_String(m_USART, tmpStr, 0);
		}

		do {
			if (i > 5) i = 5;
			if (Digits) {
				tmpByte = USART_rx_Byte(m_USART, 0);
				if (Flags & USART_FF_ECHOCHARS)
					USART_tx_Byte(m_USART, tmpByte);
			} else
				tmpByte = tmpStr[i];
			retVal += (tmpByte-'0')*(mPow(10,(i-1)));
		} while (--i);

		if (Flags & USART_FF_SHOWFORMAT && Digits) {
			USART_tx_String_P(m_USART, PSTR("\r\n"));
		}

		return retVal;
	} else
		return -2;
}

int mPow(int base, uint8_t exponent) {
	int retval = base;
	if (exponent == 0)
		return 1;
	while (--exponent){
		retval = base*retval;
	}
	return retval;
}


