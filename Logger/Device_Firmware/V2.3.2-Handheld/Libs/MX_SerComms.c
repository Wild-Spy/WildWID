/*
 * MX_SerComms.c
 *
 * Created: 17/08/2011 1:57:56 PM
 *  Author: MC
 */ 

#include "MX_SerComms.h"
#include "CC2500.h"
#include "crc32.h"
#include "MX_FlashLogger.h"
#include "LoggerLookupTable.h"
#include "../WIDLogV2.h"

extern char s[80];
extern uint32_t RecordCount;					//No. of records in dataflash
extern RTC_Time_t RTC_c_Time;					//RTC time -> real time stored here.
extern uint8_t myRF_Chan;
extern uint8_t myRF_ID[5];
extern char setting_Filter[8];
extern RTC_Time_t Send_Data_GPRS_Period;		//Send GPRS data every period
extern RTC_Time_t Send_Data_GPRS_Next;			//Next time to send data
extern RTC_Time_t Send_Data_GPRS_Next_Real;		//Next time to send data
extern char setting_DeviceName[20];
extern uint8_t TagDispMode;						//How tags are displayed (show ID only, ID and date/time or ID, Date/Time and Address)
extern uint32_t records_sent_already;
extern uint8_t Send_Data_GPRS_Retries_Remaining;
extern uint8_t EnterMobCommsBridgeMode;			
extern uint8_t EnterMobCommsConfigMode;	
extern char setting_EMAILTOADD[60];
extern char setting_GPRSAPN[30];
extern char setting_GPRSUSERID[30];
extern char setting_GPRSPASSW[30];
extern uint16_t setting_LoggerGroupId;
extern uint32_t setting_LoggerId;
extern uint16_t setting_WarningSignalDuration;
extern RTC_Time_t setting_IridiumMonthNextReset;

/************************************************************************/
/*                     Top Segment Starts here                          */
/************************************************************************/

//Serial Command Handler Prototypes:
SerCom_err_t Ser_CMD_Get_DevTypeID(USART_t* m_USART);
SerCom_err_t Ser_CMD_Get_AllRecordsFast(USART_t* m_USART);
SerCom_err_t Ser_CMD_Cmd_SaveDummy_Record(USART_t* m_USART);
SerCom_err_t Ser_CMD_Get_EntryAtAddress(USART_t* m_USART);
SerCom_err_t Ser_CMD_Cmd_EraseDataflash(USART_t* m_USART);
SerCom_err_t Ser_CMD_Cmd_PollDataFlashErased(USART_t* m_USART);
SerCom_err_t Ser_CMD_Get_Version(USART_t* m_USART);
SerCom_err_t Ser_CMD_Get_RecordCount(USART_t* m_USART);
SerCom_err_t Ser_CMD_Set_RecordCount(USART_t* m_USART);
SerCom_err_t Ser_CMD_Get_DateTime(USART_t* m_USART);
SerCom_err_t Ser_CMD_Set_DateTime(USART_t* m_USART);
SerCom_err_t Ser_CMD_Get_RFChan(USART_t* m_USART);
SerCom_err_t Ser_CMD_Set_RFChan(USART_t* m_USART);
SerCom_err_t Ser_CMD_Get_RFID(USART_t* m_USART);
SerCom_err_t Ser_CMD_Set_RFID(USART_t* m_USART);
SerCom_err_t Ser_CMD_Get_RFFilter(USART_t* m_USART);
SerCom_err_t Ser_CMD_Set_RFFilter(USART_t* m_USART);
SerCom_err_t Ser_CMD_Get_SendDataGPRSPeriod(USART_t* m_USART);
SerCom_err_t Ser_CMD_Set_SendDataGPRSPeriod(USART_t* m_USART);
SerCom_err_t Ser_CMD_Get_SendDataGPRSNext(USART_t* m_USART);
SerCom_err_t Ser_CMD_Set_SendDataGPRSNext(USART_t* m_USART);
SerCom_err_t Ser_CMD_Get_LoggerName(USART_t* m_USART);
SerCom_err_t Ser_CMD_Set_LoggerName(USART_t* m_USART);
SerCom_err_t Ser_CMD_Get_VerboseMode(USART_t* m_USART);
SerCom_err_t Ser_CMD_Set_VerboseMode(USART_t* m_USART);
SerCom_err_t Ser_CMD_Cmd_GM862(USART_t* m_USART);
SerCom_err_t Ser_CMD_Cmd_GM862BridgeMode(USART_t* m_USART);
SerCom_err_t Ser_CMD_Get_GPRSVals(USART_t* m_USART);
SerCom_err_t Ser_CMD_Set_GPRSVals(USART_t* m_USART);
SerCom_err_t Ser_CMD_Get_EmailToAddress(USART_t* m_USART);
SerCom_err_t Ser_CMD_Set_EmailToAddress(USART_t* m_USART);
SerCom_err_t Ser_CMD_Cmd_UpdateTimeFromGSM(USART_t* m_USART);
SerCom_err_t Ser_CMD_Get_SendEmailAttempts(USART_t* m_USART);
SerCom_err_t Ser_CMD_Set_SendEmailAttempts(USART_t* m_USART);
SerCom_err_t Ser_CMD_Cmd_GM862_ConfigModule(USART_t* m_USART);
SerCom_err_t Ser_CMD_Get_RFParams(USART_t* m_USART);
SerCom_err_t Ser_CMD_Set_RFParams(USART_t* m_USART);
SerCom_err_t Ser_CMD_Get_LoggerGroupId(USART_t* m_USART);
SerCom_err_t Ser_CMD_Set_LoggerGroupId(USART_t* m_USART);
SerCom_err_t Ser_CMD_Get_LoggerId(USART_t* m_USART);
SerCom_err_t Ser_CMD_Get_WarningSignalDuration(USART_t* m_USART);
SerCom_err_t Ser_CMD_Set_WarningSignalDuration(USART_t* m_USART);
SerCom_err_t Ser_CMD_Get_InterLogRFParams(USART_t* m_USART);
SerCom_err_t Ser_CMD_Set_InterLogRFParams(USART_t* m_USART);
SerCom_err_t Ser_CMD_Get_LoggerTable(USART_t* m_USART);
SerCom_err_t Ser_CMD_Get_IridiumDataMonthNextReset(USART_t* m_USART);
SerCom_err_t Ser_CMD_Set_IridiumDataMonthNextReset(USART_t* m_USART);
SerCom_err_t Ser_CMD_Get_MonthByteLimit(USART_t* m_USART);
SerCom_err_t Ser_CMD_Set_MonthByteLimit(USART_t* m_USART);

