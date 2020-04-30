//...

#include <stdint.h>
#include "../../../Libs/MX_DateTime.h"

uint8_t loadRecord(uint32_t RecordNo, RTC_Time_t* mTime, uint32_t* ID, uint8_t* RSSI, uint8_t* Activity) {
    //SET_RTC_TIME_T(*mTime, 10, 3, /*20*/16, 12, 20, 0);
    *ID = 0x9999;

    return 0;
}

uint8_t RecordToFlash(RTC_Time_t* mTime, uint32_t ID, uint8_t Flags, uint8_t RSSI, uint8_t Activity) {
    return 1;
}
