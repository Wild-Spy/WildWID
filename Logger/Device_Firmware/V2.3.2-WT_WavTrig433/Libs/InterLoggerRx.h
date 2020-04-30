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
#include "LoggerLookupTable.h"

#define recent_inter_logger_rx_max 20
#define ILRX_RSSI_STATUS 50 //if rssi = this value then the message is a status message not a tag pickup, should never pickup +50dBm signal..
#define ILRX_TIME_SYNC_BROADCAST 51 //if rssi = this value then the message is a status message not a tag pickup, should never pickup +51dBm signal..
#define ILRX_TIME_SYNC_REQUEST 52 //if rssi = this value then the message is a status message not a tag pickup, should never pickup +51dBm signal..
#define ILRX_TIME_SYNC_RESPONSE 53 //if rssi = this value then the message is a status message not a tag pickup, should never pickup +51dBm signal..

typedef struct inter_logger_rx {
	uint32_t tagID;
	uint32_t loggerID;
	RTC_Time_t pickupTime;
	int8_t tagRSSI;
	uint8_t retransmitsRemaining;
} inter_logger_rx_t;

void interLoggerRecordStatus(LLUTStruct_t* info, RTC_Time_t* now);
bool interLoggerDecodeStatusPickup(uint8_t* buff, RTC_Time_t* mTime, uint16_t* batVolt, uint8_t* flash_usage, uint8_t* flags, uint32_t* loggerId, uint16_t* loggerGroup);

void interLoggerRecordTimeSync(RTC_Time_t* now);
bool interLoggerDecodeTimeSyncPickup(uint8_t* buff, RTC_Time_t* mTime, uint32_t* loggerId, uint16_t* loggerGroup);

void interLoggerRecordTimeSyncRequest(uint32_t seed);
void interLoggerRecordTimeSyncResponse(RTC_Time_t* now, uint32_t tagid);

void interLoggerRecordRx(uint32_t tagID, uint32_t loggerID, RTC_Time_t* pickupTime, int8_t tagRSSI);
void interLoggerRecordRxNoTx(uint32_t tagID, uint32_t loggerID, RTC_Time_t* pickupTime, int8_t tagRSSI);
uint8_t interLoggerRxExists(uint32_t tagID, uint32_t loggerID, RTC_Time_t* pickupTime, int8_t tagRSSI);

void interLoggerEncodePickup(uint8_t* buff, RTC_Time_t* mTime, uint32_t id, uint8_t rssi, uint32_t loggerID, uint16_t loggerGroup);
uint8_t interLoggerDecodePickup(uint8_t* buff, RTC_Time_t* mTime, uint32_t* id, uint8_t* rssi, uint32_t* loggerID, uint16_t* loggerGroup);

//Call this function in RTCWakeupChecks
//It is expected that this function is called once every second.
void interLoggerRxUpdate(const RTC_Time_t* now);

#endif // __INTER_LOGGER_RX_H__