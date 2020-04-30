// http://eradman.com/posts/tdd-in-c.html

#include <stdio.h>
#include "../minunit.h"
#include "../../Libs/M_CountList.h"
#include "../../Libs/MX_SendData.h"
#include "../../Libs/SettingsManager.h"
#include "../avr/avr/io.h"
#include "../avr/stubs/count_list_stub.h"
#include "../avr/stubs/llut_stub.h"

USART_t USARTC0;
PORT_t PORTD;
char s[80];

extern RTC_Time_t Send_Data_GPRS_Next_Real;
extern uint16_t test_iridium_send_len;
extern uint8_t test_iridium_send_message_count;
extern char test_iridium_send_data[10][340];
extern uint16_t test_iridium_send_lengths[10] ;

int tests_run = 0;

void printHex(const char* data, uint16_t len) {
    uint16_t i;
    uint16_t cnt = 0;
    printf("len = %d\r\n", len);
    for ( i = 0; i < 16; i++) {
    if ((cnt+i) >= len) break;
        printf("%02X ", (uint8_t)i);
    }
    printf("\r\n");
    while (cnt < len) {
        for ( i = 0; i < 16; i++) {
	    if ((cnt+i) >= len) break;
            printf("%02X ", (uint8_t)data[cnt + i]);
        }
	/*printf("| ");
        for (i = 0; i < 16; i++) {
	    if ((cnt+i) >= len) break;
            printf("%c", (uint8_t)data[cnt + i]);
        }*/
	cnt += 16;
	printf("\r\n");
    }
}

int test_due_for_send() {
    RTC_Time_t time;
    SET_RTC_TIME_T(Send_Data_GPRS_Next, 10, 3, /*20*/16, 12, 0, 0);
    SET_RTC_TIME_T(time, 10, 3, /*20*/16, 11, 10, 10);

    _assert_equal(SendData_DueForSend(&time), false);

    SET_RTC_TIME_T(time, 10, 3, /*20*/16, 12, 10, 10);
    _assert_equal(SendData_DueForSend(&time), true);

    SET_RTC_TIME_T(time, 10, 5, /*20*/16, 11, 0, 10);
    _assert_equal(SendData_DueForSend(&time), true);
    return 0;
}

int test_reel_in_the_years() {
    RTC_Time_t time;
    SET_RTC_TIME_T(Send_Data_GPRS_Next_Real, 1, 1, /*20*/16, 0, 0, 0);
    SET_RTC_TIME_T(time, 1, 3, /*20*/16, 0, 1, 0);
    SET_RTC_TIME_T(Send_Data_GPRS_Period, 1,0,0,0,0,0); //1 day
    
    SendData_ReelInTheYears(&time);

    _assert_time_equal(Send_Data_GPRS_Next_Real, 16, 3, 2, 0, 0, 0);

    return 0;
}

