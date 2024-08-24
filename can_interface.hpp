#ifndef CAN_INTERFACE_HPP
#define CAN_INTERFACE_HPP

#include "PCANBasic.h"   // Include the PCANBasic library
#include <windows.h>
#include <mutex>
#include <vector>

class CANInterface {
public:
    CANInterface(TPCANHandle handle);
    ~CANInterface();

    bool sendCANMessage(TPCANMsg& message);  // Function to send CAN messages
    bool readCANMessage(TPCANMsg& message);  // Function to read CAN messages

private:
    TPCANHandle m_handle;         // CAN channel/handle to work with
    std::mutex m_mutex;           // Mutex for thread safety
};

#endif // CAN_INTERFACE_HPP
