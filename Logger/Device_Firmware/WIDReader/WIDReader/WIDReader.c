/*
 * WIDLog.c
 *
 *  Created on: 06/08/2010
 *      Author: MC
 */

#include "WIDReader.h"

char s[80];
uint8_t RF_ID[5];
uint8_t RF_Chan;
uint8_t myRF_ID[5];
uint8_t myRF_Chan = 2;
volatile uint16_t timer10ms = 0;
volatile uint8_t Wakeup_Source = WAKEUP_SOURCE_NONE;
//volatile uint8_t chkRF = 0;
uint8_t PrintTime = 0;
uint32_t RecordCount = 0;
uint8_t TagUpdate = 0;
uint8_t CheckBatVolt = 0;
struct RTCTime RTC_c_Time;
struct RTCTime RTC_Period;
uint8_t rxFastVal = 0;
uint8_t batBakMode = 0;//1;
uint8_t bBak_Reason = BBAK_REASON_NONE;
int FirstRXByte = -1;
volatile uint8_t wakeFromBB = 0;
uint8_t PC_saved[3];
uint16_t SP_saved;
volatile uint8_t batBakTrig = 2;
int ADCOffset = 0;
uint8_t PowerDown = 0;

extern struct ListStruct MList_Data[LISTMAXSIZE];
extern int MList_Count;
extern char lastKey;

ISR(RTC_OVF_vect) {
	//PORTC.OUTSET = (1<<1);
	Wakeup_Source |= WAKEUP_SOURCE_RTC;
	AddTimes(&RTC_c_Time, &RTC_Period);
}

ISR(PORTA_INT0_vect) {
	//If at least one pin is low...
	//if ((KP_PORT.IN&(KP_C1_bm|KP_C2_bm|KP_C3_bm))!=(KP_C1_bm|KP_C2_bm|KP_C3_bm))
	if (lastKey == 0)
		Wakeup_Source |= WAKEUP_SOURCE_KP;
	else {
		KP_KeyScan();
		KP_IntMode();
	}
}

ISR(PORTB_INT1_vect) {

}

ISR(PORTB_INT0_vect, ISR_NAKED) {
	//DO NOT TOUCH THIS FUNCTION!!!!!
	//DO NOT ADD ANY VARIABLES, DON'T CHANGE ANY CODE...
	//JUST DONT FUCKING TOUCH IT!  SHIT WILL EXPLODE!
	uint8_t* RetADD;

	RetADD = (uint8_t*)((SPH<<8)+SPL);

	//Just assume a hard off... if it's plugged in the nordic chip will keep running. This is OK.
	//Otherwise we need to make sure that we handle the case that the USB power could be lost while
	//shutting down the peripherals.  Unlikely, but possible.
	EnterBatBakMode(BBAK_REASON_NO_BAT);
	batBakTrig = 1;

	/*PORTB.INTFLAGS = (1<<0);	//INT0 flag
	PORTC.INTFLAGS = (1<<0);	//INT0 flag
	PORTD.INTFLAGS = (1<<0);	//INT0 flag
	RTC.INTFLAGS = (1<<0);		//OVF flag
	TCC0.INTFLAGS = (1<<0);		//OVF flag
	TCC1.INTFLAGS = (1<<0);		//OVF flag*/

	//Set return address to Saved Program Counter Address
	*(RetADD+0) = PC_saved[0];
	*(RetADD+1) = PC_saved[1];
	*(RetADD+2) = PC_saved[2];
	reti();
}

ISR(PORTC_INT0_vect) {
	Wakeup_Source |= WAKEUP_SOURCE_USART;
}

ISR(PORTD_INT0_vect) {
	Wakeup_Source |= WAKEUP_SOURCE_NORDIC;
}

ISR(TCC0_OVF_vect) {
	cli();
	if (timer10ms) timer10ms--;
	else {
		TC0_ConfigClockSource(&TCC0, TC_CLKSEL_OFF_gc);
		TCC0.INTCTRLA = TC_OVFINTLVL_OFF_gc;
	}
	sei();
}

