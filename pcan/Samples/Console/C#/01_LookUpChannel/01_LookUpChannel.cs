using Peak.Can.Basic;
using System;
using System.Text;

namespace Samples.LookUpChannel
{
    // Type alias for a CAN-FD bitrate string
    using TPCANBitrateFD = System.String;
    // Type alias for a PCAN-Basic channel handle
    using TPCANHandle = System.UInt16;
    // Type alias for a microseconds timestamp

    public class LookUpChannelSample
    {
        #region Defines
        /// <summary>
        /// Sets a TPCANDevice value. The input can be numeric, in hexadecimal or decimal format, or as string denoting 
        /// a TPCANDevice value name.
        /// </summary>
        const string DeviceType = "PCAN_USB";
        /// <summary>
        /// Sets value in range of a double. The input can be hexadecimal or decimal format.
        /// </summary>
        const string DeviceID = "";
        /// <summary>
        /// Sets a zero-based index value in range of a double. The input can be hexadecimal or decimal format.
        /// </summary>
        const string ControllerNumber = "";
        /// <summary>
        /// Sets a valid Internet Protocol address 
        /// </summary>
        const string IPAddress = "";
        #endregion

        #region Members
        /// <summary>
        /// Shows if DLL was found
        /// </summary>
        private bool m_DLLFound;
        #endregion

        /// <summary>
        /// Starts the PCANBasic Sample
        /// </summary>
        public LookUpChannelSample()
        {
            ShowConfigurationHelp(); // Shows information about this sample
            ShowCurrentConfiguration(); // Shows the current parameters configuration

            // Checks if PCANBasic.dll is available, if not, the program terminates
            m_DLLFound = CheckForLibrary();
            if (!m_DLLFound)
                return;

            Console.WriteLine("Press any key to start searching");
            Console.ReadKey();
            Console.WriteLine();

            string sParameters = "";
            if (DeviceType != "")
                sParameters += PCANBasic.LOOKUP_DEVICE_TYPE + "=" + DeviceType;
            if (DeviceID != "")
            {
                sParameters += ", " + PCANBasic.LOOKUP_DEVICE_ID + "=" + DeviceID;
            }
            if (ControllerNumber != "")
            {
                sParameters += ", " + PCANBasic.LOOKUP_CONTROLLER_NUMBER + "=" + ControllerNumber;
            }
            if (IPAddress != "")
            {
                sParameters += ", " + PCANBasic.LOOKUP_IP_ADDRESS + "=" + IPAddress;
            }

            TPCANStatus stsResult = PCANBasic.LookUpChannel(sParameters, out TPCANHandle handle);

            if (stsResult == TPCANStatus.PCAN_ERROR_OK)
            {

                if (handle != PCANBasic.PCAN_NONEBUS)
                {
                    stsResult = PCANBasic.GetValue(handle, TPCANParameter.PCAN_CHANNEL_FEATURES, out uint iFeatures, sizeof(uint));

                    if (stsResult == TPCANStatus.PCAN_ERROR_OK)
                        Console.WriteLine("The channel handle " + FormatChannelName(handle, (iFeatures & PCANBasic.FEATURE_FD_CAPABLE) == PCANBasic.FEATURE_FD_CAPABLE) + " was found");
                    else
                        Console.WriteLine("There was an issue retrieveing supported channel features");
                }
                else
                    Console.WriteLine("A handle for these lookup-criteria was not found");
            }

            if (stsResult != TPCANStatus.PCAN_ERROR_OK)
            {
                Console.WriteLine("There was an error looking up the device, are any hardware channels attached?");
                ShowStatus(stsResult);
            }

            Console.WriteLine();
            Console.WriteLine("Press any key to close");
            Console.ReadKey();
        }

        ~LookUpChannelSample()
        {
            if (m_DLLFound)
                PCANBasic.Uninitialize(PCANBasic.PCAN_NONEBUS);
        }

