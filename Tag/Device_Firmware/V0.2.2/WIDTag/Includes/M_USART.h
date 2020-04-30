/*
 * M_USART.h
 *
 *  Created on: 22/01/2010
 *      Author: Matthew Cochrane
 */

#ifndef M_USART_H_
#define M_USART_H_

//Includes
#include <avr/io.h>
#include <avr/pgmspace.h>
#include <stdio.h>

//Defines
//Register & Bit Defines - These need to be altered for your specific chip
#define UDRRRegH UBRR0H
#define UDRRRegL UBRR0L
#define UCSRAReg UCSR0A
#define UCSRBReg UCSR0B
#define UCSRCReg UCSR0C
#define UDRReg UDR0
#define U2XBit U2X0
#define RXENBit RXEN0
#define TXENBit TXEN0
#define UCSZ0Bit UCSZ00
#define UCSZ1Bit UCSZ01
#define UDREBit UDRE0
#define RXCBit RXC0
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

//extern char s[80];

#define USART_printf_P(str, ...)	do {sprintf_P(s, PSTR(str), __VA_ARGS__); USART_tx_String(s);} while (0)

//Function Prototypes
void USART_Setup(unsigned int, unsigned char);
void USART_tx_Byte(char);
void USART_tx_String(const char *);
void USART_tx_String_P(const char *);
char USART_rx_Byte(unsigned int);
int USART_rx_Byte_nb( void );
unsigned char USART_rx_String(char *, unsigned int);
void USART_UpdateTimers(void);


#endif /* M_USART_H_ */