int main() {
	float tmpBatV;

	//TODO[ ]: If it works without these, delete them
	//PORTE.DIRSET = (1<<1);
	//PORTE.OUTCLR = (1<<1);

	//if power is low startup in bat backup mode.
	//if (!(PORTB.IN&(1<<2)))
	//	bBakStartup();
	SAVESP();
	PC_saved[0] = 0;
	PC_saved[1] = 0;
	PC_saved[2] = 0;

	SetupHardware();
	EnterBatBakMode(BBAK_REASON_SW_OFF);

	while (1) {

		if (batBakTrig == 2) {
			SAVESTATE();
			if (batBakTrig == 1) {
				//Restore Stack Pointer
				SPH = (uint8_t)(SP_saved>>8);
				SPL = (uint8_t)(SP_saved);
			}
			batBakTrig = 0;
		}

		//Clear Nordic IRQ flag
		PORTD.INTFLAGS = 0x01;
		set_sleep_mode(SLEEP_SMODE_PSAVE_gc);
		PORTB.OUTSET = (1<<3);
		sleep_mode();

		if (bBak_Reason == BBAK_REASON_NONE) {
			PORTB.OUTCLR = (1<<3);

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

			if (Wakeup_Source & WAKEUP_SOURCE_KP) {
				Wakeup_Source &= ~(WAKEUP_SOURCE_KP);
				char aa;
				aa = KP_KeyScan();
				KP_INTDIS();
				LCD_printf_Pos_P(0,0,"%c", aa);
				//_delay_ms(1);
				//USART_tx_Byte(&USARTC0,aa);
				//_delay_ms(2);
				GUI_Menu_Keypress_Handler(aa);
				KP_IntMode();
			}

			//TODO[ ]: Consider removing the loop or putting a limit to the number of
			//		   loops as otherwise bombardment of tags could cause the device to
			//		   stop responding (get stuck in the below loop and never service
			//		   the USART.
			//Nordic - is IRQ low?, if so, do work!
			while ((PORTD.IN & (1<<2)) == 0) {
				doRFWork(rxFastVal);
			}
			Wakeup_Source &= ~(WAKEUP_SOURCE_NORDIC);

			//USART stuff.  This needs to be last.
			if (Wakeup_Source & WAKEUP_SOURCE_USART) {
				ShowMenu();
				Wakeup_Source &= ~(WAKEUP_SOURCE_USART);
			}

			if (PowerDown) {
				EnterBatBakMode(BBAK_REASON_SW_OFF);
				PowerDown = 0;
			}

		} else if (bBak_Reason == BBAK_REASON_SW_OFF) {
			//PORTB.OUTCLR = (1<<3);
			if (Wakeup_Source & WAKEUP_SOURCE_KP) {
				Wakeup_Source &= ~(WAKEUP_SOURCE_KP);

				SMPS_ON();
				//PORTB.OUTCLR = (1<<3);
				//while (!(PORTB.IN&(1<<PWR_GD_PIN))) {
				//	set_sleep_mode(SLEEP_SMODE_PSAVE_gc);
				//	sleep_mode();
				//}

				//_delay_ms(300);
				PORTB.OUTSET = (1<<3);
				if (KP_KeyStatus(4,1) && KP_KeyStatus(4,3)) {
					WakeFromBatBakMode();
				}
				KP_IntMode();
				lastKey = 0;
			}
		} else if (bBak_Reason == BBAK_REASON_NO_BAT) {
			//We are in battery backup mode
			if (PORTB.IN&(1<<2)) {
				bBak_Reason = BBAK_REASON_NONE;
				batBakMode = 0;
				//if batbak interrupt was called while we were waking up, exit!
				Wakeup_Source |= WAKEUP_SOURCE_PWR;
				WakeFromBatBakMode();

				_delay_ms(1);
				USART_tx_String_P(&USARTC0, PSTR("Wakeup from battery backup mode.\r\n"));
				_delay_ms(2);
			}
		} else if (bBak_Reason == BBAK_REASON_LO_BAT) {
			//We are in soft battery backup mode
			//PORTB.OUTCLR = (1<<3);
			//_delay_ms(5);
			CheckBatVolt++;
			if (CheckBatVolt >= BATVOLTCHECK) {
				CheckBatVolt = 0;
				bBak_Reason = BBAK_REASON_NONE;
				tmpBatV = CheckBatteryVoltage();
				if (tmpBatV > 3.75) {
					//Good Battery
					WakeFromBatBakMode();
					_delay_ms(1);
					USART_tx_String_P(&USARTC0, PSTR("Wakeup from battery backup mode.\r\n"));
					_delay_ms(2);
				} else if (tmpBatV < 3.4) {
					//Shit is about to explode.  Shut down! Permanently!
					//disable RTC
					CLKSYS_Disable(OSC_XOSCEN_bm);
					cli();
					PORTE.DIRSET = (1<<1);
					PORTE.OUTSET = (1<<1);

					while (1) {
						set_sleep_mode(SLEEP_SMODE_PDOWN_gc);
						sleep_mode();
					}
				}
			}
		}
	}

	return 0;
}

