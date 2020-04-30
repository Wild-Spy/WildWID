
#ifndef _AVR_IO_H_
#define _AVR_IO_H_

#include <stdint.h>

#define _WORDREGISTER(regname)   \
    __extension__ union \
    { \
        uint16_t regname; \
        struct \
        { \
            uint8_t regname ## L; \
            uint8_t regname ## H; \
        }; \
    }
/*
--------------------------------------------------------------------------
TC - 16-bit Timer/Counter With PWM
--------------------------------------------------------------------------
*/

/* High-Resolution Extension */
typedef struct HIRES_struct
{
    uint8_t CTRLA;  /* Control Register */
} HIRES_t;

/* Clock Selection */
typedef enum TC_CLKSEL_enum
{
    TC_CLKSEL_OFF_gc = (0x00<<0),  /* Timer Off */
    TC_CLKSEL_DIV1_gc = (0x01<<0),  /* System Clock */
    TC_CLKSEL_DIV2_gc = (0x02<<0),  /* System Clock / 2 */
    TC_CLKSEL_DIV4_gc = (0x03<<0),  /* System Clock / 4 */
    TC_CLKSEL_DIV8_gc = (0x04<<0),  /* System Clock / 8 */
    TC_CLKSEL_DIV64_gc = (0x05<<0),  /* System Clock / 64 */
    TC_CLKSEL_DIV256_gc = (0x06<<0),  /* System Clock / 256 */
    TC_CLKSEL_DIV1024_gc = (0x07<<0),  /* System Clock / 1024 */
    TC_CLKSEL_EVCH0_gc = (0x08<<0),  /* Event Channel 0 */
    TC_CLKSEL_EVCH1_gc = (0x09<<0),  /* Event Channel 1 */
    TC_CLKSEL_EVCH2_gc = (0x0A<<0),  /* Event Channel 2 */
    TC_CLKSEL_EVCH3_gc = (0x0B<<0),  /* Event Channel 3 */
    TC_CLKSEL_EVCH4_gc = (0x0C<<0),  /* Event Channel 4 */
    TC_CLKSEL_EVCH5_gc = (0x0D<<0),  /* Event Channel 5 */
    TC_CLKSEL_EVCH6_gc = (0x0E<<0),  /* Event Channel 6 */
    TC_CLKSEL_EVCH7_gc = (0x0F<<0),  /* Event Channel 7 */
} TC_CLKSEL_t;

/* Waveform Generation Mode */
typedef enum TC_WGMODE_enum
{
    TC_WGMODE_NORMAL_gc = (0x00<<0),  /* Normal Mode */
    TC_WGMODE_FRQ_gc = (0x01<<0),  /* Frequency Generation Mode */
    TC_WGMODE_SS_gc = (0x03<<0),  /* Single Slope */
    TC_WGMODE_DS_T_gc = (0x05<<0),  /* Dual Slope, Update on TOP */
    TC_WGMODE_DS_TB_gc = (0x06<<0),  /* Dual Slope, Update on TOP and BOTTOM */
    TC_WGMODE_DS_B_gc = (0x07<<0),  /* Dual Slope, Update on BOTTOM */
} TC_WGMODE_t;

/* Event Action */
typedef enum TC_EVACT_enum
{
    TC_EVACT_OFF_gc = (0x00<<5),  /* No Event Action */
    TC_EVACT_CAPT_gc = (0x01<<5),  /* Input Capture */
    TC_EVACT_UPDOWN_gc = (0x02<<5),  /* Externally Controlled Up/Down Count */
    TC_EVACT_QDEC_gc = (0x03<<5),  /* Quadrature Decode */
    TC_EVACT_RESTART_gc = (0x04<<5),  /* Restart */
    TC_EVACT_FRQ_gc = (0x05<<5),  /* Frequency Capture */
    TC_EVACT_FRW_gc = (0x05<<5),  /* Frequency Capture (typo in earlier header file) */
    TC_EVACT_PW_gc = (0x06<<5),  /* Pulse-width Capture */
} TC_EVACT_t;

/* Event Selection */
typedef enum TC_EVSEL_enum
{
    TC_EVSEL_OFF_gc = (0x00<<0),  /* No Event Source */
    TC_EVSEL_CH0_gc = (0x08<<0),  /* Event Channel 0 */
    TC_EVSEL_CH1_gc = (0x09<<0),  /* Event Channel 1 */
    TC_EVSEL_CH2_gc = (0x0A<<0),  /* Event Channel 2 */
    TC_EVSEL_CH3_gc = (0x0B<<0),  /* Event Channel 3 */
    TC_EVSEL_CH4_gc = (0x0C<<0),  /* Event Channel 4 */
    TC_EVSEL_CH5_gc = (0x0D<<0),  /* Event Channel 5 */
    TC_EVSEL_CH6_gc = (0x0E<<0),  /* Event Channel 6 */
    TC_EVSEL_CH7_gc = (0x0F<<0),  /* Event Channel 7 */
} TC_EVSEL_t;

