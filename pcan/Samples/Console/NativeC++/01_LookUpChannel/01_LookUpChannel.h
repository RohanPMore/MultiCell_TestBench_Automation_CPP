#include"stdafx.h"
#include "PCANBasic.h"

class LookUpChannel
{
private:
    /// <summary>
    /// Sets a TPCANDevice value. The input can be numeric, in hexadecimal or decimal format, or as string denoting 
    /// a TPCANDevice value name.
    /// </summary>
    LPCSTR DeviceType = "PCAN_USB";
    /// <summary>
    /// Sets value in range of a double. The input can be hexadecimal or decimal format.
    /// </summary>
    LPCSTR DeviceID = "";
    /// <summary>
    /// Sets a zero-based index value in range of a double. The input can be hexadecimal or decimal format.
    /// </summary>
    LPCSTR ControllerNumber = "";
    /// <summary>
    /// Sets a valid Internet Protocol address 
    /// </summary>
    LPCSTR IPAddress = "";

public:
    // LookUpChannel constructor
    //
    LookUpChannel();

    // LookUpChannel destructor
    //
    ~LookUpChannel();

private:
    /// <summary>
    /// Shows/prints the configurable parameters for this sample and information about them
    /// </summary>
    void ShowConfigurationHelp();

    /// <summary>
    /// Shows/prints the configured paramters
    /// </summary>
    void ShowCurrentConfiguration();

    /// <summary>
    /// Shows formatted status
    /// </summary>
    /// <param name="status">Will be formatted</param>
    void ShowStatus(TPCANStatus status);

    /// <summary>
    /// Gets the formatted text for a PCAN-Basic channel handle
    /// </summary>
    /// <param name="handle">PCAN-Basic Handle to format</param>
    /// <parma name="buffer">A string buffer for the channel name</param>
    /// <param name="isFD">If the channel is FD capable</param>
    void FormatChannelName(TPCANHandle handle, LPSTR buffer, bool isFD);

    /// <summary>
    /// Gets name of a TPCANHandle
    /// </summary>
    /// <param name="handle">TPCANHandle to get name</param>
    /// <param name="buffer">A string buffer for the name of the TPCANHandle (size MAX_PATH)</param>
    void GetTPCANHandleName(TPCANHandle handle, LPSTR buffer);

    /// <summary>
    /// Help Function used to get an error as text
    /// </summary>
    /// <param name="error">Error code to be translated</param>
    /// <param name="buffer">A string buffer for the translated error (size MAX_PATH)</param>
    void GetFormattedError(TPCANStatus error, LPSTR buffer);

    /// <summary>
    /// Convert bitrate c_short value to readable string
    /// </summary>
    /// <param name="bitrate">Bitrate to be converted</param>
    /// <param name="buffer">A string buffer for the converted bitrate (size MAX_PATH)</param>
    void ConvertBitrateToString(TPCANBaudrate bitrate, LPSTR buffer);

    /// <summary>
    /// Converts PCANBasic Define (wchar_t*) to char*
    /// </summary>
    /// <param name="define">Define to be converted</param>
    /// <returns>Define as char*</returns>
    char* ConvertDefinesToChar(wchar_t* define);
};