//Serial Command Handler Array Defs:
const Ser_CMD_Handler Ser_CMDAry[] PROGMEM =	{{0x00, Ser_CMD_Get_DevTypeID},
										 {0x01, Ser_CMD_Get_AllRecordsFast},
										 {0x02, Ser_CMD_Cmd_SaveDummy_Record},
										 {0x03, Ser_CMD_Get_EntryAtAddress},
										 {0x04, Ser_CMD_Cmd_EraseDataflash},
										 {0x05, Ser_CMD_Cmd_PollDataFlashErased},
										 {0x06, Ser_CMD_Get_Version},
										 {0x07, Ser_CMD_Get_RecordCount},
										 {0x08, Ser_CMD_Set_RecordCount},
										 {0x09, Ser_CMD_Get_DateTime},
										 {0x0A, Ser_CMD_Set_DateTime},
										 {0x0B, Ser_CMD_Get_RFChan},
										 {0x0C, Ser_CMD_Set_RFChan},
										 {0x0D, Ser_CMD_Get_RFID},
										 {0x0E, Ser_CMD_Set_RFID},
										 {0x0F, Ser_CMD_Get_RFFilter},
										 {0x10, Ser_CMD_Set_RFFilter},
										 {0x11, Ser_CMD_Get_SendDataGPRSPeriod},
										 {0x12, Ser_CMD_Set_SendDataGPRSPeriod},
										 {0x13, Ser_CMD_Get_SendDataGPRSNext},
										 {0x14, Ser_CMD_Set_SendDataGPRSNext},
										 {0x15, Ser_CMD_Get_LoggerName},
										 {0x16, Ser_CMD_Set_LoggerName},
										 {0x17, Ser_CMD_Get_VerboseMode},
										 {0x18, Ser_CMD_Set_VerboseMode},
										 {0x19, Ser_CMD_Cmd_GM862},
										 {0x1A, Ser_CMD_Cmd_GM862BridgeMode},
										 {0x1B, Ser_CMD_Get_GPRSVals},
										 {0x1C, Ser_CMD_Set_GPRSVals},
										 {0x1D, Ser_CMD_Get_EmailToAddress},
										 {0x1E, Ser_CMD_Set_EmailToAddress},
										 {0x1F, Ser_CMD_Cmd_UpdateTimeFromGSM},
										 {0x20, Ser_CMD_Get_SendEmailAttempts},
										 {0x21, Ser_CMD_Set_SendEmailAttempts},
										 {0x22, Ser_CMD_Cmd_GM862_ConfigModule},
										 {0x23, Ser_CMD_Get_RFParams},
										 {0x24, Ser_CMD_Set_RFParams},
										 {0x25, Ser_CMD_Get_LoggerGroupId},
										 {0x26, Ser_CMD_Set_LoggerGroupId},
										 {0x27, Ser_CMD_Get_LoggerId},
										 {0x28, Ser_CMD_Get_WarningSignalDuration},
										 {0x29, Ser_CMD_Set_WarningSignalDuration},
										 {0x30, Ser_CMD_Get_InterLogRFParams},	
										 {0x31, Ser_CMD_Set_InterLogRFParams},
										 {0x32, Ser_CMD_Get_LoggerTable},
										 {0x33, Ser_CMD_Get_IridiumDataMonthNextReset},
										 {0x34, Ser_CMD_Set_IridiumDataMonthNextReset},
										 {0x35, Ser_CMD_Get_MonthByteLimit},
										 {0x36, Ser_CMD_Set_MonthByteLimit}};	
#define SERCOM_NUMCMDS					  0x34 + 1
		
/************************************************************************/
/*                       Top Segment Ends here                          */
/************************************************************************/

SerCom_err_t Ser_CMDHandler(USART_t* m_USART) {
	uint8_t CMDByte;
	SerCom_err_t tmpSerComErr;
	uint8_t i;
	CMD_Handler_t CMD_Handler;
		
	//Receive Command Byte
	tmpSerComErr = Ser_RxSeg(m_USART, 1, &CMDByte);
	if (tmpSerComErr < 0) 
		return tmpSerComErr;
	
	tmpSerComErr = SerCom_err_NoKnownCmd;
	//Lookup Command in table and execute handler function
	for (i = 0; i < SERCOM_NUMCMDS; i++) {
		if (pgm_read_byte(&Ser_CMDAry[i].CMDByte) == CMDByte) {
			
			CMD_Handler = (CMD_Handler_t)pgm_read_word(&Ser_CMDAry[i].Handler);
			if ((tmpSerComErr = CMD_Handler(m_USART)) < 0) {
				return tmpSerComErr;
			}
			//Only execute one function!
			break;
		}
	}
	
	tmpSerComErr = SerCom_FinishSeg(m_USART);
	
	return tmpSerComErr;
	
}