/* Error Interrupt Level */
typedef enum TC_ERRINTLVL_enum
{
    TC_ERRINTLVL_OFF_gc = (0x00<<2),  /* Interrupt Disabled */
    TC_ERRINTLVL_LO_gc = (0x01<<2),  /* Low Level */
    TC_ERRINTLVL_MED_gc = (0x02<<2),  /* Medium Level */
    TC_ERRINTLVL_HI_gc = (0x03<<2),  /* High Level */
} TC_ERRINTLVL_t;

/* Overflow Interrupt Level */
typedef enum TC_OVFINTLVL_enum
{
    TC_OVFINTLVL_OFF_gc = (0x00<<0),  /* Interrupt Disabled */
    TC_OVFINTLVL_LO_gc = (0x01<<0),  /* Low Level */
    TC_OVFINTLVL_MED_gc = (0x02<<0),  /* Medium Level */
    TC_OVFINTLVL_HI_gc = (0x03<<0),  /* High Level */
} TC_OVFINTLVL_t;

/* Compare or Capture D Interrupt Level */
typedef enum TC_CCDINTLVL_enum
{
    TC_CCDINTLVL_OFF_gc = (0x00<<6),  /* Interrupt Disabled */
    TC_CCDINTLVL_LO_gc = (0x01<<6),  /* Low Level */
    TC_CCDINTLVL_MED_gc = (0x02<<6),  /* Medium Level */
    TC_CCDINTLVL_HI_gc = (0x03<<6),  /* High Level */
} TC_CCDINTLVL_t;

/* Compare or Capture C Interrupt Level */
typedef enum TC_CCCINTLVL_enum
{
    TC_CCCINTLVL_OFF_gc = (0x00<<4),  /* Interrupt Disabled */
    TC_CCCINTLVL_LO_gc = (0x01<<4),  /* Low Level */
    TC_CCCINTLVL_MED_gc = (0x02<<4),  /* Medium Level */
    TC_CCCINTLVL_HI_gc = (0x03<<4),  /* High Level */
} TC_CCCINTLVL_t;

/* Compare or Capture B Interrupt Level */
typedef enum TC_CCBINTLVL_enum
{
    TC_CCBINTLVL_OFF_gc = (0x00<<2),  /* Interrupt Disabled */
    TC_CCBINTLVL_LO_gc = (0x01<<2),  /* Low Level */
    TC_CCBINTLVL_MED_gc = (0x02<<2),  /* Medium Level */
    TC_CCBINTLVL_HI_gc = (0x03<<2),  /* High Level */
} TC_CCBINTLVL_t;

/* Compare or Capture A Interrupt Level */
typedef enum TC_CCAINTLVL_enum
{
    TC_CCAINTLVL_OFF_gc = (0x00<<0),  /* Interrupt Disabled */
    TC_CCAINTLVL_LO_gc = (0x01<<0),  /* Low Level */
    TC_CCAINTLVL_MED_gc = (0x02<<0),  /* Medium Level */
    TC_CCAINTLVL_HI_gc = (0x03<<0),  /* High Level */
} TC_CCAINTLVL_t;

/* Timer/Counter Command */
typedef enum TC_CMD_enum
{
    TC_CMD_NONE_gc = (0x00<<2),  /* No Command */
    TC_CMD_UPDATE_gc = (0x01<<2),  /* Force Update */
    TC_CMD_RESTART_gc = (0x02<<2),  /* Force Restart */
    TC_CMD_RESET_gc = (0x03<<2),  /* Force Hard Reset */
} TC_CMD_t;

/* Fault Detect Action */
typedef enum AWEX_FDACT_enum
{
    AWEX_FDACT_NONE_gc = (0x00<<0),  /* No Fault Protection */
    AWEX_FDACT_CLEAROE_gc = (0x01<<0),  /* Clear Output Enable Bits */
    AWEX_FDACT_CLEARDIR_gc = (0x03<<0),  /* Clear I/O Port Direction Bits */
} AWEX_FDACT_t;