int test_SendIridiumSBDMessages() {
    printf("test_SendIridiumSBDMessages\r\n");
    CountListStruct_t* item = 0;                                                                                         
    RTC_Time_t time;                                                                                                     
    SET_RTC_TIME_T(time, 10, 3, /*20*/16, 12, 22, 10);                                                                   
    RecordCount = 1000;
    setting_LogsSentViaIridium = 980;
    setting_IridiumBytesSentThisMonth = 0;
    setting_IridiumMonthlyByteLimit = 30*1024;
    test_iridium_send_message_count = 0;

    count_list_stub_set_data_set(1);
    mCountList_Init();
    llut_stub_set_data_set(1);
    LLUT_Init();
    SendData_SendIridiumSBDMessages(&time);

    _assert_equal(test_iridium_send_message_count, 2);

    printHex(test_iridium_send_data[0], test_iridium_send_lengths[0]);

    //message_type == 1
    _assert_equal(*((uint8_t*)&test_iridium_send_data[0][0]), IRIDIUM_MESSAGE_TYPE_PICKUP_STATS);
    //number of detections since last tx == 2
    _assert_equal(*((uint16_t*)&test_iridium_send_data[0][1]), 20);
    //number of pickups in message stats == 2
    _assert_equal(*((uint16_t*)&test_iridium_send_data[0][3]), 11);
    //last tag id detected == 0x9999
    _assert_equal_hex(*((uint32_t*)&test_iridium_send_data[0][5]), 0x9999);
    //Number of Messages to be sent == 1
    _assert_equal(*((uint8_t*)&test_iridium_send_data[0][9]), 1);
    //Message number of total == 1
    _assert_equal(*((uint8_t*)&test_iridium_send_data[0][10]), 1);

    //First Tag..
    //tag id == 0x1234
    _assert_equal_hex(*((uint32_t*)&test_iridium_send_data[0][11]), 0x1234);
    //tag pickup count == 10
    _assert_equal(*((uint16_t*)&test_iridium_send_data[0][15]), 10);
    //first pickup time == 10
    _assert_equal(*((uint8_t*)&test_iridium_send_data[0][17]), 10);
    //last pickup time == 20
    _assert_equal(*((uint8_t*)&test_iridium_send_data[0][18]), 20);
    //rssi min == 20
    _assert_equal(*((int8_t*)&test_iridium_send_data[0][19]), -102);
    //rssi Avg == 20
    _assert_equal(*((int8_t*)&test_iridium_send_data[0][20]), -99);
    //rssi max == 20
    _assert_equal(*((int8_t*)&test_iridium_send_data[0][21]), -85);
    //pickup time mask == 0x0005
    _assert_equal_hex(*((uint16_t*)&test_iridium_send_data[0][22]), 0x0005);

    //Second Tag..
    //tag id == 0x1234
    _assert_equal_hex(*((uint32_t*)&test_iridium_send_data[0][24]), 0x5555);
    //tag pickup count == 10
    _assert_equal(*((uint16_t*)&test_iridium_send_data[0][28]), 1);
    //first pickup time == 10
    _assert_equal(*((uint8_t*)&test_iridium_send_data[0][30]), 2);
    //last pickup time == 20
    _assert_equal(*((uint8_t*)&test_iridium_send_data[0][31]), 2);
    //rssi min == 20
    _assert_equal(*((int8_t*)&test_iridium_send_data[0][32]), -105);
    //rssi Avg == 20
    _assert_equal(*((int8_t*)&test_iridium_send_data[0][33]), -105);
    //rssi max == 20
    _assert_equal(*((int8_t*)&test_iridium_send_data[0][34]), -105);
    //pickup time mask == 0x0005
    _assert_equal_hex(*((uint16_t*)&test_iridium_send_data[0][35]), 0x0001);

    printHex(test_iridium_send_data[1], test_iridium_send_lengths[1]);

    //message_type == 2
    _assert_equal(*((uint8_t*)&test_iridium_send_data[1][0]), IRIDIUM_MESSAGE_TYPE_NETWORK_STATS);
    //number of loggers in group
    _assert_equal(*((uint8_t*)&test_iridium_send_data[1][1]), 2);
    //Number of Messages to be sent == 1
    _assert_equal(*((uint8_t*)&test_iridium_send_data[1][2]), 1);
    //Message number of total == 1
    _assert_equal(*((uint8_t*)&test_iridium_send_data[1][3]), 1);

    //First Tag..
    //logger id == 0x1234
    _assert_equal_hex(*((uint32_t*)&test_iridium_send_data[1][4]), 0x1234);
    //bat_volt = 12.11V
    _assert_equal(*((uint16_t*)&test_iridium_send_data[1][8]), 12110);
    //flash_usage = 81/255
    _assert_equal(*((uint8_t*)&test_iridium_send_data[1][10]), 81);
    //flags = ok
    _assert_equal(*((uint8_t*)&test_iridium_send_data[1][11]), 0b00000001);

    //Second Tag..
    //logger id == 0x5555
    _assert_equal_hex(*((uint32_t*)&test_iridium_send_data[1][12]), 0x5555);
    //bat_volt = 12.11V
    _assert_equal(*((uint16_t*)&test_iridium_send_data[1][16]), 12110);
    //flash_usage = 81/255
    _assert_equal(*((uint8_t*)&test_iridium_send_data[1][18]), 81);
    //flags = ok
    _assert_equal(*((uint8_t*)&test_iridium_send_data[1][19]), 0b00000001);

    _assert_equal(setting_IridiumBytesSentThisMonth, 37+30);

    return 0;
}

