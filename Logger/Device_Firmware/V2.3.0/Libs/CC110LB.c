/** @file cc2500.c
*
* @brief CC110LB radio functions - This is for the WID tag pickup CC110L not the inter-logger comms CC110L
*
* @author Matthew Cochrane
*/
#include "CC110LB.h"
#include <string.h>
#include <stddef.h>
#include <stdlib.h>
#include <avr/io.h>
#include <util/delay.h>
#include "avr_compiler.h"
#include "SettingsManager.h"
#include "../WIDLogV2.h"
#include "../Strings.h"
#include "TC_driver.h"
#include "ErrorCorrection.h"
extern char s[80];

volatile uint16_t timer10ms = 0;		//!<Used to keep track of how many 10ms blocks have passed since the 10ms timer was started.
uint8_t cc110LB_fixed_packet_len = CC110LB_PACKET_LENGTH;
uint8_t cc110LB_use_fixed_packet_mode = 0;

#define CSN_HIGH()		do {CC11B_CSN_PORT.OUTSET = (1<<CC11B_CSN);} while(0)
#define CSN_LOW()		do {CC11B_CSN_PORT.OUTCLR = (1<<CC11B_CSN);} while (0)

#define CSN_SELECT()	CSN_LOW()
#define CSN_DESELECT()	CSN_HIGH()


// Define positions in buffer for various fields
#define LENGTH_FIELD  (0)
//#define ADDRESS_FIELD (1)
#define DATA_FIELD    (1)

#define DEBUG_CC110LB 1

static uint8_t dummy_callback( uint8_t*, uint8_t );
//uint8_t cc110LB_receive_packet( uint8_t*, uint8_t* );

void cc110LB_spi_setup(void);
void cc110LB_write_reg(uint8_t addr, uint8_t value);
void cc110LB_write_burst_reg(uint8_t addr, uint8_t *buffer, uint8_t count);
uint8_t cc110LB_read_reg(uint8_t addr);
void cc110LB_read_burst_reg(uint8_t addr, uint8_t *buffer, uint8_t count);
uint8_t cc110LB_read_status(uint8_t addr);
void cc110LB_strobe(uint8_t strobe);
void cc110LB_powerup_reset(void);
void cc110LB_SPI_TX(uint8_t Data);
uint8_t cc110LB_SPI_RX();

/*!	\brief Start the 10 millisecond timer on TC0.
 *
 *	The timer counts down from Time to 0 and then stops.
 *	Each tick is 10 milliseconds long.
 *	
 *	\param Time The number to start counting down from
 */
void StartTim10ms(uint16_t Time);
void StopTim10ms();

// Receive buffer
//static uint8_t p_rx_buffer[CC110LB_BUFFER_LENGTH];
static uint8_t p_tx_buffer[CC110LB_BUFFER_LENGTH];

// Holds pointers to all callback functions for CCR registers (and overflow)
static uint8_t (*rx_callback)( uint8_t*, uint8_t ) = dummy_callback;



ISR(TCE0_OVF_vect) {
	//10ms Timer update.
	cli();
	if (timer10ms) timer10ms--;
	else {
		TC0_ConfigClockSource(&TCE0, TC_CLKSEL_OFF_gc);
		TCE0.INTCTRLA = TC_OVFINTLVL_OFF_gc;
	}
	sei();
}

//
// Optimum PATABLE levels according to Table 31 on CC110LB datasheet (for 868MHz)
//
/*static const uint8_t power_table[] = {
							0xc0,	// 12/11dB
							0xC5,	// 10dB
							0xCD,	// 7dB
							0x86,	// 5dB
							0x50,	// 0dB
							0x37,	// -6dB
							0x26,	// -10dB
							0x1D,	// -15dB
							0x17,	// -20dB
							0x03	// -30dB
							};           //  0,   1            dBm
*/

/*******************************************************************************
 * @fn     void setup_radio( uint8_t (*callback)(void) )
 * @brief  Initialize radio and register Rx Callback function
 * ****************************************************************************/