void SetupHardware() {

	//TODO[ ]: Tidy this function up
	//TODO[ ]: Add more comments to this function

	//Power Good pin setup (PB2)
	PORTB.DIRCLR = (1<<PWR_GD_PIN);
	PORTCFG.MPCMASK = (1<<PWR_GD_PIN);
	PORTB.PIN0CTRL = PORT_ISC_FALLING_gc;
	PORTB.INT0MASK = (1<<PWR_GD_PIN);
	PORTB.INTCTRL = PORT_INT0LVL_MED_gc;
	//TODO[ ]: Disable this interrupt before going to sleep!

	//Nordic IRQ pin setup
	PORTCFG.MPCMASK = (1<<NRF_IRQ);
	PORTD.PIN0CTRL = PORT_OPC_PULLUP_gc|PORT_ISC_LEVEL_gc;
	PORTD.DIRCLR = (1<<NRF_IRQ);
	PORTD.INT0MASK = (1<<NRF_IRQ);
	PORTD.INTCTRL = PORT_INT0LVL_MED_gc;

	//PB3 as output (Debugging LED)
	PORTB.DIRSET = (1<<3);
	PORTB.OUTCLR = (1<<3);

	//Battery voltage measurement EN - setup and turn off
	PORTE.DIRSET = (1<<BATV_READ_PIN)|(1<<SMPS_PWR_PIN);
	PORTE.OUTCLR = (1<<BATV_READ_PIN)|(1<<SMPS_PWR_PIN);

	//USART Rx pin interrupt
	PORTC.PIN2CTRL = PORT_ISC_RISING_gc; //|PORT_OPC_PULLDOWN_gc
	PORTC.INT0MASK = (1<<2);
	PORTC.INTCTRL = PORT_INT0LVL_LO_gc;

	//Screen Backlight - setup and turn off
	//TODO[ ]: Remove this if it still works - should be setup in screen setup
	PORTB.DIRSET = (1<<0);
	PORTB.OUTCLR = (1<<0);

	//Clock Setup
	//12MHz - 2MHz Internal RC Oscillator and PLL
	CLKSYS_Enable(OSC_PLLSRC_RC2M_gc);
	while (CLKSYS_IsReady(OSC_RC2MRDY_bm) == 0);
	CLKSYS_PLL_Config(OSC_PLLSRC_RC2M_gc,6);
	CLKSYS_Enable(OSC_PLLEN_bm);
	while (CLKSYS_IsReady(OSC_PLLRDY_bm) == 0);
	CLKSYS_Prescalers_Config(CLK_PSADIV_1_gc, CLK_PSBCDIV_1_1_gc);
	CLKSYS_Main_ClockSource_Select(CLK_SCLKSEL_PLL_gc);

	//USART Setup for 115200bps
	USART_Setup(&PORTC, &USARTC0, 176, -5);
	USART_tx_String_P(&USARTC0, PSTR("Serial Comms Initialised \r\n"));

	//Setup RTC
	CLKSYS_XOSC_Config(OSC_FRQRANGE_04TO2_gc, true, OSC_XOSCSEL_32KHz_gc);
	CLKSYS_Enable(OSC_XOSCEN_bm);
	while (CLKSYS_IsReady(OSC_XOSCRDY_bm) == 0);
	CLKSYS_RTC_ClockSource_Enable(CLK_RTCSRC_TOSC32_gc);
	RTC_Setup();
	//PORTB.INTFLAGS = PORT_INT0IF_bm;
	sei();

	//ReadEEPROM(myRF_ID,5,RFIDADD);
	//ReadEEPROM(&myRF_Chan,1,RFCHANADD);
	//ReadEEPROM((uint8_t*)&RecordCount,4,RECCOUNT_ADD);
	//if (!wakeFromBB) ReadTimeEEPROM(&RTC_c_Time, TIMESAVEADD);


	/*//scrTest();
	LCD_InitDisplay();
	LCD_Init();
	_delay_ms(2);

	SCR_BL_ON();
	showSplash();

	//Keypad Setup
	KP_IntMode();*/

	/*//Setup Nordic RF Chip
	NRF24L01_SPI_Setup();
	NRF24L01_SetupnRF24L01(myRF_Chan, myRF_ID);
	NRF24L01_Flush_RX_FIFO();
	NRF24L01_W_Reg(NREG_STATUS, 0x40);

	//Setup Serial Flash
	SerFlash_SPI_Setup();
	PMIC.CTRL = PMIC_HILVLEN_bm|PMIC_MEDLVLEN_bm|PMIC_LOLVLEN_bm;*/

	USART_tx_String_P(&USARTC0, PSTR("Hardware Setup Successful!\r\n"));

	//if (RecordCount == 0xFFFFFFFFl)
	//	RecordCount = 0x00000000l;

	//USART_printf_P(&USARTC0, "RecordCount: %lu\r\n", RecordCount);
	USART_tx_String_P(&USARTC0, PSTR("Going to sleep. Turn on device for serial Comms.\r\n"));
	_delay_ms(2);

	//Put Nordic Into RX mode
	//NRF_RXMODE;
}

void bBakStartup() {

	//Battery voltage measurement EN - setup and turn off
	PORTE.DIRSET = (1<<BATV_READ_PIN)|(1<<SMPS_PWR_PIN);
	PORTE.OUTCLR = (1<<BATV_READ_PIN)|(1<<SMPS_PWR_PIN);

	//Power Good pin setup (PB2)
	//PORTB.DIRCLR = (1<<2);
	//PORTB.PIN2CTRL = PORT_ISC_BOTHEDGES_gc;
	//PORTB.INT1MASK = (1<<2);
	//PORTB.INTCTRL = PORT_INT1LVL_MED_gc;
	PMIC.CTRL = PMIC_HILVLEN_bm|PMIC_MEDLVLEN_bm|PMIC_LOLVLEN_bm;
	sei();
	//_delay_ms(200);

	while (!(PORTB.IN&(1<<2))) {
		set_sleep_mode(SLEEP_SMODE_PSAVE_gc);
		PORTB.OUTSET = (1<<3);
		sleep_mode();
		PORTB.OUTCLR = (1<<3);
		_delay_ms(500);
	}

	PORTB.OUTCLR = (1<<3);
	_delay_us(500);
	PORTB.OUTSET = (1<<3);

	PC_saved[0] = 0;
	PC_saved[1] = 0;
	PC_saved[2] = 0;

	PORTB.INT1MASK = 0;
	PORTB.INTCTRL = PORT_INT1LVL_OFF_gc;

}

