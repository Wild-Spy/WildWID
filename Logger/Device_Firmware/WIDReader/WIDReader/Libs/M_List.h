/*
 * M_List.h
 *
 *  Created on: 20/11/2010
 *      Author: MC
 */

#ifndef M_LIST_H_
#define M_LIST_H_

#include <stdlib.h>
#include <string.h>
#include <avr/io.h>
#include "MX_RTC.h"

#define LISTMAXSIZE		50

struct ListStruct {
	uint32_t ID;
	uint32_t Addy;
	struct RTCTime LastTime;
};



//Function Prototypes:
uint8_t mList_Init(void);
uint8_t mList_AddItem(uint32_t* ID, uint32_t* Addy, struct RTCTime* LastTime);
uint8_t mList_RemoveItem(int Address);
uint8_t mList_RemoveItemWithID(uint32_t* remID);
uint8_t mList_Item(uint16_t Index, struct ListStruct** RetVal);
uint8_t mList_ItemWithID(uint32_t* remID, struct ListStruct** RetVal);
uint16_t mList_GetCount(void);

#endif /* M_LIST_H_ */
