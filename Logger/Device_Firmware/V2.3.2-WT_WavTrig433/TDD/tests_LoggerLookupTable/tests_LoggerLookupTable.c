// http://eradman.com/posts/tdd-in-c.html

#define UNIT_TEST

#include <stdio.h>
#include "../minunit.h"
#include "../../Libs/LoggerLookupTable.h"
#include "../../Libs/SettingsManager.h"
#include <stdint.h>
#include "../avr/stubs/serial_flash_stub.h"

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

int test_init_new_logger() {
    ser_flash_stub_reset();
    uint8_t data[1024];

    data[0] = 0x00;
    ser_flash_stub_expect_read(0, 1, data);
    LLUT_Init();

    data[0] = 0x01;
    _assert(serial_flash_stub_checkWitten(0x00, 1, data));

    _assert(ser_flash_stub_final_checks());

    return 0;
}

int test_init_single_logger_in_table() {
    ser_flash_stub_reset();
    uint8_t data[1024];

    data[0] = 0x01;
    ser_flash_stub_expect_read(0, 1, data);
    LLUT_Init();

    _assert(ser_flash_stub_final_checks());

    return 0;
}

int test_add_loggers() {
    ser_flash_stub_reset();
    uint8_t data[32];
    uint8_t retIndex;
    LLUTStruct_t* info;

    data[0] = 0x01;
    ser_flash_stub_expect_read(0, 1, data);
    LLUT_Init();
    
    //TEST 1 - Should add new ID at index 1 (1+4*1=5)
    LLUT_FindLoggerOrAdd(0x12345678, &retIndex, &info);
    _assert_equal(retIndex, 1);
    _assert_equal(info->ID, 0x12345678);
    _assert_equal(info->flags, (1<<0));
    _assert_equal(info->flash_usage, 0);
    _assert_equal(info->bat_volt, 0);
    _assert_equal(info->is_ok, 120);
    //What should have been written
    *((uint32_t*)data) = 0x12345678;
    _assert(serial_flash_stub_checkWitten(5, 4, data));
    data[0] = 0x02;
    _assert(serial_flash_stub_checkWitten(0, 1, data));
    _assert(ser_flash_stub_final_checks());

    //is_ok shoule be reset to 120 when we use FindLoggerOrAdd!
    info->is_ok = 50;
    retIndex = 0;
    info = &data; //clear the info pointer to something invalid
    //TEST 2 - Should not add new ID - should return index 1
    LLUT_FindLoggerOrAdd(0x12345678, &retIndex, &info);
    _assert_equal(retIndex, 1);
    _assert_equal(info->ID, 0x12345678);
    _assert_equal(info->flags, (1<<0));
    _assert_equal(info->flash_usage, 0);
    _assert_equal(info->bat_volt, 0);
    _assert_equal(info->is_ok, 120);
    _assert(ser_flash_stub_final_checks());

    retIndex = 0;
    info = &data; //clear the info pointer to something invalid
    //TEST 3 - Should add new ID at index 2 (1+4*2=9)
    LLUT_FindLoggerOrAdd(0x11111111, &retIndex, &info);
    _assert_equal(retIndex, 2);
    _assert_equal(info->ID, 0x11111111);
    _assert_equal(info->flags, (1<<0));
    _assert_equal(info->flash_usage, 0);
    _assert_equal(info->bat_volt, 0);
    _assert_equal(info->is_ok, 120);
    //What should have been written
    *((uint32_t*)data) = 0x11111111;
    _assert(serial_flash_stub_checkWitten(9, 4, data));
    data[0] = 0x03;
    _assert(serial_flash_stub_checkWitten(0, 1, data));
    _assert(ser_flash_stub_final_checks());

    return 0;
}

