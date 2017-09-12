PROJECT_NAME := erts-quad
PROJECT_NAME_PC := erts-pc

HW_SDK_PATH := ../nRF51_SDK_10.0.0_dc26b5e
DFU_TOOL := ../tools/dfu_serial/./serial_dfu.py

GNU_VERSION := 5.4.1
GNU_PREFIX := arm-none-eabi

export OUTPUT_FILENAME
#MAKEFILE_NAME := $(CURDIR)/$(word $(words $(MAKEFILE_LIST)),$(MAKEFILE_LIST))
MAKEFILE_NAME := $(MAKEFILE_LIST)
MAKEFILE_DIR := $(dir $(MAKEFILE_NAME) )

TEMPLATE_PATH = $(HW_SDK_PATH)/components/toolchain/gcc

ifeq ($(OS),Windows_NT)
#include $(TEMPLATE_PATH)/Makefile.windows
else
#include $(TEMPLATE_PATH)/Makefile.posix
endif

MK := mkdir
RM := rm -rf

#echo suspend
ifeq ("$(VERBOSE)","1")
NO_ECHO :=
else
NO_ECHO := @
endif

# Toolchain commands
CC              := '$(GNU_PREFIX)-gcc'
CPP             := '$(GNU_PREFIX)-g++'
AS              := '$(GNU_PREFIX)-as'
AR              := '$(GNU_PREFIX)-ar' -r
LD              := '$(GNU_PREFIX)-ld'
NM              := '$(GNU_PREFIX)-nm'
OBJDUMP         := '$(GNU_PREFIX)-objdump'
OBJCOPY         := '$(GNU_PREFIX)-objcopy'
SIZE            := '$(GNU_PREFIX)-size'

#function for removing duplicates in a list
remduplicates = $(strip $(if $1,$(firstword $1) $(call remduplicates,$(filter-out $(firstword $1),$1))))

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
$(abspath ./invensense/inv_mpu.c) \
$(abspath ./invensense/inv_mpu_dmp_motion_driver.c) \
$(abspath ./invensense/ml.c) \
$(abspath ./invensense/mpu_wrapper.c) \
$(abspath $(HW_SDK_PATH)/components/toolchain/system_nrf51.c) \
$(abspath $(HW_SDK_PATH)/components/drivers_nrf/delay/nrf_delay.c) \
$(abspath $(HW_SDK_PATH)/components/libraries/util/app_error.c) \
$(abspath $(HW_SDK_PATH)/components/libraries/timer/app_timer.c) \
$(abspath $(HW_SDK_PATH)/components/libraries/util/nrf_assert.c) \
$(abspath $(HW_SDK_PATH)/components/drivers_nrf/common/nrf_drv_common.c) \
$(abspath $(HW_SDK_PATH)/components/drivers_nrf/pstorage/pstorage.c) \
$(abspath $(HW_SDK_PATH)/components/ble/common/ble_advdata.c) \
$(abspath $(HW_SDK_PATH)/components/ble/ble_advertising/ble_advertising.c) \
$(abspath $(HW_SDK_PATH)/components/ble/common/ble_conn_params.c) \
$(abspath $(HW_SDK_PATH)/components/ble/ble_services/ble_nus/ble_nus.c) \
$(abspath $(HW_SDK_PATH)/components/ble/common/ble_srv_common.c) \
$(abspath $(HW_SDK_PATH)/components/softdevice/common/softdevice_handler/softdevice_handler.c) \

CPP_SOURCE_FILES += \
$(abspath ./$(PROJECT_NAME).cpp) \
$(abspath ./quad.cpp) \

#assembly files common to all targets
ASM_SOURCE_FILES  = $(abspath $(HW_SDK_PATH)/components/toolchain/gcc/gcc_startup_nrf51.s)

#includes common to all targets
INC_PATHS += -I$(abspath ./)
INC_PATHS += -I$(abspath ./invensense)
INC_PATHS += -I$(abspath ./drivers)
INC_PATHS += -I$(abspath ./drivers/config)
INC_PATHS += -I$(abspath ./shared)
INC_PATHS += -I$(abspath $(HW_SDK_PATH)/components/device)
INC_PATHS += -I$(abspath $(HW_SDK_PATH)/components/toolchain/gcc)
INC_PATHS += -I$(abspath $(HW_SDK_PATH)/components/toolchain)
INC_PATHS += -I$(abspath $(HW_SDK_PATH)/components/drivers_nrf/hal)
INC_PATHS += -I$(abspath $(HW_SDK_PATH)/components/drivers_nrf/delay)
INC_PATHS += -I$(abspath $(HW_SDK_PATH)/components/softdevice/s110/headers)
INC_PATHS += -I$(abspath $(HW_SDK_PATH)/components/drivers_nrf/config)
INC_PATHS += -I$(abspath $(HW_SDK_PATH)/components/libraries/util)
INC_PATHS += -I$(abspath $(HW_SDK_PATH)/components/ble/common)
INC_PATHS += -I$(abspath $(HW_SDK_PATH)/components/drivers_nrf/pstorage)
INC_PATHS += -I$(abspath $(HW_SDK_PATH)/components/libraries/timer)
INC_PATHS += -I$(abspath $(HW_SDK_PATH)/components/ble/ble_services/ble_nus)
INC_PATHS += -I$(abspath $(HW_SDK_PATH)/components/drivers_nrf/common)
INC_PATHS += -I$(abspath $(HW_SDK_PATH)/components/ble/ble_advertising)
INC_PATHS += -I$(abspath $(HW_SDK_PATH)/components/libraries/trace)
INC_PATHS += -I$(abspath $(HW_SDK_PATH)/components/softdevice/common/softdevice_handler)


