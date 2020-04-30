/* 
 * Author: Matthew Cochrane
 * Date: Nov 2015
 * Description: Wakeup Source
 * 
 */

#include "WakeupSource.h"

volatile uint8_t Wakeup_Source = WAKEUP_SOURCE_NONE;	//!<Holds information on what source woke up the device from sleep mode.