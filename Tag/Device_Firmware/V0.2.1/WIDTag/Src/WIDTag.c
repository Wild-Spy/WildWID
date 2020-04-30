/*
 * WIDTag.c
 *
 *		  Name: Wireless ID Tag
 *  Created on: 05/09/2010
 *      Author: Matthew Cochrane
 *  	   MCU: ATMEGA328P
 *   Fuse Bits:
 *   	High - 0xD9
 *   	 Low - 0xE2
 *
 */

#include "WIDTag.h"

char s[80];
char s1[50];
volatile unsigned char patience;
volatile uint8_t Wakeup_Source = WAKEUP_SOURCE_NONE;
uint32_t Device_ID;
uint8_t RF_ID[5];
uint8_t RF_Chan;
uint8_t DevArmed;
volatile uint16_t timer10ms = 0;
struct RTCTime RTC_c_Time;
struct RTCTime RTC_Period;
//struct RTCTime TX_Period;
uint8_t TX_Period;
uint8_t PrintTime = 0;
volatile uint8_t TX_Delay = 0;	//Also used in M_NRF24L01.c to check for tx timeout
uint8_t modDelay = 0;

ISR(TIMER0_OVF_vect) {
	USART_UpdateTimers();
}

ISR(TIMER0_COMPA_vect) {
	if (timer10ms) --timer10ms;
	else TCCR0B = 0;
}

ISR(PCINT2_vect) {
	Wakeup_Source = WAKEUP_SOURCE_UART;
}

ISR(TIMER2_OVF_vect) {
	Wakeup_Source = WAKEUP_SOURCE_RTC;
	AddTimes(&RTC_c_Time, &RTC_Period);
	if (TX_Delay) --TX_Delay;
	//NOTE:
	//	Ran for 10 hours WITHOUT xtal capacitors and the clock was out by about a second.
	//	After less than a day of running without caps its ~ 5s out (updated windows time before and after)
}

ISR(TIMER2_COMPA_vect) {

}

int main(void) {

	uint8_t i, j;

	/*//TEST CODE//
	//0 - LED
	DDRC = (1<<0);
	//USART Tx
	DDRD = (1<<1);

	// Enable pullups
	PORTC = 0b00111000;
	PORTD = 0b11111111;
	PRR = ~((1<<PRUSART0)|(1<<PRSPI)|(1<<PRTIM2)); // Turn ON the USART, Timer 2 and SPI ONLY

	//Setup USART for 9600bps
	USART_Setup(UBRRVAL,1);

	RF_Chan = 2;
	for (uint8_t i = 0; i < 5; i++)	RF_ID[i] = 0xE6;
	DevArmed = ARM_DEBUG;

	//Setup the Nordic Chip
	NRF24L01_SPI_Setup();
	NRF24L01_SetupnRF24L01(RF_Chan, RF_ID);
	NRF24L01_SetMode(0,1);

	NRF_TXMODE;

	PORTC |= (1);
	NRF24L01_TX_PAYLOAD(&Device_ID);
	PORTC &= !(1<<0);
	while(1)
		USART_printf_P("...: 0x%02X\r\n", NRF24L01_R_Reg(NREG_STATUS));

	NRF_POWERDOWN;

	_delay_ms(1000);

	set_sleep_mode(SLEEP_MODE_PWR_DOWN);
	sleep_mode();
	//PORTC |= (1);

	while(1);
	//END TEST CODE//*/

	HardwareSetup();

	//USART_tx_String_P(PSTR("Going to sleep. Press any key to wake.\r\n"));
	//USART_tx_String_P(PSTR("Sleep\r\n"));
	_delay_ms(10);
	j = 0;
	srand((int)Device_ID);

	while (1) {

		PCICR = (1<<PCIE2);
		Wakeup_Source = WAKEUP_SOURCE_NONE;

		PORTC &= ~(1<<0);
		// Disable bod while in sleep mode
		sleep_enable();
		sleep_bod_disable();
		sleep_cpu();
		sleep_disable();
		//PORTC |= (1<<0);
		PCICR = 0;

		if (Wakeup_Source == WAKEUP_SOURCE_RTC) {
			if (!TX_Delay) {
				//For a pseudo random delay:
				j = rand() % 5;
				for (i = 0; i < j; i++) {
					CPUSleep_1m5s();
				}
				//Now TX the payload
				TXPayload();
				TX_Delay = TX_Period;
			}

			if (PrintTime) {
				//PORTC |= (1);
				_delay_ms(20);
				sPrintTime(s, &RTC_c_Time, "Time");
				USART_tx_String(s);
				_delay_ms(10);
				//PORTC &= ~(1<<0);
			}
		}

		if (Wakeup_Source == WAKEUP_SOURCE_UART) {
			//PORTC |= (1<<0);
			_delay_ms(20);
			ShowMenu();
			_delay_ms(10);
			//PORTC &= ~(1<<0);
		}

	}

	return 0;
}