void setup_cc110LB( uint8_t (*callback)(uint8_t*, uint8_t) )
{
	//uint8_t tmpByte = 0x00;
	cli();
	CC_PORT.INTCTRL &= ~(PORT_INT0LVL_gm);		//...
	sei();
	// Set-up rx_callback function
	if (callback != NULL)
		rx_callback = callback;

	cc110LB_spi_setup();							// Initialize SPI port

	cc110LB_powerup_reset();						// Reset CCxxxx

	delay_us(500);							// Wait for device to reset (Not sure why this is needed)

	/* verify that SPI is working */
	#define TEST_VALUE 0xA5
	
	cc110LB_write_reg( TI_CC110LB_PKTLEN, TEST_VALUE );
	if ( cc110LB_read_reg( TI_CC110LB_PKTLEN ) != TEST_VALUE ) { /* SPI is not responding */
		nop();
		while(1);
	}
	
	#define RADIO_PARTNUM        0x00 //?? what should this be?
	#define RADIO_MIN_VERSION    7

	/* verify the correct radio is installed */
	if ( cc110LB_read_reg( TI_CC110LB_PARTNUM ) != RADIO_PARTNUM) {      /* incorrect radio specified */
		while(1);
	}

	if ( cc110LB_read_reg( TI_CC110LB_VERSION ) < RADIO_MIN_VERSION) {  /* obsolete radio specified  */
		while(1);
	}
	
	cc110LB_strobe(TI_CC110LB_SIDLE);
	
	cc110LB_writeRFSettings();                      // Write RF settings to config reg

	cc110LB_strobe(TI_CC110LB_SRX);				// Initialize CCxxxx in RX mode.
	// When a pkt is received, it will
	// signal on GDO0 and wake CPU

	//cc110LB_enable_addressing();
	
	cc110LB_strobe(TI_CC110LB_SIDLE);
	//flush rx fifo
	cc110LB_strobe(TI_CC110LB_SFRX);
	//put into rx
	cc110LB_strobe(TI_CC110LB_SRX);

	// Configure GDO0 port
	PORTCFG.MPCMASK = (1<<CC11B_GD0);
	CC11B_GD0_PORT.PIN0CTRL = PORT_ISC_FALLING_gc;		//PORT_OPC_PULLUP_gc|PORT_ISC_FALLING_gc;
	CC11B_GD0_PORT.DIRCLR = (1<<CC11B_GD0);				//configure pin
	CC11B_GD0_PORT.INTFLAGS = PORT_INT0IF_bm;			//clear interrupt flag
	CC11B_GD0_PORT.INT0MASK = (1<<CC11B_GD0);				//enable interrupt
	CC11B_GD0_PORT.INTCTRL |= PORT_INT0LVL_MED_gc;		//...
}

void cc110LB_calibrate()
{
	cc110LB_strobe(TI_CC110LB_SIDLE);
	cc110LB_strobe(TI_CC110LB_SCAL);
	//flush rx fifo
	cc110LB_strobe(TI_CC110LB_SFRX);
	//put into rx
	cc110LB_strobe(TI_CC110LB_SRX);
}

/*******************************************************************************
 * @fn     cc110LB_tx( uint8_t* p_buffer, uint8_t length )
 * @brief  Send raw message through radio
 * ****************************************************************************/
void cc110LB_tx( uint8_t* p_buffer, uint8_t length )
{
	if (length > CC110LB_BUFFER_LENGTH) return;
	#ifdef DEBUG_CC110LB
		uint8_t d;
	#endif
  	CC11B_GD0_PORT.INT0MASK = (1<<CC11B_GD0);
  	CC11B_GD0_PORT.INTCTRL |= PORT_INT0LVL_OFF_gc;

	#ifdef DEBUG_CC110LB
		d = cc110LB_read_status(TI_CC110LB_PKTLEN);
		USART_printf_P(&USARTC0, "STATUS: %02X\r\n", d);
	#endif
	
	cc110LB_write_burst_reg(TI_CC110LB_TXFIFO, p_buffer, length); // Write TX data
	
	#ifdef DEBUG_CC110LB
		d = cc110LB_read_reg(TI_CC110LB_TXBYTES);
		USART_printf_P(&USARTC0, "TXBYTES: %02X\r\n", d);
	#endif
	
	cc110LB_strobe(TI_CC110LB_STX);				// Change state to TX, initiating
												// data transfer

	StartTim10ms(150);
	while (!(CC11B_GD0_PORT.IN&(1<<CC11B_GD0)) && timer10ms);
											// Wait GDO0 to go hi -> sync TX'ed
	if (!timer10ms) goto finish_tx_err1;

	StartTim10ms(150);							
	while (CC11B_GD0_PORT.IN&(1<<CC11B_GD0) && timer10ms);
											// Wait GDO0 to clear -> end of pkt
	if (!timer10ms) goto finish_tx_err2;
	
	goto finish_tx;

finish_tx_err1:
	#ifdef DEBUG_CC110LB
		USART_tx_String_P(&USARTC0, PSTR("Tx Packet Failed 1!!\r\n"));
		goto finish_tx;
	#endif
finish_tx_err2:
	#ifdef DEBUG_CC110LB
		USART_tx_String_P(&USARTC0, PSTR("Tx Packet Failed 2!!\r\n"));
	#endif
finish_tx:	
	CC11B_GD0_PORT.INTFLAGS = PORT_INT0IF_bm;// After pkt TX, this flag is set.
											// Has to be cleared before existing

	CC11B_GD0_PORT.INT0MASK = (1<<CC11B_GD0);
	CC11B_GD0_PORT.INTCTRL |= PORT_INT0LVL_MED_gc;
	
	if (cc110LB_read_status(TI_CC110LB_PKTLEN) && TI_CC110LB_STAT_STATE_MASK == TI_CC110LB_STAT_STATE_TXFIFOUNDERFLOW)
	{
		#ifdef DEBUG_CC110LB
			USART_tx_String_P(&USARTC0, PSTR("Strobe SFTX!!\r\n"));
		#endif
		cc110LB_strobe(TI_CC110LB_SFTX);
	}
	
	#ifdef DEBUG_CC110LB
		USART_tx_String_P(&USARTC0, PSTR("Tx Packet Finished!!\r\n"));
		_delay_ms(2);
	#endif
}

