/*
 * SettingsManager.c
 *
 * Created: 28/05/2012 1:36:41 PM
 *  Author: MC
 */ 

#include "SettingsManager.h"

uint32_t RecordCount = 0L;
uint8_t setting_LoggingEnabled;
char setting_Filter[8];
RTC_Time_t Send_Data_GPRS_Next;
RTC_Time_t Send_Data_GPRS_Period;
char setting_EMAILTOADD[60];
char setting_DeviceName[20] = "TestLogger";
char setting_GPRSAPN[30];
char setting_GPRSUSERID[30];
char setting_GPRSPASSW[30];
uint8_t setting_RFPARAMS[40];
uint16_t setting_LoggerGroupId;
uint32_t setting_LoggerId;
uint16_t setting_WarningSignalDuration;
uint32_t setting_IridiumBytesSentThisMonth;
uint32_t setting_IridiumMonthlyByteLimit = 30*1024;
uint32_t setting_LogsSentViaIridium;
uint8_t setting_INTERLOGRFPARAMS[40];

void SettingsManager_LoadAllSettings() {
	SettingFromEEPROM(RECCOUNT);
	SettingFromEEPROM(LOGMODE);
	SettingFromEEPROM(FILTER0);
	SettingFromEEPROM(NEXTGPRSSEND);
	SettingFromEEPROM(SENDGPRSPER);
	SettingFromEEPROM(EMAILTOADD);
	SettingFromEEPROM(DEVICENAME);
	SettingFromEEPROM(GPRSAPN);
	SettingFromEEPROM(GPRSUSERID);
	SettingFromEEPROM(GPRSPASSW);
	SettingFromEEPROM(RFPARAMS);
	SettingFromEEPROM(LOGGRPID);
	SettingFromEEPROM(LOGGERID);
	SettingFromEEPROM(WARNSIGDUR);
	SettingFromEEPROM(INTERLOGRFPARAMS);
}

void SettingsManager_SaveGPRSSettings() {
	SettingToEEPROM(GPRSAPN);
	SettingToEEPROM(GPRSUSERID);
	SettingToEEPROM(GPRSPASSW);
	SettingToEEPROM(MONTHBYTESSENT);
	SettingToEEPROM(MONTHBYTELIMIT);
	SettingToEEPROM(IRIDIUMLOGSSENT);
}

void SettingsManager_LoadGPRSSettings() {
	SettingFromEEPROM(GPRSAPN);
	SettingFromEEPROM(GPRSUSERID);
	SettingFromEEPROM(GPRSPASSW);
	SettingFromEEPROM(MONTHBYTESSENT);
	SettingFromEEPROM(MONTHBYTELIMIT);
	SettingFromEEPROM(IRIDIUMLOGSSENT);
}
