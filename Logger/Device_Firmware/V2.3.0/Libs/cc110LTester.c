/* 
 * Author: Matthew Cochrane
 * Date: Nov 2015
 * Description: Testing for the cc110L module
 * 
 */

#include "cc110LTester.h"
#include <avr/io.h>
#include "MX_USART.h"
#include "CC110L.h"
#include "WakeupSource.h"

extern uint32_t RTC_sec_ctr;
extern char s[80];

void cc110LTester_Run_ChooseMode()
{
	//60 second timeout, then we just choose slave mode
	uint8_t res = 'S';
	USART_tx_String_P(&USARTC0, PSTR("CHOOSE MODE \r\n\tM for Master\r\n\tS for Slave\r\n"));
	USART_rx_Byte(&USARTC0, 60000, &res);
	if (res == 'M')
		cc110LTester_Run_MasterMode();
	else
		cc110LTester_Run_SlaveMode();
}

void cc110LTester_Run_MasterMode()
{
	uint8_t res;
	uint8_t buf[] = {0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08, 0x09, 0x0A};
	uint32_t packet_id = 1;
	USART_tx_String_P(&USARTC0, PSTR("ENTERED MASTER MODE\r\n"));
	USART_tx_String_P(&USARTC0, PSTR("X to exit.\r\n"));
	cc110L_strobe(TI_CC110L_SIDLE);
	cc110L_strobe(TI_CC110L_SFTX);
	cc110L_writeRFSettings();
	cc110L_strobe(TI_CC110L_SIDLE);
	cc110L_strobe(TI_CC110L_SFTX);
	
	
	while (1)
	{
		_delay_ms(50);
		
		cc110L_tx_packet(&packet_id, 4);
		packet_id++;
		
		if (USART_rx_Byte_nb(&USARTC0, &res) == USART_err_Success)
		{
			if (res == 'X') break;
			if (res == 'U') cc110L_writeRFSettings();
			if (res == '+') {
				//increment frequency offset
				uint8_t foff = cc110L_read_reg(TI_CC110L_FSCTRL0);
				foff++;
				USART_printf_P(&USARTC0, "FOFFSET = 0x%02X\r\n", foff);
				_delay_ms(2);
				cc110L_write_reg(TI_CC110L_FSCTRL0, foff);
			}
			if (res == '-') {
				//decrement frequency offset
				uint8_t foff = cc110L_read_reg(TI_CC110L_FSCTRL0);
				foff--;
				USART_printf_P(&USARTC0, "FOFFSET = 0x%02X\r\n", foff);
				_delay_ms(2);
				cc110L_write_reg(TI_CC110L_FSCTRL0, foff);
			}
		}
	}
	
}

void printBufferHex(uint8_t* buf, uint16_t len) {
	for (uint16_t i = 0; i < len; ++i)
	{
		USART_printf_P(&USARTC0, "%02X ", buf[i]);
	}
	USART_tx_String_P(&USARTC0, PSTR("\r\n"));
}

void handleRx(uint8_t muted, uint32_t* firstPacket, uint32_t* lastPacket, uint32_t* packetsReceived, uint32_t* bytesReceived) {
	uint8_t data[20];
	uint8_t len;
	uint8_t pktMetrics[2];
	int8_t RSSI;
	
	if (!muted)
	{
		uint8_t d = cc110L_read_reg(TI_CC110L_PKTSTATUS);
		USART_printf_P(&USARTC0, "PKTSTATUS: %02X\r\n", d);
		// 		d = cc110L_read_reg(TI_CC110L_MARCSTATE);
		// 		USART_printf_P(&USARTC0, "MARCSTATE: %02X\r\n", d);
		d = cc110L_read_reg(TI_CC110L_RXBYTES);
		USART_printf_P(&USARTC0, "RXBYTES: %02X\r\n", d);
	}
	
	len = 20; //pass in max length of buffer, returns as the len of packet received.
	if (cc110L_receive_packet(data, &len, pktMetrics))
	{
		RSSI = cc110L_AdjustRSSI(pktMetrics[0]);

		if (!muted)
		{
			USART_printf_P(&USARTC0, "RX Packet (RSSI: %d) with length %u.  Data:\r\n", RSSI, len);
			printBufferHex(data, len);
			_delay_ms(2);
		}
		
		//first 4 bytes is packet number
		*lastPacket = *((uint32_t*)data);
		if (*firstPacket == 0)
		{
			*firstPacket = *lastPacket;
		}
		
		*bytesReceived += len;
		*packetsReceived += 1;
	}
	
	cc110L_strobe(TI_CC110L_SRX);
// 	} else {
// 		len = 0;
// 	}
	
	//return len;
}