//Rx from PC (to device)
//Should not be used with large 'len' or will run out of memory.
SerCom_err_t Ser_RxSeg(USART_t* m_USART, uint16_t len, uint8_t* DataDest) {
	uint8_t Attempts = 5;
	uint8_t tmpBuf[len+2];
	uint8_t tmpByte;
	SerCom_err_t tmpSerComErr;
	USART_err_t tmpUSARTErr;
	uint16_t i;
	uint16_t* RxChkSum;
	uint16_t CalcChkSum = 0;
	
	//uint16_t len1 = len;
	
	RxChkSum = (uint16_t*)(&tmpBuf[len]);			//TODO[ ]: Check that this line works!!
	
	//(1) Transmit Rx Segment identifier and listen for Ack
	USART_tx_Byte(m_USART, SERCOM_RXSEG_ID);
	if ((tmpSerComErr = SerCom_RxAck(m_USART)) < 0) return tmpSerComErr;
	
	//(3) Transmit Segment length (2 bytes) and listen for Ack
	USART_tx_Byte(m_USART, ByteNo(&len,0));
	USART_tx_Byte(m_USART, ByteNo(&len,1));
	if ((tmpSerComErr = SerCom_RxAck(m_USART)) < 0) return tmpSerComErr;
	
	do {
		//Receive (5)data + (6)checksum
		for (i = 0; i < len+2; i++) {
			
			//If there was an error code returned from the USART_rx function, abort
			if ((tmpUSARTErr = USART_rx_Byte(m_USART, SERCOM_TIMEOUT, &tmpByte)) < 0) return SerCom_err_Timeout;
			
			//Add to buffer
			tmpBuf[i] = tmpByte;
		}
	
		//(7) Calculate our version of checksum
		CalcChkSum = 0;
		for (i = 0; i < len; i++) {
			//CalcChkSum += tmpBuf[i];
			CalcChkSum = _crc16_update(CalcChkSum, tmpBuf[i]);
		}
		
		//(7) Compare checksums
		if (CalcChkSum == *RxChkSum) {
			//checksums match... success!
			//TODO[ ]: Maybe have an option here to disable interrupts on copy? eg. a cli() before then sei() after.
			for (i = 0; i < len; i++) {
				DataDest[i] = tmpBuf[i];
			}				
			Attempts = 0;
		} else {
			//Invalid checksum, retry/abort
			Attempts--;
			if (Attempts) {
				//Still attempts left, ask to resend
				USART_tx_Byte(m_USART, SERCOM_RESP_RESEND);
				if ((tmpSerComErr = SerCom_RxAck(m_USART)) < 0) return tmpSerComErr;
			} else {
				//Sent 'Attempt's times.. abort!
				USART_tx_Byte(m_USART, SERCOM_RESP_ABORT);
				if ((tmpSerComErr = SerCom_RxAck(m_USART)) < 0) return tmpSerComErr;
				return SerCom_err_Abort;
			}
		}
	} while (Attempts);
	
	USART_tx_Byte(m_USART, SERCOM_RESP_OK);
	if ((tmpSerComErr = SerCom_RxAck(m_USART)) < 0) return tmpSerComErr;
	
	return SerCom_err_Success;
}

//Tx from device to PC
SerCom_err_t Ser_TxSeg(USART_t* m_USART, uint16_t len, uint8_t* DataSrc) {
	uint8_t Attempts = 5;
	SerCom_err_t tmpSerComErr;
	USART_err_t tmpUSARTErr;
	uint16_t i;
	uint16_t RxChkSum;
	uint16_t CalcChkSum = 0;
	uint8_t tmpByte;
	
	//(1) Transmit Tx Segment identifier and listen for Ack
	USART_tx_Byte(m_USART, SERCOM_TXSEG_ID);
	if ((tmpSerComErr = SerCom_RxAck(m_USART)) < 0) return tmpSerComErr;
	
	//(3) Transmit Segment length (2 bytes) and listen for Ack
	USART_tx_Byte(m_USART, ByteNo(&len,0));
	USART_tx_Byte(m_USART, ByteNo(&len,1));
	if ((tmpSerComErr = SerCom_RxAck(m_USART)) < 0) return tmpSerComErr;
	
	do {
		
		//(5) Transmit data
		CalcChkSum = 0;
		for (i = 0; i < len; i++) {			
			USART_tx_Byte(m_USART, DataSrc[i]);
			//CalcChkSum += DataSrc[i];
			CalcChkSum = _crc16_update(CalcChkSum, DataSrc[i]);
		}
	
		//(6) Receive checksum
		if ((tmpUSARTErr = USART_rx_Byte(m_USART, SERCOM_TIMEOUT, &tmpByte)) < 0) return SerCom_err_Timeout;
		ByteNo(&RxChkSum, 0) = tmpByte;
		if ((tmpUSARTErr = USART_rx_Byte(m_USART, SERCOM_TIMEOUT, &tmpByte)) < 0) return SerCom_err_Timeout;
		ByteNo(&RxChkSum, 1) = tmpByte;
		
		//(7) Compare checksums
		if (CalcChkSum == RxChkSum) {
			//checksums match... success!
			Attempts = 0;
		} else {
			//Invalid checksum, retry/abort
			Attempts--;
			if (Attempts) {
				//Still attempts left, ask to resend
				USART_tx_Byte(m_USART, SERCOM_RESP_RESEND);
				if ((tmpSerComErr = SerCom_RxAck(m_USART)) < 0) return tmpSerComErr;
			} else {
				//Sent 'Attempt's times.. abort!
				USART_tx_Byte(m_USART, SERCOM_RESP_ABORT);
				if ((tmpSerComErr = SerCom_RxAck(m_USART)) < 0) return tmpSerComErr;
				return SerCom_err_Abort;
			}
		}
	} while (Attempts);
	
	USART_tx_Byte(m_USART, SERCOM_RESP_OK);
	if ((tmpSerComErr = SerCom_RxAck(m_USART)) < 0) return tmpSerComErr;
	
	return SerCom_err_Success;
}