/*******************************************************************************
 * @fn     void cc110LB_tx_packet( uint8_t* p_buffer, uint8_t length,
 *                                                        uint8_t destination )
 * @brief  Send packet through radio. Takes care of adding length and
 *         destination to packet.
 * ****************************************************************************/
void cc110LB_tx_packet( uint8_t* p_buffer, uint8_t length )
{
	if (length >= CC110LB_BUFFER_LENGTH) return;
	uint16_t encoded_length = 0x00;
	
	//FEC_Encode(p_buffer, length, p_tx_buffer, &encoded_length);
	
	// Add one to packet length account for address byte
	//p_tx_buffer[0] = (uint8_t)encoded_length;
	
	// Insert destination address to buffer
	//p_tx_buffer[ADDRESS_FIELD] = destination;

	// Add one to packet length account for address byte
	p_tx_buffer[LENGTH_FIELD] = length; 
 
	// Copy message buffer into tx buffer. Add one to account for length byte
	memcpy( &p_tx_buffer[DATA_FIELD], p_buffer, length );
 
	// Add DATA_FIELD to account for packet length and address bytes
	cc110LB_tx( p_tx_buffer, length + DATA_FIELD );
}

/*******************************************************************************
 * @fn     cc110LB_set_address( uint8_t );
 * @brief  Set device address
 * ****************************************************************************/
void cc110LB_set_address( uint8_t address )
{
  cc110LB_write_reg( TI_CC110LB_ADDR, address );
}

/*******************************************************************************
 * @fn     cc110LB_set_channel( uint8_t );
 * @brief  Set device channel
 * ****************************************************************************/
void cc110LB_set_channel( uint8_t channel )
{
  cc110LB_write_reg( TI_CC110LB_CHANNR, channel );
}

uint8_t cc110LB_get_RX_FIFO_Status() {
	return cc110LB_readRXReg_FixErrata(TI_CC110LB_RXBYTES) & TI_CC110LB_NUM_RXBYTES_MSK;
}

/*******************************************************************************
 * @fn     cc110LB_set_power( uint8_t );
 * @brief  Set device transmit power. See pg50 of CC110LB revB datasheet.
 * ****************************************************************************/
void cc110LB_set_power( uint8_t power )
{
  // Set TX power
  cc110LB_write_burst_reg(TI_CC110LB_PATABLE, &power, 1 );
}

void cc110LB_set_variable_packet_length() 
{
	cc110LB_use_fixed_packet_mode = 0;
	uint8_t pktctrl0 = cc110LB_read_reg(TI_CC110LB_PKTCTRL0);
	pktctrl0 &= ~(TI_CC110LB_PACKET_LENGTH_CONFIG);
	cc110LB_write_reg(TI_CC110LB_PKTCTRL0, pktctrl0 | TI_CC110LB_VARIABLE_PACKET_LENGTH_MODE);
	cc110LB_write_reg(TI_CC110LB_PKTLEN, 0xFF);
}

void cc110LB_set_fixed_packet_length(uint8_t packet_len)
{
	cc110LB_use_fixed_packet_mode = 1;
	if (packet_len)
		cc110LB_fixed_packet_len = packet_len;
	uint8_t pktctrl0 = cc110LB_read_reg(TI_CC110LB_PKTCTRL0);
	pktctrl0 &= ~(TI_CC110LB_PACKET_LENGTH_CONFIG);
	cc110LB_write_reg(TI_CC110LB_PKTCTRL0, pktctrl0 | TI_CC110LB_FIXED_PACKET_LENGTH_MODE);
	cc110LB_write_reg(TI_CC110LB_PKTLEN, cc110LB_fixed_packet_len);
}

uint8_t cc110LB_packet_length_mode_is_fixed()
{
	return cc110LB_use_fixed_packet_mode;
}
/*******************************************************************************
 * @fn     cc110LB_enable_addressing( );
 * @brief  Enable address checking with 0x00 as a broadcast address
 * ****************************************************************************/
//void cc110LB_enable_addressing()
//{
  //uint8_t tmp_reg;
//
  //tmp_reg = ( cc110LB_read_reg( TI_CC110LB_PKTCTRL1  ) & ~0x03 ) | 0x02;
//
  //cc110LB_write_reg( TI_CC110LB_PKTCTRL1, tmp_reg );
//}

/*******************************************************************************
 * @fn     cc110LB_disable_addressing( );
 * @brief  Disable address checking
 * ****************************************************************************/