void print_slave_stats(uint32_t firstPacket, uint32_t lastPacket, uint32_t packetsReceived, uint32_t bytesReceived, uint32_t startTime) 
{
	//Print Stats:
	USART_tx_String_P(&USARTC0, PSTR("Packet Link Stats:\r\n"));
	USART_printf_P(&USARTC0, "Uptime: %lu seconds\r\n", RTC_sec_ctr-startTime);
	USART_printf_P(&USARTC0, "Packets Received: %lu/%lu\r\n", packetsReceived, lastPacket-firstPacket);
	USART_printf_P(&USARTC0, "Bytes Received: %lu\r\n", bytesReceived);
	USART_printf_P(&USARTC0, "Packet Error Rate: %u%%\r\n", 100-(packetsReceived*100/(lastPacket-firstPacket)));
	_delay_ms(2);
}

void cc110LTester_Run_SlaveMode()
{

	uint8_t res;
	uint8_t rtc_wakes = 0;
	uint32_t startTime;
	uint8_t muted = 0;
	uint32_t firstPacket = 0;
	uint32_t lastPacket = 0;
	uint32_t bytesReceived = 0;
	uint32_t packetsReceived = 0;
		
	USART_tx_String_P(&USARTC0, PSTR("ENTERED SLAVE MODE\r\n"));
	cc110L_writeRFSettings();
	cc110L_strobe(TI_CC110L_SFRX);
	Wakeup_Source &= ~(WAKEUP_SOURCE_USART);
	startTime = RTC_sec_ctr;
	
	while (1)
	{
		PORTD.INTFLAGS = (1<<0);
		set_sleep_mode(SLEEP_SMODE_PSAVE_gc);
		MCU_STAT_LED_OFF();
		sleep_mode();
				
		MCU_STAT_LED_ON();
		if (!muted)
		{
			USART_printf_P(&USARTC0, "Wakesrc: %02X\r\n", Wakeup_Source);
			_delay_ms(2);
		}
		
		if (Wakeup_Source & WAKEUP_SOURCE_CC110L) {
			Wakeup_Source &= ~(WAKEUP_SOURCE_CC110L);
			handleRx(muted, &firstPacket, &lastPacket, &packetsReceived, &bytesReceived);
			if (!muted) USART_tx_String_P(&USARTC0, PSTR("From Wakeup_Src\r\n"));
			if (!muted) print_slave_stats(firstPacket, lastPacket, packetsReceived, bytesReceived, startTime);
		}
		
		if (Wakeup_Source & WAKEUP_SOURCE_RTC)
		{
			rtc_wakes++;
			if (rtc_wakes > 20)
			{
				if (!muted) USART_tx_String_P(&USARTC0, PSTR("Start rtc_wakes check\r\n"));
				while ( cc110L_readRXReg_FixErrata( TI_CC110L_RXBYTES ) & TI_CC110L_NUM_RXBYTES_MSK > 0) {
					handleRx(muted, &firstPacket, &lastPacket, &packetsReceived, &bytesReceived);
					if (!muted) USART_tx_String_P(&USARTC0, PSTR("From Wakeup_Src\r\n"));
					if (!muted) print_slave_stats(firstPacket, lastPacket, packetsReceived, bytesReceived, startTime);
					_delay_ms(10);
				}
				if (!muted) USART_tx_String_P(&USARTC0, PSTR("End rtc_wakes check\r\n"));
				if (!muted) USART_tx_String_P(&USARTC0, PSTR("Calibrate\r\n"));
				cc110L_calibrate();
				rtc_wakes = 0;
			}
			uint8_t d = cc110L_read_reg(TI_CC110L_PKTSTATUS);
			USART_printf_P(&USARTC0, "PKTSTATUS: %02X\r\n", d);
			d = cc110L_read_reg(TI_CC110L_MARCSTATE);
			USART_printf_P(&USARTC0, "MARCSTATE: %02X\r\n", d);
			d = cc110L_read_reg(TI_CC110L_RXBYTES);
			USART_printf_P(&USARTC0, "RXBYTES: %02X\r\n", d);
			_delay_ms(2);
		}
	
		if (Wakeup_Source & WAKEUP_SOURCE_USART) 
		{
			Wakeup_Source &= ~(WAKEUP_SOURCE_USART);
			USART_rx_Byte_nb(&USARTC0, &res);  //eat up dummy byte
			if (USART_rx_Byte(&USARTC0, 10000, &res) == USART_err_Success)
			{
				if (res == 'X') break;
				if (res == 'U') cc110L_writeRFSettings();
				if (res == 'M') muted ^= 0xFF;
				if (res == 'S') 
				{
					print_slave_stats(firstPacket, lastPacket, packetsReceived, bytesReceived, startTime);
				}
			}
		}
	}
	
	USART_tx_String_P(&USARTC0, PSTR("EXIT SLAVE MODE\r\n"));
		
}