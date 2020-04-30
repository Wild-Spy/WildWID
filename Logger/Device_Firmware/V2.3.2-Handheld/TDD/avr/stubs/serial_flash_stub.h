
#ifndef __SERIAL_FLASH_STUB_H_
#define __SERIAL_FLASH_STUB_H_

#include <stdint.h>
#include <stdbool.h>

#define MAX_EXPECT_ITEMS 100
#define MAX_WRITTEN_ITEMS 100

typedef struct serfl_test_item {
	uint32_t address;
	uint8_t data[1024];
	uint16_t length;
} serfl_test_item_t;


void ser_flash_stub_reset();
bool ser_flash_stub_final_checks();
bool serial_flash_stub_checkWitten(uint32_t Address, uint16_t Length, uint8_t* Data);
bool serial_flash_stub_ignoreNextWitten();
void ser_flash_stub_expect_read(uint32_t Address, uint16_t Length, uint8_t* Data);

#endif // __SERIAL_FLASH_STUB_H_