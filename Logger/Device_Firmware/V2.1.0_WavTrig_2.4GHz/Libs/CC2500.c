/** @file cc2500.c
*
* @brief CC2500 radio functions 
*
* @author Alvaro Prieto
*/
#include "cc2500.h"
#include <string.h>
#include <stddef.h>
#include <stdlib.h>
#include <avr/io.h>
#include <util/delay.h>
#include "avr_compiler.h"
#include "SettingsManager.h"
#include "../WIDLogV2.h"
#include "../Strings.h"
extern char s[80];

#define CSN_HIGH()		do {CC_PORT.OUTSET = (1<<CC_CSN);} while(0)
#define CSN_LOW()		do {CC_PORT.OUTCLR = (1<<CC_CSN);} while (0)

#define CSN_SELECT()	CSN_LOW()
#define CSN_DESELECT()	CSN_HIGH()


// Define positions in buffer for various fields
#define LENGTH_FIELD  (0)
#define ADDRESS_FIELD (1)
#define DATA_FIELD    (2)

static uint8_t dummy_callback( uint8_t*, uint8_t );
//uint8_t cc2500_receive_packet( uint8_t*, uint8_t* );

void spi_setup(void);
void cc_write_reg(uint8_t addr, uint8_t value);
void cc_write_burst_reg(uint8_t addr, uint8_t *buffer, uint8_t count);
uint8_t cc_read_reg(uint8_t addr);
void cc_read_burst_reg(uint8_t addr, uint8_t *buffer, uint8_t count);
uint8_t cc_read_status(uint8_t addr);
void cc_strobe(uint8_t strobe);
void cc_powerup_reset(void);
void CC_SPI_TX(uint8_t Data);
uint8_t CC_SPI_RX();

// Receive buffer
//static uint8_t p_rx_buffer[CC2500_BUFFER_LENGTH];
//static uint8_t p_tx_buffer[CC2500_BUFFER_LENGTH];

// Holds pointers to all callback functions for CCR registers (and overflow)
static uint8_t (*rx_callback)( uint8_t*, uint8_t ) = dummy_callback;

//
// Optimum PATABLE levels according to Table 31 on CC2500 datasheet
//
/*static const uint8_t power_table[] = {
                              0x00, 0x50, 0x44, 0xC0, // -55, -30, -28, -26 dBm
                              0x84, 0x81, 0x46, 0x93, // -24, -22, -20, -18 dBm
                              0x55, 0x8D, 0xC6, 0x97, // -16, -14, -12, -10 dBm
                              0x6E, 0x7F, 0xA9, 0xBB, // -8,  -6,  -4,  -2  dBm
                              0xFE, 0xFF };           //  0,   1            dBm
*/

/*******************************************************************************
 * @fn     void setup_radio( uint8_t (*callback)(void) )
 * @brief  Initialize radio and register Rx Callback function
 * ****************************************************************************/
void setup_cc2500( uint8_t (*callback)(uint8_t*, uint8_t) )
{
	//uint8_t tmpByte = 0x00;
	cli();
	CC_PORT.INTCTRL &= ~(PORT_INT0LVL_gm);		//...
	sei();
	// Set-up rx_callback function
	if (callback != NULL)
		rx_callback = callback;

	spi_setup();							// Initialize SPI port

	cc_powerup_reset();						// Reset CCxxxx

	delay_us(500);							// Wait for device to reset (Not sure why this is needed)

	/* verify that SPI is working */
	#define TEST_VALUE 0xA5
	
	cc_write_reg( TI_CCxxx0_PKTLEN, TEST_VALUE );
	if ( cc_read_reg( TI_CCxxx0_PKTLEN ) != TEST_VALUE ) { /* SPI is not responding */
		nop();
		while(1);
	}
	
	#define RADIO_PARTNUM        0x80
	#define RADIO_MIN_VERSION    3

	/* verify the correct radio is installed */
	if ( cc_read_reg( TI_CCxxx0_PARTNUM ) != RADIO_PARTNUM) {      /* incorrect radio specified */
		while(1);
	}

	if ( cc_read_reg( TI_CCxxx0_VERSION ) < RADIO_MIN_VERSION) {  /* obsolete radio specified  */
		while(1);
	}
	
	cc_strobe(TI_CCxxx0_SIDLE);
	
	writeRFSettings();                      // Write RF settings to config reg
	

	cc_strobe(TI_CCxxx0_SRX);				// Initialize CCxxxx in RX mode.
	// When a pkt is received, it will
	// signal on GDO0 and wake CPU

	// Configure GDO0 port
	PORTCFG.MPCMASK = (1<<CC_GD0);
	CC_PORT.PIN0CTRL = PORT_ISC_FALLING_gc;		//PORT_OPC_PULLUP_gc|PORT_ISC_FALLING_gc;
	CC_PORT.DIRCLR = (1<<CC_GD0);				//configure pin
	CC_PORT.INTFLAGS = (1<<0);					//clear interrupt flag
	CC_PORT.INT0MASK = (1<<CC_GD0);				//enable interrupt
	CC_PORT.INTCTRL |= PORT_INT0LVL_MED_gc;		//...
	

}

/*******************************************************************************
 * @fn     cc2500_tx( uint8_t* p_buffer, uint8_t length )
 * @brief  Send raw message through radio
 * ****************************************************************************/
