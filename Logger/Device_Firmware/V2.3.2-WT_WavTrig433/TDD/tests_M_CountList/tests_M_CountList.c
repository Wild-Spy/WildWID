// http://eradman.com/posts/tdd-in-c.html

#include <stdio.h>
#include "../minunit.h"
#include "../../Libs/M_CountList.h"

int tests_run = 0;

int test_add_items() {
    CountListStruct_t* item = 0;
    RTC_Time_t time;
    SET_RTC_TIME_T(time, 10, 3, /*20*/16, 12, 22, 10);

    mCountList_Init();
    _assert(mCountList_GetCount() == 0);
    mCountList_AddItem(0x1234, &time, 0);
    mCountList_ItemWithID(0x1234, &item);
    _assert(item->count == 1);
    _assert(mCountList_GetCount() == 1);
    mCountList_AddItem(0x1234, &time, 0);
    item = 0;
    mCountList_ItemWithID(0x1234, &item);
    _assert(item->count == 2);
    _assert(mCountList_GetCount() == 1);
    mCountList_AddItem(0x5555, &time, 0);
    item = 0;
    mCountList_ItemWithID(0x5555, &item);
    _assert(item->count == 1);
    _assert(mCountList_GetCount() == 2);

    return 0;
}

int test_remove_items() {
    CountListStruct_t* item = 0;
    RTC_Time_t time;
    SET_RTC_TIME_T(time, 10, 3, /*20*/16, 12, 22, 10);

    mCountList_Init();
    mCountList_AddItem(0x1234, &time, 0);
    mCountList_AddItem(0x5555, &time, 0);
    mCountList_AddItem(0x4444, &time, 0);
    mCountList_AddItem(0x8888, &time, 0);
    _assert(mCountList_GetCount() == 4);
    mCountList_RemoveItemWithID(0x4444);
    _assert(mCountList_ItemWithID(0x4444, &item) == mx_err_NotFound);
    _assert(mCountList_GetCount() == 3);

    return 0;
}

int test_add_item_start_and_end_time() {
    CountListStruct_t* item = 0;
    RTC_Time_t time;
    SET_RTC_TIME_T(time, 10, 3, /*20*/16, 12, 22, 10);

    mCountList_Init();
    mCountList_AddItem(0x1234, &time, 0);
    mCountList_ItemWithID(0x1234, &item);
    //Time is number of 15 minute blocks since midnight)
    //12*4 = 48 + 1 (floor(22/15) = 1) = 49
    _assert_equal(item->firstTime, 49);
    _assert_equal(item->lastTime, 49);

    SET_RTC_TIME_T(time, 10, 3, /*20*/16, 15, 30, 10);
    mCountList_AddItem(0x5555, &time, 0);
    mCountList_ItemWithID(0x5555, &item);
    //Time is number of 15 minute blocks since midnight)
    //15*4 = 60 + 1 (floor(30/15) = 2) = 62 
    _assert_equal(item->lastTime, 62);

    return 0;
}

int test_update_item_doesnt_change_start_time() {
    CountListStruct_t* item = 0;
    RTC_Time_t time;
    SET_RTC_TIME_T(time, 10, 3, /*20*/16, 12, 22, 10);

    mCountList_Init();
    mCountList_AddItem(0x1234, &time, 0);
    mCountList_ItemWithID(0x1234, &item);
    //Time is number of 15 minute blocks since midnight)
    //12*4 = 48 + 1 (floor(22/15) = 1) = 49
    _assert_equal(item->firstTime, 49);

    SET_RTC_TIME_T(time, 10, 3, /*20*/16, 15, 30, 10);
    mCountList_AddItem(0x1234, &time, 0);
    mCountList_ItemWithID(0x1234, &item);
    //Should not affect firstTime, only lastTime
    _assert_equal(item->firstTime, 49);

    return 0;
}

int test_update_item_update_end_time() {
    CountListStruct_t* item = 0;
    RTC_Time_t time;
    SET_RTC_TIME_T(time, 10, 3, /*20*/16, 12, 22, 10);

    mCountList_Init();
    mCountList_AddItem(0x1234, &time, 0);
    mCountList_ItemWithID(0x1234, &item);
    //Time is number of 15 minute blocks since midnight)
    //12*4 = 48 + 1 (floor(22/15) = 1) = 49
    _assert_equal(item->lastTime, 49);

    SET_RTC_TIME_T(time, 10, 3, /*20*/16, 15, 30, 10);
    mCountList_AddItem(0x1234, &time, 0);
    mCountList_ItemWithID(0x1234, &item);
    //Time is number of 15 minute blocks since midnight)
    //15*4 = 60 + 1 (floor(30/15) = 2) = 62 
    _assert_equal(item->lastTime, 62);

    return 0;
}