void EnterBatBakMode(uint8_t Reason) {
	batBakMode = 1;
	bBak_Reason = Reason;

	SCR_BL_OFF();
	//disable IRQ interrupt
	PORTD.INTCTRL = PORT_INT0LVL_OFF_gc;
	//No pullup on nordic IRQ
	PORTCFG.MPCMASK = (1<<NRF_IRQ);
	PORTD.PIN0CTRL = PORT_ISC_LEVEL_gc;
	/*//softOFF - USB still plugged in or Low Batt, peripherals powered but put
	//into low power mode.
	if (Reason == 1) {
		batBakMode = 2;
		NRF_POWERDOWN;
		//SerFlash_DeepPwrDwn(1);
	} else {*/
		//Screen stuff
		PORTC.DIRCLR = (1<<0)|(1<<1)|(1<<4)|(1<<5);
		//Mosfets
		//Battery voltage measurement EN - setup and turn off
		PORTE.DIRSET = (1<<BATV_READ_PIN)|(1<<SMPS_PWR_PIN);
		PORTE.OUTCLR = (1<<BATV_READ_PIN)|(1<<SMPS_PWR_PIN);
		//KeyPad
		KP_INTDIS();
	//}
	//NRF_CSN and MOSI and USBTX (tx to computer) and SF_CSN as input
	PORTD.DIRCLR = (1<<NRF_CSN)|(1<<5)|(1<<1);
	PORTC.DIRCLR = (1<<3);

	KP_IntMode();
	lastKey = 0;

	//Disable PWRGD interrupt
	PORTB.INTCTRL = PORT_INT0LVL_OFF_gc;
	//Turn Power OFF
	SMPS_OFF();
	Wakeup_Source &= ~(WAKEUP_SOURCE_PWR);
}

void WakeFromBatBakMode() {
	//Turn Power ON
	SMPS_ON();

	//Clear PWRGD interrupt
	PORTB.INTFLAGS = PORT_INT0IF_bm;
	//Enable PWRGD interrupt
	PORTB.INTCTRL = PORT_INT0LVL_MED_gc;

	PORTD.DIRSET = (1<<NRF_CSN)|(1<<NRF_MOSI)|(1<<SerFlash_CSN);
	PORTD.PIN2CTRL = PORT_OPC_PULLUP_gc|PORT_ISC_LEVEL_gc;
	PORTC.DIRSET = (1<<NRF_CE);

	ReadEEPROM(myRF_ID,5,RFIDADD);
	ReadEEPROM(&myRF_Chan,1,RFCHANADD);
	ReadEEPROM((uint8_t*)&RecordCount,4,RECCOUNT_ADD);

	if (RecordCount == 0xFFFFFFFFl)
		RecordCount = 0x00000000l;

	//USART Setup for 115200bps
	USART_Setup(&PORTC, &USARTC0, 176, -5);
	USART_tx_String_P(&USARTC0, PSTR("Serial Comms Initialised \r\n"));

	//Setup Nordic RF Chip
	NRF24L01_SPI_Setup();
	NRF24L01_SetupnRF24L01(myRF_Chan, myRF_ID);
	NRF24L01_Flush_RX_FIFO();
	NRF24L01_W_Reg(NREG_STATUS, 0x40);
	PORTD.INTCTRL = PORT_INT0LVL_MED_gc;

	//Setup Serial Flash
	SerFlash_SPI_Setup();

	LCD_InitDisplay();
	LCD_Init();
	_delay_ms(2);

	SCR_BL_ON();
	showSplash();

	//Keypad Setup
	KP_IntMode();

	//TODO[ ]: if (batBakMode == 2) SerFlash_WakeupFromPowerdownMode
	USART_tx_String_P(&USARTC0, PSTR("Module Awake...\r\n"));

	CalibrateADCOffset();

	NRF_RXMODE;

	batBakMode = 0;
	bBak_Reason = BBAK_REASON_NONE;
	Wakeup_Source |= WAKEUP_SOURCE_PWR;
}

float CheckBatteryVoltage() {
	int ADCResult;
	float ResultVolt;
	//int ResVoltInt;
	//int ResVoltIntDec;

	PORTA.DIRCLR = (1<<7)|(1<<6);
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
	PORTE.OUTSET = (1<<0);
	_delay_ms(2);

	ADCResult = ADCRead(8)-ADCOffset;//-49;//- ADCOffset;

	//if (ADCResult > 10000)
	//	ADCResult = 0;

	ResultVolt = (((float)ADCResult)/4096.0)*5.553;//*11.0;//*10.33;//*10.53;//*10.813;//*10.21;//*10.53;//*9.413; // 1V ref, 12bit ADC count
	//ResVoltInt = (int)ResultVolt;
	//ResVoltIntDec = ((ResultVolt-ResVoltInt)*10000.0);

	//CLKSYS_Prescalers_Config(CLK_PSADIV_1_gc, CLK_PSBCDIV_1_1_gc);
	//USART_Setup(&PORTC, &USARTC0, 176, -5);

	//USART_printf_P(&USARTC0, "ADC: 0x%04X = %d", ADCResult, ResVoltInt);
	//USART_printf_P(&USARTC0, ".%04d\r\n", ResVoltIntDec);
	//_delay_ms(2);

	ADCA.CTRLA = 0;
	PORTE.OUTCLR = (1<<0);

	return ResultVolt;
}