SerCom_err_t SerCom_FinishSeg(USART_t* m_USART) {
	//(1) Transmit Finish Segment identifier and listen for Ack
	USART_tx_Byte(m_USART, SERCOM_FINSEG_ID);
	return SerCom_RxAck(m_USART);
}	

SerCom_err_t SerCom_RxAck(USART_t* m_USART) {
	uint8_t tmpByte;
	USART_err_t tmpUSARTErr;
	
	//If there was an error code returned or we did not receive an ACK, abort!
	if ((tmpUSARTErr = USART_rx_Byte(m_USART, SERCOM_TIMEOUT, &tmpByte)) < 0) {
		return SerCom_err_Timeout;
	} else if (tmpByte != SERCOM_PC_ACK) {
		return SerCom_err_Nak;
	}
	
	return SerCom_err_Success;
}

/************************************************************************/
/*			 	  Device Serial CMD functions go here!					*/
/*                          (bottom part)								*/
/************************************************************************/

SerCom_err_t Ser_CMD_Get_AllRecordsFast(USART_t* m_USART) {
	//decided not to do using TxSeg's because the current method loads one record from
	//the flash memory IC and then transmits it, the TxSeg method would have too high overheads!
	
	uint8_t tmpByte;
	uint8_t tmpFlag;
	SerCom_err_t tmpSerComErr;
	uint32_t ii;
	uint16_t j;
	RTC_Time_t tmpTime;
	uint32_t tmpID;
	uint8_t RSSI;
	uint32_t recStart;
	uint32_t recEnd;
	int bsel;
	int8_t bscale;
	uint8_t Activity;
	uint8_t loggerShortId;
	
	
	//Transmit RecordCount
	if ((tmpSerComErr = Ser_TxSeg(m_USART, 4, (uint8_t*)&RecordCount) < 0)) {
		return tmpSerComErr;
	}
	
	//Receive Starting Record
	if ((tmpSerComErr = Ser_RxSeg(m_USART, 4, (uint8_t*)&recStart)) < 0) {
		return tmpSerComErr;
	}
	
	//Receive Ending Record
	if ((tmpSerComErr = Ser_RxSeg(m_USART, 4, (uint8_t*)&recEnd)) < 0) {
		return tmpSerComErr;
	}
	
	//Receive BaudRate bsel
	if ((tmpSerComErr = Ser_RxSeg(m_USART, 2, (uint8_t*)&bsel)) < 0) {
		return tmpSerComErr;
	}
	
	//Receive BaudRate bscale
	if ((tmpSerComErr = Ser_RxSeg(m_USART, 1, (uint8_t*)&bscale)) < 0) {
		return tmpSerComErr;
	}
	
	//RX Records, Faster, CRC checksum.
	//Fast clock!
	SetClkSpeed(CLK_SPD_32MHz);
	//Reconfigure UART (still 115200baud)
	//USART_Setup(&PORTC, &USARTC0, 524, -5, 0);
	//Reconfigure UART (460800baud)
	//USART_Setup(&PORTC, &USARTC0, 107, -5, 0);
	//Reconfigure UART (921600baud)
	//USART_Setup(&PORTC, m_USART, 75, -6, 0); ************************ last had value
	//Reconfigure UART (2000000baud) (2.0Mbaud)
	//USART_Setup(&PORTC, &USARTC0, 2, -1, 1);
	
	//Reconfigure USART (custom baud rate)
	USART_Setup(&PORTC, m_USART, bsel, bscale, 0);
	//SERCOM_TIMEOUT
	//wait for 'READY'
	tmpSerComErr = USART_rx_Byte(m_USART, 4000 , &tmpByte);
	if ((tmpByte == 'g') && (tmpSerComErr == SerCom_err_Success)) {
		int recPerGrp = 10000;
		uint8_t k = 0;
		uint16_t CheckSUM = 0xFFFF;
		for (k = 0; k < 4; k++)
			USART_tx_Byte(m_USART, *(((uint8_t*)&RecordCount)+k));
		for (ii = 0; ii < RecordCount; ) {
			CheckSUM = 0xFFFF;
			if (ii+recPerGrp > RecordCount)
				recPerGrp = RecordCount-ii;
			USART_tx_Byte(m_USART, *(((uint8_t*)&recPerGrp)+0));
			USART_tx_Byte(m_USART, *(((uint8_t*)&recPerGrp)+1));
			for (j = 0; j < recPerGrp; j++) {
				if (USART_USB_IN) {
					tmpFlag = loadRecord(ii+j, &tmpTime, &tmpID, &RSSI, &Activity, &loggerShortId);
					//send data and update CRC
					//Time (6 bytes)
					for (k = 0; k < 6; k++) {
						USART_tx_Byte(m_USART, *(((uint8_t*)&tmpTime)+k));
						CheckSUM = _crc16_update(CheckSUM, *(((uint8_t*)&tmpTime)+k));
					}
					//ID (4 bytes)
					for (k = 0; k < 4; k++) {
						//Corrupt some data! (for testing.. it worked!! perfectly!)
						//kkkk = *(((uint8_t*)&tmpTime)+k);
						//if ((j == 76) && (recPerGrp == 10000 || recPerGrp == 5000)) {
						//kkkk += 0x2C;
						//USART_tx_Byte(&USARTC0, *(((uint8_t*)&tmpID)+k)+0x2C);
						//} else {
						USART_tx_Byte(m_USART, *(((uint8_t*)&tmpID)+k));
						//}

						CheckSUM = _crc16_update(CheckSUM, *(((uint8_t*)&tmpID)+k));
					}
					//Flags (1 byte)
					USART_tx_Byte(m_USART, tmpFlag);
					CheckSUM = _crc16_update(CheckSUM, tmpFlag);
					//RSSI (1 byte)
					USART_tx_Byte(m_USART, RSSI);
					CheckSUM = _crc16_update(CheckSUM, RSSI);
					//Activity (1 byte)
					USART_tx_Byte(m_USART, Activity);
					CheckSUM = _crc16_update(CheckSUM, Activity);
					//Activity (1 byte)
					USART_tx_Byte(m_USART, loggerShortId);
					CheckSUM = _crc16_update(CheckSUM, loggerShortId);
				} else {
					//If USB is not plugged in, abort!
					ii = RecordCount;
				}
			}
			//increment ii!
			ii+=recPerGrp;
			for (k = 0; k < 2; k++)
				USART_tx_Byte(m_USART, *(((uint8_t*)&CheckSUM)+k));
			tmpSerComErr = USART_rx_Byte(m_USART, SERCOM_TIMEOUT, &tmpByte);
			if (tmpSerComErr == SerCom_err_Success) {
				switch (tmpByte) {
				case 'g': //good
					break;
				case 'r': //resend
					ii-=recPerGrp;
					//send less data
					recPerGrp = recPerGrp/2;
					//only one packet per group? -> abort!
					if (recPerGrp < 2)
						//abort
						ii = RecordCount;
					break;
				case 'c': //cancel
					ii = RecordCount;
					break;
				}
			}
		}
	}
	//Regular Clock
	SetClkSpeed(CLK_SPD_12MHz_SWITCH);
	//Reconfigure UART
	USART_Setup(&PORTC, m_USART, 176, -5, 0);
	
	_delay_ms(200);

	return SerCom_err_Success;//Ser_<Rx/Tx>Seg(m_USART, _DATA_LEN_, (uint8_t*)&VAR_Get_AllRecordsFast);

}

