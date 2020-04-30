/*
 * MX_SerFlash.c
 *
 *  Created on: 14/11/2010
 *      Author: MC
 */

#include "MX_SerFlash.h"

//Address goes in the 3 LSB of Address
void SerFlash_ReadBytes(uint32_t Address, uint16_t Length, uint8_t* Data) {

	SerFlash_WaitRDY();

	SerFlash_PORT.OUTCLR = (1<<SerFlash_CSN);

#ifdef SerFlash_FAST_RD
	SerFlash_SPI_TX(SerFlash_OP_ReadAryF);
#else
	SerFlash_SPI_TX(SerFlash_OP_ReadAryS);
#endif

	SerFlash_SPI_TX(*(((uint8_t*)&Address)+2));
	SerFlash_SPI_TX(*(((uint8_t*)&Address)+1));
	SerFlash_SPI_TX(*(((uint8_t*)&Address)+0));

#ifdef SerFlash_FAST_RD
	SerFlash_SPI_TX(0x00);
#endif

	do {
		*Data++ = SerFlash_SPI_RX();
	} while (--Length);

	SerFlash_PORT.OUTSET = (1<<SerFlash_CSN);

}

void SerFlash_WriteBytes(uint32_t Address, uint16_t Length, uint8_t* Data) {


	//Assumes that locations being written to have already been erased!
	//Can and will write from one page into another.
	//Simple sequential write
#if SF_VER == SF_VER_ATMEL
	uint32_t ProctAdd = 0x00000000L;
#endif

	Address &= 0x00FFFFFFL;

#if SF_VER == SF_VER_GENERIC
	SerFlash_SetProt(SerFlash_PROT_NONE);
#endif

	do {
#if SF_VER == SF_VER_ATMEL
		//Check section is not protected, if it is unprotect it
		if (SerFlash_RdSectProt(Address)) {
			SerFlash_WaitRDY();
			SerFlash_ProtSect(Address, 0);
			ProctAdd = Address|0xFF000000L;
		} else
			SerFlash_WaitRDY();
#elif SF_VER == SF_VER_GENERIC
		SerFlash_WaitRDY();
#endif

		//Setup Write
		SerFlash_WriteEn();
		SerFlash_PORT.OUTCLR = (1<<SerFlash_CSN);

		SerFlash_SPI_TX(SerFlash_OP_BPPrgm);

		SerFlash_SPI_TX(*(((uint8_t*)&Address)+2));
		SerFlash_SPI_TX(*(((uint8_t*)&Address)+1));
		SerFlash_SPI_TX(*(((uint8_t*)&Address)+0));

		do {
			//Write Byte
			SerFlash_SPI_TX(*Data++);
			Address++;
			Length--;
		} while ((uint8_t)Address != 0x00 && Length);

		SerFlash_PORT.OUTSET = (1<<SerFlash_CSN);


		//Put page protection status back to how it was
#if SF_VER == SF_VER_ATMEL
		if (ProctAdd&0xFF000000L) {
			SerFlash_WaitRDY();
			SerFlash_ProtSect(ProctAdd, 1);
		}
#endif

	} while (Length);

#if SF_VER == SF_VER_GENERIC
	SerFlash_SetProt(SerFlash_PROT_ALL);
#endif

}

void SerFlash_BlockErase(uint32_t Address, uint8_t Size) {

	//Size Variable 	Erase Size
	//0					4kB			- ATMEL ONLY
	//1					32kB		- ATMEL ONLY
	//2					64kB

	uint8_t OPC;
	uint32_t ProctAdd = 0x00000000L;

	switch (Size){
#if SF_VER == SF_VER_ATMEL
	case 0:
		OPC = SerFlash_OP_BlkErs4k;
		break;
	case 1:
		OPC = SerFlash_OP_BlkErs32k;
		break;
#endif
	case 2:
		OPC = SerFlash_OP_BlkErs64k;
		break;
	default:
		return;
	}

	if (SerFlash_RdSectProt(Address)) {
		SerFlash_WaitRDY();
#if SF_VER == SF_VER_ATMEL
			SerFlash_ProtSect(Address, 0);
#elif SF_VER == SF_VER_GENERIC
			SerFlash_SetProt(SerFlash_PROT_NONE);
#endif
		ProctAdd = Address|0xFF000000L;
	} else
		SerFlash_WaitRDY();

	SerFlash_WriteEn();

	SerFlash_PORT.OUTCLR = (1<<SerFlash_CSN);

	SerFlash_SPI_TX(OPC);

	SerFlash_SPI_TX(*(((uint8_t*)&Address)+2));
	SerFlash_SPI_TX(*(((uint8_t*)&Address)+1));
	SerFlash_SPI_TX(*(((uint8_t*)&Address)+0));

	SerFlash_PORT.OUTSET = (1<<SerFlash_CSN);

	if (ProctAdd&0xFF000000L) {
		SerFlash_WaitRDY();
		SerFlash_WriteEn();
#if SF_VER == SF_VER_ATMEL
			SerFlash_ProtSect(ProctAdd, 1);
#elif SF_VER == SF_VER_GENERIC
			SerFlash_SetProt(SerFlash_PROT_ALL);
#endif
	}

}

