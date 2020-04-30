/* This file has been prepared for Doxygen automatic documentation generation.*/
/*! \file *********************************************************************
 *
 * \brief  WID Logger main source code file.
 *
 *      This file contains the main function as well as the majority of the 
 *		device specific code for the WID Logger.
 *
 * \par Documentation
 *      No further documentation as of yet.
 *
 * \author
 *      Matthew Cochrane \n
 *      Wild Spy
 *
 * $Revision: 0 $
 * $Date: 01/10/2011 $  \n
 *
 *****************************************************************************/

#include "WIDLogV2.h"

uint8_t deadByte;
char s[80];					
uint8_t myRF_ID[5];						//!<ID on the nordic chip default = {0xE6,0xE6,0xE6,0xE6,0xE6};
uint8_t myRF_Chan = 2;					//!<Channel on the nordic chip
char LoggerName[20] = "TestLogger";		//!<Used to ID the logger 19 characters long (20 with the the '\0')

volatile uint16_t timer10ms = 0;		//!<Used to keep track of how many 10ms blocks have passed since the 10ms timer was started.

volatile uint8_t Wakeup_Source = WAKEUP_SOURCE_NONE;	//!<Holds information on what source woke up the device from sleep mode.

uint32_t RecordCount = 0L;				//!<No. of records in dataflash
uint8_t gotRecCnt = 0;					//!<Have we found the record count yet?
uint8_t TagUpdate = 0;					//!<Counter.. when it gets to 0, the tagupdate function is executed

uint8_t Startup = 1;					//!<About to startup?

uint8_t CheckBatVolt = 0;				//!<Counter for checking battery voltage (checks every 5? seconds)
float tmpBatV;							//!<The last read battery voltage.

RTC_Time_t RTC_c_Time;					//!<RTC time -> real time stored here.
RTC_Time_t RTC_Period;					//!<Time that elapses per RTC period (1 second)

RTC_Time_t Send_Data_GPRS_Next;			//!<Next time to attempt to send an email
RTC_Time_t Send_Data_GPRS_Next_Real;	//!<Next scheduled time to send an email
RTC_Time_t Send_Data_GPRS_Period;		//!<An email is scheduled to be sent once every period
uint8_t Send_Data_GPRS_Retries_Remaining = SEND_DATA_GPRS_RETRIES_MAX;	//!<Number of retries remaining before aborting this email and waiting for next scheduled send email time

//RTC_Time_t RecCount_LastSave;			//!<Time when RecordCount was last saved to EEPROM

uint8_t TagDispMode = 1;				//!<How tags are displayed (None (1), show ID only(2), ID and date/time or ID(3), Date/Time and Address(4))

uint8_t bBak_Mode = 0;					//!<Are we in battery backup mode?
uint8_t bBak_Reason = BBAK_REASON_NONE;	//!<Why are we in battery backup mode?
uint8_t PC_saved[3];					//!<Saved address of Program Counter (pointer to address just before program goes to sleep) used to unsafely abort all operations and put the device to sleep immediately!
uint16_t SP_saved;						//!<Saved stack pointer address, used in conjunction with PC_saved variable above.
volatile uint8_t batBakTrig = 2;		//!<Variable used in managing battery backup modes

int ADCOffset = 0;						//!<Offset of the ADC (see datasheet on ADC offset)
//uint8_t PowerDown = 0;				//!<Flag used to tell the unit to power down when appropriate
uint8_t LoggingEnabled;// = 1;			//!<Is logging enabled? Can still use as tracking device but won't save to flash (initially set to 1 (GUI_LOGSTATEDIT_DISABLED)
uint8_t EnterGM862BridgeMode = 0;		//!<Set in the SerComms library, tells the logger to enter GM862 Bridge mode which can be exited by sending 0x03 from the computer
uint8_t EnterGM862ConfigMode = 0;

char Filter[8] = {'*','*','*','*','*','*','*','*'};	//!<Filter String
uint32_t FilterCode;								//!<Filter Code
uint32_t FilterMask;								//!<Used to 

//Variable declarations (not definitions.. they are defined in other c files)
extern ListStruct_t MList_Data[LISTMAXSIZE];		//!<From M_List.c
extern int MList_Count;								//!<From M_List.c
extern uint32_t records_sent_already;				//!<From MX_GM862.c
extern char EmailToAddress[60];						//!<From MX_GM862.c
extern char GPRS_APN[30];							//!<From MX_GM862.c
extern char GPRS_UserId[30];						//!<From MX_GM862.c
extern char GPRS_Passw[30];							//!<From MX_GM862.c

ISR(RTC_OVF_vect) {
	//PORTC.OUTSET = (1<<1);
	Wakeup_Source |= WAKEUP_SOURCE_RTC;
	AddTimes(&RTC_c_Time, &RTC_Period);
}

ISR(PORTE_INT0_vect) {
	//USB plugged in/removed (both events trigger the interrupt)
	CheckBatVolt = BATVOLTCHECK;
	Wakeup_Source |= WAKEUP_SOURCE_RTC;	//so that we do RTC wake checks
	if (USART_USB_IN) {
		//Plugged in
		USART_Setup(&PORTC, &USARTC0, 176, -5, 0);
		//PORTC.PIN2CTRL = PORT_OPC_TOTEM_gc;
		//PORTC_OUTCLR = (1<<2);
	} else {
		//Unplugged
		PORTC.DIRSET = ((1<<2)|(1<<3));
		PORTC.OUTSET = ((1<<2)|(1<<3));
		//PORTC.PIN2CTRL = PORT_OPC_TOTEM_gc;
		USARTC0.CTRLB = 0x00;
	}
}

ISR(PORTB_INT0_vect) {//, ISR_NAKED) {
	//Main Battery Removed/Power lost

	//DO NOT TOUCH THIS FUNCTION!!!!!
	//DO NOT ADD ANY VARIABLES, DON'T CHANGE ANY CODE...
	//JUST DONT FUCKING TOUCH IT!  SHIT WILL EXPLODE!
	uint8_t* RetADD;
	
	//alternate way:
	//RetADD = (uint8_t*)((SPH<<8)+SPL+17);
	
	//Interesting to note that on startup.. not sure when.., but this interrupt is executed.  I think the only reason that
	//battery backup mode is not entered is because of the 100ms delay and then batteryGood re-check in the EnterBatBakMode()
	//function.. I guess that means that that bit of code works quite well!
 
	//Just assume a hard off... if it's plugged in the nordic chip will keep running. This is OK.
	//Otherwise we need to make sure that we handle the case that the USB power could be lost while
	//shutting down the peripherals.  Unlikely, but possible.
	if (bBak_Mode == 0 && bBak_Reason == 0) {
		if (EnterBatBakMode(BBAK_REASON_NO_BAT)) {
			return;
		}
	//Can't abort this function.. we we can but if we do we need to put all the registers and the stack back to normal first!
	//Just disable the interrupt during sections of code where you don't want it to execute.
	} else {
		//reti();
		//abort
		return;
	}
	//if we canceled in EnterBatBakMode()
	if (bBak_Mode == 0 && bBak_Reason == 0) {
		//reti();
		//abort
		return;
	}

	//if we get here we want to hard abort!
	//Set return address to Saved Program Counter Address
	//Note that it doesn't matter what other crap is on the stack if you use reti(), it just uses the top of the stack
	//as the return address
	RetADD = (uint8_t*)((SPH<<8)+SPL);
	*(RetADD+0) = PC_saved[0];
	*(RetADD+1) = PC_saved[1];
	*(RetADD+2) = PC_saved[2];
	reti();
	
	//alternate way:
	//*(RetADD+0) = PC_saved[0];
	//*(RetADD+1) = PC_saved[1];
	//*(RetADD+2) = PC_saved[2];
	//return;
	
}

ISR(PORTC_INT0_vect) {
	Wakeup_Source |= WAKEUP_SOURCE_USART;
}

ISR(PORTD_INT0_vect) {
	Wakeup_Source |= WAKEUP_SOURCE_NORDIC;
}

ISR(TCC0_OVF_vect) {
	//10ms Timer update.
	cli();
	if (timer10ms) timer10ms--;
	else {
		TC0_ConfigClockSource(&TCC0, TC_CLKSEL_OFF_gc);
		TCC0.INTCTRLA = TC_OVFINTLVL_OFF_gc;
	}
	sei();
}

//! Main function
/*!
  A more elaborate description. (can be multiple lines).
  \return 0
*/
int main() {
	
	SAVESP();
	PC_saved[0] = 0;
	PC_saved[1] = 0;
	PC_saved[2] = 0;

	SetupHardware();
	
	while (1) {

		//Set restore point...
		if (batBakTrig == 2) {
			SAVESTATE();
			if (batBakTrig == 1) {
				//Restore Stack Pointer
				cli();
				SPH = (uint8_t)(SP_saved>>8);
				SPL = (uint8_t)(SP_saved);
				sei();
			}
			batBakTrig = 0;
		}

		//Clear Nordic IRQ flag
		PORTD.INTFLAGS = 0x01;
		//Go to sleep
		set_sleep_mode(SLEEP_SMODE_PSAVE_gc);
		MCU_STAT_LED_OFF();
		sleep_mode();

		if (bBak_Reason == BBAK_REASON_NONE) {
			MainLoop_Active_Mode();
		} else if (bBak_Reason == BBAK_REASON_NO_BAT) {
			//We are in battery backup mode
			MainLoop_BatBackup_NoBat_Mode();
		} else if (bBak_Reason == BBAK_REASON_LO_BAT) {
			MainLoop_BatBackup_LoBat_Mode();
		}
	}

	return 0;
}

