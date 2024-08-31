cmake_minimum_required(VERSION 3.11)

project(MultiCell-TestBench-Automation)

set(CMAKE_AUTOMOC ON)  # Ensure that moc is automatically run
set(CMAKE_AUTORCC ON)
set(CMAKE_AUTOUIC ON)

set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

set(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -fpermissive")

# Find Qt6 package
find_package(Qt6 REQUIRED COMPONENTS Widgets)

# Define the path to the PCANBasic files
set(PCAN_DIR "N:/Programming_VS/Cpp/multicell-testbench-automation/MultiCell-TestBench-Automation/pcan")
set(PCAN_INCLUDE_DIR "${PCAN_DIR}/Include")
set(PCAN_LIB_DIR "${PCAN_DIR}/x64/VC_LIB")

# Include directories for PCANBasic and other dependencies
include_directories(${PCAN_INCLUDE_DIR})

# Add the library
find_library(PCANBasic_LIBRARIES NAMES PCANBasic PATHS ${PCAN_LIB_DIR})

# Check if the library was found
if (NOT PCANBasic_LIBRARIES)
    message(FATAL_ERROR "PCANBasic library not found. Please set PCAN_LIB_DIR correctly.")
endif()

# Add can-dbc-parser library files
#set(CAN_DBC_PARSER_DIR "N:/Programming_VS/Cpp/multicell-testbench-automation/MultiCell-TestBench-Automation/can_dbc_parser/dbcppp") # Update this path with the correct path to the cloned repo
#include_directories(${CAN_DBC_PARSER_DIR}/include/dbcppp)

# Source files for can-dbc-parser
#file(GLOB CAN_DBC_PARSER_SOURCES ${CAN_DBC_PARSER_DIR}/src/*.cpp)

# Add the executable target
add_executable(MultiCell-TestBench-Automation 
  main.cpp 
  MainWindow.cpp 
  MainWindow.h
  can_interface.cpp
  can_interface.hpp
  TestBenchOperations.cpp
  TestBenchOperations.hpp
  TestOperations.cpp
  TestOperations.hpp
  TestType.hpp
  #${CAN_DBC_PARSER_SOURCES}  # Add the can-dbc-parser source files
)

# Link the Qt6 Widgets and PCANBasic library to the target
target_link_libraries(MultiCell-TestBench-Automation 
  Qt6::Widgets
  ${PCANBasic_LIBRARIES}
)

# Ensure that the runtime can find the PCANBasic DLL
set_target_properties(MultiCell-TestBench-Automation PROPERTIES
  RUNTIME_OUTPUT_DIRECTORY "${CMAKE_BINARY_DIR}/bin"
)

# Optionally, you can add post-build commands to copy the DLL to the output directory
add_custom_command(TARGET MultiCell-TestBench-Automation POST_BUILD
  COMMAND ${CMAKE_COMMAND} -E copy_if_different
  "${PCAN_DIR}/x64/PCANBasic.dll"
  $<TARGET_FILE_DIR:MultiCell-TestBench-Automation>
)
