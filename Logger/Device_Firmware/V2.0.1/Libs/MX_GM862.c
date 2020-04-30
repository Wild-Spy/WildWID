/*
 * MX_GM862.c
 *
 * Created: 15/06/2011 9:20:06 AM
 *  Author: Matthew Cochrane
 */ 

#include "MX_GM862.h"

//General Responses
const char PSTR_GM862_RESPONSE_OK[] PROGMEM = "OK\r\n";
const char PSTR_GM862_RESPONSE_ERROR[] PROGMEM = "ERROR\r\n";
const char PSTR_GM862_RESPONSE_CONNECT[] PROGMEM = "CONNECT\r\n";
const char PSTR_GM862_RESPONSE_SGACT[] PROGMEM = "#SGACT: ";
const char PSTR_GM862_RESPONSE_NITZ[] PROGMEM = "AT#NITZ=1,1\r\n";

//SMTP Responses
const char PSTR_GM862_RESPONSE_SMTP_220[] PROGMEM = "220";	//first greeting
const char PSTR_GM862_RESPONSE_SMTP_250[] PROGMEM = "250";	//response to HELO
const char PSTR_GM862_RESPONSE_SMTP_334_USRN[] PROGMEM = "334 VXNlcm5hbWU6";	//Enter username
const char PSTR_GM862_RESPONSE_SMTP_334_PASW[] PROGMEM = "334 UGFzc3dvcmQ6";	//Enter password
const char PSTR_GM862_RESPONSE_SMTP_235[] PROGMEM = "235";	//Authenticated
const char PSTR_GM862_RESPONSE_SMTP_354[] PROGMEM = "354";	//Go Ahead (DATA)
const char PSTR_GM862_RESPONSE_SMTP_221[] PROGMEM = "221";	//outbound??

//General Commands
const char PSTR_GM862_CMD_AT[] PROGMEM = "AT\r\n";
const char PSTR_GM862_CMD_CPIN[] PROGMEM = "AT+CPIN?\r\n";	//is sim card ready?
const char PSTR_GM862_CMD_CREG[] PROGMEM = "AT+CREG?\r\n";
const char PSTR_GM862_CMD_SELINT_SET[] PROGMEM = "AT+SELINT=2\r\n";
const char PSTR_GM862_CMD_FLOWCONTROL_OFF[] PROGMEM = "AT&K=0\r\n";

//Setup Commands
const char PSTR_GM862_CMD_CONFIG_1[] PROGMEM = "AT#SELINT=2\r\n";
const char PSTR_GM862_CMD_CONFIG_2[] PROGMEM = "AT#REGMODE=1\r\n";
const char PSTR_GM862_CMD_CONFIG_3[] PROGMEM = "AT+CREG=1\r\n";
const char PSTR_GM862_CMD_CONFIG_4[] PROGMEM = "AT#NITZ=7,1\r\n";
const char PSTR_GM862_CMD_CONFIG_5[] PROGMEM = "AT&W0\r\n";
const char PSTR_GM862_CMD_CONFIG_6[] PROGMEM = "AT&Y0\r\n";
const char PSTR_GM862_CMD_CONFIG_7[] PROGMEM = "AT&P0\r\n";

//SMS Commands
const char PSTR_GM862_CMD_CMGS[] PROGMEM = "AT+CMGS=";

//Clock Commands
const char PSTR_GM862_CMD_CCLK[] PROGMEM = "AT+CCLK?\r\n";
const char PSTR_GM862_CMD_NITZ_SET[] PROGMEM = "AT+NITZ=1,1\r\n";
const char PSTR_GM862_CMD_NITZ[] PROGMEM = "AT#NITZ?\r\n";

//GPRS Commands
const char PSTR_GM862_CMD_GPRSACT[] PROGMEM = "AT#GPRS?\r\n";
//char PSTR_GM862_CMD_CGDCONT[] PROGMEM = "AT+CGDCONT=1,\"IP\",\"internet\"\r\n";
const char PSTR_GM862_CMD_CGDCONT[] PROGMEM = "AT+CGDCONT=1,\"IP\",\"%s\"\r\n";
//char PSTR_GM862_CMD_USERID[] PROGMEM = "AT#USERID=\"\"\r\n";
const char PSTR_GM862_CMD_USERID[] PROGMEM = "AT#USERID=\"%s\"\r\n";
//char PSTR_GM862_CMD_PASSW[] PROGMEM = "AT#PASSW=\"\"\r\n";
const char PSTR_GM862_CMD_PASSW[] PROGMEM = "AT#PASSW=\"%s\"\r\n";
const char PSTR_GM862_CMD_SCFG[] PROGMEM = "AT#SCFG=1,1,300,90,600,50\r\n";
const char PSTR_GM862_CMD_SGACT[] PROGMEM = "AT#SGACT=1,1,\"\",\"\"\r\n";
const char PSTR_GM862_CMD_SD_SMTPIINET[] PROGMEM = "AT#SD= 1,0,25,\"mail.iinet.net.au\",0,0\r\n";