int test_SendIridiumSBDMessages_Multi() {
    CountListStruct_t* item = 0;                                                                                         
    RTC_Time_t time;                                                                                                     
    SET_RTC_TIME_T(time, 10, 3, /*20*/16, 12, 22, 10);                                                                   
    RecordCount = 100000L;
    setting_LogsSentViaIridium = 1000;
    setting_IridiumBytesSentThisMonth = 0;
    setting_IridiumMonthlyByteLimit = 30*1024;
    test_iridium_send_message_count = 0;

    count_list_stub_set_data_set(2);
    mCountList_Init();
    llut_stub_set_data_set(2);
    LLUT_Init();
    SendData_SendIridiumSBDMessages(&time);

    _assert_equal(test_iridium_send_message_count, 3);
    //FIRST SENT MESSAGE
    printHex(test_iridium_send_data[0], test_iridium_send_lengths[0]);
    //message_type == 0 
    _assert_equal(*((uint8_t*)&test_iridium_send_data[0][0]), IRIDIUM_MESSAGE_TYPE_PICKUP_STATS);
    //number of detections since last tx == 2
    _assert_equal(*((uint16_t*)&test_iridium_send_data[0][1]), 65535);
    //number of pickups in message stats == 2
    _assert_equal(*((uint16_t*)&test_iridium_send_data[0][3]), 500);
    //last tag id detected == 0x9999
    _assert_equal_hex(*((uint32_t*)&test_iridium_send_data[0][5]), 0x9999);
    //Number of Messages to be sent == 1
    _assert_equal(*((uint8_t*)&test_iridium_send_data[0][9]), 2);
    //Message number of total == 1
    _assert_equal(*((uint8_t*)&test_iridium_send_data[0][10]), 1);

    //First Message Tags
    for (uint32_t i = 0; i < 25; ++i) {
        //tag id == 0x1234
        _assert_equal_hex(*((uint32_t*)&test_iridium_send_data[0][11+i*13]), (uint32_t)i);
        //tag pickup count == 10
        _assert_equal(*((uint16_t*)&test_iridium_send_data[0][15+i*13]), 10);
        //first pickup time == 10
        _assert_equal(*((uint8_t*)&test_iridium_send_data[0][17+i*13]), 10);
        //last pickup time == 20
        _assert_equal(*((uint8_t*)&test_iridium_send_data[0][18+i*13]), 20);
        //rssi min == 20
        _assert_equal(*((int8_t*)&test_iridium_send_data[0][19+i*13]), -102);
        //rssi Avg == 20
        _assert_equal(*((int8_t*)&test_iridium_send_data[0][20+i*13]), -99);
        //rssi max == 20
        _assert_equal(*((int8_t*)&test_iridium_send_data[0][21+i*13]), -85);
        //pickup time mask == 0x0005
        _assert_equal_hex(*((uint16_t*)&test_iridium_send_data[0][22+i*13]), 0x0005);
    }

    //SECOND SENT MESSAGE
    printHex(test_iridium_send_data[1], test_iridium_send_lengths[1]);
    //message_type == 0 
    _assert_equal(*((uint8_t*)&test_iridium_send_data[1][0]), IRIDIUM_MESSAGE_TYPE_PICKUP_STATS);
    //number of detections since last tx == 2
    _assert_equal(*((uint16_t*)&test_iridium_send_data[0][1]), 65535);
    //last tag id detected == 0x9999
    _assert_equal_hex(*((uint32_t*)&test_iridium_send_data[1][5]), 0x9999);
    //Number of Messages to be sent == 1
    _assert_equal(*((uint8_t*)&test_iridium_send_data[1][9]), 2);
    //Message number of total == 2 ***************************************Different from above..
    _assert_equal(*((uint8_t*)&test_iridium_send_data[1][10]), 2);

    //First Message Tags
    for (uint32_t i = 0; i < 25; ++i) {
        //tag id == 0x1234
        _assert_equal_hex(*((uint32_t*)&test_iridium_send_data[1][11+i*13]), (uint32_t)i+25);
        //tag pickup count == 10
        _assert_equal(*((uint16_t*)&test_iridium_send_data[1][15+i*13]), 10);
        //first pickup time == 10
        _assert_equal(*((uint8_t*)&test_iridium_send_data[1][17+i*13]), 10);
        //last pickup time == 20
        _assert_equal(*((uint8_t*)&test_iridium_send_data[1][18+i*13]), 20);
        //rssi min == 20
        _assert_equal(*((int8_t*)&test_iridium_send_data[1][19+i*13]), -102);
        //rssi Avg == 20
        _assert_equal(*((int8_t*)&test_iridium_send_data[1][20+i*13]), -99);
        //rssi max == 20
        _assert_equal(*((int8_t*)&test_iridium_send_data[1][21+i*13]), -85);
        //pickup time mask == 0x0005
        _assert_equal_hex(*((uint16_t*)&test_iridium_send_data[1][22+i*13]), 0x0005);
    }

    //Network Status Messages
    printHex(test_iridium_send_data[2], test_iridium_send_lengths[2]);

    //message_type == 2
    _assert_equal(*((uint8_t*)&test_iridium_send_data[2][0]), IRIDIUM_MESSAGE_TYPE_NETWORK_STATS);
    //number of loggers in group
    _assert_equal(*((uint8_t*)&test_iridium_send_data[2][1]), 42);
    //Number of Messages to be sent == 1
    _assert_equal(*((uint8_t*)&test_iridium_send_data[2][2]), 1);
    //Message number of total == 1
    _assert_equal(*((uint8_t*)&test_iridium_send_data[2][3]), 1);

    //First Tag..
    for (uint32_t i = 0; i < 41; ++i) {
        //logger id == 0x1234
        _assert_equal_hex(*((uint32_t*)&test_iridium_send_data[2][4+i*8]), (uint32_t)i);
        //bat_volt = 12.11V
        _assert_equal(*((uint16_t*)&test_iridium_send_data[2][8+i*8]), 12110);
        //flash_usage = 81/255
        _assert_equal(*((uint8_t*)&test_iridium_send_data[2][10+i*8]), 81);
        //flags = ok
        _assert_equal(*((uint8_t*)&test_iridium_send_data[2][11+i*8]), 0b00000001);
    }

    _assert_equal(setting_IridiumBytesSentThisMonth, 672 + 340);

    return 0;
}


