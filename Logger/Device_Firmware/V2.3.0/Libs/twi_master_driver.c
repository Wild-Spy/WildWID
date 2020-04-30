/* This file has been prepared for Doxygen automatic documentation generation.*/
/*! \file *********************************************************************
 *
 * \brief
 *      XMEGA TWI master driver source file.
 *
 *      This file contains the function implementations the XMEGA master TWI
 *      driver.
 *
 *      The driver is not intended for size and/or speed critical code, since
 *      most functions are just a few lines of code, and the function call
 *      overhead would decrease code performance. The driver is intended for
 *      rapid prototyping and documentation purposes for getting started with
 *      the XMEGA TWI master module.
 *
 *      For size and/or speed critical code, it is recommended to copy the
 *      function contents directly into your application instead of making
 *      a function call.
 *
 *      Several functions use the following construct:
 *          "some_register = ... | (some_parameter ? SOME_BIT_bm : 0) | ..."
 *      Although the use of the ternary operator ( if ? then : else ) is
 *      discouraged, in some occasions the operator makes it possible to write
 *      pretty clean and neat code. In this driver, the construct is used to
 *      set or not set a configuration bit based on a boolean input parameter,
 *      such as the "some_parameter" in the example above.
 *
 * \par Application note:
 *      AVR1308: Using the XMEGA TWI
 *
 * \par Documentation
 *      For comprehensive code documentation, supported compilers, compiler
 *      settings and supported devices see readme.html
 *
 * \author
 *      Atmel Corporation: http://www.atmel.com \n
 *      Support email: avr@atmel.com
 *
 * $Revision: 1569 $
 * $Date: 2008-04-22 13:03:43 +0200 (ti, 22 apr 2008) $  \n
 *
 * Copyright (c) 2008, Atmel Corporation All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 *
 * 1. Redistributions of source code must retain the above copyright notice,
 * this list of conditions and the following disclaimer.
 *
 * 2. Redistributions in binary form must reproduce the above copyright notice,
 * this list of conditions and the following disclaimer in the documentation
 * and/or other materials provided with the distribution.
 *
 * 3. The name of ATMEL may not be used to endorse or promote products derived
 * from this software without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY ATMEL "AS IS" AND ANY EXPRESS OR IMPLIED
 * WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF
 * MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE EXPRESSLY AND
 * SPECIFICALLY DISCLAIMED. IN NO EVENT SHALL ATMEL BE LIABLE FOR ANY DIRECT,
 * INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
 * ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
 * THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 *****************************************************************************/

#include "twi_master_driver.h"
#include "MX_USART.h"
extern char s[80];

/*! \brief Initialize the TWI module.
 *
 *  TWI module initialization function.
 *  Enables master read and write interrupts.
 *  Remember to enable interrupts globally from the main application.
 *
 *  \param twi                      The TWI_Master_t struct instance.
 *  \param module                   The TWI module to use.
 *  \param intLevel                 Master interrupt level.
 *  \param baudRateRegisterSetting  The baud rate register value.
 */
void TWI_MasterInit(TWI_Master_t *twi,
                    TWI_t *module,
                    TWI_MASTER_INTLVL_t intLevel,
                    uint8_t baudRateRegisterSetting)
{
	twi->interface = module;
	twi->interface->MASTER.CTRLA = TWI_MASTER_ENABLE_bm;
	twi->interface->MASTER.BAUD = baudRateRegisterSetting;
	twi->interface->MASTER.STATUS = TWI_MASTER_BUSSTATE_IDLE_gc;
	twi->status = TWIM_RESULT_READY;
}


/*! \brief Returns the TWI bus state.
 *
 *  Returns the TWI bus state (type defined in device headerfile),
 *  unknown, idle, owner or busy.
 *
 *  \param twi The TWI_Master_t struct instance.
 *
 *  \retval TWI_MASTER_BUSSTATE_UNKNOWN_gc Bus state is unknown.
 *  \retval TWI_MASTER_BUSSTATE_IDLE_gc    Bus state is idle.
 *  \retval TWI_MASTER_BUSSTATE_OWNER_gc   Bus state is owned by the master.
 *  \retval TWI_MASTER_BUSSTATE_BUSY_gc    Bus state is busy.
 */
TWI_MASTER_BUSSTATE_t TWI_MasterState(TWI_Master_t *twi)
{
	TWI_MASTER_BUSSTATE_t twi_status;
	twi_status = (TWI_MASTER_BUSSTATE_t) (twi->interface->MASTER.STATUS &
	                                      TWI_MASTER_BUSSTATE_gm);
	return twi_status;
}

/*! \brief TWI start transaction.
 *
 *  This function starts a read or write transaction (start bit followed by address
 *  and R/W bit)
 *
 *  \param twi          The TWI_Master_t struct instance.
 *  \param address    	Address to write to.
 *  \param read			1 if a read operation, 0 if a write operation.
 *
 *	//Fix below
 *  \retval 0 	  If transaction could not be started - incorrect mode.
 *  \retval false If transaction could not be started.
 */