// void cc2500_tx( uint8_t* p_buffer, uint8_t length )
// {
//   	CC_PORT.INT0MASK = (1<<CC_GD0);
//   	CC_PORT.INTCTRL |= PORT_INT0LVL_OFF_gc;
// 
// 	cc_write_burst_reg(TI_CCxxx0_TXFIFO, p_buffer, length); // Write TX data
// 	cc_strobe(TI_CCxxx0_STX);				// Change state to TX, initiating
// 											// data transfer
// 
// 	while (!(CC_PORT.IN&CC_GD0));
// 											// Wait GDO0 to go hi -> sync TX'ed
// 	while (CC_PORT.IN&CC_GD0);
// 											// Wait GDO0 to clear -> end of pkt
// 	CC_PORT.INTFLAGS = (1<<0);			// After pkt TX, this flag is set.
// 											// Has to be cleared before existing
// 
// 	CC_PORT.INT0MASK = (1<<CC_GD0);
// 	CC_PORT.INTCTRL |= PORT_INT0LVL_MED_gc;
// }

/*******************************************************************************
 * @fn     void cc2500_tx_packet( uint8_t* p_buffer, uint8_t length,
 *                                                        uint8_t destination )
 * @brief  Send packet through radio. Takes care of adding length and
 *         destination to packet.
 * ****************************************************************************/
// void cc2500_tx_packet( uint8_t* p_buffer, uint8_t length, uint8_t destination )
// {
//   // Add one to packet length account for address byte
//   p_tx_buffer[LENGTH_FIELD] = length + 1;
// 
//   // Insert destination address to buffer
//   p_tx_buffer[ADDRESS_FIELD] = destination;
// 
//   // Copy message buffer into tx buffer. Add one to account for length byte
//   memcpy( &p_tx_buffer[DATA_FIELD], p_buffer, length );
// 
//   // Add DATA_FIELD to account for packet length and address bytes
//   cc2500_tx( p_tx_buffer, (length + DATA_FIELD) );
// }

// /*******************************************************************************
//  * @fn     cc2500_set_address( uint8_t );
//  * @brief  Set device address
//  * ****************************************************************************/
// void cc2500_set_address( uint8_t address )
// {
//   cc_write_reg( TI_CCxxx0_ADDR, address );
// }
// 
// /*******************************************************************************
//  * @fn     cc2500_set_channel( uint8_t );
//  * @brief  Set device channel
//  * ****************************************************************************/
// void cc2500_set_channel( uint8_t channel )
// {
//   cc_write_reg( TI_CCxxx0_CHANNR, channel );
// }

uint8_t cc2500_get_RX_FIFO_Status() {
	return readRXReg_FixErrata(TI_CCxxx0_RXBYTES) & TI_CCxxx0_NUM_RXBYTES;
}

/*******************************************************************************
 * @fn     cc2500_set_power( uint8_t );
 * @brief  Set device transmit power
 * ****************************************************************************/
void cc2500_set_power( uint8_t power )
{
  // Set TX power
  cc_write_burst_reg(TI_CCxxx0_PATABLE, &power, 1 );
}

// /*******************************************************************************
//  * @fn     cc2500_enable_addressing( );
//  * @brief  Enable address checking with 0x00 as a broadcast address
//  * ****************************************************************************/
// void cc2500_enable_addressing()
// {
//   uint8_t tmp_reg;
// 
//   tmp_reg = ( cc_read_reg( TI_CCxxx0_PKTCTRL1  ) & ~0x03 ) | 0x02;
// 
//   cc_write_reg( TI_CCxxx0_PKTCTRL1, tmp_reg );
// }
// 
// /*******************************************************************************
//  * @fn     cc2500_disable_addressing( );
//  * @brief  Disable address checking
//  * ****************************************************************************/
// void cc2500_disable_addressing()
// {
//   uint8_t tmp_reg;
// 
//   tmp_reg = ( cc_read_reg( TI_CCxxx0_PKTCTRL1  ) & ~0x03 );
// 
//   cc_write_reg( TI_CCxxx0_PKTCTRL1, tmp_reg );
// }

/*******************************************************************************
 * @fn     cc2500_sleep( );
 * @brief  Set device to low power sleep mode
 * ****************************************************************************/
