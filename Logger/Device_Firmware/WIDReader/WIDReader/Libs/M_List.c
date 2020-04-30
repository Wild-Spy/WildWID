/*
 * M_List.c
 *
 *  Created on: 20/11/2010
 *      Author: MC
 */

#include "M_List.h"

struct ListStruct MList_Data[LISTMAXSIZE];
int MList_Count;

uint8_t mList_Init() {
	MList_Count = 0;
	return 0;
}

uint8_t mList_AddItem(uint32_t* ID, uint32_t* Addy, struct RTCTime* LastTime) {
	if (MList_Count >= LISTMAXSIZE) {
		return 1;
	}

	memcpy(&MList_Data[MList_Count].ID,ID,4);
	memcpy(&MList_Data[MList_Count].Addy,Addy,4);
	CopyTime(LastTime, &MList_Data[MList_Count].LastTime);
	MList_Count++;

	return 0;

}

uint8_t mList_RemoveItem(int Address) {

	int i;

	if (Address > MList_Count) {
		return 1;
	}

	for (i = Address; i < MList_Count; i++) {
		memcpy(&MList_Data[i],&MList_Data[i+1],sizeof(struct ListStruct));
	}
	MList_Count--;

	return 0;
}

uint8_t mList_Item(uint16_t Index, struct ListStruct** RetVal) {

	if (Index >= MList_Count) {
		return 1;	//not found
	}

	*RetVal = &MList_Data[Index];
	return 0;

}

uint8_t mList_ItemWithID(uint32_t* ID, struct ListStruct** RetVal) {

	int i;

	for (i = 0; i < MList_Count; i++) {
		if (*ID == MList_Data[i].ID) {
			*RetVal = &MList_Data[i];
			return 0;
		}
	}

	return 1;	//not found
}

uint16_t mList_GetCount() {
	return MList_Count;
}

uint8_t mList_RemoveItemWithID(uint32_t* remID) {


	int i, Address;

	for (i = 0; i < MList_Count; i++) {
		if (*remID == MList_Data[i].ID) {
			break;
		}
	}

	if (i == MList_Count) {
		return 1;	//not found
	}
	Address = i;

	for (i = Address; i < MList_Count; i++) {
		memcpy(&MList_Data[i],&MList_Data[i+1],sizeof(struct ListStruct));
	}
	MList_Count--;

	return 0;
}
