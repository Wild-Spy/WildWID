/*
 * MX_RTC.c
 *
 *  Created on: 27/08/2010
 *      Author: MC
 */

#include "MX_WavTrig.h"
#include <avr/io.h>
#include <avr/delay.h>
#include "MX_USART.h"
#include <stdbool.h>

#define AUDIO_WAV_TRIG_MAX_CONCURRENT_VOICES	14

#define WTP_SOM1	0xF0
#define WTP_SOM2	0xAA
#define WTP_EOM		0x55

//GET_VERSION
//Message Code = 0x01, Length = 5
#define WTP_CODE_GET_VERSION		0x01
#define WTP_LEN_GET_VERSION			0x05
//Data = none
//Response = VERSION_STRING
//Comments: Requests the WAV Trigger to transmit the VERSION_STRING message
//Example: 0xf0, 0xaa, 0x05, 0x01, 0x55

//GET_SYS_INFO
//Message Code = 0x02, Length = 5
#define WTP_CODE_GET_SYS_INFO		0x02
#define WTP_LEN_GET_SYS_INFO		0x05
//Data = none
//Response = SYS_INFO
//Comments: Requests the WAV Trigger to transmit the SYS_INFO message
//Example: 0xf0, 0xaa, 0x05, 0x02, 0x55

//CONTROL_TRACK
//Message Code = 0x03, Length = 8
#define WTP_CODE_CONTROL_TRACK		0x03
#define WTP_LEN_CONTROL_TRACK		0x08
//Data = Track Control Code (1 byte), Track Number (2 bytes)
//Response = none
//Comments: Sends a Track Control Code to a specific track number
//Example: 0xf0, 0xaa, 0x08, 0x03, 0x01, 0x0a, 0x00, 0x55
//Track Control Codes:
//PLAY_SOLO = 0x00: Play track without polyphony, stops all other tracks
//PLAY_POLY = 0x01: Play track polyphonically
//PAUSE = 0x02: Pause track
//RESUME = 0x03: Resume track
//STOP = 0x04: Stop track
//LOOP_ON = 0x05: Set the track loop flag
//LOOP_OFF = 0x06: Clear the track loop flag
//LOAD = 0x07: Load and pause track
typedef enum {
	WTP_CTRL_CODE_PLAY_SOLO		= 0x00,
	WTP_CTRL_CODE_PLAY_POLY		= 0x01,
	WTP_CTRL_CODE_PAUSE			= 0x02,
	WTP_CTRL_CODE_RESUME		= 0x03,
	WTP_CTRL_CODE_STOP			= 0x04,
	WTP_CTRL_CODE_TRK_LOOP_SET	= 0x05,
	WTP_CTRL_CODE_TRK_LOOP_CLR	= 0x06,
	WTP_CTRL_CODE_LOAD_PAUSE	= 0x07
} wtp_ctrl_code_t;


//CNTRL_TRACK_EX
//(Firmware Version 1.30 and above)
//Message Code = 0x0d, Length = 9
#define WTP_CODE_CONTROL_TRACK_EX		0x0D
#define WTP_LEN_CONTROL_TRACK_EX		0x09
//Data = Track Control Code (1 byte), Track Number (2 bytes), Lock Flag (1 byte)
//Response = none
//Comments: Same as CONTROL_TRACK. Lock Flag = TRUE prevents track’s voice from being stolen
//Example: 0xf0, 0xaa, 0x08, 0x03, 0x01, 0x0a, 0x00, 0x01, 0x55
//Track Control Codes same as CONTROL_TRACK above:

//STOP_ALL
//Message Code = 0x04, Length = 5
#define WTP_CODE_STOP_ALL			0x04
#define WTP_LEN_STOP_ALL			0x05
//Data = none
//Response = none
//Comments: Commands the WAV Trigger to stop all tracks immediately
//Example: 0xf0, 0xaa, 0x05, 0x04, 0x55

//VOLUME
//Message Code = 0x05, Length = 7
#define WTP_CODE_VOLUME			0x05
#define WTP_LEN_VOLUME			0x07
//Data = Volume (2 bytes, signed int, -70dB to +10dB)
//Response = none
//Comments: Updates the output volume of the WAV Trigger with the specified gain in dB
//Example: 0xf0, 0xaa, 0x07, 0x05, 0x00, 0x00, 0x55

