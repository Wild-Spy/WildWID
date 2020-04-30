/*
 * GUI.c
 *
 *  Created on: 24/04/2011
 *      Author: MC
 */

#include "GUI.h"

GUI_Menu* GUI_CurrentMenu = 0;
uint8_t GUI_SelectedItem = 1;
uint8_t GUI_LastSelectedItem = 1;

char GUI_Menu_Item_Back[] PROGMEM = "Back";
char GUI_Menu_Item_Main[] PROGMEM = "Main Menu";

/*
 * Main Menu
 */
char GUI_MainMenu_Item1[] PROGMEM = "Tracking";
char GUI_MainMenu_Item2[] PROGMEM = "Logging";
char GUI_MainMenu_Item3[] PROGMEM = "Options";
//char GUI_MainMenu_Item4[] PROGMEM = "";
#define GUI_MainMenu_Count 3
#define GUI_MainMenu_ID 1
char* GUI_MainMenu_Items[] PROGMEM = {GUI_MainMenu_Item1, GUI_MainMenu_Item2, GUI_MainMenu_Item3};//, GUI_MainMenu_Item4};
GUI_Menu GUI_MainMenu;

/*
 * Options Menu
 */
char GUI_OptionsMenu_Item1[] PROGMEM = "Date & Time Options";
char GUI_OptionsMenu_Item2[] PROGMEM = "Tracking Options";
char GUI_OptionsMenu_Item3[] PROGMEM = "RF Settings";
char GUI_OptionsMenu_Item4[] PROGMEM = "Advanced";
char GUI_OptionsMenu_Item5[] PROGMEM = "Product Info";
#define GUI_OptionsMenu_Count 6
#define GUI_OptionsMenu_ID 2
char* GUI_OptionsMenu_Items[] PROGMEM = {GUI_OptionsMenu_Item1, GUI_OptionsMenu_Item2, GUI_OptionsMenu_Item3, GUI_OptionsMenu_Item4, GUI_OptionsMenu_Item5, GUI_Menu_Item_Main};
GUI_Menu GUI_OptionsMenu;

/*
 * RF Settings Menu
 */
char GUI_RFSetMenu_Item1[] PROGMEM = "RF Chan: ";
char GUI_RFSetMenu_Item2[] PROGMEM = "RFID: ";
#define GUI_RFSetMenu_Count 4
#define GUI_RFSetMenu_ID 3
char* GUI_RFSetMenu_Items[] PROGMEM = {GUI_RFSetMenu_Item1, GUI_RFSetMenu_Item2, GUI_Menu_Item_Back, GUI_Menu_Item_Main};
GUI_Menu GUI_RFSetMenu;

void GUI_Setup() {
	//Main Menu
	GUI_MainMenu.Count = GUI_MainMenu_Count;
	GUI_MainMenu.ID = GUI_MainMenu_ID;
	GUI_MainMenu.Items = GUI_MainMenu_Items;

	//Options Menu
	GUI_OptionsMenu.Count = GUI_OptionsMenu_Count;
	GUI_OptionsMenu.ID = GUI_OptionsMenu_ID;
	GUI_OptionsMenu.Items = GUI_OptionsMenu_Items;

	//Settings Menu
	GUI_RFSetMenu.Count = GUI_RFSetMenu_Count;
	GUI_RFSetMenu.ID = GUI_RFSetMenu_ID;
	GUI_RFSetMenu.Items = GUI_RFSetMenu_Items;
}

void GUI_Show_Header() {

}

void GUI_ShowMenu(GUI_Menu* Menu, uint8_t SelItem) {
	uint8_t NSel = NORMAL;
	GUI_ClearMenu();

	for (uint8_t i = 0; i < Menu->Count; i++) {

		if ((i+1) == SelItem) NSel = INVERTED;
		else NSel = NORMAL;

		LCD_PosSetUp(i+3,5);
		LCD_DisplayString_P(5,(char*)pgm_read_word((Menu->Items+i)),NSel);
	}

}

void GUI_RedrawMenu() {

	LCD_PosSetUp(GUI_LastSelectedItem+2,5);
	LCD_DisplayString_P(5,(char*)pgm_read_word((GUI_CurrentMenu->Items+(GUI_LastSelectedItem-1))),NORMAL);

	LCD_PosSetUp(GUI_SelectedItem+2,5);
	LCD_DisplayString_P(5,(char*)pgm_read_word((GUI_CurrentMenu->Items+(GUI_SelectedItem-1))),INVERTED);

}

void GUI_Menu_Keypress_Handler(char Key) {
	if (GUI_CurrentMenu == 0) {
		GUI_Setup();
		GUI_ShowMenu(&GUI_MainMenu, 1);
		GUI_CurrentMenu = &GUI_MainMenu;
	}

	switch (Key) {
	case '8':
		GUI_SelectedItem++;
		if (GUI_SelectedItem > GUI_CurrentMenu->Count)
			GUI_SelectedItem = 1;
		GUI_RedrawMenu();
		break;
	case '2':
		GUI_SelectedItem--;
		if (GUI_SelectedItem < 1)
			GUI_SelectedItem = GUI_CurrentMenu->Count;
		GUI_RedrawMenu();
		break;
	case '5':
		GUI_GoToSubMenu();
		break;
	}

	//GUI_ShowMenu(GUI_CurrentMenu, GUI_SelectedItem);

	GUI_LastSelectedItem = GUI_SelectedItem;
}

void GUI_GoToSubMenu() {
	switch (GUI_CurrentMenu->ID) {
	case GUI_MainMenu_ID:
		switch (GUI_SelectedItem) {
		case 1:	//Tracking

			break;
		case 2:	//Logging

			break;
		case 3:	//Options
			GUI_OptionsMenu.LastMenu = GUI_CurrentMenu;
			GUI_CurrentMenu = &GUI_OptionsMenu;
			break;
		}
		break;
	case GUI_OptionsMenu_ID:
		switch (GUI_SelectedItem) {
		case 1:	//Date/Time Options

			break;
		case 2:	//Tracking Options

			break;
		case 3:	//RF Settings
			GUI_RFSetMenu.LastMenu = GUI_CurrentMenu;
			GUI_CurrentMenu = &GUI_RFSetMenu;
			break;
		case 4:	//Advanced

			break;
		case 5:	//Product Info

			break;
		case 6:	//Main Menu
			GUI_MainMenu.LastMenu = GUI_CurrentMenu;
			GUI_CurrentMenu = &GUI_MainMenu;
			break;
		}
		break;
	case GUI_RFSetMenu_ID:
		switch (GUI_SelectedItem) {
		case 1:	//RF Chan:

			break;
		case 2:	//RFID:

			break;
		case 3:	//Back
			GUI_CurrentMenu = GUI_CurrentMenu->LastMenu;
			break;
		case 4:	//Main Menu
			GUI_MainMenu.LastMenu = GUI_CurrentMenu;
			GUI_CurrentMenu = &GUI_MainMenu;
			break;
		}
		break;
	}
	GUI_SelectedItem = 1;
	GUI_LastSelectedItem = 1;
	GUI_ShowMenu(GUI_CurrentMenu, 1);
}
