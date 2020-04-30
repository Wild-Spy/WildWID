#include <stdio.h>
#include <stdarg.h>
#include "../avr/io.h"

int sprintf_P(char* s, const char* format, ...) {
    va_list args;
    va_start(args, format);
    int res =  vsprintf(s, format, args); 
    va_end(args);
    return res;
}

USART_tx_String(USART_t m_USART, const char* data) {
    printf("Print: %s", data);
}

USART_tx_String_P(USART_t m_USART, const char* data) {
    printf("Print: %s", data);
}

void _delay_ms(double __ms) {
    //do nothing for now
}

