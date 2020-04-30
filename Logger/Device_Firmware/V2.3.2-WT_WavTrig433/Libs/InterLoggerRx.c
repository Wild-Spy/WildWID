/*
 * File: WarningFlasher.h
 * Author: Matthew Cochrane
 * HW: XMEGA-A4
 */

#include "InterLoggerRx.h"
#include "SettingsManager.h"
#include <string.h>
#include "../WIDLogV2.h"
#include "wdt_driver.h"

static inter_logger_rx_t recent_inter_logger_rx[recent_inter_logger_rx_max];
//on startup set to 0!
static uint8_t recent_inter_logger_rx_pos = 0;

static void interLoggerRetransmitRx(uint8_t array_pos, const RTC_Time_t* now);
static uint8_t interLoggerTxPickup(const RTC_Time_t* mTime, uint32_t ID, uint8_t RSSI, uint32_t loggerID, uint16_t loggerGroup);

static unsigned long Seed; 
unsigned long NextVal();

/* Call before first use of NextVal */
unsigned long InitSeed()
{
   //Your code for random seed here

   // Correct distribution errors in seed
   NextVal();
   NextVal();
   NextVal();
   return NextVal();
}

 /* Linear Congruential Generator 
  * Constants from  
  * "Numerical Recipes in C" 
  * by way of 
   * <http://en.wikipedia.org/wiki/Linear_congruential_generator#LCGs_in_common_use>
   * Note: Secure implementations may want to get uncommon/new LCG values
  */
unsigned long NextVal()
{
  Seed=Seed*1664525L+1013904223L;
  return Seed;
} 

void interLoggerRecordRx(uint32_t tagID, uint32_t loggerID, RTC_Time_t* pickupTime, int8_t tagRSSI)
{
	recent_inter_logger_rx_pos += 1;
	if (recent_inter_logger_rx_pos >= recent_inter_logger_rx_max) recent_inter_logger_rx_pos = 0;
	
	recent_inter_logger_rx[recent_inter_logger_rx_pos].tagID = tagID;
	recent_inter_logger_rx[recent_inter_logger_rx_pos].loggerID = loggerID;
	CopyTime(pickupTime, &recent_inter_logger_rx[recent_inter_logger_rx_pos].pickupTime);
	recent_inter_logger_rx[recent_inter_logger_rx_pos].tagRSSI = tagRSSI;
	recent_inter_logger_rx[recent_inter_logger_rx_pos].retransmitsRemaining = 3;
}

void interLoggerRecordRxNoTx(uint32_t tagID, uint32_t loggerID, RTC_Time_t* pickupTime, int8_t tagRSSI)
{
	recent_inter_logger_rx_pos += 1;
	if (recent_inter_logger_rx_pos >= recent_inter_logger_rx_max) recent_inter_logger_rx_pos = 0;
	
	recent_inter_logger_rx[recent_inter_logger_rx_pos].tagID = tagID;
	recent_inter_logger_rx[recent_inter_logger_rx_pos].loggerID = loggerID;
	CopyTime(pickupTime, &recent_inter_logger_rx[recent_inter_logger_rx_pos].pickupTime);
	recent_inter_logger_rx[recent_inter_logger_rx_pos].tagRSSI = tagRSSI;
	recent_inter_logger_rx[recent_inter_logger_rx_pos].retransmitsRemaining = 0;
}
void interLoggerRecordStatus(LLUTStruct_t* info, RTC_Time_t* now) {
	recent_inter_logger_rx_pos += 1;
	if (recent_inter_logger_rx_pos >= recent_inter_logger_rx_max) recent_inter_logger_rx_pos = 0;
	
	recent_inter_logger_rx[recent_inter_logger_rx_pos].tagID = (((uint32_t)info->bat_volt)<<16)|(((uint32_t)info->flash_usage)<<8)|info->flags;
	recent_inter_logger_rx[recent_inter_logger_rx_pos].loggerID = info->ID;
	CopyTime(now, &recent_inter_logger_rx[recent_inter_logger_rx_pos].pickupTime);
	recent_inter_logger_rx[recent_inter_logger_rx_pos].tagRSSI = ILRX_RSSI_STATUS;
	recent_inter_logger_rx[recent_inter_logger_rx_pos].retransmitsRemaining = 3;
}

void interLoggerRecordTimeSync(RTC_Time_t* now) {
	recent_inter_logger_rx_pos += 1;
	if (recent_inter_logger_rx_pos >= recent_inter_logger_rx_max) recent_inter_logger_rx_pos = 0;
	
	recent_inter_logger_rx[recent_inter_logger_rx_pos].tagID = now->Second | ((uint32_t)now->Minute)<<8 | ((uint32_t)now->Hour)<<16 | ((uint32_t)now->Day)<<24;
	recent_inter_logger_rx[recent_inter_logger_rx_pos].loggerID = setting_LoggerId;
	CopyTime(now, &recent_inter_logger_rx[recent_inter_logger_rx_pos].pickupTime);
	recent_inter_logger_rx[recent_inter_logger_rx_pos].tagRSSI = ILRX_TIME_SYNC_BROADCAST;
	recent_inter_logger_rx[recent_inter_logger_rx_pos].retransmitsRemaining = 3;
}

