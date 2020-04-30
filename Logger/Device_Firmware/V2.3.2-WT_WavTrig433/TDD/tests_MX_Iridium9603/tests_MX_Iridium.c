// http://eradman.com/posts/tdd-in-c.html

#include <stdio.h>
#include "../minunit.h"
#include "../../Libs/MX_DateTime.h"
#include <time.h>

int tests_run = 0;

int test_epoch_offsets() {
    RTC_Time_t epoch;
    uint32_t secs;
    RTC_Time_t systime;
    char s[80];
    uint32_t time32;

    if (sscanf("82dda59d", "%08lX", &time32) == 0) {
    //if (sscanf("2e0b6a94", "%08lX", &time32) == 0) {
        return 1;
    }

    //SET_RTC_TIME_T(epoch, 3, 3, /*200*/15, 18, 0, 0);
    SET_RTC_TIME_T(epoch, 11, 5, /*20*/14, 14, 25, 55);

    double secsAfterEpoch = (double)time32*0.09;
    AddSecondsToEpoch(&epoch, secsAfterEpoch+0.4999, &systime);

    sPrintDateTime(s, &systime, "systime");
    printf("%s\r\n", s);
    _assert_time_equal(systime, /*20*/20, 8, 14, 15, 31, 49);


    secsAfterEpoch = 20;
    AddSecondsToEpoch(&epoch, secsAfterEpoch+0.4999, &systime);

    sPrintDateTime(s, &systime, "systime");
    printf("%s\r\n", s);
    _assert_time_equal(systime, 14, 5, 11, 14, 26, 15);

    return 0;
}

int all_tests() {
    _verify(test_epoch_offsets);

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