//! Brief Description
/*!
  A more elaborate description. (can be multiple lines).
  \return Desctiption as void 
*/
void MainLoop_Active_Mode() {
	MCU_STAT_LED_ON();
			
	if (Startup) {
		//Stuff to do on startup!!
		//TODO[ ]: TEST AGAIN THAT THIS TURNS OFF WHEN DONE!!!
		//TODO[ ]: uncomment the below lines!!
		GM862_Unit_On(1);
		GM862_Unit_Off();
		SAVE_SYS_MSG(SYS_MSG_PWR_ON,0);
		
		tmpBatV = CheckBatteryVoltage();
				
		USART_printf_P(&USARTC0, "RecordCount: %lu\r\n", RecordCount);
		USART_tx_String_P(&USARTC0, PSTR("Going to sleep. Turn device on for serial Comms.\r\n"));
		_delay_ms(2);
	
		Startup = 0;
	}

	//This code just ignores the 'false byte' received by the USART
	//receives upon waking up from battery backup mode.
	if (Wakeup_Source & WAKEUP_SOURCE_PWR) {
		Wakeup_Source &= ~(WAKEUP_SOURCE_PWR);
		Wakeup_Source &= ~(WAKEUP_SOURCE_USART);
	}

	//Pretty much just for PrintRTC and TagUpdate stuff
	//The actual time updating is handled in the ISR
	if (Wakeup_Source & WAKEUP_SOURCE_RTC) {
		Wakeup_Source &= ~(WAKEUP_SOURCE_RTC);
		RTCWakeChecks();
	}

	//TODO[ ]: Consider removing the loop or putting a limit to the number of
	//		   loops as otherwise bombardment of tags could cause the device to
	//		   stop responding (get stuck in the below loop and never service
	//		   the USART.
	//Nordic - is IRQ low?, if so, do work!
	while ((PORTD.IN & (1<<NRF_IRQ)) == 0) {
		doRFWork(TagDispMode);
	}
	Wakeup_Source &= ~(WAKEUP_SOURCE_NORDIC);

	//USART stuff.  This needs to be last.
	if (Wakeup_Source & WAKEUP_SOURCE_USART) {
		//ShowMenu();
		SerCom_err_t tmpErr;
		while (USART_rx_Byte_nb(&USARTC0, &deadByte) == USART_err_Success);
		MCU_STAT_LED_ON();
		if ((tmpErr = Ser_CMDHandler(&USARTC0)) != SerCom_err_Success) {
			Wakeup_Source &= ~(WAKEUP_SOURCE_NORDIC);
		}
		Wakeup_Source &= ~(WAKEUP_SOURCE_USART);
		MCU_STAT_LED_OFF();
	}
	
	if (EnterGM862BridgeMode) {
		GM862_HW_On();
		//GM862_Unit_On(1);
		//GM862_SendEmail();
		GM862_BridgeMode();
		GM862_Unit_Off();
		EnterGM862BridgeMode = 0;
	}
	
	if (EnterGM862ConfigMode) {
		USART_tx_String_P(&USARTC0, PSTR("Configure GM862...\r\n"));
		GM862_HW_On();
		GM862_ConfigureModule();
		GM862_Unit_Off();
		USART_tx_String_P(&USARTC0, PSTR("GM862 Configure Done.\r\n"));
		_delay_ms(2);
		EnterGM862ConfigMode = 0;
	}
	
}

//! Brief Description
/*!
  A more elaborate description. (can be multiple lines).
  \return Desctiption as void 
*/
void MainLoop_BatBackup_NoBat_Mode() {
	if (PWR_GOOD) {
		//bBak_Reason = BBAK_REASON_NONE;
		//batBakMode = 0;
		//if batbak interrupt was called while we were waking up, exit!
		//Wakeup_Source |= WAKEUP_SOURCE_PWR;
		if (WakeFromBatBakMode() == 0) {		
			if (!Startup) {
				if (GM862_Unit_On(1) >= 0) {
					GM862_Unit_Off();
					//Log record.. time WAS updated from GSM
					SAVE_SYS_MSG(SYS_MSG_WAKE_UD_TIME, 0);
				} else {
					//Log record.. time was NOT updated from GSM
					SAVE_SYS_MSG(SYS_MSG_WAKE_NOTIME, 0);
				}
			}
			RTC_Time_t tmpTime;
			TimeAfterTime(&RTC_c_Time, &tmpTime, 0, 0, 0, 0, 15, 0);
				
			if (FirstTimeGreaterOrEqualTo(&tmpTime, &Send_Data_GPRS_Next)) {
				CopyTime(&tmpTime, &Send_Data_GPRS_Next);
				ReelInTheYears();
			}			

			_delay_ms(1);
			USART_tx_String_P(&USARTC0, PSTR("Wake from bat back mode.\r\n"));
			_delay_ms(2);
		}		
	}
}

void MainLoop_BatBackup_LoBat_Mode() {
	//We are in soft battery backup mode
	//MCU_STAT_ON();
	//_delay_ms(5);
	CheckBatVolt++;
	if (CheckBatVolt >= BATVOLTCHECK) {
		CheckBatVolt = 0;
		bBak_Reason = BBAK_REASON_NONE;
		tmpBatV = CheckBatteryVoltage();
		if (tmpBatV > 11.0) {
			//Good Battery
			WakeFromBatBakMode();
			//_delay_ms(1);
			//USART_tx_String_P(&USARTC0, PSTR("Wake from bat back mode.\r\n"));
			//_delay_ms(2);
		} /*else if (tmpBatV < 10.5) {
			//Shit is about to explode.  Shut down! Permanently!
			//disable RTC
			CLKSYS_Disable(OSC_XOSCEN_bm);
			cli();
			SMPS_OFF();
			//PORTE.DIRSET = (1<<1);
			//PORTE.OUTSET = (1<<1);

			//Perma-off
			while (1) {
				set_sleep_mode(SLEEP_SMODE_PDOWN_gc);
				sleep_mode();
			}
		}*/
		}
}

void SetupHardware() {

	//TODO[ ]: Tidy this function up
	//TODO[ ]: Add more comments to this function

	cli();
	//'Power Good' pin setup (PB2) as input
	PWR_GD_PORT.DIRCLR = (1<<PWR_GD_PIN);
	//'Power Good' interrupt setup (PB2)
	PORTCFG.MPCMASK = (1<<PWR_GD_PIN);
	PWR_GD_PORT.PIN0CTRL = PORT_ISC_FALLING_gc;
	PWR_GD_PORT.INT0MASK = (1<<PWR_GD_PIN);
	//Enable the interrupt
	PWR_GD_PORT.INTCTRL = PORT_INT0LVL_MED_gc;

	//'USB Plugged In' pin setup (PE0)
	USART_USB_IN_PORT.DIRCLR = (1<<USART_USB_IN_PIN);
	//'USB Plugged In' interrupt setup (PE0)
	PORTCFG.MPCMASK = (1<<USART_USB_IN_PIN);
	USART_USB_IN_PORT.PIN0CTRL = PORT_ISC_BOTHEDGES_gc;
	USART_USB_IN_PORT.INT0MASK = (1<<USART_USB_IN_PIN);
	//Enable the interrupt
	USART_USB_IN_PORT.INTCTRL = PORT_INT0LVL_LO_gc;

	//TODO[ ]: Disable INT0 (PwrGd) interrupt before going to sleep? why?... NO!!? then it won't be able to wake up again?

	//Nordic IRQ pin setup
	PORTCFG.MPCMASK = (1<<NRF_IRQ);
	PORTD.PIN0CTRL = PORT_OPC_PULLUP_gc|PORT_ISC_LEVEL_gc;
	PORTD.DIRCLR = (1<<NRF_IRQ);
	PORTD.INT0MASK = (1<<NRF_IRQ);
	PORTD.INTCTRL |= PORT_INT0LVL_MED_gc;

	//Battery voltage measurement EN - setup and turn off
	BATV_READ_EN_PORT.DIRSET = (1<<BATV_READ_EN_PIN);
	BATV_READ_EN_PORT.OUTCLR = (1<<BATV_READ_EN_PIN);

	//SMPS Enable Pin - setup and enable SMPS
	SMPS_PWR_PORT.DIRSET = (1<<SMPS_PWR_PIN);
	SMPS_PWR_PORT.OUTCLR = (1<<SMPS_PWR_PIN);

	//USART Rx pin interrupt
	PORTC.PIN2CTRL = PORT_ISC_RISING_gc; //|PORT_OPC_PULLDOWN_gc
	PORTC.INT0MASK = (1<<2);
	PORTC.INTCTRL = PORT_INT0LVL_LO_gc;

	//Clock Setup
	//12MHz - 2MHz Internal RC Oscillator and PLL
	SetClkSpeed(CLK_SPD_12MHz_SETUP);

	//USART Setup for 115200bps
	USART_Setup(&PORTC, &USARTC0, 176, -5, 0);
	USART_tx_String_P(&USARTC0, PSTR("Ser Coms Init\r\n"));

	//Setup RTC
	CLKSYS_XOSC_Config(OSC_FRQRANGE_04TO2_gc, true, OSC_XOSCSEL_32KHz_gc);
	CLKSYS_Enable(OSC_XOSCEN_bm);
	while (CLKSYS_IsReady(OSC_XOSCRDY_bm) == 0);
	CLKSYS_RTC_ClockSource_Enable(CLK_RTCSRC_TOSC32_gc);
	RTC_Setup();
	
	//Enable interrupts
	PMIC.CTRL = PMIC_HILVLEN_bm|PMIC_MEDLVLEN_bm|PMIC_LOLVLEN_bm;
	sei();
	
	//PB3 as output (Debugging LED)
	//on after CLKSYS_Enable(OSC_XOSCEN_bm);
	PORTB.DIRSET = (1<<3);
	MCU_STAT_LED_ON();

	//Load values from EEPROM
	ReadEEPROM(myRF_ID,5,RFID_ADD);
	ReadEEPROM(&myRF_Chan,1,RFCHAN_ADD);
	//RCFROMEEPROM();	//use 	FindRecordCount();
	//LOGMODEFROMEEPROM();
	LoggingEnabled = 2;
	READFILTEREEPROM();
	FilterStr2Mask(Filter);
	GPRSSENDNEXTFROMEEPROM();
	CopyTime(&Send_Data_GPRS_Next, &Send_Data_GPRS_Next_Real);
	GPRSSENDPERFROMEEPROM();
	LOGGERNAMEFROMEEPROM();
	EMAILTOADDFROMEEPROM();
	GPRSSETINGSFROMEEPROM();

	if (PWR_GOOD) {
		//Setup Nordic RF Chip
		NRF24L01_SPI_Setup();
		NRF24L01_SetupnRF24L01(myRF_Chan, myRF_ID);
		NRF24L01_Flush_RX_FIFO();
		NRF24L01_W_Reg(NREG_STATUS, 0x40);
		//Setup Serial Flash
		SerFlash_SPI_Setup();		
		RecordCount = FindRecordCount();
		gotRecCnt = 1;
	}	

	PMIC.CTRL = PMIC_HILVLEN_bm|PMIC_MEDLVLEN_bm|PMIC_LOLVLEN_bm;

	USART_tx_String_P(&USARTC0, PSTR("HW Setup Success!\r\n"));

	if (RecordCount == 0xFFFFFFFFl)
		RecordCount = 0x00000000l;
	
	//Setup GM862 hardware
	GM862_SetupHardware();
	
	//Initialise list
	mList_Init();

	//Put Nordic Into RX mode
	if (LOGGINGISENABLED && PWR_GOOD) NRF_RXMODE;
	
	if (!PWR_GOOD) {
		//If the main battery hasn't been plugged in yet... start up in battery backup mode
		EnterBatBakMode(BBAK_REASON_NO_BAT);
	}
	
}