int test_init_many_loggers() {
    ser_flash_stub_reset();
    uint8_t data[32];
    uint8_t retIndex;
    setting_LoggerId = 0x55555555;
    LLUTStruct_t* info;

    data[0] = 0x03;
    ser_flash_stub_expect_read(0, 1, data);
    *((uint32_t*)data) = 0x12345678;
    ser_flash_stub_expect_read(5, 4, data);
    *((uint32_t*)data) = 0x11111111;
    ser_flash_stub_expect_read(9, 4, data);
    LLUT_Init();

    _assert_equal(LLUT_GetCount(), 3);

    //Then check that the table values are correct
    _assert_equal(LLUT_ShortIdFromLoggerId(0x55555555, &retIndex, &info), mx_err_Success);
    _assert_equal(retIndex, 0);
    _assert_equal_hex(info->ID, 0x55555555);
    _assert_equal(info->flags, (1<<0));
    _assert_equal(info->flash_usage, 0);
    _assert_equal(info->bat_volt, 0);
    _assert_equal(info->is_ok, 0);

    _assert_equal(LLUT_ShortIdFromLoggerId(0x12345678, &retIndex, &info), mx_err_Success);
    _assert_equal(retIndex, 1);
    _assert_equal_hex(info->ID, 0x12345678);
    _assert_equal(info->flags, (1<<3));
    _assert_equal(info->flash_usage, 0);
    _assert_equal(info->bat_volt, 0);
    _assert_equal(info->is_ok, 0);

    _assert_equal(LLUT_ShortIdFromLoggerId(0x11111111, &retIndex, &info), mx_err_Success);
    _assert_equal(retIndex, 2);
    _assert_equal_hex(info->ID, 0x11111111);
    _assert_equal(info->flags, (1<<3));
    _assert_equal(info->flash_usage, 0);
    _assert_equal(info->bat_volt, 0);
    _assert_equal(info->is_ok, 0);

    _assert(ser_flash_stub_final_checks());

    return 0;
}

int test_clear_list() {
    ser_flash_stub_reset();
    uint8_t data[32];
    uint8_t retIndex;
    setting_LoggerId = 0x55555555;
    LLUTStruct_t* info;

    data[0] = 0x03;
    ser_flash_stub_expect_read(0, 1, data);
    *((uint32_t*)data) = 0x12345678;
    ser_flash_stub_expect_read(5, 4, data);
    *((uint32_t*)data) = 0x11111111;
    ser_flash_stub_expect_read(9, 4, data);
    LLUT_Init();

    _assert_equal(LLUT_GetCount(), 3);

    //Then check that the table values are correct
    _assert_equal(LLUT_ShortIdFromLoggerId(0x55555555, &retIndex, &info), mx_err_Success);
    _assert_equal(retIndex, 0);
    _assert_equal_hex(info->ID, 0x55555555);

    _assert_equal(LLUT_ShortIdFromLoggerId(0x12345678, &retIndex, &info), mx_err_Success);
    _assert_equal(retIndex, 1);
    _assert_equal_hex(info->ID, 0x12345678);

    _assert_equal(LLUT_ShortIdFromLoggerId(0x11111111, &retIndex, &info), mx_err_Success);
    _assert_equal(retIndex, 2);
    _assert_equal_hex(info->ID, 0x11111111);

    _assert(ser_flash_stub_final_checks());

    //Clear the list!
    _assert_equal(LLUT_ClearList(), mx_err_Success);

    //Check that we overwrote the index
    data[0] = 0x01;
    _assert(serial_flash_stub_checkWitten(0, 1, data));
    _assert(ser_flash_stub_final_checks());

    //Check that the count is correct
    _assert_equal(LLUT_GetCount(), 1);
    _assert(ser_flash_stub_final_checks());

    return 0;
}