SerCom_err_t Ser_CMD_Cmd_SaveDummy_Record(USART_t* m_USART) {
	
	SerCom_err_t tmpSerComErr;
	uint32_t tmpAdd = 0;
	uint32_t tmpID;
	RTC_Time_t tmpTime;
	uint8_t tmpFlags;
	int8_t tmpRSSI;
	uint8_t tmpActivity;
	uint8_t tmpLoggerShortId;
	
	//Receive ID
	if ((tmpSerComErr = Ser_RxSeg(m_USART, 4, (uint8_t*)&tmpID)) < 0) {
		return tmpSerComErr;
	}
	
	//Receive Time
	if ((tmpSerComErr = Ser_RxSeg(m_USART, 6, (uint8_t*)&tmpTime)) < 0) {
		return tmpSerComErr;
	}
	
	//Receive Flags
	if ((tmpSerComErr = Ser_RxSeg(m_USART, 1, &tmpFlags)) < 0) {
		return tmpSerComErr;
	}
	
	//Receive RSSI
	if ((tmpSerComErr = Ser_RxSeg(m_USART, 1,  (uint8_t*)&tmpRSSI)) < 0) {
		return tmpSerComErr;
	}
	
	//Receive Activity
	if ((tmpSerComErr = Ser_RxSeg(m_USART, 1,  &tmpActivity)) < 0) {
		return tmpSerComErr;
	}
	
	//Receive Logger Short Id
	if ((tmpSerComErr = Ser_RxSeg(m_USART, 1,  &tmpLoggerShortId)) < 0) {
		return tmpSerComErr;
	}
	
	if (RecordToFlash(&tmpTime, tmpID, tmpFlags, (uint8_t)tmpRSSI, tmpActivity, tmpLoggerShortId) == 1) {
		tmpAdd = RecordCount;
	} else {
		//Just to say that it's garbage, return 0xFFFFFFFF
		tmpAdd = 0xFFFFFFFF;//UINT32_MAX;
	}
	
	//Send Recorded Address
	if ((tmpSerComErr = Ser_TxSeg(m_USART, 4, (uint8_t*)&tmpAdd)) < 0) {
		return tmpSerComErr;
	}

	return SerCom_err_Success;
}

SerCom_err_t Ser_CMD_Get_EntryAtAddress(USART_t* m_USART) {
	SerCom_err_t tmpSerComErr;
	uint32_t tmpAdd = 0;
	uint32_t tmpID;
	RTC_Time_t tmpTime;
	uint8_t tmpFlags;
	uint8_t tmpRSSI;
	uint8_t tmpActivity;
	uint8_t tmpLoggerShortID;
	
	if ((tmpSerComErr = Ser_RxSeg(m_USART, 4, (uint8_t*)&tmpAdd)) < 0) {
		return tmpSerComErr;
	}
	
	tmpFlags = loadRecord(tmpAdd, &tmpTime, &tmpID, &tmpRSSI, &tmpActivity, &tmpLoggerShortID);
	
	//Send ID
	if ((tmpSerComErr = Ser_TxSeg(m_USART, 4, (uint8_t*)&tmpID)) < 0) {
		return tmpSerComErr;
	}
	
	//Send Time
	if ((tmpSerComErr = Ser_TxSeg(m_USART, 6, (uint8_t*)&tmpTime)) < 0) {
		return tmpSerComErr;
	}
	
	//Send Flags
	if ((tmpSerComErr = Ser_TxSeg(m_USART, 1, &tmpFlags)) < 0) {
		return tmpSerComErr;
	}
	
	//Send RSSI
	if ((tmpSerComErr = Ser_TxSeg(m_USART, 1, &tmpRSSI)) < 0) {
		return tmpSerComErr;
	}
	
	//Send Activity
	if ((tmpSerComErr = Ser_TxSeg(m_USART, 1, &tmpActivity)) < 0) {
		return tmpSerComErr;
	}
		
	//Send Activity
	if ((tmpSerComErr = Ser_TxSeg(m_USART, 1, &tmpLoggerShortID)) < 0) {
		return tmpSerComErr;
	}
	return SerCom_err_Success;

}