/* High Resolution Enable */
typedef enum HIRES_HREN_enum
{
    HIRES_HREN_NONE_gc = (0x00<<0),  /* No Fault Protection */
    HIRES_HREN_TC0_gc = (0x01<<0),  /* Enable High Resolution on Timer/Counter 0 */
    HIRES_HREN_TC1_gc = (0x02<<0),  /* Enable High Resolution on Timer/Counter 1 */
    HIRES_HREN_BOTH_gc = (0x03<<0),  /* Enable High Resolution both Timer/Counters */
} HIRES_HREN_t;

/*
--------------------------------------------------------------------------
TC - 16-bit Timer/Counter With PWM
--------------------------------------------------------------------------
*/

/* 16-bit Timer/Counter 0 */
typedef struct TC0_struct
{
    uint8_t CTRLA;  /* Control  Register A */
    uint8_t CTRLB;  /* Control Register B */
    uint8_t CTRLC;  /* Control register C */
    uint8_t CTRLD;  /* Control Register D */
    uint8_t CTRLE;  /* Control Register E */
    uint8_t reserved_0x05;
    uint8_t INTCTRLA;  /* Interrupt Control Register A */
    uint8_t INTCTRLB;  /* Interrupt Control Register B */
    uint8_t CTRLFCLR;  /* Control Register F Clear */
    uint8_t CTRLFSET;  /* Control Register F Set */
    uint8_t CTRLGCLR;  /* Control Register G Clear */
    uint8_t CTRLGSET;  /* Control Register G Set */
    uint8_t INTFLAGS;  /* Interrupt Flag Register */
    uint8_t reserved_0x0D;
    uint8_t reserved_0x0E;
    uint8_t TEMP;  /* Temporary Register For 16-bit Access */
    uint8_t reserved_0x10;
    uint8_t reserved_0x11;
    uint8_t reserved_0x12;
    uint8_t reserved_0x13;
    uint8_t reserved_0x14;
    uint8_t reserved_0x15;
    uint8_t reserved_0x16;
    uint8_t reserved_0x17;
    uint8_t reserved_0x18;
    uint8_t reserved_0x19;
    uint8_t reserved_0x1A;
    uint8_t reserved_0x1B;
    uint8_t reserved_0x1C;
    uint8_t reserved_0x1D;
    uint8_t reserved_0x1E;
    uint8_t reserved_0x1F;
    _WORDREGISTER(CNT);  /* Count */
    uint8_t reserved_0x22;
    uint8_t reserved_0x23;
    uint8_t reserved_0x24;
    uint8_t reserved_0x25;
    _WORDREGISTER(PER);  /* Period */
    _WORDREGISTER(CCA);  /* Compare or Capture A */
    _WORDREGISTER(CCB);  /* Compare or Capture B */
    _WORDREGISTER(CCC);  /* Compare or Capture C */
    _WORDREGISTER(CCD);  /* Compare or Capture D */
    uint8_t reserved_0x30;
    uint8_t reserved_0x31;
    uint8_t reserved_0x32;
    uint8_t reserved_0x33;
    uint8_t reserved_0x34;
    uint8_t reserved_0x35;
    _WORDREGISTER(PERBUF);  /* Period Buffer */
    _WORDREGISTER(CCABUF);  /* Compare Or Capture A Buffer */
    _WORDREGISTER(CCBBUF);  /* Compare Or Capture B Buffer */
    _WORDREGISTER(CCCBUF);  /* Compare Or Capture C Buffer */
    _WORDREGISTER(CCDBUF);  /* Compare Or Capture D Buffer */
} TC0_t;

/*
--------------------------------------------------------------------------
TC - 16-bit Timer/Counter With PWM
--------------------------------------------------------------------------
*/

