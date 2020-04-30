/* 
 * Author: Matthew Cochrane
 * Date: Nov 2015
 * Description: Testing for the CC110L module
 * 
 */

#include <stdint.h>

void cc110LTester_Run_ChooseMode();
void cc110LTester_Run_MasterMode();
void cc110LTester_Run_SlaveMode();

void printBufferHex(uint8_t* buf, uint16_t len);
