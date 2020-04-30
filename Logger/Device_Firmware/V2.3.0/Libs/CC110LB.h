/** @file CC110L.h
*
* @brief CC110L radio functions - This is for the WID tag pickup CC110L not the inter-logger comms CC110L
*
* @author Matthew Cochrane
*     derived from Alvaro Prieto CC2500 library
*/
#ifndef _CC110LB_H
#define _CC110LB_H

#include <stdint.h>

#define CC110LB_BUFFER_LENGTH 64
#define CC110LB_PACKET_LENGTH 6

#define	CC11B_RSSI_OFSET	74

//Devices:
#define CC11B_SPI			SPID

//Pins:
//#define CC_IRQ		2

#define CC11B_SPI_PORT		PORTD
#define CC11B_MISO 			6
#define CC11B_MOSI 			5
#define CC11B_SCK 			7

#define CC11B_CSN_PORT		PORTD
#define CC11B_CSN 			4

#define CC11B_GD0_PORT		PORTD
#define CC11B_GD0 			2
#define CC11B_GD2_PORT		PORTD
#define CC11B_GD2 			3


// #ifndef DEVICE_ADDRESS
// #define DEVICE_ADDRESS 0x00
// #error Device address not set!
// #endif

void setup_CC110LB( uint8_t (*)(uint8_t*, uint8_t) );
void CC110LB_tx( uint8_t* p_buffer, uint8_t length );

void CC110LB_tx_packet(  uint8_t* p_buffer, uint8_t length );
uint8_t CC110LB_receive_packet( uint8_t* p_buffer, uint8_t* length , uint8_t* metrics );

uint8_t CC110LB_get_RX_FIFO_Status(void);

void CC110LB_write_reg(uint8_t addr, uint8_t value);
void CC110LB_write_burst_reg(uint8_t addr, uint8_t *buffer, uint8_t count);
uint8_t CC110LB_read_reg(uint8_t addr);
void CC110LB_read_burst_reg(uint8_t addr, uint8_t *buffer, uint8_t count);
uint8_t CC110LB_read_status(uint8_t addr);
void CC110LB_strobe(uint8_t strobe);
uint8_t CC110LB_readRXReg_FixErrata(uint8_t address);

void CC110LB_set_address( uint8_t );
void CC110LB_set_channel( uint8_t );
void CC110LB_set_power( uint8_t );

void CC110LB_sleep( );
void CC110LB_calibrate();

void CC110LB_enable_addressing();
void CC110LB_disable_addressing();

void CC110LB_set_variable_packet_length();
void CC110LB_set_fixed_packet_length(uint8_t packet_len);
uint8_t CC110LB_packet_length_mode_is_fixed();

void CC110LB_writeRFSettings(void);

int8_t CC110LB_AdjustRSSI(uint8_t RawRSSI);

/**
 * Packet header structure. The packet length byte is omitted, since the receive
 * function strips it away. Also, the CC110LB_tx_packet function inserts it
 * automatically.
 */
typedef struct
{
  uint8_t destination;  // Packet destination
  uint8_t source;       // Packet source
  uint8_t type;         // Packet Type
  uint8_t flags;        // Misc flags
} packet_header_t;

//
// Packet Type Definitions (todo)
//
#define IO_CHANGE (0x01)