        #region Help-Functions
        /// <summary>
        /// Checks for availability of the PCANBasic library
        /// </summary>
        /// <returns>If the library was found or not</returns>
        private bool CheckForLibrary()
        {
            // Check for dll file
            try
            {
                PCANBasic.Uninitialize(PCANBasic.PCAN_NONEBUS);
                return true;
            }
            catch (DllNotFoundException)
            {
                Console.WriteLine("Unable to find the library: PCANBasic.dll !");
                Console.WriteLine("Press any key to close");
                Console.ReadKey();
            }
            return false;
        }

        /// <summary>
        /// Shows/prints the configurable parameters for this sample and information about them
        /// </summary>
        private void ShowConfigurationHelp()
        {
            Console.WriteLine("=========================================================================================");
            Console.WriteLine("|                        PCAN-Basic LookUpChannel Example                                |");
            Console.WriteLine("=========================================================================================");
            Console.WriteLine("Following parameters are to be adjusted before launching, according to the hardware used |");
            Console.WriteLine("                                                                                         |");
            Console.WriteLine("* DeviceType: Numeric value that represents a TPCANDevice                                |");
            Console.WriteLine("* DeviceID: Numeric value that represents the device identifier                          |");
            Console.WriteLine("* ControllerNumber: Numeric value that represents controller number                      |");
            Console.WriteLine("* IPAddress: String value that represents a valid Internet Protocol address              |");
            Console.WriteLine("                                                                                         |");
            Console.WriteLine("For more information see 'LookUp Parameter Definition' within the documentation          |");
            Console.WriteLine("=========================================================================================");
            Console.WriteLine("");
        }

        /// <summary>
        /// Shows/prints the configured paramters
        /// </summary>
        private void ShowCurrentConfiguration()
        {
            Console.WriteLine("Parameter values used");
            Console.WriteLine("----------------------");
            Console.WriteLine("* DeviceType: " + DeviceType);
            Console.WriteLine("* DeviceID: " + DeviceID);
            Console.WriteLine("* ControllerNumber: " + ControllerNumber);
            Console.WriteLine("* IPAddress: " + IPAddress);
            Console.WriteLine("");
        }

        /// <summary>
        /// Shows formatted status
        /// </summary>
        /// <param name="status">Will be formatted</param>
        private void ShowStatus(TPCANStatus status)
        {
            Console.WriteLine("=========================================================================================");
            Console.WriteLine(GetFormattedError(status));
            Console.WriteLine("=========================================================================================");
        }

        /// <summary>
        /// Gets the formatted text for a PCAN-Basic channel handle
        /// </summary>
        /// <param name="handle">PCAN-Basic Handle to format</param>
        /// <param name="isFD">If the channel is FD capable</param>
        /// <returns>The formatted text for a channel</returns>
        private string FormatChannelName(TPCANHandle handle, bool isFD)
        {
            TPCANDevice devDevice;
            byte byChannel;

            // Gets the owner device and channel for a PCAN-Basic handle
            if (handle < 0x100)
            {
                devDevice = (TPCANDevice)(handle >> 4);
                byChannel = (byte)(handle & 0xF);
            }
            else
            {
                devDevice = (TPCANDevice)(handle >> 8);
                byChannel = (byte)(handle & 0xFF);
            }

            // Constructs the PCAN-Basic Channel name and return it
            if (isFD)
                return string.Format("{0}:FD {1} ({2:X2}h)", devDevice, byChannel, handle);

            return string.Format("{0} {1} ({2:X2}h)", devDevice, byChannel, handle);
        }

        /// <summary>
        /// Help Function used to get an error as text
        /// </summary>
        /// <param name="error">Error code to be translated</param>
        /// <returns>A text with the translated error</returns>
        private string GetFormattedError(TPCANStatus error)
        {
            // Creates a buffer big enough for a error-text
            var strTemp = new StringBuilder(256);

            // Gets the text using the GetErrorText API function
            // If the function success, the translated error is returned. If it fails, a text describing the current 
            // error is returned.
            if (PCANBasic.GetErrorText(error, 0x09, strTemp) != TPCANStatus.PCAN_ERROR_OK)
                return string.Format("An error occurred. Error-code's text ({0:X}) couldn't be retrieved", error);

            return strTemp.ToString();
        }
        #endregion
    }
}
