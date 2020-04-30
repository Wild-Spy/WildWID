/*
 * msTimer.h
 *
 * Created: 15/10/2011 10:43:38 PM
 *  Author: MC
 */ 


#ifndef MSTIMER_H_
#define MSTIMER_H_

#include <stdint.h>

void msTimer_StartTimer(void);
void msTimer_StopTimer(void);
uint16_t msTimer_GetCounterTicks(void);


#endif /* MSTIMER_H_ */