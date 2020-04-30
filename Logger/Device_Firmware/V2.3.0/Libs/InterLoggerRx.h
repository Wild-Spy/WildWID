/*
 * File: WarningFlasher.h
 * Author: Matthew Cochrane
 * HW: XMEGA-A4
 */

#ifndef __INTER_LOGGER_RX_H__
#define __INTER_LOGGER_RX_H__

#include <stdint.h>
#include <stdbool.h>
#include "MX_RTC.h"

#define recent_inter_logger_rx_max 10

typedef struct inter_logger_rx {
	uint32_t tagID;
	uint32_t loggerID;
	RTC_Time_t pickupTime;
	int8_t tagRSSI;
	uint8_t retransmitsRemaining;
} inter_logger_rx_t;

void interLoggerRecordRx(uint32_t tagID, uint32_t loggerID, RTC_Time_t* pickupTime, int8_t tagRSSI);
uint8_t interLoggerRxExists(uint32_t tagID, uint32_t loggerID, RTC_Time_t* pickupTime, int8_t tagRSSI);

void interLoggerEncodePickup(uint8_t* buff, RTC_Time_t* mTime, uint32_t id, uint8_t rssi, uint32_t loggerID, uint16_t loggerGroup);
uint8_t interLoggerDecodePickup(uint8_t* buff, RTC_Time_t* mTime, uint32_t* id, uint8_t* rssi, uint32_t* loggerID, uint16_t* loggerGroup);

//Call this function in RTCWakeupChecks
//It is expected that this function is called once every second.
void interLoggerRxUpdate();

#endif // __INTER_LOGGER_RX_H__