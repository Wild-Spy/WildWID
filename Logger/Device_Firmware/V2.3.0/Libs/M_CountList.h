/*
 * M_CountList.h
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

#ifndef M_LISTCOUNT_H_
#define M_LISTCOUNT_H_

#include <stdlib.h>
#include <string.h>
#include <avr/io.h>
#include "MX_ErrorEnum.h"

#define COUNTLISTMAXSIZE		50		//!<Maximum number of entries in the list.

typedef struct _CountListStruct_t {
	uint32_t ID;
	uint16_t count;
} CountListStruct_t;

//Function Prototypes:
mx_err_t mCountList_Init(void);
mx_err_t mCountList_AddItem(uint32_t ID);
//mx_err_t mList_RemoveItem(uint16_t Address);
mx_err_t mCountList_RemoveItemWithID(uint32_t remID);
mx_err_t mCountList_Item(uint16_t Index, CountListStruct_t** RetVal);
mx_err_t mCountList_ItemWithID(uint32_t ID, CountListStruct_t** RetVal);
uint16_t mCountList_GetCount(void);
void mCountList_Clear();

#endif /* M_LISTCOUNT_H_ */
