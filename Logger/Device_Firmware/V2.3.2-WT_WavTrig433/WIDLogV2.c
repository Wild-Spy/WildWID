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
 *`
 * $Revision: 0 $
 * $Date: 01/10/2011 $  \n
 *
 *****************************************************************************/

#include "WIDLogV2.h"

uint8_t deadByte;
char s[80];					
//uint8_t myRF_ID[5];						//!<ID on the nordic chip default = {0xE6,0xE6,0xE6,0xE6,0xE6};
//uint8_t myRF_Chan = 2;					//!<Channel on the nordic chip
//char setting_DeviceName[20] = "TestLogger";		//!<Used to ID the logger 19 characters long (20 with the the '\0')

//uint32_t RecordCount = 0L;			//!<No. of records in dataflash
uint8_t gotRecCnt = 0;					//!<Have we found the record count yet?
uint8_t TagUpdate = 0;					//!<Counter.. when it gets to 0, the tagupdate function is executed
uint8_t RF_Cal_Cntr = 0;				//!<Counter.. when it gets to 0, the RF segment is re-calibrated
uint8_t RF_CalClr_Cntr = 0;				//!<Counter.. when it gets to 0, the RF segment is re-calibrated
volatile uint8_t secCntr = 0;

uint8_t Startup = 1;					//!<About to startup?

uint8_t CheckBatVolt = 0;				//!<Counter for checking battery voltage (checks every 5? seconds)
float tmpBatV;							//!<The last read battery voltage.

uint8_t LLUT_DecrementIsOkTimersCntr = 0;
uint16_t LLUT_TxStatusCntr = TXSTATUSUPDATE;

RTC_Time_t RTC_c_Time;					//!<RTC time -> real time stored here.
RTC_Time_t RTC_Period;					//!<Time that elapses per RTC period (1 second)
uint32_t RTC_sec_ctr = 0;

//RTC_Time_t Send_Data_GPRS_Next;		//!<Next time to attempt to send an email
extern RTC_Time_t Send_Data_GPRS_Next_Real;	//!<Next scheduled time to send an email
//RTC_Time_t Send_Data_GPRS_Period;		//!<An email is scheduled to be sent once every period
extern uint8_t Send_Data_GPRS_Retries_Remaining;	//!<Number of retries remaining before aborting this email and waiting for next scheduled send email time


//RTC_Time_t RecCount_LastSave;			//!<Time when RecordCount was last saved to EEPROM

uint8_t TagDispMode = 1;				//!<How tags are displayed (None (1), show ID only(2), ID and date/time or ID(3), Date/Time and Address(4))

uint8_t bBak_Mode = 0;					//!<Are we in battery backup mode?
uint8_t bBak_Reason = BBAK_REASON_NONE;	//!<Why are we in battery backup mode?
uint8_t PC_saved[3];					//!<Saved address of Program Counter (pointer to address just before program goes to sleep) used to unsafely abort all operations and put the device to sleep immediately!
uint16_t SP_saved;						//!<Saved stack pointer address, used in conjunction with PC_saved variable above.
volatile uint8_t batBakTrig = 2;		//!<Variable used in managing battery backup modes

int ADCOffset = 0;						//!<Offset of the ADC (see datasheet on ADC offset)
//uint8_t PowerDown = 0;				//!<Flag used to tell the unit to power down when appropriate
uint8_t setting_LoggingEnabled;// = 1;	//!<Is logging enabled? Can still use as tracking device but won't save to flash (initially set to 1 (GUI_LOGSTATEDIT_DISABLED)
uint8_t EnterMobCommsBridgeMode = 0;	//!<Set in the SerComms library, tells the logger to enter GM862 or Iridium Bridge mode which can be exited by sending 0x03 from the computer
uint8_t EnterMobCommsConfigMode = 0;

char setting_Filter[8] = {'*','*','*','*','*','*','*','*'};	//!<Filter String
uint32_t FilterCode;								//!<Filter Code
uint32_t FilterMask;								//!<Used to 

//Variable declarations (not definitions.. they are defined in other c files)
extern ListStruct_t MList_Data[LISTMAXSIZE];		//!<From M_List.c
extern int MList_Count;								//!<From M_List.c
extern uint32_t records_sent_already;				//!<From MX_GM862.c
extern char setting_EMAILTOADD[60];					//!<From MX_GM862.c
extern char setting_GPRSAPN[30];					//!<From MX_GM862.c
extern char setting_GPRSUSERID[30];					//!<From MX_GM862.c
extern char setting_GPRSPASSW[30];					//!<From MX_GM862.c

ISR(RTC_OVF_vect) {
	//PORTC.OUTSET = (1<<1);
	Wakeup_Source |= WAKEUP_SOURCE_RTC;
	AddTimes(&RTC_c_Time, &RTC_Period);
	secCntr++;
	RTC_sec_ctr++;
	//warningFlasherUpdate();
	AudioHandleSecondTick();
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
	Wakeup_Source |= WAKEUP_SOURCE_CC2500;
}

ISR(PORTA_INT0_vect) {
	Wakeup_Source |= WAKEUP_SOURCE_CC110L;
}

//! Main function
/*!
  A more elaborate description. (can be multiple lines).
  \return 0
*/
int main() {
	
	//memset(0x2000, 0xcc, 4000 );
	
	SAVESP();
	PC_saved[0] = 0;
	PC_saved[1] = 0;
	PC_saved[2] = 0;

	SetupHardware();
	
// 	while(1)
// 	{
// 		
// 		if (Wakeup_Source & WAKEUP_SOURCE_CC110L)
// 		{
// 			len = 20;
// 			if (cc110L_receive_packet(data, &len, pktMetrics))
// 			{
// 				RSSI = cc2500_AdjustRSSI(pktMetrics[0]);
// 			
// 				USART_printf_P(&USARTC0, "rx packet (RSSI: %d) with length %u\r\n", RSSI, len);
// 			
// 				for (int i = 0; i < len; ++i)
// 				{
//  					USART_printf_P(&USARTC0, "%02X ", data[i]);
//  				}
// 			 
// 				USART_tx_String_P(&USARTC0, PSTR("\r\n"));
// 			
// 				USART_printf_P(&USARTC0, "RxBytes: 0x%02X\r\n", cc110L_read_reg(TI_CC110L_RXBYTES));
// 				USART_printf_P(&USARTC0, "MARCSTATE: 0x%02X\r\n", cc110L_read_reg(TI_CC110L_MARCSTATE));
// 			}
// 			
// 			USART_tx_String_P(&USARTC0, PSTR("wake\r\n"));
// 			cc110L_strobe(TI_CC110L_SRX);
// 			Wakeup_Source &= ~(WAKEUP_SOURCE_CC110L);
// 		}
// 		//_delay_ms(100);
// 	}

// 	while(1) {
// 
// 		USART_printf_P(&USARTC0, "TX BUF: 0x%02X\r\n", reg1);
// 		cc110L_tx_packet(data, 3, 0x00);
// 		cc110L_strobe(TI_CCxxx0_SRX);
// 		
// 		_delay_ms(1000);
// 	}
	
	while (1) {
		//RecordCount = 200000UL;
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
		CC_PORT.INTFLAGS = (1<<0);
		//Go to sleep
		set_sleep_mode(SLEEP_SMODE_PSAVE_gc);
		MCU_STAT_LED_OFF();
		sleep_mode();
		
		//testing random generator
		//if (batBakTrig == 19) {
			//int32_t track_id;
			//for (int i = 0; i < 10000; i++) {
				//track_id = PlayRandomTrack();
				//if (track_id < 0) track_id = 0;
				//saveRecord(&RTC_c_Time, 100, 0, 0, track_id);
			//}
			//
			//PlayRandomTrack();
		//}
		
		//Reset watchdog timer
		WDT_RESET();

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
  \return Description as void 
*/
void MainLoop_Active_Mode() {
	MCU_STAT_LED_ON();
			
	if (Startup) {
		//Stuff to do on startup!!
		SAVE_SYS_MSG(&RTC_c_Time, SYS_MSG_PWR_ON, RST.STATUS);
		RST.STATUS = 0b00111111;  //Reset the flags
		
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
	//CC - is GD0 (IRQ) low?, if so, do work!
	//uint8_t stat = 0;
	//while (CC_PORT.IN & (1<<CC_GD0)) {
	#ifndef	CC2500_DISABLE
		if (Wakeup_Source & WAKEUP_SOURCE_CC2500) {
			doRFWorkCC2500(TagDispMode);
			Wakeup_Source &= ~(WAKEUP_SOURCE_CC2500);
			//stat = cc2500_read_reg(TI_CCxxx0_MARCSTATE);
		
			//USART_printf_P(&USARTC0, "CC_STATE: %lu\r\n", stat);
			//_delay_ms(2);
		}
		//Wakeup_Source &= ~(WAKEUP_SOURCE_CC2500);
	#endif
	
	#if defined(INTER_LOGGER_CC110L_INSTALLED) || defined(_433_PICKUP_MODE)
		if (Wakeup_Source & WAKEUP_SOURCE_CC110L) {
			#ifdef _433_PICKUP_MODE
				doRFWorkCC110L_WID(TagDispMode);
			#else
				doRFWorkCC110L();
			#endif
		
			Wakeup_Source &= ~(WAKEUP_SOURCE_CC110L);
		}
	#endif
	
	//USART stuff.  This needs to be last.
	if (Wakeup_Source & WAKEUP_SOURCE_USART) {
		//ShowMenu();
		//I figure that if you're using the USART then you have access to the thing so you could just reset it if
		//something went horribly wrong and don't need the WDT.
		WDT_DISABLE();
		SerCom_err_t tmpErr;
		while (USART_rx_Byte_nb(&USARTC0, &deadByte) == USART_err_Success);
		MCU_STAT_LED_ON();
		if ((tmpErr = Ser_CMDHandler(&USARTC0)) != SerCom_err_Success) {
			Wakeup_Source &= ~(WAKEUP_SOURCE_CC2500);
		}
		Wakeup_Source &= ~(WAKEUP_SOURCE_USART);
		MCU_STAT_LED_OFF();
		#ifdef WDT_ENABLED
		WDT_ENABLE();
		#endif
	}
	
	if (EnterMobCommsBridgeMode) {
		////Iridium_HW_On();
		#ifdef HAS_IRIDIUM_MODULE
		WDT_DISABLE();
		Iridium_SetupHardware();
		Iridium_Unit_On();
		Iridium_BridgeMode();
		Iridium_Unit_Off();
		#ifdef WDT_ENABLED
		WDT_ENABLE();
		#endif
		#else
		#ifdef INTER_LOGGER_CC110L_INSTALLED
		cc110LTester_Run_ChooseMode();
		#endif
		#endif

		EnterMobCommsBridgeMode = 0;
	}
	
	if (EnterMobCommsConfigMode) {
		#ifdef HAS_IRIDIUM_MODULE
		WDT_DISABLE();
		USART_tx_String_P(&USARTC0, PSTR("Configure Iridium...\r\n")); //GM862...\r\n"));
		//Iridium_HW_On();
		Iridium_SetupHardware();
		Iridium_Unit_On();
		Iridium_ConfigureModule();
		Iridium_Unit_Off();
		USART_tx_String_P(&USARTC0, PSTR("Iridium Configure Done.\r\n"));//GM862 Configure Done.\r\n"));
		_delay_ms(2);
		EnterMobCommsConfigMode = 0;
		#ifdef WDT_ENABLED
		WDT_ENABLE();
		#endif
		#endif
	}
	
	AudioMainLoopChecks();
	
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
				#ifdef HAS_IRIDIUM_MODULE
					WDT_DISABLE();
					if (Iridium_Unit_On(1) >= 0) {
						Iridium_Unit_Off();
						//Log record.. time WAS updated from Iridium
						SAVE_SYS_MSG(&RTC_c_Time, SYS_MSG_WAKE_UD_TIME, 0);
					} else {
						//Log record.. time was NOT updated from GSM
						SAVE_SYS_MSG(&RTC_c_Time, SYS_MSG_WAKE_NOTIME, 0);
					}
					#ifdef WDT_ENABLED
					WDT_ENABLE();
					#endif
				#else
					//Log record.. time was NOT updated from Iridium
					SAVE_SYS_MSG(&RTC_c_Time, SYS_MSG_WAKE_NOTIME, 0);
				#endif
			}
			RTC_Time_t tmpTime;
			WDT_RESET();
			TimeAfterTime(&RTC_c_Time, &tmpTime, 0, 0, 0, 0, 15, 0);
				
			if (FirstTimeGreaterOrEqualTo(&tmpTime, &Send_Data_GPRS_Next)) {
				CopyTime(&tmpTime, &Send_Data_GPRS_Next);
				WDT_DISABLE();
				SendData_ReelInTheYears(&RTC_c_Time);
				#ifdef WDT_ENABLED
				WDT_ENABLE();
				#endif
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
	SettingsManager_LoadAllSettings();
	setting_LoggingEnabled = 3; //2 = logging enabled - log everything, 3 = logging enabled - only log if not filtered, 1 = logging disabled?
	FilterStr2Mask(setting_Filter);
	CopyTime(&Send_Data_GPRS_Next, &Send_Data_GPRS_Next_Real);

	if (PWR_GOOD) {
		#if defined(INTER_LOGGER_CC110L_INSTALLED) || defined(INTER_LOGGER_CC110L_DISABLE)
		//Setup CC110L chip
		PORTA.DIRSET = (1<<4);
		PORTA.OUTSET = (1<<4);
		#endif

		SerFlash_SPI_Setup();
	
		RecordCount = FindRecordCount();
		gotRecCnt = 1;
	}	

	PMIC.CTRL = PMIC_HILVLEN_bm|PMIC_MEDLVLEN_bm|PMIC_LOLVLEN_bm;

	USART_tx_String_P(&USARTC0, PSTR("HW Setup Success!\r\n"));

	if (RecordCount == 0xFFFFFFFFl)
		RecordCount = 0x00000000l; //to handle brand new loggers
	
	//Setup Iridium hardware - do this regardless of if we have the iridium module installed - just sets up pins etc.
	Iridium_SetupHardware();
	
	//Initialise list
	//mList_Init();

	//Setup Wav Trigger
	AudioSetupHardware();

	//Init CC2500 and put in RX mode
	if (LOGGINGISENABLED && PWR_GOOD) {
		setup_cc2500(NULL);
		//cc2500_strobe(TI_CCxxx0_SFRX);
		#if defined(INTER_LOGGER_CC110L_INSTALLED) || defined(INTER_LOGGER_CC110L_DISABLE)
			setup_cc110L(NULL);
			#ifdef _433_PICKUP_MODE
				cc110L_set_fixed_packet_length(6);  //TODO: enable for 433pickupmode
			#else
				cc110L_set_variable_packet_length();
			#endif
		#endif
		#ifdef INTER_LOGGER_CC110L_DISABLE
		cc110L_sleep();
		#endif
		#ifdef CC2500_DISABLE
		cc2500_sleep();
		#endif
	}//NRF_RXMODE;
	
	if (!PWR_GOOD) {
		//If the main battery hasn't been plugged in yet... start up in battery backup mode
		EnterBatBakMode(BBAK_REASON_NO_BAT);
	}
	
	LLUT_Init();
		
	//Request time update from network...
	interLoggerRecordTimeSyncRequest(RecordCount ^ setting_LoggerId);
	RndGen_InitSeed(RecordCount^setting_LoggerId);
	
	//Enable watchdog timer
	#ifdef WDT_ENABLED
	WDT_ENABLE();
	#endif
	WDT_RESET();  //used to reset the timer
	
	
// 	uint8_t input[20];
// 	uint8_t encoded_data[128];
// 	uint16_t encoded_len;
// 	uint8_t decoded_data[40];
// 	uint16_t decoded_len;
// 	uint8_t result;
// 
// 	FEC_Encode(input, 20, encoded_data, &encoded_len);
// 
// 	//Corrupt some data (simulate noisy channel)
// 	for (uint16_t i = 1; i < encoded_len; i+=5)
// 	{
// 		encoded_data[i] = 0xFF;
// 	}
// 
// 	result = FEC_Decode(encoded_data, encoded_len, decoded_data, &decoded_len);
// 
// 	USART_printf_P(&USARTC0, "decoded: %u\r\n", result);
// 	_delay_ms(100);
}

uint8_t EnterBatBakMode(uint8_t Reason) {
	
	WDT_RESET();
	
	_delay_ms(100);
	
	if (Reason == BBAK_REASON_NO_BAT) {
		if (PWR_GOOD) {
			//abort!
			return 1;
		}
	}
	
	//Disable watchdog
	WDT_DISABLE();
	
	bBak_Mode = 1;
	bBak_Reason = Reason;

	//Save record count!
	//RCTOEEPROM();
	SettingToEEPROM(RECCOUNT);

	//Power down the nordic chip
	if (PWR_GOOD) {
		//NRF_POWERDOWN
		cc2500_sleep();
		#ifdef INTER_LOGGER_CC110L_INSTALLED
		setup_cc110L(NULL);
		cc110L_sleep();
		#endif
	}
	
	//Interrupt is now disabled in the cc2500_sleep() function.
	//disable NORDIC IRQ interrupt - keep power switch interrupt enabled!
// 	CC_PORT.INTCTRL = PORT_INT0LVL_OFF_gc;//|PORT_INT1LVL_MED_gc;
// 	//No pullup on nordic IRQ
// 	PORTCFG.MPCMASK = (1<<NRF_IRQ);
// 	CC_PORT.PIN0CTRL = PORT_ISC_LEVEL_gc;

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

	//CC_CSN, MOSI and USBTX (tx to computer) as inputs - so no leakage through
	//protection diodes on other devices.
	//SF_CSN as input - same reason as above
	PORTD.DIRCLR = (1<<CC_CSN)|(1<<CC_MOSI)|(1<<SerFlash_CSN)|(1<<CC_GD0)|(1<<CC_GD2);
	//USB_RX - input
	PORTC.DIRCLR = (1<<3);

	//Disable PWRGD interrupt
	PWR_GD_PORT.INTCTRL = PORT_INT0LVL_OFF_gc;
	//USB Detect.. disable (was set to stay enabled?!?)
	USART_USB_IN_PORT.INTCTRL = PORT_INT0LVL_OFF_gc;//PORT_INT0LVL_LO_gc;
	
	//Disable Iridium - quick off - ok to do this even if HAS_IRIDIUM_MODULE is not defined
	Iridium_DisableHardware();
	
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

	//Setting up cc2500 stuff.. a little redundant?!?! seeming that we set it up below.  Do we set the IRQ interrupt in the setup function below?... yes...
	//PORTD.DIRSET = (1<<CC_CSN)|(1<<CC_MOSI)|(1<<SerFlash_CSN);
	//PORTD.PIN2CTRL = PORT_OPC_PULLUP_gc|PORT_ISC_LEVEL_gc;
	//For USB_RX
	//PORTC.DIRSET = (1<<3);	

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
// 	NRF24L01_SPI_Setup();
// 	NRF24L01_SetupnRF24L01(myRF_Chan, myRF_ID);
// 	NRF24L01_Flush_RX_FIFO();
// 	NRF24L01_W_Reg(NREG_STATUS, 0x40);
// 	PORTD.INTCTRL |= PORT_INT0LVL_MED_gc;	//should this be done in the nordic setup function?
//	setup_cc2500(NULL);   //below.. should be here too??

	//Setup Serial Flash
	SerFlash_SPI_Setup();
	
	if (gotRecCnt == 0) {
		RecordCount = FindRecordCount();
	}

	USART_tx_String_P(&USARTC0, PSTR("Awake...\r\n"));

	//CalibrateADCOffset();		//Don't do this here.  If the power connection is dodgey then running this line of code will cause it to get stuck between power down mode and normal mode!
	//tmpBatV = CheckBatteryVoltage();
	
	//Setup Iridium hardware - ok to do this, even if HAS_IRIDIUM_MODULE is not defined
	Iridium_SetupHardware();

	//If logging is enabled or enabled and filtered turn on nordic chip
	if (LOGGINGISENABLED && PWR_GOOD) {
		setup_cc2500(NULL);
		#ifdef INTER_LOGGER_CC110L_INSTALLED
		setup_cc110L(NULL);
		#endif
		SerFlash_SPI_Setup();
		#ifdef CC2500_DISABLE
		cc2500_sleep();
		#endif
	} else {
	}
	
	//Enable watchdog timer
	#ifdef WDT_ENABLED
	WDT_ENABLE();
	#endif

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
 *	- Check that the TI RF chip is working properly and fix it if it's not.
 *	- Update the tag list once every #TAGLISTUPDATE seconds.
 *	- Handle all the SendEmail stuff****
 *	- Check the battery voltage once every #BATVOLTCHECK seconds and if it's too low, put the 
 *	device into battery backup mode.
 *
 */
void RTCWakeChecks() {
	uint8_t startSecs;

	RF_Cal_Cntr++;
	if (RF_Cal_Cntr >= RFCALUPDATE) {
		RF_Cal_Cntr = 0;
		
		startSecs = secCntr;
		
		WDT_RESET();
		#ifndef CC2500_DISABLE
			while ( cc2500_get_RX_FIFO_Status() > 0 && (secCntr-startSecs) < 2) {
				doRFWorkCC2500(TagDispMode);
				_delay_ms(10);
			}
			
			cc2500_strobe(TI_CCxxx0_SIDLE);
			cc2500_strobe(TI_CCxxx0_SFRX);
			cc2500_strobe(TI_CCxxx0_SRX);
		#endif
		#if defined(INTER_LOGGER_CC110L_INSTALLED) || defined(_433_PICKUP_MODE)
			WDT_RESET();
			startSecs = secCntr;
			while ( ((cc110L_readRXReg_FixErrata( TI_CC110L_RXBYTES ) & TI_CC110L_NUM_RXBYTES_MSK) > 0) 
					&& (secCntr-startSecs) < 2) {
				#ifdef _433_PICKUP_MODE
					doRFWorkCC110L_WID(TagDispMode);
				#else
					doRFWorkCC110L();
				#endif
				_delay_ms(10);
			}
			cc110L_calibrate();
		#endif
		
	}

	#ifdef HAS_IRIDIUM_MODULE
	if (SendData_DueForSend(&RTC_c_Time)) {
		WDT_DISABLE();
		SendData_CheckAndSend(&RTC_c_Time);
		#ifdef WDT_ENABLED
		WDT_ENABLE();
		#endif
	}
	#endif
	
	//Calibrate time...
	#ifdef HAS_IRIDIUM_MODULE
	if (RTC_c_Time.Year == 10) {
		RTC_Time_t tmpTime;
		SET_RTC_TIME_T(tmpTime, 1,1,10,0,10,0);
		if (FirstTimeGreaterOrEqualTo(&RTC_c_Time, &tmpTime) && RTC_c_Time.Second == 0 && (RTC_c_Time.Minute % 10 == 0)) {
			WDT_DISABLE();
			if (Iridium_Unit_On() >= 0) {
				USART_tx_String_P(&USARTC0, PSTR("Updating time via Iridium.\r\n"));
		
				if (Iridium_WaitToUpdateRTCFromNetworkTime() == Iridium_err_Success) {
					USART_tx_String_P(&USARTC0, PSTR("Time update success!\r\n"));
				} else {
					USART_tx_String_P(&USARTC0, PSTR("Time update failed!\r\n"));
				}
			}
			Iridium_Unit_Off();
			#ifdef WDT_ENABLED
			WDT_ENABLE();
			#endif
		}
	}
	#endif
	
	LLUT_DecrementIsOkTimersCntr++;
	if (LLUT_DecrementIsOkTimersCntr >= ISOKTIMERUPDATE) {
		LLUT_DecrementIsOkTimersCntr = 0;
		WDT_RESET();
		LLUT_DecrementIsOkTimers();	
	}
	
	LLUT_TxStatusCntr++;
	if (LLUT_TxStatusCntr >= TXSTATUSUPDATE) {
		LLUT_TxStatusCntr = 0;
		WDT_RESET();
		#ifndef _433_PICKUP_MODE
		#ifdef INTER_LOGGER_CC110L_INSTALLED
		interLoggerRecordStatus(LLUT_GetDataPtr(), &RTC_c_Time);
		#endif
		#endif
	}

	CheckBatVolt++;
	if (CheckBatVolt >= BATVOLTCHECK) {
		CheckBatVolt = 0;
		tmpBatV = CheckBatteryVoltage();
		LLUT_SetCurrentLoggerVoltage(tmpBatV);
		if (tmpBatV < 10.5 && tmpBatV > 7.0) {
			//Low battery!
			EnterBatBakMode(BBAK_REASON_LO_BAT);
			Wakeup_Source = 0;
		}
	}
	
	//packet retransmits for inter-logger comms
	#ifndef _433_PICKUP_MODE
	#ifdef INTER_LOGGER_CC110L_INSTALLED
		interLoggerRxUpdate(&RTC_c_Time);
	#endif
	#endif
}

void doRFWorkCC110L() {
	uint8_t data[20];
	uint8_t len;
	uint8_t pktMetrics[2];	
	int8_t RSSI;
	RTC_Time_t mTime;
	int8_t tagRSSI;
	uint32_t tagID, loggerID;
	uint16_t loggerGroup;
	uint8_t loggerShortId;
	LLUTStruct_t* llutInfo;
	uint16_t batVolt;
	uint8_t flashUsage;
	uint8_t statusFlags;
	uint8_t passFilter = 0;
	//char ss[40];
	
	//_delay_ms(500);
	
	#ifdef SHOW_INTERLOGGER_DEBUG
		uint8_t d = cc110L_read_reg(TI_CC110L_RXBYTES);
		USART_printf_P(&USARTC0, "RXBYTES: %02X\r\n", d);
	#endif

	len = 20; //pass in max length of buffer, returns as the len of packet received.
	if (cc110L_receive_packet(data, &len, pktMetrics))
	{
		RSSI = cc110L_AdjustRSSI(pktMetrics[0]);
		
		//make sure CRC_OK is true and RSSI is reasonable before we 'accept' a packet
		if (RSSI > -95 && (pktMetrics[1]&0x80)) {
			
			#ifdef SHOW_INTERLOGGER_DEBUG
				USART_printf_P(&USARTC0, "rx packet (RSSI: %d) with length %u (CRC_OK=%d) and Contents:\r\n", RSSI, len, pktMetrics[1]&0x80);
	 			printBufferHex(data, len);
 			#endif
		 
			//ignore the 'address' byte in the payload (thus the data+1)
			interLoggerDecodePickup( data, &mTime, &tagID, &tagRSSI, &loggerID, &loggerGroup );
		
			if (loggerGroup == setting_LoggerGroupId)
			{
				uint8_t exists = interLoggerRxExists(tagID, loggerID, &mTime, tagRSSI);
			
				//USART_printf_P(&USARTC0, "exists: %d\r\n", exists);
			
				if (exists == 0)
				{
					#ifdef SHOW_INTERLOGGER_DEBUG
						USART_tx_String_P(&USARTC0, PSTR("New Packet\r\n"));
					#endif
					//TODO[ ]: Need to save special packets if we get them - with info about network health.
					if (interLoggerDecodeStatusPickup(data, &mTime, &batVolt, &flashUsage, &statusFlags, &loggerID, &loggerGroup)) {
						//It's a Logger status message
						if (LLUT_FindLoggerOrAdd(loggerID, &loggerShortId, &llutInfo) != mx_err_Success) {
							//can't add because the log is full...  This is a problem!
							//TODO[ ]: record this error somehow!
						} else {
							llutInfo->bat_volt = batVolt;
							llutInfo->flash_usage = flashUsage;
							llutInfo->flags = statusFlags;
						}
						//will automatically retransmit 3 times on wakeups..
						interLoggerRecordRx(tagID, loggerID, &mTime, tagRSSI);
					} else if (interLoggerDecodeTimeSyncPickup(data, &mTime, &loggerID, &loggerGroup)) {
						//It's a time sync message
						if (mTime.Year != 10) { //Don't set time to this if it's an invalid time!
							SAVE_SYS_MSG(&RTC_c_Time, SYS_MSG_PING, 0);
							CopyTime(&mTime, &RTC_c_Time);  //Set the system time to the message time.
							SAVE_SYS_MSG(&RTC_c_Time, SYS_MSG_UPDATED_TIME, SYS_MSG_UPDATE_TIME_SRC_INTERLOGGER);
						}
						//Will automatically retransmit 3 times on wakeups..
						interLoggerRecordRx(tagID, loggerID, &mTime, tagRSSI);
					} else if (tagRSSI == ILRX_TIME_SYNC_REQUEST) {
						if (RTC_c_Time.Year != 10) { 
							interLoggerRecordTimeSyncResponse(&RTC_c_Time, tagID);  //Will be sent 3 times
						}
						//interLoggerRecordRxNoTx(tagID, loggerID, &mTime, tagRSSI);  //Will not be rebroadcast!
					} else if (tagRSSI == ILRX_TIME_SYNC_RESPONSE) {
						//if it's a response message and we don't have a corresponding request, then just ignore the message
					} else {
						//It's a WID tag re-transmit
						if ((tagID & FilterMask) == FilterCode)
							passFilter = 1;
						else
							passFilter = 0;
					
						//TODO[ ]: Should we be saving the WID logs?  Perhaps only on the master save all?
						if (setting_LoggingEnabled == 2 || (setting_LoggingEnabled == 3 && passFilter)) {
							if (LLUT_FindLoggerOrAdd(loggerID, &loggerShortId, &llutInfo) != mx_err_Success) {
								loggerShortId = 0xFF; //Ie, invalid id - we'll still log things but won't know which logger it came from.
							}
							//TODO[ ]: add Activity...
							saveRecord(&mTime, tagID, tagRSSI, /*Activity*/0, loggerShortId); //shortId is 0 -> ie this is our logger
						}
						//will automatically retransmit 3 times on wakeups..
						interLoggerRecordRx(tagID, loggerID, &mTime, tagRSSI);
						//warningFlasherOn(setting_WarningSignalDuration);
						AudioPlay(1);
					}
				} else {  //Tag Does 'exist'
					if (tagRSSI == ILRX_TIME_SYNC_RESPONSE) {  //In this case we found a corresponding TIME_SYNC_REQUEST
						//It's a time sync message
						if (mTime.Year != 10) { //Don't set time if it's an invalid time!
							SAVE_SYS_MSG(&RTC_c_Time, SYS_MSG_PING, 0);
							CopyTime(&mTime, &RTC_c_Time);  //Set the system time to the message time.
							SAVE_SYS_MSG(&RTC_c_Time, SYS_MSG_UPDATED_TIME, SYS_MSG_UPDATE_TIME_SRC_INTERLOGGER);
						}
					}
				}
	// 			else
	// 			{
	// 				USART_tx_String_P(&USARTC0, PSTR("Packet Ignored...\r\n"));
	// 			}
			}
// 			else
// 			{
// 				USART_tx_String_P(&USARTC0, PSTR("Wrong group..\r\n"));
// 			}
		} else {
			#ifdef SHOW_INTERLOGGER_DEBUG
				USART_printf_P(&USARTC0, "rx corrupt packet (RSSI: %d) with length %u (CRC_OK=%d)\r\n", RSSI, len, pktMetrics[1]&0x80);
			#endif
		}

	} else {
		//len = cc110L_read_reg(TI_CC110L_CRC_OK);
		#ifdef SHOW_INTERLOGGER_DEBUG
			USART_printf_P(&USARTC0, "rx corrupt packet.. %d\r\n", len);
		#endif
	}
		
	//USART_tx_String_P(&USARTC0, PSTR("wake\r\n"));
	cc110L_strobe(TI_CC110L_SFRX);
	//Wakeup_Source &= ~(WAKEUP_SOURCE_CC110L);
	
	#ifdef SHOW_INTERLOGGER_DEBUG
		if (USART_USB_IN) {
			#ifndef CC2500_DISABLE
			USART_printf_P(&USARTC0, "CC_IOCFG1: %02X\r\n", cc2500_read_reg(TI_CCxxx0_IOCFG1));
			#endif
			USART_printf_P(&USARTC0, "CC_IOCFG1: %02X\r\n", cc110L_read_reg(TI_CC110L_IOCFG1));
			_delay_ms(2);
		}
	#endif
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
void doRFWorkCC2500(uint8_t dispLvl) {
	RTC_Time_t tmpTime;
	uint32_t rxData;
	uint8_t rxPacket[15];
	uint8_t pktLen = 10;
	uint8_t passFilter = 0;
	uint8_t i = 0;
	uint8_t FIFO_Stat = 0;
	char ss[40];
	uint8_t pktMetrics[2];
	int8_t RSSI;
	uint8_t rxOVF = 0;
	uint8_t Activity = 0;
	uint8_t loggerShortId = 0;  //Always use short id 0 as id for our logger
	uint8_t startSecs = secCntr;

	do {
		CopyTime(&RTC_c_Time, &tmpTime);
		//NRF24L01_R_RX_PAYLOAD(&rxData);
		
		//RX good and CRC ok?
		if (cc2500_get_RX_FIFO_Status() >= 48) {
			//put into idle
			cc2500_strobe(TI_CCxxx0_SIDLE);
			rxOVF = 1;
		}
		
		if (cc2500_receive_packet(rxPacket, &pktLen, pktMetrics)) {
			//get rxData from packet
			//ENSURE CRC_OK is true!
			if (pktLen == CC_PKT_LEN && (pktMetrics[1]&0x80)) {
				//check that first two bytes are appropriate...  What extra info could we TX with this?
				//if (rxPacket[0] == 0x12 && rxPacket[1] == 0x34) {
					//memcpy(&rxData,rxPacket+2,4);
					*(((uint8_t*)&rxData)+0) = rxPacket[5];
					*(((uint8_t*)&rxData)+1) = rxPacket[4];
					*(((uint8_t*)&rxData)+2) = rxPacket[3];
					*(((uint8_t*)&rxData)+3) = rxPacket[2];
					Activity = rxPacket[0];
					
					RSSI = cc2500_AdjustRSSI(pktMetrics[0]);
					
					if ((rxData & FilterMask) == FilterCode)
						passFilter = 1;
					else
						passFilter = 0;

					if (setting_LoggingEnabled == 2 || (setting_LoggingEnabled == 3 && passFilter)) {
						int32_t track_id = PlayRandomTrack();
						if (track_id < 0) track_id = 0;
						saveRecord(&tmpTime, rxData, RSSI, Activity, track_id);
						//saveRecord(&tmpTime, rxData, RSSI, Activity, loggerShortId); //shortId is 0 -> ie this is our logger
					}

					if (passFilter) {					
						//warningFlasherOn(setting_WarningSignalDuration);
						AudioPlay(1);
						//TODO: UNCOMMENT FIRST LINE BELOW (AS WELL AS LINE IN RTCWAKECHECKS) TO ENABLE INTER-LOGGER TX AGAIN!!
						#ifndef _433_PICKUP_MODE
						#ifdef INTER_LOGGER_CC110L_INSTALLED
							interLoggerRecordRx(rxData, setting_LoggerId, &tmpTime, RSSI);
						#endif
						#endif
						////interLoggerTxPickup(&tmpTime, rxData, RSSI, setting_LoggerId, setting_LoggerGroupId);
						
						if (dispLvl >= RF_DISP_TAGID) {
							if (i == 0) _delay_ms(5);							
								
							uint16_t activity_perc = (unsigned int)((double)Activity/(double)0xFF*10000.0);
							uint8_t activity_perc_whole = (uint8_t)(activity_perc/100);
							uint8_t activity_perc_decimal = (uint8_t)(activity_perc-(uint16_t)activity_perc_whole*100);
							sprintf_P(s, PSTR("RX ID: 0x%08lX, RSSI: %d, Activity: %d.%02d% (inst %d)"), rxData, (RSSI), activity_perc_whole, activity_perc_decimal, rxPacket[1]);
							USART_tx_String(&USARTC0, s);
							
							//sprintf_P(s, PSTR("%02X%02X %08lX-%02X%02X"), rxPacket[0], rxPacket[1], rxData, pktMetrics[0], pktMetrics[1]);
							//USART_tx_String(&USARTC0, s);

							if (dispLvl >= RF_DISP_TAGIDTIME) {
								sPrintDateTime(ss, &tmpTime, "");
								sprintf_P(s,PSTR("\r\nTime: %s"), ss);
								USART_tx_String(&USARTC0, s);

								if (dispLvl >= RF_DISP_TAGIDTIMEADD) {
									//(LoggingEnabled>GUI_LOGSTATEDIT_DISABLED) ? ....
									sprintf_P(s, PSTR("\r\nAddress: %lu"), (setting_LoggingEnabled>1) ? ((RecordCount)) : ((uint32_t)0));
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
				//}
			}
		}

		FIFO_Stat = cc2500_get_RX_FIFO_Status();
// 		if (FIFO_Stat > 64) {
// 			//Rx Overflow.. flush buffer
// 			cc2500_strobe(TI_CCxxx0_SFRX);
// 			//put in RX mode again
// 			cc2500_strobe(TI_CCxxx0_SRX);
// 			FIFO_Stat = cc2500_get_RX_FIFO_Status();
// 			//TODO[ ]: change this so that it processes the remaining packets and then puts the CC2500 back into RX mode.
// 		}
		//While RX_EMPTY bit in FIFO_STATUS register is low - while the RX FIFO is not empty
	} while (FIFO_Stat >= CC_PKT_LEN+2 && (secCntr-startSecs) < 2);

	if (rxOVF == 1) {
		//make sure we're still in idle..
		cc2500_strobe(TI_CCxxx0_SIDLE);
		//flush rx fifo
		cc2500_strobe(TI_CCxxx0_SFRX);
		//put into rx
		cc2500_strobe(TI_CCxxx0_SRX);
	} else {
		cc2500_strobe(TI_CCxxx0_SRX);
	}

	//TODO[x]: Test below line!!
	if (dispLvl > 0 && USART_USB_IN) {
		//USART_printf_P(&USARTC0, "CC_MARCSTATE: %02X\r\n", cc2500_read_reg(TI_CCxxx0_MARCSTATE));
		//USART_printf_P(&USARTC0, "CC_IOCFG0: %02X\r\n", cc2500_read_reg(TI_CCxxx0_IOCFG0));
		//USART_printf_P(&USARTC0, "CC_IOCFG1: %02X\r\n", cc110L_read_reg(TI_CC110L_IOCFG1));
		_delay_ms(2);
	}

}

#ifdef _433_PICKUP_MODE
/*!	\brief Process received messages in the CC RX buffer.
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
void doRFWorkCC110L_WID(uint8_t dispLvl) {
	RTC_Time_t tmpTime;
	uint32_t rxData;
	uint8_t rxPacket[10];
	uint8_t pktLen = 10;
	uint8_t passFilter = 0;
	uint8_t i = 0;
	uint8_t FIFO_Stat = 0;
	char ss[40];
	uint8_t pktMetrics[2];
	int8_t RSSI;
	uint8_t rxOVF = 0;
	uint8_t Activity = 0;
	uint8_t startSecs = secCntr;

	do {
		CopyTime(&RTC_c_Time, &tmpTime);
		//NRF24L01_R_RX_PAYLOAD(&rxData);
		
		//RX good and CRC ok?
		if (cc110L_get_RX_FIFO_Status() >= 48) {
			//put into idle
			cc110L_strobe(TI_CC110L_SIDLE);
			rxOVF = 1;
		}
		
		if (cc110L_receive_packet(rxPacket, &pktLen, pktMetrics)) {
			//get rxData from packet
			if (pktLen == CC_PKT_LEN) {
				//check that first two bytes are appropriate...  What extra info could we TX with this?
				//if (rxPacket[0] == 0x12 && rxPacket[1] == 0x34) {
					//memcpy(&rxData,rxPacket+2,4);
					*(((uint8_t*)&rxData)+0) = rxPacket[5];
					*(((uint8_t*)&rxData)+1) = rxPacket[4];
					*(((uint8_t*)&rxData)+2) = rxPacket[3];
					*(((uint8_t*)&rxData)+3) = rxPacket[2];
					Activity = rxPacket[1];
					//rxPacket[0] represents the number of pickups over the last 'SUMMARY_PER' 'SMALL_PER's (in seconds) as a percentage (in hex - so 0xFF=100% and 0x00=0%)
					//rxPacket[1] represents the number of times the accelerometer has triggered in the last 'SMALL_PER' seconds.  It's like instantaneous activity.
					//  If we set SMALL_PER to TX_PERIOD_SEC then frame[1] represents the number of times the accelerometer was triggered since the last tx.  This is what we want...!
					
					RSSI = cc110L_AdjustRSSI(pktMetrics[0]);
					
					//if (!pktMetrics[1]&(1<<7)) RSSI = 127;
					if (!(pktMetrics[1]&(1<<7))) RSSI = 127;
					
					if ((rxData & FilterMask) == FilterCode && RSSI != 127)
						passFilter = 1;
					else
						passFilter = 0;


					if (setting_LoggingEnabled == 2 || (setting_LoggingEnabled == 3 && passFilter)) {
						int32_t track_id = PlayRandomTrack();
						if (track_id < 0) track_id = 0;
						saveRecord(&tmpTime, rxData, RSSI, Activity, track_id);
						//saveRecord(&tmpTime, rxData, RSSI, Activity, setting_LoggerId);
					}

					if (passFilter) {					
						//warningFlasherOn(setting_WarningSignalDuration);
						//AudioPlay(1);
						//TODO: UNCOMMENT FIRST LINE BELOW (AS WELL AS LINE IN RTCWAKECHECKS) TO ENABLE INTER-LOGGER TX AGAIN!!
						#ifndef _433_PICKUP_MODE
						#ifdef INTER_LOGGER_CC110L_INSTALLED
							interLoggerRecordRx(rxData, setting_LoggerId, &tmpTime, RSSI);
						#endif
						#endif
						////interLoggerTxPickup(&tmpTime, rxData, RSSI, setting_LoggerId, setting_LoggerGroupId);
						
						if (dispLvl >= RF_DISP_TAGID) {
							if (i == 0) _delay_ms(5);							
							
							// This version doesn't print Activity as a percent, it prints it as number of avtivations in the last TX_PERIOD seconds.
							//uint16_t activity_perc = (unsigned int)((double)Activity/(double)0xFF*10000.0);
							//uint8_t activity_perc_whole = (uint8_t)(activity_perc/100);
							//uint8_t activity_perc_decimal = (uint8_t)(activity_perc-(uint16_t)activity_perc_whole*100);
							//sprintf_P(s, PSTR("RX ID: 0x%08lX, RSSI: %d, Activity: %d.%02d% (inst %d)"), rxData, (RSSI), activity_perc_whole, activity_perc_decimal, rxPacket[1]);
							sprintf_P(s, PSTR("RX ID: 0x%08lX, RSSI: %d, Activity: %d"), rxData, (RSSI), Activity);

							USART_tx_String(&USARTC0, s);
							
							
							//sprintf_P(s, PSTR("%02X%02X %08lX-%02X%02X"), rxPacket[0], rxPacket[1], rxData, pktMetrics[0], pktMetrics[1]);
							//USART_tx_String(&USARTC0, s);

							if (dispLvl >= RF_DISP_TAGIDTIME) {
								sPrintDateTime(ss, &tmpTime, "");
								sprintf_P(s,PSTR("\r\nTime: %s"), ss);
								USART_tx_String(&USARTC0, s);

								if (dispLvl >= RF_DISP_TAGIDTIMEADD) {
									//(LoggingEnabled>GUI_LOGSTATEDIT_DISABLED) ? ....
									sprintf_P(s, PSTR("\r\nAddress: %lu"), (setting_LoggingEnabled>1) ? ((RecordCount)) : ((uint32_t)0));
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
				//}
			}
		}

		FIFO_Stat = cc110L_get_RX_FIFO_Status();
// 		if (FIFO_Stat > 64) {
// 			//Rx Overflow.. flush buffer
// 			cc2500_strobe(TI_CCxxx0_SFRX);
// 			//put in RX mode again
// 			cc2500_strobe(TI_CCxxx0_SRX);
// 			FIFO_Stat = cc2500_get_RX_FIFO_Status();
// 			//TODO[ ]: change this so that it processes the remaining packets and then puts the CC2500 back into RX mode.
// 		}
		//While RX_EMPTY bit in FIFO_STATUS register is low - while the RX FIFO is not empty
	} while (FIFO_Stat >= CC_PKT_LEN+2 &&  (secCntr-startSecs) < 2);


	//if (rxOVF == 1) {
		////make sure we're still in idle..
		//cc110L_strobe(TI_CC110L_SIDLE);
		////flush rx fifo
		//cc110L_strobe(TI_CC110L_SFRX);
		////put into rx
		//cc110L_strobe(TI_CC110L_SRX);
	//}
	cc110L_calibrate(); //this is important!! otherwise it doesn't pickup more than one packet. 
						//it's not the calibration itself that's important I think it falls out of RX mode?

	//TODO[x]: Test below line!!
	if (dispLvl > 0 && USART_USB_IN) _delay_ms(2);

}
#endif

/*!	
 *	If a tag in #MList_Data has not been picked up in the last #TAGLISTUPDATE seconds, save a
 *	stop record to the flash for that tag at the last time it was picked up and remove it
 *	from the list.  Does this for each tag in #MList_Data.
 */
// void UpdateTagList() {
// 
// 	int i;
// 	RTC_Time_t t1,t2;
// 
// 	ClearTime(&t2);
// 	t2.Second = TAGLISTUPDATE;
// 
// 	//(1)USART_printf_P(&USARTC0, "MList_Count: %d\r\n", MList_Count);
// 
// 	for (i = 0; i < MList_Count; i++) {
// 		//USART_printf_P(&USARTC0, "MList_Item: %d ID = %08lX\r\n", i, MList_Data[i].ID);
// 		CopyTime(&(MList_Data[i].LastTime), &t1);
// 		AddTimes(&t1, &t2);
// 
// 		if (FirstTimeGreaterOrEqualTo(&RTC_c_Time, &t1)) {
// 			//(1)USART_printf_P(&USARTC0, "Erase: %d ID = %08lX\r\n", i, MList_Data[i].ID);
// 			//if the current time is after the last wakeup plus TAGLISTUPDATE seconds...
// 			//Remove from list and add a finish record
// 			RecordToFlash(&(MList_Data[i].LastTime), MList_Data[i].ID, 0x01);
// 			mList_RemoveItemWithID(MList_Data[i].ID);
// 			i--;
// 			//(1)USART_printf_P(&USARTC0, "MList_Count: %d\r\n", MList_Count);
// 		}
// 
// 	}
// 
// }

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

void clearWakeupSource(uint8_t src)
{
	Wakeup_Source &= ~(src);
}