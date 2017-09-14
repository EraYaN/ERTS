PROJECT_NAME := erts-quad

include ../../libraries/Shared.mk


export OUTPUT_FILENAME
#MAKEFILE_NAME := $(CURDIR)/$(word $(words $(MAKEFILE_LIST)),$(MAKEFILE_LIST))
MAKEFILE_NAME := $(MAKEFILE_LIST)
MAKEFILE_DIR := $(dir $(MAKEFILE_NAME) )




#source common to all targets
C_SOURCE_FILES += \
$(abspath ./drivers/gpio.c) \
$(abspath ./drivers/timers.c) \
$(abspath ./drivers/uart.c) \
$(abspath ./drivers/queue.c) \
$(abspath ./drivers/twi.c) \
$(abspath ./drivers/adc.c) \
$(abspath ./drivers/baro.c) \
$(abspath ./drivers/ble.c) \
$(abspath ./drivers/spi_flash.c) \
$(abspath ./drivers/invensense/inv_mpu.c) \
$(abspath ./drivers/invensense/inv_mpu_dmp_motion_driver.c) \
$(abspath ./drivers/invensense/ml.c) \
$(abspath ./drivers/invensense/mpu_wrapper.c) \
$(HW_SDK_C_SOURCE_FILES) # from Shared.mk

CPP_SOURCE_FILES += \
$(abspath ./$(PROJECT_NAME).cpp) \
$(abspath ./quad.cpp)

#assembly files common to all targets
ASM_SOURCE_FILES  = $(HW_SDK_ASM_SOURCE_FILES) # from Shared.mk

#includes common to all targets
INC_PATHS += -I$(abspath ./)
INC_PATHS += -I$(abspath ./comm)
INC_PATHS += -I$(abspath ./drivers)
INC_PATHS += -I$(abspath ./drivers/config)
INC_PATHS += -I$(abspath ./shared)
INC_PATHS += $(EXT_DEPS_INC_PATHS) # from Shared.mk
INC_PATHS += $(HW_SDK_INC_PATHS) # from Shared.mk

LD_PATHS += $(EXT_DEPS_LD_PATHS) # from Shared.mk

OBJECT_DIRECTORY = obj
LISTING_DIRECTORY = $(OBJECT_DIRECTORY)
OUTPUT_BINARY_DIRECTORY = bin

# Sorting removes duplicates
BUILD_DIRECTORIES := $(sort $(OBJECT_DIRECTORY) $(OUTPUT_BINARY_DIRECTORY) $(LISTING_DIRECTORY) )

CFLAGS = --std=gnu11
CFLAGS += $(COMMON_FLAGS) # from Shared.mk

CXXFLAGS = --std=c++11
CXXFLAGS += $(COMMON_FLAGS) # from Shared.mk

LDFLAGS += $(SHARED_LDFLAGS)
# math library
LDFLAGS += -lm

LIBS = -lm -l$(EXT_DEPS_COMM) -l$(EXT_DEPS_CRC)

# Assembler flags
ASMFLAGS += -x assembler-with-cpp
ASMFLAGS += -DNRF51
ASMFLAGS += -DSOFTDEVICE_PRESENT
ASMFLAGS += -DS110
ASMFLAGS += -DBLE_STACK_SUPPORT_REQD
#default target - first one defined

#It should not clean by default, that is stupid.
default: $(PROJECT_NAME)

#building all targets
all: clean
	$(NO_ECHO)$(MAKE) -f $(MAKEFILE_NAME) -C $(MAKEFILE_DIR) -e cleanobj
	$(NO_ECHO)$(MAKE) -f $(MAKEFILE_NAME) -C $(MAKEFILE_DIR) -e $(PROJECT_NAME)	

deps: $(EXT_DEPS_LIB_NAMES) 

#target for printing all targets
help:
	@echo following targets are available:
	@echo 	$(PROJECT_NAME)

# SDK_C_SOURCE_FILE_NAMES = $(notdir $(SDK_C_SOURCE_FILES))
# SDK_C_PATHS = $(call remduplicates, $(dir $(SDK_C_SOURCE_FILES) ) )
# SDK_C_OBJECTS = $(addprefix $(OBJECT_DIRECTORY)/, $(SDK_C_SOURCE_FILE_NAMES:.c=.o) )

C_SOURCE_FILE_NAMES = $(notdir $(C_SOURCE_FILES))
C_PATHS = $(call remduplicates, $(dir $(C_SOURCE_FILES) ) )
C_OBJECTS = $(addprefix $(OBJECT_DIRECTORY)/, $(C_SOURCE_FILE_NAMES:.c=.o) )

CPP_SOURCE_FILE_NAMES = $(notdir $(CPP_SOURCE_FILES))
CPP_PATHS = $(call remduplicates, $(dir $(CPP_SOURCE_FILES) ) )
CPP_OBJECTS = $(addprefix $(OBJECT_DIRECTORY)/, $(CPP_SOURCE_FILE_NAMES:.cpp=.o) )

ASM_SOURCE_FILE_NAMES = $(notdir $(ASM_SOURCE_FILES))
ASM_PATHS = $(call remduplicates, $(dir $(ASM_SOURCE_FILES) ))
ASM_OBJECTS = $(addprefix $(OBJECT_DIRECTORY)/, $(ASM_SOURCE_FILE_NAMES:.s=.o) )

