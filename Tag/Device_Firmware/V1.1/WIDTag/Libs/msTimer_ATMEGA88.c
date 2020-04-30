/*
 * msTimer_ATMEGA88.c
 *
 * Created: 15/10/2011 10:48:28 PM
 *  Author: MC
 */ 

#include "msTimer.h"
#include <avr/interrupt.h>

static volatile uint16_t msTimerCount;

ISR(TIMER0_COMPA_vect) {
	msTimerCount++;
}

void msTimer_StartTimer(void) {
	PRR &= ~(1<<PRTIM0);
	msTimerCount = 0;
	//Setup Timer 0 for 1ms Timeout
	TCCR0A = (1<<WGM01);
	TIMSK0 = (1<<OCIE0A);
	//Output Compare Register A
	OCR0A = 0x83;
	TCNT0 = 0;
	//Start the timer - 64 (From prescaler)
	TCCR0B = (1<<CS01)|(1<<CS00);
	sei();
}

void msTimer_StopTimer(void) {
	//Stop the timer
	TCCR0B = 0;
	//Clear the timer
	//timer10ms = 0;
	PRR |= (1<<PRTIM0);
}

uint16_t msTimer_GetCounterTicks(void) {
	return msTimerCount;
}
