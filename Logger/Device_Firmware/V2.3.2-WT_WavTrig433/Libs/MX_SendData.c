/*
 * MX_SendData.c
 *
 * Created: 22/07/2016 12:01 PM
 *  Author: Matthew Cochrane
 */ 

#include "MX_SendData.h"
#include "SettingsManager.h"
#include "MX_FlashLogger.h"
#include "MX_USART.h"
#include "MX_Iridium9603.h"
#include "M_CountList.h"
#include "MX_DateTime.h"
#include "LoggerLookupTable.h"

extern char s[80];
//RTC_Time_t Send_Data_GPRS_Next;		//!<Next time to attempt to send an email
RTC_Time_t Send_Data_GPRS_Next_Real;	//!<Next scheduled time to send an email
//RTC_Time_t Send_Data_GPRS_Period;		//!<An email is scheduled to be sent once every period
uint8_t Send_Data_GPRS_Retries_Remaining = SEND_DATA_GPRS_RETRIES_MAX;	//!<Number of retries remaining before aborting this email and waiting for next scheduled send email time

bool SendData_DueForSend(RTC_Time_t* curTime) {
	return FirstTimeGreaterOrEqualTo(curTime, &Send_Data_GPRS_Next);
}

bool SendData_CheckMonthDataRolledOver(RTC_Time_t* curTime) {
	
	if (FirstTimeGreaterOrEqualTo(curTime, &setting_IridiumMonthNextReset)) {
		RTC_Time_t per1month;
		SET_RTC_TIME_T(per1month, 0,1,0,0,0,0);
		SendData_AddToTimeUntilAfter(curTime, &per1month, &setting_IridiumMonthNextReset);
		setting_IridiumBytesSentThisMonth = 0;
		SettingToEEPROM(IRIDIUMMONTHNXTRST);
		SettingToEEPROM(MONTHBYTESSENT);
		return true;
	}
	return false;
}

/*!	\brief This is a brief description
 *
 * So the basic gist of how this function works:
 * There are three important variables:
 * (A) Send_Data_GPRS_Next - This is the next time this section of code will be entered
 * (B) Send_Data_GPRS_Next_Real - This is the next 'scheduled' send-email time
 * (C) Send_Data_GPRS_Retries_Remaining - This is the number of send-email attempts left before
 *										  skipping this email and waiting for the next scheduled
 *										  email time to try again
 * 
 * The code will try and send an email up to (SEND_DATA_GPRS_RETRIES_MAX) times with 15? minutes wait
 * between attempts.  If the email send succeeds or we run out of attempts then the next time is set
 * to (B) which is the next scheduled time (one sendemailperiod after the last send_data_next_real).
 * (A) is set to 15? minutes after the current RTC time every time the section is entered (unless
 * it's the last attempt, then it's set to the next scheduled attempt).  This happens before the
 * sendEmail() subroutine is executed so that if the module goes into bat-backup mode during an email
 * send, the next time is still in the future.  More importantly, (3) is decremented before sendEmail()
 * is executed so that the module doesn't get stuck in an endless loop with the system restarting?
 * If the device enters bat-bak mode (battery removed) during an email send then, when the device 
 * wakes up again, if (A) is not at least 15? minutes after the current time, it is set to 15 minutes
 * after the current time and (B) is set to the next scheduled time after the current time.  This
 * means that the device won't wake up and send an email straight away but will wait at least 15?
 * minutes first (if the next wakeup time was before now or in the next 15 minutes).
 */
