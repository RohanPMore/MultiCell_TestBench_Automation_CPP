#include "pch.h"
#include "PCANBasicCLR.h"

using namespace Peak::Can::Basic;
using namespace System;

public ref class LookUpChannel
{
#pragma region Defines
private:
    /// <summary>
    /// Sets a TPCANDevice value. The input can be numeric, in hexadecimal or decimal format, or as string denoting 
    /// a TPCANDevice value name.
    /// </summary>
    literal String^ DeviceType = "PCAN_USB";
    /// <summary>
    /// Sets value in range of a double. The input can be hexadecimal or decimal format.
    /// </summary>
    literal String^ DeviceID = "";
    /// <summary>
    /// Sets a zero-based index value in range of a double. The input can be hexadecimal or decimal format.
    /// </summary>
    literal String^ ControllerNumber = "";
    /// <summary>
    /// Sets a valid Internet Protocol address 
    /// </summary>
    literal String^ IPAddress = "";
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
public: LookUpChannel()
{
    ShowConfigurationHelp(); // Shows information about this sample
    ShowCurrentConfiguration(); // Shows the current parameters configuration

    // Checks if PCANBasic.dll is available, if not, the program terminates
    m_DLLFound = CheckForLibrary();
    if (!m_DLLFound)
        return;

    Console::WriteLine("Press any key to start searching");
    Console::ReadKey();
    Console::WriteLine();

    String^ sParameters = "";
    if (DeviceType != "")
        sParameters += (String^)PCANBasic::LOOKUP_DEVICE_TYPE + "=" + DeviceType;
    if (DeviceID != "")
    {
        if (sParameters != "")
            sParameters += ", ";
        sParameters += (String^)PCANBasic::LOOKUP_DEVICE_ID + "=" + DeviceID;
    }
    if (ControllerNumber != "")
    {
        if (sParameters != "")
            sParameters += ", ";
        sParameters += (String^)PCANBasic::LOOKUP_CONTROLLER_NUMBER + "=" + ControllerNumber;
    }
    if (IPAddress != "")
    {
        if (sParameters != "")
            sParameters += ", ";
        sParameters += (String^)PCANBasic::LOOKUP_IP_ADDRESS + "=" + IPAddress;
    }

    TPCANHandle handle;
    TPCANStatus stsResult = PCANBasic::LookUpChannel(sParameters, handle);

    if (stsResult == TPCANStatus::PCAN_ERROR_OK)
    {

        if (handle != PCANBasic::PCAN_NONEBUS)
        {
            UInt32 iFeatures;
            stsResult = PCANBasic::GetValue(handle, TPCANParameter::PCAN_CHANNEL_FEATURES,iFeatures, sizeof(UInt32));

            if (stsResult == TPCANStatus::PCAN_ERROR_OK)
                Console::WriteLine("The channel handle " + FormatChannelName(handle, (iFeatures & PCANBasic::FEATURE_FD_CAPABLE) == PCANBasic::FEATURE_FD_CAPABLE) + " was found");
            else
                Console::WriteLine("There was an issue retrieveing supported channel features");
        }
        else
            Console::WriteLine("A handle for these lookup-criteria was not found");
    }

    if (stsResult != TPCANStatus::PCAN_ERROR_OK)
    {
        Console::WriteLine("There was an error looking up the device, are any hardware channels attached?");
        ShowStatus(stsResult);
    }

    Console::WriteLine();
    Console::WriteLine("Press any key to close");
    Console::ReadKey();
}

public: ~LookUpChannel()
{
    if (m_DLLFound)
        PCANBasic::Uninitialize(PCANBasic::PCAN_NONEBUS);
}

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
        Console::WriteLine("|                        PCAN-Basic LookUpChannel Example                                |");
        Console::WriteLine("=========================================================================================");
        Console::WriteLine("Following parameters are to be adjusted before launching, according to the hardware used |");
        Console::WriteLine("                                                                                         |");
        Console::WriteLine("* DeviceType: Numeric value that represents a TPCANDevice                                |");
        Console::WriteLine("* DeviceID: Numeric value that represents the device identifier                          |");
        Console::WriteLine("* ControllerNumber: Numeric value that represents controller number                      |");
        Console::WriteLine("* IPAddress: String value that represents a valid Internet Protocol address              |");
        Console::WriteLine("                                                                                         |");
        Console::WriteLine("For more information see 'LookUp Parameter Definition' within the documentation          |");
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
        Console::WriteLine("* DeviceType: " + DeviceType);
        Console::WriteLine("* DeviceID: " + DeviceID);
        Console::WriteLine("* ControllerNumber: " + ControllerNumber);
        Console::WriteLine("* IPAddress: " + IPAddress);
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
#pragma endregion

};


int main(array<System::String^>^ args)
{
    LookUpChannel^ start = gcnew LookUpChannel();
    return 0;
}