uint8_t EnterBatBakMode(uint8_t Reason) {
	
	_delay_ms(100);
	
	if (Reason == BBAK_REASON_NO_BAT) {
		if (PWR_GOOD) {
			//abort!
			return 1;
		}
	}
	
	bBak_Mode = 1;
	bBak_Reason = Reason;

	//Save record count!
	RCTOEEPROM();

	//Power down the nordic chip
	if (PWR_GOOD) {
		NRF_POWERDOWN;
	}
	//disable NORDIC IRQ interrupt - keep power switch interrupt enabled!
	PORTD.INTCTRL = PORT_INT0LVL_OFF_gc;//|PORT_INT1LVL_MED_gc;
	//No pullup on nordic IRQ
	PORTCFG.MPCMASK = (1<<NRF_IRQ);
	PORTD.PIN0CTRL = PORT_ISC_LEVEL_gc;

	//SerFlash_DeepPwrDwn(1);

	//If the battery voltage is too low, show low battery message before
	//turning off!
	if (Reason == BBAK_REASON_LO_BAT) {
		USART_tx_String_P(&USARTC0, PSTR("Low Battery!\r\n"));
	} else {
		USART_tx_String_P(&USARTC0, PSTR("PWRDWN!\r\n"));	
	}
	_delay_ms(5);

	//Battery voltage measurement EN - setup and turn off
	BATV_READ_EN_PORT.DIRSET = (1<<BATV_READ_EN_PIN);
	BATV_READ_EN_PORT.OUTCLR = (1<<BATV_READ_EN_PIN);

	//SMPS Enable Pin - setup and enable SMPS
	SMPS_PWR_PORT.DIRSET = (1<<SMPS_PWR_PIN);
	SMPS_PWR_PORT.OUTCLR = (1<<SMPS_PWR_PIN);

	//NRF_CSN, MOSI and USBTX (tx to computer) as inputs - so no leakage through
	//protection diodes on other devices.
	//SF_CSN as input - same reason as above
	PORTD.DIRCLR = (1<<NRF_CSN)|(1<<NRF_MOSI)|(1<<SerFlash_CSN)|(1<<NRF_CE);
	//USB_RX - input
	PORTC.DIRCLR = (1<<3);

	//Disable PWRGD interrupt
	PWR_GD_PORT.INTCTRL = PORT_INT0LVL_OFF_gc;
	//USB Detect.. disable (was set to stay enabled?!?)
	USART_USB_IN_PORT.INTCTRL = PORT_INT0LVL_OFF_gc;//PORT_INT0LVL_LO_gc;
	
	//Disable GM862 - quick off
	GM862_DisableHardware();
	
	//Turn Power OFF
	if (Reason == BBAK_REASON_SOFT_OFF || Reason == BBAK_REASON_LO_BAT)
		SMPS_OFF();
	CheckBatVolt = 0;
	Wakeup_Source &= ~(WAKEUP_SOURCE_PWR);
	
	return 0;
	
}

uint8_t WakeFromBatBakMode() {
	
	_delay_ms(5);
	
	if (!PWR_GOOD)
		return 1;
	
	//Turn Power ON
	SMPS_ON();
	
	SetClkSpeed(CLK_SPD_12MHz_SETUP);

	//Clear PWRGD interrupt
	PWR_GD_PORT.INTFLAGS = PORT_INT0IF_bm;
	//Enable PWRGD interrupt
	PWR_GD_PORT.INTCTRL = PORT_INT0LVL_MED_gc;
	//Enable USB_IN interrupt
	USART_USB_IN_PORT.INTCTRL = PORT_INT0LVL_LO_gc;

	//Setting up nordic stuff.. a little redundant?!?! seeming that we set it up below.  Do we set the IRQ interrupt in the setup function below?
	PORTD.DIRSET = (1<<NRF_CSN)|(1<<NRF_MOSI)|(1<<SerFlash_CSN);
	PORTD.PIN2CTRL = PORT_OPC_PULLUP_gc|PORT_ISC_LEVEL_gc;
	//For USB_RX
	PORTC.DIRSET = (1<<3);	

	//Load values from EEPROM
	//ReadEEPROM(myRF_ID,5,RFIDADD);
	//ReadEEPROM(&myRF_Chan,1,RFCHANADD);
	//RCFROMEEPROM();
	//FILTFROMEEPROM();
	//LOGMODEFROMEEPROM();
	//for (uint8_t i = 0; i < 5; i++)
	//	ReadFilterEEPROM(i);

	//USART Setup for 115200bps
	//USART_Setup(&PORTC, &USARTC0, 176, -5, 0);
	//USART_tx_String_P(&USARTC0, PSTR("Ser Coms Init\r\n"));

	//Setup Nordic RF Chip
	NRF24L01_SPI_Setup();
	NRF24L01_SetupnRF24L01(myRF_Chan, myRF_ID);
	NRF24L01_Flush_RX_FIFO();
	NRF24L01_W_Reg(NREG_STATUS, 0x40);
	PORTD.INTCTRL |= PORT_INT0LVL_MED_gc;	//should this be done in the nordic setup function?

	//Setup Serial Flash
	SerFlash_SPI_Setup();
	
	if (gotRecCnt == 0) {
		RecordCount = FindRecordCount();
	}

	USART_tx_String_P(&USARTC0, PSTR("Awake...\r\n"));

	//CalibrateADCOffset();		//Don't do this here.  If the power connection is dodgey then running this line of code will cause it to get stuck between power down mode and normal mode!
	//tmpBatV = CheckBatteryVoltage();
	
	//Setup GM862 hardware
	GM862_SetupHardware();

	//If logging is enabled or enabled and filtered turn on nordic chip
	if (LOGGINGISENABLED && PWR_GOOD) {
		NRF24L01_SPI_Setup();
		NRF24L01_SetupnRF24L01(myRF_Chan, myRF_ID);
		NRF_RXMODE;
	}

	CheckBatVolt = 0;
	bBak_Mode = 0;
	bBak_Reason = BBAK_REASON_NONE;
	Wakeup_Source |= WAKEUP_SOURCE_PWR;
	return 0;
}

/*!	\brief Get the main battery voltage in volts.
 *
 *	\return Battery voltage, in volts, as a floating point number.
 */
float CheckBatteryVoltage() {
	int ADCResult;
	float ResultVolt;
	//int ResVoltInt, ResVoltIntDec;

	PORTA.DIRCLR = (1<<7);//|(1<<6);//if you clear (1<<6) you'll stop a Row/Column of the keypad from working!
	ADCA.CTRLB =  ADC_RESOLUTION_12BIT_gc;//|(1<<4);
	ADCA.REFCTRL = ADC_REFSEL_INT1V_gc|ADC_BANDGAP_bm;//ADC_REFSEL_VCC_gc; //Vcc/1.6
	CLKSYS_Prescalers_Config(CLK_PSADIV_1_gc, CLK_PSBCDIV_1_1_gc);
	ADCA.PRESCALER = ADC_PRESCALER_DIV128_gc; //93.750 kHz
	ADCA.CH0.CTRL = ADC_CH_GAIN_1X_gc|ADC_CH_INPUTMODE_SINGLEENDED_gc;
	ADCA.CH0.MUXCTRL = ADC_CH_MUXPOS_PIN7_gc;	//SHOULD BE PIN 7!!!!!!!!!!!!!!!!!!!!!!!! TESTING WITH PIN 6

	//ADC Calibration Bytes
	ADCA.CALL = ReadCalibrationByte( offsetof(NVM_PROD_SIGNATURES_t, ADCACAL0) );
	ADCA.CALH = ReadCalibrationByte( offsetof(NVM_PROD_SIGNATURES_t, ADCACAL1) );

	//Enable
	ADCA.CTRLA |= ADC_ENABLE_bm;

	//PORTE0 - MOSFET for Bat Voltage Read
	BATV_READ_EN_PORT.OUTSET = (1<<BATV_READ_EN_PIN);
	_delay_ms(2);

	ADCResult = ADCRead(8)-ADCOffset;//-49;//- ADCOffset;

	//			  		(Vadc*ConvFactor)+Vdiode
	ResultVolt = (((float)ADCResult)/4096.0)*19.611+0.0562;
	//ResVoltInt = (int)ResultVolt;
	//ResVoltIntDec = ((ResultVolt-ResVoltInt)*10000.0);

	//CLKSYS_Prescalers_Config(CLK_PSADIV_1_gc, CLK_PSBCDIV_1_1_gc);
	//USART_Setup(&PORTC, &USARTC0, 176, -5);

	//USART_printf_P(&USARTC0, "ADC: 0x%04X = %d", ADCResult, ResVoltInt);
	//USART_printf_P(&USARTC0, ".%04d\r\n", ResVoltIntDec);
	//_delay_ms(2);

	ADCA.CTRLA = 0;
	BATV_READ_EN_PORT.OUTCLR = (1<<BATV_READ_EN_PIN);

	return ResultVolt;
}

void CalibrateADCOffset() {

	PORTA.DIRCLR = (1<<7);
	ADCA.CTRLB =  ADC_RESOLUTION_12BIT_gc;//|(1<<4);
	ADCA.REFCTRL = ADC_REFSEL_INT1V_gc|ADC_BANDGAP_bm;//ADC_REFSEL_VCC_gc; //Vcc/1.6ADC_REFSEL_INT1V_gc|ADC_BANDGAP_bm;
	//CLKSYS_Prescalers_Config(CLK_PSADIV_1_gc, CLK_PSBCDIV_1_1_gc);
	ADCA.PRESCALER = ADC_PRESCALER_DIV128_gc; //5.859375 kHz
	ADCA.CH0.CTRL = ADC_CH_GAIN_1X_gc|ADC_CH_INPUTMODE_SINGLEENDED_gc;
	ADCA.CH0.MUXCTRL = ADC_CH_MUXPOS_PIN7_gc;

	//ADC Calibration Bytes
	ADCA.CALL = ReadCalibrationByte( offsetof(NVM_PROD_SIGNATURES_t, ADCACAL0) );
	ADCA.CALH = ReadCalibrationByte( offsetof(NVM_PROD_SIGNATURES_t, ADCACAL1) );

	//Enable
	ADCA.CTRLA |= ADC_ENABLE_bm;

	//PORTE0 - MOSFET for Bat Voltage Read
	//PORTE.OUTSET = (1<<0);
	//PORTA.DIRCLR = (1<<5);
	//PORTA.PIN5CTRL = PORT_OPC_PULLDOWN_gc;
	_delay_ms(2);

	ADCOffset = ADCRead(12);

	//WriteEEPROM((uint8_t*)&ADCOffset,2,ADCOFFSETV);

	//PORTA.PIN5CTRL = PORT_OPC_TOTEM_gc;
	//CLKSYS_Prescalers_Config(CLK_PSADIV_1_gc, CLK_PSBCDIV_1_1_gc);

	ADCA.CTRLA = 0;
	//PORTE.OUTCLR = (1<<0);

}