void ShowMenu() {
	struct RTCTime tmpTime;
	int i;
	char retChar;
	uint8_t RFedit = 0;

	//USART_tx_String_P(PSTR("USART Woke me.\r\n"));
	//USART_tx_String_P(PSTR("Enter appropriate command.\r\n>"));
	USART_tx_String_P(PSTR(">"));
	//flush USART RX buffer
	while (USART_rx_Byte_nb() != -1);
	//Receive byte
	retChar = USART_rx_Byte(TIMEOUT);
	switch (retChar) {
	case 'T':
		USART_tx_String_P(PSTR("ET (DDMMYYHHMMSS):\r\n"));//Enter Date/Time (DDMMYYHHMMSS).\r\n>"));

		for (i = 0; i < 12; i++)
			s[i] = USART_rx_Byte(TIMEOUT<<2);
		s[12] = '\0';

		if (str2Time(s, &tmpTime) == 0)
			CopyTime(&tmpTime, &RTC_c_Time);

		sPrintTime(s, &RTC_c_Time, "Time");
		USART_tx_String(s);
		break;
	case 'P':
		USART_tx_String_P(PSTR("ETX (MSS):\r\n"));//Enter TX Period (MSS).\r\n>"));

		s[3] = 0;
		for (i = 0; i < 3; i++) {
			s[i] = USART_rx_Byte(TIMEOUT<<1)-'0';
			if (s[i] > 9 || s[i] < 0)
				s[3] = 1;
		}

		if (!s[3]) {
			i = s[0]*60+s[1]*10+s[2];
			if (i > 255) {
				USART_tx_String_P(PSTR("Bad\r\n"));//Invalid TX period entered.\r\n"));
			} else {
				TX_Period = i;
				TX_Delay = i;
				eeprom_write_byte((uint8_t*)TXPERADD,TX_Period);
				//USART_printf_P("TX Period set to %d seconds\r\n", TX_Period);
				USART_printf_P("TX %ds\r\n", TX_Period);
			}
		} else {
			USART_tx_String_P(PSTR("Bad\r\n"));//Invalid TX period entered.\r\n"));
		}
		break;
	case 's':
		TXPayload();
		break;
	case 'N':
		USART_printf_P("Status:0x%02X\r\n", NRF24L01_R_Reg(NREG_STATUS));
		break;
	case 't':
		PrintTime ^= 1;
		//USART_printf_P("PrintTime :0x%02X\r\n", PrintTime);
		break;
	case '?':
		ReadAllVarsFromEEPROM();
		//DEV_ID
		USART_printf_P("DEV_ID:%08lX\r\n", Device_ID);
		//RFID
		USART_tx_String_P(PSTR("RFID:"));
		for (i = 0; i < 4; i++) {
			USART_printf_P("%02X-", RF_ID[i]);
		}
		USART_printf_P("%02X\r\n", RF_ID[4]);
		//RFCHAN
		USART_printf_P("RFCHAN:%d\r\n", RF_Chan);
		//ARMED
		USART_printf_P("DEVARMED:%u\r\n", DevArmed);
		//TXPERIOD
		USART_printf_P("TXPERIOD:%d\r\n", TX_Period);
		break;
	case '*':
		//DEV_ID
		s[0] = 's';
		USART_tx_String_P(PSTR("DEV_ID (XXXXXXXX):\r\n"));
		USART_rx_String(s,TIMEOUT<<3);
		if (s[0] != 's' && s[0] != 0xFF) {
			Device_ID = (uint32_t)strtoul(s, (char **)NULL,16);
			WriteEEPROM((uint8_t*)&Device_ID, 4,DEVIDADD);
		}
		s[0] = 's';
		//RFID
		USART_tx_String_P(PSTR("RFID (XX-XX-XX-XX-XX):\r\n"));
		USART_rx_String(s,TIMEOUT<<3);
		if (s[0] != 's' && s[0] != 0xFF) {
			for (i = 0; i < 5; i++)
				RF_ID[i] = (uint8_t)strtoul(s+i*3, (char **)NULL,16);
			WriteEEPROM(RF_ID,5,RFIDADD);
			RFedit = 1;
		}
		s[0] = 's';
		//RFCHAN
		USART_tx_String_P(PSTR("RFCHAN (###):\r\n"));
		USART_rx_String(s,TIMEOUT<<3);
		if (s[0] != 's' && s[0] != 0xFF) {
			RF_Chan = 100*(s[0]-'0')  + 10*(s[1]-'0') + (s[2]-'0');
			eeprom_write_byte((uint8_t*)RFCHANADD, RF_Chan);
			RFedit = 1;
		}
		s[0] = 's';
		//DEVARMED
		USART_tx_String_P(PSTR("DEVARMED (X):\r\n"));
		USART_rx_String(s,TIMEOUT<<3);
		if (s[0] != 's' && s[0] != 0xFF) {
			DevArmed = s[0]-'0';
			eeprom_write_byte((uint8_t*)DEVARMEDADD,DevArmed);
			DevArmSetup();
		}
		s[0] = 's';
		//TXPERIOD
		USART_tx_String_P(PSTR("TXPERIOD (MSS):\r\n"));
		USART_rx_String(s,TIMEOUT<<3);
		if (s[0] != 's' && s[0] != 0xFF) {
			i = (s[0]-'0')*60+(s[1]-'0')*10+(s[2]-'0');
			//if (i < 255) {
			TX_Period = i;
			TX_Delay = i;
			eeprom_write_byte((uint8_t*)TXPERADD,TX_Period);
			//}
		}

		if (RFedit && DevArmed != ARM_SHELF) {
			//Setup the Nordic Chip with new values
			NRF24L01_SPI_Setup();
			NRF24L01_SetupnRF24L01(RF_Chan, RF_ID);
			NRF_POWERDOWN;
		}
		break;
	}
	//USART_tx_String_P(PSTR("Going to sleep.  Press any key to wake.\r\n"));
	USART_tx_String_P(PSTR("Sleep.\r\n"));

}

