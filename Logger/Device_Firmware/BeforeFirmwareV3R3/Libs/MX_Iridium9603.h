/*
 *  MX_Iridium9603.h
 *
 *  Created: 07/03/2013 8:22:27 PM
 *  Author: Matthew Cochrane
 */ 


#ifndef MX_IRIDIUM9603_H_
#define MX_IRIDIUM9603_H_

//#include "MX_ErrorEnum.h"
#include "MX_USART.h"
#include "..\Strings.h"
#include "..\WIDLogV2.h"

typedef enum _Iridium_err_t {
	Iridium_err_SuccessAndUDTime	=  1,
	Iridium_err_Success			=  0,
	Iridium_err_Timeout			= -1,
	Iridium_err_USBUnplugged		= -2,
	Iridium_err_InvalidData		= -3,
	Iridium_err_BadResponse		= -5,
	Iridium_err_TimeNotUpdated	= -6,
	Iridium_err_Unknown			= -20
} Iridium_err_t;

//Module specifics

//Hardware definitions:
//USART:
#define IRIDIUM_USART_PORT			PORTE
#define IRIDIUM_USART				USARTE0
//Power Control Pin: (Active High)
#define IRIDIUM_PWR_CTRL_PORT		PORTC
#define IRIDIUM_PWR_CTRL_PIN_bp		6
#define IRIDIUM_PWR_CTRL_PIN_bm		(1<<IRIDIUM_PWR_CTRL_PIN_bp)
//#define IRIDIUM_PWR_ON()			do { IRIDIUM_PWR_CTRL_PORT.OUTCLR = IRIDIUM_PWR_CTRL_PIN_bm; /*Setup the USART 19200*/ USART_Setup(&IRIDIUM_USART_PORT, &IRIDIUM_USART, 622, -4, 0);} while(0)
#define IRIDIUM_PWR_ON()			do { IRIDIUM_PWR_CTRL_PORT.OUTCLR = IRIDIUM_PWR_CTRL_PIN_bm; /*Setup the USART 19200*/ USART_Setup(&IRIDIUM_USART_PORT, &IRIDIUM_USART, 609, -4, 0);} while(0)
#define IRIDIUM_PWR_OFF()			do { IRIDIUM_PWR_CTRL_PORT.OUTSET = IRIDIUM_PWR_CTRL_PIN_bm; IRIDIUM_USART_PORT.DIRCLR = (1<<2)|(1<<3);} while(0)
//On/Off Control Pin: (Active High)
#define IRIDIUM_ONOFF_PORT			PORTC
#define IRIDIUM_ONOFF_PIN_bp		7
#define IRIDIUM_ONOFF_PIN_bm		(1<<IRIDIUM_ONOFF_PIN_bp)
#define IRIDIUM_ONOFF_ON()			do { IRIDIUM_ONOFF_PORT.OUTCLR = IRIDIUM_ONOFF_PIN_bm; } while(0)
#define IRIDIUM_ONOFF_OFF()			do { IRIDIUM_ONOFF_PORT.OUTSET = IRIDIUM_ONOFF_PIN_bm; } while(0)

//Macros:

#define Iridium_Exit_With_PWRGD_Enable()							do {PWR_GOOD_INT_EN(); return a;} while (0)
#define Iridium_SendGeneralCmd_P(cmdstrP)							Iridium_SendCmdAndCheck_P(cmdstrP, PSTR_IRIDIUM_RESPONSE_OK, 3000)
#define Iridium_SendGeneralCmd_ret_P(cmdstrP, retVal)				(retVal = Iridium_SendCmdAndCheck_P(cmdstrP, PSTR_IRIDIUM_RESPONSE_OK, 3000))
// #define	GM862_SendFormatedString_P(cmdstrP, replacementText)		do { \
// 																		sprintf_P(GM862_tmpStr1, cmdstrP, replacementText); \
// 																		USART_tx_String(&USARTE0, GM862_tmpStr1); \
// 																	} while (0)
// #define	GM862_SendFormatedString_PP(cmdstrP, replacementTextP)		do { \
// 																		stringCopyP(replacementTextP, s); \
// 																		sprintf_P(GM862_tmpStr1, cmdstrP, s); \
// 																		USART_tx_String(&USARTE0, GM862_tmpStr1); \
// 																	} while (0)	
// #define	GM862_SendFormatedStringAndCheckStart_PP(cmdstrP, replacementTextP, responseStrP, retPtr, timeout, retVal)		\
// 																	do { \
// 																		stringCopyP(replacementTextP, s); \
// 																		sprintf_P(GM862_tmpStr1, cmdstrP, s); \
// 																		retVal = GM862_SendCmdAndCheckStart(GM862_tmpStr1, responseStrP, retPtr, timeout); \
// 																	} while (0)	
// #define	GM862_SendFormatedStringAndCheckStart_P(cmdstrP, replacementText, responseStrP, retPtr, timeout, retVal)		\
// 																	do { \
// 																		sprintf_P(GM862_tmpStr1, cmdstrP, replacementText); \
// 																		retVal = GM862_SendCmdAndCheckStart(GM862_tmpStr1, responseStrP, retPtr, timeout); \
// 																	} while (0)	

//Function Prototypes:
Iridium_err_t Iridium_SetupHardware();
Iridium_err_t Iridium_DisableHardware();
Iridium_err_t Iridium_HW_On();
Iridium_err_t Iridium_Unit_On();
Iridium_err_t Iridium_Unit_Off();
void Iridium_BridgeMode();
Iridium_err_t Iridium_SendCmdAndCheck_P(const char* cmdstrP, const char* expectedResponseP, uint16_t timeout);
Iridium_err_t Iridium_ConfigureModule();
// Iridium_err_t GM862_SendEmail();
// Iridium_err_t GM862_SendCmdAndCheckStart_P(const char* cmdstrP, const char* expectedResponseStartP, char** ReturnStr, uint16_t timeout);
// Iridium_err_t GM862_ListenForResponseAndCheckStart_P(const char* expectedResponseStartP, char** ReturnStr, uint16_t timeout);
// Iridium_err_t GM862_SendCmdAndCheckStart(char* cmdstr, const char* expectedResponseStartP, char** ReturnStr, uint16_t timeout);
// Iridium_err_t GM862_CalibratetoRTCTime();
// Iridium_err_t GM862_WaitForNITZ(uint16_t secTimeout);

//yeah
#endif /* MX_IRIDIUM9603_H_ */
