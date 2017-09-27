PROJECT_NAME := libertscommunication

include ../Shared.mk

CPP_SOURCE_FILES += \
$(abspath ./acknowledge_data.cpp) \
$(abspath ./mode_switch_data.cpp) \
$(abspath ./packet.cpp) \
$(abspath ./remote_control_data.cpp) \
$(abspath ./telemetry_data.cpp) \
$(abspath ./parameter_data.cpp) \
$(abspath ./exception_data.cpp)

INC_PATHS += -I$(abspath ./)
INC_PATHS += $(EXT_DEPS_INC_PATHS) # from Shared.mk

LD_PATHS += $(EXT_DEPS_LD_PATHS) # from Shared.mk

OBJECT_DIRECTORY = obj
LISTING_DIRECTORY = $(OBJECT_DIRECTORY)
OUTPUT_LIBRARY_DIRECTORY = lib

# Sorting removes duplicates
BUILD_DIRECTORIES := $(sort $(OBJECT_DIRECTORY) $(OUTPUT_LIBRARY_DIRECTORY) $(LISTING_DIRECTORY) )

CXXFLAGS = --std=c++11
CXXFLAGS += $(COMMON_FLAGS)

# keep every function in separate section. This will allow linker to dump unused functions
LDFLAGS += $(SHARED_LDFLAGS)

LIBS = -lm -lcrc

libonly: default
#It should not clean by default, that is stupid.
default: $(PROJECT_NAME)

deps: $(EXT_DEPS_LIB_NAMES)

#target for printing all targets
help:
	@echo following targets are available:
	@echo 	$(PROJECT_NAME)

CPP_SOURCE_FILE_NAMES = $(notdir $(CPP_SOURCE_FILES))
CPP_PATHS = $(call remduplicates, $(dir $(CPP_SOURCE_FILES) ) )
CPP_OBJECTS = $(addprefix $(OBJECT_DIRECTORY)/, $(CPP_SOURCE_FILE_NAMES:.cpp=.o) )

vpath %.cpp $(CPP_PATHS)

OUTPUT_FILENAME := $(PROJECT_NAME)

$(PROJECT_NAME): LINKER_SCRIPT=$(abspath ../shared.ld)
$(PROJECT_NAME): $(EXT_DEPS_CRC_LIB) $(BUILD_DIRECTORIES) $(CPP_OBJECTS)
	@echo Packing target: $(OUTPUT_FILENAME).a
	$(NO_ECHO)$(AR)cvs $(OUTPUT_LIBRARY_DIRECTORY)/$(OUTPUT_FILENAME).a $(OBJECT_DIRECTORY)/*.o

## Create build directories
$(BUILD_DIRECTORIES):
#	echo $(MAKEFILE_NAME)
	$(MK) $@

$(OBJECT_DIRECTORY)/%.o: %.cpp
	@echo Compiling file: $(notdir $<)
	$(NO_ECHO)$(CPP) $(CXXFLAGS) $(INC_PATHS) -c -o $@ $<

clean: $(addsuffix clean, $(EXT_DEPS_CRC_LIB))
	$(RM) $(BUILD_DIRECTORIES)

cleanobj:
	$(RM) $(BUILD_DIRECTORIES)/*.o