void SerFlash_ChipErase(uint8_t Saftey) {
	if (Saftey == 0x74) {

		SerFlash_WaitRDY();

#if SF_VER == SF_VER_ATMEL
		uint32_t Addy = 0x00000000L;

		do {
			SerFlash_ProtSect(Addy, 0);
			Addy += SerFlash_SECTORSIZE;
		} while (Addy < SerFlash_SIZE);
#elif SF_VER == SF_VER_GENERIC
		SerFlash_SetProt(SerFlash_PROT_NONE);
#endif

		SerFlash_WriteEn();
		SerFlash_PORT.OUTCLR = (1<<SerFlash_CSN);
#if SF_VER == SF_VER_ATMEL
		SerFlash_SPI_TX(SerFlash_OP_ChpErs1);
#elif SF_VER == SF_VER_GENERIC
		SerFlash_SPI_TX(SerFlash_OP_ChpErs);
#endif
		SerFlash_PORT.OUTSET = (1<<SerFlash_CSN);


#if SF_VER == SF_VER_GENERIC
		SerFlash_SetProt(SerFlash_PROT_ALL);
#endif

	}
}

void SerFlash_WriteEn() {
	SerFlash_PORT.OUTCLR = (1<<SerFlash_CSN);
	SerFlash_SPI_TX(SerFlash_OP_WrtEn);
	SerFlash_PORT.OUTSET = (1<<SerFlash_CSN);
}

void SerFlash_WriteDis() {
	SerFlash_PORT.OUTCLR = (1<<SerFlash_CSN);
	SerFlash_SPI_TX(SerFlash_OP_WrtDis);
	SerFlash_PORT.OUTSET = (1<<SerFlash_CSN);
}

void SerFlash_DeepPwrDwn(uint8_t Value) {
	uint8_t OPC;

	if (Value)
		OPC = SerFlash_OP_DPwrDwn;
	else
		OPC = SerFlash_OP_ResDPwrDwn;

	SerFlash_WaitRDY();

	SerFlash_PORT.OUTCLR = (1<<SerFlash_CSN);
	SerFlash_SPI_TX(OPC);
	SerFlash_PORT.OUTSET = (1<<SerFlash_CSN);
}

#if SF_VER == SF_VER_ATMEL
//Address goes in the 3 LSB of 'Address', the variable
void SerFlash_ProtSect(uint32_t Address, uint8_t Value) {

	uint8_t OPC;

	if (Value)
		OPC = SerFlash_OP_ProtSect;
	else
		OPC = SerFlash_OP_UnProtSect;

	SerFlash_WaitRDY();

	SerFlash_WriteEn();

	SerFlash_PORT.OUTCLR = (1<<SerFlash_CSN);

	SerFlash_SPI_TX(OPC);

	SerFlash_SPI_TX(*(((uint8_t*)&Address)+2));
	SerFlash_SPI_TX(*(((uint8_t*)&Address)+1));
	SerFlash_SPI_TX(*(((uint8_t*)&Address)+0));

	SerFlash_PORT.OUTSET = (1<<SerFlash_CSN);

}
#elif SF_VER == SF_VER_GENERIC
//Address goes in the 3 LSB of 'Address', the variable
void SerFlash_SetProt(uint8_t protVal) {

	//do {
		SerFlash_WaitRDY();

		SerFlash_WriteEn();

		SerFlash_PORT.OUTCLR = (1<<SerFlash_CSN);

		SerFlash_SPI_TX(SerFlash_OP_WrtStatus);

		SerFlash_SPI_TX(protVal & SerFlash_PROT_MASK);

		SerFlash_PORT.OUTSET = (1<<SerFlash_CSN);

		SerFlash_WaitRDY();
	//} while ((SerFlash_RdStatusReg() & SerFlash_PROT_MASK) != protVal);

}
#endif