// Configuration Registers
#define TI_CC110LB_IOCFG2       0x00        // GDO2 output pin configuration
#define TI_CC110LB_IOCFG1       0x01        // GDO1 output pin configuration
#define TI_CC110LB_IOCFG0       0x02        // GDO0 output pin configuration
#define TI_CC110LB_FIFOTHR      0x03        // RX FIFO and TX FIFO thresholds
#define TI_CC110LB_SYNC1        0x04        // Sync word, high byte
#define TI_CC110LB_SYNC0        0x05        // Sync word, low byte
#define TI_CC110LB_PKTLEN       0x06        // Packet length
#define TI_CC110LB_PKTCTRL1     0x07        // Packet automation control
#define TI_CC110LB_PKTCTRL0     0x08        // Packet automation control
#define TI_CC110LB_ADDR         0x09        // Device address
#define TI_CC110LB_CHANNR       0x0A        // Channel number
#define TI_CC110LB_FSCTRL1      0x0B        // Frequency synthesizer control
#define TI_CC110LB_FSCTRL0      0x0C        // Frequency synthesizer control
#define TI_CC110LB_FREQ2        0x0D        // Frequency control word, high byte
#define TI_CC110LB_FREQ1        0x0E        // Frequency control word, middle byte
#define TI_CC110LB_FREQ0        0x0F        // Frequency control word, low byte
#define TI_CC110LB_MDMCFG4      0x10        // Modem configuration
#define TI_CC110LB_MDMCFG3      0x11        // Modem configuration
#define TI_CC110LB_MDMCFG2      0x12        // Modem configuration
#define TI_CC110LB_MDMCFG1      0x13        // Modem configuration
#define TI_CC110LB_MDMCFG0      0x14        // Modem configuration
#define TI_CC110LB_DEVIATN      0x15        // Modem deviation setting
#define TI_CC110LB_MCSM2        0x16        // Main Radio Cntrl State Machine config
#define TI_CC110LB_MCSM1        0x17        // Main Radio Cntrl State Machine config
#define TI_CC110LB_MCSM0        0x18        // Main Radio Cntrl State Machine config
#define TI_CC110LB_FOCCFG       0x19        // Frequency Offset Compensation config
#define TI_CC110LB_BSCFG        0x1A        // Bit Synchronization configuration
#define TI_CC110LB_AGCCTRL2     0x1B        // AGC control
#define TI_CC110LB_AGCCTRL1     0x1C        // AGC control
#define TI_CC110LB_AGCCTRL0     0x1D        // AGC control
//#define TI_CC110LB_WOREVT1      0x1E        // High byte Event 0 timeout
//#define TI_CC110LB_WOREVT0      0x1F        // Low byte Event 0 timeout
//#define TI_CC110LB_WORCTRL      0x20        // Wake On Radio control
#define TI_CC110LB_FREND1       0x21        // Front end RX configuration
#define TI_CC110LB_FREND0       0x22        // Front end TX configuration
#define TI_CC110LB_FSCAL3       0x23        // Frequency synthesizer calibration
#define TI_CC110LB_FSCAL2       0x24        // Frequency synthesizer calibration
#define TI_CC110LB_FSCAL1       0x25        // Frequency synthesizer calibration
#define TI_CC110LB_FSCAL0       0x26        // Frequency synthesizer calibration
//#define TI_CC110LB_RCCTRL1      0x27        // RC oscillator configuration
//#define TI_CC110LB_RCCTRL0      0x28        // RC oscillator configuration
//#define TI_CC110LB_FSTEST       0x29        // Frequency synthesizer cal control
//#define TI_CC110LB_PTEST        0x2A        // Production test
//#define TI_CC110LB_AGCTEST      0x2B        // AGC test
#define TI_CC110LB_TEST2        0x2C        // Various test settings
#define TI_CC110LB_TEST1        0x2D        // Various test settings
#define TI_CC110LB_TEST0        0x2E        // Various test settings