//void cc110LB_disable_addressing()
//{
  //uint8_t tmp_reg;
//
  //tmp_reg = ( cc110LB_read_reg( TI_CC110LB_PKTCTRL1  ) & ~0x03 );
//
  //cc110LB_write_reg( TI_CC110LB_PKTCTRL1, tmp_reg );
//}

/*******************************************************************************
 * @fn     cc110LB_sleep( );
 * @brief  Set device to low power sleep mode
 * ****************************************************************************/
void cc110LB_sleep( )
{
	//Disable interrupt!
	CC11B_GD0_PORT.INT0MASK = (1<<CC11B_GD0);
	CC11B_GD0_PORT.INTCTRL |= PORT_INT0LVL_OFF_gc;
	//hmm, this could screw with some other stuff..  Consider just changing the int mask and not disabling the interrupt.
		  
	// Set device to idle
	cc110LB_strobe(TI_CC110LB_SIDLE);

	// Set device to power-down (sleep) mode
	cc110LB_strobe(TI_CC110LB_SPWD);
}

/*******************************************************************************
 * @fn     void dummy_callback( void )
 * @brief  empty function works as default callback
 * ****************************************************************************/
static uint8_t dummy_callback( uint8_t* buffer, uint8_t length )
{
  //__no_operation();
  nop();
  
  return 0;
}

uint8_t cc110LB_readRXReg_FixErrata(uint8_t address) {
	//avoids the errata in the chip by waiting for two identical reads and then agreeing that this is the correct value
	uint8_t readVal[2];
	
	readVal[1] = cc110LB_read_reg(address);
	
	do {
		readVal[0] = readVal[1];
		readVal[1] = cc110LB_read_reg(address);
	} while (readVal[1] != readVal[0]);

	return readVal[1];
}

/*******************************************************************************
 * @fn     uint8_t receive_packet( uint8_t* p_buffer, uint8_t* length )
 * @brief  Receive packet from the radio using CC110LB
 * ****************************************************************************/
//length param is the buffer length as an input and then it is changed to return the length of the packet.
uint8_t cc110LB_receive_packet( uint8_t* p_buffer, uint8_t* length, uint8_t* metrics )
{
	uint8_t status[2];
	uint8_t packet_length_votes[3]; //3 votes for packet length
	uint8_t packet_length; //Actual (voted) length
	uint8_t rxBytes;
	uint16_t* len;
	uint8_t out_buffer_len = *length;
  
	rxBytes = cc110LB_readRXReg_FixErrata( TI_CC110LB_RXBYTES );
  
	//_delay_ms(1000);
  
	*length = (rxBytes & TI_CC110LB_NUM_RXBYTES_MSK);
	// Make sure there are bytes to be read in the FIFO buffer
	if ( ( rxBytes & TI_CC110LB_NUM_RXBYTES_MSK ) >= 1 ) {
			
		if (cc110LB_use_fixed_packet_mode)
		{
			//if ( ( rxBytes & TI_CC110LB_NUM_RXBYTES_MSK ) < (cc110LB_fixed_packet_len+2) ) return 0;
			packet_length = cc110LB_fixed_packet_len;
		}
		else
		{
			// Read the first 3 bytes which contains the packet length -> it's meant to be 3 copies of the same thing, and we vote on the real value.
			packet_length = cc110LB_read_reg( TI_CC110LB_RXFIFO );
			//cc110LB_read_burst_reg( TI_CC110LB_RXFIFO, packet_length_votes, 3 );
			//FEC_Decode_Vote(packet_length_votes, 3, &packet_length, &len);
			//assert(len == 3);
		}
		
		if (packet_length >= CC110LB_BUFFER_LENGTH) goto AbortRxPacketAndFlush;

		StartTim10ms(10); // 1000ms timeout
		//TODO: should be packet_length+2??
		while ((cc110LB_readRXReg_FixErrata( TI_CC110LB_RXBYTES ) < packet_length) && timer10ms);
		
		if (timer10ms == 0) {
			//Timed out
			goto AbortRxPacketAndFlush;
		}
		
		// Read the rest of the packet
		cc110LB_read_burst_reg( TI_CC110LB_RXFIFO, p_buffer, packet_length );
		*length = packet_length;
		// Read two byte status
		cc110LB_read_burst_reg( TI_CC110LB_RXFIFO, status, 2 );
			
		//uint8_t decode_success = FEC_Decode(p_tx_buffer, packet_length, p_buffer, length);
		
		// Make sure the packet length is smaller than our buffer - kinda irrelevant, we already copied the data!
// 		if ( *length <= out_buffer_len )
// 		{
			
			// Append status bytes to metrics (was appending to buffer too, just removed that.
			//memcpy( &p_buffer[packet_length], status, 2 );
			memcpy( metrics, status, 2 );
			return 1;
			
			// Return 1 when CRC matches, 0 otherwise
			//return ( status[TI_CC110LB_LQI_RX] & TI_CC110LB_CRC_OK );
			//check page 30 of cc110LB.pdf - 2 byte status can be appended.  We assume it is here
			//byte 1 contains RSSI
			//byte 2 contains CRC_OK (bit 7 = 1 if CRC is good, 0 if CRC error).
			//return decode_success;//( status[1] & TI_CC110LB_CRC_OK );
// 		} else {
// 			// If the packet is larger than the buffer, flush the RX FIFO
// 			cc110LB_strobe(TI_CC110LB_SFRX);
// 			return 0;
// 		}

	}

	cc110LB_strobe(TI_CC110LB_SRX);
	//TODO: below return did not exist before... I think we want it here though!
	return 0;
	
AbortRxPacketAndFlush:
	*length = packet_length;
	cc110LB_strobe(TI_CC110LB_SIDLE);
	cc110LB_strobe(TI_CC110LB_SFRX);      // Flush RXFIFO
	cc110LB_strobe(TI_CC110LB_SRX);
	return 0;
}