/*inline void WriteTimeEEPROM(struct RTCTime* mTime, uint16_t Address) {
	WriteEEPROM((uint8_t*)mTime, 5, Address);
}

inline void ReadTimeEEPROM(struct RTCTime* mTime, uint16_t Address) {
	ReadEEPROM((uint8_t*)mTime, 5, Address);
}*/

void ReadEEPROM(uint8_t* Data, uint8_t Length, uint16_t Address) {
	for (int i = 0; i < Length; i++)
		*Data++ = eeprom_read_byte((uint8_t*)Address++);
}

void WriteEEPROM(uint8_t* Data, uint8_t Length, uint16_t Address) {
	for (int i = 0; i < Length; i++)
		eeprom_write_byte((uint8_t*)Address++, *Data++);
}

static inline void delay_ms(uint16_t ms) {
  do {
    _delay_ms(1);
  } while (--ms != 0);
}

void StartTim10ms(uint16_t Time) {
	PRR &= ~(1<<PRTIM0);
	timer10ms = Time;
	//Setup Timer 0 for 10ms Timeout
	TCCR0A = (1<<WGM01);
	TIMSK0 = (1<<OCIE0A);
	//Output Compare Register A
	OCR0A = T0WAIT10MSOCA;
	TCNT0 = 0;
	//Start the timer - 1024 (From prescaler)
	TCCR0B = (1<<CS02)|(1<<CS00);
	sei();
}

void StopTim10ms() {
	//Stop the timer
	TCCR0B = 0;
	//Clear the timer
	timer10ms = 0;
	PRR |= (1<<PRTIM0);
}

void HardwareSetup() {
	PCMSK2 = (1<<PCINT16); // for RS232/UART

	//0 - LED
	DDRC = (1<<0);
	//USART Tx
	DDRD = (1<<1);

	// Enable pullups
	PORTC = 0b00111001;
	PORTD = 0b11111111;

	ReadAllVarsFromEEPROM();
	TX_Delay = TX_Period;

	DevArmSetup();

	//Enable BOD Sleep (BOD only enabled in active mode)
	//Write both BODS and BODE to 1
	//MCUCR |= (1<<BODS)|(1<<BODSE);
	/*uint8_t tR1, tR2;
	tR1 = MCUCR;
	tR2 = tR1 & (~(1<<BODSE));
	tR1 |= (1<<BODS)|(1<<BODSE);
	MCUCR = tR1;
	//Set BODS to 1 (and BODE must then be set to 0 within 4 clock cycles)
	MCUCR = tR2;
	//MCUCR |= (1<<BODS);
	//MCUCR &= ~(1<<BODSE);*/

	//Setup USART for 9600bps
	USART_Setup(UBRRVAL,1);

	_delay_ms(20);
	USART_tx_String_P(PSTR("Hardware Initialised.\r\n"));

}