SerCom_err_t Ser_CMD_Cmd_EraseDataflash(USART_t* m_USART) {
// 	SerCom_err_t tmpSerComErr;
// 	uint8_t SendTimeout = 0;	//
// 	
// 
// 	if ((tmpSerComErr = Ser_TxSeg(m_USART, 1, &tmpFlags)) < 0) {
// 		return tmpSerComErr;
// 	}
	
	//Erase DataFlash
	SerFlash_ChipErase(0x74);
	SerFlash_WaitRDY();
	RecordCount = 0;
	//RCTOEEPROM();
	SettingToEEPROM(RECCOUNT);
	records_sent_already = 0L;
	//mList_Init();
	LLUT_ClearList();
	setting_LogsSentViaIridium = 0;
 	return 0;

}

SerCom_err_t Ser_CMD_Cmd_PollDataFlashErased(USART_t* m_USART) {
	return 0;//Ser_<Rx/Tx>Seg(m_USART, _DATA_LEN_, (uint8_t*)&VAR_Cmd_PollDataFlashErased);

}

SerCom_err_t Ser_CMD_Get_Version(USART_t* m_USART) {
	char ss[100];
	char s1[20];
	char s2[20];
	
	stringCopyP(PSTR_PRODUCT_N, s1);
	stringCopyP(PSTR_CODE_V, s2);
	sprintf_P(ss, PSTR("%s - %s"), s1, s2);
	return Ser_TxSeg(m_USART, 100, (uint8_t*)ss);
}

SerCom_err_t Ser_CMD_Get_RecordCount(USART_t* m_USART) {
	return Ser_TxSeg(m_USART, 4, (uint8_t*)&RecordCount);
}

SerCom_err_t Ser_CMD_Set_RecordCount(USART_t* m_USART) {
	return Ser_RxSeg(m_USART, 4, (uint8_t*)&RecordCount);
}

SerCom_err_t Ser_CMD_Get_DateTime(USART_t* m_USART) {
	return Ser_TxSeg(m_USART, 6, (uint8_t*)&RTC_c_Time);
}

SerCom_err_t Ser_CMD_Set_DateTime(USART_t* m_USART) {
	SerCom_err_t tmpSerComErr;
	RTC_Time_t tmpTime;
	
	//Use a buffer...
	
	if ((tmpSerComErr = Ser_RxSeg(m_USART, 6, (uint8_t*)&tmpTime)) < 0) {
		return tmpSerComErr;
	}
	
	SAVE_SYS_MSG(&RTC_c_Time, SYS_MSG_PING, 0);
	CopyTime(&tmpTime, &RTC_c_Time);
	SAVE_SYS_MSG(&RTC_c_Time, SYS_MSG_UPDATED_TIME, SYS_MSG_UPDATE_TIME_SRC_PC);
	return tmpSerComErr;
}

SerCom_err_t Ser_CMD_Get_RFChan(USART_t* m_USART) {
	uint8_t A;
	return Ser_TxSeg(m_USART, 1, &A);
}

SerCom_err_t Ser_CMD_Set_RFChan(USART_t* m_USART) {
	SerCom_err_t tmpErr;
	uint8_t A;
	tmpErr = Ser_RxSeg(m_USART, 1, &A);
	if (tmpErr == SerCom_err_Success) {
// 		RFCHANTOEEPROM();
// 		NRF_POWERDOWN;
// 		NRF24L01_SPI_Setup();
// 		NRF24L01_SetupnRF24L01(myRF_Chan, myRF_ID);
// 		NRF24L01_Flush_RX_FIFO();
// 		NRF24L01_W_Reg(NREG_STATUS, 0x40);
// 		NRF_RXMODE;
	}
	return tmpErr;
}

SerCom_err_t Ser_CMD_Get_RFID(USART_t* m_USART) {
	uint8_t tmpA[5];
	return Ser_TxSeg(m_USART, 5, tmpA);
}

SerCom_err_t Ser_CMD_Set_RFID(USART_t* m_USART) {
	SerCom_err_t tmpErr;
	uint8_t tmpA[5];
	tmpErr = Ser_RxSeg(m_USART, 5, tmpA);
	if (tmpErr == SerCom_err_Success) {
// 		RFIDTOEEPROM();
// 		NRF_POWERDOWN;
// 		NRF24L01_SPI_Setup();
// 		NRF24L01_SetupnRF24L01(myRF_Chan, myRF_ID);
// 		NRF24L01_Flush_RX_FIFO();
// 		NRF24L01_W_Reg(NREG_STATUS, 0x40);
// 		NRF_RXMODE;
	}
	return tmpErr;
}

SerCom_err_t Ser_CMD_Get_RFFilter(USART_t* m_USART) {
	return Ser_TxSeg(m_USART, 8, (uint8_t*)&setting_Filter);

}

SerCom_err_t Ser_CMD_Set_RFFilter(USART_t* m_USART) {
	SerCom_err_t tmpErr;
	tmpErr = Ser_RxSeg(m_USART, 8, (uint8_t*)&setting_Filter);
	if (tmpErr == SerCom_err_Success) {
		//WRITEFILTEREEPROM();
		SettingToEEPROM(FILTER0);
		FilterStr2Mask(setting_Filter);
	}
	return tmpErr;
}

SerCom_err_t Ser_CMD_Get_SendDataGPRSPeriod(USART_t* m_USART) {
	return Ser_TxSeg(m_USART, 6, (uint8_t*)&Send_Data_GPRS_Period);

}