mx_err_t SendData_CheckAndSend(RTC_Time_t* curTime) {
	//TODO[ ]: this whole section OK???
	if (Send_Data_GPRS_Retries_Remaining == SEND_DATA_GPRS_RETRIES_MAX) {
		//Full on, Start of a real send...
		//CopyTime(&RTC_c_Time, &Send_Data_GPRS_Next_Real);
		//AddTimes(&Send_Data_GPRS_Next_Real, &Send_Data_GPRS_Period);
		SendData_ReelInTheYears(curTime);
		//Backup real next wakeup time
		WriteTimeEEPROM(&Send_Data_GPRS_Next_Real, NEXTGPRSSEND_ADD);
	}
	if (Send_Data_GPRS_Retries_Remaining > 1) {	
		//The > 1 is correct!! because we havent done the decrement yet
		//As long as it's not the very last attempt, set the Next time to 15 minutes after now (ie. next retry time).
		//We have to do this at the start of the function (before the sendemail call) because if the unit resets or goes into batbackup mode
		//during it then we want the next time to be set-up already.
		//Calculate new wakeup time 
		//TODO[ ]: set to 5 minutes or 10 minutes...
		//AddToTime(&Send_Data_GPRS_Next, 0, 0, 0, 0, 0, 30);
		TimeAfterTime(curTime, &Send_Data_GPRS_Next, 0, 0, 0, 0, 15, 0);
	} else { //Send_Data_GPRS_Retries_Remaining <= 1
		//If it is our last attempt, set the next time to the REAL next wakeup (ie 1 GPRS_send_period after the previous real wakeup time)
		//We have to do this at the start of the function (before the sendemail call) because if the unit resets or goes into batbackup mode
		//during it then we want the next time to be set-up already.
		CopyTime(&Send_Data_GPRS_Next_Real, &Send_Data_GPRS_Next);
	}
	//Decrement no. of attempts remaining (before the sendemail function)
	Send_Data_GPRS_Retries_Remaining--;
		
	sPrintDateTime(s, curTime, "Current Time");
	USART_tx_String(&USARTC0, s);
	sPrintDateTime(s, &Send_Data_GPRS_Next, "Next_GPRS");
	USART_tx_String(&USARTC0, s);
	sPrintDateTime(s, &Send_Data_GPRS_Next_Real, "Next_GPRS_Real");
	USART_tx_String(&USARTC0, s);
	USART_printf_P(&USARTC0, "Attempts: %u\r\n", SEND_DATA_GPRS_RETRIES_MAX-Send_Data_GPRS_Retries_Remaining);
		
	uint8_t send_success = 0;
	USART_tx_String_P(&USARTC0, PSTR("Turning Iridium On!\r\n"));

	SendData_CheckMonthDataRolledOver(curTime);
	//TODO[ ]: This section all looks good.. haven't tested the actual send email bit though
	if (Iridium_Unit_On() >= 0) {
		USART_tx_String_P(&USARTC0, PSTR("Sending MO Message.\r\n"));
		
		if (SendData_SendIridiumSBDMessages(curTime) == mx_err_Success)
		{
			send_success = 1;
			USART_tx_String_P(&USARTC0, PSTR("Send Data Success!\r\n"));
		}
		USART_printf_P(&USARTC0, "Bytes Sent This Month: %lu of %lu\r\n", setting_IridiumBytesSentThisMonth, setting_IridiumMonthlyByteLimit);
		USART_printf_P(&USARTC0, "Current Monthly Usage: %d%%\r\n", (int)((double)setting_IridiumBytesSentThisMonth/(double)setting_IridiumMonthlyByteLimit*100.0));
	}
	Iridium_Unit_Off();
		
	//Now that we're done sending...
	if (send_success == 0) {
		if (Send_Data_GPRS_Retries_Remaining == 0) {
			//if it was our last shot and it still failed, save the fail event to flash
			SAVE_SYS_MSG(curTime, SYS_MSG_SEND_EMAIL_FAIL, SEND_DATA_GPRS_RETRIES_MAX);
			USART_tx_String_P(&USARTC0, PSTR("Send Data Failed!\r\n"));
			Send_Data_GPRS_Retries_Remaining = SEND_DATA_GPRS_RETRIES_MAX;
		}
	} else {
		//Success! Save the event to flash
		SAVE_SYS_MSG(curTime, SYS_MSG_SENT_EMAIL, SEND_DATA_GPRS_RETRIES_MAX - Send_Data_GPRS_Retries_Remaining);
		//TODO[ ]: Test this!
		CopyTime(&Send_Data_GPRS_Next_Real, &Send_Data_GPRS_Next);
		Send_Data_GPRS_Retries_Remaining = SEND_DATA_GPRS_RETRIES_MAX;
		//GPRSSENDNEXTTOEEPROM();
		SettingToEEPROM(NEXTGPRSSEND);
		USART_tx_String_P(&USARTC0, PSTR("Send Data Succeeded!\r\n"));
	}
		
	//Debugging:
	sPrintDateTime(s, &Send_Data_GPRS_Next, "Next GPRS Send Time");
	USART_tx_String(&USARTC0, s);
	USART_tx_String_P(&USARTC0, PSTR("\r\n"));
	_delay_ms(2);
	
	return mx_err_InvalidData;
}

