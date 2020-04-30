/*
 * MX_RTC.h
 *
 *  Created on: 27/08/2010
 *      Author: MC
 */

#ifndef MX_WAVTRIG_H_
#define MX_WAVTRIG_H_

#include "MX_ErrorEnum.h"
#include <stdint.h>

/**********************
 *  Hardware Setup
 **********************/
#define AUDIO_AMP_ON_PORT	PORTA
#define AUDIO_AMP_ON_PIN	0

#define AUDIO_WAV_TRIG_ON_PORT	PORTA
#define AUDIO_WAV_TRIG_ON_PIN	1

#define AUDIO_WAV_TRIG_USART_PORT			PORTE
#define AUDIO_WAV_TRIG_USART				USARTE0

#define AUDIO_WAV_TRIG_BAUD 57600UL

#define AUDIO_MAX_SOUND_LENGTH_SECONDS		240

//#define AUDIO_WAV_TRIG_PLAY_SOUND_1_PORT	PORTA
//#define AUDIO_WAV_TRIG_PLAY_SOUND_1_PIN		5
//#define AUDIO_WAV_TRIG_PLAY_SOUND_2_PORT	PORTA
//#define AUDIO_WAV_TRIG_PLAY_SOUND_2_PIN		6
//#define AUDIO_WAV_TRIG_PLAY_SOUND_3_PORT	PORTA
//#define AUDIO_WAV_TRIG_PLAY_SOUND_3_PIN		6
//#define AUDIO_WAV_TRIG_PLAY_SOUND_4_PORT	PORTA
//#define AUDIO_WAV_TRIG_PLAY_SOUND_4_PIN		6
//#define AUDIO_WAV_TRIG_PLAY_SOUND_5_PORT	PORTA
//#define AUDIO_WAV_TRIG_PLAY_SOUND_5_PIN		6

/*********************************/
/*          Sound Setup          */
/*********************************/
//#define AUDIO_1_LENGTH_SECONDS 60
//#define AUDIO_2_LENGTH_SECONDS 10
//#define AUDIO_3_LENGTH_SECONDS 10
//#define AUDIO_4_LENGTH_SECONDS 10
//#define AUDIO_5_LENGTH_SECONDS 10

//typedef enum {
	//AUDIO_SOUND_ID_NONE,
	//AUDIO_SOUND_ID_1,
	//AUDIO_SOUND_ID_2,
	//AUDIO_SOUND_ID_3,
	//AUDIO_SOUND_ID_4,
	//AUDIO_SOUND_ID_5
//} audio_sound_id_t;

void AudioSetupHardware();						//Must be called before calling play or stop
//void AudioWavTrigOn();
//void AudioWavTrigOff();
//
//mx_err_t AudioPlayTrackAndTurnOff(); 
//
//


mx_err_t AudioPlay(uint16_t track_number);		//Start playing sound, turn on if necessary
void AudioStop();								//Stop playing all sounds and turn off
void AudioHandleSecondTick();					//Call once per second from timer interrupt
void AudioMainLoopChecks();
uint16_t AudioGetTrackCount();

//////////////////////
//  Internal Setup  //
//////////////////////
#define AUDIO_SETUP_TIME_SECONDS	5

#endif /* MX_WAVTRIG_H_ */
