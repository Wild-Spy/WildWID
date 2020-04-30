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
 *      This file implements a list of type #ListStruct_t.  The list is a linear
 *		list with #LISTMAXSIZE items.  There are functions available to add items
 *		to the list, remove items from the list (at any index), and search the list
 *		for entries by ID.  This list is managed, so removing items will not leave
 *		gaps in memory.  The list can ALWAYS hold #LISTMAXSIZE items.  The list is
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

#include "M_List.h"

ListStruct_t MList_Data[LISTMAXSIZE];	//!<Array containing list data.  Is a constant size depending on #LISTMAXSIZE.
uint16_t MList_Count;	//!<The number of entries currently stored in the list.

/*!	\brief [99.9%] Initialises the list
 *
 *	\return Whether function succeeded.  Always #mx_err_Success.
 */
mx_err_t mList_Init() {
	MList_Count = 0;
	return mx_err_Success;
}

/*!	\brief [99.9%] Add an item to the list.
 *
 *	If there is room in the list, add a new item with the specified values.
 *
 *	\param ID Tag ID
 *	\param Addy Tag address in memory
 *	\param LastTime Last time the tag was picked up
 *	\return #mx_err_Overflow if list is already full, #mx_err_Success if successful.
 */
mx_err_t mList_AddItem(uint32_t ID, uint32_t Addy, RTC_Time_t* LastTime) {
	
	//Declaring Data[MAX] means Data[0] -> Data[MAX-1] exist.
	//So when Count == MAX (and beyond) the list is full.
	if (MList_Count >= LISTMAXSIZE) {
		return mx_err_Overflow;
	}

	//These have been tested empirically.
	MList_Data[MList_Count].ID = ID;
	MList_Data[MList_Count].Addy = Addy;
	CopyTime(LastTime, &MList_Data[MList_Count].LastTime);
	MList_Count++;

	return mx_err_Success;

}

/*
/*!	\brief [??%]Remove an item from the list at Address
 *
 *	If the item at the specified index exists, remove the item from the list.
 *	Shuffle all following list entries back one spot to fill in the memory gap.
 *	THIS FUNCTION HAS NOT BEEN TESTED!
 *
 *	\param Address Index of the item to be removed
 *	\return #mx_err_NotFound or #mx_err_Success
 *
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
mx_err_t mList_Item(uint16_t Index, ListStruct_t** RetVal) {

	//As an example, if we have one item in our list, Count = 1 and
	//the associated index is 0.  If the passed Index to this function
	//is equal to count (eg. if count = 1 and passed index = 1) or is
	//greater than count then the index has no associated data yet and
	//an error should be returned.
	if (Index >= MList_Count) {
		return mx_err_NotFound;	//not found
	}

	//Only 95% sure that this works
	*RetVal = &MList_Data[Index];
	return mx_err_Success;

}

/*!	\brief [??%] Find first item with specified ID in the list.
 *
 *	Search through the list and return a pointer to the first occurrence
 *	of an entry with the ID being searched for.
 *
 *	\param [in] ID ID being searched for
 *	\param [out] RetVal Pointer to first matching item if found
 *	\return #mx_err_NotFound or #mx_err_Success
 */
mx_err_t mList_ItemWithID(uint32_t ID, ListStruct_t** RetVal) {

	int i;

	for (i = 0; i < MList_Count; i++) {
		if (ID == MList_Data[i].ID) {
			*RetVal = &MList_Data[i];
			return mx_err_Success;
		}
	}

	return mx_err_NotFound;	//not found
}

/*!	\brief [100%] Number of items currently in the list.
 *
 *	\return #MList_Count
 */
uint16_t mList_GetCount() {
	return MList_Count;
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
mx_err_t mList_RemoveItemWithID(uint32_t remID) {

	uint16_t i, Address;

	for (i = 0; i < MList_Count; i++) {
		if (remID == MList_Data[i].ID) {
			break;
		}
	}

	if (i == MList_Count) {
		return mx_err_NotFound;	//not found
	}
	Address = i;

	for (i = Address; i < MList_Count; i++) {
		memcpy(&MList_Data[i],&MList_Data[i+1],sizeof(ListStruct_t));
	}
	MList_Count--;

	return mx_err_Success;
}
