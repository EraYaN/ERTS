normal: default

EXT_DEPS_COMM := ertscommunication
EXT_DEPS_CRC := crc

EXT_DEPS_COMM_LIB := $(addsuffix .a, $(addprefix lib, $(EXT_DEPS_COMM)))
EXT_DEPS_CRC_LIB := $(addsuffix .a, $(addprefix lib, $(EXT_DEPS_CRC)))

EXT_DEPS_COMM_PATH += $(abspath ../../libraries/lib$(EXT_DEPS_COMM))
EXT_DEPS_CRC_PATH += $(abspath ../../libraries/lib$(EXT_DEPS_CRC)-2.0)

EXT_DEPS += $(EXT_DEPS_COMM)
EXT_DEPS += $(EXT_DEPS_CRC)

EXT_DEPS_LIB_NAMES += $(EXT_DEPS_COMM_LIB)
EXT_DEPS_LIB_NAMES += $(EXT_DEPS_CRC_LIB)

HW_SDK_PATH := $(abspath ../../libraries/nRF51_SDK_10.0.0_dc26b5e)
#HW_SDK_PATH := $(abspath ../../libraries/nRF5_SDK_12.3.0_d7731ad)
DFU_TOOL := $(abspath ../../tools/dfu_serial/./serial_dfu.py)

GNU_VERSION := 5.4.1
GNU_PREFIX := arm-none-eabi

TEMPLATE_PATH = $(HW_SDK_PATH)/components/toolchain/gcc

ifeq ($(OS),Windows_NT)
MK := -mkdir
else
MK := mkdir
endif

ifeq ($(OS),Windows_NT)
RM := -del /S /F /Q
else
RM := rm -rf
endif

#echo suspend
ifeq ("$(VERBOSE)","1")
NO_ECHO :=
else
NO_ECHO := @
endif

ifeq ($(shell uname),Darwin)
GNU_INSTALL_ROOT :=
else
GNU_INSTALL_ROOT := $(realpath $(PROJECT_ROOT)/src/gcc-arm-none-eabi-x64/bin/)/
endif

printdebug:
	@echo $(GNU_INSTALL_ROOT)	

# Toolchain commands
CC              := '$(GNU_INSTALL_ROOT)$(GNU_PREFIX)-gcc'
CPP				:= '$(GNU_INSTALL_ROOT)$(GNU_PREFIX)-g++'
AS              := '$(GNU_INSTALL_ROOT)$(GNU_PREFIX)-as'
AR              := '$(GNU_INSTALL_ROOT)$(GNU_PREFIX)-ar' -r
LD              := '$(GNU_INSTALL_ROOT)$(GNU_PREFIX)-ld'
NM              := '$(GNU_INSTALL_ROOT)$(GNU_PREFIX)-nm'
OBJDUMP         := '$(GNU_INSTALL_ROOT)$(GNU_PREFIX)-objdump'
OBJCOPY         := '$(GNU_INSTALL_ROOT)$(GNU_PREFIX)-objcopy'
SIZE            := '$(GNU_INSTALL_ROOT)$(GNU_PREFIX)-size'
STRIP  			:= '$(GNU_INSTALL_ROOT)$(GNU_PREFIX)-strip'
RANLIB			:= '$(GNU_INSTALL_ROOT)$(GNU_PREFIX)-ranlib'
OBJEXT = .o
LIBEXT = .a
EXEEXT =
OFLAG  = -o
XFLAG  = -o

#function for removing duplicates in a list
remduplicates = $(strip $(if $1,$(firstword $1) $(call remduplicates,$(filter-out $(firstword $1),$1))))

HW_SDK_C_SOURCE_FILES += $(abspath $(HW_SDK_PATH)/components/toolchain/system_nrf51.c) 
HW_SDK_C_SOURCE_FILES += $(abspath $(HW_SDK_PATH)/components/drivers_nrf/delay/nrf_delay.c) 
HW_SDK_C_SOURCE_FILES +=$(abspath $(HW_SDK_PATH)/components/libraries/util/app_error.c) 
HW_SDK_C_SOURCE_FILES += $(abspath $(HW_SDK_PATH)/components/libraries/timer/app_timer.c) 
HW_SDK_C_SOURCE_FILES += $(abspath $(HW_SDK_PATH)/components/libraries/util/nrf_assert.c) 
HW_SDK_C_SOURCE_FILES += $(abspath $(HW_SDK_PATH)/components/drivers_nrf/common/nrf_drv_common.c) 
#HW_SDK_C_SOURCE_FILES += $(abspath $(HW_SDK_PATH)/components/drivers_nrf/pstorage/pstorage.c) # Replace with fstorage in components\libraries\fstorage
#HW_SDK_C_SOURCE_FILES += $(abspath $(HW_SDK_PATH)/components/ble/common/ble_advdata.c) 
#HW_SDK_C_SOURCE_FILES += $(abspath $(HW_SDK_PATH)/components/ble/ble_advertising/ble_advertising.c) 
#HW_SDK_C_SOURCE_FILES += $(abspath $(HW_SDK_PATH)/components/ble/common/ble_conn_params.c) 
#HW_SDK_C_SOURCE_FILES += $(abspath $(HW_SDK_PATH)/components/ble/ble_services/ble_nus/ble_nus.c) 
#HW_SDK_C_SOURCE_FILES += $(abspath $(HW_SDK_PATH)/components/ble/common/ble_srv_common.c) 
#HW_SDK_C_SOURCE_FILES += $(abspath $(HW_SDK_PATH)/components/softdevice/common/softdevice_handler/softdevice_handler.c) 

