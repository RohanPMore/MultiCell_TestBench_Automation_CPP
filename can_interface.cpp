#include "can_interface.hpp"
#include <iostream>
#include "PCANBasic.h"
#include <windows.h>

CANInterface::CANInterface(TPCANHandle handle) : m_handle(handle) {
    // Initialize the PCANBasic library for the given CAN handle
    TPCANStatus status = CAN_Initialize(m_handle, PCAN_BAUD_500K);
    if (status != PCAN_ERROR_OK) {
        std::cerr << "CAN Initialization failed! Error code: " << status << std::endl;
    }
}

CANInterface::~CANInterface() {
    // Uninitialize the PCANBasic library
    CAN_Uninitialize(m_handle);
}

bool CANInterface::sendCANMessage(TPCANMsg& message) {
    std::lock_guard<std::mutex> lock(m_mutex);  // Ensure thread safety
    TPCANStatus status = CAN_Write(m_handle, &message);
    if (status != PCAN_ERROR_OK) {
        std::cerr << "CAN Write failed! Error code: " << status << std::endl;
        return false;
    }
    return true;
}

bool CANInterface::readCANMessage(TPCANMsg& message) {
    std::lock_guard<std::mutex> lock(m_mutex);  // Ensure thread safety
    TPCANStatus status = CAN_Read(m_handle, &message, nullptr);
    if (status != PCAN_ERROR_OK) {
        if (status != PCAN_ERROR_QRCVEMPTY) { // Ignore empty queue errors
            std::cerr << "CAN Read failed! Error code: " << status << std::endl;
        }
        return false;
    }
    return true;
}