void interLoggerRecordTimeSyncRequest(uint32_t seed) {
	recent_inter_logger_rx_pos += 1;
	if (recent_inter_logger_rx_pos >= recent_inter_logger_rx_max) recent_inter_logger_rx_pos = 0;
	
	Seed = seed;
	InitSeed();
	
	recent_inter_logger_rx[recent_inter_logger_rx_pos].tagID = Seed;
	recent_inter_logger_rx[recent_inter_logger_rx_pos].loggerID = setting_LoggerId;
	//CopyTime(now, &recent_inter_logger_rx[recent_inter_logger_rx_pos].pickupTime);
	recent_inter_logger_rx[recent_inter_logger_rx_pos].tagRSSI = ILRX_TIME_SYNC_REQUEST;
	recent_inter_logger_rx[recent_inter_logger_rx_pos].retransmitsRemaining = 3;
}

void interLoggerRecordTimeSyncResponse(RTC_Time_t* now, uint32_t tagid) {
	recent_inter_logger_rx_pos += 1;
	if (recent_inter_logger_rx_pos >= recent_inter_logger_rx_max) recent_inter_logger_rx_pos = 0;
	
	recent_inter_logger_rx[recent_inter_logger_rx_pos].tagID = tagid;
	recent_inter_logger_rx[recent_inter_logger_rx_pos].loggerID = setting_LoggerId;
	CopyTime(now, &recent_inter_logger_rx[recent_inter_logger_rx_pos].pickupTime);
	recent_inter_logger_rx[recent_inter_logger_rx_pos].tagRSSI = ILRX_TIME_SYNC_RESPONSE;
	recent_inter_logger_rx[recent_inter_logger_rx_pos].retransmitsRemaining = 3;
}

void interLoggerRxUpdate(const RTC_Time_t* now)
{
	for(uint8_t i = 0; i < recent_inter_logger_rx_max; ++i)
	{
		WDT_RESET();
		if (recent_inter_logger_rx[i].retransmitsRemaining)
		{
			//transmit
			interLoggerRetransmitRx(i, now);	
			--recent_inter_logger_rx[i].retransmitsRemaining;
		}
	}
}

uint8_t interLoggerRxExists(uint32_t tagID, uint32_t loggerID, RTC_Time_t* pickupTime, int8_t tagRSSI)
{
	if (tagRSSI == ILRX_TIME_SYNC_RESPONSE) {
		for(uint8_t i = 0; i < recent_inter_logger_rx_max; ++i)
		{
			if (recent_inter_logger_rx[i].tagRSSI == ILRX_TIME_SYNC_REQUEST && 
				recent_inter_logger_rx[i].loggerID != 0 &&
				recent_inter_logger_rx[i].tagID == tagID ) {
				//For time sync messages the time will change, we just want the rest of the packet to be the same.
				//Also, we only want to sync to one response, so set the loggerID to 0 now to ignore future responses
				recent_inter_logger_rx[i].loggerID = 0;
				return 1;
			}
		}
		return 0;
	}
	
	for(uint8_t i = 0; i < recent_inter_logger_rx_max; ++i)
	{	
		if (recent_inter_logger_rx[i].tagRSSI == tagRSSI && tagRSSI == ILRX_TIME_SYNC_BROADCAST &&
			recent_inter_logger_rx[i].loggerID == loggerID &&
			recent_inter_logger_rx[i].tagID == tagID) {
			//For time sync messages the time will change, we just want the rest of the packet to be the same.
			return 1;
		}
		if (recent_inter_logger_rx[i].tagID == tagID &&
			recent_inter_logger_rx[i].loggerID == loggerID &&
			recent_inter_logger_rx[i].tagRSSI == tagRSSI &&
			// and times are equal
			FirstTimeGreaterOrEqualTo(&recent_inter_logger_rx[i].pickupTime, pickupTime) == 2 )
		{
			return 1;
		}
	}
	
	return 0;
}

//Buffer needs to be at least 16 bytes long
void interLoggerEncodePickup(uint8_t* buff, RTC_Time_t* mTime, uint32_t id, uint8_t rssi, uint32_t loggerID, uint16_t loggerGroup) {
	uint8_t tZip[5];
	uint8_t flags = 0x03;

	ZipTime(mTime, tZip, flags);
	//	   dest				source			bytes
	memcpy(buff,			tZip,			5    );
	memcpy(buff+5,			&id,			4    );
	memcpy(buff+5+4,		&rssi,			1    );
	memcpy(buff+5+4+1,		&loggerID,		4    );
	memcpy(buff+5+4+1+4,	&loggerGroup,	2    );
}

