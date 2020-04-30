/*
 * M_USART.h
 *
 *  Created on: 22/01/2010
 *      Author: Matthew Cochrane
 */

#ifndef MX_USART_H_
#define MX_USART_H_

//Includes
#include <avr/io.h>
#include <avr/interrupt.h>
#include <stdio.h>
#include <math.h>
#include <avr/pgmspace.h>
#include "TC_driver.h"
#include "MX_RTC.h"
#include "MX_ErrorEnum.h"
#include "MX_SerComms.h"

typedef enum _USART_err_t {
	USART_err_Success			=  0,
	USART_err_Timeout			= -1,
	USART_err_USBUnplugged		= -2,
	USART_err_InvalidData		= -3,
	USART_err_BadResponse		= -5,
	USART_err_NoData			= -6,
	USART_err_Unknown			= -20
} USART_err_t;

//Defines
//Formatting and Escape Sequences
#define ECC_COL_BLACK "0"
#define ECC_COL_RED "1"
#define ECC_COL_GREEN "2"
#define ECC_COL_YELLOW "3"
#define ECC_COL_BLUE "4"
#define ECC_COL_MAGENTA "5"
#define ECC_COL_CYAN "6"
#define ECC_COL_WHITE "7"
#define ECC_COL_RESET "9"
#define ESC_FG "3"
#define ESC_BG "4"
#define ESC_FONT_NORMAL "0"			//ALL attributes off
#define ESC_FONT_BOLD "1"
#define ESC_FONT_ITALIC "3"			//Not widely supported. Sometimes treated as inverse.
#define ESC_FONT_UNDERLINE "4"
#define ESC_FONT_NOUNDERLINE "24"
#define ESC_FONT_2XUNDERLINE "21"	//Not widely supported
#define ESC_INTENSITY_FAINT "2"		//Not widely supported
#define ESC_INTENSITY_NORMAL "22"	//Not bold and not faint
#define ESC_BLINK_SLOW "5"			//less than 150 per minute
#define ESC_BLINK_RAPID "6"			//more than 150 per minute
#define ESC_BLINK_OFF "25"
#define ESC_IMG_NEG "7"				//Inverse - swap foreground and background colours
#define ESC_IMG_POS "27"
#define ESC_CONCEAL	 "8"			//Not widely supported.  Hides any further writing
#define ESC_REVEAL "28"				//conceal off

//Macros
#define ESC_COL(type,col) type col
#define ESC_SGR1(var1) "\x1B[" var1 "m"
#define ESC_SGR2(var1, var2) "\x1B[" var1 ";" var2 "m"
#define ESC_SGR3(var1, var2, var3) "\x1B[" var1 ";" var2 ";" var3 "m"
#define ESC_SGR4(var1, var2, var3, var4) "\x1B[" var1 ";" var2 ";" var3 ";" var4 "m"
#define ESC_SGR5(var1, var2, var3, var4, var5) "\x1B[" var1 ";" var2 ";" var3 ";" var4 ";" var5 "m"
#define ESC_MOV_CUR_UP(n) "\x1B[" #n "A"
#define ESC_MOV_CUR_DOWN(n) "\x1B[" #n "B"
#define ESC_MOV_CUR_RIGHT(n) "\x1B[" #n "C"
#define ESC_MOV_CUR_LEFT(n) "\x1B[" #n "D"
#define ESC_MOV_CUR_ROW(n) "\x1B[" #n "A"
#define ESC_MOV_CUR_COL(n) "\x1B[" #n "A"
#define ESC_MOV_CUR(x,y) "\x1B[" #y ";" #x "H"	//Row x, Col y
#define ESC_CLS  "\x1B[2J"
#define ESC_CLR_LINE  "\x1B[2K"
#define ESC_SCROLL_UP(n)  "\x1B[" #n "S"
#define ESC_SCROLL_DOWN(n)  "\x1B[" #n "T"
#define ESC_GET_CUR_POS  "\x1B[6n"
#define ESC_CUR_SAVE  "\x1B[s"
#define ESC_CUR_RESTORE  "\x1B[u"

//Format Flags For Advanced Input
#define USART_FF_SHOWFORMAT		0b00000001
#define USART_FF_ECHOCHARS		0b00000010
#define USART_FF_HEX			0b00000100
#define USART_FF_VALIDDATE		0b00001000
#define USART_FF_ENSABORT		0b00010000

#define USART_USB_IN_PORT		PORTE
#define USART_USB_IN_PIN		0
#define USART_USB_IN			(USART_USB_IN_PORT.IN&(1<<USART_USB_IN_PIN))

#define USART_printf_P(usart, str, ...)	sprintf_P(s, PSTR(str), __VA_ARGS__); USART_tx_String(usart, s)

inline uint16_t AtomicRead16(uint16_t readVar) {
	cli();
	uint16_t retVal = readVar;
	sei();
	return retVal;
}

inline void AtomicWrite16(uint16_t* writeVar, uint16_t Value) {
	cli();
	*writeVar = Value;
	sei();
}

//Function Prototypes
void USART_Setup (PORT_t* m_PORT, USART_t* m_USART, int bsel, uint8_t bscale, uint8_t doubleSpeed);
USART_err_t USART_tx_Byte(USART_t* m_USART, char data);
USART_err_t USART_tx_String(USART_t* m_USART, const char * data);
USART_err_t USART_tx_String_P(USART_t* m_USART, const char * data);
USART_err_t USART_rx_Byte (USART_t* m_USART, uint16_t timeout_ms, uint8_t* data);
USART_err_t USART_rx_Byte_nb (USART_t* m_USART, uint8_t* data);
USART_err_t USART_rx_String (USART_t* m_USART, char* data, uint16_t timeout_ms, uint8_t* len);
USART_err_t USART_rx_String_F (USART_t* m_USART, char* data, uint16_t timeout_ms, uint8_t* Len, uint8_t Flags);
void USART_StartTim10ms(uint16_t Time);
void USART_StopTim10ms( void );

#endif /* MX_USART_H_ */
