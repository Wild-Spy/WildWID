/*
 * SettingsManager.h
 *
 * Created: 28/05/2012 1:36:27 PM
 *  Author: MC
 */ 


#ifndef SETTINGSMANAGER_H_
#define SETTINGSMANAGER_H_

#include <stdint.h>
#include "eeprom_driver.h"
#include "MX_RTC.h"


#define RECCOUNT_ADD				0x04	//a 4 byte unsigned integer (uint32_t)
//#define RFID_ADD					0x08	//a 5-byte unsigned integer	(uint8_t[5])
//#define RFCHAN_ADD					0x0D	//a 1-byte unsigned integer	(uint8_t)
//#define TIMESAVE_ADD				0x0E	//a 6-byte structure (RTC_Time_t)				*****
//#define ADCOFFSETV_ADD				0x14	//a 2-byte unsigned integer (uint16_t)
//#define SELFILTER_ADD				0x16	//a 1-byte unsigned integer	(uint8_t)			*****
#define LOGMODE_ADD					0x17	//a 1-byte unsigned integer	(uint8_t)
#define FILTER0_ADD					0x18	//char Filter[8]
#define NEXTGPRSSEND_ADD			0x20	//a 6-byte structure (RTC_Time_t)
#define SENDGPRSPER_ADD				0x26	//a 6-byte structure (RTC_Time_t)
#define DEVICENAME_ADD				0x2C	//char LoggerName[20]
#define EMAILTOADD_ADD				0x40	//char EmailToAddress[60]
#define GPRSAPN_ADD					0x7C	//char GPRS_APN[30];
#define GPRSUSERID_ADD				0x9A	//char GPRS_UserId[30];
#define GPRSPASSW_ADD				0xB8	//char GPRS_Passw[30];
#define RFPARAMS_ADD				0xD6	//40 byte array of unsigned integers (uint8_t)

#define RECCOUNT_VAR				RecordCount		//a 4-byte unsigned integer (uint32_t)
#define RECCOUNT_TYPE				(uint8_t*)&//(uint32_t*)&
#define RECCOUNT_SIZE				4

#define LOGMODE_VAR					setting_LoggingEnabled			//a 1-byte unsigned integer	(uint8_t)
#define LOGMODE_TYPE				(uint8_t*)&
#define LOGMODE_SIZE				1

#define FILTER0_VAR					setting_Filter			//an 8-byte string (char[8])
#define FILTER0_TYPE				(uint8_t*)&//(char*)&
#define FILTER0_SIZE				8

#define NEXTGPRSSEND_VAR			Send_Data_GPRS_Next
#define NEXTGPRSSEND_TYPE			(uint8_t*)&//(RTC_Time_t*)&
#define NEXTGPRSSEND_SIZE			6

#define SENDGPRSPER_VAR				Send_Data_GPRS_Period
#define SENDGPRSPER_TYPE			(uint8_t*)&//(RTC_Time_t*)&
#define SENDGPRSPER_SIZE			6

#define EMAILTOADD_VAR				setting_EMAILTOADD	//char setting_EMAILTOADD[60]
#define EMAILTOADD_TYPE				(uint8_t*)&//(char*)
#define EMAILTOADD_SIZE				60

#define DEVICENAME_VAR				setting_DeviceName	//char setting_DeviceName[20]
#define DEVICENAME_TYPE				(uint8_t*)&//(char*)
#define DEVICENAME_SIZE				20

#define GPRSAPN_VAR					setting_GPRSAPN	//char setting_GPRSAPN[30]
#define GPRSAPN_TYPE				(uint8_t*)&//(char*)
#define GPRSAPN_SIZE				30

#define GPRSUSERID_VAR				setting_GPRSUSERID	//char setting_GPRSUSERID[30]
#define GPRSUSERID_TYPE				(uint8_t*)&//(char*)
#define GPRSUSERID_SIZE				30

#define GPRSPASSW_VAR				setting_GPRSPASSW	//char setting_GPRSPASSW[30]
#define GPRSPASSW_TYPE				(uint8_t*)&//(char*)
#define GPRSPASSW_SIZE				30

#define RFPARAMS_VAR				setting_RFPARAMS	//uint8_t setting_RFPARAMS[40]
#define RFPARAMS_TYPE				(uint8_t*)&
#define RFPARAMS_SIZE				40

extern uint32_t RecordCount;
extern uint8_t setting_LoggingEnabled;
extern char setting_Filter[8];
extern RTC_Time_t Send_Data_GPRS_Next;
extern RTC_Time_t Send_Data_GPRS_Period;
extern char setting_EMAILTOADD[60];
extern char setting_DEVICENAME[20];
extern char setting_GPRSAPN[30];
extern char setting_GPRSUSERID[30];
extern char setting_GPRSPASSW[30];
extern uint8_t setting_RFPARAMS[40];

#define SettingToEEPROM(SettingName)		WriteEEPROM(SettingName##_TYPE SettingName##_VAR, SettingName##_SIZE, SettingName##_ADD)
#define SettingFromEEPROM(SettingName)		ReadEEPROM(SettingName##_TYPE SettingName##_VAR, SettingName##_SIZE, SettingName##_ADD)

void SettingsManager_LoadAllSettings();
void SettingsManager_SaveGPRSSettings();
void SettingsManager_LoadGPRSSettings();

#endif /* SETTINGSMANAGER_H_ */