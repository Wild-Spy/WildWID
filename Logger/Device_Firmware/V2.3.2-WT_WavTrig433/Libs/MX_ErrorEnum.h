/*
 * MX_ErrorEnum.h
 *
 * Created: 15/06/2011 9:31:59 AM
 *  Author: MC
 */ 


#ifndef MX_ERRORENUM_H_
#define MX_ERRORENUM_H_

typedef enum _mx_err_t {
	mx_err_Success			=  0,
	mx_err_Timeout			= -1,
	mx_err_USBUnplugged		= -2,
	mx_err_InvalidData		= -3,
	mx_err_BadResponse		= -5,
	mx_err_Overflow			= -6,
	mx_err_NotFound			= -7,
	mx_err_InsufficientResources = -8,
	mx_err_ResourceBeingUsed = -9,
	
	mx_err_Unknown			= -20
	
} mx_err_t;





#endif /* MX_ERRORENUM_H_ */