HW_SDK_ASM_SOURCE_FILES  = $(abspath $(HW_SDK_PATH)/components/toolchain/gcc/gcc_startup_nrf51.s)

HW_SDK_INC_PATHS += -I$(abspath $(HW_SDK_PATH)/components/device)
HW_SDK_INC_PATHS += -I$(abspath $(HW_SDK_PATH)/components/toolchain/gcc)
HW_SDK_INC_PATHS += -I$(abspath $(HW_SDK_PATH)/components/toolchain)
HW_SDK_INC_PATHS += -I$(abspath $(HW_SDK_PATH)/components/toolchain/cmsis/include)
HW_SDK_INC_PATHS += -I$(abspath $(HW_SDK_PATH)/components/drivers_nrf/hal)
HW_SDK_INC_PATHS += -I$(abspath $(HW_SDK_PATH)/components/drivers_nrf/delay)
HW_SDK_INC_PATHS += -I$(abspath $(HW_SDK_PATH)/components/softdevice/s110/headers)
HW_SDK_INC_PATHS += -I$(abspath $(HW_SDK_PATH)/components/softdevice/s130/headers)
HW_SDK_INC_PATHS += -I$(abspath $(HW_SDK_PATH)/components/drivers_nrf/config)
HW_SDK_INC_PATHS += -I$(abspath $(HW_SDK_PATH)/components/libraries/log)
HW_SDK_INC_PATHS += -I$(abspath $(HW_SDK_PATH)/components/libraries/log/src)
HW_SDK_INC_PATHS += -I$(abspath $(HW_SDK_PATH)/components/libraries/util)
HW_SDK_INC_PATHS += -I$(abspath $(HW_SDK_PATH)/components/ble/common)
HW_SDK_INC_PATHS += -I$(abspath $(HW_SDK_PATH)/components/drivers_nrf/pstorage)
HW_SDK_INC_PATHS += -I$(abspath $(HW_SDK_PATH)/components/libraries/timer)
HW_SDK_INC_PATHS += -I$(abspath $(HW_SDK_PATH)/components/ble/ble_services/ble_nus)
HW_SDK_INC_PATHS += -I$(abspath $(HW_SDK_PATH)/components/drivers_nrf/common)
HW_SDK_INC_PATHS += -I$(abspath $(HW_SDK_PATH)/components/ble/ble_advertising)
HW_SDK_INC_PATHS += -I$(abspath $(HW_SDK_PATH)/components/libraries/trace)
HW_SDK_INC_PATHS += -I$(abspath $(HW_SDK_PATH)/components/softdevice/common/softdevice_handler)


EXT_DEPS_INC_PATHS += -I$(abspath ../../libraries/libertscommunication)
EXT_DEPS_INC_PATHS += -I$(abspath ../../libraries/libcrc-2.0/include)

EXT_DEPS_LD_PATHS += -L$(abspath $(EXT_DEPS_COMM_PATH)/lib)
EXT_DEPS_LD_PATHS += -L$(abspath $(EXT_DEPS_CRC_PATH)/lib)

#flags common to all targets
COMMON_FLAGS  = -DNRF51
COMMON_FLAGS += -DNRF51822
#COMMON_FLAGS += -DSOFTDEVICE_PRESENT
#COMMON_FLAGS += -DS130
#COMMON_FLAGS += -DS110
#COMMON_FLAGS += -DBLE_STACK_SUPPORT_REQD
COMMON_FLAGS += -mcpu=cortex-m0
COMMON_FLAGS += -mthumb -mabi=aapcs # abi=Application Binary Interface
COMMON_FLAGS += -Wall -Werror -O3
COMMON_FLAGS += -mfloat-abi=soft # we don't want floats - no hardware fpu
# keep every function in separate section. This will allow linker to dump unused functions
COMMON_FLAGS += -ffunction-sections -fdata-sections -fno-strict-aliasing
COMMON_FLAGS += -fno-builtin --short-enums -fshort-wchar

# keep every function in separate section. This will allow linker to dump unused functions
SHARED_LDFLAGS += -Xlinker -Map=$(LISTING_DIRECTORY)/$(OUTPUT_FILENAME).map
SHARED_LDFLAGS += -mthumb -mabi=aapcs -L $(TEMPLATE_PATH) -T$(LINKER_SCRIPT)
SHARED_LDFLAGS += -mcpu=cortex-m0
# let linker to dump unused sections
SHARED_LDFLAGS += -Wl,--gc-sections
# suppress wchar 2-byte vs 4-byte warnings
SHARED_LDFLAGS += -Wl,--no-wchar-size-warning
# use newlib in nano version
SHARED_LDFLAGS += --specs=nano.specs -lc -lnosys

$(EXT_DEPS_COMM_LIB):
	@echo 	Building $@
	$(MAKE) -C $(EXT_DEPS_COMM_PATH) libonly

$(EXT_DEPS_CRC_LIB):
	@echo 	Building $@
	$(MAKE) -C $(EXT_DEPS_CRC_PATH) -f Makefile.arm libonly

$(addsuffix clean, $(EXT_DEPS_CRC_LIB)):
	$(MAKE) -C $(EXT_DEPS_CRC_PATH) -e clean

$(addsuffix clean, $(EXT_DEPS_COMM_LIB)):
	$(MAKE) -C $(EXT_DEPS_COMM_PATH) -e clean