//SMTP Server Commands
const char PSTR_GM862_CMD_SMPT_HELO[] PROGMEM = "HELO\r\n";
const char PSTR_GM862_CMD_SMPT_AUTH[] PROGMEM = "AUTH LOGIN\r\n";
const char PSTR_GM862_CMD_SMPT_USERNAME[] PROGMEM = "bWF0dGhldy5jb2NocmFuZUBpaW5ldC5uZXQuYXU=\r\n";
const char PSTR_GM862_CMD_SMPT_PASSWORD[] PROGMEM = "Nzg5NDg5MTU2MTIzYUE=\r\n";
const char PSTR_GM862_CMD_SMPT_DATA[] PROGMEM = "DATA\r\n";

//char PSTR_GM862_CMD_SMPT_MAILFROM[] PROGMEM = "MAIL FROM: <matthew.cochrane@iinet.net.au>\r\n";
//char PSTR_GM862_CMD_SMPT_RCPTRO[] PROGMEM = "RCPT TO: <mattisback@gmail.com>\r\n";
//char PSTR_GM862_CMD_SMPT_DATA1[] PROGMEM = "From: matthew.cochrane@iinet.net.au\r\n"
//										   "To: mattisback@gmail.com\r\n"
//										   "Subject: "
//										   "Hello World Subject\r\n"
//										   "\r\n"
//										   "This is a test message!\r\n"
//										   ".\r\n";

const char PSTR_GM862_CMD_SMPT_MAILFROM[] PROGMEM			= "MAIL FROM: <%s>\r\n";
const char PSTR_GM862_CMD_SMPT_RCPTTO[] PROGMEM			= "RCPT TO: <%s>\r\n";
const char PSTR_GM862_CMD_SMPT_DATA_HEAD_FROM[] PROGMEM	= "From: %s\r\n";
const char PSTR_GM862_CMD_SMPT_DATA_HEAD_TO[] PROGMEM		= "To: %s\r\n";
const char PSTR_GM862_CMD_SMPT_DATA_HEAD_SUBJ[] PROGMEM	= "Subject: %s\r\n"
													  "\r\n";
const char PSTR_GM862_CMD_SMPT_DATA_END[] PROGMEM = ".\r\n";
const char PSTR_GM862_CMD_SMPT_QUIT[] PROGMEM = "QUIT\r\n";

//char PSTR_EMAILTOADD[] PROGMEM = "wildspyworkshop@gmail.com";
const char PSTR_EMAILTOADD[] PROGMEM = "mattisback@gmail.com";
//char PSTR_EMAILFROMADD[] PROGMEM = "matthew.cochrane@iinet.net.au";
char EmailToAddress[60];// = "mattisback@gmail.com";
char GPRS_APN[30];// = "internet"; //(for optus)
char GPRS_UserId[30];// = "";
char GPRS_Passw[30];// = "";


//char PSTR_GM862_CMD_GPRS[] PROGMEM = "AT#GPRS=1\r";
//char PSTR_GM862_CMD_GPRS[] PROGMEM = "AT#GPRS=1\r";

extern char s[80];
extern uint32_t RecordCount;
extern char LoggerName[20];
extern float tmpBatV;

RTC_Time_t RTC_c_Time;
uint32_t records_sent_already = 0L;

char GM862_tmpStr[100];
char GM862_tmpStr1[100];

char GM862_GPRS_IP[15];

//************************************
// Method:    GM862_SetupHardware
// FullName:  GM862_SetupHardware
// Access:    public 
// Returns:   GM862_err_t
// Qualifier:
//************************************
GM862_err_t GM862_SetupHardware() {
	
	//Setup the USART
	USART_Setup(&GM862_USART_PORT, &GM862_USART, 176, -5, 0);
	
	GM862_PWR_CTRL_PORT.DIRSET = GM862_PWR_CTRL_PIN_bm;
	GM862_ONOFF_PORT.DIRSET = GM862_ONOFF_PIN_bm;
	
	//Turn Unit Off
	GM862_PWR_CTRL_PORT.OUTSET = GM862_PWR_CTRL_PIN_bm;
	GM862_ONOFF_PORT.OUTCLR = GM862_ONOFF_PIN_bm;
	
	return GM862_err_Success;

}

//************************************
// Method:    GM862_DisableHardware
// FullName:  GM862_DisableHardware
// Access:    public 
// Returns:   GM862_err_t
// Qualifier:
//************************************
GM862_err_t GM862_DisableHardware() {
	
	//TODO[ ]:	When the datalogger is off, for the GM862 to be off also, the PWR_CTRL pin must be set high.
	//			Make sure power consumption is kept to a minimum.. Does leaving this pin high create a leakage
	//			current?  Is there any time when we want to turn the unit 'off' even when the main battery
	//			is connected (and therefore want the GM862 off as well)?
	
	//Disable the USART, pins back to imputs
	GM862_USART.CTRLB &= ~(USART_RXEN_bm | USART_TXEN_bm);
	GM862_USART_PORT.OUTCLR = (1<<2)|(1<<3);
	GM862_USART_PORT.DIRCLR = (1<<2)|(1<<3);
	
	//Turn Unit Off
	GM862_PWR_CTRL_PORT.OUTSET = GM862_PWR_CTRL_PIN_bm;

	//Pins as inputs
	GM862_PWR_CTRL_PORT.DIRCLR = GM862_PWR_CTRL_PIN_bm;
	GM862_ONOFF_PORT.DIRCLR = GM862_ONOFF_PIN_bm;
	
	return GM862_err_Success;

}

