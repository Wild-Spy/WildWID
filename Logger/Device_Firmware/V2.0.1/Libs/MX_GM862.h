/*
 * MX_GM862.h
 *
 * Created: 15/06/2011 9:22:27 AM
 *  Author: Matthew Cochrane
 */ 


#ifndef MX_GM862_H_
#define MX_GM862_H_

//#include "MX_ErrorEnum.h"
#include "MX_USART.h"
#include "..\Strings.h"
#include "..\WIDLogV2.h"

typedef enum _GM862_err_t {
	GM862_err_SuccessAndUDTime	=  1,
	GM862_err_Success			=  0,
	GM862_err_Timeout			= -1,
	GM862_err_USBUnplugged		= -2,
	GM862_err_InvalidData		= -3,
	GM862_err_BadResponse		= -5,
	GM862_err_TimeNotUpdated	= -6,
	GM862_err_Unknown			= -20
} GM862_err_t;

//Module specifics
#define GM862_VER_GPS		0

//Hardware definitions:
//USART:
#define GM862_USART_PORT		PORTE
#define GM862_USART				USARTE0
//Power Control Pin: (Active High)
#define GM862_PWR_CTRL_PORT		PORTC
#define GM862_PWR_CTRL_PIN_bp	6
#define GM862_PWR_CTRL_PIN_bm	(1<<GM862_PWR_CTRL_PIN_bp)
//On/Off Control Pin: (Active High)
#define GM862_ONOFF_PORT		PORTC
#define GM862_ONOFF_PIN_bp		7
#define GM862_ONOFF_PIN_bm		(1<<GM862_ONOFF_PIN_bp)

//Macros:
#define GM862_Exit_With_PWRGD_Enable()								do {PWR_GOOD_INT_EN(); return a;} while (0);
#define GM862_SendGeneralCmd_P(cmdstrP)								GM862_SendCmdAndCheck_P(cmdstrP, PSTR_GM862_RESPONSE_OK, 3000)
#define GM862_SendGeneralCmd_ret_P(cmdstrP, retVal)					(retVal = GM862_SendCmdAndCheck_P(cmdstrP, PSTR_GM862_RESPONSE_OK, 3000))
#define	GM862_SendFormatedString_P(cmdstrP, replacementText)		do { \
																		sprintf_P(GM862_tmpStr1, cmdstrP, replacementText); \
																		USART_tx_String(&USARTE0, GM862_tmpStr1); \
																	} while (0)
#define	GM862_SendFormatedString_PP(cmdstrP, replacementTextP)		do { \
																		stringCopyP(replacementTextP, s); \
																		sprintf_P(GM862_tmpStr1, cmdstrP, s); \
																		USART_tx_String(&USARTE0, GM862_tmpStr1); \
																	} while (0)	
#define	GM862_SendFormatedStringAndCheckStart_PP(cmdstrP, replacementTextP, responseStrP, retPtr, timeout, retVal)		\
																	do { \
																		stringCopyP(replacementTextP, s); \
																		sprintf_P(GM862_tmpStr1, cmdstrP, s); \
																		retVal = GM862_SendCmdAndCheckStart(GM862_tmpStr1, responseStrP, retPtr, timeout); \
																	} while (0)	
#define	GM862_SendFormatedStringAndCheckStart_P(cmdstrP, replacementText, responseStrP, retPtr, timeout, retVal)		\
																	do { \
																		sprintf_P(GM862_tmpStr1, cmdstrP, replacementText); \
																		retVal = GM862_SendCmdAndCheckStart(GM862_tmpStr1, responseStrP, retPtr, timeout); \
																	} while (0)	

//Function Prototypes:
GM862_err_t GM862_SetupHardware();
GM862_err_t GM862_DisableHardware();
GM862_err_t GM862_HW_On();
GM862_err_t GM862_Unit_On(uint8_t updateRTC);
GM862_err_t GM862_Unit_Off();
void GM862_BridgeMode();
GM862_err_t GM862_SendCmdAndCheck_P(char* cmdstrP, char* expectedResponseP, uint16_t timeout);
GM862_err_t GM862_ConfigureModule();
GM862_err_t GM862_SendEmail();
GM862_err_t GM862_SendCmdAndCheckStart_P(char* cmdstrP, char* expectedResponseStartP, char** ReturnStr, uint16_t timeout);
GM862_err_t GM862_ListenForResponseAndCheckStart_P(char* expectedResponseStartP, char** ReturnStr, uint16_t timeout);
GM862_err_t GM862_SendCmdAndCheckStart(char* cmdstr, char* expectedResponseStartP, char** ReturnStr, uint16_t timeout);
GM862_err_t GM862_CalibratetoRTCTime();
GM862_err_t GM862_WaitForNITZ(uint16_t secTimeout);

//yeah
#endif /* MX_GM862_H_ */