mx_err_t SendData_SendIridiumSBDMessages(RTC_Time_t* curTime)
{
	char buffer[340];
	int16_t len;
	RTC_Time_t tmpTime;
	uint32_t tmpID;
	//GM862_err_t a = SerCom_err_Success;
	uint8_t tmpFlags;
	int8_t RSSI;
	uint8_t Activity;
	uint32_t i;
	CountListStruct_t* listItem;
	uint8_t loggerShortId;
	uint32_t old_setting_LogsSentViaIridium = setting_LogsSentViaIridium;
	mx_err_t retVal;
	
	SettingsManager_LoadGPRSSettings();
	
// 	setting_IridiumBytesSentThisMonth
// 	setting_IridiumMonthlyByteLimit
// 	setting_LogsSentViaIridium
	
	//In terms of satellite data, I think number of detections since last transmission, which tags (not likely to be that many) and last tag detected and whether a sign was activated (unless the default to being with is to activate after any detection) would all be useful I think, provided data allowance isn't exceeded.
	
	//Message Format:
	//Index and Length both in bytes below
	/*     Index   | Length | Description
	 *           0 |      1 | Message Type 
	 *           1 |      2 | Number of detections since last successful transmission (if > 65535 then set to 65535)
	 *           3 |      2 | Total pickups in message stats (ie tags detected in last 24 hours) (if > 65525, set to 65535)
	 *           5 |      4 | Last tag ID detected
	 *           9 |      1 | Number of Messages to be sent
	 *           A |      1 | Message number of total
	 * For each tag detected since last transmission (t is counter)
	 *   11+(t*13) |      4 | Tag ID	
	 * 11+(t*13+4) |      2 | Tag pickups since last transmission
	 * 11+(t*13+6) |      1 | First Pickup time (in number of 15 min intervals since midnight)
	 * 11+(t*13+7) |      1 | Last Pickup time (in number of 15 min intervals since midnight)
	 * 11+(t*13+8) |      1 | MIN RSSI
	 * 11+(t*13+9) |      1 | Average RSSI
	 * 11+(t*13+A) |      1 | MAX RSSI
	 * 11+(t*13+B) |      2 | Pickup time mask (1 bit per 90 mins -> 1 for pickup in interval, 0 for no pickup in interval)
	 */

	#define PICKUPMSG_HEADER_LEN	11
	#define TAG_STAT_LEN			13
	
	uint32_t dets = RecordCount - setting_LogsSentViaIridium;
		
	//Message Type
	*((uint8_t*)(buffer+0)) = IRIDIUM_MESSAGE_TYPE_PICKUP_STATS;
	//Number of detections since last transmission
	*((uint16_t*)(buffer+1)) = (uint16_t)((dets > 0xFFFFL)?0xFFFFL:dets);
	*((uint16_t*)(buffer+3)) = 0;//(pickups_in_last_24_hrs > 0xFFFFL)?0xFFFFL:pickups_in_last_24_hrs;
	//Last tag ID detected
	tmpFlags = loadRecord(RecordCount-1, &tmpTime, &tmpID, (uint8_t*)&RSSI, &Activity, &loggerShortId);
	*((uint32_t*)(buffer+5)) = tmpID;
	*((uint8_t*)(buffer+9)) = 0; //total number of messages
	*((uint8_t*)(buffer+10)) = 1; //this is message #? out of total number of messages
	len = PICKUPMSG_HEADER_LEN;
	
	//Create the list
	//Determine start of day and end of day
	RTC_Time_t yesterday;
	CopyTime(curTime, &yesterday);
	SubtractSecondsFromTime(&yesterday, 60L*60L*24L);
	RTC_Time_t dayStart;
	RTC_Time_t dayStop;
	CopyTime(&yesterday, &dayStart);
	CopyTime(&yesterday, &dayStop);
	dayStart.Hour = 0; dayStart.Minute = 0; dayStart.Second = 0;
	dayStop.Hour = 23; dayStop.Minute = 59; dayStop.Second = 59;
	//Clear the list
	mCountList_Clear();
	//Loop through items
	for (i = setting_LogsSentViaIridium; i < RecordCount; ++i)
	{
		tmpFlags = loadRecord(i, &tmpTime, &tmpID, (uint8_t*)&RSSI, &Activity, &loggerShortId);
		//Only want tags from yesterday.  Ie if we do a transmission at 1:00am on 10/1/2016 then we want tags picked up between
		//00:00:00 on 9/1/2016 to 23:59:59 on 9/1/2016 (inclusive)
		if (TimeBetweenTimes(&tmpTime, &dayStart, &dayStop)) {
			if (mCountList_AddItem(tmpID, &tmpTime, RSSI) == mx_err_Overflow) break;
		}
		if (FirstTimeGreaterOrEqualTo(&tmpTime, &dayStop)) break; //don't want logssentviairidium to include logs from today, otherwise it will skip them later!
		//This is a little dangerous though.  If we somehow got a corrupt time in the logs and it was way in the future we would then never send another message!
	}
	setting_LogsSentViaIridium = i;
	
	*((uint16_t*)(buffer+3)) = (mCountList_GetItemsAdded() > 0xFFFFL)?0xFFFFL:mCountList_GetItemsAdded();
	
	uint8_t logs_per_message = (IRIDIUM_SBD_MESSAGE_LEN_MAX-PICKUPMSG_HEADER_LEN)/TAG_STAT_LEN;
	uint8_t total_messages = mCountList_GetCount()==0?1:(mCountList_GetCount()-1)/logs_per_message+1;
	
	*((uint8_t*)(buffer+9)) = total_messages; //total number of messages
	
	//Start iridium connection
	if (Iridium_SendMO_Setup() != Iridium_err_Success) goto abort_tx;
	
	uint8_t current_message;
	uint16_t recid = 0;
	for (current_message = 1; current_message <= total_messages; ++current_message) {
		*((uint8_t*)(buffer+10)) = current_message; //this is message #? out of total number of messages
		len = PICKUPMSG_HEADER_LEN;
		//Put the summary into the message
		uint16_t first_i_of_message_part = recid;
		while (recid < mCountList_GetCount())
		{
			//if (i > (IRIDIUM_SBD_MESSAGE_LEN_MAX-PICKUPMSG_HEADER_LEN)/TAG_STAT_LEN) break;
			if (len+TAG_STAT_LEN > IRIDIUM_SBD_MESSAGE_LEN_MAX) break;
		
			if (mCountList_Item(recid, &listItem) == mx_err_Success)
			{
				uint8_t ind = recid-first_i_of_message_part;
				//Tag ID
				*((uint32_t*)(buffer+(PICKUPMSG_HEADER_LEN+ind*TAG_STAT_LEN))) = listItem->ID;
				//Number of Pickups
				*((uint16_t*)(buffer+(PICKUPMSG_HEADER_LEN+ind*TAG_STAT_LEN+4))) = listItem->count;
				//First Pickup Time (in # of 15 min blocks since midnight)
				*((uint8_t*)(buffer+(PICKUPMSG_HEADER_LEN+ind*TAG_STAT_LEN+6))) = listItem->firstTime;
				//Last Pickup Time (in # of 15 min blocks since midnight)
				*((uint8_t*)(buffer+(PICKUPMSG_HEADER_LEN+ind*TAG_STAT_LEN+7))) = listItem->lastTime;
				*((int8_t*)(buffer+(PICKUPMSG_HEADER_LEN+ind*TAG_STAT_LEN+8))) = listItem->rssiMin;
				*((int8_t*)(buffer+(PICKUPMSG_HEADER_LEN+ind*TAG_STAT_LEN+9))) = listItem->rssiAvg;
				*((int8_t*)(buffer+(PICKUPMSG_HEADER_LEN+ind*TAG_STAT_LEN+10))) = listItem->rssiMax;
				*((uint16_t*)(buffer+(PICKUPMSG_HEADER_LEN+ind*TAG_STAT_LEN+11))) = listItem->pickupTimeMask;
				len += TAG_STAT_LEN;
			}
			++recid;
		}

		if (setting_IridiumBytesSentThisMonth + len > setting_IridiumMonthlyByteLimit) {
			if (current_message > 1) {
				SettingToEEPROM(IRIDIUMLOGSSENT);
				SettingToEEPROM(MONTHBYTESSENT);
			}
			retVal = mx_err_InsufficientResources;
			goto abort_tx;
		}
	
		//Try and send the message -> 3 attempts.
		i = 3;
		while (i) {
			if (Iridium_SendMO_SendMessage(buffer, len) == Iridium_err_Success) break;
			--i;
		}
	
		if (i == 0) {
			if (current_message > 1) {
				SettingToEEPROM(MONTHBYTESSENT);
			}
			retVal = Iridium_err_BadResponse;
			goto abort_tx;
		}
	
		//Add charge to settings
		if (len < SBD_MIN_MSG_CHARGE)
		{
			setting_IridiumBytesSentThisMonth += SBD_MIN_MSG_CHARGE;
		}
		else
		{
			setting_IridiumBytesSentThisMonth += len;
		}
	}
	
	//Message Format:
	//Index and Length both in bytes below
	/*    Index   | Length | Description
	 *          0 |      1 | Message Type
	 *          1 |      1 | Number of loggers in group
	 *          2 |      1 | Number of Messages to be sent
	 *          3 |      1 | Message number of total
	 * For each tag detected since last transmission (t is counter)
	 *  4+(t*8+0) |      4 | Logger ID	
	 *  4+(t*8+4) |      2 | Battery Voltage
	 *  4+(t*8+6) |      1 | Flash Usage
	 *  4+(t*8+7) |      1 | Flags
	 */
	
	#define LGRSTATUSMSG_HEADER_LEN	4
	#define LGRSTATUS_LEN			8
	
	//Message Type
	*((uint8_t*)(buffer+0)) = IRIDIUM_MESSAGE_TYPE_NETWORK_STATS;
	*((uint8_t*)(buffer+1)) = LLUT_GetCount(); //Number of loggers in group
	*((uint8_t*)(buffer+2)) = 0; //total number of messages
	*((uint8_t*)(buffer+3)) = 1; //this is message #? out of total number of messages
	len = LGRSTATUSMSG_HEADER_LEN;
	
	logs_per_message = (IRIDIUM_SBD_MESSAGE_LEN_MAX-LGRSTATUSMSG_HEADER_LEN)/LGRSTATUS_LEN; // = 42 -> so many!
	total_messages = (LLUT_GetCount()-1)/logs_per_message+1;
	LLUTStruct_t* firstLlutItem = LLUT_GetDataPtr();
	
	*((uint8_t*)(buffer+2)) = total_messages; //total number of messages
	
	current_message;
	recid = 0;
	for (current_message = 1; current_message <= total_messages; ++current_message) {
		*((uint8_t*)(buffer+3)) = current_message; //this is message #? out of total number of messages
		len = LGRSTATUSMSG_HEADER_LEN;
		//Put the summary into the message
		uint16_t first_i_of_message_part = recid;
		while (recid < LLUT_GetCount())
		{
			//if (i > (IRIDIUM_SBD_MESSAGE_LEN_MAX-PICKUPMSG_HEADER_LEN)/TAG_STAT_LEN) break;
			if (len+LGRSTATUS_LEN > IRIDIUM_SBD_MESSAGE_LEN_MAX) break;
			
			LLUTStruct_t* item = firstLlutItem + recid;
			
			uint8_t ind = recid-first_i_of_message_part;
			*((uint32_t*)(buffer+(LGRSTATUSMSG_HEADER_LEN+ind*LGRSTATUS_LEN))) = item->ID; //Logger ID
			*((uint16_t*)(buffer+(LGRSTATUSMSG_HEADER_LEN+ind*LGRSTATUS_LEN+4))) = item->bat_volt;
			*((uint8_t*)(buffer+(LGRSTATUSMSG_HEADER_LEN+ind*LGRSTATUS_LEN+6))) = item->flash_usage;
			*((uint8_t*)(buffer+(LGRSTATUSMSG_HEADER_LEN+ind*LGRSTATUS_LEN+7))) = item->flags;
			len += LGRSTATUS_LEN;
			
			++recid;
		}

		if (setting_IridiumBytesSentThisMonth + len > setting_IridiumMonthlyByteLimit) {
			if (current_message > 1) {
				SettingToEEPROM(IRIDIUMLOGSSENT);
				SettingToEEPROM(MONTHBYTESSENT);
			}
			retVal = mx_err_InsufficientResources;
			goto abort_tx;
		}
		
		//Try and send the message -> 3 attempts.
		i = 3;
		while (i) {
			if (Iridium_SendMO_SendMessage(buffer, len) == Iridium_err_Success) break;
			--i;
		}
		
		//if we retried 3 times above and it still didn't work..
		if (i == 0) {
			if (current_message > 1) {
				SettingToEEPROM(MONTHBYTESSENT);
			}
			retVal = Iridium_err_BadResponse;
			goto abort_tx;
		}
		
		//Add charge to settings
		if (len < SBD_MIN_MSG_CHARGE)
		{
			setting_IridiumBytesSentThisMonth += SBD_MIN_MSG_CHARGE;
		}
		else
		{
			setting_IridiumBytesSentThisMonth += len;
		}
	}
	
	Iridium_SendMO_Finish();
	SettingToEEPROM(IRIDIUMLOGSSENT);
	SettingToEEPROM(MONTHBYTESSENT);
	return mx_err_Success;
	
abort_tx:
	Iridium_SendMO_Finish();
	setting_LogsSentViaIridium = old_setting_LogsSentViaIridium;
	//SettingToEEPROM(MONTHBYTESSENT);
	return retVal;
}

