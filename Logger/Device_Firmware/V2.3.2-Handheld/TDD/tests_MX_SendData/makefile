
CC = gcc
CFLAGS = -g -Wall -std=gnu99
INCLUDES = ../avr/
#TARGET = tests_M_CountList
TARGET = tests_MX_SendData
BUILDDIR = build/

DEPFOLDERS = ../avr/stubs/ ../../Libs/ ../../Libs/ ../avr/stubs/ ../../Libs/ ../avr/stubs/ ../avr/stubs/ ../avr/stubs/ ../avr/stubs/ ../avr/stubs/
DEPSOURCES = count_list_stub MX_SendData SettingsManager eeprom_stub MX_DateTime extra MX_Iridium9603 flash_logger_stub atmel_time llut_stub
DEPSOURCESFULL = $(addsuffix .c,$(join $(DEPFOLDERS),$(DEPSOURCES)))
DEPSFULL = $(addsuffix .o,$(addprefix $(BUILDDIR),$(DEPSOURCES)))

define ONERULE
$1: $2
	build $$^ > $$@
endef

all: $(TARGET)
	./${TARGET}

$(TARGET): $(BUILDDIR)$(TARGET).o $(DEPSFULL)
#$(TARGET): $(DEPSFULL)
	@echo $(DEPSOURCESFULL)
	$(CC) $(CFLAGS) -I$(INCLUDES) -o $(TARGET) $(BUILDDIR)$(TARGET).o $(DEPSFULL)

$(BUILDDIR)$(TARGET).o: $(TARGET).c
	$(CC) $(CFLAGS) -I$(INCLUDES) -c $(TARGET).c -o $(BUILDDIR)$(TARGET).o

#$(foreach $(eval $(call ONERULE,t1,p1 p2 p3))

$(DEPSFULL): %.o : $(filter %$(notdir $*).c, $(DEPSOURCESFULL))
	#@echo $@ #$(filter %$(notdir $*).c, $(DEPSOURCESFULL))
	$(CC) $(CFLAGS) -I$(INCLUDES) -c $(filter %$(notdir $*).c, $(DEPSOURCESFULL)) -o $@


#$(BUILDDIR)$(DEP1).o: $(DEP1FULL)
#@echo BUILDING M_COUNTLIST
#	$(CC) $(CFLAGS) -I$(INCLUDES) -c $(DEP1FOLDER)$(DEP1).c -o $(BUILDDIR)$(DEP1).o

clean:
	$(RM) $(TARGET)
	$(RM) $(DEPSFULL)
	$(RM) $(BUILDDIR)$(TARGET).o