///*******************************************************************************
 //* @fn     uint8_t receive_packet( uint8_t* p_buffer, uint8_t* length )
 //* @brief  Receive packet from the radio using CC110LB
 //* ****************************************************************************/
////length param is the buffer length as an input and then it is changed to return the length of the packet.
//uint8_t cc110LB_receive_packet( uint8_t* p_buffer, uint8_t* length, uint8_t* metrics )
//{
	//uint8_t status[2];
	//uint8_t packet_length_votes[3]; //3 votes for packet length
	//uint8_t packet_length; //Actual (voted) length
	//uint8_t rxBytes;
	//uint16_t* len;
	//uint8_t out_buffer_len = *length;
  //
	//rxBytes = cc110LB_readRXReg_FixErrata( TI_CC110LB_RXBYTES );
  //
	////_delay_ms(1000);
  //
	//*length = (rxBytes & TI_CC110LB_NUM_RXBYTES_MSK);
	//// Make sure there are bytes to be read in the FIFO buffer
	//if ( ( rxBytes & TI_CC110LB_NUM_RXBYTES_MSK ) >= 1 ) {
			//
		//// Read the first 3 bytes which contains the packet length -> it's meant to be 3 copies of the same thing, and we vote on the real value.
		//packet_length = cc110LB_read_reg( TI_CC110LB_RXFIFO );
		////cc110LB_read_burst_reg( TI_CC110LB_RXFIFO, packet_length_votes, 3 );
		////FEC_Decode_Vote(packet_length_votes, 3, &packet_length, &len);
		////assert(len == 3);
		//
		//if (packet_length >= CC110LB_BUFFER_LENGTH) goto AbortRxPacketAndFlush;
//
		//StartTim10ms(100); // 1000ms timeout
		//while ((cc110LB_readRXReg_FixErrata( TI_CC110LB_RXBYTES ) < packet_length) && timer10ms);
		//
		//if (timer10ms == 0) {
			////Timed out
			//goto AbortRxPacketAndFlush;
		//}
		//
		//// Read the rest of the packet
		//cc110LB_read_burst_reg( TI_CC110LB_RXFIFO, p_buffer, packet_length );
		//*length = packet_length;
		//// Read two byte status
		//cc110LB_read_burst_reg( TI_CC110LB_RXFIFO, status, 2 );
			//
		////uint8_t decode_success = FEC_Decode(p_tx_buffer, packet_length, p_buffer, length);
		//
		//// Make sure the packet length is smaller than our buffer - kinda irrelevant, we already copied the data!
//// 		if ( *length <= out_buffer_len )
//// 		{
			//
			//// Append status bytes to metrics (was appending to buffer too, just removed that.
			////memcpy( &p_buffer[packet_length], status, 2 );
			//memcpy( metrics, status, 2 );
			//return 1;
			//
			//// Return 1 when CRC matches, 0 otherwise
			////return ( status[TI_CC110LB_LQI_RX] & TI_CC110LB_CRC_OK );
			////check page 30 of cc110LB.pdf - 2 byte status can be appended.  We assume it is here
			////byte 1 contains RSSI
			////byte 2 contains CRC_OK (bit 7 = 1 if CRC is good, 0 if CRC error).
			////return decode_success;//( status[1] & TI_CC110LB_CRC_OK );
//// 		} else {
//// 			// If the packet is larger than the buffer, flush the RX FIFO
//// 			cc110LB_strobe(TI_CC110LB_SFRX);
//// 			return 0;
//// 		}
//
	//}
//
	////return 0;
	//
//AbortRxPacketAndFlush:
	//*length = packet_length;
	//cc110LB_strobe(TI_CC110LB_SIDLE);
	//cc110LB_strobe(TI_CC110LB_SFRX);      // Flush RXFIFO
	//cc110LB_strobe(TI_CC110LB_SRX);
	//return 0;
//}