SerCom_err_t Ser_CMD_Set_SendDataGPRSPeriod(USART_t* m_USART) {
	SerCom_err_t tmpErr;
	tmpErr = Ser_RxSeg(m_USART, 6, (uint8_t*)&Send_Data_GPRS_Period);
	if (tmpErr == SerCom_err_Success) {
		//GPRSSENDPERTOEEPROM();
		SettingToEEPROM(SENDGPRSPER);
		//Update logs sent via Iridium -> only send records saved from now on.
		setting_LogsSentViaIridium = RecordCount;
		SettingToEEPROM(IRIDIUMLOGSSENT);
	}
	return tmpErr;

}

SerCom_err_t Ser_CMD_Get_SendDataGPRSNext(USART_t* m_USART) {
	return Ser_TxSeg(m_USART, 6, (uint8_t*)&Send_Data_GPRS_Next_Real);

}

SerCom_err_t Ser_CMD_Set_SendDataGPRSNext(USART_t* m_USART) {
	SerCom_err_t tmpErr;
	tmpErr = Ser_RxSeg(m_USART, 6, (uint8_t*)&Send_Data_GPRS_Next);
	if (tmpErr == SerCom_err_Success) {
		Send_Data_GPRS_Retries_Remaining = SEND_DATA_GPRS_RETRIES_MAX;
		CopyTime(&Send_Data_GPRS_Next, &Send_Data_GPRS_Next_Real);
		//GPRSSENDNEXTTOEEPROM();
		SettingToEEPROM(NEXTGPRSSEND);
		//Update logs sent via Iridium -> only send records saved from now on.
		setting_LogsSentViaIridium = RecordCount;
		SettingToEEPROM(IRIDIUMLOGSSENT);
	}
	return tmpErr;
}

SerCom_err_t Ser_CMD_Get_LoggerName(USART_t* m_USART) {
	return Ser_TxSeg(m_USART, 20, (uint8_t*)&setting_DeviceName);
}

SerCom_err_t Ser_CMD_Set_LoggerName(USART_t* m_USART) {
	SerCom_err_t tmpErr;
	tmpErr = Ser_RxSeg(m_USART, 20, (uint8_t*)&setting_DeviceName);
	if (tmpErr == SerCom_err_Success) {
		//TODO[ ]: test this code!
		//calculate the new device Id
		uint8_t len = strlen(setting_DeviceName);
		if (len > 20) len = 20;
		
		setting_LoggerId = crc32(0x12345678, setting_DeviceName, len);
		SettingToEEPROM(LOGGERID);
		
		SettingToEEPROM(DEVICENAME);
	}
	return tmpErr;
}

SerCom_err_t Ser_CMD_Get_VerboseMode(USART_t* m_USART) {
	return Ser_TxSeg(m_USART, 1, (uint8_t*)&TagDispMode);
}

SerCom_err_t Ser_CMD_Set_VerboseMode(USART_t* m_USART) {
	return Ser_RxSeg(m_USART, 1, (uint8_t*)&TagDispMode);
}

SerCom_err_t Ser_CMD_Cmd_GM862(USART_t* m_USART) {
	return 0;//Ser_RxSeg(m_USART, _DATA_LEN_, (uint8_t*)&VAR_Cmd_GM862);
}

SerCom_err_t Ser_CMD_Get_DevTypeID(USART_t* m_USART) {
	uint16_t DevTypeID = DEVTYPE_ID;
	
	return Ser_TxSeg(m_USART, 2, (uint8_t*)&DevTypeID);
}

SerCom_err_t Ser_CMD_Cmd_GM862BridgeMode(USART_t* m_USART) {
	EnterMobCommsBridgeMode = 1;
	return 0;
}

SerCom_err_t Ser_CMD_Get_GPRSVals(USART_t* m_USART) {
	SerCom_err_t tmpErr;
	
	tmpErr = Ser_TxSeg(m_USART, 30, (uint8_t*)&setting_GPRSAPN);
	if (tmpErr < 0) return tmpErr;
	
	tmpErr = Ser_TxSeg(m_USART, 30, (uint8_t*)&setting_GPRSUSERID);
	if (tmpErr < 0) return tmpErr;
	
	tmpErr = Ser_TxSeg(m_USART, 30, (uint8_t*)&setting_GPRSPASSW);
	if (tmpErr < 0) return tmpErr;
	
	return tmpErr;

}

SerCom_err_t Ser_CMD_Set_GPRSVals(USART_t* m_USART) {
	SerCom_err_t tmpErr;
	
	tmpErr = Ser_RxSeg(m_USART, 30, (uint8_t*)&setting_GPRSAPN);
	if (tmpErr < 0) return tmpErr;
	
	tmpErr = Ser_RxSeg(m_USART, 30, (uint8_t*)&setting_GPRSUSERID);
	if (tmpErr < 0) return tmpErr;
	
	tmpErr = Ser_RxSeg(m_USART, 30, (uint8_t*)&setting_GPRSPASSW);
	if (tmpErr < 0) return tmpErr;
	
	//GPRSSETINGSTOEEPROM();
	SettingsManager_SaveGPRSSettings();
	
	return tmpErr;

}

SerCom_err_t Ser_CMD_Get_EmailToAddress(USART_t* m_USART) {
	return Ser_TxSeg(m_USART, 60, (uint8_t*)&setting_EMAILTOADD);
}

SerCom_err_t Ser_CMD_Set_EmailToAddress(USART_t* m_USART) {
	SerCom_err_t tmpErr;
	tmpErr = Ser_RxSeg(m_USART, 60, (uint8_t*)&setting_EMAILTOADD);
	if (tmpErr == SerCom_err_Success) {
		//EMAILTOADDTOEEPROM();
		SettingToEEPROM(EMAILTOADD);
	}
	return tmpErr;
}

SerCom_err_t Ser_CMD_Cmd_UpdateTimeFromGSM(USART_t* m_USART) {
	return 0;//Ser_<Rx/Tx>Seg(m_USART, _DATA_LEN_, (uint8_t*)&VAR_Cmd_UpdateTimeFromGSM, 5);

}

