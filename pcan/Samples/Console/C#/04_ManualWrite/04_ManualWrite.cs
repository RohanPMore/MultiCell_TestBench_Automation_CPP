using System;
using System.Text;
using System.Threading;
using Peak.Can.Basic;

namespace Samples.ManualWrite
{
    // Type alias for a PCAN-Basic channel handle
    using TPCANHandle = System.UInt16;
    // Type alias for a CAN-FD bitrate string
    using TPCANBitrateFD = System.String;
    // Type alias for a microseconds timestamp
    using TPCANTimestampFD = System.UInt64;

    public class ManualWriteSample
    {
        #region Defines
        /// <summary>
        /// Sets the PCANHandle (Hardware Channel)
        /// </summary>
        const TPCANHandle PcanHandle = PCANBasic.PCAN_USBBUS1;
        /// <summary>
        /// Sets the desired connection mode (CAN = false / CAN-FD = true)
        /// </summary>
        const bool IsFD = false;
        /// <summary>
        /// Sets the bitrate for normal CAN devices
        /// </summary>
        const TPCANBaudrate Bitrate = TPCANBaudrate.PCAN_BAUD_500K;
        /// <summary>
        /// Sets the bitrate for CAN FD devices. 
        /// Example - Bitrate Nom: 1Mbit/s Data: 2Mbit/s:
        ///   "f_clock_mhz=20, nom_brp=5, nom_tseg1=2, nom_tseg2=1, nom_sjw=1, data_brp=2, data_tseg1=3, data_tseg2=1, data_sjw=1"
        /// </summary>
        const TPCANBitrateFD BitrateFD = "f_clock_mhz=20, nom_brp=5, nom_tseg1=2, nom_tseg2=1, nom_sjw=1, data_brp=2, data_tseg1=3, data_tseg2=1, data_sjw=1";
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
        public ManualWriteSample()
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
                stsResult = PCANBasic.InitializeFD(PcanHandle, BitrateFD);
            else
                stsResult = PCANBasic.Initialize(PcanHandle, Bitrate);

            if (stsResult != TPCANStatus.PCAN_ERROR_OK)
            {
                Console.WriteLine("Can not initialize. Please check the defines in the code.");
                ShowStatus(stsResult);
                Console.WriteLine();
                Console.WriteLine("Press any key to close");
                Console.Read();
                return;
            }

            // Writing messages...
            Console.WriteLine("Successfully initialized.");
            Console.WriteLine("Press any key to write");
            Console.ReadKey();
            do
            {
                Console.Clear();
                WriteMessages();
                Console.WriteLine("Do you want to write again? yes[y] or any other key to close");
            } while (Console.ReadKey().Key == ConsoleKey.Y);

        }

        ~ManualWriteSample()
        {
            if (m_DLLFound)
                PCANBasic.Uninitialize(PCANBasic.PCAN_NONEBUS);
        }

        #region Main-Functions
        /// <summary>
        /// Function for writing PCAN-Basic messages
        /// </summary>
        private void WriteMessages()
        {
            TPCANStatus stsResult;

            if (IsFD)
                stsResult = WriteMessageFD();
            else
                stsResult = WriteMessage();

            // Checks if the message was sent
            if (stsResult != TPCANStatus.PCAN_ERROR_OK)
                ShowStatus(stsResult);
            else
                Console.WriteLine("Message was successfully SENT");
        }

        /// <summary>
        /// Function for writing messages on CAN devices
        /// </summary>
        /// <returns>A TPCANStatus error code</returns>
        private TPCANStatus WriteMessage()
        {
            // Sends a CAN message with extended ID, and 8 data bytes
            var msgCanMessage = new TPCANMsg();
            msgCanMessage.DATA = new byte[8];
            msgCanMessage.ID = 0x100;
            msgCanMessage.LEN = Convert.ToByte(8);
            msgCanMessage.MSGTYPE = TPCANMessageType.PCAN_MESSAGE_EXTENDED;
            for (byte i = 0; i < 8; i++)
            {
                msgCanMessage.DATA[i] = i;
            }
            return PCANBasic.Write(PcanHandle, ref msgCanMessage);
        }

