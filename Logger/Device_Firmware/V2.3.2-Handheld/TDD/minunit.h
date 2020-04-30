/* file: minunit.h */

#define FAIL() printf("\nfailure in %s() line %d\n", __func__, __LINE__)

#define FAIL_EQUAL(a, b) printf("\nfailure in %s() line %d.  %d != %d\n", __func__, __LINE__, a, b)
#define FAIL_EQUAL_HEX(a, b) printf("\nfailure in %s() line %d.  0x%X != 0x%X\n", __func__, __LINE__, a, b)

#define _assert_equal_hex(a, b) do { if (!((a) == (b))) {FAIL_EQUAL_HEX((a), (b)); return 1;} } while (0)
#define _assert_equal(a, b) do { if (!((a) == (b))) {FAIL_EQUAL((a), (b)); return 1;} } while (0)
#define _assert(test) do { if (!(test)) {FAIL(); return 1;} } while (0)
#define _verify(test) do { int r=test(); tests_run++; if(r) return r; } while(0)

#define _assert_time_equal(time, year, month, day, hour, min, sec) do { _assert_equal(time.Year, year); \
									_assert_equal(time.Month, month); \
									_assert_equal(time.Day, day); \
									_assert_equal(time.Hour, hour); \
									_assert_equal(time.Minute, min); \
									_assert_equal(time.Second, sec); \
								      } while(0)

extern int tests_run;
