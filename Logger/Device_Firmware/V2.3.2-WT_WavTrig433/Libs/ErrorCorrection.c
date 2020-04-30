/* 
 * Author: Matthew Cochrane
 * Date: Nov 2015
 * Description: Forward Error Correction algorithm wrapper
 * 
 */

#include "ErrorCorrection.h"
#include <string.h>

#define REPITITIONS 3

uint16_t checksum(uint8_t* data, uint16_t len)
{
	uint16_t sum = 0;
	
	for (uint16_t i = 0; i < len; ++i)
	{
		sum += data[i];
	}
	
	return sum;
}

void FEC_Encode(uint8_t* data_to_encode, uint16_t data_length,
				uint8_t* encoded_data, uint16_t* encoded_length)
{
	//Just repeat the message N times (with checksum)	
	uint16_t csum = checksum(data_to_encode, data_length);
	
	for (uint8_t i = 0; i < REPITITIONS; ++i)
	{
		memcpy(encoded_data+(data_length+2)*i, data_to_encode, data_length);
		memcpy(encoded_data+(data_length+2)*i+data_length, &csum, 2);
	}
	
	//The plus 2 is for the checksum
	*encoded_length = (data_length+2)*REPITITIONS;
}

uint8_t FEC_Decode(uint8_t* data_to_decode, uint16_t data_length,
				   uint8_t* decoded_data, uint16_t* decoded_length)
{
	uint16_t msg_with_chksum_length = data_length/REPITITIONS;
	uint8_t voted_msg[msg_with_chksum_length];
	uint8_t ret_value = 0;
	if (msg_with_chksum_length*REPITITIONS != data_length) return 0; //Length error, not a multiple of REPITITONS

	FEC_Decode_Vote(data_to_decode, data_length, voted_msg, decoded_length);
	*decoded_length -= 2;  //Minus checksum length 
	
	uint16_t calculated_checksum = checksum(voted_msg, *decoded_length);
	uint16_t received_checksum = *((uint16_t*)(&voted_msg[*decoded_length]));
	
	if (calculated_checksum == received_checksum) {
		ret_value = 1;
	} else {
		ret_value = 0;
	}
	
	return ret_value;
}

void FEC_Decode_Vote(uint8_t* data_to_decode, uint16_t data_length,
					 uint8_t* decoded_data, uint16_t* decoded_length)
{
	uint8_t votes[REPITITIONS];
	uint16_t msg_length = data_length/REPITITIONS;
	*decoded_length = msg_length;
	if (msg_length*REPITITIONS != data_length) return; //Length error, not a multiple of REPITITONS
	
	for (uint16_t i = 0; i < msg_length; ++i)
	{
		for (uint8_t j = 0; j < REPITITIONS; ++j)
		{
			votes[j] = data_to_decode[i + msg_length*j];
		}
		#if REPITITIONS == 3
			decoded_data[i] = (votes[0] & (votes[1] | votes[2])) | (votes[1] & votes[2]);
		#else
			#error Repitition voting method not defined
		#endif
	}
}

// void test() 
// {
// 	uint8_t input[20];
// 	uint8_t encoded_data[128];
// 	uint16_t encoded_len;
// 	uint8_t decoded_data[40];
// 	uint16_t decoded_len;
// 	uint8_t result;
// 		
// 	FEC_Encode(input, 20, encoded_data, &encoded_len);
// 		
// 	//Corrupt some data (simulate noisy channel)
// 	for (uint16_t i = 1; i < encoded_len; i+=2)
// 	{
// 		encoded_data[i] = 0xFF;
// 	}
// 		
// 	result = FEC_Decode(encoded_data, encoded_len, decoded_data, &decoded_len);
// 		
// 	USART_printf_P(&USARTC0, "decoded: %u\r\n", result);
// 	_delay_ms(100);
// }