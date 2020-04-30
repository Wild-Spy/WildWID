/*
 * GUI.h
 *
 *  Created on: 24/04/2011
 *      Author: MC
 */

#ifndef GUI_H_
#define GUI_H_

#include <stdlib.h>
#include <avr/io.h>
#include <avr/pgmspace.h>
#include "KeyPad.h"
#include "NHDC160100DiZ.h"

//Defines:
#define GUI_KEY_UP		'2'
#define GUI_KEY_DOWN	'8'
#define GUI_KEY_LEFT	'4'
#define GUI_KEY_RIGHT	'6'
#define GUI_KEY_ENTER	'#'
#define GUI_KEY_BACK	'*'

typedef struct _GUI_Menu {
	uint8_t Count;
	uint8_t ID;
	char** Items;
	struct _GUI_Menu* LastMenu;
} GUI_Menu;

//Macros:
#define GUI_ClearMenu()		LCD_Clear_Display(3,20)

//Prototypes:
void GUI_Show_Header( void );
void GUI_ShowMenu(GUI_Menu* Menu, uint8_t SelItem);
void GUI_RedrawMenu();
void GUI_GoToSubMenu();

void GUI_Menu_Keypress_Handler(char Key);

#endif /* GUI_H_ */