//Return FLG (FLG0 in  bit 0, FLG1 in bit 1)
uint8_t interLoggerDecodePickup(uint8_t* buff, RTC_Time_t* mTime, uint32_t* id, uint8_t* rssi, uint32_t* loggerID, uint16_t* loggerGroup) {
	uint8_t tZip[5];

	ClearTime(mTime);
	//	   dest         source			bytes
	memcpy(tZip,		buff,			5    );
	memcpy(id,			buff+5,			4    );
	memcpy(rssi,		buff+5+4,		1    );
	memcpy(loggerID,	buff+5+4+1,		4    );
	memcpy(loggerGroup,	buff+5+4+1+4,	2    );

	return UnzipTime(mTime, tZip);
}

bool interLoggerDecodeStatusPickup(uint8_t* buff, RTC_Time_t* mTime, uint16_t* batVolt, uint8_t* flash_usage, uint8_t* flags, uint32_t* loggerId, uint16_t* loggerGroup) {
	uint8_t tZip[5];
	volatile int8_t rssi;
	uint32_t id;

	
	//	   dest         source			bytes
	memcpy(&rssi,		buff+5+4,		1    );
	
	if (rssi != ILRX_RSSI_STATUS) {
		return false;
	}
	
	ClearTime(mTime);
	memcpy(tZip,		buff,			5    );
	memcpy(&id,			buff+5,			4    );
	*batVolt = (uint16_t)((id>>16)&0x0000FFFFL);
	*flash_usage = (uint16_t)((id>>8)&0x000000FFL);
	*flags = (uint16_t)(id&0x000000FFL);
	//...
	memcpy(loggerId,	buff+5+4+1,		4    );
	memcpy(loggerGroup,	buff+5+4+1+4,	2    );

	UnzipTime(mTime, tZip);
	
	return true;
}


bool interLoggerDecodeTimeSyncPickup(uint8_t* buff, RTC_Time_t* mTime, uint32_t* loggerId, uint16_t* loggerGroup) {
	uint8_t tZip[5];
	volatile int8_t rssi;

	//	   dest         source			bytes
	memcpy(&rssi,		buff+5+4,		1    );
	
	if (rssi != ILRX_TIME_SYNC_BROADCAST) {
		return false;
	}

	ClearTime(mTime);	
	memcpy(tZip,		buff,			5    );
	//...
	memcpy(loggerId,	buff+5+4+1,		4    );
	memcpy(loggerGroup,	buff+5+4+1+4,	2    );

	UnzipTime(mTime, tZip);
	
	return true;
}

//Private...

static void interLoggerRetransmitRx(uint8_t array_pos, const RTC_Time_t* now)
{
	if (array_pos >= recent_inter_logger_rx_max) return;
	if (recent_inter_logger_rx[array_pos].tagRSSI == ILRX_TIME_SYNC_BROADCAST || recent_inter_logger_rx[array_pos].tagRSSI == ILRX_TIME_SYNC_RESPONSE) {
		if (now->Year == 10) return; //time hasn't been set so don't send invalid sync.
		interLoggerTxPickup(now, //it's a time sync message so tx our current time!
							recent_inter_logger_rx[array_pos].tagID,
							recent_inter_logger_rx[array_pos].tagRSSI,
							recent_inter_logger_rx[array_pos].loggerID,
							setting_LoggerGroupId);
	} else if (recent_inter_logger_rx[array_pos].tagRSSI == ILRX_TIME_SYNC_REQUEST) {
		interLoggerTxPickup(now,
							recent_inter_logger_rx[array_pos].tagID,
							ILRX_TIME_SYNC_REQUEST,  //
							recent_inter_logger_rx[array_pos].loggerID,
							setting_LoggerGroupId);
	} else {
		interLoggerTxPickup(&recent_inter_logger_rx[array_pos].pickupTime,
							recent_inter_logger_rx[array_pos].tagID,
							recent_inter_logger_rx[array_pos].tagRSSI,
							recent_inter_logger_rx[array_pos].loggerID,
							setting_LoggerGroupId);
	}
}

static uint8_t interLoggerTxPickup(const RTC_Time_t* mTime, uint32_t ID, uint8_t RSSI, uint32_t loggerID, uint16_t loggerGroup) {
	uint8_t buf[16];
	
	interLoggerEncodePickup(buf, mTime, ID, RSSI, loggerID, loggerGroup);

	cc110L_tx_packet(buf, 16);
	cc110L_strobe(TI_CCxxx0_SFRX); //flush rx fifo
	cc110L_strobe(TI_CCxxx0_SRX);
	clearWakeupSource(WAKEUP_SOURCE_CC110L);
	return 1;
}