void cc110LB_writeRFSettings(void)
{
	// The settings you need to export from TI Smart RF studio:
	//Params to export
	//TI_CC110LB_SYNC1
	//TI_CC110LB_SYNC0
	//TI_CC110LB_PKTLEN
	//TI_CC110LB_PKTCTRL1
	//TI_CC110LB_PKTCTRL0
	//TI_CC110LB_ADDR
	//TI_CC110LB_CHANNR
	//TI_CC110LB_FSCTRL1
	//TI_CC110LB_FSCTRL0
	//TI_CC110LB_FREQ2
	//TI_CC110LB_FREQ1
	//TI_CC110LB_FREQ0
	//TI_CC110LB_MDMCFG4
	//TI_CC110LB_MDMCFG3
	//TI_CC110LB_MDMCFG2
	//TI_CC110LB_MDMCFG1
	//TI_CC110LB_MDMCFG0
	//TI_CC110LB_DEVIATN
	//TI_CC110LB_MCSM2
	//TI_CC110LB_MCSM1
	//TI_CC110LB_MCSM0
	//TI_CC110LB_FOCCFG
	//TI_CC110LB_BSCFG
	//TI_CC110LB_AGCCTRL2
	//TI_CC110LB_AGCCTRL1
	//TI_CC110LB_AGCCTRL0
	//TI_CC110LB_FREND1
	//TI_CC110LB_FREND0
	//TI_CC110LB_FSCAL3
	//TI_CC110LB_FSCAL2
	//TI_CC110LB_FSCAL1
	//TI_CC110LB_FSCAL0
	//TI_CC110LB_TEST2
	//TI_CC110LB_TEST1
	//TI_CC110LB_TEST0
	//initial_power
	
	
	//
	// Rf settings for CC110LB
	//
	uint8_t initial_power = 0xFF;			// 0 dBm
	uint8_t i = 0;
		
	cc110LB_write_reg(TI_CC110LB_IOCFG2,           0x29);
	cc110LB_write_reg(TI_CC110LB_IOCFG1,           0x2E);
	cc110LB_write_reg(TI_CC110LB_IOCFG0,           0x06);
		
	cc110LB_write_reg(TI_CC110LB_SYNC1,            setting_RFPARAMS[i++]);
	cc110LB_write_reg(TI_CC110LB_SYNC0,            setting_RFPARAMS[i++]);
	cc110LB_write_reg(TI_CC110LB_PKTLEN,           setting_RFPARAMS[i++]);
	cc110LB_fixed_packet_len = setting_RFPARAMS[i-1];
	cc110LB_write_reg(TI_CC110LB_PKTCTRL1,         setting_RFPARAMS[i++]);
	cc110LB_write_reg(TI_CC110LB_PKTCTRL0,         setting_RFPARAMS[i++]);
	uint8_t packet_length_mode = setting_RFPARAMS[i-1] & TI_CC110LB_PACKET_LENGTH_CONFIG;
	if (packet_length_mode == TI_CC110LB_VARIABLE_PACKET_LENGTH_MODE)
	{
		cc110LB_use_fixed_packet_mode = 0;
	}
	else if (packet_length_mode == TI_CC110LB_FIXED_PACKET_LENGTH_MODE)
	{
		cc110LB_use_fixed_packet_mode = 1;
	}
	cc110LB_write_reg(TI_CC110LB_ADDR,             setting_RFPARAMS[i++]);
	cc110LB_write_reg(TI_CC110LB_CHANNR,           setting_RFPARAMS[i++]);
	cc110LB_write_reg(TI_CC110LB_FSCTRL1,          setting_RFPARAMS[i++]);
	cc110LB_write_reg(TI_CC110LB_FSCTRL0,          setting_RFPARAMS[i++]);
	cc110LB_write_reg(TI_CC110LB_FREQ2,            setting_RFPARAMS[i++]);
	cc110LB_write_reg(TI_CC110LB_FREQ1,            setting_RFPARAMS[i++]);
	cc110LB_write_reg(TI_CC110LB_FREQ0,            setting_RFPARAMS[i++]);
	cc110LB_write_reg(TI_CC110LB_MDMCFG4,          setting_RFPARAMS[i++]);
	cc110LB_write_reg(TI_CC110LB_MDMCFG3,          setting_RFPARAMS[i++]);
	cc110LB_write_reg(TI_CC110LB_MDMCFG2,          setting_RFPARAMS[i++]);
	cc110LB_write_reg(TI_CC110LB_MDMCFG1,          setting_RFPARAMS[i++]);
	cc110LB_write_reg(TI_CC110LB_MDMCFG0,          setting_RFPARAMS[i++]);
	cc110LB_write_reg(TI_CC110LB_DEVIATN,          setting_RFPARAMS[i++]);
	cc110LB_write_reg(TI_CC110LB_MCSM2,            setting_RFPARAMS[i++]);
	cc110LB_write_reg(TI_CC110LB_MCSM1,            setting_RFPARAMS[i++]);
	cc110LB_write_reg(TI_CC110LB_MCSM0,            setting_RFPARAMS[i++]);
	cc110LB_write_reg(TI_CC110LB_FOCCFG,           setting_RFPARAMS[i++]);
	cc110LB_write_reg(TI_CC110LB_BSCFG,            setting_RFPARAMS[i++]);
	cc110LB_write_reg(TI_CC110LB_AGCCTRL2,         setting_RFPARAMS[i++]);
	cc110LB_write_reg(TI_CC110LB_AGCCTRL1,         setting_RFPARAMS[i++]);
	cc110LB_write_reg(TI_CC110LB_AGCCTRL0,         setting_RFPARAMS[i++]);

	cc110LB_write_reg(TI_CC110LB_FREND1,           setting_RFPARAMS[i++]);
	cc110LB_write_reg(TI_CC110LB_FREND0,           setting_RFPARAMS[i++]);
	cc110LB_write_reg(TI_CC110LB_FSCAL3,           setting_RFPARAMS[i++]);
	cc110LB_write_reg(TI_CC110LB_FSCAL2,           setting_RFPARAMS[i++]);
	cc110LB_write_reg(TI_CC110LB_FSCAL1,           setting_RFPARAMS[i++]);
	cc110LB_write_reg(TI_CC110LB_FSCAL0,           setting_RFPARAMS[i++]);
		
	cc110LB_write_reg(TI_CC110LB_TEST2,            setting_RFPARAMS[i++]);
	cc110LB_write_reg(TI_CC110LB_TEST1,            setting_RFPARAMS[i++]);
	cc110LB_write_reg(TI_CC110LB_TEST0,            setting_RFPARAMS[i++]);
		
	initial_power = setting_RFPARAMS[i++];
	cc110LB_write_reg(TI_CC110LB_IOCFG2, 0x2E); i++;	//High Impedance
	i++;
	cc110LB_write_reg(TI_CC110LB_IOCFG0, 0x06);		//Asserts when packet, deasserts on end of packet
	//cc110LB_write_reg(TI_CC110LB_IOCFG0, 0x00);		//Associated to the RX FIFO: Asserts when RX FIFO is filled at or above the RX FIFO threshold. De-asserts when RX FIFO is drained below the same threshold.
	cc110LB_write_reg(TI_CC110LB_FIFOTHR, 60);//0x01);		//RX FIFO and TX FIFO Thresholds
		
	//uint8_t initial_power = 0xc8;	//7dB
	//cc110LB_write_burst_reg( TI_CC110LB_PATABLE, &initial_power, 1);	//Write PATABLE
	uint8_t pwr = 0xC0; //10 dB
	cc110LB_write_burst_reg( TI_CC110LB_PATABLE, &pwr, 1);	//Write PATABLE
}

