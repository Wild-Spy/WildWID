/*
 * MX_SendData.h
 *
 * Created: 22/07/2016 12:01 PM
 *  Author: Matthew Cochrane
 */ 


#ifndef MX_SENDDATA_H_
#define MX_SENDDATA_H_

#include <stdbool.h>
#include "MX_ErrorEnum.h"
#include "MX_DateTime.h"

#define SEND_DATA_GPRS_RETRIES_MAX		15
#define SBD_MIN_MSG_CHARGE	30 //ie if a message is < 30 bytes you still get charged for 30 bytes.

#define IRIDIUM_MESSAGE_TYPE_PICKUP_STATS	1
#define IRIDIUM_MESSAGE_TYPE_NETWORK_STATS	2
#define IRIDIUM_MESSAGE_TYPE_NETWORK_INFO	3 //to give names +Id's of nodes on network

//void SendData_Init(void);
bool SendData_DueForSend(RTC_Time_t* curTime);
bool SendData_CheckMonthDataRolledOver(RTC_Time_t* curTime);
mx_err_t SendData_CheckAndSend(RTC_Time_t* curTime);
mx_err_t SendData_SendIridiumSBDMessages(RTC_Time_t* curTime);

void SendData_ReelInTheYears(RTC_Time_t* curTime);
void SendData_AddToTimeUntilAfter(RTC_Time_t* curTime, RTC_Time_t* periodToAdd, RTC_Time_t* timeToModify);

#endif