int test_SendIridiumSBDMessages_MultiOneExtra() {
    //The last message should have a single tag in it so we have:
    //message 1 -> 25 tags
    //message 2 -> 25 tags
    //message 3 -> 1 tag
    CountListStruct_t* item = 0;                                                                                         
    RTC_Time_t time;                                                                                                     
    SET_RTC_TIME_T(time, 10, 3, /*20*/16, 12, 22, 10);                                                                   
    RecordCount = 1000;
    setting_LogsSentViaIridium = 998;
    setting_IridiumBytesSentThisMonth = 0;
    setting_IridiumMonthlyByteLimit = 30*1024;
    test_iridium_send_message_count = 0;

    count_list_stub_set_data_set(3);
    mCountList_Init();
    llut_stub_set_data_set(3);
    LLUT_Init();
    SendData_SendIridiumSBDMessages(&time);

    _assert_equal(test_iridium_send_message_count, 5);
    //FIRST SENT MESSAGE
    printHex(test_iridium_send_data[0], test_iridium_send_lengths[0]);
    //message_type == 0 
    _assert_equal(*((uint8_t*)&test_iridium_send_data[0][0]), IRIDIUM_MESSAGE_TYPE_PICKUP_STATS);
    //number of detections since last tx == 2
    _assert_equal(*((uint16_t*)&test_iridium_send_data[0][1]), 2);
    //number of pickups in message stats == 2
    _assert_equal(*((uint16_t*)&test_iridium_send_data[0][3]), 65535);
    //last tag id detected == 0x9999
    _assert_equal_hex(*((uint32_t*)&test_iridium_send_data[0][5]), 0x9999);
    //Number of Messages to be sent == 1
    _assert_equal(*((uint8_t*)&test_iridium_send_data[0][9]), 3);
    //Message number of total == 1
    _assert_equal(*((uint8_t*)&test_iridium_send_data[0][10]), 1);

    //First Message Tags
    for (uint32_t i = 0; i < 25; ++i) {
        //tag id == 0x1234
        _assert_equal_hex(*((uint32_t*)&test_iridium_send_data[0][11+i*13]), (uint32_t)i);
        //tag pickup count == 10
        _assert_equal(*((uint16_t*)&test_iridium_send_data[0][15+i*13]), 10);
        //first pickup time == 10
        _assert_equal(*((uint8_t*)&test_iridium_send_data[0][17+i*13]), 10);
        //last pickup time == 20
        _assert_equal(*((uint8_t*)&test_iridium_send_data[0][18+i*13]), 20);
        //rssi min == 20
        _assert_equal(*((int8_t*)&test_iridium_send_data[0][19+i*13]), -102);
        //rssi Avg == 20
        _assert_equal(*((int8_t*)&test_iridium_send_data[0][20+i*13]), -99);
        //rssi max == 20
        _assert_equal(*((int8_t*)&test_iridium_send_data[0][21+i*13]), -85);
        //pickup time mask == 0x0005
        _assert_equal_hex(*((uint16_t*)&test_iridium_send_data[0][22+i*13]), 0x0005);
    }

    //SECOND SENT MESSAGE
    printHex(test_iridium_send_data[1], test_iridium_send_lengths[1]);
    //message_type == 0 
    _assert_equal(*((uint8_t*)&test_iridium_send_data[1][0]), IRIDIUM_MESSAGE_TYPE_PICKUP_STATS);
    //number of detections since last tx == 2
    _assert_equal(*((uint16_t*)&test_iridium_send_data[0][1]), 2);
    //number of pickups in message stats == 2
    _assert_equal(*((uint16_t*)&test_iridium_send_data[0][3]), 65535);
    //last tag id detected == 0x9999
    _assert_equal_hex(*((uint32_t*)&test_iridium_send_data[1][5]), 0x9999);
    //Number of Messages to be sent == 1
    _assert_equal(*((uint8_t*)&test_iridium_send_data[1][9]), 3);
    //Message number of total == 2 ***************************************Different from above..
    _assert_equal(*((uint8_t*)&test_iridium_send_data[1][10]), 2);

    //First Message Tags
    for (uint32_t i = 0; i < 25; ++i) {
        //tag id == 0x1234
        _assert_equal_hex(*((uint32_t*)&test_iridium_send_data[1][11+i*13]), (uint32_t)i+25);
        //tag pickup count == 10
        _assert_equal(*((uint16_t*)&test_iridium_send_data[1][15+i*13]), 10);
        //first pickup time == 10
        _assert_equal(*((uint8_t*)&test_iridium_send_data[1][17+i*13]), 10);
        //last pickup time == 20
        _assert_equal(*((uint8_t*)&test_iridium_send_data[1][18+i*13]), 20);
        //rssi min == 20
        _assert_equal(*((int8_t*)&test_iridium_send_data[1][19+i*13]), -102);
        //rssi Avg == 20
        _assert_equal(*((int8_t*)&test_iridium_send_data[1][20+i*13]), -99);
        //rssi max == 20
        _assert_equal(*((int8_t*)&test_iridium_send_data[1][21+i*13]), -85);
        //pickup time mask == 0x0005
        _assert_equal_hex(*((uint16_t*)&test_iridium_send_data[1][22+i*13]), 0x0005);
    }

    //THIRD SENT MESSAGE
    printHex(test_iridium_send_data[2], test_iridium_send_lengths[2]);
    //message_type == 0 
    _assert_equal(*((uint8_t*)&test_iridium_send_data[2][0]), IRIDIUM_MESSAGE_TYPE_PICKUP_STATS);
    //number of detections since last tx == 2
    _assert_equal(*((uint16_t*)&test_iridium_send_data[0][1]), 2);
    //number of pickups in message stats == 2
    _assert_equal(*((uint16_t*)&test_iridium_send_data[0][3]), 65535);
    //last tag id detected == 0x9999
    _assert_equal_hex(*((uint32_t*)&test_iridium_send_data[2][5]), 0x9999);
    //Number of Messages to be sent == 1
    _assert_equal(*((uint8_t*)&test_iridium_send_data[2][9]), 3);
    //Message number of total == 3 ***************************************Different from above..
    _assert_equal(*((uint8_t*)&test_iridium_send_data[2][10]), 3);

    //First Message Tags
    for (uint32_t i = 0; i < 1; ++i) {
        //tag id == 0x1234
        _assert_equal_hex(*((uint32_t*)&test_iridium_send_data[2][11+i*13]), (uint32_t)i+50);
        //tag pickup count == 10
        _assert_equal(*((uint16_t*)&test_iridium_send_data[2][15+i*13]), 10);
        //first pickup time == 10
        _assert_equal(*((uint8_t*)&test_iridium_send_data[2][17+i*13]), 10);
        //last pickup time == 20
        _assert_equal(*((uint8_t*)&test_iridium_send_data[2][18+i*13]), 20);
        //rssi min == 20
        _assert_equal(*((int8_t*)&test_iridium_send_data[2][19+i*13]), -102);
        //rssi Avg == 20
        _assert_equal(*((int8_t*)&test_iridium_send_data[2][20+i*13]), -99);
        //rssi max == 20
        _assert_equal(*((int8_t*)&test_iridium_send_data[2][21+i*13]), -85);
        //pickup time mask == 0x0005
        _assert_equal_hex(*((uint16_t*)&test_iridium_send_data[2][22+i*13]), 0x0005);
    }

    //Network Status Messages
    printHex(test_iridium_send_data[3], test_iridium_send_lengths[3]);

    //message_type == 2
    _assert_equal(*((uint8_t*)&test_iridium_send_data[3][0]), IRIDIUM_MESSAGE_TYPE_NETWORK_STATS);
    //number of loggers in group
    _assert_equal(*((uint8_t*)&test_iridium_send_data[3][1]), 43);
    //Number of Messages to be sent == 1
    _assert_equal(*((uint8_t*)&test_iridium_send_data[3][2]), 2);
    //Message number of total == 1
    _assert_equal(*((uint8_t*)&test_iridium_send_data[3][3]), 1);

    //First Tag..
    for (uint32_t i = 0; i < 41; ++i) {
        //logger id == i
        _assert_equal_hex(*((uint32_t*)&test_iridium_send_data[3][4+i*8]), (uint32_t)i);
        //bat_volt = 12.11V
        _assert_equal(*((uint16_t*)&test_iridium_send_data[3][8+i*8]), 12110);
        //flash_usage = 81/255
        _assert_equal(*((uint8_t*)&test_iridium_send_data[3][10+i*8]), 81);
        //flags = ok
        _assert_equal(*((uint8_t*)&test_iridium_send_data[3][11+i*8]), 0b00000001);
    }

    printHex(test_iridium_send_data[4], test_iridium_send_lengths[4]);

    //message_type == 2
    _assert_equal(*((uint8_t*)&test_iridium_send_data[4][0]), IRIDIUM_MESSAGE_TYPE_NETWORK_STATS);
    //number of loggers in group
    _assert_equal(*((uint8_t*)&test_iridium_send_data[4][1]), 43);
    //Number of Messages to be sent == 1
    _assert_equal(*((uint8_t*)&test_iridium_send_data[4][2]), 2);
    //Message number of total == 1
    _assert_equal(*((uint8_t*)&test_iridium_send_data[4][3]), 2);

    //First Tag..
    //logger id == 43
    _assert_equal_hex(*((uint32_t*)&test_iridium_send_data[4][4]), (uint32_t)42);
    //bat_volt = 12.11V
    _assert_equal(*((uint16_t*)&test_iridium_send_data[4][8]), 12110);
    //flash_usage = 81/255
    _assert_equal(*((uint8_t*)&test_iridium_send_data[4][10]), 81);
    //flags = ok
    _assert_equal(*((uint8_t*)&test_iridium_send_data[4][11]), 0b00000001);

    //bytes (for pickup stats) should be -> 672+11+13 but 11+13 < 30 which is the minimum so it should be 672+30 = 702
    //bytes for logger stats is 340 for first message and 2nd message is < 30 bytes so add 30 bytes
    _assert_equal(setting_IridiumBytesSentThisMonth, 702 + (340 + 30));

    return 0;
}