int test_update_logger() {
    ser_flash_stub_reset();
    uint8_t data[32];
    uint8_t retIndex;
    setting_LoggerId = 0x55555555;
    LLUTStruct_t* info;

    data[0] = 0x03;
    ser_flash_stub_expect_read(0, 1, data);
    *((uint32_t*)data) = 0x12345678;
    ser_flash_stub_expect_read(5, 4, data);
    *((uint32_t*)data) = 0x11111111;
    ser_flash_stub_expect_read(9, 4, data);
    LLUT_Init();

    _assert_equal(LLUT_GetCount(), 3);

    //Then check that the table values are correct
    _assert_equal(LLUT_ShortIdFromLoggerId(0x55555555, &retIndex, &info), mx_err_Success);
    _assert_equal(retIndex, 0);
    _assert_equal_hex(info->ID, 0x55555555);

    _assert_equal(LLUT_ShortIdFromLoggerId(0x12345678, &retIndex, &info), mx_err_Success);
    _assert_equal(retIndex, 1);
    _assert_equal_hex(info->ID, 0x12345678);

    _assert_equal(LLUT_ShortIdFromLoggerId(0x11111111, &retIndex, &info), mx_err_Success);
    _assert_equal(retIndex, 2);
    _assert_equal_hex(info->ID, 0x11111111);

    _assert(ser_flash_stub_final_checks());

    //Do checks now
    _assert_equal(LLUT_FindLoggerOrAdd(0x12345678, &retIndex, &info), mx_err_Success);
    info->bat_volt = 13120;  //13.12V
    info->flash_usage = 10;  //3.9% usage
    info->flags = (1<<0);
    _assert_equal(LLUT_FindLoggerOrAdd(0x12345678, &retIndex, &info), mx_err_Success);
    _assert_equal_hex(info->ID, 0x12345678);
    _assert_equal(info->flags, (1<<0));
    _assert_equal(info->flash_usage, 10);
    _assert_equal(info->bat_volt, 13120);
    _assert_equal(info->is_ok, 120);


    _assert(ser_flash_stub_final_checks());

    return 0;
}

int test_decrement_isok_timers() {
    ser_flash_stub_reset();
    uint8_t data[32];
    uint8_t retIndex;
    setting_LoggerId = 0x55555555;
    LLUTStruct_t* info0;
    LLUTStruct_t* info1;
    LLUTStruct_t* info2;

    data[0] = 0x03;
    ser_flash_stub_expect_read(0, 1, data);
    *((uint32_t*)data) = 0x12345678;
    ser_flash_stub_expect_read(5, 4, data);
    *((uint32_t*)data) = 0x11111111;
    ser_flash_stub_expect_read(9, 4, data);
    LLUT_Init();

    _assert_equal(LLUT_GetCount(), 3);

    //Then check that the table values are correct
    _assert_equal(LLUT_FindLoggerOrAdd(0x55555555, &retIndex, &info0), mx_err_Success);
    _assert_equal(retIndex, 0);
    _assert_equal_hex(info0->ID, 0x55555555);

    _assert_equal(LLUT_FindLoggerOrAdd(0x12345678, &retIndex, &info1), mx_err_Success);
    _assert_equal(retIndex, 1);
    _assert_equal_hex(info1->ID, 0x12345678);

    _assert_equal(LLUT_FindLoggerOrAdd(0x11111111, &retIndex, &info2), mx_err_Success);
    _assert_equal(retIndex, 2);
    _assert_equal_hex(info2->ID, 0x11111111);

    _assert_equal(LLUT_GetCount(), 3);

    _assert(ser_flash_stub_final_checks());

    //Check that is_ok == 120 for all 3 -> FindLoggerOrAdd should have set them to 120
    _assert_equal(info0->is_ok, 120);
    _assert_equal(info1->is_ok, 120);
    _assert_equal(info2->is_ok, 120);
    
    for (int i = 119; i >= 0; --i) {
        //Decrement all
	LLUT_DecrementIsOkTimers();
        //Check that is_ok was decremented
        _assert_equal(info0->is_ok, 120); //Logger 0 (this logger) never decrements, it's always up-to-date.
        _assert_equal(info1->is_ok, i);
        _assert_equal(info2->is_ok, i);
    }
    _assert_equal(info1->is_ok, 0);
    _assert_equal(info2->is_ok, 0);
    //Shouldn't decrement past 0, just stop at 0
    LLUT_DecrementIsOkTimers();
    _assert_equal(info1->is_ok, 0);
    _assert_equal(info2->is_ok, 0);

    _assert(ser_flash_stub_final_checks());

    return 0;
}