//GET_STATUS
//Message Code = 0x07, Length = 5
#define WTP_CODE_GET_STATUS			0x07
#define WTP_LEN_GET_STATUS			0x05
//Data = none
//Response = STATUS
//Comments: Requests the WAV Trigger to transmit the STATUS message
//Example: 0xf0, 0xaa, 0x05, 0x07, 0x55

/////////////////////////////////////////////////////////////////////////////////////////
//Response Messages
/////////////////////////////////////////////////////////////////////////////////////////

//VERSION_STRING
//Message Code = 0x81, Length = 25
#define WTP_RSP_CODE_VERSION_STRING		0x81
#define WTP_RSP_LEN_VERSION_STRING		25
//Data = Firmware version string (char[20])
//Comments: Data contains the WAV Trigger version – ASCII, not null-terminated
//Example: 0xf0, 0xaa, 0x19, 0x81, ( char[20] ), 0x55

//SYS_INFO
//Message Code = 0x82, Length = 8
#define WTP_RSP_CODE_SYS_INFO			0x82
#define WTP_RSP_LEN_SYS_INFO			8
//Data = Number of voices (1 byte), Number of tracks (2 bytes)
//Comments: Data contains current config information
//Example: 0xf0, 0xaa, 0x08, 0x82, 0x08, 0x7f, 0x00, 0x55

//STATUS
//Message Code = 0x83, Length = variable
#define WTP_RSP_CODE_STATUS			0x83
#define WTP_RSP_LEN_STATUS_MAX		2*AUDIO_WAV_TRIG_MAX_CONCURRENT_VOICES+5
//Data = 2 * T, where T are the track numbers that are playing (LSB, MSB)
//Comments: The data is a list of 2-byte track numbers that are currently playing. If there are no tracks playing, the number of data bytes will be 0.
//Example: 0xf0, 0xaa, 0x09, 0x83, 0x01, 0x00, 0x0e, 0x00, 0x55

#define AUDIO_TRACK_ID_NONE		0xFFFF
typedef enum
{
	AUDIO_MODE_OFF,
	AUDIO_MODE_SETUP,
	AUDIO_MODE_PLAY
} audio_mode_t;

audio_mode_t audio_mode = AUDIO_MODE_OFF;
uint16_t audio_mode_time_remaining_secs = 0;
uint16_t track_to_play = 0;
uint16_t wav_trig_track_count = 0;
//void Audio_Wav_Trig_On(uint16_t onTime);
//void TurnAudioOff();

int8_t AudioWavTrigOn();
void AudioWavTrigOff();

#define Pin2BitMask(pin)	(1<<pin)
#define WAV_TRIG_ON()		AUDIO_WAV_TRIG_ON_PORT.OUTSET = Pin2BitMask(AUDIO_WAV_TRIG_ON_PIN)
#define WAV_TRIG_OFF()		AUDIO_WAV_TRIG_ON_PORT.OUTCLR = Pin2BitMask(AUDIO_WAV_TRIG_ON_PIN)
#define AMP_ON()			AUDIO_AMP_ON_PORT.OUTSET = Pin2BitMask(AUDIO_AMP_ON_PIN)
#define AMP_OFF()			AUDIO_AMP_ON_PORT.OUTCLR = Pin2BitMask(AUDIO_AMP_ON_PIN)

#define WAV_TRIG_PLAY_SOUND_PIN_ON(sound_id)	AUDIO_WAV_TRIG_PLAY_SOUND_ ## sound_id ## _PORT.OUTSET = Pin2MitMask(AUDIO_WAV_TRIG_PLAY_SOUND_ ## sound_id ## _PIN)
#define WAV_TRIG_PLAY_SOUND_PIN_OFF(sound_id)	AUDIO_WAV_TRIG_PLAY_SOUND_ ## sound_id ## _PORT.OUTCLR = Pin2MitMask(AUDIO_WAV_TRIG_PLAY_SOUND_ ## sound_id ## _PIN)
//#define WAV_TRIG_PLAY_SOUND(sound_id)	do { WAV_TRIG_PLAY_SOUND_PIN_ON(sound_id); \
											 //_delay_ms(100); \
											 //WAV_TRIG_PLAY_SOUND_PIN_OFF(sound_id); \
										   //} while(0)

