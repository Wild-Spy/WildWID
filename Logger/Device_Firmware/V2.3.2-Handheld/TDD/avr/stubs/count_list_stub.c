
#include <stdint.h>
#include "../../../Libs/MX_DateTime.h"
#include "../../../Libs/M_CountList.h"

uint16_t items_in_list = 2;
uint32_t items_added = 0;
uint16_t list_set = 0;
CountListStruct_t clitems[2014];

void count_list_stub_set_data_set(int i) {
    list_set = i;
}

mx_err_t mCountList_Init() {
    if (list_set == 1) {
        items_in_list = 2;
	items_added = 11;
        clitems[0].ID = 0x1234;
        clitems[0].count = 10;
        clitems[0].firstTime = 10;
        clitems[0].lastTime = 20;
        clitems[0].rssiMin = -102;
        clitems[0].rssiMax = -85;
        clitems[0].rssiAvg = -99;
        clitems[0].pickupTimeMask = 0x0005;

        clitems[1].ID = 0x5555;
        clitems[1].count = 1;
        clitems[1].firstTime = 2;
        clitems[1].lastTime = 2;
        clitems[1].rssiMin = -105;
        clitems[1].rssiMax = -105;
        clitems[1].rssiAvg = -105;
        clitems[1].pickupTimeMask = 0x0001;
    } else if (list_set == 2) {
        items_in_list = 50;
	items_added = 500;

        uint16_t i;
        for (i = 0; i < 50; i++) {
            clitems[i].ID = i;
            clitems[i].count = 10;
            clitems[i].firstTime = 10;
            clitems[i].lastTime = 20;
            clitems[i].rssiMin = -102;
            clitems[i].rssiMax = -85;
            clitems[i].rssiAvg = -99;
            clitems[i].pickupTimeMask = 0x0005;
        }
    } else if (list_set == 3) {
        items_in_list = 51;
	items_added = 65555;

        uint16_t i;
        for (i = 0; i < 51; i++) {
            clitems[i].ID = i;
            clitems[i].count = 10;
            clitems[i].firstTime = 10;
            clitems[i].lastTime = 20;
            clitems[i].rssiMin = -102;
            clitems[i].rssiMax = -85;
            clitems[i].rssiAvg = -99;
            clitems[i].pickupTimeMask = 0x0005;
        }
    } else {
        items_in_list = 0;
	items_added = 0;
    }
}


void mCountList_Clear() {
}

uint16_t mCountList_GetCount() {
    return items_in_list;
}

uint32_t mCountList_GetItemsAdded() {
    return items_added;
}

mx_err_t mCountList_Item(uint16_t Index, CountListStruct_t** RetVal) {
    if (Index >= items_in_list) {
        return mx_err_NotFound;
    }
    *RetVal = &clitems[Index];
    return mx_err_Success;
}

mx_err_t mCountList_AddItem(uint32_t ID, RTC_Time_t* time, int8_t rssi) {
    return mx_err_Success;
}