int test_GetDataPtr_UpdateLoggerFlags() {
    ser_flash_stub_reset();
    uint8_t data[32];
    uint8_t retIndex;
    setting_LoggerId = 0x55555555;
    LLUTStruct_t* info0;
    LLUTStruct_t* info1;
    LLUTStruct_t* info2;
    RecordCount = 10000;

    data[0] = 0x03;
    ser_flash_stub_expect_read(0, 1, data);
    *((uint32_t*)data) = 0x12345678;
    ser_flash_stub_expect_read(5, 4, data);
    *((uint32_t*)data) = 0x11111111;
    ser_flash_stub_expect_read(9, 4, data);
    LLUT_Init();

    _assert_equal(LLUT_GetCount(), 3);

    //Then check that the table values are correct
    _assert_equal(LLUT_FindLoggerOrAdd(0x55555555, &retIndex, &info0), mx_err_Success);
    _assert_equal(retIndex, 0);
    _assert_equal_hex(info0->ID, 0x55555555);

    _assert_equal(LLUT_FindLoggerOrAdd(0x12345678, &retIndex, &info1), mx_err_Success);
    _assert_equal(retIndex, 1);
    _assert_equal_hex(info1->ID, 0x12345678);

    //info2 should be (1<<3) below line checks that even though we don't have a pointer to info2 yet we can 
    //get it by adding LLUTRECORD_SIZE to info1
    _assert_equal(((LLUTStruct_t*)(((uint8_t*)info1)+LLUTRECORD_SIZE))->flags, (1<<3));
    _assert_equal(LLUT_FindLoggerOrAdd(0x11111111, &retIndex, &info2), mx_err_Success);
    _assert_equal(retIndex, 2);
    _assert_equal_hex(info2->ID, 0x11111111);
    _assert_equal(info2->flags, 0x00);

    _assert_equal(LLUT_GetCount(), 3);

    _assert(ser_flash_stub_final_checks());

    LLUT_SetCurrentLoggerVoltage(12.71);
    info1->flash_usage = 120;
    info1->bat_volt = 11100;//11.1
    info1->flags = (1<<1);
    info2->flash_usage = 245;
    info2->bat_volt = 10000;//10.0
    info2->flags = (1<<2)|(1<<1);
    info2->is_ok = 0;
    LLUT_GetDataPtr();
    _assert_equal(info0->flags, (1<<0)); //isok == true
    _assert_equal(info1->flags, (1<<1)|(1<<0)); //low voltage, isok == true
    _assert_equal(info2->flags, (1<<2)|(1<<1)); //isok == false, low voltage, data storage low

    RecordCount = 0x7A0000L;
    LLUT_SetCurrentLoggerVoltage(10.71);
    info2->is_ok = 123;
    LLUT_GetDataPtr();
    _assert_equal(info0->flash_usage, 243);
    _assert_equal(info0->flags, (1<<2)|(1<<1)|(1<<0)); //isok == true, low voltage, data storage low
    _assert_equal(info2->flags, (1<<2)|(1<<1)|(1<<0)); //isok == true

    _assert(ser_flash_stub_final_checks());

    return 0;
}

int all_tests() {
    _verify(test_init_new_logger);
    _verify(test_init_single_logger_in_table);
    _verify(test_add_loggers);
    _verify(test_init_many_loggers);
    _verify(test_clear_list);
    _verify(test_update_logger);
    _verify(test_decrement_isok_timers);
    _verify(test_GetDataPtr_UpdateLoggerFlags);
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