void CalibrateADCOffset() {

	PORTA.DIRCLR = (1<<7);
	ADCA.CTRLB =  ADC_RESOLUTION_12BIT_gc;//|(1<<4);
	ADCA.REFCTRL = ADC_REFSEL_INT1V_gc|ADC_BANDGAP_bm;//ADC_REFSEL_VCC_gc; //Vcc/1.6ADC_REFSEL_INT1V_gc|ADC_BANDGAP_bm;
	CLKSYS_Prescalers_Config(CLK_PSADIV_1_gc, CLK_PSBCDIV_1_1_gc);
	ADCA.PRESCALER = ADC_PRESCALER_DIV128_gc; //5.859375 kHz
	ADCA.CH0.CTRL = ADC_CH_GAIN_1X_gc|ADC_CH_INPUTMODE_SINGLEENDED_gc;
	ADCA.CH0.MUXCTRL = ADC_CH_MUXPOS_PIN5_gc;	//SHOULD BE PIN 7!!!!!!!!!!!!!!!!!!!!!!!! TESTING WITH PIN 6

	//ADC Calibration Bytes
	ADCA.CALL = ReadCalibrationByte( offsetof(NVM_PROD_SIGNATURES_t, ADCACAL0) );
	ADCA.CALH = ReadCalibrationByte( offsetof(NVM_PROD_SIGNATURES_t, ADCACAL1) );

	//Enable
	ADCA.CTRLA |= ADC_ENABLE_bm;

	//PORTE0 - MOSFET for Bat Voltage Read
	PORTE.OUTSET = (1<<0);
	PORTA.DIRCLR = (1<<5);
	PORTA.PIN5CTRL = PORT_OPC_PULLDOWN_gc;
	_delay_ms(2);

	ADCOffset = ADCRead(12);

	//WriteEEPROM((uint8_t*)&ADCOffset,2,ADCOFFSETV);

	PORTA.PIN5CTRL = PORT_OPC_TOTEM_gc;
	CLKSYS_Prescalers_Config(CLK_PSADIV_1_gc, CLK_PSBCDIV_1_1_gc);

	ADCA.CTRLA = 0;
	PORTE.OUTCLR = (1<<0);

}

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
	ADCResult = ADCResult>>SAMP;///(1<<SAMP);
	return (uint16_t)ADCResult;
}

void RTCWakeChecks() {


	//TODO[ ]: Is this needed??
	//if (((tmpByte = NRF24L01_R_Reg(NREG_STATUS))&0b01000000)) {
	if (!(NRF24L01_R_Reg(NREG_FIFOSTATUS)&(1<<0))) {
		if ((PORTD.IN & (1<<2)) == 0) {
			//doRFWork(rxFastVal);
			//USART_printf_P(&USARTC0, "RF Work in RTC wake, Int Low\r\n" ,1);
			//_delay_ms(1);
		} else {
			//Flush RX buffer
			NRF24L01_Flush_RX_FIFO();
			//Clear RX interrupt flag
			NRF24L01_W_Reg(NREG_STATUS, 0x40);
			USART_printf_P(&USARTC0, "RF Work in RTC wake, Int High\r\n" ,1);
			_delay_ms(1);
		}
	}

	//TagUpdate++;
	//if (TagUpdate >= TAGLISTUPDATE) {
	//	TagUpdate = 0;
	//	UpdateTagList();
	//}

	if (PrintTime) {
		_delay_ms(2);
		sPrintDateTime(s, &RTC_c_Time, "Time");
		USART_tx_String(&USARTC0, s);
		LCD_PosSetUp(0,0);
		sPrintDateTime(s, &RTC_c_Time,"");
		LCD_DisplayString(LCD_ALIGN_CENTER, s, NORMAL);
		_delay_ms(1);
	}

	CheckBatVolt++;
	if (CheckBatVolt >= BATVOLTCHECK) {
		CheckBatVolt = 0;
		if (CheckBatteryVoltage() < 3.6) {
			//Low battery!
			USART_printf_P(&USARTC0, "Entering Soft Battery Backup Mode!\r\n" ,1);
			_delay_ms(2);
			EnterBatBakMode(1);
			Wakeup_Source = 0;
		}
	}

}