uint8_t SerFlash_RdSectProt(uint32_t Address) {
	uint8_t TmpVar;

	SerFlash_PORT.OUTCLR = (1<<SerFlash_CSN);

#if SF_VER == SF_VER_ATMEL
	SerFlash_SPI_TX(SerFlash_OP_RdProtReg);
	SerFlash_SPI_TX(*(((uint8_t*)&Address)+2));
	SerFlash_SPI_TX(*(((uint8_t*)&Address)+1));
	SerFlash_SPI_TX(*(((uint8_t*)&Address)+0));
	TmpVar = SerFlash_SPI_RX();
	SerFlash_PORT.OUTSET = (1<<SerFlash_CSN);

	return TmpVar;
#elif SF_VER == SF_VER_GENERIC
	TmpVar = SerFlash_RdStatusReg() & SerFlash_PROT_MASK;

	switch (TmpVar) {
	case SerFlash_PROT_NONE:
		//Whole chip unprotected
		return 0;
	case SerFlash_PROT_U32:
		//Upper 32nd protected
		if (Address > SerFlash_SIZE*(31/32))
			return 1;
		else
			return 0;
	case SerFlash_PROT_U16:
		//Upper 16th protected
		if (Address > SerFlash_SIZE*(15/16))
			return 1;
		else
			return 0;
	case SerFlash_PROT_U8:
		//Upper 8th protected
		if (Address > SerFlash_SIZE*(7/8))
			return 1;
		else
			return 0;
	case SerFlash_PROT_U4:
		//Upper quarter protected
		if (Address > SerFlash_SIZE*(3/4))
			return 1;
		else
			return 0;
	case SerFlash_PROT_U2:
		//Upper half protected
		if (Address > SerFlash_SIZE*(1/2))
			return 1;
		else
			return 0;
	case 0b00011000:
		//All Sectors protected
		return 1;
	case SerFlash_PROT_ALL:
		//All Sectors protected
		return 1;
	default:
		return 1;
	}
#endif
}

uint8_t SerFlash_RdStatusReg() {
	uint8_t tmpVar;

	SerFlash_PORT.OUTCLR = (1<<SerFlash_CSN);
	SerFlash_SPI_TX(SerFlash_OP_RdStatus);
	tmpVar = SerFlash_SPI_RX();
	SerFlash_PORT.OUTSET = (1<<SerFlash_CSN);

	return tmpVar;
}

void SerFlash_SPI_TX(uint8_t Data) {
	SerFlash_SPI.DATA = Data;
	while(!(SerFlash_SPI.STATUS & (1<<SPI_IF_bp)));
}

uint8_t SerFlash_SPI_RX() {
	SerFlash_SPI.DATA = 0x00;
	while(!(SerFlash_SPI.STATUS & (1<<SPI_IF_bp)));
	return SerFlash_SPI.DATA;
}

void SerFlash_SPI_Setup() {

	//Note: pin 4 must always be either an output or held high otherwise SPI will default to slave mode
	// PX7 (SCK), PX5 (MOSI), PX1 (CSN) as outputs
	SerFlash_PORT.DIRSET = (1<<SerFlash_SCK)|(1<<SerFlash_MOSI)|(1<<SerFlash_CSN)|(1<<4);
#ifdef SerFlash_WPN
	SerFlash_PORT.DIRSET = (1<<SerFlash_WPN);
#endif
#ifdef SerFlash_HOLDN
	SerFlash_PORT.DIRSET = (1<<SerFlash_HOLDN);
#endif
	// PX6 (MISO) as inputs
	SerFlash_PORT.DIRCLR = (1<<SerFlash_MISO);
	// PX4 (CSN) enable pull-up. (TODO[ ]: VERIFY THAT THIS IS HOW YOU DO THIS IN AN XMEGA!!)
	//PORTCFG_MPCMASK = (1<<SerFlash_CSN)|(1<<SerFlash_MISO)|(1<<SerFlash_MOSI);
	//Set the pins selected above to the pull up configuration
	//SerFlash_PORT.PIN0CTRL = 0b00011000;

	//TODO[ ]: Set this up so that it uses SerFlash_SCK and SerFlash_CSN macros rather than just specific pin values
	//Pull-up on CSN pin
	//SerFlash_PORT.PIN1CTRL = PORT_OPC_PULLUP_gc;
	//Pull-down on SCK pin
	//SerFlash_PORT.PIN7CTRL = PORT_OPC_PULLDOWN_gc;

	//PORTCFG_MPCMASK = 0xFF;
	//SerFlash_PORT.PIN0CTRL = 0;

	SerFlash_PORT.OUTCLR = (1<<SerFlash_SCK);
	SerFlash_PORT.OUTSET = (1<<SerFlash_CSN);


	//TODO[ ]: Experiment with changing the clock speed.. up to 32MHz!!
	SerFlash_SPI.CTRL = SPI_ENABLE_bm|SPI_MASTER_bm|SPI_MODE_0_gc|SPI_PRESCALER_DIV4_gc;

}
