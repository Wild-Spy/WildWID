/*
 * Strings.c
 *
 *  Created on: 24/05/2011
 *      Author: Matthew Cochrane
 */

#include "Strings.h"

const char PSTR_PRODUCT_N[] PROGMEM = STR_PRODUCT_N;
const char PSTR_CODE_V[] PROGMEM = STR_CODE_V;
const char PSTR_WILDSPY[] PROGMEM = STR_WILDSPY;
const char PSTR_NEWLINE[] PROGMEM = STR_NEWLINE;

//Return 0 = strings match, 1 = strings don't match
uint8_t compareStringWithP(char* str, const char* Pstr) {
	char cN, cP;
	//if (len == 0) len = 255;
	do {
		cN = *(str++);
		cP = pgm_read_byte(Pstr++);
		//len--;
	} while (cN == cP && cN != '\0'); //&& len);
	
	if (cN == cP) {
		//0 = success
		return 0;
	} else {
		//1 = failure
		return 1;
	}
}

//Return (length of matching part of string) = strings match, -1 = strings don't match
//note that if the Pstring terminates and all previous characters matched we considder this a success.
int compareStartofStringWithP(char* str, const char* Pstr) {
	char cN, cP;
	uint8_t len = 0;
	do {
		cN = *(str++);
		cP = pgm_read_byte(Pstr++);
		len++;
	} while (cN == cP && cP != '\0');
	
	if (cP == '\0') {
		//len = success
		return len-1; //(removing the '\0')
	} else {
		//-1 = failure
		return -1;
	}
}

int stringCopy(char* srcStr, char* destStr) {
	int cnt = 0;
	
	do 	{
		*destStr++ = *srcStr;
		cnt++;
	} while (*srcStr++ != '\0');
	
	return cnt;
}

int stringCopyP(const char* srcStrP, char* destStr) {
	int cnt = 0;
	char tmpChar;
	
	do 	{
		tmpChar = pgm_read_byte(srcStrP++);
		*destStr++ = tmpChar;
		cnt++;
	} while (tmpChar != '\0');
	
	return cnt;
}

void memCopy(uint8_t* src, uint8_t* dest, uint16_t len) {

	while (len) {
		*dest++ = *src++;
		len--;
	};

}