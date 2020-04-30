#include "serial_flash_stub.h"
#include <stdio.h>
#include <string.h>
#include <stdlib.h>

serfl_test_item_t expected_items[MAX_EXPECT_ITEMS];
serfl_test_item_t written_items[MAX_WRITTEN_ITEMS];
uint16_t written_item_count = 0;
uint16_t expected_items_count = 0;

uint16_t written_item_index = 0;
uint16_t expected_items_index = 0;

void ser_flash_stub_reset() {
	written_item_count = 0;
	expected_items_count = 0;

	written_item_index = 0;
	expected_items_index = 0;
}


bool ser_flash_stub_final_checks() {
	if (written_item_index != written_item_count) {
		printf("ERROR IN serial_flash_stub! LINE %d\r\n", __LINE__);
		printf("written_item_index = %d written_item_count = %d\r\n", written_item_index, written_item_count);
		return false;
	}
	if (expected_items_index != expected_items_count) {
		printf("ERROR IN serial_flash_stub! LINE %d\r\n", __LINE__);
		printf("expected_items_index = %d expected_items_count = %d\r\n", expected_items_index, expected_items_count);
		return false;
	}

	return true;
}


bool serial_flash_stub_checkWitten(uint32_t Address, uint16_t Length, uint8_t* Data) {
	if (written_item_index >= written_item_count) {
		printf("ERROR IN serial_flash_stub! LINE %d\r\n", __LINE__);
		return false;
	}

	serfl_test_item_t* item = &written_items[written_item_index];
	if (Address != item->address || Length != item->length) {
		return false;
	}
	if (memcmp(Data, item->data, Length) != 0) {
		return false;
	}

	written_item_index++;
	return true;
}

bool serial_flash_stub_ignoreNextWitten() {
	if (written_item_index >= written_item_count) {
		printf("ERROR IN serial_flash_stub! LINE %d\r\n", __LINE__);
		return false;
	}
	written_item_index++;
	return true;
}

void ser_flash_stub_expect_read(uint32_t Address, uint16_t Length, uint8_t* Data) {
	if (expected_items_count >= MAX_EXPECT_ITEMS) {
		//crash the program!?
		printf("ERROR IN serial_flash_stub! LINE %d\r\n", __LINE__);
	}
	serfl_test_item_t* item = &expected_items[expected_items_count];
	item->address = Address;
	memcpy(item->data, Data, Length);
	item->length = Length;
	expected_items_count++;
}






void SerFlash_ReadBytes(uint32_t Address, uint16_t Length, uint8_t* Data) {
	if (expected_items_index >= expected_items_count) {
		printf("ERROR IN serial_flash_stub! LINE %d\r\n", __LINE__);
		printf("Index = %d count = %d\r\n", expected_items_index, expected_items_count);
		exit(1);
	}
	
	serfl_test_item_t* item = &expected_items[expected_items_index];
	if (Address != item->address || Length != item->length) {
		printf("ERROR IN serial_flash_stub! LINE %d\r\n", __LINE__);
		printf("Index = %d count = %d\r\n", expected_items_index, expected_items_count);
		printf("No match Address was %08lX and expected %08lX\r\n", Address, item->address);
		printf("No match Length was %08lX and expected %08lX\r\n", Length, item->length);
		exit(1);
	}

	if (memcmp(Data, item->data, Length) != 0) {
		memcpy(Data, item->data, Length);
		// printf("ERROR IN serial_flash_stub! LINE %d\r\n", __LINE__);
		// printf("Index = %d count = %d\r\n", expected_items_index, expected_items_count);
		// printf("No match Data was %08lX and expected %08lX\r\n", *Data, *item->data);
		// exit(1);
	}

	expected_items_index++;	
}

void SerFlash_WriteBytes(uint32_t Address, uint16_t Length, uint8_t* Data) {
	if (written_item_count >= MAX_WRITTEN_ITEMS) {
		//crash the program!?
		printf("ERROR IN serial_flash_stub! LINE %d\r\n", __LINE__);
	}
	serfl_test_item_t* item = &written_items[written_item_count];
	item->address = Address;
	memcpy(item->data, Data, Length);
	item->length = Length;
	written_item_count++;
}