void ReadAllVarsFromEEPROM() {
	ReadEEPROM((uint8_t*)&Device_ID, 4,DEVIDADD);
	ReadEEPROM(&RF_Chan, 1, RFCHANADD);
	ReadEEPROM(RF_ID, 5, RFIDADD);
	ReadEEPROM(&DevArmed, 1, DEVARMEDADD);
	ReadEEPROM(&TX_Period, 1, TXPERADD);
}

void DevArmSetup() {
	if (DevArmed != ARM_SHELF) {
		PRR = ~((1<<PRUSART0)|(1<<PRSPI)|(1<<PRTIM2)); // Turn ON the USART, Timer 2 and SPI ONLY

		//Setup the Nordic Chip
		NRF24L01_SPI_Setup();
		NRF24L01_SetupnRF24L01(RF_Chan, RF_ID);
		NRF24L01_SetMode(0,1);

		//enable interrupts
		sei();
		set_sleep_mode(SLEEP_MODE_PWR_SAVE);

		RTC_Setup();
	} else {
		sei();
		PRR = ~((1<<PRUSART0)); // Turn ON the USART ONLY
		set_sleep_mode(SLEEP_MODE_PWR_DOWN);
	}
}

void TXPayload() {

	if (DevArmed == ARM_DEBUG || DevArmed == ARM_ON) {

		//PORTC |= (1);
		NRF_TXMODE;											//28.8us
		//PORTC &= ~(1);

		//PORTC |= (1);
		CPUSleep_1m5s();									//1.68ms
		//PORTC &= ~(1);

		//PORTC |= (1);
		if (DevArmed == ARM_DEBUG)
			USART_tx_String_P( PSTR("TX Packet.\r\n"));
		//PORTC &= ~(1);

		//_delay_ms(20);
		//USART_printf_P("Status:0x%02X\r\n", NRF24L01_R_Reg(NREG_STATUS));
		//USART_printf_P("Config:0x%02X\r\n", NRF24L01_R_Reg(NREG_CONFIG));
		//_delay_ms(10);

		PORTC |= (1);
		//if transmitting times out, setup nordic again
		if (!NRF24L01_TX_PAYLOAD(&Device_ID)) {				//536us
			//Setup the Nordic Chip
			NRF24L01_SPI_Setup();
			NRF24L01_SetupnRF24L01(RF_Chan, RF_ID);
			NRF24L01_SetMode(0,1);
		}
		PORTC &= ~(1);

		//PORTC |= (1);
		NRF_POWERDOWN;										//29.2us
		//PORTC &= ~(1);
	}
}

void CPUSleep_1m5s() {
	//disrupts the RTC!
	//Disable interrupts
	TIMSK2 = 0;
	//Select 32768Hz xtal oscillator as clock.
	ASSR = (1<<AS2);
	//Normal port operation OC2A, OC2B.  CTC Mode.
	TCCR2A = 0b00000010;
	//Normal Mode.  prescalar: 8 (gives ~1.5ms overflow with 32768Hz clock)
	TCCR2B = 0b00000010;
	OCR2A = 0x06;
	//wait for registers to update
	while( ASSR & ((1<<TCN2UB)|(1<<TCR2BUB)|(1<<OCR2AUB)));
	//clear interrupt flag
	TIFR2 = (1<<TOV2);
	//Enable overflow interrupt
	TIMSK2 = (1<<OCIE2A);
	//clear the counter
	TCNT2 = 0;
	sei();

	//now go to sleep.. will wake up in 1.5ms
	sleep_mode();

	//set the RTC up as before with 1s period
	//Disable interrupts
	TIMSK2 = 0;
	//Select 32768Hz xtal oscillator as clock.
	ASSR = (1<<AS2);
	//clear the counter
	TCNT2 = 0;
	//Normal port operation OC2A, OC2B.  Normal Mode.
	TCCR2A = 0b00000000;
	//Normal Mode.  prescalar: 128 (gives 1s overflow with 32768Hz clock)
	TCCR2B = 0b00000101;
	//wait for registers to update
	while( ASSR & ((1<<TCN2UB)|(1<<TCR2BUB)));
	//clear interrupt flag
	TIFR2 = (1<<TOV2);
	//Enable overflow interrupt
	TIMSK2 = (1<<TOIE2);
}
