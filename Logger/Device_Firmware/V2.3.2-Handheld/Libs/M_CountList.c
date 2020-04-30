/*
 * M_List.c
 *
 *  Created on: 20/11/2010
 *      Author: MC
 */
/* This file has been prepared for Doxygen automatic documentation generation.*/
/*! \file *********************************************************************
 *
 * \brief  List 'class' implementation.
 *
 *      This file implements a list of type #CountListStruct_t.  The list is a linear
 *		list with #LISTMAXSIZE items.  There are functions available to add items
 *		to the list, remove items from the list (at any index), and search the list
 *		for entries by ID.  This list is managed, so removing items will not leave
 *		gaps in memory.  The list can ALWAYS hold #COUNTLISTMAXSIZE items.  The list is
 *		not a class and these functions only currently work on a SINGLE list.
 *
 * \author
 *      Matthew Cochrane \n
 *      Wild Spy
 *
 * $Revision: 1 $
 * $Date: 03/10/2011 $  \n
 *
 *****************************************************************************/

#include "M_CountList.h"

CountListStruct_t MCountList_Data[COUNTLISTMAXSIZE];	//!<Array containing list data.  Is a constant size depending on #LISTMAXSIZE.
uint16_t MCountList_Count;	//!<The number of entries currently stored in the list.
uint32_t MCountList_ItemsAdded;

/*!	\brief [99.9%] Initialises the list
 *
 *	\return Whether function succeeded.  Always #mx_err_Success.
 */
mx_err_t mCountList_Init() {
	MCountList_Count = 0;
	MCountList_ItemsAdded = 0;
	return mx_err_Success;
}

uint16_t getTimemaskMask(RTC_Time_t* time) {
	uint16_t t = time->Hour*60+time->Minute;
	t = t/90;
	return 1<<t;
}

/*!	\brief [99.9%] Add an item to the list.
 *
 *	If there is room in the list, add a new item with the specified values.
 *
 *	\param ID Tag ID
 *	\param Addy Tag address in memory
 *	\param LastTime Last time the tag was picked up
 *	\return #mx_err_Overflow if list is already full, #mx_err_Success if successful.
 *  Note: Assumes items are updated in chronological order!
 */
mx_err_t mCountList_AddItem(uint32_t ID, RTC_Time_t* time, int8_t rssi) {
	CountListStruct_t* item;
	
	MCountList_ItemsAdded++;
	
	if (mCountList_ItemWithID(ID, &item) == mx_err_Success)
	{
		//Found an item.  Increment it.
		++(item->count);
		item->lastTime = time->Hour*4 + (time->Minute/15);
		if (rssi < item->rssiMin) item->rssiMin = rssi;		
		if (rssi > item->rssiMax) item->rssiMax = rssi;
		
		item->rssiAvg -= item->rssiAvg/item->count;
		item->rssiAvg += rssi/item->count;
		
		item->pickupTimeMask |= getTimemaskMask(time);
	}
	else
	{
		//Didn't find an item.  Add it.
		//Declaring Data[MAX] means Data[0] -> Data[MAX-1] exist.
		//So when Count == MAX (and beyond) the list is full.
		if (MCountList_Count >= COUNTLISTMAXSIZE) {
			return mx_err_Overflow;
		}

		//These have been tested empirically.
		item = &MCountList_Data[MCountList_Count];
		uint8_t itime = time->Hour*4 + (time->Minute/15);
		
		item->ID = ID;
		item->count = 1;
		item->firstTime = itime;
		item->lastTime = itime;
		item->rssiMin = rssi;
		item->rssiMax = rssi;
		item->rssiAvg = rssi;
		
		item->pickupTimeMask = getTimemaskMask(time);
		MCountList_Count++;
	}

	return mx_err_Success;
}

/*
// !	\brief [??%]Remove an item from the list at Address
//  *
//  *	If the item at the specified index exists, remove the item from the list.
//  *	Shuffle all following list entries back one spot to fill in the memory gap.
//  *	THIS FUNCTION HAS NOT BEEN TESTED!
//  *
//  *	\param Address Index of the item to be removed
//  *	\return #mx_err_NotFound or #mx_err_Success
//  *
mx_err_t mList_RemoveItem(uint16_t Address) {

	int i;

	if (Address > MList_Count) {
		return mx_err_NotFound;
	}
	
	//All the ones after it need to be moved back one spot.
	for (i = Address; i < MList_Count; i++) {
		memcpy(&MList_Data[i], &MList_Data[i+1], sizeof(ListStruct_t));
	}
	MList_Count--;

	return mx_err_Success;
}
*/

/*!	\brief [95%] Returns a pointer to the item in the list with the passed index.
 *
 *	\param [in] Index Index in list
 *	\param [out] RetVal Returned Pointer
 *	\return #mx_err_NotFound or #mx_err_Success
 */
mx_err_t mCountList_Item(uint16_t Index, CountListStruct_t** RetVal) {

	//As an example, if we have one item in our list, Count = 1 and
	//the associated index is 0.  If the passed Index to this function
	//is equal to count (eg. if count = 1 and passed index = 1) or is
	//greater than count then the index has no associated data yet and
	//an error should be returned.
	if (Index >= MCountList_Count) {
		return mx_err_NotFound;	//not found
	}

	//Only 95% sure that this works
	*RetVal = &MCountList_Data[Index];
	return mx_err_Success;

}

/*!	\brief [??%] Find item with specified ID in the list.
 *
 *	Search through the list and return a pointer to the first occurrence
 *	of an entry with the ID being searched for.
 *
 *	\param [in] ID ID being searched for
 *	\param [out] RetVal Pointer to first matching item if found
 *	\return #mx_err_NotFound or #mx_err_Success
 */
mx_err_t mCountList_ItemWithID(uint32_t ID, CountListStruct_t** RetVal) {

	int i;

	for (i = 0; i < MCountList_Count; i++) {
		if (ID == MCountList_Data[i].ID) {
			*RetVal = &MCountList_Data[i];
			return mx_err_Success;
		}
	}

	return mx_err_NotFound;	//not found
}

/*!	\brief [100%] Number of items currently in the list.
 *
 *	\return #MList_Count
 */
uint16_t mCountList_GetCount() {
	return MCountList_Count;
}

uint32_t mCountList_GetItemsAdded() {
	return MCountList_ItemsAdded;
}

/*!	\brief [Remove first item with specified ID from list.
 *
 *	Search through the list for an item with the specified ID.  If one is
 *	found, remove it from the list.  Shuffle all following list entries back one
 *	spot to fill in the memory gap. eg. if item 2 was removed and list had 4 items,
 *	old list looks like 1,2,3,4.  New list has 3 items and looks like 1,3,4.  Note
 *	that a maximum of one item will be removed from the list and it will be the first
 *	occurrence of that ID in the list if the ID appears more than once.
 *
 *	\param ID ID to remove.
 *	\return #mx_err_NotFound or #mx_err_Success
 */
mx_err_t mCountList_RemoveItemWithID(uint32_t remID) {

	uint16_t i, Address;

	for (i = 0; i < MCountList_Count; i++) {
		if (remID == MCountList_Data[i].ID) {
			break;
		}
	}

	if (i == MCountList_Count) {
		return mx_err_NotFound;	//not found
	}
	Address = i;

	for (i = Address; i < MCountList_Count; i++) {
		memcpy(&MCountList_Data[i],&MCountList_Data[i+1],sizeof(CountListStruct_t));
	}
	MCountList_Count--;

	return mx_err_Success;
}

void mCountList_Clear()
{
	MCountList_Count = 0;
	MCountList_ItemsAdded = 0;
}