/*******************************************************************************
 * @fn void spi_setup(void)
 * @brief Setup SPI with the appropriate settings for CCxxxx communication
 * ****************************************************************************/
void cc110LB_spi_setup(void)
{
	//Outputs
	CC11B_SPI_PORT.DIRSET = (1<<CC11B_SCK)|(1<<CC11B_MOSI);
	CC11B_CSN_PORT.DIRSET = (1<<CC11B_CSN);
	
	//Inputs
	CC11B_GD0_PORT.DIRCLR = (1<<CC11B_GD0);
	CC11B_GD2_PORT.DIRCLR = (1<<CC11B_GD2);
	CC11B_SPI_PORT.DIRCLR = (1<<CC11B_MISO);
	
	// PX4 (CSN) enable pull-up. (TODO[ ]: VERIFY THAT THIS IS HOW YOU DO THIS IN AN XMEGA!!)
	CSN_DESELECT();
	
	//PORTCFG_MPCMASK = (1<<CC_CSN)|(1<<CC_MISO);//|(1<<CC_MOSI);//|(1<<CC_GD0);
	//pull up configuration
	//CC_PORT.PIN0CTRL = 0b00011000;

	CC11B_SPI.CTRL = SPI_ENABLE_bm|SPI_MASTER_bm|SPI_MODE_0_gc|SPI_PRESCALER_DIV16_gc;
}

/*******************************************************************************
 * @fn void cc110LB_write_reg(uint8_t addr, uint8_t value)
 * @brief Write single register value to CCxxxx
 * ****************************************************************************/
void cc110LB_write_reg(uint8_t addr, uint8_t value)
{
	CSN_SELECT();
	//wait for SO pin to go low
	while (CC11B_SPI_PORT.IN & (1<<CC11B_MISO));
	cc110LB_SPI_TX(addr);
	cc110LB_SPI_TX(value);
	CSN_DESELECT();
}

/*******************************************************************************
 * @fn cc110LB_write_burst_reg(uint8_t addr, uint8_t *buffer, uint8_t count)
 * @brief Write multiple values to CCxxxx
 * ****************************************************************************/
