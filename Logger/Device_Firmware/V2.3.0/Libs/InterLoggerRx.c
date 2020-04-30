/*
 * File: WarningFlasher.h
 * Author: Matthew Cochrane
 * HW: XMEGA-A4
 */

#include "InterLoggerRx.h"
#include "SettingsManager.h"
#include <string.h>
#include "../WIDLogV2.h"

static inter_logger_rx_t recent_inter_logger_rx[recent_inter_logger_rx_max];
//on startup set to 0!
static uint8_t recent_inter_logger_rx_pos = 0;

static void interLoggerRetransmitRx(uint8_t array_pos);
static uint8_t interLoggerTxPickup(RTC_Time_t* mTime, uint32_t ID, uint8_t RSSI, uint32_t loggerID, uint16_t loggerGroup);

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

void interLoggerRxUpdate()
{
	for(uint8_t i = 0; i < recent_inter_logger_rx_max; ++i)
	{
		if (recent_inter_logger_rx[i].retransmitsRemaining)
		{
			//transmit
			interLoggerRetransmitRx(i);	
			--recent_inter_logger_rx[i].retransmitsRemaining;
		}
	}
}

uint8_t interLoggerRxExists(uint32_t tagID, uint32_t loggerID, RTC_Time_t* pickupTime, int8_t tagRSSI)
{
	for(uint8_t i = 0; i < recent_inter_logger_rx_max; ++i)
	{	
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

//Private...

static void interLoggerRetransmitRx(uint8_t array_pos)
{
	if (array_pos >= recent_inter_logger_rx_max) return;
	interLoggerTxPickup(&recent_inter_logger_rx[array_pos].pickupTime,
						recent_inter_logger_rx[array_pos].tagID,
						recent_inter_logger_rx[array_pos].tagRSSI,
						recent_inter_logger_rx[array_pos].loggerID,
						setting_LoggerGroupId);
}

static uint8_t interLoggerTxPickup(RTC_Time_t* mTime, uint32_t ID, uint8_t RSSI, uint32_t loggerID, uint16_t loggerGroup) {
	uint8_t buf[16];
	
	interLoggerEncodePickup(buf, mTime, ID, RSSI, loggerID, loggerGroup);

	cc110L_tx_packet(buf, 16);
	cc110L_strobe(TI_CCxxx0_SFRX); //flush rx fifo
	cc110L_strobe(TI_CCxxx0_SRX);
	clearWakeupSource(WAKEUP_SOURCE_CC110L);
	return 1;
}
