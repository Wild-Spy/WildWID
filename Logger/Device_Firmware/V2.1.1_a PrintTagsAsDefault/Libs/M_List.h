/*
 * M_List.h
 *
 *  Created on: 20/11/2010
 *      Author: MC
 */
/* This file has been prepared for Doxygen automatic documentation generation.*/
/*! \file *********************************************************************
 *
 * \brief  List 'class' implementation header file.
 *
 * \author
 *      Matthew Cochrane \n
 *      Wild Spy
 *
 * $Revision: 1 $
 * $Date: 03/10/2011 $  \n
 *
 *****************************************************************************/

#ifndef M_LIST_H_
#define M_LIST_H_

#include <stdlib.h>
#include <string.h>
#include <avr/io.h>
#include "MX_RTC.h"
#include "../Strings.h"
#include "MX_ErrorEnum.h"

#define LISTMAXSIZE		50		//!<Maximum number of entries in the list.

typedef struct _ListStruct_t {
	uint32_t ID;
	uint32_t Addy;
	RTC_Time_t LastTime;
} ListStruct_t;

//Function Prototypes:
mx_err_t mList_Init(void);
mx_err_t mList_AddItem(uint32_t ID, uint32_t Addy, RTC_Time_t* LastTime);
//mx_err_t mList_RemoveItem(uint16_t Address);
mx_err_t mList_RemoveItemWithID(uint32_t remID);
mx_err_t mList_Item(uint16_t Index, ListStruct_t** RetVal);
mx_err_t mList_ItemWithID(uint32_t ID, ListStruct_t** RetVal);
uint16_t mList_GetCount(void);

#endif /* M_LIST_H_ */