/*!	\brief Read the voltage on ADC A, Channel 0.
 *
 *	Take 2^SAMP samples of the pin voltage and average them
 *
 *	\param SAMP number of samples = 2^SAMP
 *	\return Voltage on ADC A, Channel 0.
 */
int ADCRead(uint8_t SAMP) {//ADC_CH_t* ADCChan, uint8_t SAMP) {
	long ADCResult = 0;
	int Res;
	int i;

	for (i = 0; i < (1<<SAMP); i++) {
		ADCA.CH0.CTRL |= ADC_CH_START_bm;	//Start a conversion on CH0
		while (!(ADCA.CH0.INTFLAGS));
		Res = ADCA.CH0RES;
		//if (Res < ADCResult)
			ADCResult += Res;
		_delay_us(100);
	}
	ADCResult = ADCResult>>SAMP;//(1<<SAMP);
	return (uint16_t)ADCResult;
}

/*!	\brief Performs many checks every time the RTC ticks.
 *
 *	This function is called only if the device is in active mode (ie not in battery backup
 *	mode) once every RTC tick period (which is once a second).  It does the following tasks:
 *	- Check that the nordic chip is working properly and fix it if it's not.
 *	- Update the tag list once every #TAGLISTUPDATE seconds.
 *	- Handle all the SendEmail stuff****
 *	- Check the battery voltage once every #BATVOLTCHECK seconds and if it's too low, put the 
 *	device into battery backup mode.
 *
 */