// Strobe commands
#define TI_CC110LB_SRES         0x30        // Reset chip.
#define TI_CC110LB_SFSTXON      0x31        // Enable/calibrate freq synthesizer
#define TI_CC110LB_SXOFF        0x32        // Turn off crystal oscillator.
#define TI_CC110LB_SCAL         0x33        // Calibrate freq synthesizer & disable
#define TI_CC110LB_SRX          0x34        // Enable RX.
#define TI_CC110LB_STX          0x35        // Enable TX.
#define TI_CC110LB_SIDLE        0x36        // Exit RX / TX - enter idel state
//#define TI_CC110LB_SAFC         0x37        // AFC adjustment of freq synthesizer
//#define TI_CC110LB_SWOR         0x38        // Start automatic RX polling sequence
#define TI_CC110LB_SPWD         0x39        // Enter pwr down mode when CSn goes hi
#define TI_CC110LB_SFRX         0x3A        // Flush the RX FIFO buffer.
#define TI_CC110LB_SFTX         0x3B        // Flush the TX FIFO buffer.
//#define TI_CC110LB_SWORRST      0x3C        // Reset real time clock.
#define TI_CC110LB_SNOP         0x3D        // No operation.

// Status registers
#define TI_CC110LB_PARTNUM      0x30        // Part number
#define TI_CC110LB_VERSION      0x31        // Current version number
#define TI_CC110LB_FREQEST      0x32        // Frequency offset estimate
#define TI_CC110LB_CRC_REG      0x33        // CRC OK
#define TI_CC110LB_RSSI         0x34        // Received signal strength indication
#define TI_CC110LB_MARCSTATE    0x35        // Control state machine state
//#define TI_CC110LB_WORTIME1     0x36        // High byte of WOR timer
//#define TI_CC110LB_WORTIME0     0x37        // Low byte of WOR timer
#define TI_CC110LB_PKTSTATUS    0x38        // Current GDOx status and packet status
//#define TI_CC110LB_VCO_VC_DAC   0x39        // Current setting from PLL cal module
#define TI_CC110LB_TXBYTES      0x3A        // Underflow and # of bytes in TXFIFO
#define TI_CC110LB_RXBYTES      0x3B        // Overflow and # of bytes in RXFIFO
#define TI_CC110LB_NUM_RXBYTES_MSK  0x7F        // Mask "# of bytes" field in _RXBYTES
#define TI_CC110LB_OVF_RXBYTES_MSK  0x80        // Mask "# of bytes" field in _RXBYTES
#define TI_CC110LB_NUM_TXBYTES_MSK  0x7F        // Mask "# of bytes" field in _TXBYTES
#define TI_CC110LB_OVF_TXBYTES_MSK  0x80        // Mask "# of bytes" field in _TXBYTES

// Other memory locations
#define TI_CC110LB_PATABLE      0x3E
#define TI_CC110LB_TXFIFO       0x3F
#define TI_CC110LB_RXFIFO       0x3F

// Masks for appended status bytes
//#define TI_CC110LB_LQI_RX       0x01        // Position of LQI byte
#define TI_CC110LB_CRC_OK       0x80        // Mask "CRC_OK" bit within LQI byte

// Definitions to support burst/single access:
#define TI_CC110LB_WRITE_BURST  0x40
#define TI_CC110LB_READ_SINGLE  0x80
#define TI_CC110LB_READ_BURST   0xC0

#define TI_CC110LB_STAT_STATE_MASK	0x70
#define TI_CC110LB_STAT_FIFOBYTES_MASK	0x0F
#define TI_CC110LB_STAT_STATE_IDLE	(0x00<<4)
#define TI_CC110LB_STAT_STATE_RX	(0x01<<4)
#define TI_CC110LB_STAT_STATE_TX	(0x02<<4)
#define TI_CC110LB_STAT_STATE_TXFIFOUNDERFLOW	(0x07<<4)

#define TI_CC110LB_PACKET_LENGTH_CONFIG (0x03)
#define TI_CC110LB_FIXED_PACKET_LENGTH_MODE (0x00)
#define TI_CC110LB_VARIABLE_PACKET_LENGTH_MODE (0x01)


//CC110LB_PACKET_LENGTH above replaces this
#define CC_PKT_LEN			   0x06  // Indicates the packet length when fixed packet length mode
									 // is enabled. If variable packet length mode is used, this 
									 // value indicates the maximum packet length allowed. This
									 // value must be different from 0.

#endif /* _CC110LB_H */