/* 16-bit Timer/Counter 1 */
typedef struct TC1_struct
{
    uint8_t CTRLA;  /* Control  Register A */
    uint8_t CTRLB;  /* Control Register B */
    uint8_t CTRLC;  /* Control register C */
    uint8_t CTRLD;  /* Control Register D */
    uint8_t CTRLE;  /* Control Register E */
    uint8_t reserved_0x05;
    uint8_t INTCTRLA;  /* Interrupt Control Register A */
    uint8_t INTCTRLB;  /* Interrupt Control Register B */
    uint8_t CTRLFCLR;  /* Control Register F Clear */
    uint8_t CTRLFSET;  /* Control Register F Set */
    uint8_t CTRLGCLR;  /* Control Register G Clear */
    uint8_t CTRLGSET;  /* Control Register G Set */
    uint8_t INTFLAGS;  /* Interrupt Flag Register */
    uint8_t reserved_0x0D;
    uint8_t reserved_0x0E;
    uint8_t TEMP;  /* Temporary Register For 16-bit Access */
    uint8_t reserved_0x10;
    uint8_t reserved_0x11;
    uint8_t reserved_0x12;
    uint8_t reserved_0x13;
    uint8_t reserved_0x14;
    uint8_t reserved_0x15;
    uint8_t reserved_0x16;
    uint8_t reserved_0x17;
    uint8_t reserved_0x18;
    uint8_t reserved_0x19;
    uint8_t reserved_0x1A;
    uint8_t reserved_0x1B;
    uint8_t reserved_0x1C;
    uint8_t reserved_0x1D;
    uint8_t reserved_0x1E;
    uint8_t reserved_0x1F;
    _WORDREGISTER(CNT);  /* Count */
    uint8_t reserved_0x22;
    uint8_t reserved_0x23;
    uint8_t reserved_0x24;
    uint8_t reserved_0x25;
    _WORDREGISTER(PER);  /* Period */
    _WORDREGISTER(CCA);  /* Compare or Capture A */
    _WORDREGISTER(CCB);  /* Compare or Capture B */
    uint8_t reserved_0x2C;
    uint8_t reserved_0x2D;
    uint8_t reserved_0x2E;
    uint8_t reserved_0x2F;
    uint8_t reserved_0x30;
    uint8_t reserved_0x31;
    uint8_t reserved_0x32;
    uint8_t reserved_0x33;
    uint8_t reserved_0x34;
    uint8_t reserved_0x35;
    _WORDREGISTER(PERBUF);  /* Period Buffer */
    _WORDREGISTER(CCABUF);  /* Compare Or Capture A Buffer */
    _WORDREGISTER(CCBBUF);  /* Compare Or Capture B Buffer */
} TC1_t;

/*
--------------------------------------------------------------------------
USART - Universal Asynchronous Receiver-Transmitter
--------------------------------------------------------------------------
*/

/* Universal Synchronous/Asynchronous Receiver/Transmitter */
typedef struct USART_struct
{
    uint8_t DATA;  /* Data Register */
    uint8_t STATUS;  /* Status Register */
    uint8_t reserved_0x02;
    uint8_t CTRLA;  /* Control Register A */
    uint8_t CTRLB;  /* Control Register B */
    uint8_t CTRLC;  /* Control Register C */
    uint8_t BAUDCTRLA;  /* Baud Rate Control Register A */
    uint8_t BAUDCTRLB;  /* Baud Rate Control Register B */
} USART_t;

/*
--------------------------------------------------------------------------
PORT - Port Configuration
--------------------------------------------------------------------------
*/

/* I/O Ports */
typedef struct PORT_struct
{
    uint8_t DIR;  /* I/O Port Data Direction */
    uint8_t DIRSET;  /* I/O Port Data Direction Set */
    uint8_t DIRCLR;  /* I/O Port Data Direction Clear */
    uint8_t DIRTGL;  /* I/O Port Data Direction Toggle */
    uint8_t OUT;  /* I/O Port Output */
    uint8_t OUTSET;  /* I/O Port Output Set */
    uint8_t OUTCLR;  /* I/O Port Output Clear */
    uint8_t OUTTGL;  /* I/O Port Output Toggle */
    uint8_t IN;  /* I/O port Input */
    uint8_t INTCTRL;  /* Interrupt Control Register */
    uint8_t INT0MASK;  /* Port Interrupt 0 Mask */
    uint8_t INT1MASK;  /* Port Interrupt 1 Mask */
    uint8_t INTFLAGS;  /* Interrupt Flag Register */
    uint8_t reserved_0x0D;
    uint8_t reserved_0x0E;
    uint8_t reserved_0x0F;
    uint8_t PIN0CTRL;  /* Pin 0 Control Register */
    uint8_t PIN1CTRL;  /* Pin 1 Control Register */
    uint8_t PIN2CTRL;  /* Pin 2 Control Register */
    uint8_t PIN3CTRL;  /* Pin 3 Control Register */
    uint8_t PIN4CTRL;  /* Pin 4 Control Register */
    uint8_t PIN5CTRL;  /* Pin 5 Control Register */
    uint8_t PIN6CTRL;  /* Pin 6 Control Register */
    uint8_t PIN7CTRL;  /* Pin 7 Control Register */
} PORT_t;




extern USART_t USARTC0;
extern PORT_t PORTD;

#endif // _AVR_IO_H_