        /// <summary>
        /// Function for writing messages on CAN-FD devices
        /// </summary>
        /// <returns>A TPCANStatus error code</returns>
        private TPCANStatus WriteMessageFD()
        {
            // Sends a CAN-FD message with standard ID, 64 data bytes, and bitrate switch
            var msgCanMessageFD = new TPCANMsgFD();
            msgCanMessageFD.DATA = new byte[64];
            msgCanMessageFD.ID = 0x100;
            msgCanMessageFD.DLC = 15;
            msgCanMessageFD.MSGTYPE = TPCANMessageType.PCAN_MESSAGE_FD | TPCANMessageType.PCAN_MESSAGE_BRS;
            for (byte i = 0; i < 64; i++)
            {
                msgCanMessageFD.DATA[i] = i;
            }
            return PCANBasic.WriteFD(PcanHandle, ref msgCanMessageFD);
        }
        #endregion

        #region Help-Functions
        /// <summary>
        /// Checks for availability of the PCANBasic labrary
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
            Console.WriteLine("|                          PCAN-Basic ManualWrite Example                                |");
            Console.WriteLine("=========================================================================================");
            Console.WriteLine("Following parameters are to be adjusted before launching, according to the hardware used |");
            Console.WriteLine("                                                                                         |");
            Console.WriteLine("* PcanHandle: Numeric value that represents the handle of the PCAN-Basic channel to use. |");
            Console.WriteLine("              See 'PCAN-Handle Definitions' within the documentation                     |");
            Console.WriteLine("* IsFD: Boolean value that indicates the communication mode, CAN (false) or CAN-FD (true)|");
            Console.WriteLine("* Bitrate: Numeric value that represents the BTR0/BR1 bitrate value to be used for CAN   |");
            Console.WriteLine("           communication                                                                 |");
            Console.WriteLine("* BitrateFD: String value that represents the nominal/data bitrate value to be used for  |");
            Console.WriteLine("             CAN-FD communication                                                        |");
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
            Console.WriteLine("* PCANHandle: " + FormatChannelName(PcanHandle, IsFD));
            Console.WriteLine("* IsFD: " + IsFD);
            Console.WriteLine("* Bitrate: " + ConvertBitrateToString(Bitrate));
            Console.WriteLine("* BitrateFD: " + BitrateFD);
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
            // Gets the text using the GetErrorText API function. If the function success, the translated error is returned. 
            // If it fails, a text describing the current error is returned.
            if (PCANBasic.GetErrorText(error, 0x09, strTemp) != TPCANStatus.PCAN_ERROR_OK)
                return string.Format("An error occurred. Error-code's text ({0:X}) couldn't be retrieved", error);
            return strTemp.ToString();
        }

        /// <summary>
        /// Convert bitrate c_short value to readable string
        /// </summary>
        /// <param name="bitrate">Bitrate to be converted</param>
        /// <returns>A text with the converted bitrate</returns>
        private string ConvertBitrateToString(TPCANBaudrate bitrate)
        {
            switch (bitrate)
            {
                case TPCANBaudrate.PCAN_BAUD_1M:
                    return "1 MBit/sec";
                case TPCANBaudrate.PCAN_BAUD_800K:
                    return "800 kBit/sec";
                case TPCANBaudrate.PCAN_BAUD_500K:
                    return "500 kBit/sec";
                case TPCANBaudrate.PCAN_BAUD_250K:
                    return "250 kBit/sec";
                case TPCANBaudrate.PCAN_BAUD_125K:
                    return "125 kBit/sec";
                case TPCANBaudrate.PCAN_BAUD_100K:
                    return "100 kBit/sec";
                case TPCANBaudrate.PCAN_BAUD_95K:
                    return "95,238 kBit/sec";
                case TPCANBaudrate.PCAN_BAUD_83K:
                    return "83,333 kBit/sec";
                case TPCANBaudrate.PCAN_BAUD_50K:
                    return "50 kBit/sec";
                case TPCANBaudrate.PCAN_BAUD_47K:
                    return "47,619 kBit/sec";
                case TPCANBaudrate.PCAN_BAUD_33K:
                    return "33,333 kBit/sec";
                case TPCANBaudrate.PCAN_BAUD_20K:
                    return "20 kBit/sec";
                case TPCANBaudrate.PCAN_BAUD_10K:
                    return "10 kBit/sec";
                case TPCANBaudrate.PCAN_BAUD_5K:
                    return "5 kBit/sec";
                default:
                    return "Unknown Bitrate";
            }
        }
        #endregion
    }
}
