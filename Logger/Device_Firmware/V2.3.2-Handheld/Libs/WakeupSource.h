/* 
 * Author: Matthew Cochrane
 * Date: Nov 2015
 * Description: Wakeup Source
 * 
 */

#include <stdint.h>

#define WAKEUP_SOURCE_NONE				0
#define WAKEUP_SOURCE_RTC				0b00000001
#define WAKEUP_SOURCE_USART				0b00000010
#define WAKEUP_SOURCE_CC2500			0b00000100
#define WAKEUP_SOURCE_PWR				0b00001000
#define WAKEUP_SOURCE_KP				0b00010000
#define WAKEUP_SOURCE_SOFT_OFF			0b00100000
#define WAKEUP_SOURCE_CC110L			0b01000000

extern volatile uint8_t Wakeup_Source;	//!<Holds information on what source woke up the device from sleep mode.