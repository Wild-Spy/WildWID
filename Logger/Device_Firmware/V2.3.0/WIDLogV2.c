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

RTC_Time_t RTC_c_Time;					//!<RTC time -> real time stored here.
RTC_Time_t RTC_Period;					//!<Time that elapses per RTC period (1 second)
uint32_t RTC_sec_ctr = 0;

//RTC_Time_t Send_Data_GPRS_Next;		//!<Next time to attempt to send an email
RTC_Time_t Send_Data_GPRS_Next_Real;	//!<Next scheduled time to send an email
//RTC_Time_t Send_Data_GPRS_Period;		//!<An email is scheduled to be sent once every period
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
	warningFlasherUpdate();
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
		//GM862_Unit_On(1);
		//GM862_Unit_Off();
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
	//CC - is GD0 (IRQ) low?, if so, do work!
	//uint8_t stat = 0;
	//while (CC_PORT.IN & (1<<CC_GD0)) {
	if (Wakeup_Source & WAKEUP_SOURCE_CC2500) {
		doRFWorkCC2500(TagDispMode);
		Wakeup_Source &= ~(WAKEUP_SOURCE_CC2500);
		//stat = cc2500_read_reg(TI_CCxxx0_MARCSTATE);
		
		//USART_printf_P(&USARTC0, "CC_STATE: %lu\r\n", stat);
		//_delay_ms(2);
	}
	//Wakeup_Source &= ~(WAKEUP_SOURCE_CC2500);
	
	#if INTER_LOGGER_CC110L_INSTALLED
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
		SerCom_err_t tmpErr;
		while (USART_rx_Byte_nb(&USARTC0, &deadByte) == USART_err_Success);
		MCU_STAT_LED_ON();
		if ((tmpErr = Ser_CMDHandler(&USARTC0)) != SerCom_err_Success) {
			Wakeup_Source &= ~(WAKEUP_SOURCE_CC2500);
		}
		Wakeup_Source &= ~(WAKEUP_SOURCE_USART);
		MCU_STAT_LED_OFF();
	}
	
	if (EnterMobCommsBridgeMode) {
		////Iridium_HW_On();
		//Iridium_SetupHardware();
		//Iridium_Unit_On();
		//Iridium_BridgeMode();
		//Iridium_Unit_Off();
		
		cc110LTester_Run_ChooseMode();

		EnterMobCommsBridgeMode = 0;
	}
	
	if (EnterMobCommsConfigMode) {
		USART_tx_String_P(&USARTC0, PSTR("Configure Iridium...\r\n")); //GM862...\r\n"));
		//Iridium_HW_On();
		Iridium_SetupHardware();
		Iridium_Unit_On();
		Iridium_ConfigureModule();
		Iridium_Unit_Off();
		USART_tx_String_P(&USARTC0, PSTR("Iridium Configure Done.\r\n"));//GM862 Configure Done.\r\n"));
		_delay_ms(2);
		EnterMobCommsConfigMode = 0;
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
				if (Iridium_Unit_On(1) >= 0) {
					Iridium_Unit_Off();
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
	//setup in CC2500 setup function!
// 	PORTCFG.MPCMASK = (1<<NRF_IRQ);
// 	PORTD.PIN0CTRL = PORT_OPC_PULLUP_gc|PORT_ISC_LEVEL_gc;
// 	PORTD.DIRCLR = (1<<NRF_IRQ);
// 	PORTD.INT0MASK = (1<<NRF_IRQ);
// 	PORTD.INTCTRL |= PORT_INT0LVL_MED_gc;

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
	//ReadEEPROM(myRF_ID,5,RFID_ADD);
	//ReadEEPROM(&myRF_Chan,1,RFCHAN_ADD);
	
	//RCFROMEEPROM();	//use 	FindRecordCount();
	//LOGMODEFROMEEPROM();
	SettingsManager_LoadAllSettings();
	setting_LoggingEnabled = 2;
	FilterStr2Mask(setting_Filter);
	CopyTime(&Send_Data_GPRS_Next, &Send_Data_GPRS_Next_Real);
	
// 	READFILTEREEPROM();
// 	FilterStr2Mask(setting_Filter);
// 	GPRSSENDNEXTFROMEEPROM();
// 	CopyTime(&Send_Data_GPRS_Next, &Send_Data_GPRS_Next_Real);
// 	GPRSSENDPERFROMEEPROM();
// 	LOGGERNAMEFROMEEPROM();
// 	EMAILTOADDFROMEEPROM();
// 	GPRSSETINGSFROMEEPROM();

	if (PWR_GOOD) {
		//Setup CC110L chip
		PORTA.DIRSET = (1<<4);
		PORTA.OUTSET = (1<<4);
		
		//Setup Nordic RF Chip
// 		NRF24L01_SPI_Setup();
// 		NRF24L01_SetupnRF24L01(myRF_Chan, myRF_ID);
// 		NRF24L01_Flush_RX_FIFO();
// 		NRF24L01_W_Reg(NREG_STATUS, 0x40);
		//moved cc2500 init to below
		SerFlash_SPI_Setup();
		//setup_cc2500(NULL);
		//setup_cc110L(NULL);
		//#ifdef _433_PICKUP_MODE
			//cc110L_set_fixed_packet_length(6);  //TODO: enable for 433pickupmode
		//#else
			//cc110L_set_variable_packet_length();
		//#endif 
		
		//USART_printf_P(&USARTC0, "CC_PKTLEN: %02X\r\n", cc2500_read_reg(TI_CCxxx0_PKTLEN));
		//USART_printf_P(&USARTC0, "CC_PATABLE: %02X\r\n", cc2500_read_reg(TI_CCxxx0_PATABLE));
		//USART_printf_P(&USARTC0, "CC_IOCFG1: %02X\r\n", cc2500_read_reg(TI_CCxxx0_IOCFG1));
		//USART_printf_P(&USARTC0, "CC_IOCFG1: %02X\r\n", cc110L_read_reg(TI_CC110L_IOCFG1));
		//_delay_ms(2);

		
		//Setup Serial Flash
		//SerFlash_SPI_Setup();		
		RecordCount = FindRecordCount();
		gotRecCnt = 1;
	}	

	PMIC.CTRL = PMIC_HILVLEN_bm|PMIC_MEDLVLEN_bm|PMIC_LOLVLEN_bm;

	USART_tx_String_P(&USARTC0, PSTR("HW Setup Success!\r\n"));

	if (RecordCount == 0xFFFFFFFFl)
		RecordCount = 0x00000000l;
	
	//Setup Iridium hardware
	Iridium_SetupHardware();
	
	//Initialise list
	mList_Init();

	//Init CC2500 and put in RX mode
	if (LOGGINGISENABLED && PWR_GOOD) {
		setup_cc2500(NULL);
		//cc2500_strobe(TI_CCxxx0_SFRX);
		setup_cc110L(NULL);
		#ifdef _433_PICKUP_MODE
			cc110L_set_fixed_packet_length(6);  //TODO: enable for 433pickupmode
		#else
			cc110L_set_variable_packet_length();
		#endif 
	}//NRF_RXMODE;
	
	if (!PWR_GOOD) {
		//If the main battery hasn't been plugged in yet... start up in battery backup mode
		EnterBatBakMode(BBAK_REASON_NO_BAT);
	}
	
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
	//RCTOEEPROM();
	SettingToEEPROM(RECCOUNT);

	//Power down the nordic chip
	if (PWR_GOOD) {
		//NRF_POWERDOWN
		cc2500_sleep();
		cc110L_sleep();
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
	
	//Disable Iridium - quick off
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
	
	//Setup Iridium hardware
	Iridium_SetupHardware();

	//If logging is enabled or enabled and filtered turn on nordic chip
	if (LOGGINGISENABLED && PWR_GOOD) {
		setup_cc2500(NULL);
		setup_cc110L(NULL);
		SerFlash_SPI_Setup();
	} else {
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
	uint8_t startSecs;

	RF_Cal_Cntr++;
	if (RF_Cal_Cntr >= RFCALUPDATE) {
		RF_Cal_Cntr = 0;
		
		startSecs = secCntr;
		
		while ( cc2500_get_RX_FIFO_Status() > 0 && (secCntr-startSecs) < 2) {
			doRFWorkCC2500(TagDispMode);
			_delay_ms(10);
		}
		cc2500_strobe(TI_CCxxx0_SIDLE);
		cc2500_strobe(TI_CCxxx0_SFRX);
		cc2500_strobe(TI_CCxxx0_SRX);
		
		#if INTER_LOGGER_CC110L_INSTALLED
			while ( cc110L_readRXReg_FixErrata( TI_CC110L_RXBYTES ) & TI_CC110L_NUM_RXBYTES_MSK > 0 
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

	if (FirstTimeGreaterOrEqualTo(&RTC_c_Time, &Send_Data_GPRS_Next)) {
		SendEmailChecks();
	}

	CheckBatVolt++;
	if (CheckBatVolt >= BATVOLTCHECK) {
		CheckBatVolt = 0;
		tmpBatV = CheckBatteryVoltage();
		if (tmpBatV < 10.5 && tmpBatV > 7.0) {
			//Low battery!
			EnterBatBakMode(BBAK_REASON_LO_BAT);
			Wakeup_Source = 0;
		}
	}
	
	//packet retransmits for inter-logger comms
	//REMOVE FOR NOW - REPLACE WHEN YOU WANT INTER-LOGGER RX again!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
	#ifndef _433_PICKUP_MODE
		interLoggerRxUpdate();
	#endif
}

mx_err_t SendIridiumSBDMessages()
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
	
	#define SBD_MIN_MSG_CHARGE	30 //ie if a message is < 30 bytes you still get charged for 30 bytes.
	SettingsManager_LoadGPRSSettings();
	
// 	setting_IridiumBytesSentThisMonth
// 	setting_IridiumMonthlyByteLimit
// 	setting_LogsSentViaIridium
	
	//In terms of satellite data, I think number of detections since last transmission, which tags (not likely to be that many) and last tag detected and whether a sign was activated (unless the default to being with is to activate after any detection) would all be useful I think, provided data allowance isn't exceeded.
	
	//Message Format:
	//Index and Length both in bytes below
	/*    Index   | Length | Description
	 *          0 |      1 | Message Type 
	 *          1 |      4 | Number of detections since last transmission
	 *          5 |      4 | Last tag ID detected
	 * For each tag detected since last transmission (t is counter)
	 *    9+(t*6) |      4 | Tag ID
	 *  9+(t*6+4) |      2 | Tag pickups since last transmission
	 */

	//Message Type
	*((uint8_t*)(buffer+0)) = 0;
	//Number of detections since last transmission
	*((uint32_t*)(buffer+1)) = RecordCount - setting_LogsSentViaIridium;
	//Last tag ID detected
	tmpFlags = loadRecord(RecordCount, &tmpTime, &tmpID, (uint8_t*)&RSSI, &Activity);
	*((uint32_t*)(buffer+5)) = tmpID;
	len = 9;
	
	//Create the list
	mCountList_Clear();
	for (i = setting_LogsSentViaIridium; i < RecordCount; ++i)
	{
		tmpFlags = loadRecord(i, &tmpTime, &tmpID, (uint8_t*)&RSSI, &Activity);
		if (mCountList_AddItem(tmpID) == mx_err_Overflow) break;
	}
	setting_LogsSentViaIridium = i;
	
	//Put the summary into the message
	for (i = 0; i < mCountList_GetCount(); ++i)
	{
		if (i > (340-9)/6) break;
		
		if (mCountList_Item(i, &listItem) == mx_err_Success)
		{
			//Tag ID
			*((uint32_t*)(buffer+(9+i*6))) = listItem->ID;
			//Number of Pickups
			*((uint16_t*)(buffer+(9+i*6+4))) = listItem->count;
		}
	}

	for (i = 3; i > 0; --i) {
		if (Iridium_SendMOMessage(buffer, len) == Iridium_err_Success) break;
	}
	
	//if (!i) something went wrong;
	
	if (len < SBD_MIN_MSG_CHARGE)
	{
		setting_IridiumBytesSentThisMonth += SBD_MIN_MSG_CHARGE;
	}
	else
	{
		setting_IridiumBytesSentThisMonth += len;
	}
	
	SettingsManager_SaveGPRSSettings();
	return mx_err_Success;
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
	USART_tx_String_P(&USARTC0, PSTR("Turning Iridium On!\r\n"));
	/*uint8_t tmpByte = 0;
	USART_rx_Byte(&USARTC0, 10000, &tmpByte);
	if (tmpByte == '1') {
		send_success = 1;
	}*/
	//TODO[ ]: This section all looks good.. haven't tested the actual send email bit though
	if (Iridium_Unit_On() >= 0) {
		USART_tx_String_P(&USARTC0, PSTR("Sending MO Message.\r\n"));
		
		if (SendIridiumSBDMessages() == mx_err_Success)
		{
			send_success = 1;
			USART_tx_String_P(&USARTC0, PSTR("Send Data Success!\r\n"));
		}
	}
	Iridium_Unit_Off();
		
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
		//GPRSSENDNEXTTOEEPROM();
		SettingToEEPROM(NEXTGPRSSEND);
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

void doRFWorkCC110L() {
	uint8_t data[20];
	uint8_t len;
	uint8_t pktMetrics[2];	
	int8_t RSSI;
	RTC_Time_t mTime;
	int8_t tagRSSI;
	uint32_t tagID, loggerID;
	uint16_t loggerGroup;
	//char ss[40];
	
	//_delay_ms(500);
	
	uint8_t d = cc110L_read_reg(TI_CC110L_RXBYTES);
	USART_printf_P(&USARTC0, "RXBYTES: %02X\r\n", d);
	
	len = 20; //pass in max length of buffer, returns as the len of packet received.
	if (cc110L_receive_packet(data, &len, pktMetrics))
	{
		RSSI = cc110L_AdjustRSSI(pktMetrics[0]);
		
		USART_printf_P(&USARTC0, "rx packet (RSSI: %d) with length %u and Contents:\r\n", RSSI, len);
 		printBufferHex(data, len);
		 
		//ignore the 'address' byte in the payload (thus the data+1)
		interLoggerDecodePickup( data, &mTime, &tagID, &tagRSSI, &loggerID, &loggerGroup );
		
		if (loggerGroup == setting_LoggerGroupId)
		{
			uint8_t exists = interLoggerRxExists(tagID, loggerID, &mTime, tagRSSI);
			
			//USART_printf_P(&USARTC0, "exists: %d\r\n", exists);
			
			if (exists == 0)
			{
				USART_tx_String_P(&USARTC0, PSTR("New Packet\r\n"));
				interLoggerRecordRx(tagID, loggerID, &mTime, tagRSSI);
				warningFlasherOn(setting_WarningSignalDuration);
				//will automatically retransmit 3 times on wakeups..
			}
// 			else
// 			{
// 				USART_tx_String_P(&USARTC0, PSTR("Packet Ignored...\r\n"));
// 			}
		}
// 		else
// 		{
// 			USART_tx_String_P(&USARTC0, PSTR("Wrong group..\r\n"));
// 		}
	} else {
		//len = cc110L_read_reg(TI_CC110L_CRC_OK);
		USART_printf_P(&USARTC0, "rx corrupt packet.. %d\r\n", len);
	}
		
	//USART_tx_String_P(&USARTC0, PSTR("wake\r\n"));
	cc110L_strobe(TI_CC110L_SRX);
	//Wakeup_Source &= ~(WAKEUP_SOURCE_CC110L);
		
	if (USART_USB_IN) {
		USART_printf_P(&USARTC0, "CC_IOCFG1: %02X\r\n", cc2500_read_reg(TI_CCxxx0_IOCFG1));
		USART_printf_P(&USARTC0, "CC_IOCFG1: %02X\r\n", cc110L_read_reg(TI_CC110L_IOCFG1));
		_delay_ms(2);
	}
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
			if (pktLen == CC_PKT_LEN) {
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
						saveRecord(&tmpTime, rxData, RSSI, Activity);
					}

					if (passFilter) {					
						warningFlasherOn(setting_WarningSignalDuration);
						//TODO: UNCOMMENT FIRST LINE BELOW (AS WELL AS LINE IN RTCWAKECHECKS) TO ENABLE INTER-LOGGER TX AGAIN!!
						#ifndef _433_PICKUP_MODE
							interLoggerRecordRx(rxData, setting_LoggerId, &tmpTime, RSSI);
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
	} while (FIFO_Stat >= CC_PKT_LEN+2);


	if (rxOVF == 1) {
		//make sure we're still in idle..
		cc2500_strobe(TI_CCxxx0_SIDLE);
		//flush rx fifo
		cc2500_strobe(TI_CCxxx0_SFRX);
		//put into rx
		cc2500_strobe(TI_CCxxx0_SRX);
	}

	//TODO[x]: Test below line!!
	if (dispLvl > 0 && USART_USB_IN) {
		USART_printf_P(&USARTC0, "CC_IOCFG1: %02X\r\n", cc2500_read_reg(TI_CCxxx0_IOCFG1));
		USART_printf_P(&USARTC0, "CC_IOCFG1: %02X\r\n", cc110L_read_reg(TI_CC110L_IOCFG1));
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
					Activity = rxPacket[0];
					
					RSSI = cc110L_AdjustRSSI(pktMetrics[0]);
					
					if (!pktMetrics[1]&(1<<7)) RSSI = 127;
					
					if ((rxData & FilterMask) == FilterCode && RSSI != 127)
						passFilter = 1;
					else
						passFilter = 0;


					if (setting_LoggingEnabled == 2 || (setting_LoggingEnabled == 3 && passFilter)) {
						saveRecord(&tmpTime, rxData, RSSI, Activity);
					}

					if (passFilter) {					
						warningFlasherOn(setting_WarningSignalDuration);
						//TODO: UNCOMMENT FIRST LINE BELOW (AS WELL AS LINE IN RTCWAKECHECKS) TO ENABLE INTER-LOGGER TX AGAIN!!
						#ifndef _433_PICKUP_MODE
							interLoggerRecordRx(rxData, setting_LoggerId, &tmpTime, RSSI);
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
	} while (FIFO_Stat >= CC_PKT_LEN+2);


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
uint8_t saveRecord(RTC_Time_t* mTime, uint32_t ID, uint8_t RSSI, uint8_t Activity) {
	//ListStruct_t* ListItem;

	//if in current list of tags...
	//if (mList_ItemWithID(ID, &ListItem) == 0) {
		//Update the latest time
	//	CopyTime(mTime, &(ListItem->LastTime));
	//} else {
		//if not in the current list of tags...
		if (RecordToFlash(mTime, ID, 0x03, RSSI, Activity) == 0) {
			return 2;
		}
	//	uint32_t Addy = RecordCount - 1;
	//	if (mList_AddItem(ID, Addy, mTime) != 0) {
			//we're fucked!! - out of space in the list.
			//well, not really, it will just log every event rather than just start and stop now
			//because it won't actually get added to the list.
			//(1)USART_tx_String_P(&USARTC0, PSTR("LIST FULL!!! we're boned!\r\n"));
	//		return 1;
	//	}
	//}
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
uint8_t loadRecord(uint32_t RecordNo, RTC_Time_t* mTime, uint32_t* ID, uint8_t* RSSI, uint8_t* Activity) {
	uint8_t tZip[5];

	if (RecordNo*FLASHRECORD_SIZE + FLASHRECORD_SIZE+1 < SerFlash_MAXADDRESS) {
		ClearTime(mTime);
		SerFlash_ReadBytes(RecordNo*FLASHRECORD_SIZE, 5, tZip);
		SerFlash_ReadBytes(RecordNo*FLASHRECORD_SIZE+5, 4, (uint8_t*)ID);
		SerFlash_ReadBytes(RecordNo*FLASHRECORD_SIZE+9, 1, (uint8_t*)RSSI);
		SerFlash_ReadBytes(RecordNo*FLASHRECORD_SIZE+10, 1, (uint8_t*)Activity);

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
uint8_t RecordToFlash(RTC_Time_t* mTime, uint32_t ID, uint8_t Flags, uint8_t RSSI, uint8_t Activity) {
	uint8_t tZip[5];

	if (RecordCount*FLASHRECORD_SIZE + FLASHRECORD_SIZE <= SerFlash_MAXADDRESS) {
		//RTC_Time_t tmpTime1, tmpTimeAdd;

		Flags &= 0b00000011;
		ZipTime(mTime, tZip, Flags);
		SerFlash_WriteBytes((uint32_t)(RecordCount*FLASHRECORD_SIZE), 5, tZip);
		//SerFlash_WriteBytes((uint32_t)(RecordCount*FLASHRECORD_SIZE+5), 4, (uint8_t*)ID);
		SerFlash_WriteBytes((uint32_t)(RecordCount*FLASHRECORD_SIZE+5), 4, (uint8_t*)(&ID));
		SerFlash_WriteBytes((uint32_t)(RecordCount*FLASHRECORD_SIZE+9), 1, &RSSI);
		SerFlash_WriteBytes((uint32_t)(RecordCount*FLASHRECORD_SIZE+10), 1, &Activity);
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
	uint8_t tmpData[FLASHRECORD_SIZE];
	
	//return (uint16_t)1000;
	
	//return (SerFlash_MAXADDRESS/FLASHRECORD_SIZE);
	
	//TODO[ ]: Can improve the efficiency of this function
	for (cnt = 0; cnt*FLASHRECORD_SIZE <= SerFlash_MAXADDRESS-FLASHRECORD_SIZE; cnt++) {
		SerFlash_ReadBytes(cnt*FLASHRECORD_SIZE, FLASHRECORD_SIZE, tmpData);
		
		FFCount = 0;
		for (i = 0; i < FLASHRECORD_SIZE; i++) {
			if (tmpData[i] == 0xFF) {
				FFCount++;
			}
		}			
		if 	(FFCount == FLASHRECORD_SIZE) {
			//How it works:
			//RC		1 2 3 4 5
			//hasData	1 1 1 1 1 0
			//cnt		0 1 2 3 4 5
			//return val = 5
			return cnt;
		}
	}
	
	return (SerFlash_MAXADDRESS/FLASHRECORD_SIZE);
	
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