uint8_t TWI_MasterStart(TWI_Master_t *twi, uint8_t address, uint8_t read)
{
	//if (!(twi->status == TWIM_RESULT_READYFORRX || twi->status == TWIM_RESULT_READYFORTX)) {

	twi->interface->MASTER.STATUS = TWI_MASTER_BUSSTATE_IDLE_gc;

	twi->status = TWIM_RESULT_UNKNOWN;

	twi->address = address<<1;

	/* If write command, send the START condition + Address +
	 * 'R/_W = 0'
	 */
	if (read == 0) {
		uint8_t writeAddress = twi->address & ~0x01;
		twi->interface->MASTER.ADDR = writeAddress;
		twi->status = TWIM_RESULT_READY;
		//Wait for TX of initial byte to occur
		while(!(twi->interface->MASTER.STATUS&TWI_MASTER_WIF_bm));
		twi->status = TWIM_RESULT_READYFORTX;
		return 1;
	}

	/* If read command, send the START condition + Address +
	 * 'R/_W = 1'
	 */
	else {
		uint8_t readAddress = twi->address | 0x01;
		twi->interface->MASTER.ADDR = readAddress;
		twi->status = TWIM_RESULT_READY;
		//Wait for TX of initial byte to occur
		while(!(twi->interface->MASTER.STATUS&TWI_MASTER_WIF_bm));
		twi->status = TWIM_RESULT_READYFORRX;
		return 2;
	}
	//} else {
	//	return 0;
	//}
}

/*! \brief TWI write transaction.
 *
 *  This function is TWI Master wrapper for a write-only transaction.
 *
 *  \param twi          The TWI_Master_t struct instance.
 *  \param writeData    Single byte to write.
 *
 *	//Fix below
 *  \retval 0 	  If transaction could not be started - incorrect mode.
 *  \retval false If transaction could not be started.
 */
uint8_t TWI_MasterWrite(TWI_Master_t *twi, uint8_t writeData)
{

	//if (twi->status == TWIM_RESULT_READYFORTX || twi->status == TWIM_RESULT_OK) {

	twi->interface->MASTER.DATA = writeData;

	//Wait for interrupt (change of status)
	while(!((twi->interface->MASTER.STATUS)&TWI_MASTER_WIF_bm));

	//if (twi->interface->MASTER.STATUS&TWI_MASTER_ACK) {
		/* If NOT acknowledged (NACK) by slave cancel the transaction. */
		if (twi->interface->MASTER.STATUS & TWI_MASTER_RXACK_bm) {
			twi->interface->MASTER.CTRLC = TWI_MASTER_CMD_STOP_gc;
			twi->status = TWIM_RESULT_UNKNOWN;
			return TWIM_RESULT_NACK_RECEIVED<<4;
		} else {
			//All went well
			twi->status = TWIM_RESULT_READYFORTX;
			return 1; //Success
		}
	/*} else {
		//Error!!
		uint8_t a = twi->status;
		twi->status = TWIM_RESULT_UNKNOWN;
		return a<<4;
	}*/
	//} else {
	//	return 0;
	//}
}

/*! \brief TWI read transaction.
 *  \param twi          The TWI_Master_t struct instance.
 *  \param readData   	pointer to single byte to store read data in.
 */
uint8_t TWI_MasterRead(TWI_Master_t *twi, uint8_t* readData)
{

	if (twi->status == TWIM_RESULT_READYFORRX || twi->status == TWIM_RESULT_OK) {

		twi->interface->MASTER.CTRLC = TWI_MASTER_CMD_RECVTRANS_gc;

		//Wait for interrupt (change of status)
		while(!(twi->interface->MASTER.STATUS&TWI_MASTER_RIF_bm));

		if (twi->status == TWIM_RESULT_OK) {
			//All went well
			*readData = twi->interface->MASTER.DATA;
			twi->status = TWIM_RESULT_READYFORRX;
			return 1; //Success
		} else {
			//Error!!
			uint8_t a = twi->status;
			twi->status = TWIM_RESULT_UNKNOWN;
			return a<<4;
		}
	} else {
		return 0;
	}
}

void TWI_MasterStop(TWI_Master_t* twi)
{

	twi->interface->MASTER.CTRLC = TWI_MASTER_CMD_STOP_gc;
	twi->status = TWIM_RESULT_READY;

}

/*! \brief Common TWI master interrupt service routine.
 *
 *  Check current status and sets the status register as appropriate.
 *
 *  \param twi  The TWI_Master_t struct instance.
 */
void TWI_MasterInterruptHandler(TWI_Master_t *twi)
{
	uint8_t currentStatus = twi->interface->MASTER.STATUS;

	//USART_printf_P(&USARTC0, "SR: 0x%02X\r\n", currentStatus);
	//USART_tx_Byte(&USARTC0, currentStatus);

	/* If arbitration lost or bus error. */
	if ((currentStatus & TWI_MASTER_ARBLOST_bm) ||
	    (currentStatus & TWI_MASTER_BUSERR_bm)) {

		/* If bus error. */
		if (currentStatus & TWI_MASTER_BUSERR_bm) {
			twi->status = TWIM_RESULT_BUS_ERROR;
		}
		/* If arbitration lost. */
		else {
			twi->status = TWIM_RESULT_ARBITRATION_LOST;
		}

		/* Clear interrupt flag. */
		twi->interface->MASTER.STATUS = currentStatus | TWI_MASTER_ARBLOST_bm;
	}

	/* If master write interrupt. */
	else if (currentStatus & TWI_MASTER_WIF_bm) {
		//TWI_MasterWriteHandler(twi);
		twi->status = TWIM_RESULT_OK;
	}

	/* If master read interrupt. */
	else if (currentStatus & TWI_MASTER_RIF_bm) {
		//TWI_MasterReadHandler(twi);
		twi->status = TWIM_RESULT_OK;
	}

	else if (currentStatus & TWI_MASTER_CLKHOLD_bm) {
		//do nothing... wait.
	}

	/* If unexpected state. */
	else {
		twi->status = currentStatus>>4;
	}
}