void cc110LB_write_burst_reg(uint8_t addr, uint8_t *buffer, uint8_t count)
{
	uint16_t i;
  
  	CSN_SELECT();
	while (CC11B_SPI_PORT.IN & (1<<CC11B_MISO));
  	cc110LB_SPI_TX(addr | TI_CC110LB_WRITE_BURST);
	for (i = 0; i < count; i++) {
  		cc110LB_SPI_TX(buffer[i]);
	}
  	CSN_DESELECT();

}

/*******************************************************************************
 * @fn uint8_t cc110LB_read_reg(uint8_t addr)
 * @brief read single register from CCxxxx
 * ****************************************************************************/
uint8_t cc110LB_read_reg(uint8_t addr)
{
	uint8_t x;
	CSN_SELECT();
	while (CC11B_SPI_PORT.IN & (1<<CC11B_MISO));
	cc110LB_SPI_TX(addr | TI_CC110LB_READ_BURST);//TI_CC110LB_READ_SINGLE);
	x = cc110LB_SPI_RX();
	CSN_DESELECT();

	return x;
}

/*******************************************************************************
 * @fn cc110LB_read_burst_reg(uint8_t addr, uint8_t *buffer, uint8_t count)
 * @brief read multiple registers from CCxxxx
 * ****************************************************************************/
void cc110LB_read_burst_reg(uint8_t addr, uint8_t *buffer, uint8_t count)
{
	uint8_t i;
	
	CSN_SELECT();
	while (CC11B_SPI_PORT.IN & (1<<CC11B_MISO));
	cc110LB_SPI_TX(addr | TI_CC110LB_READ_BURST);
	for (i = 0; i < count; i++)
	{
		buffer[i] = cc110LB_SPI_RX();
	}
	CSN_DESELECT();
}

/*******************************************************************************
 * @fn uint8_t cc110LB_read_status(uint8_t addr)
 * @brief send status command and read returned status byte
 * ****************************************************************************/
uint8_t cc110LB_read_status(uint8_t addr)
{
	uint8_t status;
  
	CSN_SELECT();
	while (CC11B_SPI_PORT.IN & (1<<CC11B_MISO));
	cc110LB_SPI_TX(addr | TI_CC110LB_READ_BURST);
	status = CC11B_SPI.DATA;
	CSN_DESELECT();

	return status;
}

/*******************************************************************************
 * @fn void cc110LB_strobe (uint8_t strobe)
 * @brief send strobe command
 * ****************************************************************************/
void cc110LB_strobe(uint8_t strobe)
{
	CSN_SELECT();
	while (CC11B_SPI_PORT.IN & (1<<CC11B_MISO));
	cc110LB_SPI_TX(strobe);
	CSN_DESELECT();
}
/*******************************************************************************
 * @fn void cc110LB_powerup_reset()
 * @brief reset radio
 * ****************************************************************************/
void cc110LB_powerup_reset(void)
{
	
	CSN_DESELECT();
	delay_us(1);
	CSN_SELECT();
	delay_us(1);
	CSN_DESELECT();
	delay_us(41);
	
	CSN_SELECT();
	while (CC11B_SPI_PORT.IN & (1<<CC11B_MISO));
	cc110LB_SPI_TX(TI_CC110LB_SRES);
	while (CC11B_SPI_PORT.IN & (1<<CC11B_MISO));
	CSN_DESELECT();

}


void cc110LB_SPI_TX(uint8_t Data) {
	CC11B_SPI.DATA = Data;
	while(!(CC11B_SPI.STATUS & (1<<SPI_IF_bp)));
}

uint8_t cc110LB_SPI_RX() {

	CC11B_SPI.DATA = 0x00;
	while(!(CC11B_SPI.STATUS & (1<<SPI_IF_bp)));
	return CC11B_SPI.DATA;
}

int8_t cc110LB_AdjustRSSI( uint8_t RawRSSI )
{
	int8_t RSSI;
	if (RawRSSI >= 128)
		RSSI = (((int16_t)RawRSSI-256)/2)-CC11B_RSSI_OFSET;
	else
		RSSI = (((int16_t)RawRSSI)/2)-CC11B_RSSI_OFSET;
	return RSSI;
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
	TC_SetPeriod(&TCE0, 0x3A97);
	TCE0.CNT = 0;
	TC0_ConfigClockSource(&TCE0, TC_CLKSEL_DIV8_gc);
	TCE0.INTCTRLA = TC_OVFINTLVL_MED_gc;
	sei();
}

/*!	\brief Stop the 10 millisecond timer on TC0
 *
 *	Stops the 10 millisecond timer on TC0 from running and sets
 *	the timer10ms variable to 0.
 */
void StopTim10ms() {
	//Stop the timer
	TC0_ConfigClockSource(&TCE0, TC_CLKSEL_OFF_gc);
	TCE0.INTCTRLA = TC_OVFINTLVL_OFF_gc;
	//Clear the timer
	timer10ms = 0;
}