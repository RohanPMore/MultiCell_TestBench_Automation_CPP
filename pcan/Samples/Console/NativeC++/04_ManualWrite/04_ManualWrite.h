#include "stdafx.h"
#include "PCANBasic.h"

class ManualWrite
{
private:
    /// <summary>
    /// Sets the PCANHandle (Hardware Channel)
    /// </summary>
    const TPCANHandle PcanHandle = PCAN_USBBUS1;
    /// <summary>
    /// Sets the desired connection mode (CAN = false / CAN-FD = true)
    /// </summary>
    const bool IsFD = false;
    /// <summary>
    /// Sets the bitrate for normal CAN devices
    /// </summary>
    const TPCANBaudrate Bitrate = PCAN_BAUD_500K;
    /// <summary>
    /// Sets the bitrate for CAN FD devices. 
    /// Example - Bitrate Nom: 1Mbit/s Data: 2Mbit/s:
    ///   "f_clock_mhz=20, nom_brp=5, nom_tseg1=2, nom_tseg2=1, nom_sjw=1, data_brp=2, data_tseg1=3, data_tseg2=1, data_sjw=1"
    /// </summary>
    const TPCANBitrateFD BitrateFD = const_cast<LPSTR>("f_clock_mhz=20, nom_brp=5, nom_tseg1=2, nom_tseg2=1, nom_sjw=1, data_brp=2, data_tseg1=3, data_tseg2=1, data_sjw=1");

public:
    // ManualWrite constructor
    //
    ManualWrite();

    // ManualWrite destructor
    //
    ~ManualWrite();

private:
    /// <summary>
    /// Function for writing PCAN-Basic messages
    /// </summary>
    void WriteMessages();

    /// <summary>
    /// Function for writing messages on CAN devices
    /// </summary>
    /// <returns>A TPCANStatus error code</returns>
    TPCANStatus WriteMessage();

    /// <summary>
    /// Function for writing messages on CAN-FD devices
    /// </summary>
    /// <returns>A TPCANStatus error code</returns>
    TPCANStatus WriteMessageFD();

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
};