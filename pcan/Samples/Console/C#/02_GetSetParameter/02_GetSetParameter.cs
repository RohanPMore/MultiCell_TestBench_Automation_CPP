using Peak.Can.Basic;
using System;
using System.Text;

namespace Samples.GetSetParameter
{
    // Type alias for a CAN-FD bitrate string
    using TPCANBitrateFD = System.String;
    // Type alias for a PCAN-Basic channel handle
    using TPCANHandle = System.UInt16;
    // Type alias for a microseconds timestamp

    public class GetSetParameterSample
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
        public GetSetParameterSample()
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
                Console.ReadKey();
                return;
            }

            Console.WriteLine("Successfully initialized.");
            Console.WriteLine("Press any key to get/set parameter");
            Console.ReadKey();
            Console.WriteLine();

            RunSelectedCommands();

            Console.WriteLine();
            Console.WriteLine("Press any key to close");
            Console.ReadKey();
        }

        ~GetSetParameterSample()
        {
            if (m_DLLFound)
            {
                PCANBasic.Uninitialize(PCANBasic.PCAN_NONEBUS);
            }
        }

        #region Main-Functions
        /// <summary>
        /// Runs all commands for get or set parameters
        /// </summary>
        private void RunSelectedCommands()
        {
            // Fill commands here 
            Console.WriteLine("Fill \"RunSelectedCommands\"-function with parameter functions from \"Parameter commands\"-Region in the code.");
        }

        #region Parameter commands
        #region PCAN_DEVICE_ID
        /// <summary>
        /// Shows device identifier parameter
        /// </summary>
        private void GetPCAN_DEVICE_ID()
        {
            TPCANStatus stsResult = PCANBasic.GetValue(PcanHandle, TPCANParameter.PCAN_DEVICE_ID, out uint iDeviceID, sizeof(uint));

            if (stsResult == TPCANStatus.PCAN_ERROR_OK)
            {
                Console.WriteLine("-----------------------------------------------------------------------------------------");
                Console.WriteLine("Get PCAN_DEVICE_ID: " + iDeviceID);
                Console.WriteLine();
            }
            else
                ShowStatus(stsResult);
        }


        /// <summary>
        /// Sets device identifier parameter
        /// </summary>
        /// <param name="iDeviceID"></param>
        private void SetPCAN_DEVICE_ID(uint iDeviceID)
        {
            TPCANStatus stsResult = PCANBasic.SetValue(PcanHandle, TPCANParameter.PCAN_DEVICE_ID, ref iDeviceID, sizeof(uint));

            if (stsResult == TPCANStatus.PCAN_ERROR_OK)
            {
                Console.WriteLine("-----------------------------------------------------------------------------------------");
                Console.WriteLine("Set PCAN_DEVICE_ID: " + iDeviceID);
                Console.WriteLine();
            }
            else
                ShowStatus(stsResult);
        }
        #endregion

        #region PCAN_ATTACHED_CHANNELS
        /// <summary>
        /// Shows all information about attached channels
        /// </summary>
        private void GetPCAN_ATTACHED_CHANNELS()
        {
            TPCANStatus stsResult = PCANBasic.GetValue(PCANBasic.PCAN_NONEBUS, TPCANParameter.PCAN_ATTACHED_CHANNELS_COUNT, out uint iChannelsCount, sizeof(uint));

            if (stsResult == TPCANStatus.PCAN_ERROR_OK)
            {
                TPCANChannelInformation[] ciChannelInformation = new TPCANChannelInformation[iChannelsCount];

                stsResult = PCANBasic.GetValue(PCANBasic.PCAN_NONEBUS, TPCANParameter.PCAN_ATTACHED_CHANNELS, ciChannelInformation);

                if (stsResult == TPCANStatus.PCAN_ERROR_OK)
                {
                    Console.WriteLine("-----------------------------------------------------------------------------------------");
                    Console.WriteLine("Get PCAN_ATTACHED_CHANNELS:");

                    foreach (TPCANChannelInformation currentChannelInformation in ciChannelInformation)
                    {
                        Console.WriteLine("---------------------------");
                        Console.WriteLine("channel_handle:      " + ConvertToChannelHandle(currentChannelInformation.channel_handle));
                        Console.WriteLine("device_type:         " + currentChannelInformation.device_type);
                        Console.WriteLine("controller_number:   " + currentChannelInformation.controller_number);
                        Console.WriteLine("device_features:     " + ConvertToChannelFeatures(currentChannelInformation.device_features));
                        Console.WriteLine("device_name:         " + currentChannelInformation.device_name);
                        Console.WriteLine("device_id:           " + currentChannelInformation.device_id);
                        Console.WriteLine("channel_condition:   " + ConvertToChannelCondition(currentChannelInformation.channel_condition));
                    }
                    Console.WriteLine();
                }
            }
            if (stsResult != TPCANStatus.PCAN_ERROR_OK)
                ShowStatus(stsResult);
        }
        #endregion

        #region PCAN_CHANNEL_CONDITION
        /// <summary>
        /// Shows the status of selected PCAN-Channel
        /// </summary>
        private void GetPCAN_CHANNEL_CONDITION()
        {
            TPCANStatus stsResult = PCANBasic.GetValue(PcanHandle, TPCANParameter.PCAN_CHANNEL_CONDITION, out uint iChannelCondition, sizeof(uint));

            if (stsResult == TPCANStatus.PCAN_ERROR_OK)
            {
                Console.WriteLine("-----------------------------------------------------------------------------------------");
                Console.WriteLine("Get PCAN_CHANNEL_CONDITION: " + ConvertToChannelCondition(iChannelCondition));
                Console.WriteLine();
            }
            else
                ShowStatus(stsResult);
        }
        #endregion

        #region PCAN_CHANNEL_IDENTIFYING
        /// <summary>
        ///  Shows the status from the status LED of the USB devices
        /// </summary>
        private void GetPCAN_CHANNEL_IDENTIFYING()
        {
            TPCANStatus stsResult = PCANBasic.GetValue(PcanHandle, TPCANParameter.PCAN_CHANNEL_IDENTIFYING, out uint iChannelIdentifying, sizeof(uint));

            if (stsResult == TPCANStatus.PCAN_ERROR_OK)
            {
                Console.WriteLine("-----------------------------------------------------------------------------------------");
                Console.WriteLine("Get PCAN_CHANNEL_IDENTIFYING: " + ConvertToParameterOnOff(iChannelIdentifying));
                Console.WriteLine();
            }
            else
                ShowStatus(stsResult);
        }

        /// <summary>
        /// De/Activates the status LED of the USB devices
        /// </summary>
        /// <param name="value">True to turn on; False to turn off</param>
        private void SetPCAN_CHANNEL_IDENTIFYING(bool value)
        {
            uint ciChannelIdentifying;
            if (value)
                ciChannelIdentifying = PCANBasic.PCAN_PARAMETER_ON;
            else
                ciChannelIdentifying = PCANBasic.PCAN_PARAMETER_OFF;

            TPCANStatus stsResult = PCANBasic.SetValue(PcanHandle, TPCANParameter.PCAN_CHANNEL_IDENTIFYING, ref ciChannelIdentifying, sizeof(uint));

            if (stsResult == TPCANStatus.PCAN_ERROR_OK)
            {

                Console.WriteLine("-----------------------------------------------------------------------------------------");
                Console.WriteLine("Set PCAN_CHANNEL_IDENTIFYING: " + ConvertToParameterOnOff(ciChannelIdentifying));
                Console.WriteLine();
            }
            else
                ShowStatus(stsResult);
        }
        #endregion

        #region PCAN_CHANNEL_FEATURES
        /// <summary>
        /// Shows information about features
        /// </summary>
        private void GetPCAN_CHANNEL_FEATURES()
        {
            TPCANStatus stsResult = PCANBasic.GetValue(PcanHandle, TPCANParameter.PCAN_CHANNEL_FEATURES, out uint iChannelFeatures, sizeof(uint));

            if (stsResult == TPCANStatus.PCAN_ERROR_OK)
            {
                Console.WriteLine("-----------------------------------------------------------------------------------------");
                Console.WriteLine("Get PCAN_CHANNEL_FEATURES: " + ConvertToChannelFeatures(iChannelFeatures));
                Console.WriteLine();
            }
            else
                ShowStatus(stsResult);
        }
        #endregion

        #region PCAN_BITRATE_ADAPTING
        /// <summary>
        /// Shows the status from Bitrate-Adapting mode
        /// </summary>
        private void GetPCAN_BITRATE_ADAPTING()
        {
            TPCANStatus stsResult = PCANBasic.GetValue(PcanHandle, TPCANParameter.PCAN_BITRATE_ADAPTING, out uint iBitrateAdapting, sizeof(uint));

            if (stsResult == TPCANStatus.PCAN_ERROR_OK)
            {
                Console.WriteLine("-----------------------------------------------------------------------------------------");
                Console.WriteLine("Get PCAN_BITRATE_ADAPTING: " + ConvertToParameterOnOff(iBitrateAdapting));
                Console.WriteLine();
            }
            else
                ShowStatus(stsResult);
        }

        /// <summary>
        /// De/Activates the Bitrate-Adapting mode
        /// </summary>
        /// <param name="value">True to turn on; False to turn off</param>
        private void SetPCAN_BITRATE_ADAPTING(bool value)
        {
            uint iBitrateAdapting;

            // Note: SetPCAN_BITRATE_ADAPTING requires an uninitialized channel, 
            //
            PCANBasic.Uninitialize(PCANBasic.PCAN_NONEBUS);

            if (value)
                iBitrateAdapting = PCANBasic.PCAN_PARAMETER_ON;
            else
                iBitrateAdapting = PCANBasic.PCAN_PARAMETER_OFF;

            TPCANStatus stsResult = PCANBasic.SetValue(PcanHandle, TPCANParameter.PCAN_BITRATE_ADAPTING, ref iBitrateAdapting, sizeof(uint));

            if (stsResult == TPCANStatus.PCAN_ERROR_OK)
            {
                Console.WriteLine("-----------------------------------------------------------------------------------------");
                Console.WriteLine("Set PCAN_BITRATE_ADAPTING: " + ConvertToParameterOnOff(iBitrateAdapting));
                Console.WriteLine();
            }
            else
                ShowStatus(stsResult);

            // Channel will be connected again
            if (IsFD)
                stsResult = PCANBasic.InitializeFD(PcanHandle, BitrateFD);
            else
                stsResult = PCANBasic.Initialize(PcanHandle, Bitrate);

            if (stsResult != TPCANStatus.PCAN_ERROR_OK)
            {
                Console.WriteLine("Error while re-initializing the channel.");
                ShowStatus(stsResult);
            }
        }
        #endregion

        #region PCAN_ALLOW_STATUS_FRAMES
        /// <summary>
        /// Shows the status from the reception of status frames
        /// </summary>
        private void GetPCAN_ALLOW_STATUS_FRAMES()
        {
            TPCANStatus stsResult = PCANBasic.GetValue(PcanHandle, TPCANParameter.PCAN_ALLOW_STATUS_FRAMES, out uint iAllowStatusFrames, sizeof(uint));

            if (stsResult == TPCANStatus.PCAN_ERROR_OK)
            {
                Console.WriteLine("-----------------------------------------------------------------------------------------");
                Console.WriteLine("Get PCAN_ALLOW_STATUS_FRAMES: " + ConvertToParameterOnOff(iAllowStatusFrames));
                Console.WriteLine();
            }
            else
                ShowStatus(stsResult);
        }

        /// <summary>
        /// De/Activates the reception of status frames
        /// </summary>
        /// <param name="value">True to turn on; False to turn off</param>
        private void SetPCAN_ALLOW_STATUS_FRAMES(bool value)
        {
            uint iAllowStatusFrames;

            if (value)
                iAllowStatusFrames = PCANBasic.PCAN_PARAMETER_ON;
            else
                iAllowStatusFrames = PCANBasic.PCAN_PARAMETER_OFF;

            TPCANStatus stsResult = PCANBasic.SetValue(PcanHandle, TPCANParameter.PCAN_ALLOW_STATUS_FRAMES, ref iAllowStatusFrames, sizeof(uint));

            if (stsResult == TPCANStatus.PCAN_ERROR_OK)
            {
                Console.WriteLine("-----------------------------------------------------------------------------------------");
                Console.WriteLine("Set PCAN_ALLOW_STATUS_FRAMES: " + ConvertToParameterOnOff(iAllowStatusFrames));
                Console.WriteLine();
            }
            else
                ShowStatus(stsResult);
        }
        #endregion

        #region PCAN_ALLOW_RTR_FRAMES
        /// <summary>
        /// Shows the status from the reception of RTR frames
        /// </summary>
        private void GetPCAN_ALLOW_RTR_FRAMES()
        {
            TPCANStatus stsResult = PCANBasic.GetValue(PcanHandle, TPCANParameter.PCAN_ALLOW_RTR_FRAMES, out uint iAllowRTRFrames, sizeof(uint));

            if (stsResult == TPCANStatus.PCAN_ERROR_OK)
            {
                Console.WriteLine("-----------------------------------------------------------------------------------------");
                Console.WriteLine("Get PCAN_ALLOW_RTR_FRAMES: " + ConvertToParameterOnOff(iAllowRTRFrames));
                Console.WriteLine();
            }
            else
                ShowStatus(stsResult);
        }

        /// <summary>
        /// De/Activates the reception of RTR frames
        /// </summary>
        /// <param name="value">True to turn on; False to turn off</param>
        private void SetPCAN_ALLOW_RTR_FRAMES(bool value)
        {
            uint iAllowRTRFrames;

            if (value)
                iAllowRTRFrames = PCANBasic.PCAN_PARAMETER_ON;
            else
                iAllowRTRFrames = PCANBasic.PCAN_PARAMETER_OFF;

            TPCANStatus stsResult = PCANBasic.SetValue(PcanHandle, TPCANParameter.PCAN_ALLOW_RTR_FRAMES, ref iAllowRTRFrames, sizeof(uint));

            if (stsResult == TPCANStatus.PCAN_ERROR_OK)
            {
                Console.WriteLine("-----------------------------------------------------------------------------------------");
                Console.WriteLine("Set PCAN_ALLOW_RTR_FRAMES: " + ConvertToParameterOnOff(iAllowRTRFrames));
                Console.WriteLine();
            }
            else
                ShowStatus(stsResult);
        }
        #endregion

        #region PCAN_ALLOW_ERROR_FRAMES
        /// <summary>
        /// Shows the status from the reception of CAN error frames
        /// </summary>
        private void GetPCAN_ALLOW_ERROR_FRAMES()
        {
            TPCANStatus stsResult = PCANBasic.GetValue(PcanHandle, TPCANParameter.PCAN_ALLOW_ERROR_FRAMES, out uint iAllowErrorFrames, sizeof(uint));

            if (stsResult == TPCANStatus.PCAN_ERROR_OK)
            {
                Console.WriteLine("-----------------------------------------------------------------------------------------");
                Console.WriteLine("Get PCAN_ALLOW_ERROR_FRAMES: " + ConvertToParameterOnOff(iAllowErrorFrames));
                Console.WriteLine();
            }
            else
                ShowStatus(stsResult);
        }

        /// <summary>
        /// De/Activates the reception of CAN error frames
        /// </summary>
        /// <param name="value">True to turn on; False to turn off</param>
        private void SetPCAN_ALLOW_ERROR_FRAMES(bool value)
        {
            uint iAllowErrorFrames;

            if (value)
                iAllowErrorFrames = PCANBasic.PCAN_PARAMETER_ON;
            else
                iAllowErrorFrames = PCANBasic.PCAN_PARAMETER_OFF;

            TPCANStatus stsResult = PCANBasic.SetValue(PcanHandle, TPCANParameter.PCAN_ALLOW_ERROR_FRAMES, ref iAllowErrorFrames, sizeof(uint));

            if (stsResult == TPCANStatus.PCAN_ERROR_OK)
            {
                Console.WriteLine("-----------------------------------------------------------------------------------------");
                Console.WriteLine("Set PCAN_ALLOW_ERROR_FRAMES: " + ConvertToParameterOnOff(iAllowErrorFrames));
                Console.WriteLine();
            }
            else
                ShowStatus(stsResult);
        }
        #endregion

        #region PCAN_ALLOW_ECHO_FRAMES
        /// <summary>
        /// Shows the status from the reception of Echo frames
        /// </summary>
        private void GetPCAN_ECHO_ERROR_FRAMES()
        {
            TPCANStatus stsResult = PCANBasic.GetValue(PcanHandle, TPCANParameter.PCAN_ALLOW_ECHO_FRAMES, out uint iAllowEchoFrames, sizeof(uint));

            if (stsResult == TPCANStatus.PCAN_ERROR_OK)
            {
                Console.WriteLine("-----------------------------------------------------------------------------------------");
                Console.WriteLine("Get PCAN_ALLOW_ECHO_FRAMES: " + ConvertToParameterOnOff(iAllowEchoFrames));
                Console.WriteLine();
            }
            else
                ShowStatus(stsResult);
        }

        /// <summary>
        /// De/Activates the reception of Echo frames
        /// </summary>
        /// <param name="value">True to turn on; False to turn off</param>
        private void SetPCAN_ALLOW_ECHO_FRAMES(bool value)
        {
            uint iAllowEchoFrames;

            if (value)
                iAllowEchoFrames = PCANBasic.PCAN_PARAMETER_ON;
            else
                iAllowEchoFrames = PCANBasic.PCAN_PARAMETER_OFF;

            TPCANStatus stsResult = PCANBasic.SetValue(PcanHandle, TPCANParameter.PCAN_ALLOW_ECHO_FRAMES, ref iAllowEchoFrames, sizeof(uint));

            if (stsResult == TPCANStatus.PCAN_ERROR_OK)
            {
                Console.WriteLine("-----------------------------------------------------------------------------------------");
                Console.WriteLine("Set PCAN_ALLOW_ECHO_FRAMES: " + ConvertToParameterOnOff(iAllowEchoFrames));
                Console.WriteLine();
            }
            else
                ShowStatus(stsResult);
        }
        #endregion

        #region PCAN_ACCEPTANCE_FILTER_11BIT
        /// <summary>
        /// Shows the reception filter with a specific 11-bit acceptance code and mask
        /// </summary>
        private void GetPCAN_ACCEPTANCE_FILTER_11BIT()
        {
            TPCANStatus stsResult = PCANBasic.GetValue(PcanHandle, TPCANParameter.PCAN_ACCEPTANCE_FILTER_11BIT, out ulong iAcceptanceFilter11Bit, sizeof(uint));

            if (stsResult == TPCANStatus.PCAN_ERROR_OK)
            {
                Console.WriteLine("-----------------------------------------------------------------------------------------");
                Console.WriteLine("Get PCAN_ACCEPTANCE_FILTER_11BIT: " + iAcceptanceFilter11Bit.ToString("X16") + "h");
                Console.WriteLine();
            }
            else
                ShowStatus(stsResult);
        }

        /// <summary>
        /// Sets the reception filter with a specific 11-bit acceptance code and mask
        /// </summary>
        /// <param name="iacceptancefilter11bit">Acceptance code and mask</param>
        private void SetPCAN_ACCEPTANCE_FILTER_11BIT(ulong iacceptancefilter11bit)
        {
            TPCANStatus stsResult = PCANBasic.SetValue(PcanHandle, TPCANParameter.PCAN_ACCEPTANCE_FILTER_11BIT, ref iacceptancefilter11bit, sizeof(uint));

            if (stsResult == TPCANStatus.PCAN_ERROR_OK)
            {
                Console.WriteLine("-----------------------------------------------------------------------------------------");
                Console.WriteLine("Set PCAN_ACCEPTANCE_FILTER_11BIT: " + iacceptancefilter11bit.ToString("X16") + "h");
                Console.WriteLine();
            }
            else
                ShowStatus(stsResult);
        }
        #endregion

        #region PCAN_ACCEPTANCE_FILTER_29BIT
        /// <summary>
        /// Shows the reception filter with a specific 29-bit acceptance code and mask
        /// </summary>
        private void GetPCAN_ACCEPTANCE_FILTER_29BIT()
        {
            TPCANStatus stsResult = PCANBasic.GetValue(PcanHandle, TPCANParameter.PCAN_ACCEPTANCE_FILTER_29BIT, out ulong acceptanceFilter29Bit, sizeof(uint));

            if (stsResult == TPCANStatus.PCAN_ERROR_OK)
            {
                Console.WriteLine("-----------------------------------------------------------------------------------------");
                Console.WriteLine("Get PCAN_ACCEPTANCE_FILTER_29BIT: " + acceptanceFilter29Bit.ToString("X16") + "h");
                Console.WriteLine();
            }
            else
                ShowStatus(stsResult);
        }

        /// <summary>
        /// Sets the reception filter with a specific 29-bit acceptance code and mask
        /// </summary>
        /// <param name="acceptancefilter29bit">Acceptance code and mask</param>
        private void SetPCAN_ACCEPTANCE_FILTER_29BIT(ulong acceptancefilter29bit)
        {
            TPCANStatus stsResult = PCANBasic.SetValue(PcanHandle, TPCANParameter.PCAN_ACCEPTANCE_FILTER_29BIT, ref acceptancefilter29bit, sizeof(uint));

            if (stsResult == TPCANStatus.PCAN_ERROR_OK)
            {
                Console.WriteLine("-----------------------------------------------------------------------------------------");
                Console.WriteLine("Set PCAN_ACCEPTANCE_FILTER_29BIT: " + acceptancefilter29bit.ToString("X16") + "h");
                Console.WriteLine();
            }
            else
                ShowStatus(stsResult);
        }
        #endregion

        #region PCAN_MESSAGE_FILTER
        /// <summary>
        /// Shows the status of the reception filter
        /// </summary>
        private void GetPCAN_MESSAGE_FILTER()
        {
            TPCANStatus stsResult = PCANBasic.GetValue(PcanHandle, TPCANParameter.PCAN_MESSAGE_FILTER, out uint iMessageFilter, sizeof(uint));

            if (stsResult == TPCANStatus.PCAN_ERROR_OK)
            {
                Console.WriteLine("-----------------------------------------------------------------------------------------");
                Console.WriteLine("Get PCAN_MESSAGE_FILTER: " + ConvertToFilterOpenCloseCustom(iMessageFilter));
                Console.WriteLine();
            }
            else
                ShowStatus(stsResult);
        }

        /// <summary>
        /// De/Activates the reception filter
        /// </summary>
        /// <param name="imessagefilter">Configure reception filter</param>
        private void SetPCAN_MESSAGE_FILTER(uint imessagefilter)
        {
            TPCANStatus stsResult = PCANBasic.SetValue(PcanHandle, TPCANParameter.PCAN_MESSAGE_FILTER, ref imessagefilter, sizeof(uint));

            if (stsResult == TPCANStatus.PCAN_ERROR_OK)
            {
                Console.WriteLine("-----------------------------------------------------------------------------------------");
                Console.WriteLine("Set PCAN_MESSAGE_FILTER: " + ConvertToFilterOpenCloseCustom(imessagefilter));
                Console.WriteLine();
            }
            else
                ShowStatus(stsResult);
        }
        #endregion

        #region PCAN_HARD_RESET_STATUS
        /// <summary>
        /// Shows the status of the hard reset within the PCANBasic.Reset method
        /// </summary>
        private void GetPCAN_HARD_RESET_STATUS()
        {
            TPCANStatus stsResult = PCANBasic.GetValue(PcanHandle, TPCANParameter.PCAN_HARD_RESET_STATUS, out uint iHardResetStatus, sizeof(uint));

            if (stsResult == TPCANStatus.PCAN_ERROR_OK)
            {
                Console.WriteLine("-----------------------------------------------------------------------------------------");
                Console.WriteLine("Get PCAN_HARD_RESET_STATUS: " + ConvertToParameterOnOff(iHardResetStatus));
                Console.WriteLine();
            }
            else
                ShowStatus(stsResult);
        }

        /// <summary>
        /// De/Activates the hard reset within the PCANBasic.Reset method
        /// </summary>
        /// <param name="value">True to turn on; False to turn off</param>
        private void SetPCAN_HARD_RESET_STATUS(bool value)
        {
            uint iHardResetStatus;

            if (value)
                iHardResetStatus = PCANBasic.PCAN_PARAMETER_ON;
            else
                iHardResetStatus = PCANBasic.PCAN_PARAMETER_OFF;

            TPCANStatus stsResult = PCANBasic.SetValue(PcanHandle, TPCANParameter.PCAN_HARD_RESET_STATUS, ref iHardResetStatus, sizeof(uint));

            if (stsResult == TPCANStatus.PCAN_ERROR_OK)
            {
                Console.WriteLine("-----------------------------------------------------------------------------------------");
                Console.WriteLine("Set PCAN_HARD_RESET_STATUS: " + ConvertToParameterOnOff(iHardResetStatus));
                Console.WriteLine();
            }
            else
                ShowStatus(stsResult);
        }
        #endregion

        #region PCAN_LAN_CHANNEL_DIRECTION
        /// <summary>
        /// Shows the communication direction of a PCAN-Channel representing a LAN interface
        /// </summary>
        private void GetPCAN_LAN_CHANNEL_DIRECTION()
        {
            TPCANStatus stsResult = PCANBasic.GetValue(PcanHandle, TPCANParameter.PCAN_LAN_CHANNEL_DIRECTION, out uint iChannelDirection, sizeof(uint));

            if (stsResult == TPCANStatus.PCAN_ERROR_OK)
            {
                Console.WriteLine("-----------------------------------------------------------------------------------------");
                Console.WriteLine("Get PCAN_LAN_CHANNEL_DIRECTION: " + ConvertToChannelDirection(iChannelDirection));
                Console.WriteLine();
            }
            else
                ShowStatus(stsResult);
        }
        #endregion
        #endregion
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
            Console.WriteLine("|                        PCAN-Basic GetSetParameter Example                              |");
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
            //  If it fails, a text describing the current error is returned.
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

        /// <summary>
        /// Convert uint value to readable string value
        /// </summary>
        /// <param name="value">A value representing a PCAN-Channel handle</param>
        /// <returns>A text representing a PCAN-Channel handle</returns>
        private string ConvertToChannelHandle(uint value)
        {
            switch (value)
            {
                case PCANBasic.PCAN_USBBUS1:
                    return "PCAN_USBBUS1";
                case PCANBasic.PCAN_USBBUS2:
                    return "PCAN_USBBUS2";
                case PCANBasic.PCAN_USBBUS3:
                    return "PCAN_USBBUS3";
                case PCANBasic.PCAN_USBBUS4:
                    return "PCAN_USBBUS4";
                case PCANBasic.PCAN_USBBUS5:
                    return "PCAN_USBBUS5";
                case PCANBasic.PCAN_USBBUS6:
                    return "PCAN_USBBUS6";
                case PCANBasic.PCAN_USBBUS7:
                    return "PCAN_USBBUS7";
                case PCANBasic.PCAN_USBBUS8:
                    return "PCAN_USBBUS8";
                case PCANBasic.PCAN_USBBUS9:
                    return "PCAN_USBBUS9";
                case PCANBasic.PCAN_USBBUS10:
                    return "PCAN_USBBUS10";
                case PCANBasic.PCAN_USBBUS11:
                    return "PCAN_USBBUS11";
                case PCANBasic.PCAN_USBBUS12:
                    return "PCAN_USBBUS12";
                case PCANBasic.PCAN_USBBUS13:
                    return "PCAN_USBBUS13";
                case PCANBasic.PCAN_USBBUS14:
                    return "PCAN_USBBUS14";
                case PCANBasic.PCAN_USBBUS15:
                    return "PCAN_USBBUS15";
                case PCANBasic.PCAN_USBBUS16:
                    return "PCAN_USBBUS16";

                case PCANBasic.PCAN_ISABUS1:
                    return "PCAN_ISABUS1";
                case PCANBasic.PCAN_ISABUS2:
                    return "PCAN_ISABUS2";
                case PCANBasic.PCAN_ISABUS3:
                    return "PCAN_ISABUS3";
                case PCANBasic.PCAN_ISABUS4:
                    return "PCAN_ISABUS4";
                case PCANBasic.PCAN_ISABUS5:
                    return "PCAN_ISABUS5";
                case PCANBasic.PCAN_ISABUS6:
                    return "PCAN_ISABUS6";
                case PCANBasic.PCAN_ISABUS7:
                    return "PCAN_ISABUS7";
                case PCANBasic.PCAN_ISABUS8:
                    return "PCAN_ISABUS8";

                case PCANBasic.PCAN_LANBUS1:
                    return "PCAN_LANBUS1";
                case PCANBasic.PCAN_LANBUS2:
                    return "PCAN_LANBUS2";
                case PCANBasic.PCAN_LANBUS3:
                    return "PCAN_LANBUS3";
                case PCANBasic.PCAN_LANBUS4:
                    return "PCAN_LANBUS4";
                case PCANBasic.PCAN_LANBUS5:
                    return "PCAN_LANBUS5";
                case PCANBasic.PCAN_LANBUS6:
                    return "PCAN_LANBUS6";
                case PCANBasic.PCAN_LANBUS7:
                    return "PCAN_LANBUS7";
                case PCANBasic.PCAN_LANBUS8:
                    return "PCAN_LANBUS8";
                case PCANBasic.PCAN_LANBUS9:
                    return "PCAN_LANBUS9";
                case PCANBasic.PCAN_LANBUS10:
                    return "PCAN_LANBUS10";
                case PCANBasic.PCAN_LANBUS11:
                    return "PCAN_LANBUS11";
                case PCANBasic.PCAN_LANBUS12:
                    return "PCAN_LANBUS12";
                case PCANBasic.PCAN_LANBUS13:
                    return "PCAN_LANBUS13";
                case PCANBasic.PCAN_LANBUS14:
                    return "PCAN_LANBUS14";
                case PCANBasic.PCAN_LANBUS15:
                    return "PCAN_LANBUS15";
                case PCANBasic.PCAN_LANBUS16:
                    return "PCAN_LANBUS16";

                case PCANBasic.PCAN_PCCBUS1:
                    return "PCAN_PCCBUS1";
                case PCANBasic.PCAN_PCCBUS2:
                    return "PCAN_PCCBUS2";

                case PCANBasic.PCAN_PCIBUS1:
                    return "PCAN_PCIBUS1";
                case PCANBasic.PCAN_PCIBUS2:
                    return "PCAN_PCIBUS2";
                case PCANBasic.PCAN_PCIBUS3:
                    return "PCAN_PCIBUS3";
                case PCANBasic.PCAN_PCIBUS4:
                    return "PCAN_PCIBUS4";
                case PCANBasic.PCAN_PCIBUS5:
                    return "PCAN_PCIBUS5";
                case PCANBasic.PCAN_PCIBUS6:
                    return "PCAN_PCIBUS6";
                case PCANBasic.PCAN_PCIBUS7:
                    return "PCAN_PCIBUS7";
                case PCANBasic.PCAN_PCIBUS8:
                    return "PCAN_PCIBUS8";
                case PCANBasic.PCAN_PCIBUS9:
                    return "PCAN_PCIBUS9";
                case PCANBasic.PCAN_PCIBUS10:
                    return "PCAN_PCIBUS10";
                case PCANBasic.PCAN_PCIBUS11:
                    return "PCAN_PCIBUS11";
                case PCANBasic.PCAN_PCIBUS12:
                    return "PCAN_PCIBUS12";
                case PCANBasic.PCAN_PCIBUS13:
                    return "PCAN_PCIBUS13";
                case PCANBasic.PCAN_PCIBUS14:
                    return "PCAN_PCIBUS14";
                case PCANBasic.PCAN_PCIBUS15:
                    return "PCAN_PCIBUS15";
                case PCANBasic.PCAN_PCIBUS16:
                    return "PCAN_PCIBUS16";
                default:
                    return "Handle unknown: " + value;
            }
        }

        /// <summary>
        /// Convert uint value to readable string value
        /// </summary>
        /// <param name="value">A value representing an on/off status</param>
        /// <returns>A text representing an on/ff status</returns>
        private string ConvertToParameterOnOff(uint value)
        {
            switch (value)
            {
                case PCANBasic.PCAN_PARAMETER_OFF:
                    return "PCAN_PARAMETER_OFF";
                case PCANBasic.PCAN_PARAMETER_ON:
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
        private string ConvertToChannelDirection(uint value)
        {
            switch(value)
            {
                case PCANBasic.LAN_DIRECTION_READ:
                    return "incoming only";
                case PCANBasic.LAN_DIRECTION_WRITE:
                    return "outgoing only";
                case PCANBasic.LAN_DIRECTION_READ_WRITE:
                    return "bidirectional";
                default:
                    return string.Format("undefined (0x{0:X4})", value);
            }
        }

        /// <summary>
        /// Convert uint value to readable string value
        /// </summary>
        /// <param name="value">A value representing channel features</param>
        /// <returns>A text representing channel features</returns>
        private string ConvertToChannelFeatures(uint value)
        {
            string sFeatures = "";
            if ((value & PCANBasic.FEATURE_FD_CAPABLE) == PCANBasic.FEATURE_FD_CAPABLE)
                sFeatures += "FEATURE_FD_CAPABLE";
            if ((value & PCANBasic.FEATURE_DELAY_CAPABLE) == PCANBasic.FEATURE_DELAY_CAPABLE)
                if (sFeatures != "")
                    sFeatures += ", FEATURE_DELAY_CAPABLE";
                else
                    sFeatures += "FEATURE_DELAY_CAPABLE";
            if ((value & PCANBasic.FEATURE_IO_CAPABLE) == PCANBasic.FEATURE_IO_CAPABLE)
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
        private string ConvertToChannelCondition(uint value)
        {
            switch (value)
            {
                case PCANBasic.PCAN_CHANNEL_UNAVAILABLE:
                    return "PCAN_CHANNEL_UNAVAILABLE";
                case PCANBasic.PCAN_CHANNEL_AVAILABLE:
                    return "PCAN_CHANNEL_AVAILABLE";
                case PCANBasic.PCAN_CHANNEL_OCCUPIED:
                    return "PCAN_CHANNEL_OCCUPIED";
                case PCANBasic.PCAN_CHANNEL_PCANVIEW:
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
        private string ConvertToFilterOpenCloseCustom(uint value)
        {
            switch (value)
            {
                case PCANBasic.PCAN_FILTER_CLOSE:
                    return "PCAN_FILTER_CLOSE";
                case PCANBasic.PCAN_FILTER_OPEN:
                    return "PCAN_FILTER_OPEN";
                case PCANBasic.PCAN_FILTER_CUSTOM:
                    return "PCAN_FILTER_CUSTOM";
                default:
                    return "Status unknown: " + value;
            }
        }
        #endregion
    }
}