int test_checkMonthDataRolledOverHasnt() {
    RTC_Time_t time;
    SET_RTC_TIME_T(setting_IridiumMonthNextReset, 20, 1, /*20*/16, 0, 0, 0);
    SET_RTC_TIME_T(time, 1, 1, /*20*/16, 0, 1, 0);
    //SET_RTC_TIME_T(Send_Data_GPRS_Period, 1,0,0,0,0,0); //1 day

    _assert_equal(SendData_CheckMonthDataRolledOver(&time), false);

    return 0;
}

int test_checkMonthDataRolledOverHas() {
    RTC_Time_t time;
    SET_RTC_TIME_T(setting_IridiumMonthNextReset, 20, 1, /*20*/16, 0, 0, 0);
    SET_RTC_TIME_T(time, 21, 1, /*20*/16, 0, 1, 0);
    //SET_RTC_TIME_T(next_, 1,0,0,0,0,0); //1 day

    _assert_equal(SendData_CheckMonthDataRolledOver(&time), true);

    _assert_time_equal(setting_IridiumMonthNextReset, 16, 2, 20, 0, 0, 0);

    return 0;
}

int test_checkMonthDataRolledOverHasLong() {
    RTC_Time_t time;
    SET_RTC_TIME_T(setting_IridiumMonthNextReset, 20, 1, /*20*/16, 0, 0, 0);
    SET_RTC_TIME_T(time, 21, 1, /*20*/20, 0, 1, 0);
    //SET_RTC_TIME_T(next_, 1,0,0,0,0,0); //1 day

    _assert_equal(SendData_CheckMonthDataRolledOver(&time), true);

    _assert_time_equal(setting_IridiumMonthNextReset, /*20*/20, 2, 20, 0, 0, 0);

    return 0;
}