GM862_err_t GM862_HW_On() {
	
	//Turn Power On
	//GM862_PWR_CTRL_PORT.OUTCLR = GM862_PWR_CTRL_PIN_bm;
	GM862_PWR_CTRL_PORT.OUTCLR = (1<<GM862_PWR_CTRL_PIN_bp);
	
	_delay_ms(2000);
	
	//Send Power On Signal
	GM862_ONOFF_PORT.OUTSET = GM862_ONOFF_PIN_bm;
	_delay_ms(1200);
	GM862_ONOFF_PORT.OUTCLR = GM862_ONOFF_PIN_bm;
	
	_delay_ms(1000);
	
	//Wait 10 seconds for GM862 to start up
	//_delay_ms(10000);
	return SerCom_err_Success;
}

//************************************
// Method:    GM862_Unit_On
// FullName:  GM862_Unit_On
// Access:    public 
// Returns:   GM862_err_t
// Qualifier:
// Parameter: uint8_t updateRTC
//************************************
//TODO[ ]: test all this again!!!  have modified but can compare with last version which was backed up to a .rar file
GM862_err_t GM862_Unit_On(uint8_t updateRTC) {
	//uint8_t len = 0;
	//USART_err_t tmpUSARTErr;
	//uint8_t b = 10; //40;
	GM862_err_t a;
	//uint8_t initialSetup = 1;
	//uint8_t gotTime = 0;
	//uint8_t Registered = 0;
	RTC_Time_t endTime;
	char* responsePtr = 0;
	
	PWR_GOOD_INT_DIS();
	
	GM862_HW_On();
	
	//GM862_BridgeMode();
	
	TimeAfterTime(&RTC_c_Time, &endTime, 0, 0, 0, 0, 2, 0);
	
	//GM862_SendGeneralCmd_P(PSTR_GM862_CMD_AT);
	
	//Turn off flow control
	GM862_SendGeneralCmd_P(PSTR_GM862_CMD_FLOWCONTROL_OFF);
	
	do {
		a = GM862_SendCmdAndCheckStart_P(PSTR_GM862_CMD_CREG, PSTR("+CREG:"), &responsePtr, 500);
		if (a == GM862_err_Success) {
			//if "+CREG: 1,X...." X == '1' -> means registered on network
			//responsePtr points to the char after "+CREG:" which is the space between "REG:" and "1,X"
			//responsePtr[3] points to the X character which we want to know
			if (responsePtr[3] == '1') {
				//Give 10 seconds after now for the time to be sent from the network, otherwise, give up!
				if (updateRTC) {a = GM862_WaitForNITZ(10);}
				else {a = GM862_err_Success;}
					
				if (a == GM862_err_Success) {
					PWR_GOOD_INT_EN();
					return GM862_err_SuccessAndUDTime;
				} else {
					PWR_GOOD_INT_EN();
					return GM862_err_Success;
				}
// 				TimeAfterTime(&RTC_c_Time, &endTime, 0, 0, 0, 0, 0, 10);
// 				do {
// 					if (GM862_CalibratetoRTCTime() == GM862_err_Success) {
// 						PWR_GOOD_INT_EN();
// 						return GM862_err_SuccessAndUDTime;
// 					} else if (a == GM862_err_InvalidData) {
// 						PWR_GOOD_INT_EN();
// 						return GM862_err_Success;
// 					} else {
// 						//Somthing's gone wrong. Better "break"
// 						break;
// 					}	
// 				} while (FirstTimeGreaterOrEqualTo(&endTime, &RTC_c_Time));
			} else {
				//we aren't registered to the network yet but that's all good.. let's just wait longer.
			}
		} else if ((a < 0) && (a != GM862_err_InvalidData)) {
			//All the 'bad' errors
			break;
		}
		//exit if it has been more than 2 minutes
	} while (FirstTimeGreaterOrEqualTo(&endTime, &RTC_c_Time));
	
	/*while (b) {
		tmpUSARTErr = USART_rx_String(&USARTE0, GM862_tmpStr, 10000, &len);
		//USART_printf_P(&USARTC0, "a=%d, b=%u\r\n", a, b);
		if (tmpUSARTErr == USART_err_Timeout) {
			b--;
			//USART_tx_Byte(&USARTC0, b+'0');
			//USART_printf_P(&USARTC0, "a=%d, b=%u\r\n", a, b);
		} else if (len > 0) {
			USART_tx_String(&USARTC0, GM862_tmpStr);
			if (compareStartofStringWithP(GM862_tmpStr, PSTR("+CREG: 1")) > 0) {
				Registered = 1;
				b = 2;
			}
			if (!gotTime && updateRTC) {
				if (compareStartofStringWithP(GM862_tmpStr, PSTR("#NITZ: ")) > 0) {
					//if (GM862_tmpStr[24] == '+') {
					//GM862_tmpStr = "#NITZ: 11/06/22,08:30:38+40/r/n"
					//                0123456789012345678901234567 8
					//				  0         1         2
					//DD
					s[0] = GM862_tmpStr[13];
					s[1] = GM862_tmpStr[14];
					//MM
					s[2] = GM862_tmpStr[10];
					s[3] = GM862_tmpStr[11];
					//YY
					s[4] = GM862_tmpStr[7];
					s[5] = GM862_tmpStr[8];
					//HH
					s[6] = GM862_tmpStr[16];
					s[7] = GM862_tmpStr[17];
					//mm
					s[8] = GM862_tmpStr[19];
					s[9] = GM862_tmpStr[20];
					//ss
					s[10] = GM862_tmpStr[22];
					s[11] = GM862_tmpStr[23];
						
					str2Time(s, &RTC_c_Time);
					USART_tx_String_P(&USARTC0, PSTR("UPDATED RTC TIME!\r\n"));
					SAVE_SYS_MSG(SYS_MSG_UPDATED_TIME, SYS_MSG_UPDATE_TIME_SRC_GSM);
					gotTime = 1;
					//}
				}	
			}
			
			//If we have the time updated and are registered on the network, we're done.  Exit the loop!
			if (gotTime == 1 && Registered == 1) {
				break;
			}
			
			//TODO[ ]: Have a 'First Start' Mode where all these values are set on the GM862! for use in production and assembly.
			//if (initialSetup) {
				//if (GM862_SendGeneralCmd_P(PSTR_GM862_CMD_AT)) {
					//initialSetup = 0;
					//if (GM862_SendCmdAndCheck_P(PSTR_GM862_CMD_SELINT_SET, PSTR_GM862_CMD_SELINT_SET, 10000) < 0) {
						////Some kind of error!
						//GM862_PWR_CTRL_PORT.OUTSET = (1<<GM862_PWR_CTRL_PIN_bp);
						//PWG_GOOD_INT_EN();
						//return GM862_err_Unknown;
					//} else {
						//if (GM862_SendCmdAndCheck_P(PSTR_GM862_CMD_NITZ, PSTR_GM862_RESPONSE_NITZ, 10000) < 0) {
							//if (GM862_SendCmdAndCheck_P(PSTR_GM862_CMD_NITZ_SET, PSTR_GM862_RESPONSE_NITZ, 10000) < 0) {
								////Some kind of error!
								//GM862_PWR_CTRL_PORT.OUTSET = (1<<GM862_PWR_CTRL_PIN_bp);
								//PWG_GOOD_INT_EN();
								//return GM862_err_Unknown;
							//}
						//}
					//}					
				//}
			//}
			
		}
	}
	
	if (Registered == 1) {
		PWR_GOOD_INT_EN();
		if (!gotTime && updateRTC) {
			return 1; //<-(NOT EQUIVALENT!!)->GM862_err_Timeout;
		} else {
			return GM862_err_Success;
		}	
	}*/
	
	//Unit OFF
	GM862_PWR_CTRL_PORT.OUTSET = GM862_PWR_CTRL_PIN_bm;

	PWR_GOOD_INT_EN();
	return GM862_err_Unknown;

}