void ShowMenu() {
	struct RTCTime tmpTime;
	unsigned int i;
	int j;
	uint32_t ii;
	char retChar;
	uint32_t rxData;

	_delay_ms(1);
	USART_tx_String_P(&USARTC0, PSTR("USART Woke me.\r\n"));
	USART_tx_String_P(&USARTC0, PSTR("Enter appropriate command.\r\n>"));
	//flush USART RX buffer
	while (USART_rx_Byte_nb(&USARTC0) > 0);
	//Receive byte
	i = USART_rx_Byte(&USARTC0,10000);
	if (i > 0) {
		retChar = (char)i;
		switch (retChar) {
		case 'v':
			//Print Version
			USART_tx_String_P(&USARTC0, PSTR_PRODUCT_N);
			USART_tx_Byte(&USARTC0, ' ');
			USART_tx_String_P(&USARTC0, PSTR_CODE_V);
			USART_tx_String_P(&USARTC0, PSTR_NEWLINE);
			USART_tx_String_P(&USARTC0, PSTR_WILDSPY);
			USART_tx_String_P(&USARTC0, PSTR_NEWLINE);
			break;
		case 'T':
			//Set Date/Time
			USART_tx_String_P(&USARTC0, PSTR("Enter Date/Time (DDMMYYHHMMSS).\r\n>"));

			for (i = 0; i < 12; i++) {
				j = USART_rx_Byte(&USARTC0, 0);
				if (j > 0)
					s[i] = j;
				else
					break;
			}
			s[12] = '\0';

			if (j > 0) {
				if (str2Time(s, &tmpTime) == 0)
					CopyTime(&tmpTime, &RTC_c_Time);

				sPrintDateTime(s, &RTC_c_Time, "Time");
				USART_tx_String(&USARTC0, s);
			}
			break;
		case 'e':
			//Set RecordCount
			i = USART_RX_Int(&USARTC0, 4, USART_FF_SHOWFORMAT|USART_FF_ECHOCHARS);
			RecordCount = i;
			RCTOEEPROM();
			break;
		case 'r':
			//Read Entry at Address
			i = USART_RX_Int(&USARTC0, 4, USART_FF_SHOWFORMAT|USART_FF_ECHOCHARS);
			if (i > 0) {
				//Testing.. read and print it..
				loadRecord(i, &tmpTime, &rxData);
				USART_printf_P(&USARTC0, "Address: %lu\r\n", ((uint32_t)i)*RECORD_SIZE);
				sPrintDateTime(s, &tmpTime, "Read Time");
				USART_tx_String(&USARTC0, s);
				USART_printf_P(&USARTC0, "RX Data:0x%08lX\r\n", rxData);
			}
			break;
		case '.':
			//Save a dummy record
			//read an value (XXXX)
			rxData = USART_RX_Int(&USARTC0, 4, USART_FF_SHOWFORMAT|USART_FF_ECHOCHARS);
			//Testing.. read and print it..
			if (rxData > 0)
				saveRecord(&RTC_c_Time, &rxData);
			break;
		case 't':
			//Enable Time Printing Every Second
			PrintTime ^= 1;
			break;
		case 'c':
			//Print Record Count
			USART_printf_P(&USARTC0, "Record Count: %lu\r\n", RecordCount);
			break;
		case 'R':
			//RX Records one at a time, no CRC
			for (ii = 0; ii < RecordCount; ii++) {
				if (USART_USB_IN) {
					loadRecord(ii, &tmpTime, &rxData);
					for (uint8_t j = 0; j < 6; j++)
						USART_tx_Byte(&USARTC0, *(((uint8_t*)&tmpTime)+j));
					for (uint8_t j = 0; j < 4; j++)
						USART_tx_Byte(&USARTC0, *(((uint8_t*)&rxData)+j));
					j = USART_rx_Byte(&USARTC0, 0);
					if (retChar > 0) {
						switch (retChar) {
						case 'g': //good
							break;
						case 'r': //resend
							ii--;
							break;
						case 'c': //cancel
							ii = RecordCount;
							break;
						}
					}
				} else
					break;
			}
			break;
		case 'A':
			//RX Records, Faster, CRC checksum.
			;int recPerGrp = 10000;
			uint8_t k = 0;
			uint16_t CheckSUM = 0xFFFF;
			for (k = 0; k < 4; k++)
				USART_tx_Byte(&USARTC0, *(((uint8_t*)&RecordCount)+k));
			for (ii = 0; ii < RecordCount; ii+=recPerGrp) {
				CheckSUM = 0xFFFF;
				if (ii+recPerGrp > RecordCount)
					recPerGrp = RecordCount-ii;
				USART_tx_Byte(&USARTC0, *(((uint8_t*)&recPerGrp)+0));
				USART_tx_Byte(&USARTC0, *(((uint8_t*)&recPerGrp)+1));
				for (j = 0; j < recPerGrp; j++) {
					if (USART_USB_IN) {
						loadRecord(ii+j, &tmpTime, &rxData);
						//send data and update CRC
						for (k = 0; k < 6; k++) {
							USART_tx_Byte(&USARTC0, *(((uint8_t*)&tmpTime)+k));
							CheckSUM = _crc16_update(CheckSUM, *(((uint8_t*)&tmpTime)+k));
						}
						for (k = 0; k < 4; k++) {
							USART_tx_Byte(&USARTC0, *(((uint8_t*)&rxData)+k));
							CheckSUM = _crc16_update(CheckSUM, *(((uint8_t*)&rxData)+k));
						}
					} else
						//If USB is not plugged in, abort!
						ii = RecordCount;
				}
				for (k = 0; k < 2; k++)
					USART_tx_Byte(&USARTC0, *(((uint8_t*)&CheckSUM)+k));
				j = USART_rx_Byte(&USARTC0, 0);
				retChar = (uint8_t)j;
				if (retChar > 0) {
					switch (retChar) {
					case 'g': //good
						break;
					case 'r': //resend
						ii-=recPerGrp;
						//send less data
						recPerGrp = recPerGrp>>1;
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
			break;
		case 'E':
			//Erase DataFlash
			USART_tx_String_P(&USARTC0, PSTR("Are you sure you want to erase the data flash? (y/n)\r\n"));
			retChar = USART_rx_Byte(&USARTC0, 0);
			if (retChar == 'y' || retChar == 'Y') {
				USART_tx_String_P(&USARTC0, PSTR("Erasing Data Flash.  (May take a few seconds.)\r\n"));
				SerFlash_ChipErase(0x74);
				SerFlash_WaitRDY();
				USART_tx_String_P(&USARTC0, PSTR("DataFlash Erased!\r\n"));
				RecordCount = 0;
				RCTOEEPROM();
			}
			break;
		case '+':
			//Add Entry
			rxData = 0x12345678;
			saveRecord(&RTC_c_Time, &rxData);
			break;
		case '?':
			//Get RF Values
			//RFCHAN
			sprintf_P(s,PSTR("RFCHAN:%d\r\n"), myRF_Chan);
			USART_tx_String(&USARTC0, s);
			//RFID
			sprintf_P(s,PSTR("RFID:"));
			USART_tx_String(&USARTC0, s);
			for (i = 0; i < 4; i++) {
				sprintf_P(s,PSTR("%02X-"), myRF_ID[i]);
				USART_tx_String(&USARTC0, s);
			}
			sprintf_P(s,PSTR("%02X\r\n"), myRF_ID[4]);
			USART_tx_String(&USARTC0, s);
			break;
		case '*':
			//Set RF Values
			//RFCHAN
			USART_tx_String_P(&USARTC0, PSTR("RFCHAN (###):\r\n"));
			if (USART_rx_String_F(&USARTC0, s, USART_TIMEOUT, 3, USART_FF_ECHOCHARS) == 0) {
				myRF_Chan = 100*(s[0]-'0')  + 10*(s[1]-'0') + (s[2]-'0');
				WriteEEPROM(&myRF_Chan, 1, RFCHANADD);
			}
			USART_tx_String_P(&USARTC0, PSTR("\r\n"));
			//RFID
			USART_tx_String_P(&USARTC0, PSTR("RFID (XX-XX-XX-XX-XX):\r\n"));
			for (i = 0; i < 5; i++) {
				if (USART_USB_IN) {
					if (USART_rx_String_F(&USARTC0, s, USART_TIMEOUT, 2, USART_FF_ECHOCHARS) == 0) {
						if (i < 4) USART_tx_Byte(&USARTC0, '-');
						myRF_ID[i] = (uint8_t)strtoul(s, (char **)NULL,16);
					} else
						i = 6;
				}
			}
			USART_tx_String_P(&USARTC0, PSTR("\r\n"));

			WriteEEPROM(myRF_ID,5,RFIDADD);
			//Setup the SPI port
			NRF24L01_SPI_Setup();
			NRF24L01_SetupnRF24L01(myRF_Chan, myRF_ID);
			NRF_RXMODE;

			break;
		case 'n':
			//Get Nordic Info
			USART_printf_P(&USARTC0, "Nordic STATUS: 0x%02X\r\n", NRF24L01_R_Reg(NREG_STATUS));
			USART_printf_P(&USARTC0, "Nordic CONFIG: 0x%02X\r\n", NRF24L01_R_Reg(NREG_CONFIG));
			USART_printf_P(&USARTC0, "Nordic ENRXADDR: 0x%02X\r\n", NRF24L01_R_Reg(NREG_EN_RXADDR));
			USART_printf_P(&USARTC0, "Nordic SETUP_AW: 0x%02X\r\n", NRF24L01_R_Reg(NREG_SETUP_AW));
			USART_printf_P(&USARTC0, "Nordic RF_CH: 0x%02X\r\n", NRF24L01_R_Reg(NREG_RF_CH));
			USART_printf_P(&USARTC0, "Nordic RF_SETUP: 0x%02X\r\n", NRF24L01_R_Reg(NREG_RF_SETUP));
			USART_printf_P(&USARTC0, "Nordic RX_ADDR_P0: 0x%02X\r\n", NRF24L01_R_Reg(NREG_RX_ADDR_P0));
			break;
		case 'f':
			//Set Verbose Mode
			USART_printf_P(&USARTC0, "Verbose mode: ", 0);
			if ((j = USART_rx_Byte(&USARTC0,0)) >= '0') {
				rxFastVal = (uint8_t)(j-'0');
				USART_printf_P(&USARTC0, "%d\r\n", rxFastVal);
			}
			break;
		case 'b':
			//Put into forced battery backup mode
			//TODO[ ]: Make this work!
			USART_printf_P(&USARTC0, "BBak Enabled\r\n",0);
			_delay_ms(2);
			EnterBatBakMode(1);
			break;
		}
	}

	if (batBakMode == 0) {
		USART_tx_String_P(&USARTC0, PSTR("Going to sleep.  Press any key to wake.\r\n"));

		while ((PORTD.IN & (1<<2)) == 0)
			doRFWork(rxFastVal);
		Wakeup_Source &= ~(WAKEUP_SOURCE_NORDIC);

		_delay_ms(2);
	}

}

void doRFWork(uint8_t fast) {
	struct RTCTime tmpTime;
	uint32_t rxData;
	uint8_t i = 0;

	do {
		CopyTime(&RTC_c_Time, &tmpTime);
		NRF24L01_R_RX_PAYLOAD(&rxData);
		saveRecord(&tmpTime, &rxData);

		if (fast > 0) {
			if (i == 0) _delay_ms(5);
			//USART_tx_String_P(&USARTC0, PSTR("nRF24L01+ Woke me.\r\n"));
			USART_printf_P(&USARTC0, "RX ID: 0x%08lX", rxData);
			//LCD_printf_Pos_P(LCD_Line, 0,"RX ID: 0x%08lX", rxData);
			//LCD_DisplayString(0,s,NORMAL);
			LCD_Print_Terminal(s);

			if (fast > 1) {
				USART_tx_String_P(&USARTC0, PSTR("\r\nTime: "));
				sPrintDateTime(s, &tmpTime, "");
				USART_tx_String(&USARTC0, s);
				if (fast > 2) {
					USART_printf_P(&USARTC0, "\r\nAddress: %lu", (uint32_t)((RecordCount-1)*RECORD_SIZE));
				}
			}
			USART_tx_String_P(&USARTC0, PSTR(".\r\n"));
		}
		i++;
		//While RX_EMPTY bit in FIFO_STATUS register is low - while the RX FIFO is not empty
	} while (!(NRF24L01_R_Reg(NREG_FIFOSTATUS)&(1<<0)));

	//NRF24L01_Flush_RX_FIFO();
	NRF24L01_W_Reg(NREG_STATUS, 0x40);

	if (fast > 0) _delay_ms(2);

}

void UpdateTagList() {

	int i;
	struct RTCTime t1,t2;

	ClearTime(&t2);
	t2.Second = TAGLISTUPDATE;

	USART_printf_P(&USARTC0, "MList_Count: %d\r\n", MList_Count);

	for (i = 0; i < MList_Count; i++) {
		USART_printf_P(&USARTC0, "MList_Item: %d ID = %08lX\r\n", i, MList_Data[i].ID);
		CopyTime(&(MList_Data[i].LastTime),&t1);
		AddTimes(&t1,&t2);

		if (FirstTimeGreater(&RTC_c_Time, &t1)) {
			USART_printf_P(&USARTC0, "Erase: %d ID = %08lX\r\n", i, MList_Data[i].ID);
			//if the current time is after the last wakeup plus TAGLISTUPDATE seconds...
			//Remove from list and add a finish record
			//RecordToFlash(&(MList_Data[i].LastTime),&(MList_Data[i].ID), 0x01);
			mList_RemoveItemWithID(&(MList_Data[i].ID));
			USART_printf_P(&USARTC0, "MList_Count: %d\r\n", MList_Count);
		}

	}

}

uint8_t saveRecord(struct RTCTime* mTime, uint32_t* ID) {
	//struct ListStruct* ListItem;

	//if in current list of tags...
	//if (mList_ItemWithID(ID, &ListItem) == 0) {
		//Update the latest time
		//CopyTime(mTime,&(ListItem->LastTime));
	//} else {
		//if not in the current list of tags...
		return RecordToFlash(mTime, ID, 0x03);
		//uint32_t Addy = RecordCount - 1;
		//if (mList_AddItem(ID, &Addy, mTime) != 0) {
			//we're fucked!! - out of space in the list.
		//}
	//}
}

uint8_t loadRecord(uint32_t RecordNo, struct RTCTime* mTime, uint32_t* ID) {
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

uint8_t RecordToFlash(struct RTCTime* mTime, uint32_t* ID, uint8_t Flags) {
	uint8_t tZip[5];

	if (RecordCount*RECORD_SIZE + 10 < SerFlash_MAXADDRESS) {
		Flags &= 0b00000011;
		ZipTime(mTime, tZip, Flags);
		SerFlash_WriteBytes((uint32_t)(RecordCount*RECORD_SIZE), 5, tZip);
		SerFlash_WriteBytes((uint32_t)(RecordCount*RECORD_SIZE+5), 4, (uint8_t*)ID);
		//USART_printf_P(&USARTC0, "RC: %lu\r\n", RecordCount);
		RecordCount++;
		//USART_printf_P(&USARTC0, "RC: %lu\r\n", RecordCount);
		RCTOEEPROM();
		return 1;
	} else
		return 0;
}

inline void WriteTimeEEPROM(struct RTCTime* mTime, uint16_t Address) {
	WriteEEPROM((uint8_t*)mTime, 6, Address);
}

inline void ReadTimeEEPROM(struct RTCTime* mTime, uint16_t Address) {
	ReadEEPROM((uint8_t*)mTime, 6, Address);
}

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

void StopTim10ms() {
	//Stop the timer
	TC0_ConfigClockSource(&TCC0, TC_CLKSEL_OFF_gc);
	TCC0.INTCTRLA = TC_OVFINTLVL_OFF_gc;
	//Clear the timer
	timer10ms = 0;
}

void save_PC(void){
	//I worry that changing any small build variables will cause
	//this function not to work!!

	uint8_t* RetADD;
	RetADD = (uint8_t*)((SPH<<8)+SPL);
	//Save Program Counter Address
	PC_saved[0] = *(RetADD+0);
	PC_saved[1] = *(RetADD+1);
	PC_saved[2] = *(RetADD+2);
}

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

