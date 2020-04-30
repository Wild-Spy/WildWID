/*
 * Strings.h
 *
 *  Created on: 22/04/2011
 *      Author: Matthew Cochrane
 */

#ifndef STRINGS_H_
#define STRINGS_H_

#include <stdlib.h>
#include <avr/io.h>
#include <avr/pgmspace.h>

#define STR_PRODUCT_N			"WID Logger T"
#define STR_CODE_V				"V2.1.0"
#define STR_WILDSPY				"Wild Spy"
#define STR_NEWLINE				"\r\n"
#define DEVTYPE_ID				0x0001

extern const char PSTR_PRODUCT_N[];// PROGMEM = STR_PRODUCT_N;
extern const char PSTR_CODE_V[];// PROGMEM = STR_CODE_V;
extern const char PSTR_WILDSPY[];// PROGMEM = STR_WILDSPY;
extern const char PSTR_NEWLINE[];// PROGMEM = STR_NEWLINE;
extern uint16_t P_DEV_ID;// PROGMEM = DEV_ID;

//Function Prototypes:
uint8_t compareStringWithP(char* str, const char* Pstr);
int compareStartofStringWithP(char* str, const char* Pstr);
int stringCopy(char* srcStr, char* destStr);
int stringCopyP(const char* srcStrP, char* destStr);
void memCopy(uint8_t* src, uint8_t* dest, uint16_t len);
//yeah
#endif /* STRINGS_H_ */