int test_checkMonthDataRolledOverBadMonths() {
    RTC_Time_t time;
    SET_RTC_TIME_T(setting_IridiumMonthNextReset, 31, 8, /*20*/16, 0, 0, 0);
    SET_RTC_TIME_T(time, 31, 8, /*20*/16, 0, 1, 0);

    _assert_equal(SendData_CheckMonthDataRolledOver(&time), true);

    _assert_time_equal(setting_IridiumMonthNextReset, /*20*/16, 9, 31, 0, 0, 0); //not a valid date but it's actually what we want!

    SET_RTC_TIME_T(time, 30, 9, /*20*/16, 0, 1, 0);
    _assert_equal(SendData_CheckMonthDataRolledOver(&time), false);

    SET_RTC_TIME_T(time, 1, 10, /*20*/16, 0, 1, 0);
    _assert_equal(SendData_CheckMonthDataRolledOver(&time), true);

    _assert_time_equal(setting_IridiumMonthNextReset, /*20*/16, 10, 31, 0, 0, 0);

    return 0;
}

int all_tests() {
    _verify(test_due_for_send);
    _verify(test_reel_in_the_years);
    _verify(test_SendIridiumSBDMessages);
    _verify(test_SendIridiumSBDMessages_Multi);
    _verify(test_SendIridiumSBDMessages_MultiOneExtra);
    _verify(test_checkMonthDataRolledOverHasnt);
    _verify(test_checkMonthDataRolledOverHas);
    _verify(test_checkMonthDataRolledOverHasLong);
    //_verify(test_checkMonthDataRolledOverBadMonths);
    //_verify();

    return 0;
}

int main(int argc, char **argv) {
    int result = all_tests();
    if (result == 0) {
        printf("ALL TESTS PASSED\n");
    }
    printf("Tests run: %d\n", tests_run);

    return result != 0;
}