void RTCWakeChecks() {
	//TODO[ ]: Is this needed??
	//if (((tmpByte = NRF24L01_R_Reg(NREG_STATUS))&0b01000000)) {
	if (!(NRF24L01_R_Reg(NREG_FIFOSTATUS)&(1<<0))) {
		if ((PORTD.IN & (1<<NRF_IRQ)) == 0) {
			//doRFWork(TagDispMode);
			//USART_tx_String_P(&USARTC0, PSTR("RF Work in RTC wake, Int Low\r\n");
			//_delay_ms(1);
		} else {
			//Flush RX buffer
			NRF24L01_Flush_RX_FIFO();
			//Clear RX interrupt flag
			NRF24L01_W_Reg(NREG_STATUS, 0x40);
			//USART_tx_String_P(&USARTC0, PSTR("RF Work in RTC wake, Int High\r\n"));
			_delay_ms(1);
		}
	}

	TagUpdate++;
	if (TagUpdate >= TAGLISTUPDATE) {
		TagUpdate = 0;
		UpdateTagList();
	}	

	if (FirstTimeGreaterOrEqualTo(&RTC_c_Time, &Send_Data_GPRS_Next)) {
		SendEmailChecks();
	}

	CheckBatVolt++;
	if (CheckBatVolt >= BATVOLTCHECK) {
		CheckBatVolt = 0;
		tmpBatV = CheckBatteryVoltage();
		if (tmpBatV < 10.5 && tmpBatV > 5.0) {
			//Low battery!
			EnterBatBakMode(BBAK_REASON_LO_BAT);
			Wakeup_Source = 0;
		}
	}

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
void SendEmailChecks() {
	//TODO[ ]: this whole section OK???
	if (Send_Data_GPRS_Retries_Remaining == SEND_DATA_GPRS_RETRIES_MAX) {
		//Full on, Start of a real send...
		//CopyTime(&RTC_c_Time, &Send_Data_GPRS_Next_Real);
		//AddTimes(&Send_Data_GPRS_Next_Real, &Send_Data_GPRS_Period);
		ReelInTheYears();
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
		TimeAfterTime(&RTC_c_Time, &Send_Data_GPRS_Next, 0, 0, 0, 0, 15, 0);
	} else { //Send_Data_GPRS_Retries_Remaining <= 1
		//If it is our last attempt, set the next time to the REAL next wakeup (ie 1 GPRS_send_period after the previous real wakeup time)
		//We have to do this at the start of the function (before the sendemail call) because if the unit resets or goes into batbackup mode
		//during it then we want the next time to be set-up already.
		CopyTime(&Send_Data_GPRS_Next_Real, &Send_Data_GPRS_Next);
	}
	//Decrement no. of attempts remaining (before the sendemail function)
	Send_Data_GPRS_Retries_Remaining--;
		
	sPrintDateTime(s, &RTC_c_Time, "Current Time");
	USART_tx_String(&USARTC0, s);
	sPrintDateTime(s, &Send_Data_GPRS_Next, "Next_GPRS");
	USART_tx_String(&USARTC0, s);
	sPrintDateTime(s, &Send_Data_GPRS_Next_Real, "Next_GPRS_Real");
	USART_tx_String(&USARTC0, s);
	USART_printf_P(&USARTC0, "Attempts: %u\r\n", SEND_DATA_GPRS_RETRIES_MAX-Send_Data_GPRS_Retries_Remaining);
		
	uint8_t send_success = 0;
	USART_tx_String_P(&USARTC0, PSTR("Turning GM862 On!\r\n"));
	/*uint8_t tmpByte = 0;
	USART_rx_Byte(&USARTC0, 10000, &tmpByte);
	if (tmpByte == '1') {
		send_success = 1;
	}*/
	//TODO[ ]: This section all looks good.. haven't tested the actual send email bit though
	if (GM862_Unit_On(1) >= 0) {
		USART_tx_String_P(&USARTC0, PSTR("Sending Email.\r\n"));
		if (GM862_SendEmail() == 0) {
			send_success = 1;
			USART_tx_String_P(&USARTC0, PSTR("Send Data Success!\r\n"));
		}
	}
	GM862_Unit_Off();
		
	//Now that we're done sending...
	if (send_success == 0) {
		if (Send_Data_GPRS_Retries_Remaining == 0) {
			//if it was our last shot and it still failed, save the fail event to flash
			SAVE_SYS_MSG(SYS_MSG_SEND_EMAIL_FAIL, SEND_DATA_GPRS_RETRIES_MAX);
			USART_tx_String_P(&USARTC0, PSTR("Send Data Failed!\r\n"));
			Send_Data_GPRS_Retries_Remaining = SEND_DATA_GPRS_RETRIES_MAX;
		}
	} else {
		//Success! Save the event to flash
		SAVE_SYS_MSG(SYS_MSG_SENT_EMAIL, SEND_DATA_GPRS_RETRIES_MAX - Send_Data_GPRS_Retries_Remaining);
		//TODO[ ]: Test this!
		CopyTime(&Send_Data_GPRS_Next_Real, &Send_Data_GPRS_Next);
		Send_Data_GPRS_Retries_Remaining = SEND_DATA_GPRS_RETRIES_MAX;
		GPRSSENDNEXTTOEEPROM();
		USART_tx_String_P(&USARTC0, PSTR("Send Data Succeeded!\r\n"));
	}
		
	//Debugging:
	sPrintDateTime(s, &Send_Data_GPRS_Next, "Next GPRS Send Time");
	USART_tx_String(&USARTC0, s);
	USART_tx_String_P(&USARTC0, PSTR("\r\n"));
	_delay_ms(1);	
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
void ReelInTheYears() {
	sPrintDateTime(s, &Send_Data_GPRS_Next_Real, "Next_GPRS_Real(RIY1)");
	USART_tx_String(&USARTC0, s);
	//Timed and tested: each iteration of this loop takes approx 18us to complete
	//(Did 1557952 iterations in about 27 seconds)
	while (FirstTimeGreaterOrEqualTo(&RTC_c_Time, &Send_Data_GPRS_Next_Real)) {
		//CopyTime(&RTC_c_Time, &Send_Data_GPRS_Next_Real);
		AddTimes(&Send_Data_GPRS_Next_Real, &Send_Data_GPRS_Period);
	} 
	sPrintDateTime(s, &Send_Data_GPRS_Next_Real, "Next_GPRS_Real(RIY2)");
	USART_tx_String(&USARTC0, s);
}

/*!	\brief Process received messages in the nordic RX buffer.
 *
 *	For each received message (tag), if the received ID passes the filter criteria,
 *	print out the desired level of information to the serial port (based on dispLvl)
 *	and save the record using the #saveRecord() function.
 *	
 *	\param dispLvl 
 *	- RF_DISP_TAGID: Print the tag ID only.
 *	- RF_DISP_TAGIDTIME: Print tag ID and pickup time.
 *	- RF_DISP_TAGIDTIMEADD: Print tag ID, pickup time and flash save address (this doesn't really make sense any more, with the taglist)
 *
 *	\sa saveRecord(), FilterStr2Mask() and TagDispMode
 */
void doRFWork(uint8_t dispLvl) {
	RTC_Time_t tmpTime;
	uint32_t rxData;
	uint8_t passFilter = 0;
	uint8_t i = 0;
	char ss[40];

	do {
		CopyTime(&RTC_c_Time, &tmpTime);
		NRF24L01_R_RX_PAYLOAD(&rxData);

		if ((rxData & FilterMask) == FilterCode)
			passFilter = 1;
		else
			passFilter = 0;

		if (LoggingEnabled == 2 || (LoggingEnabled == 3 && passFilter))
			saveRecord(&tmpTime, rxData);

		if (passFilter) {
			if (dispLvl >= RF_DISP_TAGID) {
				if (i == 0) _delay_ms(5);
				sprintf_P(s, PSTR("RX ID: 0x%08lX"), rxData);
				USART_tx_String(&USARTC0, s);

				if (dispLvl >= RF_DISP_TAGIDTIME) {
					sPrintDateTime(ss, &tmpTime, "");
					sprintf_P(s,PSTR("\r\nTime: %s"), ss);
					USART_tx_String(&USARTC0, s);

					if (dispLvl >= RF_DISP_TAGIDTIMEADD) {
						//(LoggingEnabled>GUI_LOGSTATEDIT_DISABLED) ? ....
						sprintf_P(s, PSTR("\r\nAddress: %lu"), (LoggingEnabled>1) ? ((RecordCount)) : ((uint32_t)0));
						USART_tx_String(&USARTC0, s);
					}
					//if there is more than one piece of information (eg. ID + time) then
					//add an extra new line after to separate out records.
					USART_tx_String(&USARTC0, "\r\n");
				}
				USART_tx_String(&USARTC0, "\r\n");
			}
			i++;
		}
		//While RX_EMPTY bit in FIFO_STATUS register is low - while the RX FIFO is not empty
	} while (!(NRF24L01_R_Reg(NREG_FIFOSTATUS)&(1<<0)));

	//NRF24L01_Flush_RX_FIFO();
	NRF24L01_W_Reg(NREG_STATUS, 0x40);

	//TODO[x]: Test below line!!
	if (dispLvl > 0 && USART_USB_IN) _delay_ms(2);

}

/*!	
 *	If a tag in #MList_Data has not been picked up in the last #TAGLISTUPDATE seconds, save a
 *	stop record to the flash for that tag at the last time it was picked up and remove it
 *	from the list.  Does this for each tag in #MList_Data.
 */
void UpdateTagList() {

	int i;
	RTC_Time_t t1,t2;

	ClearTime(&t2);
	t2.Second = TAGLISTUPDATE;

	//(1)USART_printf_P(&USARTC0, "MList_Count: %d\r\n", MList_Count);

	for (i = 0; i < MList_Count; i++) {
		//USART_printf_P(&USARTC0, "MList_Item: %d ID = %08lX\r\n", i, MList_Data[i].ID);
		CopyTime(&(MList_Data[i].LastTime), &t1);
		AddTimes(&t1, &t2);

		if (FirstTimeGreaterOrEqualTo(&RTC_c_Time, &t1)) {
			//(1)USART_printf_P(&USARTC0, "Erase: %d ID = %08lX\r\n", i, MList_Data[i].ID);
			//if the current time is after the last wakeup plus TAGLISTUPDATE seconds...
			//Remove from list and add a finish record
			RecordToFlash(&(MList_Data[i].LastTime), MList_Data[i].ID, 0x01);
			mList_RemoveItemWithID(MList_Data[i].ID);
			i--;
			//(1)USART_printf_P(&USARTC0, "MList_Count: %d\r\n", MList_Count);
		}

	}

}

/*!	\brief Saves a tag pickup.
 *
 *	Doesn't necessarily save that event to flash.  Only the first and last events are
 *	written to flash.  If the tag is not already in the list #MList_Data then it will be 
 *	saved to flash with a start condition and added to the list.  If the tag is already in
 *	the list then update the time of the list entry to the new time but do not save it
 *	to the serial flash.
 *
 *	\param mTime The time of the tag pickup event
 *	\param ID The ID of the tag that was picked up
 *	\return 0 for success, 1 for full buffer, 2 for Flash write failure
 */
uint8_t saveRecord(RTC_Time_t* mTime, uint32_t ID) {
	ListStruct_t* ListItem;

	//if in current list of tags...
	if (mList_ItemWithID(ID, &ListItem) == 0) {
		//Update the latest time
		CopyTime(mTime, &(ListItem->LastTime));
	} else {
		//if not in the current list of tags...
		if (RecordToFlash(mTime, ID, 0x03) == 0) {
			return 2;
		}
		uint32_t Addy = RecordCount - 1;
		if (mList_AddItem(ID, Addy, mTime) != 0) {
			//we're fucked!! - out of space in the list.
			//well, not really, it will just log every event rather than just start and stop now
			//because it won't actually get added to the list.
			//(1)USART_tx_String_P(&USARTC0, PSTR("LIST FULL!!! we're boned!\r\n"));
			return 1;
		}
	}
	return 0;
}

/*!	\brief Reads a record from the serial flash.
 *
 *	The record at record number RecordNo is read from the serial flash and returned through the two pointers
 *	mTime and ID.
 *
 *	\param [in] RecordNo
 *	\param [out] mTime
 *	\param [out] ID
 *	\return 0xFF if the function failed, the entries flag value otherwise
 */
uint8_t loadRecord(uint32_t RecordNo, RTC_Time_t* mTime, uint32_t* ID) {
	uint8_t tZip[5];

	if (RecordNo*RECORD_SIZE + 10 < SerFlash_MAXADDRESS) {
		ClearTime(mTime);
		SerFlash_ReadBytes(RecordNo*RECORD_SIZE, 5, tZip);
		SerFlash_ReadBytes(RecordNo*RECORD_SIZE+5, 4, (uint8_t*)ID);

		return UnzipTime(mTime, tZip);
	} else {
		return 0xFF;
	}
}

/*!	\brief Saves the passed information to flash.
 * 
 *	The information is saved to the next free entry in the data flash which is calculated from
 *	the value of RecordCount.
 *	
 *	\param mTime the time that the event occurred
 *	\param ID The ID of the tag
 *	\param Flags 2-bit flag value, usually represents whether this is to be a start, stop or system message entry
 *	\return 1 if success, 0 if failure
*/
uint8_t RecordToFlash(RTC_Time_t* mTime, uint32_t ID, uint8_t Flags) {
	uint8_t tZip[5];

	if (RecordCount*RECORD_SIZE + 9 <= SerFlash_MAXADDRESS) {
		//RTC_Time_t tmpTime1, tmpTimeAdd;

		Flags &= 0b00000011;
		ZipTime(mTime, tZip, Flags);
		SerFlash_WriteBytes((uint32_t)(RecordCount*RECORD_SIZE), 5, tZip);
		//SerFlash_WriteBytes((uint32_t)(RecordCount*RECORD_SIZE+5), 4, (uint8_t*)ID);
		SerFlash_WriteBytes((uint32_t)(RecordCount*RECORD_SIZE+5), 4, (uint8_t*)(&ID));
		//USART_printf_P(&USARTC0, "RC: %lu\r\n", RecordCount);
		RecordCount++;
		//USART_printf_P(&USARTC0, "RC: %lu\r\n", RecordCount);

		/*//Save recordCount to eeprom once every 100 writes or once per day.
		//TODO[ ]: make sure this works!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!1
		ClearTime(&tmpTimeAdd);
		tmpTimeAdd.Day = 1;
		CopyTime(&RecCount_LastSave, &tmpTime1);
		AddTimes(&tmpTime1, &tmpTimeAdd);
		if (FirstTimeGreater(&RTC_c_Time, &tmpTime1) || ((RecordCount % 100) == 0)) {
			RCTOEEPROM();
			CopyTime(&RTC_c_Time, &RecCount_LastSave);
		}*/

		//TODO[ ]: Don't write RecordCount to EEPROM EVERY time!! maybe once per 100?
		return 1;
	} else
		return 0;
}

/*!	\brief Start the 10 millisecond timer on TC0.
 *
 *	The timer counts down from Time to 0 and then stops.
 *	Each tick is 10 milliseconds long.
 *	
 *	\param Time The number to start counting down from
 */
void StartTim10ms(uint16_t Time) {
	cli();
	timer10ms = Time;
	//Setup Timer 0 for 10ms Timeout
	TC_SetPeriod(&TCC0, 0x3A97);
	TCC0.CNT = 0;
	TC0_ConfigClockSource(&TCC0, TC_CLKSEL_DIV8_gc);
	TCC0.INTCTRLA = TC_OVFINTLVL_MED_gc;
	sei();
}

/*!	\brief Stop the 10 millisecond timer on TC0
 *
 *	Stops the 10 millisecond timer on TC0 from running and sets
 *	the timer10ms variable to 0.
 */
void StopTim10ms() {
	//Stop the timer
	TC0_ConfigClockSource(&TCC0, TC_CLKSEL_OFF_gc);
	TCC0.INTCTRLA = TC_OVFINTLVL_OFF_gc;
	//Clear the timer
	timer10ms = 0;
}

/*!	\brief Save the program counter.
 *
 *	The the return address of this function (ie. the address of the program
 *	counter after the function returns) is saved into the PC_saved array.
 */
void save_PC(void) {
	//I worry that changing any small build variables will cause
	//this function not to work!!

	uint8_t* RetADD;
	RetADD = (uint8_t*)((SPH<<8)+SPL);
	//Save Program Counter Address
	PC_saved[0] = *(RetADD+0);
	PC_saved[1] = *(RetADD+1);
	PC_saved[2] = *(RetADD+2);
}

/*!	\brief Reads a calibration byte from the MCU at address of index.
 *
 *	Returns a calibration byte, read from the MCU calibration bytes at
 *	the address of the parameter index
 *
 *	\param index Index of the calibration byte to read
 *	\return The value of the read calibration byte
 */
uint8_t ReadCalibrationByte( uint8_t index )
{
	uint8_t result;

	/* Load the NVM Command register to read the calibration row. */
	NVM_CMD = NVM_CMD_READ_CALIB_ROW_gc;
	result = pgm_read_byte(index);

	/* Clean up NVM Command register. */
	NVM_CMD = NVM_CMD_NO_OPERATION_gc;

	return( result );
}


/*!	\brief Sets the active filter to the filter String passed
 *
 *	Takes an 8 character string as an input and produces a filter
 *	code and a filter mask which are used to choose which received
 *	tags are saved to flash.  The active filter for the logger is
 *	set to this.
 *
 *	\param filtStr - Filter String input
 */
void FilterStr2Mask(char* filtStr) {
	uint8_t i;
	char tmpfiltStr[8];
	FilterMask = 0L;
	for (i = 0; i < 8; i++) {
		if (filtStr[i] == '*') {
			tmpfiltStr[i] = '0';
			//FilterMask += mPow(0x0F,(7-i));
			//ByteNO(&FilterMask,(4-(uint8_t)(i/2))) |= 0x0F<<(i%2);
		} else {
			tmpfiltStr[i] = filtStr[i];
			FilterMask |= ((uint32_t)(0x0F))<<((7-i)*4);
		}
	}
	FilterCode = strtoul(tmpfiltStr, (char **)NULL, 16);
	//USART_printf_P(&USARTC0, "Str:%08lX, Msk:%08lX\r\n", FilterCode, FilterMask);
}

/*!	\brief Count the number of stored records in the flash memory
 *
 *	Search through the external flash memory and count how many records
 *	are stored there in total.  It does this by counting until it finds
 *	an empty record.
 *
 *	\return Record count on serial flash
 */
uint32_t FindRecordCount() {
	uint32_t cnt = 0;
	uint8_t i;
	uint8_t FFCount;
	uint8_t tmpData[9];
	
	//TODO[ ]: Can improve the efficiency of this function
	for (cnt = 0; cnt*RECORD_SIZE <= SerFlash_MAXADDRESS-9; cnt++) {
		SerFlash_ReadBytes(cnt*RECORD_SIZE, 9, tmpData);
		
		FFCount = 0;
		for (i = 0; i < 9; i++) {
			if (tmpData[i] == 0xFF) {
				FFCount++;
			}
		}			
		if 	(FFCount == 9) {
			//How it works:
			//RC		1 2 3 4 5
			//hasData	1 1 1 1 1 0
			//cnt		0 1 2 3 4 5
			//return val = 5
			return cnt;
		}
	}
	
	return (SerFlash_MAXADDRESS/RECORD_SIZE);
	
}

/*!	\brief Sets the system's clock speed
 *
 *	Sets the system clock based on the parameter 'speed'.\n
 *	speed: 
 *		- CLK_SPD_32MHz: Enable and select 32MHz internal RC oscillator.
 *		- CLK_SPD_12MHz_SETUP: Enable and select 12MHz clock source from 2MHz internal RC oscillator and PLL
 *		- CLK_SPD_12MHz_SWITCH: Switch to already configured 12MHz clock source.  
 *
 *	\param speed Speed to set the clock to.
 */
void SetClkSpeed(uint8_t speed) {

	if (speed == CLK_SPD_32MHz) {
	  	//32MHz - 32MHz Internal RC Oscillator
		CLKSYS_Enable(OSC_RC32MEN_bm);
		while (CLKSYS_IsReady(OSC_RC32MEN_bm) == 0);
		CLKSYS_Prescalers_Config(CLK_PSADIV_1_gc, CLK_PSBCDIV_1_1_gc);
		CLKSYS_Main_ClockSource_Select(CLK_SCLKSEL_RC32M_gc);
	} else {
		//disable 32MHz oscillator
		CLKSYS_Disable(OSC_RC32MEN_bm);
		if (speed == CLK_SPD_12MHz_SETUP) {
			//12MHz - 2MHz Internal RC Oscillator and PLL
			CLKSYS_Enable(OSC_PLLSRC_RC2M_gc);
			while (CLKSYS_IsReady(OSC_RC2MRDY_bm) == 0);
			CLKSYS_PLL_Config(OSC_PLLSRC_RC2M_gc,6);
			CLKSYS_Enable(OSC_PLLEN_bm);
			while (CLKSYS_IsReady(OSC_PLLRDY_bm) == 0);
		}
		//if (speed == CLK_SPD_12MHz_SWITCH) {
		//Switch to 12MHz clock
		CLKSYS_Prescalers_Config(CLK_PSADIV_1_gc, CLK_PSBCDIV_1_1_gc);
		CLKSYS_Main_ClockSource_Select(CLK_SCLKSEL_PLL_gc);
		//}
	}
}

/* Old Code:
	//Setup Clock
  	//32MHz - 32MHz Internal RC Oscillator
	//CLKSYS_Enable(OSC_RC32MEN_bm);
	//while (CLKSYS_IsReady(OSC_RC32MEN_bm) == 0);
	//CLKSYS_Prescalers_Config(CLK_PSADIV_1_gc, CLK_PSBCDIV_1_1_gc);
	//CLKSYS_Main_ClockSource_Select(CLK_SCLKSEL_RC32M_gc);

	//Print stack pointer or saved stack pointer
	USART_printf_P(&USARTC0, "SPII: 0x%04X\r\n",(SPH<<8)+SPL);
	USART_printf_P(&USARTC0, "SPII: 0x%04X\r\n",SP_saved);

	//USART_printf_P(&USARTC0, "RetAdd: 0x%02X%02X%02X\r\n",*(RetADD+0),*(RetADD+1), *(RetADD+2));
	//USART_printf_P(&USARTC0, "SPII: 0x%02X%02X BB:%d\r\n",SPH,SPL, batBakMode);
 *
 */

/*
 *
	USART_printf_P(&USARTC0, "ADCOffset: %d\r\n", ADCOffset);
	_delay_ms(500);

	USART_printf_P(&USARTC0, "ADCCAL: LOW:0x%02X HIGH:0x%02X\r\n", ADCA.CALL, ADCA.CALH);

	while (1) {
		//CalibrateADCOffset();
		//_delay_ms(2);
		//USART_printf_P(&USARTC0, "ADCOffset: 0x%04X\r\n", ADCOffset);
		CheckBatteryVoltage();
		_delay_ms(500);
	}
 *
 */

/*
 *
	//PORTCFG with MPCMASK Test
	//This shows that when using the MPCMASK, the pin that is actually written to
	//(Pin0 in this case) is only modified IF IT IS ENABLED IN THE MPC MASK!!
	PORTA.PIN0CTRL = 0;
	PORTCFG.MPCMASK = (1<<2);
	PORTA.PIN0CTRL = PORT_OPC_PULLDOWN_gc;
	//The results of these lines are "Pin0: 0x00", "Pin2: 0x02"
	LCD_printf_Pos_P(2,0,"Pin0: 0x%02X", PORTA.PIN0CTRL);
	LCD_printf_Pos_P(3,0,"Pin2: 0x%02X", PORTA.PIN2CTRL);
 *
 */

/*//Serial Flash Test
	//char n[512];
	//int i;
	//for (i = 0; i < 512; i++) {
	//	n[i] = 0x57;
	//}
	//USART_printf_P(&USARTC0, "SFLStat: 0x%02X\r\n", n[0] = SerFlash_RdStatusReg());
	//do {
	//	SerFlash_SetProt(0x00);
	//	USART_printf_P(&USARTC0, "SFLStat: 0x%02X\r\n", n[0] = SerFlash_RdStatusReg());
	//} while ((n[0] & SerFlash_PROT_MASK) != 0x00);
	//SerFlash_WriteBytes(0L,512,(uint8_t*)n);
	//for (i = 0; i < 512; i++) {
	//	n[i] = 0x00;
	//}
	//SerFlash_ReadBytes(0L, 512, (uint8_t*)n);
	//for (i = 0; i < 512; i++) {
	//	USART_printf_P(&USARTC0, "%02x ", n[i]);
	//}

	//TESTING - Output Clock on pin...
	//Output clock on PC7 (clock is CLK1X = CLKPER = CLKCPU)
	//PORTCFG.CLKEVOUT = 0x01;//PORTCFG_CLKOUT_PC7_gc;
	//PORTC.DIRSET = (1<<7);
*/

/*void GM862Test () {
	//USART Setup for 19200bps 115200bps
	USART_Setup(&PORTC, &USARTC0, 176, -5);//1218, -5);//
	USART_tx_String_P(&USARTC0, PSTR("Serial Comms Initialised \r\n"));

	//USART Setup for 19200bps 115200bps
	USART_Setup(&PORTE, &USARTE0, 176, -5);//1218, -5);//
	//PORTCFG.MPCMASK = (1<<2)|(1<<3);
	//PORTE.PIN2CTRL = PORT_OPC_PULLUP_gc;//|PORT_INVEN_bm;
	//PORTE.PIN3CTRL = PORT_OPC_PULLUP_gc;//|PORT_INVEN_bm;

	PORTC.DIRSET = (1<<6)|(1<<7);
	PORTC.OUTCLR = (1<<7);
	//Supply power
	PORTC.OUTSET = (1<<6);
	_delay_ms(2000);
	PORTC.OUTCLR = (1<<6);

	MCU_STAT_OFF();
	_delay_ms(10000);

	MCU_STAT_ON();
	_delay_ms(500);
	MCU_STAT_OFF();

	//Power On
	PORTC.OUTSET = (1<<7);
	_delay_ms(1000);
	PORTC.OUTCLR = (1<<7);

	_delay_ms(10000);

	MCU_STAT_ON();

	//USART_tx_String_P(&USARTE0, PSTR("AT\r"));
	//USART_tx_Byte(&USARTC0, (char)USART_rx_Byte(&USARTE0, 0));

	//USART_rx_String(&USARTE0, s, 0);
	//USART_tx_String(&USARTC0, s);

	int aa;
	while (1) {
		MCU_STAT_OFF();
		if ((aa=USART_rx_Byte_nb(&USARTC0)) >= 0) {
			MCU_STAT_ON();
			if ((char)aa == 0x02) {
				PORTC.OUTCLR = (1<<7);
				_delay_ms(2000);
				PORTC.OUTSET = (1<<7);
			}
			USART_tx_Byte(&USARTE0,(char)aa);
			//USART_rx_String(&USARTC0, s, 10000);
			//USART_tx_String(&USARTE0, s);
		}
		if ((aa=USART_rx_Byte_nb(&USARTE0)) > 0) {
			MCU_STAT_ON();
			USART_tx_Byte(&USARTC0,(char)aa);
			//USART_tx_String_P(&USARTC0, PSTR("RX!!! \r\n"));
			//USART_rx_String(&USARTE0, s, 10);
			//USART_tx_String(&USARTC0, s);
		}
	}
}*/

/*uint8_t ShowLog(uint8_t dispLvl, uint32_t* rxID, RTC_Time_t* dispTime, uint32_t* FlAdd) {
	char ss[40];
	if (dispLvl >= RF_DISP_TAGID) {

		sprintf_P(s, PSTR("RX ID: 0x%08lX"), rxID);
		USART_tx_String(&USARTC0, s);
		if (TermMode) LCD_Print_Terminal(s);

		if (dispLvl >= RF_DISP_TAGIDTIME) {
			sPrintDateTime(ss, dispTime, "");
			sprintf_P(s,PSTR("\r\nTime: %s"), ss);
			USART_tx_String(&USARTC0, s);
			if (TermMode) LCD_Print_Terminal(s+2);

			if (dispLvl >= RF_DISP_TAGIDTIMEADD) {
				//(LoggingEnabled>GUI_LOGSTATEDIT_DISABLED) ? ....
				sprintf_P(s, PSTR("\r\nAddress: %lu"), (LoggingEnabled>1) ? ((uint32_t)(RecordCount-1)*RECORD_SIZE) : ((uint32_t)0));
				USART_tx_String(&USARTC0, s);
				if (TermMode) LCD_Print_Terminal(s+2);
			}
			//if there is more than one bit of information (eg. ID + time) then
			//add an extra new line after to separate out records.
			USART_tx_String(&USARTC0, "\r\n");
		}
		USART_tx_String(&USARTC0, "\r\n");
	}
	return 0;
}*/


		
// 	USART_printf_P(&USARTC0, "%u\t= %04X\r\n", 0, pgm_read_byte(&(Ser_CMDAry[0].CMDByte)));
// 	for (uint8_t i = 0; i < 10; i++) {
// 		USART_printf_P(&USARTC0, "%u\t= %04X\r\n", i, pgm_read_word(&Ser_CMDAry[i].Handler));
// 		USART_printf_P(&USARTC0, "\t= %04X\r\n", i, pgm_read_byte(&(Ser_CMDAry[i].CMDByte)));
// 	}	

// 	mList_Init();
// 	uint8_t i;
// 	uint32_t tmpID = 0xABCD1234;
// 	uint32_t tmpADD = 0x00005555;
// 	ListStruct_t* tmpStruct;
// 	mList_AddItem(&tmpID, &tmpADD, &RTC_c_Time);
// 	
// 	USART_printf_P(&USARTC0, "ID[0]: 0x%08X\r\n", MList_Data[0].ID);
// 	USART_printf_P(&USARTC0, "Addy[0]: 0x%08X\r\n", MList_Data[0].Addy);
// 	tmpID++;
// 	tmpADD++;
// 	mList_AddItem(&tmpID, &tmpADD, &RTC_c_Time);
// 	USART_printf_P(&USARTC0, "ID[1]: 0x%08X\r\n", MList_Data[1].ID);
// 	USART_printf_P(&USARTC0, "Addy[1]: 0x%08X\r\n", MList_Data[1].Addy);
// 	tmpID++;
// 	tmpADD++;
// 	mList_AddItem(&tmpID, &tmpADD, &RTC_c_Time);
// 	USART_printf_P(&USARTC0, "ID[2]: 0x%08X\r\n", MList_Data[2].ID);
// 	USART_printf_P(&USARTC0, "Addy[2]: 0x%08X\r\n", MList_Data[2].Addy);
// 	_delay_ms(100);
// 	mList_RemoveItemWithID(0xABCD1235);//second item
// 	
// 	for (i = 0; i < 5; i++) {
// 		USART_printf_P(&USARTC0, "ID[%u]: 0x%08X\r\n", i, MList_Data[i].ID);
// 		USART_printf_P(&USARTC0, "Addy[%u]: 0x%08X\r\n", i, MList_Data[i].Addy);
// 	}	
// 	mList_ItemWithID(0xABCD1234, &tmpStruct);
// 	
// 	//tmpStruct->ID = 0xABCD1233;
// 	USART_printf_P(&USARTC0, "ID[found]: 0x%08X\r\n", tmpStruct->ID);
// 	USART_printf_P(&USARTC0, "Addy[found]: 0x%08X\r\n", tmpStruct->Addy);
// 	_delay_ms(100);


//void ShowMenu() {
// 	RTC_Time_t tmpTime;
// 	unsigned int i;
// 	int j;
// 	uint32_t ii;
// 	char retChar;
// 	uint32_t rxData;
// 
// 	_delay_ms(1);
// 	USART_tx_String_P(&USARTC0, PSTR("USART Woke me.\r\n"));
// 	USART_tx_String_P(&USARTC0, PSTR("Enter command.\r\n>"));
// 
// 	//flush USART RX buffer
// 	while (USART_rx_Byte_nb(&USARTC0) > 0);
// 	//Receive byte
// 	i = USART_rx_Byte(&USARTC0,10000);
// 	if (i > 0) {
// 		retChar = (char)i;
// 		switch (retChar) {
// 		case 'v':
// 			//Print Version
// 			USART_tx_String_P(&USARTC0, PSTR_PRODUCT_N);
// 			USART_tx_Byte(&USARTC0, ' ');
// 			USART_tx_String_P(&USARTC0, PSTR_CODE_V);
// 			USART_tx_String_P(&USARTC0, PSTR_NEWLINE);
// 			USART_tx_String_P(&USARTC0, PSTR_WILDSPY);
// 			USART_tx_String_P(&USARTC0, PSTR_NEWLINE);
// 			break;
// 		case 'T':
// 			//Set Date/Time
// 			USART_tx_String_P(&USARTC0, PSTR("Enter Date/Time (DDMMYYHHMMSS).\r\n>"));
// 
// 			for (i = 0; i < 12; i++) {
// 				j = USART_rx_Byte(&USARTC0, 0);
// 				if (j > 0)
// 					s[i] = j;
// 				else
// 					break;
// 			}
// 			s[12] = '\0';
// 
// 			if (j > 0) {
// 				if (str2Time(s, &tmpTime) == 0)
// 					CopyTime(&tmpTime, &RTC_c_Time);
// 
// 				sPrintDateTime(s, &RTC_c_Time, "Time");
// 				USART_tx_String(&USARTC0, s);
// 			}
// 			break;
// 		case 'e':
// 			//Set RecordCount
// 			i = USART_RX_Int(&USARTC0, 5, USART_FF_SHOWFORMAT|USART_FF_ECHOCHARS, 10000);
// 			RecordCount = i;
// 			RCTOEEPROM();
// 			break;
// 		case 'r':
// 			//Read Entry at Address
// 			i = USART_RX_Int(&USARTC0, 4, USART_FF_SHOWFORMAT|USART_FF_ECHOCHARS, 10000);
// 			if (i > 0) {
// 				//Testing.. read and print it..
// 				loadRecord(i, &tmpTime, &rxData);
// 				USART_printf_P(&USARTC0, "Address: %lu\r\n", ((uint32_t)i)*RECORD_SIZE);
// 				sPrintDateTime(s, &tmpTime, "Read Time");
// 				USART_tx_String(&USARTC0, s);
// 				USART_printf_P(&USARTC0, "RX Data:0x%08lX\r\n", rxData);
// 			}
// 			break;
// 		case '.':
// 			//Save a dummy record
// 			//read an value (XXXX)
// 			rxData = 0x12345678L;//USART_RX_Int(&USARTC0, 4, USART_FF_SHOWFORMAT|USART_FF_ECHOCHARS);
// 			//Testing.. read and print it..
// 			if (rxData > 0)
// 				saveRecord(&RTC_c_Time, &rxData);
// 			break;
// 		case 't':
// 			//Enable Time Printing Every Second
// 			PrintTime ^= 1;
// 			break;
// 		case 'c':
// 			//Print Record Count
// 			USART_printf_P(&USARTC0, "Record Count: %lu\r\n", RecordCount);
// 			break;
// 		case 'N':
// 			//Logger Name:
// 			USART_tx_String_P(&USARTC0, PSTR("Logger Name: "));
// 			if ((j = USART_rx_String(&USARTC0, s, 20000)) > 0) {
// 				s[20] = '\0';
// 				s[j-1] = '\0';
// 				if (s[j-2] == '\r') s[j-2] = '\0';
// 				stringCopy(s, LoggerName);
// 				LOGGERNAMETOEEPROM();
// 				USART_tx_String(&USARTC0, LoggerName);
// 				USART_tx_String_P(&USARTC0, PSTR("\r\n"));
// 			}				
// 			break;
// 		/*case 'R':
// 			//RX Records one at a time, no CRC
// 			for (ii = 0; ii < RecordCount; ii++) {
// 				if (USART_USB_IN) {
// 					loadRecord(ii, &tmpTime, &rxData);
// 					for (uint8_t j = 0; j < 6; j++)
// 						USART_tx_Byte(&USARTC0, *(((uint8_t*)&tmpTime)+j));
// 					for (uint8_t j = 0; j < 4; j++)
// 						USART_tx_Byte(&USARTC0, *(((uint8_t*)&rxData)+j));
// 					j = USART_rx_Byte(&USARTC0, 0);
// 					if (retChar > 0) {
// 						switch (retChar) {
// 						case 'g': //good
// 							break;
// 						case 'r': //resend
// 							ii--;
// 							break;
// 						case 'c': //cancel
// 							ii = RecordCount;
// 							break;
// 						}
// 					}
// 				} else
// 					break;
// 			}
// 			break;*/
// 		case 'A':
// 			//RX Records, Faster, CRC checksum.
// 			//Fast clock!
// 			SetClkSpeed(CLK_SPD_32MHz);
// 			//Reconfigure UART (still 115200baud)
// 			//USART_Setup(&PORTC, &USARTC0, 524, -5, 0);
// 			//Reconfigure UART (460800baud)
// 			//USART_Setup(&PORTC, &USARTC0, 107, -5, 0);
// 			//Reconfigure UART (921600baud)
// 			USART_Setup(&PORTC, &USARTC0, 75, -6, 0);
// 			//Reconfigure UART (2000000baud) (2.0Mbaud)
// 			//USART_Setup(&PORTC, &USARTC0, 2, -1, 1);
// 			//wait for 'READY'
// 			if (((uint8_t)USART_rx_Byte(&USARTC0, 0) == 'g')) {
// 				;int recPerGrp = 10000;
// 				uint8_t k = 0;
// 				uint16_t CheckSUM = 0xFFFF;
// 				for (k = 0; k < 4; k++)
// 					USART_tx_Byte(&USARTC0, *(((uint8_t*)&RecordCount)+k));
// 				for (ii = 0; ii < RecordCount; ) {
// 					CheckSUM = 0xFFFF;
// 					if (ii+recPerGrp > RecordCount)
// 						recPerGrp = RecordCount-ii;
// 					USART_tx_Byte(&USARTC0, *(((uint8_t*)&recPerGrp)+0));
// 					USART_tx_Byte(&USARTC0, *(((uint8_t*)&recPerGrp)+1));
// 					for (j = 0; j < recPerGrp; j++) {
// 						if (USART_USB_IN) {
// 							loadRecord(ii+j, &tmpTime, &rxData);
// 							//send data and update CRC
// 							for (k = 0; k < 6; k++) {
// 								USART_tx_Byte(&USARTC0, *(((uint8_t*)&tmpTime)+k));
// 								CheckSUM = _crc16_update(CheckSUM, *(((uint8_t*)&tmpTime)+k));
// 							}
// 							for (k = 0; k < 4; k++) {
// 								//Corrupt some data! (for testing.. it worked!! perfectly!)
// 								//kkkk = *(((uint8_t*)&tmpTime)+k);
// 								//if ((j == 76) && (recPerGrp == 10000 || recPerGrp == 5000)) {
// 								//kkkk += 0x2C;
// 								//USART_tx_Byte(&USARTC0, *(((uint8_t*)&rxData)+k)+0x2C);
// 								//} else {
// 								USART_tx_Byte(&USARTC0, *(((uint8_t*)&rxData)+k));
// 								//}
// 
// 								CheckSUM = _crc16_update(CheckSUM, *(((uint8_t*)&rxData)+k));
// 							}
// 						} else
// 							//If USB is not plugged in, abort!
// 							ii = RecordCount;
// 					}
// 					//increment ii!
// 					ii+=recPerGrp;
// 					for (k = 0; k < 2; k++)
// 						USART_tx_Byte(&USARTC0, *(((uint8_t*)&CheckSUM)+k));
// 					j = USART_rx_Byte(&USARTC0, 0);
// 					if (j > 0) {
// 						switch ((uint8_t)j) {
// 						case 'g': //good
// 							break;
// 						case 'r': //resend
// 							ii-=recPerGrp;
// 							//send less data
// 							recPerGrp = recPerGrp/2;
// 							//only one packet per group? -> abort!
// 							if (recPerGrp < 2)
// 								//abort
// 								ii = RecordCount;
// 							break;
// 						case 'c': //cancel
// 							ii = RecordCount;
// 							break;
// 						}
// 					}
// 				}
// 			}
// 			//Regular Clock
// 			SetClkSpeed(CLK_SPD_12MHz_SWITCH);
// 			//Reconfigure UART
// 			USART_Setup(&PORTC, &USARTC0, 176, -5, 0);
// 			break;
// 		case 'E':
// 			//Erase DataFlash
// 			USART_tx_String_P(&USARTC0, PSTR("Sure you want to erase data flash? (y/n)\r\n"));
// 			retChar = USART_rx_Byte(&USARTC0, 0);
// 			if (retChar == 'y' || retChar == 'Y') {
// 				USART_tx_String_P(&USARTC0, PSTR("Erasing Data Flash.\r\n"));
// 				SerFlash_ChipErase(0x74);
// 				SerFlash_WaitRDY();
// 				USART_tx_String_P(&USARTC0, PSTR("DataFlash Erased!\r\n"));
// 				RecordCount = 0;
// 				RCTOEEPROM();
// 				records_sent_already = 0;
// 			}
// 			break;
// 		//case '+':
// 		//	//Add Entry
// 		//	rxData = 0x12345678;
// 		//	saveRecord(&RTC_c_Time, &rxData);
// 		//	break;
// 		case '?':
// 			//Get RF Values
// 			//RFCHAN
// 			ReadEEPROM(&myRF_Chan,1,RFCHANADD);
// 			sprintf_P(s,PSTR("RFCHAN:%d\r\n"), myRF_Chan);
// 			USART_tx_String(&USARTC0, s);
// 			//RFID
// 			ReadEEPROM(myRF_ID,5,RFIDADD);
// 			USART_tx_String_P(&USARTC0, PSTR("RFID:"));
// 			for (i = 0; i < 4; i++) {
// 				sprintf_P(s,PSTR("%02X-"), myRF_ID[i]);
// 				USART_tx_String(&USARTC0, s);
// 			}
// 			sprintf_P(s,PSTR("%02X\r\n"), myRF_ID[4]);
// 			USART_tx_String(&USARTC0, s);
// 			
// 			READFILTEREEPROM();
// 			FilterStr2Mask(Filter);
// 			USART_printf_P(&USARTC0, "RF Filter: %s\r\n", Filter);
// 			
// 			GPRSSENDPERFROMEEPROM();
// 			sPrintDateTime(s, &Send_Data_GPRS_Period, "SendData Period");
// 			USART_tx_String(&USARTC0, s);
// 			
// 			GPRSSENDNEXTFROMEEPROM();
// 			sPrintDateTime(s, &Send_Data_GPRS_Next, "Next SendData Time");
// 			USART_tx_String(&USARTC0, s);
// 			
// 			LOGGERNAMEFROMEEPROM();
// 			USART_printf_P(&USARTC0, "Logger Name: %s\r\n", LoggerName);
// 			
// 			break;
// 		case '*':
// 			//Set Values
// 			;uint8_t RFEdit = 0;
// 
// 			//RFCHAN
// 			USART_tx_String_P(&USARTC0, PSTR("RFCHAN "));// (###):\r\n"));
// 			if ((j = USART_RX_Int(&USARTC0, 3, USART_FF_SHOWFORMAT|USART_FF_ECHOCHARS, 10000)) > 0) {
// 				myRF_Chan = (uint8_t)j;
// 				WriteEEPROM(&myRF_Chan, 1, RFCHANADD);
// 				RFEdit = 1;
// 			}
// 			
// 			//RFID
// 			USART_tx_String_P(&USARTC0, PSTR("RFID (XX-XX-XX-XX-XX): "));
// 			for (i = 0; i < 5; i++) {
// 				if (USART_USB_IN) {
// 					if ((j = USART_RX_Int(&USARTC0, 2, USART_FF_ECHOCHARS|USART_FF_HEX, 10000)) > 0) {
// 						if (i < 4) USART_tx_Byte(&USARTC0, '-');
// 						tmpAry[i] = (uint8_t)j;//strtoul(s, (char **)NULL,16);
// 					} else
// 						i = 10;
// 				}
// 			}
// 			//copy tmp buffer into real address
// 			if (i < 10) {
// 				for (i = 0; i < 5; i++) {
// 					myRF_ID[i] = tmpAry[i];
// 				}
// 				WriteEEPROM(myRF_ID,5,RFIDADD);
// 				RFEdit = 1;
// 			}
// 			USART_tx_String_P(&USARTC0, PSTR("\r\n"));
// 			
// 			//Filter
// 			USART_tx_String_P(&USARTC0, PSTR("Filter (XXXXXXXX): "));
// 			if (USART_rx_String_F(&USARTC0, s, 20000, 8, USART_FF_ECHOCHARS|USART_FF_ENSABORT) == 0) {
// 				stringCopy(s,Filter);
// 				FilterStr2Mask(Filter);
// 				WRITEFILTEREEPROM();
// 			}
// 			USART_tx_String_P(&USARTC0, PSTR("\r\n"));
// 			
// 			RTC_Time_t tmpTime;
// 			//SendDataGPRSPeriod
// 			USART_tx_String_P(&USARTC0, PSTR("SendData Period "));
// 			if (USART_rx_Time(&USARTC0, &tmpTime , USART_FF_ECHOCHARS|USART_FF_SHOWFORMAT, 10000) >= 0) {
// 				CopyTime(&tmpTime, &Send_Data_GPRS_Period);
// 				GPRSSENDPERTOEEPROM();
// 			}
// 			
// 			//NextSendDataGPRS
// 			USART_tx_String_P(&USARTC0, PSTR("Next SendData Time "));
// 			if (USART_rx_Time(&USARTC0, &tmpTime , USART_FF_ECHOCHARS|USART_FF_SHOWFORMAT, 10000) >= 0) {
// 				CopyTime(&tmpTime, &Send_Data_GPRS_Next);
// 				GPRSSENDNEXTTOEEPROM();
// 			}
// 			
// 			if (RFEdit) {
// 				//Setup the SPI port
// 				NRF24L01_SPI_Setup();
// 				NRF24L01_SetupnRF24L01(myRF_Chan, myRF_ID);
// 				if (LOGGINGISENABLED && PWR_GOOD) NRF_RXMODE;
// 			}			
// 
// 			break;
// 		//case 'n':
// 		//	//Get Nordic Info
// 		//	USART_printf_P(&USARTC0, "Nordic STATUS: 0x%02X\r\n", NRF24L01_R_Reg(NREG_STATUS));
// 		//	USART_printf_P(&USARTC0, "Nordic CONFIG: 0x%02X\r\n", NRF24L01_R_Reg(NREG_CONFIG));
// 		//	USART_printf_P(&USARTC0, "Nordic ENRXADDR: 0x%02X\r\n", NRF24L01_R_Reg(NREG_EN_RXADDR));
// 		//	USART_printf_P(&USARTC0, "Nordic SETUP_AW: 0x%02X\r\n", NRF24L01_R_Reg(NREG_SETUP_AW));
// 		//	USART_printf_P(&USARTC0, "Nordic RF_CH: 0x%02X\r\n", NRF24L01_R_Reg(NREG_RF_CH));
// 		//	USART_printf_P(&USARTC0, "Nordic RF_SETUP: 0x%02X\r\n", NRF24L01_R_Reg(NREG_RF_SETUP));
// 		//	USART_printf_P(&USARTC0, "Nordic RX_ADDR_P0: 0x%02X\r\n", NRF24L01_R_Reg(NREG_RX_ADDR_P0));
// 		//	break;
// 		case 'f':
// 			//Set Verbose Mode
// 			USART_printf_P(&USARTC0, "Verbose mode: ", 0);
// 			if ((j = USART_rx_Byte(&USARTC0,0)) >= '0') {
// 				TagDispMode = (uint8_t)(j-'0');
// 				USART_printf_P(&USARTC0, "%d\r\n", TagDispMode);
// 			}
// 			break;
// 		case 'G':
// 			//GM862 on
// 			GM862_SetupHardware();
// 			USART_tx_String_P(&USARTC0, PSTR("Enabling GM862\r\n"));
// 			if (GM862_Unit_On(1) >= 0) {
// 			//if (1 == 1) {
// 				USART_tx_String_P(&USARTC0, PSTR("GM862 On!\r\n"));
// 			} else {
// 				USART_tx_String_P(&USARTC0, PSTR("Error Starting GM862!\r\n"));
// 			}
// 			//return;
// 			break;
// 		case '1':
// 			//GM862 send email
// 			GM862_SendEmail();
// 			break;
// 		case '2':
// 			//GM862 in bridge mode
// 			GM862_BridgeMode();
// 			break;
// 		case '3':
// 			//Update Time from GSM
// 			if (GM862_CalibratetoRTCTime() == 0) {
// 				USART_tx_String_P(&USARTC0, PSTR("UPDATED RTC TIME!\r\n"));
// 			} else {
// 				USART_tx_String_P(&USARTC0, PSTR("Failed to update RTC time.\r\n"));
// 			}				
// 			break;
// 		case '4':
// 			//GM862 OFF
// 			GM862_Unit_Off();
// 			USART_tx_String_P(&USARTC0, PSTR("GM862 Off.\r\n"));
// 			break;
// 		/*case '3':
// 			if (GM862_Unit_On(1) == 0) {
// 				GM862_SendEmail();
// 			}
// 			break;*/
// 		}
// 	}
// 
// 	if (bBak_Mode == 0) {
// 		USART_tx_String_P(&USARTC0, PSTR("Going to sleep.\r\n"));
// 
// 		while ((PORTD.IN & (1<<NRF_IRQ)) == 0)
// 			doRFWork(TagDispMode);
// 		Wakeup_Source &= ~(WAKEUP_SOURCE_NORDIC);
// 
// 		_delay_ms(2);
// 	}

//}