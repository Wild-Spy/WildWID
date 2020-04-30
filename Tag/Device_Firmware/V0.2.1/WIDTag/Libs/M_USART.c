/*
 * M_USART.c
 *
 *  Created on: 22/01/2010
 *      Author: Matty
 */

#include "M_USART.h"

volatile unsigned int Timer1;

/* Function Name:	SetupUSART
** Created On:		22/01/2010
** Created By:		Matthew Cochrane
** Description:		Sets up the USART in asynchronous mode to a Baud rate specified by
** 					UBRR and UU2X with 8 data bits no parity and one stop bit.
** Inputs:
** 	UBRR	-	Value to set the UBRR register to
** 	UU2X	-	Boolean variable which if true will enable double data rate for the USART.
*/
void USART_Setup (unsigned int UBRR, unsigned char UU2X) {
	//Setup Baud Rate
	UDRRRegH = (unsigned char)(UBRR >> 8);
	UDRRRegL = (unsigned char)(UBRR & 0x00FF);

	//Setup Double Speed Setting
	if (UU2X) {
		UCSRAReg = (1<<U2XBit);
	} else {
		UCSRAReg = 0;
	}

	//Enable Transmitter and Receiver
	UCSRBReg = (1<<TXENBit)|(1<<RXENBit);

	//8 data bits per frame
	UCSRCReg = (1<<UCSZ0Bit)|(1<<UCSZ1Bit);
}

/* Function Name:	USART_tx_Byte
** Created On:		22/01/2010
** Created By:		Matthew Cochrane
** Description:		Transmits a single character via the USART.
** Note:			Requires the USART to be initialised!
** Inputs:
** 	data	-	The char to be transmitted
*/
void USART_tx_Byte (char data) {
	/* Wait for empty transmit buffer */
	while ( !( UCSRAReg & (1<<UDREBit)) );
	/* Put data into buffer, sends the data */
	UDRReg = data;
}

/* Function Name:	USART_tx_String
** Created On:		22/01/2010
** Created By:		Matthew Cochrane
** Description:		Transmits a null terminated string via the USART not including the null
** 					character terminating it.
** Note:			Requires the USART to be initialised!
** Inputs:
** 	data	-	A pointer to the start of the string to be transmitted.  The string will not
** 				be altered by the function and is cast as const.
*/
void USART_tx_String (const char * data) {
	while (*data++ != '\0') {
		USART_tx_Byte(*(data-1));
	}
}

/* Function Name:	USART_tx_String_P
** Created On:		22/01/2010
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
void USART_tx_String_P (const char * data) {
	char tmpChar;
	tmpChar = pgm_read_byte(data++);
	if (tmpChar) {
		do {
			USART_tx_Byte(tmpChar);
			tmpChar = pgm_read_byte(data++);
		} while (tmpChar != '\0');
	}
}

/* Function Name:	USART_rx_Byte
** Created On:		22/01/2010
** Created By:		Matthew Cochrane
** Description:		Waits for and receives a single character from the USART.
** Note:			Requires the USART to be initialised!
** 					A timeout of 0 will cause the function to abort immediately! (unless there is already data in the USART ready to be read)
** Outputs:
** 	Returns	-	The character received from the USART.
**
*/
char USART_rx_Byte (unsigned int timeout) {

	//Setup the timer
	PRR &= ~(1<<PRTIM0);
	TCCR0A = 0;
	TCCR0B = (1<<CS02)|(1<<CS00); // 1024 clock about 4Hz
	TIMSK0 = (1<<TOIE0);

	Timer1 = timeout;

	// Wait for data to be received
	while ( !(UCSRAReg & (1<<RXCBit)) && Timer1 );

	//Disable the timer
	TIMSK0 &= ~(1<<TOIE0);
	TCCR0B = 0;
	PRR |= (1<<PRTIM0);

	if (Timer1 == 0) return 0xFF;

	// Get and return received data from buffer
	return UDRReg;
}

int USART_rx_Byte_nb () {

	// Check if data has been received
	if ((UCSRAReg & 0b10000000))
	// Get and return received data from buffer
		return (int)UDRReg;
	else
		return -1;
}

/* Function Name:	USART_rx_String
** Created On:		22/01/2010
** Created By:		Matthew Cochrane
** Description:		Receives a line (terminated by a '\n' character) from the USART.
** Note:			Requires the USART to be initialised!
** Outputs:
** 	data	-	A pointer to the string received by the USART.
*/
unsigned char USART_rx_String (char* data, unsigned int timeout) {
	char tmpChar;
	unsigned char count = 0;
	tmpChar = USART_rx_Byte(timeout);
	if ((tmpChar != '\n') && (tmpChar != 0xFF)) {
		count++;
		do {
			*data++ = tmpChar;
			tmpChar = USART_rx_Byte(timeout);
			count++;
		} while ((tmpChar != '\n') && (tmpChar != 0xFF));
	}
	if (tmpChar != 0xFF) {
		*data++ = '\0';
	} else {
		*data++ = tmpChar;
		*data++ = '\0';
	}
	return count;
}

void USART_UpdateTimers () {
	if (Timer1 > 0) Timer1--;
}