GM862_err_t GM862_WaitForNITZ(uint16_t secTimeout) {
	RTC_Time_t endTime;
	RTC_Time_t tmpTime;
	USART_err_t tmpUSARTErr;
	uint8_t len;
	
	TimeAfterTime(&RTC_c_Time, &endTime, 0, 0, 0, 0, 0, secTimeout);

	do {
		tmpUSARTErr = USART_rx_String(&USARTE0, GM862_tmpStr, 100, &len);
		if (len > 0) {
			USART_tx_String(&USARTC0, GM862_tmpStr);
			if (compareStartofStringWithP(GM862_tmpStr, PSTR("#NITZ: ")) > 0) {
				//GM862_tmpStr = "#NITZ: 11/06/22,08:30:38+40/r/n"
				//                0123456789012345678901234567 8
				//				  0         1         2
				//DD
				s[0] = GM862_tmpStr[13];
				s[1] = GM862_tmpStr[14];
				//MM
				s[2] = GM862_tmpStr[10];
				s[3] = GM862_tmpStr[11];
				//YY
				s[4] = GM862_tmpStr[7];
				s[5] = GM862_tmpStr[8];
				//HH
				s[6] = GM862_tmpStr[16];
				s[7] = GM862_tmpStr[17];
				//mm
				s[8] = GM862_tmpStr[19];
				s[9] = GM862_tmpStr[20];
				//ss
				s[10] = GM862_tmpStr[22];
				s[11] = GM862_tmpStr[23];
						
				if (str2Time(s, &tmpTime) == 0) {
					CopyTime(&tmpTime, &RTC_c_Time);
					USART_tx_String_P(&USARTC0, PSTR("UPDATED RTC TIME!\r\n"));
					SAVE_SYS_MSG(SYS_MSG_UPDATED_TIME, SYS_MSG_UPDATE_TIME_SRC_GSM);
					return GM862_err_Success;
				} else {
					return GM862_err_InvalidData;
				}
			}
		}			
	} while (FirstTimeGreaterOrEqualTo(&endTime, &RTC_c_Time));
	
	return GM862_err_Timeout;
	
}

//************************************
// Method:    GM862_Unit_Off
// FullName:  GM862_Unit_Off
// Access:    public 
// Returns:   GM862_err_t
// Qualifier:
//************************************
GM862_err_t GM862_Unit_Off() {
	
	//Send Power Off Signal - let it shut down properly
	GM862_ONOFF_PORT.OUTSET = GM862_ONOFF_PIN_bm;
	_delay_ms(1200);
	GM862_ONOFF_PORT.OUTCLR = GM862_ONOFF_PIN_bm;
	
	//TODO[ ]: Go to sleep or do this in background!
	
	//wait 5 seconds
	_delay_ms(5000);
	
	//Turn Power Off
	GM862_PWR_CTRL_PORT.OUTSET = GM862_PWR_CTRL_PIN_bm;

	return GM862_err_Success;

}