void AudioSetupHardware() {
	AUDIO_AMP_ON_PORT.DIRSET = Pin2BitMask(AUDIO_AMP_ON_PIN);
	AUDIO_WAV_TRIG_ON_PORT.DIRSET = Pin2BitMask(AUDIO_WAV_TRIG_ON_PIN);
	
	//USART pins as inputs
	AUDIO_WAV_TRIG_USART_PORT.DIRCLR = (1<<2)|(1<<3);
	//AUDIO_WAV_TRIG_PLAY_SOUND_1_PORT.DIRSET = Pin2BitMask(AUDIO_WAV_TRIG_PLAY_SOUND_1_PIN);
	//AUDIO_WAV_TRIG_PLAY_SOUND_2_PORT.DIRSET = Pin2BitMask(AUDIO_WAV_TRIG_PLAY_SOUND_2_PIN);
	//AUDIO_WAV_TRIG_PLAY_SOUND_3_PORT.DIRSET = Pin2BitMask(AUDIO_WAV_TRIG_PLAY_SOUND_3_PIN);
	//AUDIO_WAV_TRIG_PLAY_SOUND_4_PORT.DIRSET = Pin2BitMask(AUDIO_WAV_TRIG_PLAY_SOUND_4_PIN);
	//AUDIO_WAV_TRIG_PLAY_SOUND_5_PORT.DIRSET = Pin2BitMask(AUDIO_WAV_TRIG_PLAY_SOUND_5_PIN);
	
	AudioWavTrigOff();
}

inline USART_err_t AudioTxByte(char txData) {
	return USART_tx_Byte(&AUDIO_WAV_TRIG_USART, txData);
}

inline USART_err_t AudioRxByte(uint16_t timeout_ms, uint8_t* data) {
	return USART_rx_Byte(&AUDIO_WAV_TRIG_USART, timeout_ms, data);
}

//response_len is the full packet length, response_data will have a length of response_len-5
//max_len is the max length of the FULL response packet.
int8_t AudioReceiveCommand(uint8_t* response_len, uint8_t* response_command, uint8_t* response_data, uint16_t timeout_ms, uint8_t max_len) {
	uint8_t byte;
	USART_err_t err;
	
	err = AudioRxByte(timeout_ms, &byte);
	if (err < 0) return -1; //usart rx error
	if (byte != WTP_SOM1) return -2; //protocol error
	
	err = AudioRxByte(timeout_ms, &byte);
	if (err < 0) return -1; //usart rx error
	if (byte != WTP_SOM2) return -2; //protocol error
	
	err = AudioRxByte(timeout_ms, response_len);
	if (err < 0) return -1; //usart rx error
	
	err = AudioRxByte(timeout_ms, response_command);
	if (err < 0) return -1; //usart rx error
	
	uint8_t data_len;
	if (*response_len < 5)
		return -2; //protocol error
	else if (*response_len > max_len)
		return -3; //too long...
	else
		data_len = *response_len-5;
	
	for (uint8_t i = 0; i < data_len; i++) {
		err = AudioRxByte(timeout_ms, response_data++);
		if (err < 0) return -1; //usart rx error
	}
	
	err = AudioRxByte(timeout_ms, &byte);
	if (err < 0) return -1; //usart rx error
	if (byte != WTP_EOM) return -2; //protocol error
	
	return 0;
}