void cc2500_sleep( )
{
	
	//Disable interrupt!
	CC_PORT.INT0MASK = (1<<CC_GD0);
	CC_PORT.INTCTRL |= PORT_INT0LVL_OFF_gc;
		  
	// Set device to idle
	cc_strobe(TI_CCxxx0_SIDLE);

	// Set device to power-down (sleep) mode
	cc_strobe(TI_CCxxx0_SPWD);
  
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

uint8_t readRXReg_FixErrata(uint8_t address) {
	//avoids the errata in the chip by waiting for two identical reads and then agreeing that this is the correct value
	uint8_t readVal[2];
	
	readVal[1] = cc_read_reg(address);
	
	do {
		readVal[0] = readVal[1];
		readVal[1] = cc_read_reg(address);
	} while (readVal[1] != readVal[0]);

	return readVal[1];
}

/*******************************************************************************
 * @fn     uint8_t receive_packet( uint8_t* p_buffer, uint8_t* length )
 * @brief  Receive packet from the radio using CC2500
 * ****************************************************************************/
//length param is the buffer length as an input and then it is changed to return the length of the packet.
uint8_t cc2500_receive_packet( uint8_t* p_buffer, uint8_t* length, uint8_t* metrics )
{
	uint8_t status[2];
	uint8_t packet_length;
	uint8_t rxBytes;
  
	rxBytes = readRXReg_FixErrata( TI_CCxxx0_RXBYTES );
  
	//if (rxBytes & TI_CCxxx0_OVF_RXBYTES) {
	//	nop();
	//}
  
	//sprintf_P(s, PSTR("%02X: "), rxBytes);
	//USART_tx_String(&USARTC0, s);

	// Make sure there are bytes to be read in the FIFO buffer
	if ( ( rxBytes & TI_CCxxx0_NUM_RXBYTES ) >= (CC_PKT_LEN+2) ) {
		// Read the first byte which contains the packet length
		//packet_length = cc_read_reg( TI_CCxxx0_RXFIFO );
		packet_length = CC_PKT_LEN;


		// Make sure the packet length is smaller than our buffer
		if ( packet_length <= *length )
		{
			// Read the rest of the packet
			cc_read_burst_reg( TI_CCxxx0_RXFIFO, p_buffer, packet_length );

			// Return packet size in length variable
			*length = packet_length;

			// Read two byte status
			cc_read_burst_reg( TI_CCxxx0_RXFIFO, status, 2 );

			// Append status bytes to buffer
			memcpy( &p_buffer[packet_length], status, 2 );
			memcpy( metrics, status, 2 );
	  
			//if (p_buffer[0] == 0xCC && p_buffer[1] == 0xCC) { // && p_buffer[1] == 0xCC) {
			//nop();
			//	if (rxBytes > 1000) {
			//		nop();
			//	}			  
			//}

			// Return 1 when CRC matches, 0 otherwise
			return ( status[TI_CCxxx0_LQI_RX] & TI_CCxxx0_CRC_OK );
		} else {
			// If the packet is larger than the buffer, flush the RX FIFO
			*length = packet_length;

			// Flush RX FIFO
			cc_strobe(TI_CCxxx0_SFRX);      // Flush RXFIFO

			return 0;
		}

	}

	return 0;
}

// Sync word qualifier mode = 30/32 sync word bits detected
// Channel spacing = 199.951172
// Data rate = 2.39897
// RX filter BW = 203.125000
// Preamble count = 4
// Whitening = false
// Address config = No address check
// Carrier frequency = 2432.999908
// Device address = 0
// TX power = 0
// Manchester enable = true
// CRC enable = true
// Deviation = 38.085938
// Packet length mode = Fixed packet length mode. Length configured in PKTLEN register
// Packet length = 6
// Modulation format = 2-FSK
// Base frequency = 2432.999908
// Modulated = true
// Channel number = 0
void writeRFSettings(void)
{
	//
	// Rf settings for CC2510
	//
	uint8_t initial_power = 0xFF;			// 0 dBm
	uint8_t i = 0;
	
	cc_write_reg(TI_CCxxx0_SYNC1,setting_RFPARAMS[i++]);    //Sync Word, High Byte
	cc_write_reg(TI_CCxxx0_SYNC0,setting_RFPARAMS[i++]);    //Sync Word, Low Byte
	cc_write_reg(TI_CCxxx0_PKTLEN,setting_RFPARAMS[i++]);   //Packet Length
	cc_write_reg(TI_CCxxx0_PKTCTRL1,setting_RFPARAMS[i++]|0x08); //Packet Automation Control
	cc_write_reg(TI_CCxxx0_PKTCTRL0,setting_RFPARAMS[i++]); //Packet Automation Control
	cc_write_reg(TI_CCxxx0_ADDR,setting_RFPARAMS[i++]);     //Device Address
	cc_write_reg(TI_CCxxx0_CHANNR,setting_RFPARAMS[i++]);   //Channel Number
	cc_write_reg(TI_CCxxx0_FSCTRL1,setting_RFPARAMS[i++]);  //Frequency Synthesizer Control
	cc_write_reg(TI_CCxxx0_FSCTRL0,setting_RFPARAMS[i++]);  //Frequency Synthesizer Control
	cc_write_reg(TI_CCxxx0_FREQ2,setting_RFPARAMS[i++]);    //Frequency Control Word, High Byte
	cc_write_reg(TI_CCxxx0_FREQ1,setting_RFPARAMS[i++]);    //Frequency Control Word, Middle Byte
	cc_write_reg(TI_CCxxx0_FREQ0,setting_RFPARAMS[i++]);    //Frequency Control Word, Low Byte
	cc_write_reg(TI_CCxxx0_MDMCFG4,setting_RFPARAMS[i++]);  //Modem configuration
	cc_write_reg(TI_CCxxx0_MDMCFG3,setting_RFPARAMS[i++]);  //Modem Configuration
	cc_write_reg(TI_CCxxx0_MDMCFG2,setting_RFPARAMS[i++]);  //Modem Configuration
	cc_write_reg(TI_CCxxx0_MDMCFG1,setting_RFPARAMS[i++]);  //Modem Configuration
	cc_write_reg(TI_CCxxx0_MDMCFG0,setting_RFPARAMS[i++]);  //Modem Configuration
	cc_write_reg(TI_CCxxx0_DEVIATN,setting_RFPARAMS[i++]);  //Modem Deviation Setting
	cc_write_reg(TI_CCxxx0_MCSM2,setting_RFPARAMS[i++]);    //Main Radio Control State Machine Configuration
	cc_write_reg(TI_CCxxx0_MCSM1,setting_RFPARAMS[i++]|0x0C);    //Main Radio Control State Machine Configuration
	cc_write_reg(TI_CCxxx0_MCSM0,setting_RFPARAMS[i++]);    //Main Radio Control State Machine Configuration
	cc_write_reg(TI_CCxxx0_FOCCFG,setting_RFPARAMS[i++]);   //Frequency Offset Compensation Configuration
	cc_write_reg(TI_CCxxx0_BSCFG,setting_RFPARAMS[i++]);    //Bit Synchronization Configuration
	cc_write_reg(TI_CCxxx0_AGCCTRL2,setting_RFPARAMS[i++]); //AGC Control
	cc_write_reg(TI_CCxxx0_AGCCTRL1,setting_RFPARAMS[i++]); //AGC Control
	cc_write_reg(TI_CCxxx0_AGCCTRL0,setting_RFPARAMS[i++]); //AGC Control
	cc_write_reg(TI_CCxxx0_FREND1,setting_RFPARAMS[i++]);   //Front End RX Configuration
	cc_write_reg(TI_CCxxx0_FREND0,setting_RFPARAMS[i++]);   //Front End TX Configuration
	cc_write_reg(TI_CCxxx0_FSCAL3,setting_RFPARAMS[i++]);   //Frequency Synthesizer Calibration
	cc_write_reg(TI_CCxxx0_FSCAL2,setting_RFPARAMS[i++]);   //Frequency Synthesizer Calibration
	cc_write_reg(TI_CCxxx0_FSCAL1,setting_RFPARAMS[i++]);   //Frequency Synthesizer Calibration
	cc_write_reg(TI_CCxxx0_FSCAL0,setting_RFPARAMS[i++]);   //Frequency Synthesizer Calibration
	cc_write_reg(TI_CCxxx0_TEST2,setting_RFPARAMS[i++]);    //Various Test Settings
	cc_write_reg(TI_CCxxx0_TEST1,setting_RFPARAMS[i++]);    //Various Test Settings
	cc_write_reg(TI_CCxxx0_TEST0,setting_RFPARAMS[i++]);    //Various Test Settings
	initial_power = setting_RFPARAMS[i++];
	cc_write_reg(TI_CCxxx0_IOCFG2,0x2E); i++;				//Radio Test Signal Configuration (P1_7)
	//cc_write_reg(TI_CCxxx0_IOCFG1,setting_RFPARAMS[i++]); //Radio Test Signal Configuration (P1_6)
	i++;
	cc_write_reg(TI_CCxxx0_IOCFG0, 0x06);//0x00); i++;//0x40);		//Associated to the RX FIFO: Asserts when RX FIFO is filled at or above the RX FIFO threshold. De-asserts when RX FIFO is drained below the same threshold.
	cc_write_reg(TI_CCxxx0_FIFOTHR, 0x01);//RX FIFO and TX FIFO Thresholds
	cc_write_burst_reg( TI_CCxxx0_PATABLE, &initial_power, 1);//Write PATABLE
		  
// 	cc_write_reg(TI_CCxxx0_PKTLEN,setting_RFPARAMS[i++]);  //Packet Length
// 	cc_write_reg(TI_CCxxx0_PKTCTRL1,setting_RFPARAMS[i++]);//Packet Automation Control
// 	cc_write_reg(TI_CCxxx0_PKTCTRL0,setting_RFPARAMS[i++]);//Packet Automation Control
// 	cc_write_reg(TI_CCxxx0_ADDR,setting_RFPARAMS[i++]);    //Device Address
// 	cc_write_reg(TI_CCxxx0_CHANNR,setting_RFPARAMS[i++]);  //Channel Number
// 	cc_write_reg(TI_CCxxx0_FSCTRL1,setting_RFPARAMS[i++]); //Frequency Synthesizer Control
// 	cc_write_reg(TI_CCxxx0_FSCTRL0,setting_RFPARAMS[i++]); //Frequency Synthesizer Control
// 	cc_write_reg(TI_CCxxx0_FREQ2,setting_RFPARAMS[i++]);   //Frequency Control Word, High Byte
// 	cc_write_reg(TI_CCxxx0_FREQ1,setting_RFPARAMS[i++]);   //Frequency Control Word, Middle Byte
// 	cc_write_reg(TI_CCxxx0_FREQ0,setting_RFPARAMS[i++]);   //Frequency Control Word, Low Byte
// 	cc_write_reg(TI_CCxxx0_MDMCFG4,setting_RFPARAMS[i++]); //Modem configuration
// 	cc_write_reg(TI_CCxxx0_MDMCFG3,setting_RFPARAMS[i++]); //Modem Configuration
// 	cc_write_reg(TI_CCxxx0_MDMCFG2,setting_RFPARAMS[i++]);//0x0B); //Modem Configuration
// 	cc_write_reg(TI_CCxxx0_MDMCFG1,setting_RFPARAMS[i++]); //Modem Configuration
// 	cc_write_reg(TI_CCxxx0_MDMCFG0,setting_RFPARAMS[i++]); //Modem Configuration
// 	cc_write_reg(TI_CCxxx0_DEVIATN,setting_RFPARAMS[i++]); //Modem Deviation Setting
// 	cc_write_reg(TI_CCxxx0_MCSM2,setting_RFPARAMS[i++]);   //Main Radio Control State Machine Configuration
// 	cc_write_reg(TI_CCxxx0_MCSM1,setting_RFPARAMS[i++]);   //Main Radio Control State Machine Configuration
// 	cc_write_reg(TI_CCxxx0_MCSM0,0x14);   //Main Radio Control State Machine Configuration
// 	cc_write_reg(TI_CCxxx0_FOCCFG,0x16);  //Frequency Offset Compensation Configuration
// 	cc_write_reg(TI_CCxxx0_BSCFG,0x6C);   //Bit Synchronization Configuration
// 	cc_write_reg(TI_CCxxx0_AGCCTRL2,0x03);//AGC Control
// 	cc_write_reg(TI_CCxxx0_AGCCTRL1,0x40);//AGC Control
// 	cc_write_reg(TI_CCxxx0_AGCCTRL0,0x91);//AGC Control
// 	cc_write_reg(TI_CCxxx0_FREND1,0x56);  //Front End RX Configuration
// 	cc_write_reg(TI_CCxxx0_FREND0,0x10);  //Front End TX Configuration
// 	cc_write_reg(TI_CCxxx0_FSCAL3,0xA9);  //Frequency Synthesizer Calibration
// 	cc_write_reg(TI_CCxxx0_FSCAL2,0x0A);  //Frequency Synthesizer Calibration
// 	cc_write_reg(TI_CCxxx0_FSCAL1,0x00);  //Frequency Synthesizer Calibration
// 	cc_write_reg(TI_CCxxx0_FSCAL0,0x11);  //Frequency Synthesizer Calibration
// 	cc_write_reg(TI_CCxxx0_TEST2,0x88);   //Various Test Settings
// 	cc_write_reg(TI_CCxxx0_TEST1,0x31);   //Various Test Settings
// 	cc_write_reg(TI_CCxxx0_TEST0,0x09);   //Various Test Settings
// 	cc_write_reg(TI_CCxxx0_IOCFG2,0x2E);  //Radio Test Signal Configuration (P1_7)
// 	cc_write_reg(TI_CCxxx0_IOCFG0,0x00);//0x40);  //Associated to the RX FIFO: Asserts when RX FIFO is filled at or above the RX FIFO threshold. De-asserts when RX FIFO is drained below the same threshold.
// 	cc_write_reg(TI_CCxxx0_FIFOTHR, 0x00);//RX FIFO and TX FIFO Thresholds
// 	cc_write_burst_reg( TI_CCxxx0_PATABLE, &initial_power, 1);//Write PATABLE
}
// Sync word qualifier mode = 30/32 sync word bits detected
// Channel spacing = 199.951172
// Data rate = 249.939
// RX filter BW = 541.666667
// Preamble count = 4
// Whitening = false
// Address config = No address check
// Carrier frequency = 2432.999908
// Device address = 0
// TX power = 0
// Manchester enable = false
// CRC enable = true
// Phase transition time = 0
// Packet length mode = Fixed packet length mode. Length configured in PKTLEN register
// Packet length = 6
// Modulation format = MSK
// Base frequency = 2432.999908
// Modulated = true
// Channel number = 0
// GDO0 signal selection = ( 0x00 ) Asserts when sync word has been sent / received, and de-asserts at the end of the packet
// GDO2 signal selection = ( 0x2E ) High impedance (3-state)
// void writeRFSettings(void)
// {
// 	//
// 	// Rf settings for CC2510
// 	//
// 	cc_write_reg(TI_CCxxx0_PKTLEN,0x06);  //Packet Length
// 	cc_read_reg(TI_CCxxx0_PKTLEN);
// 	cc_write_reg(TI_CCxxx0_PKTCTRL1,0x04);//Packet Automation Control
// 	cc_write_reg(TI_CCxxx0_PKTCTRL0,0x04);//Packet Automation Control
// 	cc_write_reg(TI_CCxxx0_ADDR,0x00);    //Device Address
// 	cc_write_reg(TI_CCxxx0_CHANNR,0x00);  //Channel Number
// 	cc_write_reg(TI_CCxxx0_FSCTRL1,0x0A); //Frequency Synthesizer Control
// 	cc_write_reg(TI_CCxxx0_FSCTRL0,0x00); //Frequency Synthesizer Control
// 	cc_write_reg(TI_CCxxx0_FREQ2,0x5D);   //Frequency Control Word, High Byte
// 	cc_write_reg(TI_CCxxx0_FREQ1,0x93);   //Frequency Control Word, Middle Byte
// 	cc_write_reg(TI_CCxxx0_FREQ0,0xB1);   //Frequency Control Word, Low Byte
// 	cc_write_reg(TI_CCxxx0_MDMCFG4,0x2D); //Modem configuration
// 	cc_write_reg(TI_CCxxx0_MDMCFG3,0x3B); //Modem Configuration
// 	cc_write_reg(TI_CCxxx0_MDMCFG2,0x73); //Modem Configuration
// 	cc_write_reg(TI_CCxxx0_MDMCFG1,0x22); //Modem Configuration
// 	cc_write_reg(TI_CCxxx0_MDMCFG0,0xF8); //Modem Configuration
// 	cc_write_reg(TI_CCxxx0_DEVIATN,0x00); //Modem Deviation Setting
// 	cc_write_reg(TI_CCxxx0_MCSM2,0x07);   //Main Radio Control State Machine Configuration
// 	cc_write_reg(TI_CCxxx0_MCSM1,0x30);   //Main Radio Control State Machine Configuration
// 	cc_write_reg(TI_CCxxx0_MCSM0,0x14);   //Main Radio Control State Machine Configuration
// 	cc_write_reg(TI_CCxxx0_FOCCFG,0x1D);  //Frequency Offset Compensation Configuration
// 	cc_write_reg(TI_CCxxx0_BSCFG,0x1C);   //Bit Synchronization Configuration
// 	cc_write_reg(TI_CCxxx0_AGCCTRL2,0xC7);//AGC Control
// 	cc_write_reg(TI_CCxxx0_AGCCTRL1,0x00);//AGC Control
// 	cc_write_reg(TI_CCxxx0_AGCCTRL0,0xB2);//AGC Control
// 	cc_write_reg(TI_CCxxx0_FREND1,0xB6);  //Front End RX Configuration
// 	cc_write_reg(TI_CCxxx0_FREND0,0x10);  //Front End TX Configuration
// 	cc_write_reg(TI_CCxxx0_FSCAL3,0xEA);  //Frequency Synthesizer Calibration
// 	cc_write_reg(TI_CCxxx0_FSCAL2,0x0A);  //Frequency Synthesizer Calibration
// 	cc_write_reg(TI_CCxxx0_FSCAL1,0x00);  //Frequency Synthesizer Calibration
// 	cc_write_reg(TI_CCxxx0_FSCAL0,0x11);  //Frequency Synthesizer Calibration
// 	cc_write_reg(TI_CCxxx0_TEST2,0x88);   //Various Test Settings
// 	cc_write_reg(TI_CCxxx0_TEST1,0x31);   //Various Test Settings
// 	cc_write_reg(TI_CCxxx0_TEST0,0x09);   //Various Test Settings
// 	cc_write_reg(TI_CCxxx0_IOCFG2,0x2E);  //Radio Test Signal Configuration (P1_7)
// 	cc_write_reg(TI_CCxxx0_IOCFG0,0x00);//0x40);  //Associated to the RX FIFO: Asserts when RX FIFO is filled at or above the RX FIFO threshold. De-asserts when RX FIFO is drained below the same threshold.
// 	cc_write_reg(TI_CCxxx0_FIFOTHR, 0x00);//RX FIFO and TX FIFO Thresholds
// }

// Product = CC2500
// Crystal accuracy = 40 ppm
// X-tal frequency = 26 MHz
// RF output power = 0 dBm
// RX filterbandwidth = 540.000000 kHz
// Deviation = 0.000000
// Return state:  Return to RX state upon leaving either TX or RX
// Datarate = 250.000000 kbps
// Modulation = (7) MSK
// Manchester enable = (0) Manchester disabled
// RF Frequency = 2433.000000 MHz
// Channel spacing = 199.950000 kHz
// Channel number = 0
// Optimization = Sensitivity
// Sync mode = (3) 30/32 sync word bits detected
// Format of RX/TX data = (0) Normal mode, use FIFOs for RX and TX
// CRC operation = (1) CRC calculation in TX and CRC check in RX enabled
// Forward Error Correction = (0) FEC disabled
// Length configuration = (1) Variable length packets, packet length configured by the first received byte after sync word.
// Packetlength = 255
// Preamble count = (2)  4 bytes
// Append status = 1
// Address check = Address check and 0 (0x00) broadcast
// CRC autoflush = true
// Device address = 1
// GDO0 signal selection = ( 0x06 ) Asserts when sync word has been sent / received, and de-asserts at the end of the packet
// GDO2 signal selection = ( 0x0E ) Carrier sense. High if RSSI level is above threshold.
// void writeRFSettings(void)
// {
// 
// 	  // Write register settings
// 	  cc_write_reg(TI_CCxxx0_IOCFG2,   0x0E);  // GDO2 output pin config.
// 	  cc_write_reg(TI_CCxxx0_IOCFG0,   0x06);  // GDO0 output pin config.
// 	  cc_write_reg(TI_CCxxx0_PKTLEN,   0x3D);  // Packet length.
// 	  cc_write_reg(TI_CCxxx0_PKTCTRL1, 0x0E);  // Packet automation control.
// 	  cc_write_reg(TI_CCxxx0_PKTCTRL0, 0x05);  // Packet automation control.
// 	  cc_write_reg(TI_CCxxx0_ADDR,     0x01);  // Device address.
// 	  cc_write_reg(TI_CCxxx0_CHANNR,   0x00); // Channel number.
// 	  cc_write_reg(TI_CCxxx0_FSCTRL1,  0x07); // Freq synthesizer control.
// 	  cc_write_reg(TI_CCxxx0_FSCTRL0,  0x00); // Freq synthesizer control.
// 	  cc_write_reg(TI_CCxxx0_FREQ2,    0x5D); // Freq control word, high byte
// 	  cc_write_reg(TI_CCxxx0_FREQ1,    0x93); // Freq control word, mid byte.
// 	  cc_write_reg(TI_CCxxx0_FREQ0,    0xB1); // Freq control word, low byte.
// 	  cc_write_reg(TI_CCxxx0_MDMCFG4,  0x2D); // Modem configuration.
// 	  cc_write_reg(TI_CCxxx0_MDMCFG3,  0x3B); // Modem configuration.
// 	  cc_write_reg(TI_CCxxx0_MDMCFG2,  0x73); // Modem configuration.
// 	  cc_write_reg(TI_CCxxx0_MDMCFG1,  0x22); // Modem configuration.
// 	  cc_write_reg(TI_CCxxx0_MDMCFG0,  0xF8); // Modem configuration.
// 	  cc_write_reg(TI_CCxxx0_DEVIATN,  0x00); // Modem dev (when FSK mod en)
// 	  cc_write_reg(TI_CCxxx0_MCSM1 ,   0x2F); //MainRadio Cntrl State Machine
// 	  cc_write_reg(TI_CCxxx0_MCSM0 ,   0x18); //MainRadio Cntrl State Machine
// 	  cc_write_reg(TI_CCxxx0_FOCCFG,   0x1D); // Freq Offset Compens. Config
// 	  cc_write_reg(TI_CCxxx0_BSCFG,    0x1C); //  Bit synchronization config.
// 	  cc_write_reg(TI_CCxxx0_AGCCTRL2, 0xC7); // AGC control.
// 	  cc_write_reg(TI_CCxxx0_AGCCTRL1, 0x00); // AGC control.
// 	  cc_write_reg(TI_CCxxx0_AGCCTRL0, 0xB2); // AGC control.
// 	  cc_write_reg(TI_CCxxx0_FREND1,   0xB6); // Front end RX configuration.
// 	  cc_write_reg(TI_CCxxx0_FREND0,   0x10); // Front end RX configuration.
// 	  cc_write_reg(TI_CCxxx0_FSCAL3,   0xEA); // Frequency synthesizer cal.
// 	  cc_write_reg(TI_CCxxx0_FSCAL2,   0x0A); // Frequency synthesizer cal.
// 	  cc_write_reg(TI_CCxxx0_FSCAL1,   0x00); // Frequency synthesizer cal.
// 	  cc_write_reg(TI_CCxxx0_FSCAL0,   0x11); // Frequency synthesizer cal.
// 	  cc_write_reg(TI_CCxxx0_FSTEST,   0x59); // Frequency synthesizer cal.
// 	  cc_write_reg(TI_CCxxx0_TEST2,    0x88); // Various test settings.
// 	  cc_write_reg(TI_CCxxx0_TEST1,    0x31); // Various test settings.
// 	  cc_write_reg(TI_CCxxx0_TEST0,    0x0B);  // Various test settings.
// }

// /*******************************************************************************
//  * @fn     void port2_isr( void )
//  * @brief  SPI ISR (NOTE: Port must be the same as GDO0 port!)
//  * ****************************************************************************/
// #pragma vector=PORT2_VECTOR
// __interrupt void port2_isr(void) // CHANGE
// {
//   uint8_t length = CC2500_BUFFER_LENGTH;
// 
//   // Check to see if this interrupt was caused by the GDO0 pin from the CC2500
//   if ( GDO0_PxIFG & GDO0_PIN )
//   {
//       if( receive_packet(p_rx_buffer,&length) )
//       {
//         // Successful packet receive, now send data to callback function
//         if( rx_callback( p_rx_buffer, length ) )
//         {
//           // If rx_callback returns nonzero, wakeup the processor
//           __bic_SR_register_on_exit(LPM1_bits);
//         }
// 
//         // Clear the buffer
//         memset( p_rx_buffer, 0x00, sizeof(p_rx_buffer) );
//       }
//       else
//       {
//         // A failed receive can occur due to bad CRC or (if address checking is
//         // enabled) an address mismatch
// 
//         //uart_write("CRC NOK\r\n", 9);
//       }
// 
//   }
//   GDO0_PxIFG &= ~GDO0_PIN;  // Clear interrupt flag
// 
//   // Only needed if radio is configured to return to IDLE after transmission
//   // Check register MCSM1.TXOFF_MODE
//   // cc_strobe(TI_CCxxx0_SRX); // enter receive mode again
// }
// void CC_interrupt_Handler() {
// 	uint8_t length = CC2500_BUFFER_LENGTH;
// 
// 	// Check to see if this interrupt was caused by the GDO0 pin from the CC2500
// 	if ( CC_PORT.IN & CC_GD0 )
// 	{
// 		if( cc2500_receive_packet(p_rx_buffer,&length) )
// 		{
// 			// Successful packet receive, now send data to callback function
// 			if( rx_callback( p_rx_buffer, length ) )
// 			{
// 				// If rx_callback returns nonzero, wakeup the processor
// 				__bic_SR_register_on_exit(LPM1_bits);
// 			}
// 
// 			// Clear the buffer
// 			memset( p_rx_buffer, 0x00, sizeof(p_rx_buffer) );
// 		}
// 		else
// 		{
// 			// A failed receive can occur due to bad CRC or (if address checking is
// 			// enabled) an address mismatch
// 
// 			//uart_write("CRC NOK\r\n", 9);
// 		}
// 
// 	}
// 	CC_PORT.INTFLAGS = (1<<0);
// 
// 	// Only needed if radio is configured to return to IDLE after transmission
// 	// Check register MCSM1.TXOFF_MODE
// 	// cc_strobe(TI_CCxxx0_SRX); // enter receive mode again
// }

/*******************************************************************************
 * @fn void spi_setup(void)
 * @brief Setup SPI with the appropriate settings for CCxxxx communication
 * ****************************************************************************/
void spi_setup(void)
{
	// PX7 (SCK), PX5 (MOSI), PX4 (CSN), PX3 (CE) as outputs
	CC_PORT.DIRSET = (1<<CC_SCK)|(1<<CC_MOSI)|(1<<CC_CSN);//|(1<<CC_CE);
	// PX6 (MISO), PX2 (IRQ) as inputs
	CC_PORT.DIRCLR = (1<<CC_GD0)|(1<<CC_MISO)|(1<<CC_GD2);
	// PX4 (CSN) enable pull-up. (TODO[ ]: VERIFY THAT THIS IS HOW YOU DO THIS IN AN XMEGA!!)
	CSN_DESELECT();
	
	//PORTCFG_MPCMASK = (1<<CC_CSN)|(1<<CC_MISO);//|(1<<CC_MOSI);//|(1<<CC_GD0);
	//pull up configuration
	//CC_PORT.PIN0CTRL = 0b00011000;

	CC_SPI.CTRL = SPI_ENABLE_bm|SPI_MASTER_bm|SPI_MODE_0_gc|SPI_PRESCALER_DIV16_gc;
}

/*******************************************************************************
 * @fn void cc_write_reg(uint8_t addr, uint8_t value)
 * @brief Write single register value to CCxxxx
 * ****************************************************************************/
void cc_write_reg(uint8_t addr, uint8_t value)
{
	CSN_SELECT();
	//wait for SO pin to go low
	while (CC_PORT.IN & (1<<CC_MISO));
	CC_SPI_TX(addr);
	CC_SPI_TX(value);
	CSN_DESELECT();
}

/*******************************************************************************
 * @fn cc_write_burst_reg(uint8_t addr, uint8_t *buffer, uint8_t count)
 * @brief Write multiple values to CCxxxx
 * ****************************************************************************/
void cc_write_burst_reg(uint8_t addr, uint8_t *buffer, uint8_t count)
{
	uint16_t i;
  
  	CSN_SELECT();
	while (CC_PORT.IN & (1<<CC_MISO));
  	CC_SPI_TX(addr | TI_CCxxx0_WRITE_BURST);
	for (i = 0; i < count; i++) {
  		CC_SPI_TX(buffer[i]);
	}
  	CSN_DESELECT();

}

/*******************************************************************************
 * @fn uint8_t cc_read_reg(uint8_t addr)
 * @brief read single register from CCxxxx
 * ****************************************************************************/
uint8_t cc_read_reg(uint8_t addr)
{
	uint8_t x;
	CSN_SELECT();
	while (CC_PORT.IN & (1<<CC_MISO));
	CC_SPI_TX(addr | TI_CCxxx0_READ_BURST);//TI_CCxxx0_READ_SINGLE);
	x = CC_SPI_RX();
	CSN_DESELECT();

	return x;
}

/*******************************************************************************
 * @fn cc_read_burst_reg(uint8_t addr, uint8_t *buffer, uint8_t count)
 * @brief read multiple registers from CCxxxx
 * ****************************************************************************/
void cc_read_burst_reg(uint8_t addr, uint8_t *buffer, uint8_t count)
{
	uint8_t i;
	
	CSN_SELECT();
	while (CC_PORT.IN & (1<<CC_MISO));
	CC_SPI_TX(addr | TI_CCxxx0_READ_BURST);
	for (i = 0; i < count; i++)
	{
		buffer[i] = CC_SPI_RX();
	}
	CSN_DESELECT();
}

/*******************************************************************************
 * @fn uint8_t cc_read_status(uint8_t addr)
 * @brief send status command and read returned status byte
 * ****************************************************************************/
uint8_t cc_read_status(uint8_t addr)
{
	uint8_t status;
  
	CSN_SELECT();
	while (CC_PORT.IN & (1<<CC_MISO));
	CC_SPI_TX(addr | TI_CCxxx0_READ_BURST);
	status = CC_SPI.DATA;
	CSN_DESELECT();

	return status;
}

/*******************************************************************************
 * @fn void cc_strobe (uint8_t strobe)
 * @brief send strobe command
 * ****************************************************************************/
void cc_strobe(uint8_t strobe)
{
	CSN_SELECT();
	while (CC_PORT.IN & (1<<CC_MISO));
	CC_SPI_TX(strobe);
	CSN_DESELECT();
}
/*******************************************************************************
 * @fn void cc_powerup_reset()
 * @brief reset radio
 * ****************************************************************************/
void cc_powerup_reset(void)
{
	
	CSN_DESELECT();
	delay_us(1);
	CSN_SELECT();
	delay_us(1);
	CSN_DESELECT();
	delay_us(41);
	
	CSN_SELECT();
	while (CC_PORT.IN & (1<<CC_MISO));
	CC_SPI_TX(TI_CCxxx0_SRES);
	while (CC_PORT.IN & (1<<CC_MISO));
	CSN_DESELECT();
	
	
// 	CSN_DESELECT();
// 	nop();
// 	CC_PORT.OUTSET = (1<<CC_SCK);
// 	nop();
// 	CC_PORT.OUTCLR = (1<<CC_MOSI);
// 	nop();
// 	
// 	cc_strobe(TI_CCxxx0_SIDLE);
// 	
// 	nop();
// 	CC_PORT.OUTSET = (1<<CC_SCK);
// 	nop();
// 	CC_PORT.OUTCLR = (1<<CC_MOSI);
// 	nop();
// 	
// 	CSN_DESELECT();
// 	delay_us(10);
// 	CSN_SELECT();
// 	delay_us(8);
// 	CSN_DESELECT();
// 	delay_us(35);
// 	CSN_SELECT();
// 	while (CC_PORT.IN & (1<<CC_MISO));
// 	CC_SPI_TX(TI_CCxxx0_SRES);
// 	while (CC_PORT.IN & (1<<CC_MISO));
// 	
// 	CSN_DESELECT();
	
// 	//CC_PORT.DIRSET = (1<<CC_SCK);
// 	//CC_PORT.DIRSET = (1<<CC_MOSI);
// 	//CC_PORT.OUTSET = (1<<CC_SCK);
// 	//CC_PORT.OUTCLR = (1<<CC_MOSI);
// 	
// 	//CSN_DESELECT()
// 	//delay_us(30);
// 	CSN_SELECT()
// 	delay_us(10);
// 	CSN_DESELECT()
// 	delay_us(35);
// 	
// 	//CC_PORT.DIRCLR = (1<<CC_SCK);
// 	//CC_PORT.DIRCLR = (1<<CC_MOSI);
// 
// 	CSN_SELECT()
// 	while (CC_PORT.IN & (1<<CC_MISO));
// 	//_delay_ms(10);
// 	CC_SPI_TX(TI_CCxxx0_SRES);
// 	while (CC_PORT.IN & (1<<CC_MISO)); // Wait until the device has reset
// 	//_delay_ms(10);
// 	CC_PORT.OUTCLR = (1<<CC_MOSI);
// 	CSN_DESELECT()
	
	

}


void CC_SPI_TX(uint8_t Data) {
	CC_SPI.DATA = Data;
	while(!(CC_SPI.STATUS & (1<<SPI_IF_bp)));
}

uint8_t CC_SPI_RX() {

	CC_SPI.DATA = 0x00;
	while(!(CC_SPI.STATUS & (1<<SPI_IF_bp)));
	return CC_SPI.DATA;
}

int8_t AdjustRSSI( uint8_t RawRSSI )
{
	int8_t RSSI;
	if (RawRSSI >= 128)
		RSSI = (((int16_t)RawRSSI-256)/2)-CC_RSSI_OFSET;
	else
		RSSI = (((int16_t)RawRSSI)/2)-CC_RSSI_OFSET;
	return RSSI;
}