/*!	\brief Finds the next scheduled send email time.
 *
 *	Keeps adding #Send_Data_GPRS_Period to #Send_Data_GPRS_Next until
 *	#Send_Data_GPRS_Next is after the current RTC time (#RTC_c_Time).
 *	This has nothing to do with the send email retry attempts.  It is
 *	only modifying the next scheduled real send data time.
 *
 *	\sa AddTimes() and FirstTimeGreaterOrEqualTo()
 */
void SendData_ReelInTheYears(RTC_Time_t* curTime) {
	sPrintDateTime(s, &Send_Data_GPRS_Next_Real, "Next_GPRS_Real(RIY1)");
	USART_tx_String(&USARTC0, s);
	//Timed and tested: each iteration of this loop takes approx 18us to complete
	//(Did 1557952 iterations in about 27 seconds)
	while (FirstTimeGreaterOrEqualTo(curTime, &Send_Data_GPRS_Next_Real)) {
		//CopyTime(&RTC_c_Time, &Send_Data_GPRS_Next_Real);
		AddTimes(&Send_Data_GPRS_Next_Real, &Send_Data_GPRS_Period);
	} 
	sPrintDateTime(s, &Send_Data_GPRS_Next_Real, "Next_GPRS_Real(RIY2)");
	USART_tx_String(&USARTC0, s);
}

void SendData_AddToTimeUntilAfter(RTC_Time_t* curTime, RTC_Time_t* periodToAdd, RTC_Time_t* timeToModify) {
	sPrintDateTime(s, &timeToModify, "(RIY1a)");
	USART_tx_String(&USARTC0, s);
	//Timed and tested: each iteration of this loop takes approx 18us to complete
	//(Did 1557952 iterations in about 27 seconds)
	while (FirstTimeGreaterOrEqualTo(curTime, timeToModify)) {
		//CopyTime(&RTC_c_Time, &Send_Data_GPRS_Next_Real);
		AddTimes(timeToModify, periodToAdd);
	}
	sPrintDateTime(s, timeToModify, "(RIY2a)");
	USART_tx_String(&USARTC0, s);
}