//len is the full length of the command therefore data should have a length of len-5
//response
//response_len is the full packet length, response_data will have a length of response_len-5
int8_t AudioSendCommandAndRx(uint8_t cmd_id, uint8_t len, uint8_t* data, uint8_t* response_data, uint8_t* response_len, uint8_t* response_command, uint8_t max_response_len) {
	AudioTxByte(WTP_SOM1);
	AudioTxByte(WTP_SOM2);
	AudioTxByte(len);
	AudioTxByte(cmd_id);
	
	for (int16_t i = 0; i < len-5; i++) {
		AudioTxByte(data[i]);
	}
	
	AudioTxByte(WTP_EOM);
	
	if (response_data == NULL) return 0; //no rx necessary... just return
	
	//Flush UART
	uint8_t tmpb;
	//USART_rx_Byte_nb(&AUDIO_WAV_TRIG_USART, &tmpb);
	
	return AudioReceiveCommand(response_len, response_command, response_data, 1000, max_response_len);
	
}

int8_t AudioSendCommand(uint8_t cmd_id, uint8_t len, uint8_t* data) {
	return AudioSendCommandAndRx(cmd_id, len, data, 0, 0, 0, 0);
}


//length of response_str will be WTP_RSP_LEN_VERSION_STRING if successful
int8_t AudioGetVersion(uint8_t* response) {
	uint8_t cmd_id;
	uint8_t cmd_len;
	int8_t resp;
	resp = AudioSendCommandAndRx(WTP_CODE_GET_VERSION, WTP_LEN_GET_VERSION, 0, response, &cmd_len, &cmd_id, WTP_RSP_LEN_VERSION_STRING);
	if (resp < 0) return resp;
	
	if (cmd_id != WTP_RSP_CODE_VERSION_STRING) return -10;
	if (cmd_len != WTP_RSP_LEN_VERSION_STRING) return -11;
	return 0;
}

//length of response_str will be WTP_RSP_LEN_SYS_INFO-5 if successful
int8_t AudioGetSysInfo(uint8_t* response) {
	uint8_t cmd_id;
	uint8_t cmd_len;
	int8_t resp;
	resp = AudioSendCommandAndRx(WTP_CODE_GET_SYS_INFO, WTP_LEN_GET_SYS_INFO, 0, response, &cmd_len, &cmd_id, WTP_RSP_LEN_SYS_INFO);
	if (resp < 0) return resp;
	
	if (cmd_id != WTP_RSP_CODE_SYS_INFO) return -10;
	if (cmd_len != WTP_RSP_LEN_SYS_INFO) return -11;
	return 0;
}

//voice_track_ids array must be at least AUDIO_WAV_TRIG_MAX_CONCURRENT_VOICES*2 bytes long. That's AUDIO_WAV_TRIG_MAX_CONCURRENT_VOICES uint16_t's)
//voice_track_ids is just the data not the header bytes (ie 0xF0 0xAA LEN_BYTE CMD BYTE [voice_track_ids data here] 0x55)
//number_voices_playing returns the number of voices currently playing
//voice_track_ids returns the actual track numbers playing
int8_t AudioGetStatus(uint16_t* voice_track_ids, uint8_t* number_voices_playing) {
	uint8_t cmd_id;
	
	uint8_t cmd_len;
	int8_t resp;
	resp = AudioSendCommandAndRx(WTP_CODE_GET_STATUS, WTP_LEN_GET_STATUS, 0, (uint8_t*)voice_track_ids, &cmd_len, &cmd_id, AUDIO_WAV_TRIG_MAX_CONCURRENT_VOICES);
	if (resp < 0) return resp;
	
	if (cmd_id != WTP_RSP_CODE_STATUS) return -10;
	//if (cmd_len != WTP_RSP_LEN_SYS_INFO) return -11;
	*number_voices_playing = (cmd_len-5)/2;
	return 0;
}

int8_t AudioControlTrack(wtp_ctrl_code_t code, uint16_t track_id) {
		uint8_t data[3];
		data[0] = code;
		data[1] = *(((uint8_t*)&track_id)+0);
		data[2] = *(((uint8_t*)&track_id)+1);
		return AudioSendCommand(WTP_CODE_CONTROL_TRACK, WTP_LEN_CONTROL_TRACK, data);
}

int8_t AudioStopAll() {
	return AudioSendCommand(WTP_CODE_STOP_ALL, WTP_LEN_STOP_ALL, 0);
}