OBJECT_DIRECTORY = _build
LISTING_DIRECTORY = $(OBJECT_DIRECTORY)
OUTPUT_BINARY_DIRECTORY = $(OBJECT_DIRECTORY)

# Sorting removes duplicates
BUILD_DIRECTORIES := $(sort $(OBJECT_DIRECTORY) $(OUTPUT_BINARY_DIRECTORY) $(LISTING_DIRECTORY) )

#flags common to all targets
COMMON_FLAGS  = -DNRF51
COMMON_FLAGS += -DSOFTDEVICE_PRESENT
COMMON_FLAGS += -DS110
COMMON_FLAGS += -DBLE_STACK_SUPPORT_REQD
COMMON_FLAGS += -mcpu=cortex-m0
COMMON_FLAGS += -mthumb -mabi=aapcs # abi=Application Binary Interface
COMMON_FLAGS += -Wall -Werror -O3
COMMON_FLAGS += -mfloat-abi=soft # we don't want floats - no hardware fpu
# keep every function in separate section. This will allow linker to dump unused functions
COMMON_FLAGS += -ffunction-sections -fdata-sections -fno-strict-aliasing
COMMON_FLAGS += -fno-builtin --short-enums

CFLAGS = --std=gnu11
CFLAGS += $(COMMON_FLAGS)

CXXFLAGS = --std=c++11
CXXFLAGS += $(COMMON_FLAGS)

# keep every function in separate section. This will allow linker to dump unused functions
LDFLAGS += -Xlinker -Map=$(LISTING_DIRECTORY)/$(OUTPUT_FILENAME).map
LDFLAGS += -mthumb -mabi=aapcs -L $(TEMPLATE_PATH) -T$(LINKER_SCRIPT)
LDFLAGS += -mcpu=cortex-m0
# let linker to dump unused sections
LDFLAGS += -Wl,--gc-sections
# use newlib in nano version
LDFLAGS += --specs=nano.specs -lc -lnosys
# math library
LDFLAGS += -lm
LIBS = -lm

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
vpath %.cpp $(C_PATHS)
vpath %.s $(ASM_PATHS)

OBJECTS = $(C_OBJECTS) $(CPP_OBJECTS) $(ASM_OBJECTS)

OUTPUT_FILENAME := $(PROJECT_NAME)

$(PROJECT_NAME): LINKER_SCRIPT=$(PROJECT_NAME).ld
$(PROJECT_NAME): $(BUILD_DIRECTORIES) $(OBJECTS)
	@echo Linking target: $(OUTPUT_FILENAME).out
	$(NO_ECHO)$(CPP) $(LDFLAGS) $(C_OBJECTS) $(CPP_OBJECTS) $(ASM_OBJECTS) $(LIBS) -o $(OUTPUT_BINARY_DIRECTORY)/$(OUTPUT_FILENAME).out
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
	$(NO_ECHO)$(CPP) $(LDFLAGS) $(OBJECTS) $(LIBS) -o $(OUTPUT_BINARY_DIRECTORY)/$(OUTPUT_FILENAME).out


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

clean:
	$(RM) $(BUILD_DIRECTORIES)

cleanobj:
	$(RM) $(BUILD_DIRECTORIES)/*.o

# Upload should not build.
upload:
	$(DFU_TOOL) $(OUTPUT_BINARY_DIRECTORY)/$(OUTPUT_FILENAME).bin

upload_mac:
	python $(DFU_TOOL) -p "/dev/tty.usbserial-DN00P2TE" $(OUTPUT_BINARY_DIRECTORY)/$(OUTPUT_FILENAME).bin

pc:
	cd ../$(PROJECT_NAME_PC)/; make

pc-run:
	cd ../$(PROJECT_NAME_PC)/; make run

# Upload should not build.
upload-run: pc
	$(DFU_TOOL) $(OUTPUT_BINARY_DIRECTORY)/$(OUTPUT_FILENAME).bin
	cd ../$(PROJECT_NAME_PC)/; make run