//0 = success
//************************************
// Method:    GM862_SendCmdAndCheck_P
// FullName:  GM862_SendCmdAndCheck_P
// Access:    public 
// Returns:   GM862_err_t
// Qualifier:
// Parameter: char * cmdstrP
// Parameter: char * expectedResponseP
// Parameter: uint16_t timeout
//************************************
GM862_err_t GM862_SendCmdAndCheck_P(char* cmdstrP, char* expectedResponseP, uint16_t timeout) {
	uint8_t tries = 10;
	uint8_t len;
	
	USART_rx_String(&USARTE0, GM862_tmpStr, 100, &len);
	_delay_ms(500);
	USART_tx_Byte(&USARTC0, 'O');
	USART_tx_Byte(&USARTC0, '-');
	USART_tx_String_P(&USARTC0, cmdstrP);
	USART_tx_String_P(&USARTE0, cmdstrP);

	//so if it times out we abort or if we receive > 10 messages we time out!
	while ((USART_rx_String(&USARTE0, GM862_tmpStr, timeout, &len) != USART_err_Timeout) && tries--) {
		if (compareStringWithP(GM862_tmpStr, expectedResponseP)==0) {
			USART_tx_Byte(&USARTC0, 'I');
			USART_tx_Byte(&USARTC0, '-');
			USART_tx_String(&USARTC0, GM862_tmpStr);
			//_delay_ms(2);
			return GM862_err_Success;
		}
	}
	
	if (tries) {
		USART_tx_String_P(&USARTC0, PSTR("TimeOut\r\n"));
		return GM862_err_Timeout;
	} else {
		USART_tx_Byte(&USARTC0, 'I');
		USART_tx_Byte(&USARTC0, '-');
		USART_tx_String(&USARTC0, GM862_tmpStr);
		return GM862_err_BadResponse;
	}
}

//0 = success
//returnStr is only a pointer to the start of the rest of the response in GM862_tmpStr
//************************************
// Method:    GM862_SendCmdAndCheckStart
// FullName:  GM862_SendCmdAndCheckStart
// Access:    public 
// Returns:   GM862_err_t
// Qualifier:
// Parameter: char * cmdstr
// Parameter: char * expectedResponseStartP
// Parameter: char * * ReturnStr
// Parameter: uint16_t timeout
//************************************
GM862_err_t GM862_SendCmdAndCheckStart(char* cmdstr, char* expectedResponseStartP, char** ReturnStr,  uint16_t timeout) {
	//uint8_t tries = 10;
	//int a;
	uint8_t len;
	
	USART_rx_String(&USARTE0, GM862_tmpStr, 100, &len);
	_delay_ms(500);
	USART_tx_Byte(&USARTC0, 'O');
	USART_tx_Byte(&USARTC0, '-');
	USART_tx_String(&USARTC0, (const char*) cmdstr);
	USART_tx_String(&USARTE0, (const char*) cmdstr);

	return GM862_ListenForResponseAndCheckStart_P(expectedResponseStartP, ReturnStr, timeout);

}

//0 = success
//returnStr is only a pointer to the start of the rest of the response in GM862_tmpStr
//************************************
// Method:    GM862_SendCmdAndCheckStart_P
// FullName:  GM862_SendCmdAndCheckStart_P
// Access:    public 
// Returns:   GM862_err_t
// Qualifier:
// Parameter: char * cmdstrP
// Parameter: char * expectedResponseStartP
// Parameter: char * * ReturnStr
// Parameter: uint16_t timeout
//************************************
GM862_err_t GM862_SendCmdAndCheckStart_P(char* cmdstrP, char* expectedResponseStartP, char** ReturnStr,  uint16_t timeout) {
	//uint8_t tries = 10;
	//int a;
	uint8_t len;
	
	USART_rx_String(&USARTE0, GM862_tmpStr, 100, &len);
	_delay_ms(500);
	USART_tx_Byte(&USARTC0, 'O');
	USART_tx_Byte(&USARTC0, '-');
	USART_tx_String_P(&USARTC0, (const char*) cmdstrP);
	USART_tx_String_P(&USARTE0, (const char*) cmdstrP);

	return GM862_ListenForResponseAndCheckStart_P(expectedResponseStartP, ReturnStr, timeout);

}

//************************************
// Method:    GM862_ListenForResponseAndCheckStart_P
// FullName:  GM862_ListenForResponseAndCheckStart_P
// Access:    public 
// Returns:   GM862_err_t
// Qualifier:
// Parameter: char * expectedResponseStartP
// Parameter: char * * ReturnStr
// Parameter: uint16_t timeout
//************************************
GM862_err_t GM862_ListenForResponseAndCheckStart_P(char* expectedResponseStartP, char** ReturnStr,  uint16_t timeout) {
	uint8_t tries = 10;
	uint8_t len;
	
	int a;
	
	
	//so if it times out we abort or if we receive > 10 messages we time out!
	while ((USART_rx_String(&USARTE0, GM862_tmpStr, timeout, &len) != USART_err_Timeout) && tries--) {
		if ((a = compareStartofStringWithP(GM862_tmpStr, expectedResponseStartP)) >= 0) {
			USART_tx_Byte(&USARTC0, 'I');
			USART_tx_Byte(&USARTC0, '-');
			USART_tx_String(&USARTC0, GM862_tmpStr);
			*ReturnStr = GM862_tmpStr + a;
			return GM862_err_Success;
		}
		//USART_tx_String(&USARTC0, GM862_tmpStr);
	}
	
	if (tries) {
		USART_tx_String_P(&USARTC0, PSTR("TimeOut\r\n"));
		return GM862_err_Timeout;
	} else {
		USART_tx_Byte(&USARTC0, 'I');
		USART_tx_Byte(&USARTC0, '-');
		USART_tx_String(&USARTC0, GM862_tmpStr);
		*ReturnStr = GM862_tmpStr;
		return GM862_err_InvalidData;
	}
	
}