int test_update_item_update_min_max_rssi() {
    CountListStruct_t* item = 0;
    RTC_Time_t time;
    SET_RTC_TIME_T(time, 10, 3, /*20*/16, 12, 22, 10);

    mCountList_Init();
    mCountList_AddItem(0x1234, &time, -30);
    mCountList_ItemWithID(0x1234, &item);
    _assert_equal(item->rssiMin, -30);
    _assert_equal(item->rssiMax, -30);

    mCountList_AddItem(0x1234, &time, -50);
    mCountList_ItemWithID(0x1234, &item);
    _assert_equal(item->rssiMin, -50);
    _assert_equal(item->rssiMax, -30);

    mCountList_AddItem(0x1234, &time, -20);
    mCountList_ItemWithID(0x1234, &item);
    _assert_equal(item->rssiMin, -50);
    _assert_equal(item->rssiMax, -20);

    mCountList_AddItem(0x1234, &time, -45);
    mCountList_ItemWithID(0x1234, &item);
    _assert_equal(item->rssiMin, -50);
    _assert_equal(item->rssiMax, -20);

    return 0;
}

int test_update_item_update_avg_rssi() {
    CountListStruct_t* item = 0;
    RTC_Time_t time;
    SET_RTC_TIME_T(time, 10, 3, /*20*/16, 12, 22, 10);

    //rssiAvg should be the running average after each update
    mCountList_Init();
    mCountList_AddItem(0x1234, &time, -30);
    mCountList_ItemWithID(0x1234, &item);
    _assert_equal(item->rssiAvg, -30);

    mCountList_AddItem(0x1234, &time, -50);
    mCountList_ItemWithID(0x1234, &item);
    _assert_equal(item->rssiAvg, -40);

    mCountList_AddItem(0x1234, &time, -20);
    mCountList_ItemWithID(0x1234, &item);
    _assert_equal(item->rssiAvg, -33);

    mCountList_AddItem(0x1234, &time, -45);
    mCountList_ItemWithID(0x1234, &item);
    _assert_equal(item->rssiAvg, -36);

    mCountList_AddItem(0x1234, &time, -82);
    mCountList_ItemWithID(0x1234, &item);
    _assert_equal(item->rssiAvg, -45);

    return 0;
}

int test_update_pickup_time_mask() {
    CountListStruct_t* item = 0;
    RTC_Time_t time;
    SET_RTC_TIME_T(time, 10, 3, /*20*/16, 0, 0, 0);

    //Each bit of pickupTimeMask represents a time zone where there was a pickup
    //Each zone is 90 minutes long.  bit 0 is zone 0 which is from midnight to 1:30am
    //bit 1 is zone 1 which is from 1:30am to 3:00am, etc.

    mCountList_Init();
    mCountList_AddItem(0x1234, &time, 0);
    _assert(mCountList_ItemWithID(0x1234, &item) == mx_err_Success);
    _assert_equal_hex(item->pickupTimeMask, 0x0001);

    //Add the same time zone again - mask shouldn't change
    SET_RTC_TIME_T(time, 10, 3, /*20*/16, 0, 0, 30);
    mCountList_AddItem(0x1234, &time, 0);
    _assert(mCountList_ItemWithID(0x1234, &item) == mx_err_Success);
    _assert_equal_hex(item->pickupTimeMask, 0x0001);

    //Tag in a different time zone (zone 1)
    SET_RTC_TIME_T(time, 10, 3, /*20*/16, 2, 30, 0);
    mCountList_AddItem(0x1234, &time, 0);
    _assert(mCountList_ItemWithID(0x1234, &item) == mx_err_Success);
    _assert_equal_hex(item->pickupTimeMask, 0x0003);

    //Tag in a different time zone (zone 11)
    SET_RTC_TIME_T(time, 10, 3, /*20*/16, 15, 20, 0);
    mCountList_AddItem(0x1234, &time, 0);
    _assert(mCountList_ItemWithID(0x1234, &item) == mx_err_Success);
    _assert_equal_hex(item->pickupTimeMask, 0x0403);

    return 0;
}

int all_tests() {
    _verify(test_add_items);
    _verify(test_remove_items);
    _verify(test_add_item_start_and_end_time);
    _verify(test_update_item_doesnt_change_start_time);
    _verify(test_update_item_update_end_time);
    _verify(test_update_item_update_min_max_rssi);
    _verify(test_update_item_update_avg_rssi);
    _verify(test_update_pickup_time_mask);

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
