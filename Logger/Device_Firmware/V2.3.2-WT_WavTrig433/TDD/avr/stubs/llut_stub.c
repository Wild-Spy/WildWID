#include <stdint.h>
#include <stdlib.h>
#include "../../../Libs/MX_DateTime.h"
#include "../../../Libs/LoggerLookupTable.h"

static uint16_t items_in_list = 2;
static uint16_t list_set = 0;
static LLUTStruct_t llutItems[256];

void llut_stub_set_data_set(int i) {
    list_set = i;
}

mx_err_t LLUT_Init() {
    if (list_set == 1) {
        items_in_list = 2;
        llutItems[0].ID = 0x1234;
        llutItems[0].bat_volt = 12110;
        llutItems[0].flash_usage = 81;
        llutItems[0].flags = 0b00000001;
        llutItems[0].is_ok = 110;

        llutItems[1].ID = 0x5555;
        llutItems[1].bat_volt = 12110;
        llutItems[1].flash_usage = 81;
        llutItems[1].flags = 0b00000001;
        llutItems[1].is_ok = 110;
    } else if (list_set == 2) {
        items_in_list = 42;

        uint16_t i;
        for (i = 0; i < 41; i++) {
            llutItems[i].ID = i;
            llutItems[i].bat_volt = 12110;
            llutItems[i].flash_usage = 81;
            llutItems[i].flags = 0b00000001;
            llutItems[i].is_ok = 110;
        }
    } else if (list_set == 3) {
        items_in_list = 43;

        uint16_t i;
        for (i = 0; i < 43; i++) {
            llutItems[i].ID = i;
            llutItems[i].bat_volt = 12110;
            llutItems[i].flash_usage = 81;
            llutItems[i].flags = 0b00000001;
            llutItems[i].is_ok = 110;
        }
    } else {
        items_in_list = 0;
    }
}


void LLUT_Clear() {
    exit(-1);
}

uint8_t LLUT_GetCount() {
    return items_in_list;
}

LLUTStruct_t* LLUT_GetDataPtr() {
    return llutItems;
}