//************************************
// Method:    GM862_BridgeMode
// FullName:  GM862_BridgeMode
// Access:    public 
// Returns:   void
// Qualifier:
//************************************
void GM862_BridgeMode() {
	uint8_t tmpByte;
	USART_err_t tmpUSARTErr;
	
	while(1)
    {
		if ((tmpUSARTErr = USART_rx_Byte_nb(&USARTC0, &tmpByte)) >= 0) {
			if (tmpByte == 0x02) {
				//Send Power On Signal
				GM862_ONOFF_PORT.OUTSET = GM862_ONOFF_PIN_bm;
				_delay_ms(1200);
				GM862_ONOFF_PORT.OUTCLR = GM862_ONOFF_PIN_bm;
			} else if (tmpByte == 0x03) {
				//Exit
				//First, flush RX buffers...
				while (USART_rx_Byte_nb(&USARTC0, &tmpByte) >= 0);
				while (USART_rx_Byte_nb(&USARTE0, &tmpByte) >= 0);
				return;
			}
			USART_tx_Byte(&USARTE0, tmpByte);
		}
		if ((tmpUSARTErr = USART_rx_Byte_nb(&USARTE0, &tmpByte)) >= 0) {
			USART_tx_Byte(&USARTC0, tmpByte);
		}
    }	
}

//************************************
// Method:    GM862_CalibratetoRTCTime
// FullName:  GM862_CalibratetoRTCTime
// Access:    public 
// Returns:   GM862_err_t
// Qualifier:
//************************************
GM862_err_t GM862_CalibratetoRTCTime() {
	char* responsePtr;
	RTC_Time_t tmpTime;
	int a;
	
	GM862_SendGeneralCmd_P(PSTR_GM862_CMD_AT);
	if ((a = GM862_SendCmdAndCheckStart_P(PSTR_GM862_CMD_CCLK, PSTR("+CCLK: \""), &responsePtr, 2000)) < 0) return a;
	//if ((a = GM862_SendCmdAndCheckStart_P(PSTR_GM862_CMD_NITZ, PSTR("#NITZ: \""), &responsePtr, 2000)) < 0) return a;
	//responsePtr = "11/06/22,09:50:23+40", 0\r\n
	//               0123456789012345678901 2
	//               0         1         2
	
	//DD
	s[0] = responsePtr[6];
	s[1] = responsePtr[7];
	//MM
	s[2] = responsePtr[3];
	s[3] = responsePtr[4];
	//YY
	s[4] = responsePtr[0];
	s[5] = responsePtr[1];
	//HH
	s[6] = responsePtr[9];
	s[7] = responsePtr[10];
	//mm
	s[8] = responsePtr[12];
	s[9] = responsePtr[13];
	//ss
	s[10] = responsePtr[15];
	s[11] = responsePtr[16];

	if (str2Time(s, &tmpTime) == 0) {
		CopyTime(&tmpTime, &RTC_c_Time);
		SAVE_SYS_MSG(SYS_MSG_UPDATED_TIME, SYS_MSG_UPDATE_TIME_SRC_GSM);
		return GM862_err_Success;
	} else {
		return GM862_err_InvalidData;
	}		
	
}

GM862_err_t GM862_ConfigureModule() {
	uint8_t i;

	i = 0;
	while(GM862_SendGeneralCmd_P(PSTR_GM862_CMD_CONFIG_1) != GM862_err_Success && i < 3) {i++;} ;
	i = 0;
	while(GM862_SendGeneralCmd_P(PSTR_GM862_CMD_CONFIG_2) != GM862_err_Success && i < 3) {i++;} ;
	i = 0;
	while(GM862_SendGeneralCmd_P(PSTR_GM862_CMD_CONFIG_3) != GM862_err_Success && i < 3) {i++;} ;
	i = 0;
	while(GM862_SendGeneralCmd_P(PSTR_GM862_CMD_CONFIG_4) != GM862_err_Success && i < 3) {i++;} ;
	i = 0;
	while(GM862_SendGeneralCmd_P(PSTR_GM862_CMD_CONFIG_5) != GM862_err_Success && i < 3) {i++;} ;
	i = 0;
	while(GM862_SendGeneralCmd_P(PSTR_GM862_CMD_CONFIG_6) != GM862_err_Success && i < 3) {i++;} ;
	i = 0;
	while(GM862_SendGeneralCmd_P(PSTR_GM862_CMD_CONFIG_7) != GM862_err_Success && i < 3) {i++;} ;
	i = 0;
}