int8_t AudioGetVoicePlayingCount() {
	uint16_t data[AUDIO_WAV_TRIG_MAX_CONCURRENT_VOICES];
	uint8_t tracks_playing;
	int8_t resp;
	
	//Wrong... this get's the total possible voices, not the voices playing..
	resp = AudioGetStatus(data, &tracks_playing);
	if (resp < 0) return resp;
	
	return tracks_playing;	
}

//return true or false
bool AudioIsWavTrigResponding() {
	//Data = Number of voices (1 byte), Number of tracks (2 bytes)
	uint8_t data[WTP_RSP_LEN_SYS_INFO-5]; //ie length of 3
	int8_t resp;
	resp = AudioGetSysInfo(data);
	
	if (resp == 0) {
		wav_trig_track_count = ((uint16_t)data[2]<<8)+data[1];
	}
	
	return (resp == 0);
}

uint16_t audio_play_sound(uint8_t track_id) {
	AudioControlTrack(WTP_CTRL_CODE_PLAY_SOLO, track_id);
	
	return AUDIO_MAX_SOUND_LENGTH_SECONDS;
}

uint16_t AudioGetTrackCount() {
	return wav_trig_track_count;
}


int8_t AudioWavTrigOn() {
	//Does not turn the amplifier on, that has to be done separately
	
	//setup serial
	/*Setup the USART 57600*/
	USART_Setup(&AUDIO_WAV_TRIG_USART_PORT, &AUDIO_WAV_TRIG_USART, 769, -6, 0); //0.04% error
	
	//turn on
	WAV_TRIG_ON();
	
	//uint8_t data[20];
	//int8_t res;
	//AudioGetVersion(data);
	//if (res < 0) return res;
	
	return 0;
}

void AudioWavTrigOff() {
	//does not turn the amplifier off, that has to be done separately
	WAV_TRIG_OFF();
	
	AUDIO_WAV_TRIG_USART.CTRLB &= ~(USART_RXEN_bm | USART_TXEN_bm);
	AUDIO_WAV_TRIG_USART_PORT.OUTCLR = (1<<2)|(1<<3);
	AUDIO_WAV_TRIG_USART_PORT.DIRCLR = (1<<2)|(1<<3);
}

void AudioHandleSecondTick() {
	if (audio_mode_time_remaining_secs) {
		audio_mode_time_remaining_secs--;
	}
	
	if (audio_mode_time_remaining_secs == 0 && audio_mode == AUDIO_MODE_SETUP) {
		audio_mode = AUDIO_MODE_PLAY;
		audio_mode_time_remaining_secs = audio_play_sound(track_to_play);
	}
	
	if (audio_mode_time_remaining_secs == 0 && audio_mode == AUDIO_MODE_PLAY) {
		AudioStop();
	}
}

void AudioMainLoopChecks() {
	if (audio_mode == AUDIO_MODE_PLAY) {
		if (AudioGetVoicePlayingCount() == 0) {
			AudioStop();
		}
	} else if (audio_mode == AUDIO_MODE_SETUP) {
		if (AudioIsWavTrigResponding() == true) {
			audio_mode = AUDIO_MODE_PLAY;
			audio_mode_time_remaining_secs = audio_play_sound(track_to_play);
			_delay_ms(500);
		}
	}
}

/**
 * \brief Plays the specified sound on the WAV trigger board.
 * This function sets up the 
 * 
 * \param sound_id
 * 
 * \return mx_err_t
 */
mx_err_t AudioPlay(uint16_t track_id) {
	if (audio_mode != AUDIO_MODE_OFF) return mx_err_ResourceBeingUsed;
	
	AMP_ON();
	AudioWavTrigOn();
	track_to_play = track_id;
	audio_mode  = AUDIO_MODE_SETUP;
	audio_mode_time_remaining_secs = AUDIO_SETUP_TIME_SECONDS;
	
	return mx_err_Success;
}

/**
 * \brief Stops currently playing audio and turns the WAV trigger and the amplifier off.
 * \return void
 */
void AudioStop() {
	audio_mode = AUDIO_MODE_OFF;
	AMP_OFF();
	AudioWavTrigOff();
	audio_mode_time_remaining_secs = 0;
	track_to_play = AUDIO_TRACK_ID_NONE;
}