vpath %.c $(C_PATHS)
vpath %.cpp $(CPP_PATHS)
vpath %.s $(ASM_PATHS)

OBJECTS = $(C_OBJECTS) $(CPP_OBJECTS) $(ASM_OBJECTS)

OUTPUT_FILENAME := $(PROJECT_NAME)

$(PROJECT_NAME): LINKER_SCRIPT=$(abspath ../../libraries/shared.ld)
$(PROJECT_NAME): $(EXT_DEPS_COMM_LIB) $(EXT_DEPS_CRC_LIB) $(BUILD_DIRECTORIES) $(OBJECTS)
	@echo Linking target: $(OUTPUT_FILENAME).out
	$(NO_ECHO)$(CPP) $(LDFLAGS) $(LD_PATHS) $(C_OBJECTS) $(CPP_OBJECTS) $(ASM_OBJECTS) $(LIBS) -o $(OUTPUT_BINARY_DIRECTORY)/$(OUTPUT_FILENAME).out
	$(NO_ECHO)$(MAKE) -f $(MAKEFILE_NAME) -C $(MAKEFILE_DIR) -e finalize

## Create build directories
$(BUILD_DIRECTORIES):
#	echo $(MAKEFILE_NAME)
	$(MK) $@
	
# Create objects from C SRC files
$(OBJECT_DIRECTORY)/%.o: %.c
	@echo Compiling file: $(notdir $<)
	$(NO_ECHO)$(CC) $(CFLAGS) $(INC_PATHS) -c -o $@ $<

$(OBJECT_DIRECTORY)/%.o: %.cpp
	@echo Compiling file: $(notdir $<)
	$(NO_ECHO)$(CPP) $(CXXFLAGS) $(INC_PATHS) -c -o $@ $<

# Assemble files
$(OBJECT_DIRECTORY)/%.o: %.s
	@echo Compiling file: $(notdir $<)
	$(NO_ECHO)$(CC) $(ASMFLAGS) $(INC_PATHS) -c -o $@ $<


# Link
$(OUTPUT_BINARY_DIRECTORY)/$(OUTPUT_FILENAME).out: $(BUILD_DIRECTORIES) $(OBJECTS)
	@echo Linking target: $(OUTPUT_FILENAME).out
	$(NO_ECHO)$(CPP) $(LDFLAGS) $(LD_PATHS) $(OBJECTS) $(LIBS) -o $(OUTPUT_BINARY_DIRECTORY)/$(OUTPUT_FILENAME).out


## Create binary .bin file from the .out file
$(OUTPUT_BINARY_DIRECTORY)/$(OUTPUT_FILENAME).bin: $(OUTPUT_BINARY_DIRECTORY)/$(OUTPUT_FILENAME).out
	@echo Preparing: $(OUTPUT_FILENAME).bin
	$(NO_ECHO)$(OBJCOPY) -O binary $(OUTPUT_BINARY_DIRECTORY)/$(OUTPUT_FILENAME).out $(OUTPUT_BINARY_DIRECTORY)/$(OUTPUT_FILENAME).bin

## Create binary .hex file from the .out file
$(OUTPUT_BINARY_DIRECTORY)/$(OUTPUT_FILENAME).hex: $(OUTPUT_BINARY_DIRECTORY)/$(OUTPUT_FILENAME).out
	@echo Preparing: $(OUTPUT_FILENAME).hex
	$(NO_ECHO)$(OBJCOPY) -O ihex $(OUTPUT_BINARY_DIRECTORY)/$(OUTPUT_FILENAME).out $(OUTPUT_BINARY_DIRECTORY)/$(OUTPUT_FILENAME).hex

finalize: genbin genhex echosize

genbin:
	@echo Preparing: $(OUTPUT_FILENAME).bin
	$(NO_ECHO)$(OBJCOPY) -O binary $(OUTPUT_BINARY_DIRECTORY)/$(OUTPUT_FILENAME).out $(OUTPUT_BINARY_DIRECTORY)/$(OUTPUT_FILENAME).bin

## Create binary .hex file from the .out file
genhex:
	@echo Preparing: $(OUTPUT_FILENAME).hex
	$(NO_ECHO)$(OBJCOPY) -O ihex $(OUTPUT_BINARY_DIRECTORY)/$(OUTPUT_FILENAME).out $(OUTPUT_BINARY_DIRECTORY)/$(OUTPUT_FILENAME).hex

echosize:
	-@echo ''
	$(NO_ECHO)$(SIZE) $(OUTPUT_BINARY_DIRECTORY)/$(OUTPUT_FILENAME).out
	-@echo ''

clean: $(addsuffix clean, $(EXT_DEPS_CRC_LIB) $(EXT_DEPS_COMM_LIB))
	$(RM) $(BUILD_DIRECTORIES)
	

cleanobj:
	$(RM) $(BUILD_DIRECTORIES)/*.o

# Upload should not build.
upload:
	$(DFU_TOOL) $(OUTPUT_BINARY_DIRECTORY)/$(OUTPUT_FILENAME).bin

upload_mac:
	python $(DFU_TOOL) -p "/dev/tty.usbserial-DN00P2TE" $(OUTPUT_BINARY_DIRECTORY)/$(OUTPUT_FILENAME).bin
