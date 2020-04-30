/*
 * NHDC160100DiZ.h
 *
 *  Created on: 05/04/2011
 *      Author: Matthew Cochrane
 *     Company: Wild Spy
 */

#ifndef NHDC160100DIZ_H_
#define NHDC160100DIZ_H_

#include <stdlib.h>
#include <avr/io.h>
#include <util/delay.h>
#include <avr/pgmspace.h>
#include "twi_master_driver.h"
//#include "MX_USART.h"
#include "../Version.h"

#define DELAYUS			0
#define SCLBIT 			1
#define SDABIT 			0
#define SDA_HI()		do {PORTC.OUTSET = (1<<SDABIT); _delay_us(DELAYUS);} while (0)
#define SDA_LO()		do {PORTC.OUTCLR = (1<<SDABIT); _delay_us(DELAYUS);} while (0)
#define SDA_IN()		do {PORTC.DIRCLR = (1<<SDABIT);} while (0)
#define SDA_OUT()		do {PORTC.DIRSET = (1<<SDABIT);} while (0)
#define SCL_HI()		do {PORTC.OUTSET = (1<<SCLBIT); _delay_us(DELAYUS);} while (0)
#define SCL_LO()		do {PORTC.OUTCLR = (1<<SCLBIT); _delay_us(DELAYUS);} while (0)


#define SCR_SETUP()		PORTB.DIRSET = (1<<0); PORTC.DIRSET = (1<<SDABIT)|(1<<SCLBIT)|(1<<4)|(1<<5)
#define SCR_BL_ON()		PORTB.OUTSET = (1<<0)
#define SCR_BL_OFF()	PORTB.OUTCLR = (1<<0)
#define SCR_RST_HI()	PORTC.OUTSET = (1<<5)
#define SCR_RST_LO()	PORTC.OUTCLR = (1<<5)
#define SCR_CS_HI()		PORTC.OUTSET = (1<<4)
#define SCR_CS_LO()		PORTC.OUTCLR = (1<<4)

#define BAUDRATE	100000
#define TWI_BAUDSETTING 55 	//TWI_BAUD(F_CPU, BAUDRATE)

#define SLAVEADD	0x7E	//0x3F
#define COMSEND 	0x00
#define DATASEND	0x40

/**********************************************************************************/
// LCD commands - use LCD_write function to write these commands to the LCD.
/**********************************************************************************/
#define LCD_ON  	    	0xAF    // Turn LCD display on
#define LCD_OFF  			0xAE    // Turn LCD display off
#define COLUMN_0_MSB		0x10	// Set column address to 0
#define COLUMN_0_LSB		0x00
#define LINE_0				0xB0	// Set Row address to 0
#define NORMAL      		1
#define INVERTED     		0
#define SPACE 				0
#define LOW					0
#define HIGH				1
#define PAGE_0				0xB0
#define P_0					0x00
#define P_1					0x01
#define P_2					0x02
#define P_3					0x03
#define P_4					0x04
#define P_5					0x05
#define P_6					0x06
#define P_7					0x07
#define P_8					0x08
#define P_9					0x09
#define P_10				0x0A
#define P_11				0x0B
#define P_12				0x0C
#define P_13				0x0D
#define P_14				0x0E
#define P_15				0x0F
#define LCD_COLS			160
#define	LCD_ROWS			100
#define LCD_LINES			12
#define LCD_ALIGN_CENTER	170
#define LCD_ALIGN_LEFT		0
#define LCD_ALIGN_RIGHT		171

//Macros
#define LCD_printf_P(Start_Col, str, ...)	sprintf_P(s, PSTR(str), __VA_ARGS__); LCD_DisplayString(Start_Col, s, NORMAL)
#define LCD_printf_Pos_P(Line, Col, str, ...)	sprintf_P(s, PSTR(str), __VA_ARGS__); LCD_PosSetUp(Line,0); LCD_DisplayString(Col, s, NORMAL)

void I2C_out(uint8_t data);
void I2C_Start( void );
void I2C_Stop( void );

void LCD_Init(void);
void LCD_InitDisplay( void );
char* char_conv(char conv_strng);

void LCD_write_Ctrl(unsigned char datum);
void LCD_OutputData(unsigned char Start_Col, int j,int nor_inv);
void LCD_write_Data(unsigned char value, int nor_inv);
void LCD_write_COLUMN(long value);
void LCD_PosSetUp(unsigned char Page, unsigned char column);
void LCD_Clear_Display(int start, int end);
void LCD_DisplayString(unsigned char Start_Col, char* value, int nor_inv);
void LCD_DisplayString_P(unsigned char Start_Col, char *value, int nor_inv);
void LCD_DisplayChar(unsigned char Start_Col, char value, int nor_inv);
void LCD_DisplaySpecialChar(unsigned char Start_Col, char value, int nor_inv);
void LCD_write_DataGS(uint32_t value);
void LCD_Display_Bitmap(uint8_t Row, uint8_t Col, char* Pic);
void LCD_Display_Graphic(char *lcd_string);
void LCD_Show_Display(char* lcd_string, int nor_inv);
void LCD_Print_Terminal(char* str);
void LCD_DispOn(uint8_t isOn);
void showSplash( void );


/*NO - REPLACE!!*/ //void Wait(int time);

void Show();	//delete
void scrTest( void );	//modify!

#endif /* NHDC160100DIZ_H_ */