//************************************
// Method:    GM862_SendEmail
// FullName:  GM862_SendEmail
// Access:    public 
// Returns:   GM862_err_t
// Qualifier:
//************************************
GM862_err_t GM862_SendEmail() {//char* to_addressP, char* from_addressP) {
	char* responsePtr = 0;
	char* strPtr1;
	char* strPtr2;
	char PwrLvl[15] = "PWR:?dbm";
	uint32_t i;
	uint16_t Sent = 0;
	RTC_Time_t tmpTime;
	uint32_t tmpID;
	GM862_err_t a = SerCom_err_Success;
	uint8_t tmpFlags;

	PWR_GOOD_INT_DIS();

	do {
		_delay_ms(2000);
		
		//Align serial
		GM862_SendGeneralCmd_P(PSTR_GM862_CMD_AT);
		
		//MONI command -> get signal level
		//"#MONI: N6 70 55FA 1D77 756 -99dbm 3 11"
		if ((a = GM862_SendCmdAndCheckStart_P(PSTR("AT#MONI\r\n"), PSTR("#MONI: "), &responsePtr, 10000)) < 0) GM862_Exit_With_PWRGD_Enable(); //return a;
		if ((strPtr1 = strstr_P(responsePtr, PSTR("PWR:"))) != NULL) {
			if ((strPtr2 = strstr_P(strPtr1, PSTR("dbm"))) != NULL) {
				*(strPtr2+3) = '\0';
				strcpy(PwrLvl, strPtr1);
			}				
		}
		
		//Setup GPRS
		if ((a = GM862_SendCmdAndCheckStart_P(PSTR_GM862_CMD_GPRSACT, PSTR("#GPRS: "), &responsePtr, 10000)) < 0) GM862_Exit_With_PWRGD_Enable(); //return a;
		if (*responsePtr == '0') {
			GM862_SendFormatedStringAndCheckStart_P(PSTR_GM862_CMD_CGDCONT, GPRS_APN, PSTR_GM862_RESPONSE_OK, &responsePtr, 5000, a);
			if (a < 0) GM862_Exit_With_PWRGD_Enable();
			GM862_SendFormatedStringAndCheckStart_P(PSTR_GM862_CMD_USERID, GPRS_UserId, PSTR_GM862_RESPONSE_OK, &responsePtr, 5000, a);
			if (a < 0) GM862_Exit_With_PWRGD_Enable();
			GM862_SendFormatedStringAndCheckStart_P(PSTR_GM862_CMD_PASSW, GPRS_Passw, PSTR_GM862_RESPONSE_OK, &responsePtr, 5000, a);
			if (a < 0) GM862_Exit_With_PWRGD_Enable();
			
			//GM862_SendGeneralCmd_P(PSTR_GM862_CMD_USERID);
			//GM862_SendFormatedString_P(PSTR_GM862_CMD_USERID, GPRS_UserId);
			//GM862_SendGeneralCmd_P(PSTR_GM862_CMD_PASSW);
			//GM862_SendFormatedString_P(PSTR_GM862_CMD_PASSW, GPRS_Passw);
			
			GM862_SendGeneralCmd_P(PSTR_GM862_CMD_SCFG);
			//GPRS Connect
			if ((a = GM862_SendCmdAndCheckStart_P(PSTR_GM862_CMD_SGACT, PSTR_GM862_RESPONSE_SGACT, &responsePtr, 7000)) < 0) GM862_Exit_With_PWRGD_Enable(); //return a;
			stringCopy(responsePtr, GM862_GPRS_IP);
			USART_tx_String_P(&USARTC0, PSTR("Connected to GPRS.  IP Address is: "));
			USART_tx_String(&USARTC0, GM862_GPRS_IP);
			USART_tx_String_P(&USARTC0, PSTR_NEWLINE);
		} else if (*responsePtr != '1') {
			//should be #GPRS: 1
			PWR_GOOD_INT_EN();
			return GM862_err_BadResponse;
		}		
		
		GM862_SendCmdAndCheck_P(PSTR_GM862_CMD_SD_SMTPIINET, PSTR_GM862_RESPONSE_CONNECT, 30000);
		#define TMP_ATTEMPTS 3
		for (uint8_t ii = 0; ii < TMP_ATTEMPTS; ii++) {
			if ((a = GM862_ListenForResponseAndCheckStart_P(PSTR_GM862_RESPONSE_SMTP_220, &responsePtr, 30000)) == GM862_err_Success) {
				break;
			} else {
				if (ii == TMP_ATTEMPTS-1) {
					GM862_Exit_With_PWRGD_Enable(); //return a;
				}
			}
		}		
		
		if ((a = GM862_SendCmdAndCheckStart_P(PSTR_GM862_CMD_SMPT_HELO, PSTR_GM862_RESPONSE_SMTP_250, &responsePtr, 20000)) < 0) GM862_Exit_With_PWRGD_Enable(); //return a;
		
		//if ((a = GM862_SendCmdAndCheckStart_P(PSTR_GM862_CMD_SMPT_AUTH, PSTR_GM862_RESPONSE_SMTP_334_USRN, &responsePtr, 20000)) < 0) GM862_Exit_With_PWRGD_Enable(); //return a;
		for (uint8_t ii = 0; ii < TMP_ATTEMPTS; ii++) {
			if ((a = GM862_SendCmdAndCheckStart_P(PSTR_GM862_CMD_SMPT_AUTH, PSTR_GM862_RESPONSE_SMTP_334_USRN, &responsePtr, 20000)) == GM862_err_Success) {
				break;
			} else {
				if (ii == TMP_ATTEMPTS-1) {
					GM862_Exit_With_PWRGD_Enable(); //return a;
				}
			}
		}	
		
		if ((a = GM862_SendCmdAndCheckStart_P(PSTR_GM862_CMD_SMPT_USERNAME, PSTR_GM862_RESPONSE_SMTP_334_PASW, &responsePtr, 20000)) < 0) GM862_Exit_With_PWRGD_Enable(); //return a;
		if ((a = GM862_SendCmdAndCheckStart_P(PSTR_GM862_CMD_SMPT_PASSWORD, PSTR_GM862_RESPONSE_SMTP_235, &responsePtr, 20000)) < 0) GM862_Exit_With_PWRGD_Enable(); //return a;	//Authenticated!
		
		char sss[40];
		sprintf_P(sss, PSTR("%s.WSLog@wildspy.com.au"), LoggerName);
		
		GM862_SendFormatedStringAndCheckStart_P(PSTR_GM862_CMD_SMPT_MAILFROM, sss, PSTR_GM862_RESPONSE_SMTP_250, &responsePtr, 20000, a);
		if (a < 0) GM862_Exit_With_PWRGD_Enable(); //return a;
		//GM862_SendFormatedStringAndCheckStart_PP(PSTR_GM862_CMD_SMPT_RCPTTO, PSTR_EMAILTOADD, PSTR_GM862_RESPONSE_SMTP_250, &responsePtr, 20000, a);
		GM862_SendFormatedStringAndCheckStart_P(PSTR_GM862_CMD_SMPT_RCPTTO, EmailToAddress, PSTR_GM862_RESPONSE_SMTP_250, &responsePtr, 20000, a);
		if (a < 0) GM862_Exit_With_PWRGD_Enable(); //return a;
		
		if ((a = GM862_SendCmdAndCheckStart_P(PSTR_GM862_CMD_SMPT_DATA, PSTR_GM862_RESPONSE_SMTP_354, &responsePtr, 20000)) < 0) GM862_Exit_With_PWRGD_Enable(); //return a;
		
		GM862_SendFormatedString_P(PSTR_GM862_CMD_SMPT_DATA_HEAD_FROM, sss);
		//GM862_SendFormatedString_P(PSTR_GM862_CMD_SMPT_DATA_HEAD_TO, PSTR_EMAILTOADD);
		GM862_SendFormatedString_P(PSTR_GM862_CMD_SMPT_DATA_HEAD_TO, EmailToAddress);
		sprintf_P(GM862_tmpStr, PSTR("Data from Logger: %s"), LoggerName);
		GM862_SendFormatedString_P(PSTR_GM862_CMD_SMPT_DATA_HEAD_SUBJ, GM862_tmpStr);
		
		//Message body goes here:
		int ResVoltInt, ResVoltIntDec;
		_delay_ms(100);
		USART_tx_String_P(&USARTC0, PSTR("Sending Body Text.....\r\n"));
		
		ResVoltInt = (int)tmpBatV;
		ResVoltIntDec = ((tmpBatV-ResVoltInt)*10000.0);
		USART_printf_P(&USARTE0, "Logger Name: %s\r\n\r\nBattery Voltage: %d.%04d, (0x%02X)\r\nTime:", LoggerName, ResVoltInt, ResVoltIntDec, RST.STATUS);
		sPrintDateTime(s, &RTC_c_Time, "");
		USART_tx_String(&USARTE0, s);
		USART_tx_String_P(&USARTE0, PSTR("\r\n"));
		USART_tx_String(&USARTE0, PwrLvl);
		USART_tx_String_P(&USARTE0, PSTR("\r\n\r\nFileds:\r\nAddress\t\tFlags\tID\t\t\tTime\r\n"));
		
		Sent = 0;
		for (i = (records_sent_already == 0 ? 0 : records_sent_already - 1); i < RecordCount; i++) {
			_delay_ms(70);
			tmpFlags = loadRecord(i, &tmpTime, &tmpID);
			sPrintDateTime(GM862_tmpStr, &tmpTime, "");
			USART_printf_P(&USARTE0, "%08lX\t%02X\t%08lX\t%s\t\r\n", i, tmpFlags, tmpID, GM862_tmpStr);
			Sent++;
			if (Sent >= 1000) break;
		}
	
		USART_printf_P(&USARTC0, "At End... RC: %lu, RAS: %lu, i: %lu\r\n", RecordCount, records_sent_already, i);
		
		if ((a = GM862_SendCmdAndCheckStart_P(PSTR_GM862_CMD_SMPT_DATA_END, PSTR_GM862_RESPONSE_SMTP_250, &responsePtr, 20000)) < 0) GM862_Exit_With_PWRGD_Enable(); //return a;
		GM862_SendCmdAndCheckStart_P(PSTR_GM862_CMD_SMPT_QUIT, PSTR_GM862_RESPONSE_SMTP_221, &responsePtr, 20000);
	
		if (i < RecordCount) 
			records_sent_already = i;
		else
			records_sent_already = RecordCount;
		
	} while (records_sent_already < RecordCount);
	
	PWR_GOOD_INT_EN();
	return GM862_err_Success;
}