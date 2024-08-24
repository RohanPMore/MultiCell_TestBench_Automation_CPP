#include "pch.h"
#include "PCANBasicCLR.h"

using namespace Peak::Can::Basic;
using namespace System;

public ref class GetSetParameter
{
#pragma region Defines
private:
    /// <summary>
    /// Sets the PCANHandle (Hardware Channel)
    /// </summary>
    literal TPCANHandle PcanHandle = PCANBasic::PCAN_USBBUS1;
    /// <summary>
    /// Sets the desired connection mode (CAN = false / CAN-FD = true)
    /// </summary>
    literal bool IsFD = false;
    /// <summary>
    /// Sets the bitrate for normal CAN devices
    /// </summary>
    literal TPCANBaudrate Bitrate = TPCANBaudrate::PCAN_BAUD_500K;
    /// <summary>
    /// Sets the bitrate for CAN FD devices. 
    /// Example - Bitrate Nom: 1Mbit/s Data: 2Mbit/s:
    ///   "f_clock_mhz=20, nom_brp=5, nom_tseg1=2, nom_tseg2=1, nom_sjw=1, data_brp=2, data_tseg1=3, data_tseg2=1, data_sjw=1"
    /// </summary>
    literal TPCANBitrateFD BitrateFD = "f_clock_mhz=20, nom_brp=5, nom_tseg1=2, nom_tseg2=1, nom_sjw=1, data_brp=2, data_tseg1=3, data_tseg2=1, data_sjw=1";
#pragma endregion

#pragma region Members
private:
    /// <summary>
    /// Shows if DLL was found
    /// </summary>
    bool m_DLLFound;
#pragma endregion

    /// <summary>
    /// Starts the PCANBasic Sample
    /// </summary>
public: GetSetParameter()
{
    ShowConfigurationHelp(); // Shows information about this sample
    ShowCurrentConfiguration(); // Shows the current parameters configuration

    // Checks if PCANBasic.dll is available, if not, the program terminates
    m_DLLFound = CheckForLibrary();
    if (!m_DLLFound)
        return;

    TPCANStatus stsResult;
    // Initialization of the selected channel
    if (IsFD)
        stsResult = PCANBasic::InitializeFD(PcanHandle, (String^)BitrateFD);
    else
        stsResult = PCANBasic::Initialize(PcanHandle, Bitrate);

    if (stsResult != TPCANStatus::PCAN_ERROR_OK)
    {
        Console::WriteLine("Can not initialize. Please check the defines in the code.");
        ShowStatus(stsResult);
        Console::WriteLine();
        Console::WriteLine("Press any key to close");
        Console::Read();
        return;
    }

    Console::WriteLine("Successfully initialized.");
    Console::WriteLine("Press any key to get/set parameter");
    Console::ReadKey();
    Console::WriteLine();

    RunSelectedCommands();

    Console::WriteLine();
    Console::WriteLine("Press any key to close");
    Console::ReadKey();
}

public: ~GetSetParameter()
{
    if (m_DLLFound)
        PCANBasic::Uninitialize(PCANBasic::PCAN_NONEBUS);
}

#pragma region Main-Functions
private:
    /// <summary>
    /// Runs all commands for get or set parameters
    /// </summary>
    void RunSelectedCommands()
    {
        // Fill commands here 
        Console::WriteLine("Fill \"RunSelectedCommands\"-function with parameter functions from \"Parameter commands\"-Region in the code.");
    }

#pragma region Parameter commands
private:
#pragma region PCAN_DEVICE_ID
    /// <summary>
    /// Shows device identifier parameter
    /// </summary>
    void GetPCAN_DEVICE_ID()
    {
        UInt32 iDeviceID;
        TPCANStatus stsResult = PCANBasic::GetValue(PcanHandle, TPCANParameter::PCAN_DEVICE_ID, iDeviceID, sizeof(UInt32));

        if (stsResult == TPCANStatus::PCAN_ERROR_OK)
        {
            Console::WriteLine("-----------------------------------------------------------------------------------------");
            Console::WriteLine("Get PCAN_DEVICE_ID: " + iDeviceID);
            Console::WriteLine();
        }
        else
            ShowStatus(stsResult);
    }


    /// <summary>
    /// Sets device identifier parameter
    /// </summary>
    /// <param name="iDeviceID"></param>
    void SetPCAN_DEVICE_ID(UInt32 iDeviceID)
    {
        TPCANStatus stsResult = PCANBasic::SetValue(PcanHandle, TPCANParameter::PCAN_DEVICE_ID, iDeviceID, sizeof(UInt32));

        if (stsResult == TPCANStatus::PCAN_ERROR_OK)
        {
            Console::WriteLine("-----------------------------------------------------------------------------------------");
            Console::WriteLine("Set PCAN_DEVICE_ID: " + iDeviceID);
            Console::WriteLine();
        }
        else
            ShowStatus(stsResult);
    }
#pragma endregion

#pragma region PCAN_ATTACHED_CHANNELS
    /// <summary>
    /// Shows all information about attached channels
    /// </summary>
    void GetPCAN_ATTACHED_CHANNELS()
    {
        UInt32 iChannelsCount;
        TPCANStatus stsResult = PCANBasic::GetValue(PCANBasic::PCAN_NONEBUS, TPCANParameter::PCAN_ATTACHED_CHANNELS_COUNT, iChannelsCount, sizeof(UInt32));

        if (stsResult == TPCANStatus::PCAN_ERROR_OK)
        {
            array<TPCANChannelInformation>^ ciChannelInformation = gcnew array<TPCANChannelInformation>(iChannelsCount);

            stsResult = PCANBasic::GetValue(PCANBasic::PCAN_NONEBUS, TPCANParameter::PCAN_ATTACHED_CHANNELS, ciChannelInformation);

            if (stsResult == TPCANStatus::PCAN_ERROR_OK)
            {
                Console::WriteLine("-----------------------------------------------------------------------------------------");
                Console::WriteLine("Get PCAN_ATTACHED_CHANNELS:");

                for each (TPCANChannelInformation ^ currentChannelInformation in ciChannelInformation)
                {
                    Console::WriteLine("---------------------------");
                    Console::WriteLine("channel_handle:      " + ConvertToChannelHandle(currentChannelInformation->channel_handle));
                    Console::WriteLine("device_type:         " + ConvertDeviceTypeToString(currentChannelInformation->device_type));
                    Console::WriteLine("controller_number:   " + currentChannelInformation->controller_number);
                    Console::WriteLine("device_features:     " + ConvertToChannelFeatures(currentChannelInformation->device_features));
                    Console::WriteLine("device_name:         " + currentChannelInformation->device_name);
                    Console::WriteLine("device_id:           " + currentChannelInformation->device_id);
                    Console::WriteLine("channel_condition:   " + ConvertToChannelCondition(currentChannelInformation->channel_condition));
                }
                Console::WriteLine();
            }
        }
        if (stsResult != TPCANStatus::PCAN_ERROR_OK)
            ShowStatus(stsResult);
    }
#pragma endregion

#pragma region PCAN_CHANNEL_CONDITION
    /// <summary>
    /// Shows the status of selected PCAN-Channel
    /// </summary>
    void GetPCAN_CHANNEL_CONDITION()
    {
        UInt32 iChannelCondition;
        TPCANStatus stsResult = PCANBasic::GetValue(PcanHandle, TPCANParameter::PCAN_CHANNEL_CONDITION, iChannelCondition, sizeof(UInt32));

        if (stsResult == TPCANStatus::PCAN_ERROR_OK)
        {
            Console::WriteLine("-----------------------------------------------------------------------------------------");
            Console::WriteLine("Get PCAN_CHANNEL_CONDITION: " + ConvertToChannelCondition(iChannelCondition));
            Console::WriteLine();
        }
        else
            ShowStatus(stsResult);
    }
#pragma endregion

#pragma region PCAN_CHANNEL_IDENTIFYING
    /// <summary>
    ///  Shows the status from the status LED of the USB devices
    /// </summary>
    void GetPCAN_CHANNEL_IDENTIFYING()
    {
        UInt32 iChannelIdentifying;
        TPCANStatus stsResult = PCANBasic::GetValue(PcanHandle, TPCANParameter::PCAN_CHANNEL_IDENTIFYING, iChannelIdentifying, sizeof(UInt32));

        if (stsResult == TPCANStatus::PCAN_ERROR_OK)
        {
            Console::WriteLine("-----------------------------------------------------------------------------------------");
            Console::WriteLine("Get PCAN_CHANNEL_IDENTIFYING: " + ConvertToParameterOnOff(iChannelIdentifying));
            Console::WriteLine();
        }
        else
            ShowStatus(stsResult);
    }

    /// <summary>
    /// De/Activates the status LED of the USB devices
    /// </summary>
    /// <param name="value">True to turn on; False to turn off</param>
    void SetPCAN_CHANNEL_IDENTIFYING(bool value)
    {
        UInt32 ciChannelIdentifying;
        if (value)
            ciChannelIdentifying = PCANBasic::PCAN_PARAMETER_ON;
        else
            ciChannelIdentifying = PCANBasic::PCAN_PARAMETER_OFF;

        TPCANStatus stsResult = PCANBasic::SetValue(PcanHandle, TPCANParameter::PCAN_CHANNEL_IDENTIFYING, ciChannelIdentifying, sizeof(UInt32));

        if (stsResult == TPCANStatus::PCAN_ERROR_OK)
        {

            Console::WriteLine("-----------------------------------------------------------------------------------------");
            Console::WriteLine("Set PCAN_CHANNEL_IDENTIFYING: " + ConvertToParameterOnOff(ciChannelIdentifying));
            Console::WriteLine();
        }
        else
            ShowStatus(stsResult);
    }
#pragma endregion

#pragma region PCAN_CHANNEL_FEATURES
    /// <summary>
    /// Shows information about features
    /// </summary>
    void GetPCAN_CHANNEL_FEATURES()
    {
        UInt32 iChannelFeatures;
        TPCANStatus stsResult = PCANBasic::GetValue(PcanHandle, TPCANParameter::PCAN_CHANNEL_FEATURES, iChannelFeatures, sizeof(UInt32));

        if (stsResult == TPCANStatus::PCAN_ERROR_OK)
        {
            Console::WriteLine("-----------------------------------------------------------------------------------------");
            Console::WriteLine("Get PCAN_CHANNEL_FEATURES: " + ConvertToChannelFeatures(iChannelFeatures));
            Console::WriteLine();
        }
        else
            ShowStatus(stsResult);
    }
#pragma endregion

#pragma region PCAN_BITRATE_ADAPTING
    /// <summary>
    /// Shows the status from Bitrate-Adapting mode
    /// </summary>
    void GetPCAN_BITRATE_ADAPTING()
    {
        UInt32 iBitrateAdapting;
        TPCANStatus stsResult = PCANBasic::GetValue(PcanHandle, TPCANParameter::PCAN_BITRATE_ADAPTING, iBitrateAdapting, sizeof(UInt32));

        if (stsResult == TPCANStatus::PCAN_ERROR_OK)
        {
            Console::WriteLine("-----------------------------------------------------------------------------------------");
            Console::WriteLine("Get PCAN_BITRATE_ADAPTING: " + ConvertToParameterOnOff(iBitrateAdapting));
            Console::WriteLine();
        }
        else
            ShowStatus(stsResult);
    }

    /// <summary>
    /// De/Activates the Bitrate-Adapting mode
    /// </summary>
    /// <param name="value">True to turn on; False to turn off</param>
    void SetPCAN_BITRATE_ADAPTING(bool value)
    {
        UInt32 iBitrateAdapting;

        // Note: SetPCAN_BITRATE_ADAPTING requires an uninitialized channel
        //
        PCANBasic::Uninitialize(PCANBasic::PCAN_NONEBUS);

        if (value)
            iBitrateAdapting = PCANBasic::PCAN_PARAMETER_ON;
        else
            iBitrateAdapting = PCANBasic::PCAN_PARAMETER_OFF;

        TPCANStatus stsResult = PCANBasic::SetValue(PcanHandle, TPCANParameter::PCAN_BITRATE_ADAPTING, iBitrateAdapting, sizeof(UInt32));

        if (stsResult == TPCANStatus::PCAN_ERROR_OK)
        {
            Console::WriteLine("-----------------------------------------------------------------------------------------");
            Console::WriteLine("Set PCAN_BITRATE_ADAPTING: " + ConvertToParameterOnOff(iBitrateAdapting));
            Console::WriteLine();
        }
        else
            ShowStatus(stsResult);

        // Channel will be connected again
        //
        if (IsFD)
            stsResult = PCANBasic::InitializeFD(PcanHandle, (String^)BitrateFD);
        else
            stsResult = PCANBasic::Initialize(PcanHandle, Bitrate);

        if (stsResult != TPCANStatus::PCAN_ERROR_OK)
        {
            Console::WriteLine("Error while re-initializing the channel.");
            ShowStatus(stsResult);
        }
    }
#pragma endregion

#pragma region PCAN_ALLOW_STATUS_FRAMES
    /// <summary>
    /// Shows the status from the reception of status frames
    /// </summary>
    void GetPCAN_ALLOW_STATUS_FRAMES()
    {
        UInt32 iAllowStatusFrames;
        TPCANStatus stsResult = PCANBasic::GetValue(PcanHandle, TPCANParameter::PCAN_ALLOW_STATUS_FRAMES, iAllowStatusFrames, sizeof(UInt32));

        if (stsResult == TPCANStatus::PCAN_ERROR_OK)
        {
            Console::WriteLine("-----------------------------------------------------------------------------------------");
            Console::WriteLine("Get PCAN_ALLOW_STATUS_FRAMES: " + ConvertToParameterOnOff(iAllowStatusFrames));
            Console::WriteLine();
        }
        else
            ShowStatus(stsResult);
    }

    /// <summary>
    /// De/Activates the reception of status frames
    /// </summary>
    /// <param name="value">True to turn on; False to turn off</param>
    void SetPCAN_ALLOW_STATUS_FRAMES(bool value)
    {
        UInt32 iAllowStatusFrames;

        if (value)
            iAllowStatusFrames = PCANBasic::PCAN_PARAMETER_ON;
        else
            iAllowStatusFrames = PCANBasic::PCAN_PARAMETER_OFF;

        TPCANStatus stsResult = PCANBasic::SetValue(PcanHandle, TPCANParameter::PCAN_ALLOW_STATUS_FRAMES, iAllowStatusFrames, sizeof(UInt32));

        if (stsResult == TPCANStatus::PCAN_ERROR_OK)
        {
            Console::WriteLine("-----------------------------------------------------------------------------------------");
            Console::WriteLine("Set PCAN_ALLOW_STATUS_FRAMES: " + ConvertToParameterOnOff(iAllowStatusFrames));
            Console::WriteLine();
        }
        else
            ShowStatus(stsResult);
    }
#pragma endregion

#pragma region PCAN_ALLOW_RTR_FRAMES
    /// <summary>
    /// Shows the status from the reception of RTR frames
    /// </summary>
    void GetPCAN_ALLOW_RTR_FRAMES()
    {
        UInt32 iAllowRTRFrames;
        TPCANStatus stsResult = PCANBasic::GetValue(PcanHandle, TPCANParameter::PCAN_ALLOW_RTR_FRAMES, iAllowRTRFrames, sizeof(UInt32));

        if (stsResult == TPCANStatus::PCAN_ERROR_OK)
        {
            Console::WriteLine("-----------------------------------------------------------------------------------------");
            Console::WriteLine("Get PCAN_ALLOW_RTR_FRAMES: " + ConvertToParameterOnOff(iAllowRTRFrames));
            Console::WriteLine();
        }
        else
            ShowStatus(stsResult);
    }

    /// <summary>
    /// De/Activates the reception of RTR frames
    /// </summary>
    /// <param name="value">True to turn on; False to turn off</param>
    void SetPCAN_ALLOW_RTR_FRAMES(bool value)
    {
        UInt32 iAllowRTRFrames;

        if (value)
            iAllowRTRFrames = PCANBasic::PCAN_PARAMETER_ON;
        else
            iAllowRTRFrames = PCANBasic::PCAN_PARAMETER_OFF;

        TPCANStatus stsResult = PCANBasic::SetValue(PcanHandle, TPCANParameter::PCAN_ALLOW_RTR_FRAMES, iAllowRTRFrames, sizeof(UInt32));

        if (stsResult == TPCANStatus::PCAN_ERROR_OK)
        {
            Console::WriteLine("-----------------------------------------------------------------------------------------");
            Console::WriteLine("Set PCAN_ALLOW_RTR_FRAMES: " + ConvertToParameterOnOff(iAllowRTRFrames));
            Console::WriteLine();
        }
        else
            ShowStatus(stsResult);
    }
#pragma endregion

#pragma region PCAN_ALLOW_ERROR_FRAMES
    /// <summary>
    /// Shows the status from the reception of CAN error frames
    /// </summary>
    void GetPCAN_ALLOW_ERROR_FRAMES()
    {
        UInt32 iAllowErrorFrames;
        TPCANStatus stsResult = PCANBasic::GetValue(PcanHandle, TPCANParameter::PCAN_ALLOW_ERROR_FRAMES, iAllowErrorFrames, sizeof(UInt32));

        if (stsResult == TPCANStatus::PCAN_ERROR_OK)
        {
            Console::WriteLine("-----------------------------------------------------------------------------------------");
            Console::WriteLine("Get PCAN_ALLOW_ERROR_FRAMES: " + ConvertToParameterOnOff(iAllowErrorFrames));
            Console::WriteLine();
        }
        else
            ShowStatus(stsResult);
    }

    /// <summary>
    /// De/Activates the reception of CAN error frames
    /// </summary>
    /// <param name="value">True to turn on; False to turn off</param>
    void SetPCAN_ALLOW_ERROR_FRAMES(bool value)
    {
        UInt32 iAllowErrorFrames;

        if (value)
            iAllowErrorFrames = PCANBasic::PCAN_PARAMETER_ON;
        else
            iAllowErrorFrames = PCANBasic::PCAN_PARAMETER_OFF;

        TPCANStatus stsResult = PCANBasic::SetValue(PcanHandle, TPCANParameter::PCAN_ALLOW_ERROR_FRAMES, iAllowErrorFrames, sizeof(UInt32));

        if (stsResult == TPCANStatus::PCAN_ERROR_OK)
        {
            Console::WriteLine("-----------------------------------------------------------------------------------------");
            Console::WriteLine("Set PCAN_ALLOW_ERROR_FRAMES: " + ConvertToParameterOnOff(iAllowErrorFrames));
            Console::WriteLine();
        }
        else
            ShowStatus(stsResult);
    }
#pragma endregion

#pragma region PCAN_ALLOW_ECHO_FRAMES
    /// <summary>
    /// Shows the status from the reception of Echo frames
    /// </summary>
    void GetPCAN_ALLOW_ECHO_FRAMES()
    {
        UInt32 iAllowEchoFrames;
        TPCANStatus stsResult = PCANBasic::GetValue(PcanHandle, TPCANParameter::PCAN_ALLOW_ECHO_FRAMES, iAllowEchoFrames, sizeof(UInt32));

        if (stsResult == TPCANStatus::PCAN_ERROR_OK)
        {
            Console::WriteLine("-----------------------------------------------------------------------------------------");
            Console::WriteLine("Get PCAN_ALLOW_ECHO_FRAMES: " + ConvertToParameterOnOff(iAllowEchoFrames));
            Console::WriteLine();
        }
        else
            ShowStatus(stsResult);
    }

    /// <summary>
    /// De/Activates the reception of Echo frames
    /// </summary>
    /// <param name="value">True to turn on; False to turn off</param>
    void SetPCAN_ALLOW_ECHO_FRAMES(bool value)
    {
        UInt32 iAllowEchoFrames;

        if (value)
            iAllowEchoFrames = PCANBasic::PCAN_PARAMETER_ON;
        else
            iAllowEchoFrames = PCANBasic::PCAN_PARAMETER_OFF;

        TPCANStatus stsResult = PCANBasic::SetValue(PcanHandle, TPCANParameter::PCAN_ALLOW_ECHO_FRAMES, iAllowEchoFrames, sizeof(UInt32));

        if (stsResult == TPCANStatus::PCAN_ERROR_OK)
        {
            Console::WriteLine("-----------------------------------------------------------------------------------------");
            Console::WriteLine("Set PCAN_ALLOW_ECHO_FRAMES: " + ConvertToParameterOnOff(iAllowEchoFrames));
            Console::WriteLine();
        }
        else
            ShowStatus(stsResult);
    }
#pragma endregion

#pragma region PCAN_ACCEPTANCE_FILTER_11BIT
    /// <summary>
    /// Shows the reception filter with a specific 11-bit acceptance code and mask
    /// </summary>
    void GetPCAN_ACCEPTANCE_FILTER_11BIT()
    {
        UInt64 iAcceptanceFilter11Bit;
        TPCANStatus stsResult = PCANBasic::GetValue(PcanHandle, TPCANParameter::PCAN_ACCEPTANCE_FILTER_11BIT, iAcceptanceFilter11Bit, sizeof(UInt64));

        if (stsResult == TPCANStatus::PCAN_ERROR_OK)
        {
            Console::WriteLine("-----------------------------------------------------------------------------------------");
            Console::WriteLine("Get PCAN_ACCEPTANCE_FILTER_11BIT: " + iAcceptanceFilter11Bit.ToString("X16") + "h");
            Console::WriteLine();
        }
        else
            ShowStatus(stsResult);
    }

    /// <summary>
    /// Sets the reception filter with a specific 11-bit acceptance code and mask
    /// </summary>
    /// <param name="iacceptancefilter11bit">Acceptance code and mask</param>
    void SetPCAN_ACCEPTANCE_FILTER_11BIT(UInt64 iacceptancefilter11bit)
    {
        TPCANStatus stsResult = PCANBasic::SetValue(PcanHandle, TPCANParameter::PCAN_ACCEPTANCE_FILTER_11BIT, iacceptancefilter11bit, sizeof(UInt64));

        if (stsResult == TPCANStatus::PCAN_ERROR_OK)
        {
            Console::WriteLine("-----------------------------------------------------------------------------------------");
            Console::WriteLine("Set PCAN_ACCEPTANCE_FILTER_11BIT: " + iacceptancefilter11bit.ToString("X16") + "h");
            Console::WriteLine();
        }
        else
            ShowStatus(stsResult);
    }
#pragma endregion

#pragma region PCAN_ACCEPTANCE_FILTER_29BIT
    /// <summary>
    /// Shows the reception filter with a specific 29-bit acceptance code and mask
    /// </summary>
    void GetPCAN_ACCEPTANCE_FILTER_29BIT()
    {
        UInt32 iAcceptanceFilter29Bit;
        TPCANStatus stsResult = PCANBasic::GetValue(PcanHandle, TPCANParameter::PCAN_ACCEPTANCE_FILTER_29BIT, iAcceptanceFilter29Bit, sizeof(UInt32));

        if (stsResult == TPCANStatus::PCAN_ERROR_OK)
        {
            Console::WriteLine("-----------------------------------------------------------------------------------------");
            Console::WriteLine("Get PCAN_ACCEPTANCE_FILTER_29BIT: " + iAcceptanceFilter29Bit.ToString("X16") + "h");
            Console::WriteLine();
        }
        else
            ShowStatus(stsResult);
    }

    /// <summary>
    /// Sets the reception filter with a specific 29-bit acceptance code and mask
    /// </summary>
    /// <param name="iacceptancefilter29bit">Acceptance code and mask</param>
    void SetPCAN_ACCEPTANCE_FILTER_29BIT(UInt32 iacceptancefilter29bit)
    {
        TPCANStatus stsResult = PCANBasic::SetValue(PcanHandle, TPCANParameter::PCAN_ACCEPTANCE_FILTER_29BIT, iacceptancefilter29bit, sizeof(UInt32));

        if (stsResult == TPCANStatus::PCAN_ERROR_OK)
        {
            Console::WriteLine("-----------------------------------------------------------------------------------------");
            Console::WriteLine("Set PCAN_ACCEPTANCE_FILTER_29BIT: " + iacceptancefilter29bit.ToString("X16") + "h");
            Console::WriteLine();
        }
        else
            ShowStatus(stsResult);
    }
#pragma endregion

#pragma region PCAN_MESSAGE_FILTER
    /// <summary>
    /// Shows the status of the reception filter
    /// </summary>
    void GetPCAN_MESSAGE_FILTER()
    {
        UInt32 iMessageFilter;
        TPCANStatus stsResult = PCANBasic::GetValue(PcanHandle, TPCANParameter::PCAN_MESSAGE_FILTER, iMessageFilter, sizeof(UInt32));

        if (stsResult == TPCANStatus::PCAN_ERROR_OK)
        {
            Console::WriteLine("-----------------------------------------------------------------------------------------");
            Console::WriteLine("Get PCAN_MESSAGE_FILTER: " + ConvertToFilterOpenCloseCustom(iMessageFilter));
            Console::WriteLine();
        }
        else
            ShowStatus(stsResult);
    }

    /// <summary>
    /// De/Activates the reception filter
    /// </summary>
    /// <param name="imessagefilter">Configure reception filter</param>
    void SetPCAN_MESSAGE_FILTER(UInt32 imessagefilter)
    {
        TPCANStatus stsResult = PCANBasic::SetValue(PcanHandle, TPCANParameter::PCAN_MESSAGE_FILTER, imessagefilter, sizeof(UInt32));

        if (stsResult == TPCANStatus::PCAN_ERROR_OK)
        {
            Console::WriteLine("-----------------------------------------------------------------------------------------");
            Console::WriteLine("Set PCAN_MESSAGE_FILTER: " + ConvertToFilterOpenCloseCustom(imessagefilter));
            Console::WriteLine();
        }
        else
            ShowStatus(stsResult);
    }
#pragma endregion

#pragma region PCAN_HARD_RESET_STATUS
    /// <summary>
    /// Shows the status from of the hard reset within the PCANBasic.Reset method
    /// </summary>
    void GetPCAN_HARD_RESET_STATUS()
    {
        UInt32 iHardResetStatus;
        TPCANStatus stsResult = PCANBasic::GetValue(PcanHandle, TPCANParameter::PCAN_HARD_RESET_STATUS, iHardResetStatus, sizeof(UInt32));

        if (stsResult == TPCANStatus::PCAN_ERROR_OK)
        {
            Console::WriteLine("-----------------------------------------------------------------------------------------");
            Console::WriteLine("Get PCAN_HARD_RESET_STATUS: " + ConvertToFilterOpenCloseCustom(iHardResetStatus));
            Console::WriteLine();
        }
        else
            ShowStatus(stsResult);
    }

    /// <summary>
    /// De/Activates the hard reset within the PCANBasic.Reset method
    /// </summary>
    /// <param name="value">True to turn on; False to turn off</param>
    void SetPCAN_HARD_RESET_STATUS(bool  value)
    {
        UInt32 iHardResetStatus;

        if (value)
            iHardResetStatus = PCANBasic::PCAN_PARAMETER_ON;
        else
            iHardResetStatus = PCANBasic::PCAN_PARAMETER_OFF;

        TPCANStatus stsResult = PCANBasic::SetValue(PcanHandle, TPCANParameter::PCAN_HARD_RESET_STATUS, iHardResetStatus, sizeof(UInt32));

        if (stsResult == TPCANStatus::PCAN_ERROR_OK)
        {
            Console::WriteLine("-----------------------------------------------------------------------------------------");
            Console::WriteLine("Set PCAN_HARD_RESET_STATUS: " + ConvertToParameterOnOff(iHardResetStatus));
            Console::WriteLine();
        }
        else
            ShowStatus(stsResult);
    }
#pragma endregion

#pragma region PCAN_LAN_CHANNEL_DIRECTION
    /// <summary>
    /// Shows the communication direction of a PCAN-Channel representing a LAN interface
    /// </summary>
    void GetPCAN_LAN_CHANNEL_DIRECTION()
    {
        UInt32 iChannelDirection;
        TPCANStatus stsResult = PCANBasic::GetValue(PcanHandle, TPCANParameter::PCAN_LAN_CHANNEL_DIRECTION, iChannelDirection, sizeof(UInt32));

        if (stsResult == TPCANStatus::PCAN_ERROR_OK)
        {
            Console::WriteLine("-----------------------------------------------------------------------------------------");
            Console::WriteLine("Get PCAN_LAN_CHANNEL_DIRECTION: " + ConvertToChannelDirection(iChannelDirection));
            Console::WriteLine();
        }
        else
            ShowStatus(stsResult);
    }
#pragma endregion
#pragma endregion
#pragma endregion

#pragma region Help-Functions
private:
    /// <summary>
    /// Checks for availability of the PCANBasic labrary
    /// </summary>
    /// <returns>If the library was found or not</returns>
    bool CheckForLibrary()
    {
        // Check for dll file
        try
        {
            PCANBasic::Uninitialize(PCANBasic::PCAN_NONEBUS);
            return true;
        }
        catch (DllNotFoundException^)
        {
            Console::WriteLine("Unable to find the library: PCANBasic::dll !");
            Console::WriteLine("Press any key to close");
            Console::ReadKey();
        }

        return false;
    }

    /// <summary>
    /// Shows/prints the configurable parameters for this sample and information about them
    /// </summary>
    void ShowConfigurationHelp()
    {
        Console::WriteLine("=========================================================================================");
        Console::WriteLine("|                           PCAN-Basic GetSetParameter Example                           |");
        Console::WriteLine("=========================================================================================");
        Console::WriteLine("Following parameters are to be adjusted before launching, according to the hardware used |");
        Console::WriteLine("                                                                                         |");
        Console::WriteLine("* PcanHandle: Numeric value that represents the handle of the PCAN-Basic channel to use. |");
        Console::WriteLine("              See 'PCAN-Handle Definitions' within the documentation                     |");
        Console::WriteLine("* IsFD: Boolean value that indicates the communication mode, CAN (false) or CAN-FD (true)|");
        Console::WriteLine("* Bitrate: Numeric value that represents the BTR0/BR1 bitrate value to be used for CAN   |");
        Console::WriteLine("           communication                                                                 |");
        Console::WriteLine("* BitrateFD: String value that represents the nominal/data bitrate value to be used for  |");
        Console::WriteLine("             CAN-FD communication                                                        |");
        Console::WriteLine("=========================================================================================");
        Console::WriteLine("");
    }

    /// <summary>
    /// Shows/prints the configured paramters
    /// </summary>
    void ShowCurrentConfiguration()
    {
        Console::WriteLine("Parameter values used");
        Console::WriteLine("----------------------");
        Console::WriteLine("* PCANHandle: " + FormatChannelName(PcanHandle, IsFD));
        Console::WriteLine("* IsFD: " + IsFD);
        Console::WriteLine("* Bitrate: " + ConvertBitrateToString(Bitrate));
        Console::WriteLine("* BitrateFD: " + BitrateFD);
        Console::WriteLine("");
    }

    /// <summary>
    /// Shows formatted status
    /// </summary>
    /// <param name="status">Will be formatted</param>
    void ShowStatus(TPCANStatus status)
    {
        Console::WriteLine("=========================================================================================");
        Console::WriteLine(GetFormattedError(status));
        Console::WriteLine("=========================================================================================");
    }

    /// <summary>
    /// Gets the formatted text for a PCAN-Basic channel handle
    /// </summary>
    /// <param name="handle">PCAN-Basic Handle to format</param>
    /// <param name="isFD">If the channel is FD capable</param>
    /// <returns>The formatted text for a channel</returns>
    String^ FormatChannelName(TPCANHandle handle, bool isFD)
    {
        TPCANDevice devDevice;
        Byte byChannel;

        // Gets the owner device and channel for a PCAN-Basic handle
        if (handle < 0x100)
        {
            devDevice = (TPCANDevice)(handle >> 4);
            byChannel = (Byte)(handle & 0xF);
        }
        else
        {
            devDevice = (TPCANDevice)(handle >> 8);
            byChannel = (Byte)(handle & 0xFF);
        }

        // Constructs the PCAN-Basic Channel name and return it
        if (isFD)
            return String::Format("{0}:FD {1} ({2:X2}h)", devDevice, byChannel, handle);

        return String::Format("{0} {1} ({2:X2}h)", devDevice, byChannel, handle);
    }

    /// <summary>
    /// Help Function used to get an error as text
    /// </summary>
    /// <param name="error">Error code to be translated</param>
    /// <returns>A text with the translated error</returns>
    String^ GetFormattedError(TPCANStatus error)
    {
        // Creates a buffer big enough for a error-text
        StringBuilder^ strTemp = gcnew StringBuilder(256);
        // Gets the text using the GetErrorText API function. If the function success, the translated error is returned. 
        // If it fails, a text describing the current error is returned.
        if (PCANBasic::GetErrorText(error, 0x09, strTemp) != TPCANStatus::PCAN_ERROR_OK)
            return String::Format("An error occurred. Error-code's text ({0:X}) couldn't be retrieved", error);

        return strTemp->ToString();
    }

    /// <summary>
    /// Convert bitrate c_short value to readable string
    /// </summary>
    /// <param name="bitrate">Bitrate to be converted</param>
    /// <returns>A text with the converted bitrate</returns>
    String^ ConvertBitrateToString(TPCANBaudrate bitrate)
    {
        switch (bitrate)
        {
        case TPCANBaudrate::PCAN_BAUD_1M:
            return "1 MBit/sec";
        case TPCANBaudrate::PCAN_BAUD_800K:
            return "800 kBit/sec";
        case TPCANBaudrate::PCAN_BAUD_500K:
            return "500 kBit/sec";
        case TPCANBaudrate::PCAN_BAUD_250K:
            return "250 kBit/sec";
        case TPCANBaudrate::PCAN_BAUD_125K:
            return "125 kBit/sec";
        case TPCANBaudrate::PCAN_BAUD_100K:
            return "100 kBit/sec";
        case TPCANBaudrate::PCAN_BAUD_95K:
            return "95,238 kBit/sec";
        case TPCANBaudrate::PCAN_BAUD_83K:
            return "83,333 kBit/sec";
        case TPCANBaudrate::PCAN_BAUD_50K:
            return "50 kBit/sec";
        case TPCANBaudrate::PCAN_BAUD_47K:
            return "47,619 kBit/sec";
        case TPCANBaudrate::PCAN_BAUD_33K:
            return "33,333 kBit/sec";
        case TPCANBaudrate::PCAN_BAUD_20K:
            return "20 kBit/sec";
        case TPCANBaudrate::PCAN_BAUD_10K:
            return "10 kBit/sec";
        case TPCANBaudrate::PCAN_BAUD_5K:
            return "5 kBit/sec";
        default:
            return "Unknown Bitrate";
        }
    }

    /// <summary>
    /// Convert uint value to readable string value
    /// </summary>
    /// <param name="value">A value representing a PCAN-Channel handle</param>
    /// <returns>A representing a PCAN-Channel handle</returns>
    String^ ConvertToChannelHandle(UInt32 value)
    {
        switch (value)
        {
        case PCANBasic::PCAN_USBBUS1:
            return "PCAN_USBBUS1";
        case PCANBasic::PCAN_USBBUS2:
            return "PCAN_USBBUS2";
        case PCANBasic::PCAN_USBBUS3:
            return "PCAN_USBBUS3";
        case PCANBasic::PCAN_USBBUS4:
            return "PCAN_USBBUS4";
        case PCANBasic::PCAN_USBBUS5:
            return "PCAN_USBBUS5";
        case PCANBasic::PCAN_USBBUS6:
            return "PCAN_USBBUS6";
        case PCANBasic::PCAN_USBBUS7:
            return "PCAN_USBBUS7";
        case PCANBasic::PCAN_USBBUS8:
            return "PCAN_USBBUS8";
        case PCANBasic::PCAN_USBBUS9:
            return "PCAN_USBBUS9";
        case PCANBasic::PCAN_USBBUS10:
            return "PCAN_USBBUS10";
        case PCANBasic::PCAN_USBBUS11:
            return "PCAN_USBBUS11";
        case PCANBasic::PCAN_USBBUS12:
            return "PCAN_USBBUS12";
        case PCANBasic::PCAN_USBBUS13:
            return "PCAN_USBBUS13";
        case PCANBasic::PCAN_USBBUS14:
            return "PCAN_USBBUS14";
        case PCANBasic::PCAN_USBBUS15:
            return "PCAN_USBBUS15";
        case PCANBasic::PCAN_USBBUS16:
            return "PCAN_USBBUS16";

        case PCANBasic::PCAN_LANBUS1:
            return "PCAN_LANBUS1";
        case PCANBasic::PCAN_LANBUS2:
            return "PCAN_LANBUS2";
        case PCANBasic::PCAN_LANBUS3:
            return "PCAN_LANBUS3";
        case PCANBasic::PCAN_LANBUS4:
            return "PCAN_LANBUS4";
        case PCANBasic::PCAN_LANBUS5:
            return "PCAN_LANBUS5";
        case PCANBasic::PCAN_LANBUS6:
            return "PCAN_LANBUS6";
        case PCANBasic::PCAN_LANBUS7:
            return "PCAN_LANBUS7";
        case PCANBasic::PCAN_LANBUS8:
            return "PCAN_LANBUS8";
        case PCANBasic::PCAN_LANBUS9:
            return "PCAN_LANBUS9";
        case PCANBasic::PCAN_LANBUS10:
            return "PCAN_LANBUS10";
        case PCANBasic::PCAN_LANBUS11:
            return "PCAN_LANBUS11";
        case PCANBasic::PCAN_LANBUS12:
            return "PCAN_LANBUS12";
        case PCANBasic::PCAN_LANBUS13:
            return "PCAN_LANBUS13";
        case PCANBasic::PCAN_LANBUS14:
            return "PCAN_LANBUS14";
        case PCANBasic::PCAN_LANBUS15:
            return "PCAN_LANBUS15";
        case PCANBasic::PCAN_LANBUS16:
            return "PCAN_LANBUS16";

        case PCANBasic::PCAN_PCIBUS1:
            return "PCAN_PCIBUS1";
        case PCANBasic::PCAN_PCIBUS2:
            return "PCAN_PCIBUS2";
        case PCANBasic::PCAN_PCIBUS3:
            return "PCAN_PCIBUS3";
        case PCANBasic::PCAN_PCIBUS4:
            return "PCAN_PCIBUS4";
        case PCANBasic::PCAN_PCIBUS5:
            return "PCAN_PCIBUS5";
        case PCANBasic::PCAN_PCIBUS6:
            return "PCAN_PCIBUS6";
        case PCANBasic::PCAN_PCIBUS7:
            return "PCAN_PCIBUS7";
        case PCANBasic::PCAN_PCIBUS8:
            return "PCAN_PCIBUS8";
        case PCANBasic::PCAN_PCIBUS9:
            return "PCAN_PCIBUS9";
        case PCANBasic::PCAN_PCIBUS10:
            return "PCAN_PCIBUS10";
        case PCANBasic::PCAN_PCIBUS11:
            return "PCAN_PCIBUS11";
        case PCANBasic::PCAN_PCIBUS12:
            return "PCAN_PCIBUS12";
        case PCANBasic::PCAN_PCIBUS13:
            return "PCAN_PCIBUS13";
        case PCANBasic::PCAN_PCIBUS14:
            return "PCAN_PCIBUS14";
        case PCANBasic::PCAN_PCIBUS15:
            return "PCAN_PCIBUS15";
        case PCANBasic::PCAN_PCIBUS16:
            return "PCAN_PCIBUS16";
        default:
            return "Handle unknown: " + value;
        }
    }

    /// <summary>
    /// Convert BYTE value to readable string value
    /// </summary>
    /// <param name="devicetype"></param>
    /// <returns></returns>
    String^ ConvertDeviceTypeToString(TPCANDevice devicetype)
    {
        switch (devicetype)
        {
        case TPCANDevice::PCAN_NONE:
            return "PCAN_NONE";
        case TPCANDevice::PCAN_PEAKCAN:
            return "PCAN_PEAKCAN";
        case TPCANDevice::PCAN_ISA:
            return "PCAN_ISA";
        case TPCANDevice::PCAN_DNG:
            return "PCAN_DNG";
        case TPCANDevice::PCAN_PCI:
            return "PCAN_PCI";
        case TPCANDevice::PCAN_USB:
            return "PCAN_USB";
        case TPCANDevice::PCAN_PCC:
            return "PCAN_PCC";
        case TPCANDevice::PCAN_VIRTUAL:
            return "PCAN_VIRTUAL";
        case TPCANDevice::PCAN_LAN:
            return "PCAN_LAN";
        default:
            return "";
        }
    }

    /// <summary>
    /// Convert uint value to readable string value
    /// </summary>
    /// <param name="value">A value representing an on/off status</param>
    /// <returns>A text representing an on/ff status</returns>
    String^ ConvertToParameterOnOff(UInt32 value)
    {
        switch (value)
        {
        case PCANBasic::PCAN_PARAMETER_OFF:
            return "PCAN_PARAMETER_OFF";
        case PCANBasic::PCAN_PARAMETER_ON:
            return "PCAN_PARAMETER_ON";
        default:
            return "Status unknown: " + value.ToString();
        }
    }

    /// <summary>
    /// Convert uint value to readable string value
    /// </summary>
    /// <param name="value">A value representing a communication direction</param>
    /// <returns>A text representing a LAN channel direction</returns>
    String^ ConvertToChannelDirection(UInt32 value)
    {
        switch (value)
        {
        case PCANBasic::LAN_DIRECTION_READ:
            return "incoming only";
            break;
        case PCANBasic::LAN_DIRECTION_WRITE:
            return "outgoing only";
            break;
        default:
            if (value == PCANBasic::LAN_DIRECTION_READ_WRITE)
                return "bidirectional";
            else
                return String::Format("undefined (0x{0:X4})", value);
        }
    }

    /// <summary>
    /// Convert uint value to readable string value
    /// </summary>
    /// <param name="value">A value representing channel features</param>
    /// <returns>A text representing channel features</returns>
    String^ ConvertToChannelFeatures(UInt32 value)
    {
        String^ sFeatures = "";
        if ((value & PCANBasic::FEATURE_FD_CAPABLE) == PCANBasic::FEATURE_FD_CAPABLE)
            sFeatures += "FEATURE_FD_CAPABLE";
        if ((value & PCANBasic::FEATURE_DELAY_CAPABLE) == PCANBasic::FEATURE_DELAY_CAPABLE)
            if (sFeatures != "")
                sFeatures += ", FEATURE_DELAY_CAPABLE";
            else
                sFeatures += "FEATURE_DELAY_CAPABLE";
        if ((value & PCANBasic::FEATURE_IO_CAPABLE) == PCANBasic::FEATURE_IO_CAPABLE)
            if (sFeatures != "")
                sFeatures += ", FEATURE_IO_CAPABLE";
            else
                sFeatures += "FEATURE_IO_CAPABLE";
        return sFeatures;
    }

    /// <summary>
    /// Convert uint value to readable string value
    /// </summary>
    /// <param name="value">A value representing a channel condition</param>
    /// <returns>A text representing a channel condition</returns>
    String^ ConvertToChannelCondition(UInt32 value)
    {
        switch (value)
        {
        case PCANBasic::PCAN_CHANNEL_UNAVAILABLE:
            return "PCAN_CHANNEL_UNAVAILABLE";
        case PCANBasic::PCAN_CHANNEL_AVAILABLE:
            return "PCAN_CHANNEL_AVAILABLE";
        case PCANBasic::PCAN_CHANNEL_OCCUPIED:
            return "PCAN_CHANNEL_OCCUPIED";
        case (PCANBasic::PCAN_CHANNEL_AVAILABLE | PCANBasic::PCAN_CHANNEL_OCCUPIED):
            return "PCAN_CHANNEL_PCANVIEW";
        default:
            return "Status unknow: " + value;
        }
    }

    /// <summary>
    /// Convert uint value to readable string value
    /// </summary>
    /// <param name="value">A value representing filter status</param>
    /// <returns>A text representing a filter status</returns>
    String^ ConvertToFilterOpenCloseCustom(UInt32 value)
    {
        switch (value)
        {
        case PCANBasic::PCAN_FILTER_CLOSE:
            return "PCAN_FILTER_CLOSE";
        case PCANBasic::PCAN_FILTER_OPEN:
            return "PCAN_FILTER_OPEN";
        case PCANBasic::PCAN_FILTER_CUSTOM:
            return "PCAN_FILTER_CUSTOM";
        default:
            return "Status unknown: " + value;
        }
    }
#pragma endregion

};


int main(array<System::String^>^ args)
{
    GetSetParameter^ start = gcnew GetSetParameter();
    return 0;
}
