//...
#include <stdint.h>
#include <string.h>

typedef enum _Iridium_err_t {
	Iridium_err_SuccessAndUDTime	=  1,
	Iridium_err_Success				=  0,
	Iridium_err_Timeout				= -1,
	Iridium_err_USBUnplugged		= -2,
	Iridium_err_InvalidData			= -3,
	Iridium_err_BadResponse			= -5,
	Iridium_err_TimeNotUpdated		= -6,
	Iridium_err_Unknown				= -20
} Iridium_err_t;

uint8_t test_iridium_on = 0;
uint8_t test_iridium_send_message_count = 0;
char test_iridium_send_data[10][340];
uint16_t test_iridium_send_lengths[10] = {0,0,0,0,0,0,0,0,0,0};

Iridium_err_t Iridium_Unit_On() {
    test_iridium_on = 1;
}

Iridium_err_t Iridium_Unit_Off() {
    test_iridium_on = 0;
}

Iridium_err_t Iridium_SendMOMessage(char* data, uint16_t len) {
    // memcpy(test_iridium_send_data[test_iridium_send_message_count], data, len); 
    // test_iridium_send_lengths[test_iridium_send_message_count] = len;
    // test_iridium_send_message_count++;
    return Iridium_err_Success;
}

Iridium_err_t Iridium_SendMO_Setup() {
    return Iridium_err_Success;
}

Iridium_err_t Iridium_SendMO_SendMessage(char* data, uint16_t len) {
    memcpy(test_iridium_send_data[test_iridium_send_message_count], data, len); 
    test_iridium_send_lengths[test_iridium_send_message_count] = len;
    test_iridium_send_message_count++;
    return Iridium_err_Success;
}

Iridium_err_t Iridium_SendMO_Finish() {
    return Iridium_err_Success;
}

