/* 
 * Author: Matthew Cochrane
 * Date: Nov 2015
 * Description: Forward Error Correction algorithm wrapper
 * 
 */

#include <stdint.h>

#define FEC_NONE			0
#define FEC_REED_SOLOMON	1
#define FEC_HAMMING_GOLAY	2
#define FEC_SIMPLE_PARITY	3

#define FEC_METHOD FEC_SIMPLE_PARITY

void FEC_Encode(uint8_t* data_to_encode, 
				uint16_t data_length,
				uint8_t* encoded_data,
				uint16_t* encoded_length);
				
//Returns 1 if CRC valid, 0 if CRC invalid
uint8_t FEC_Decode(uint8_t* data_to_decode, 
				   uint16_t data_length, 
				   uint8_t* decoded_data,
				   uint16_t* decoded_length);

void FEC_Decode_Vote(uint8_t* data_to_decode, uint16_t data_length,
					 uint8_t* decoded_data, uint16_t* decoded_length);