

#ifndef _PowerMacros_H_
#define _PowerMacros_H_

#define BBAK_REASON_NONE				0
#define BBAK_REASON_SOFT_OFF			1
#define BBAK_REASON_LO_BAT				2
#define BBAK_REASON_NO_BAT				3

//Pins
#define SMPS_PWR_PORT				PORTE
#define SMPS_PWR_PIN				1
#define BATV_READ_EN_PORT			PORTB
#define BATV_READ_EN_PIN			0
#define PWR_GD_PORT					PORTB
#define	PWR_GD_PIN					2
#define MCU_STAT_PORT				PORTB
#define MCU_STAT_PIN				3
#define UNIT_PWR_SW_PORT			PORTD
#define UNIT_PWR_SW_PIN				0

#define MCU_STAT_DMY_PORT				PORTB
#define MCU_STAT_DMY_PIN				4

#define PWR_GOOD					(PWR_GD_PORT.IN&(1<<PWR_GD_PIN))
#define UNIT_PWR					(UNIT_PWR_SW_PORT.IN&(1<<UNIT_PWR_SW_PIN))
#define PWR_GOOD_INT_EN()			do {PWR_GD_PORT.INTFLAGS |= PORT_INT0IF_bm; PWR_GD_PORT.INTCTRL &= ~(PORT_INT0LVL_gm); PWR_GD_PORT.INTCTRL |= PORT_INT0LVL_MED_gc; if (!PWR_GOOD) {EnterBatBakMode(BBAK_REASON_NO_BAT);}} while (0)	//have to clear the bits first
#define PWR_GOOD_INT_DIS()			do {PWR_GD_PORT.INTCTRL &= ~(PORT_INT0LVL_gm);} while (0)
	
#define PIN_SET(PinName)			PinName##_PORT.OUTSET = (1<<PinName##_PIN)
#define PIN_CLR(PinName)			PinName##_PORT.OUTCLR = (1<<PinName##_PIN)
#define SMPS_ON()					PIN_CLR(SMPS_PWR)//SMPS_PWR_PORT.OUTCLR = (1<<SMPS_PWR_PIN)
#define SMPS_OFF()					PIN_SET(SMPS_PWR)//SMPS_PWR_PORT.OUTSET = (1<<SMPS_PWR_PIN)
#define BATV_READ_ON()				PIN_SET(BATV_READ_EN)//BATV_READ_EN_PORT.OUTSET = (1<<BATV_READ_EN_PIN)
#define BATV_READ_OFF()				PIN_CLR(BATV_READ_EN)//BATV_READ_EN_PORT.OUTCLR = (1<<BATV_READ_EN_PIN)
#define MCU_STAT_LED_ON()			PIN_CLR(MCU_STAT)//MCU_STAT_PORT.OUTCLR = (1<<MCU_STAT_PIN)
#define MCU_STAT_LED_OFF()			PIN_SET(MCU_STAT)//MCU_STAT_PORT.OUTSET = (1<<MCU_STAT_PIN)

uint8_t EnterBatBakMode(uint8_t Reason);

#endif //_PowerMacros_H_