SerCom_err_t Ser_CMD_Get_SendEmailAttempts(USART_t* m_USART) {
	return 0;//Ser_<Rx/Tx>Seg(m_USART, _DATA_LEN_, (uint8_t*)&VAR_Get_SendEmailAttempts, 5);

}

SerCom_err_t Ser_CMD_Set_SendEmailAttempts(USART_t* m_USART) {
	return 0;//Ser_<Rx/Tx>Seg(m_USART, _DATA_LEN_, (uint8_t*)&VAR_Set_SendEmailAttempts, 5);

}

SerCom_err_t Ser_CMD_Cmd_GM862_ConfigModule(USART_t* m_USART) {
	EnterMobCommsConfigMode = 1;
	return SerCom_err_Success;
}

SerCom_err_t Ser_CMD_Get_RFParams(USART_t* m_USART) {
	return Ser_TxSeg(m_USART, 40, (uint8_t*)&setting_RFPARAMS);
	
}

SerCom_err_t Ser_CMD_Set_RFParams(USART_t* m_USART) {
	SerCom_err_t tmpErr;
	tmpErr = Ser_RxSeg(m_USART, 40, (uint8_t*)&setting_RFPARAMS);
	if (tmpErr == SerCom_err_Success) {
		SettingToEEPROM(RFPARAMS);
		setup_cc2500(NULL);
	}
	return tmpErr;
}

SerCom_err_t Ser_CMD_Get_LoggerGroupId(USART_t* m_USART)
{
	return Ser_TxSeg(m_USART, 2, (uint8_t*)&setting_LoggerGroupId);
}

SerCom_err_t Ser_CMD_Set_LoggerGroupId(USART_t* m_USART)
{
	SerCom_err_t tmpErr;
	tmpErr = Ser_RxSeg(m_USART, 2, (uint8_t*)&setting_LoggerGroupId);
	if (tmpErr == SerCom_err_Success) {
		SettingToEEPROM(LOGGRPID);
	}
	return tmpErr;
}

SerCom_err_t Ser_CMD_Get_LoggerId(USART_t* m_USART)
{
	return Ser_TxSeg(m_USART, 4, (uint8_t*)&setting_LoggerId);
}

SerCom_err_t Ser_CMD_Get_WarningSignalDuration(USART_t* m_USART)
{
	return Ser_TxSeg(m_USART, 2, (uint8_t*)&setting_WarningSignalDuration);
}

SerCom_err_t Ser_CMD_Set_WarningSignalDuration(USART_t* m_USART)
{
	SerCom_err_t tmpErr;
	tmpErr = Ser_RxSeg(m_USART, 2, (uint8_t*)&setting_WarningSignalDuration);
	if (tmpErr == SerCom_err_Success) {
		SettingToEEPROM(WARNSIGDUR);
	}
	return tmpErr;
}

SerCom_err_t Ser_CMD_Get_InterLogRFParams(USART_t* m_USART)
{
	return Ser_TxSeg(m_USART, 40, (uint8_t*)&setting_INTERLOGRFPARAMS);
}

SerCom_err_t Ser_CMD_Set_InterLogRFParams(USART_t* m_USART)
{
	SerCom_err_t tmpErr;
	tmpErr = Ser_RxSeg(m_USART, 40, (uint8_t*)&setting_INTERLOGRFPARAMS);
	if (tmpErr == SerCom_err_Success) {
		SettingToEEPROM(INTERLOGRFPARAMS);
		#if defined(INTER_LOGGER_CC110L_INSTALLED)
		setup_cc110L(NULL);
		#endif
	}
	return tmpErr;
}

SerCom_err_t Ser_CMD_Get_LoggerTable(USART_t* m_USART) {
	SerCom_err_t tmpErr;
	uint8_t len = LLUT_GetCount();
	tmpErr = Ser_TxSeg(m_USART, 1, &len);
	if (tmpErr != SerCom_err_Success) return tmpErr;
	LLUTStruct_t* ptr = LLUT_GetDataPtr();
	for (uint8_t i = 0; i < len; i++) {
		tmpErr = Ser_TxSeg(m_USART, 4, (uint8_t*)&ptr[i]);
		if (tmpErr != SerCom_err_Success) return tmpErr;
	}
	return SerCom_err_Success;
}

SerCom_err_t Ser_CMD_Get_IridiumDataMonthNextReset(USART_t* m_USART) {
	return Ser_TxSeg(m_USART, 6, (uint8_t*)&setting_IridiumMonthNextReset);
}

SerCom_err_t Ser_CMD_Set_IridiumDataMonthNextReset(USART_t* m_USART) {
	SerCom_err_t tmpErr;
	tmpErr = Ser_RxSeg(m_USART, 6, (uint8_t*)&setting_IridiumMonthNextReset);
	if (tmpErr == SerCom_err_Success) {
		SettingToEEPROM(IRIDIUMMONTHNXTRST);
	}
	return tmpErr;
}

SerCom_err_t Ser_CMD_Get_MonthByteLimit(USART_t* m_USART)
{
	return Ser_TxSeg(m_USART, 2, (uint8_t*)&setting_IridiumMonthlyByteLimit);
}

SerCom_err_t Ser_CMD_Set_MonthByteLimit(USART_t* m_USART)
{
	SerCom_err_t tmpErr;
	uint32_t monbyteliml = 0;
	tmpErr = Ser_RxSeg(m_USART, 2, (uint8_t*)&monbyteliml);
	if (tmpErr == SerCom_err_Success) {
		setting_IridiumMonthlyByteLimit = monbyteliml;
		SettingToEEPROM(MONTHBYTELIMIT);
	}
	return tmpErr;
}