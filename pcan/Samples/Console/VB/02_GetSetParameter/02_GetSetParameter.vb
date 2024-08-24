' Inclusion of PEAK PCAN-Basic namespace
'
Imports GetSetParameter.Peak.Can.Basic
Imports TPCANHandle = System.UInt16
Imports System.Text

Class GetSetParameter

#Region "Defines"
    ''' <summary>
    ''' Sets the PCANHandle (Hardware Channel)
    ''' </summary>
    Private Const PcanHandle = PCANBasic.PCAN_USBBUS1
    ''' <summary>
    ''' Sets the desired connection mode (CAN = false / CAN-FD = true)
    ''' </summary>
    Private Const IsFD = False
    ''' <summary>
    ''' Sets the bitrate for normal CAN devices
    ''' </summary>
    Private Const Bitrate = TPCANBaudrate.PCAN_BAUD_500K
    ''' <summary>
    ''' Sets the bitrate for CAN FD devices. 
    ''' Example - Bitrate Nom: 1Mbit/s Data: 2Mbit/s:
    '''   "f_clock_mhz=20, nom_brp=5, nom_tseg1=2, nom_tseg2=1, nom_sjw=1, data_brp=2, data_tseg1=3, data_tseg2=1, data_sjw=1"
    ''' </summary>
    Private Const BitrateFD = "f_clock_mhz=20, nom_brp=5, nom_tseg1=2, nom_tseg2=1, nom_sjw=1, data_brp=2, data_tseg1=3, data_tseg2=1, data_sjw=1"
#End Region

#Region "Members"
    ''' <summary>
    ''' Shows if DLL was found
    ''' </summary>
    Private m_DLLFound As Boolean
#End Region

    Sub Main()
        ShowConfigurationHelp() ' Shows information about this sample
        ShowCurrentConfiguration() ' Shows the current parameters configuration

        ' Checks if PCANBasic.dll is available, if not, the program terminates
        m_DLLFound = CheckForLibrary()
        If Not m_DLLFound Then
            Return
        End If

        Dim stsResult As TPCANStatus
        ' Initialization of the selected channel
        If IsFD Then
            stsResult = PCANBasic.InitializeFD(PcanHandle, BitrateFD)
        Else
            stsResult = PCANBasic.Initialize(PcanHandle, Bitrate)
        End If

        If stsResult <> TPCANStatus.PCAN_ERROR_OK Then
            Console.WriteLine("Can not initialize. Please check the defines in the code.")
            ShowStatus(stsResult)
            Console.WriteLine()
            Console.WriteLine("Press any key to close")
            Console.Read()
            Return
        End If

        Console.WriteLine("Successfully initialized.")
        Console.WriteLine("Press any key to get/set parameter")
        Console.ReadKey()
        Console.WriteLine()

        RunSelectedCommands()

        Console.WriteLine()
        Console.WriteLine("Press any key to close")
        Console.ReadKey()
    End Sub

    Protected Overrides Sub Finalize()
        If m_DLLFound Then
            PCANBasic.Uninitialize(PCANBasic.PCAN_NONEBUS)
        End If
    End Sub

#Region "Main-Functions"
    ''' <summary>
    ''' Runs all commands for get or set parameters
    ''' </summary>
    Private Sub RunSelectedCommands()
        '' Fill commands here 
        Console.WriteLine("Fill ""RunSelectedCommands""-function with parameter functions from ""Parameter commands""-Region in the code.")
    End Sub

#Region "Parameter commands"
#Region "PCAN_DEVICE_ID"
    ''' <summary>
    ''' Shows device identifier parameter
    ''' </summary>
    Private Sub GetPCAN_DEVICE_ID()
        Dim stsResult As TPCANStatus
        Dim iDeviceID As UInteger
        stsResult = PCANBasic.GetValue(PcanHandle, TPCANParameter.PCAN_DEVICE_ID, iDeviceID, CType(System.Runtime.InteropServices.Marshal.SizeOf(iDeviceID), UInteger))

        If stsResult = TPCANStatus.PCAN_ERROR_OK Then
            Console.WriteLine("-----------------------------------------------------------------------------------------")
            Console.WriteLine("Get PCAN_DEVICE_ID: " + iDeviceID.ToString())
            Console.WriteLine()

        Else
            ShowStatus(stsResult)
        End If
    End Sub

    ''' <summary>
    ''' Sets device identifier parameter
    ''' </summary>
    ''' <param name="iDeviceID"></param>
    Private Sub SetPCAN_DEVICE_ID(ByVal iDeviceID As UInteger)
        Dim stsResult As TPCANStatus
        stsResult = PCANBasic.SetValue(PcanHandle, TPCANParameter.PCAN_DEVICE_ID, iDeviceID, CType(System.Runtime.InteropServices.Marshal.SizeOf(iDeviceID), UInteger))

        If stsResult = TPCANStatus.PCAN_ERROR_OK Then
            Console.WriteLine("-----------------------------------------------------------------------------------------")
            Console.WriteLine("Set PCAN_DEVICE_ID: " + iDeviceID.ToString())
            Console.WriteLine()

        Else
            ShowStatus(stsResult)
        End If
    End Sub
#End Region

#Region "PCAN_ATTACHED_CHANNELS"
    ''' <summary>
    ''' Shows all information about attached channels
    ''' </summary>
    Private Sub GetPCAN_ATTACHED_CHANNELS()
        Dim stsResult As TPCANStatus
        Dim iChannelsCount As UInt32
        stsResult = PCANBasic.GetValue(PCANBasic.PCAN_NONEBUS, TPCANParameter.PCAN_ATTACHED_CHANNELS_COUNT, iChannelsCount, CType(System.Runtime.InteropServices.Marshal.SizeOf(iChannelsCount), UInteger))

        If stsResult = TPCANStatus.PCAN_ERROR_OK Then
            Dim ciChannelInformation(iChannelsCount - 1) As TPCANChannelInformation

            stsResult = PCANBasic.GetValue(PCANBasic.PCAN_NONEBUS, TPCANParameter.PCAN_ATTACHED_CHANNELS, ciChannelInformation)
            If (stsResult = TPCANStatus.PCAN_ERROR_OK) Then
                Console.WriteLine("-----------------------------------------------------------------------------------------")
                Console.WriteLine("Get PCAN_ATTACHED_CHANNELS:")

                For Each currentChannelInformation As TPCANChannelInformation In ciChannelInformation
                    Console.WriteLine("---------------------------")
                    Console.WriteLine("channel_handle:      " + ConvertToChannelHandle(currentChannelInformation.channel_handle))
                    Console.WriteLine("device_type:         " + currentChannelInformation.device_type.ToString())
                    Console.WriteLine("controller_number:   " + currentChannelInformation.controller_number.ToString())
                    Console.WriteLine("device_features:     " + ConvertToChannelFeatures(currentChannelInformation.device_features))
                    Console.WriteLine("device_name:         " + currentChannelInformation.device_name)
                    Console.WriteLine("device_id:           " + currentChannelInformation.device_id.ToString())
                    Console.WriteLine("channel_condition:   " + ConvertToChannelCondition(currentChannelInformation.channel_condition))
                Next
                Console.WriteLine()
            End If
        End If
        If stsResult <> TPCANStatus.PCAN_ERROR_OK Then
            ShowStatus(stsResult)
        End If
    End Sub
#End Region

#Region "PCAN_CHANNEL_CONDITION"
    ''' <summary>
    ''' Shows the status of selected PCAN-Channel
    ''' </summary>
    Private Sub GetPCAN_CHANNEL_CONDITION()
        Dim stsResult As TPCANStatus
        Dim iChannelCondition As UInteger
        stsResult = PCANBasic.GetValue(PcanHandle, TPCANParameter.PCAN_CHANNEL_CONDITION, iChannelCondition, CType(System.Runtime.InteropServices.Marshal.SizeOf(iChannelCondition), UInteger))

        If stsResult = TPCANStatus.PCAN_ERROR_OK Then
            Console.WriteLine("-----------------------------------------------------------------------------------------")
            Console.WriteLine("Get PCAN_CHANNEL_CONDITION: " + ConvertToChannelCondition(iChannelCondition))
            Console.WriteLine()
        Else
            ShowStatus(stsResult)
        End If
    End Sub
#End Region

#Region "PCAN_CHANNEL_IDENTIFYING"
    ''' <summary>
    ''' Shows the status from the status LED of the USB devices
    ''' </summary>
    Private Sub GetPCAN_CHANNEL_IDENTIFYING()
        Dim stsResult As TPCANStatus
        Dim iChannelIdentifying As UInteger
        stsResult = PCANBasic.GetValue(PcanHandle, TPCANParameter.PCAN_CHANNEL_IDENTIFYING, iChannelIdentifying, CType(System.Runtime.InteropServices.Marshal.SizeOf(iChannelIdentifying), UInteger))

        If stsResult = TPCANStatus.PCAN_ERROR_OK Then
            Console.WriteLine("-----------------------------------------------------------------------------------------")
            Console.WriteLine("Get PCAN_CHANNEL_IDENTIFYING: " + ConvertToParameterOnOff(iChannelIdentifying))
            Console.WriteLine()
        Else
            ShowStatus(stsResult)
        End If
    End Sub

    ''' <summary>
    ''' De/Activates the status LED of the USB devices
    ''' </summary>
    ''' <param name="value">True to turn on; False to turn off</param>
    Private Sub SetPCAN_CHANNEL_IDENTIFYING(ByVal value As Boolean)
        Dim ciChannelIdentifying As UInteger
        If value Then
            ciChannelIdentifying = PCANBasic.PCAN_PARAMETER_ON
        Else
            ciChannelIdentifying = PCANBasic.PCAN_PARAMETER_OFF
        End If

        Dim stsResult As TPCANStatus
        stsResult = PCANBasic.SetValue(PcanHandle, TPCANParameter.PCAN_CHANNEL_IDENTIFYING, ciChannelIdentifying, CType(System.Runtime.InteropServices.Marshal.SizeOf(ciChannelIdentifying), UInteger))

        If stsResult = TPCANStatus.PCAN_ERROR_OK Then
            Console.WriteLine("-----------------------------------------------------------------------------------------")
            Console.WriteLine("Set PCAN_CHANNEL_IDENTIFYING: " + ConvertToParameterOnOff(ciChannelIdentifying))
            Console.WriteLine()
        Else
            ShowStatus(stsResult)
        End If
    End Sub
#End Region

#Region "PCAN_CHANNEL_FEATURES"
    ''' <summary>
    ''' Shows information about features
    ''' </summary>
    Private Sub GetPCAN_CHANNEL_FEATURES()
        Dim stsResult As TPCANStatus
        Dim iChannelFeatures As UInteger
        stsResult = PCANBasic.GetValue(PcanHandle, TPCANParameter.PCAN_CHANNEL_FEATURES, iChannelFeatures, CType(System.Runtime.InteropServices.Marshal.SizeOf(iChannelFeatures), UInteger))

        If stsResult = TPCANStatus.PCAN_ERROR_OK Then
            Console.WriteLine("-----------------------------------------------------------------------------------------")
            Console.WriteLine("Get PCAN_CHANNEL_FEATURES: " + ConvertToChannelFeatures(iChannelFeatures))
            Console.WriteLine()
        Else
            ShowStatus(stsResult)
        End If
    End Sub
#End Region

#Region "PCAN_BITRATE_ADAPTING"
    ''' <summary>
    ''' Shows the status from Bitrate-Adapting mode
    ''' </summary>
    Private Sub GetPCAN_BITRATE_ADAPTING()
        Dim stsResult As TPCANStatus
        Dim iBitrateAdapting As UInteger
        stsResult = PCANBasic.GetValue(PcanHandle, TPCANParameter.PCAN_BITRATE_ADAPTING, iBitrateAdapting, CType(System.Runtime.InteropServices.Marshal.SizeOf(iBitrateAdapting), UInteger))

        If stsResult = TPCANStatus.PCAN_ERROR_OK Then
            Console.WriteLine("-----------------------------------------------------------------------------------------")
            Console.WriteLine("Get PCAN_BITRATE_ADAPTING: " + ConvertToParameterOnOff(iBitrateAdapting))
            Console.WriteLine()
        Else
            ShowStatus(stsResult)
        End If
    End Sub

    ''' <summary>
    ''' De/Activates the Bitrate-Adapting mode
    ''' </summary>
    ''' <param name="value">True to turn on; False to turn off</param>
    Private Sub SetPCAN_BITRATE_ADAPTING(ByVal value As Boolean)
        Dim iBitrateAdapting As UInteger

        ' Note: SetPCAN_BITRATE_ADAPTING requires an uninitialized channel
        '
        PCANBasic.Uninitialize(PCANBasic.PCAN_NONEBUS)

        If value Then
            iBitrateAdapting = PCANBasic.PCAN_PARAMETER_ON
        Else
            iBitrateAdapting = PCANBasic.PCAN_PARAMETER_OFF
        End If

        Dim stsResult As TPCANStatus
        stsResult = PCANBasic.SetValue(PcanHandle, TPCANParameter.PCAN_BITRATE_ADAPTING, iBitrateAdapting, CType(System.Runtime.InteropServices.Marshal.SizeOf(iBitrateAdapting), UInteger))

        If stsResult = TPCANStatus.PCAN_ERROR_OK Then
            Console.WriteLine("-----------------------------------------------------------------------------------------")
            Console.WriteLine("Set PCAN_BITRATE_ADAPTING: " + ConvertToParameterOnOff(iBitrateAdapting))
            Console.WriteLine()
        Else
            ShowStatus(stsResult)
        End If

        ' Initialization of the selected channel
        If IsFD Then
            stsResult = PCANBasic.InitializeFD(PcanHandle, BitrateFD)
        Else
            stsResult = PCANBasic.Initialize(PcanHandle, Bitrate)
        End If

        If stsResult <> TPCANStatus.PCAN_ERROR_OK Then
            Console.WriteLine("Error while re-initializing the channel.")
            ShowStatus(stsResult)
        End If

    End Sub
#End Region

#Region "PCAN_ALLOW_STATUS_FRAMES"
    ''' <summary>
    ''' Shows the status from the reception of status frames
    ''' </summary>
    Private Sub GetPCAN_ALLOW_STATUS_FRAMES()
        Dim stsResult As TPCANStatus
        Dim iAllowStatusFrames As UInteger
        stsResult = PCANBasic.GetValue(PcanHandle, TPCANParameter.PCAN_ALLOW_STATUS_FRAMES, iAllowStatusFrames, CType(System.Runtime.InteropServices.Marshal.SizeOf(iAllowStatusFrames), UInteger))

        If stsResult = TPCANStatus.PCAN_ERROR_OK Then
            Console.WriteLine("-----------------------------------------------------------------------------------------")
            Console.WriteLine("Get PCAN_ALLOW_STATUS_FRAMES: " + ConvertToParameterOnOff(iAllowStatusFrames))
            Console.WriteLine()
        Else
            ShowStatus(stsResult)
        End If
    End Sub

    ''' <summary>
    ''' De/Activates the reception of status frames
    ''' </summary>
    ''' <param name="value">True to turn on; False to turn off</param>
    Private Sub SetPCAN_ALLOW_STATUS_FRAMES(ByVal value As Boolean)
        Dim iAllowStatusFrames As UInteger
        If value Then
            iAllowStatusFrames = PCANBasic.PCAN_PARAMETER_ON
        Else
            iAllowStatusFrames = PCANBasic.PCAN_PARAMETER_OFF
        End If

        Dim stsResult As TPCANStatus
        stsResult = PCANBasic.SetValue(PcanHandle, TPCANParameter.PCAN_ALLOW_STATUS_FRAMES, iAllowStatusFrames, CType(System.Runtime.InteropServices.Marshal.SizeOf(iAllowStatusFrames), UInteger))

        If stsResult = TPCANStatus.PCAN_ERROR_OK Then
            Console.WriteLine("-----------------------------------------------------------------------------------------")
            Console.WriteLine("Set PCAN_ALLOW_STATUS_FRAMES: " + ConvertToParameterOnOff(iAllowStatusFrames))
            Console.WriteLine()
        Else
            ShowStatus(stsResult)
        End If
    End Sub
#End Region

#Region "PCAN_ALLOW_RTR_FRAMES"
    ''' <summary>
    ''' Shows the status from the reception of RTR frames
    ''' </summary>
    Private Sub GetPCAN_ALLOW_RTR_FRAMES()
        Dim stsResult As TPCANStatus
        Dim iAllowRTRFrames As UInteger
        stsResult = PCANBasic.GetValue(PcanHandle, TPCANParameter.PCAN_ALLOW_RTR_FRAMES, iAllowRTRFrames, CType(System.Runtime.InteropServices.Marshal.SizeOf(iAllowRTRFrames), UInteger))

        If stsResult = TPCANStatus.PCAN_ERROR_OK Then
            Console.WriteLine("-----------------------------------------------------------------------------------------")
            Console.WriteLine("Get PCAN_ALLOW_RTR_FRAMES: " + ConvertToParameterOnOff(iAllowRTRFrames))
            Console.WriteLine()
        Else
            ShowStatus(stsResult)
        End If
    End Sub

    ''' <summary>
    ''' De/Activates the reception of RTR frames
    ''' </summary>
    ''' <param name="value">True to turn on; False to turn off</param>
    Private Sub SetPCAN_ALLOW_RTR_FRAMES(ByVal value As Boolean)
        Dim iAllowRTRFrames As UInteger
        If value Then
            iAllowRTRFrames = PCANBasic.PCAN_PARAMETER_ON
        Else
            iAllowRTRFrames = PCANBasic.PCAN_PARAMETER_OFF
        End If

        Dim stsResult As TPCANStatus
        stsResult = PCANBasic.SetValue(PcanHandle, TPCANParameter.PCAN_ALLOW_RTR_FRAMES, iAllowRTRFrames, CType(System.Runtime.InteropServices.Marshal.SizeOf(iAllowRTRFrames), UInteger))

        If stsResult = TPCANStatus.PCAN_ERROR_OK Then
            Console.WriteLine("-----------------------------------------------------------------------------------------")
            Console.WriteLine("Set PCAN_ALLOW_RTR_FRAMES: " + ConvertToParameterOnOff(iAllowRTRFrames))
            Console.WriteLine()
        Else
            ShowStatus(stsResult)
        End If
    End Sub
#End Region

#Region "PCAN_ALLOW_ERROR_FRAMES"
    ''' <summary>
    ''' Shows the status from the reception of CAN error frames
    ''' </summary>
    Private Sub GetPCAN_ALLOW_ERROR_FRAMES()
        Dim stsResult As TPCANStatus
        Dim iAllowErrorFrames As UInteger
        stsResult = PCANBasic.GetValue(PcanHandle, TPCANParameter.PCAN_ALLOW_ERROR_FRAMES, iAllowErrorFrames, CType(System.Runtime.InteropServices.Marshal.SizeOf(iAllowErrorFrames), UInteger))

        If stsResult = TPCANStatus.PCAN_ERROR_OK Then
            Console.WriteLine("-----------------------------------------------------------------------------------------")
            Console.WriteLine("Get PCAN_ALLOW_ERROR_FRAMES: " + ConvertToParameterOnOff(iAllowErrorFrames))
            Console.WriteLine()
        Else
            ShowStatus(stsResult)
        End If
    End Sub

    ''' <summary>
    ''' De/Activates the reception of CAN error frames
    ''' </summary>
    ''' <param name="value">True to turn on; False to turn off</param>
    Private Sub SetPCAN_ALLOW_ERROR_FRAMES(ByVal value As Boolean)
        Dim iAllowErrorFrames As UInteger
        If value Then
            iAllowErrorFrames = PCANBasic.PCAN_PARAMETER_ON
        Else
            iAllowErrorFrames = PCANBasic.PCAN_PARAMETER_OFF
        End If

        Dim stsResult As TPCANStatus
        stsResult = PCANBasic.SetValue(PcanHandle, TPCANParameter.PCAN_ALLOW_ERROR_FRAMES, iAllowErrorFrames, CType(System.Runtime.InteropServices.Marshal.SizeOf(iAllowErrorFrames), UInteger))

        If stsResult = TPCANStatus.PCAN_ERROR_OK Then
            Console.WriteLine("-----------------------------------------------------------------------------------------")
            Console.WriteLine("Set PCAN_ALLOW_ERROR_FRAMES: " + ConvertToParameterOnOff(iAllowErrorFrames))
            Console.WriteLine()
        Else
            ShowStatus(stsResult)
        End If
    End Sub
#End Region

#Region "PCAN_ALLOW_ECHO_FRAMES"
    ''' <summary>
    ''' Shows the status from the reception of Echo frames
    ''' </summary>
    Private Sub GetPCAN_ALLOW_ECHO_FRAMES()
        Dim stsResult As TPCANStatus
        Dim iAllowEchoFrames As UInteger
        stsResult = PCANBasic.GetValue(PcanHandle, TPCANParameter.PCAN_ALLOW_ECHO_FRAMES, iAllowEchoFrames, CType(System.Runtime.InteropServices.Marshal.SizeOf(iAllowEchoFrames), UInteger))

        If stsResult = TPCANStatus.PCAN_ERROR_OK Then
            Console.WriteLine("-----------------------------------------------------------------------------------------")
            Console.WriteLine("Get PCAN_ALLOW_ECHO_FRAMES: " + ConvertToParameterOnOff(iAllowEchoFrames))
            Console.WriteLine()
        Else
            ShowStatus(stsResult)
        End If
    End Sub

    ''' <summary>
    ''' De/Activates the reception of Echo frames
    ''' </summary>
    ''' <param name="value">True to turn on; False to turn off</param>
    Private Sub SetPCAN_ALLOW_ECHO_FRAMES(ByVal value As Boolean)
        Dim iAllowEchoFrames As UInteger
        If value Then
            iAllowEchoFrames = PCANBasic.PCAN_PARAMETER_ON
        Else
            iAllowEchoFrames = PCANBasic.PCAN_PARAMETER_OFF
        End If

        Dim stsResult As TPCANStatus
        stsResult = PCANBasic.SetValue(PcanHandle, TPCANParameter.PCAN_ALLOW_ECHO_FRAMES, iAllowEchoFrames, CType(System.Runtime.InteropServices.Marshal.SizeOf(iAllowEchoFrames), UInteger))

        If stsResult = TPCANStatus.PCAN_ERROR_OK Then
            Console.WriteLine("-----------------------------------------------------------------------------------------")
            Console.WriteLine("Set PCAN_ALLOW_ECHO_FRAMES: " + ConvertToParameterOnOff(iAllowEchoFrames))
            Console.WriteLine()
        Else
            ShowStatus(stsResult)
        End If
    End Sub
#End Region

#Region "PCAN_ACCEPTANCE_FILTER_11BIT"
    ''' <summary>
    ''' Shows the reception filter with a specific 11-bit acceptance code and mask
    ''' </summary>
    Private Sub GetPCAN_ACCEPTANCE_FILTER_11BIT()
        Dim stsResult As TPCANStatus
        Dim iAcceptanceFilter11Bit As ULong
        stsResult = PCANBasic.GetValue(PcanHandle, TPCANParameter.PCAN_ACCEPTANCE_FILTER_11BIT, iAcceptanceFilter11Bit, CType(System.Runtime.InteropServices.Marshal.SizeOf(iAcceptanceFilter11Bit), UInteger))

        If stsResult = TPCANStatus.PCAN_ERROR_OK Then
            Console.WriteLine("-----------------------------------------------------------------------------------------")
            Console.WriteLine("Get PCAN_ACCEPTANCE_FILTER_11BIT: " + iAcceptanceFilter11Bit.ToString("X16") + "h")
            Console.WriteLine()
        Else
            ShowStatus(stsResult)
        End If
    End Sub

    ''' <summary>
    ''' Sets the reception filter with a specific 11-bit acceptance code and mask
    ''' </summary>
    ''' <param name="iAcceptanceFilter11Bit">Acceptance code and mask</param>
    Private Sub SetPCAN_ACCEPTANCE_FILTER_11BIT(ByVal iAcceptanceFilter11Bit As ULong)
        Dim stsResult As TPCANStatus
        stsResult = PCANBasic.SetValue(PcanHandle, TPCANParameter.PCAN_ACCEPTANCE_FILTER_11BIT, iAcceptanceFilter11Bit, CType(System.Runtime.InteropServices.Marshal.SizeOf(iAcceptanceFilter11Bit), UInteger))

        If stsResult = TPCANStatus.PCAN_ERROR_OK Then
            Console.WriteLine("-----------------------------------------------------------------------------------------")
            Console.WriteLine("Set PCAN_ACCEPTANCE_FILTER_11BIT: " + iAcceptanceFilter11Bit.ToString("X16") + "h")
            Console.WriteLine()
        Else
            ShowStatus(stsResult)
        End If
    End Sub
#End Region

#Region "PCAN_ACCEPTANCE_FILTER_29BIT"
    ''' <summary>
    ''' Shows the reception filter with a specific 29-bit acceptance code and mask
    ''' </summary>
    Private Sub GetPCAN_ACCEPTANCE_FILTER_29BIT()
        Dim stsResult As TPCANStatus
        Dim iAcceptanceFilter29Bit As ULong
        stsResult = PCANBasic.GetValue(PcanHandle, TPCANParameter.PCAN_ACCEPTANCE_FILTER_29BIT, iAcceptanceFilter29Bit, CType(System.Runtime.InteropServices.Marshal.SizeOf(iAcceptanceFilter29Bit), UInteger))

        If stsResult = TPCANStatus.PCAN_ERROR_OK Then
            Console.WriteLine("-----------------------------------------------------------------------------------------")
            Console.WriteLine("Get PCAN_ACCEPTANCE_FILTER_29BIT: " + iAcceptanceFilter29Bit.ToString("X16") + "h")
            Console.WriteLine()
        Else
            ShowStatus(stsResult)
        End If
    End Sub

    ''' <summary>
    ''' Sets the reception filter with a specific 29-bit acceptance code and mask
    ''' </summary>
    ''' <param name="iacceptancefilter29bit">Acceptance code and mask</param>
    Private Sub SetPCAN_ACCEPTANCE_FILTER_29BIT(ByVal iacceptancefilter29bit As ULong)
        Dim stsResult As TPCANStatus
        stsResult = PCANBasic.SetValue(PcanHandle, TPCANParameter.PCAN_ACCEPTANCE_FILTER_29BIT, iacceptancefilter29bit, CType(System.Runtime.InteropServices.Marshal.SizeOf(iacceptancefilter29bit), UInteger))

        If stsResult = TPCANStatus.PCAN_ERROR_OK Then
            Console.WriteLine("-----------------------------------------------------------------------------------------")
            Console.WriteLine("Set PCAN_ACCEPTANCE_FILTER_29BIT: " + iacceptancefilter29bit.ToString("X16") + "h")
            Console.WriteLine()
        Else
            ShowStatus(stsResult)
        End If
    End Sub
#End Region

#Region "PCAN_MESSAGE_FILTER"
    ''' <summary>
    ''' Shows the status of the reception filter
    ''' </summary>
    Private Sub GetPCAN_MESSAGE_FILTER()
        Dim stsResult As TPCANStatus
        Dim imessagefilter As UInteger
        stsResult = PCANBasic.GetValue(PcanHandle, TPCANParameter.PCAN_MESSAGE_FILTER, imessagefilter, CType(System.Runtime.InteropServices.Marshal.SizeOf(imessagefilter), UInteger))

        If stsResult = TPCANStatus.PCAN_ERROR_OK Then
            Console.WriteLine("-----------------------------------------------------------------------------------------")
            Console.WriteLine("Get PCAN_MESSAGE_FILTER: " + ConvertToFilterOpenCloseCustom(imessagefilter))
            Console.WriteLine()
        Else
            ShowStatus(stsResult)
        End If
    End Sub

    ''' <summary>
    ''' De/Activates the reception filter
    ''' </summary>
    ''' <param name="imessagefilter">Configure reception filter</param>
    Private Sub SetPCAN_MESSAGE_FILTER(ByVal imessagefilter As UInteger)
        Dim stsResult As TPCANStatus
        stsResult = PCANBasic.SetValue(PcanHandle, TPCANParameter.PCAN_MESSAGE_FILTER, imessagefilter, CType(System.Runtime.InteropServices.Marshal.SizeOf(imessagefilter), UInteger))

        If stsResult = TPCANStatus.PCAN_ERROR_OK Then
            Console.WriteLine("-----------------------------------------------------------------------------------------")
            Console.WriteLine("Set PCAN_MESSAGE_FILTER: " + ConvertToFilterOpenCloseCustom(imessagefilter))
            Console.WriteLine()
        Else
            ShowStatus(stsResult)
        End If
    End Sub
#End Region

#Region "PCAN_HARD_RESET_STATUS"
    ''' <summary>
    ''' Shows the status from of the hard reset within the PCANBasic.Reset method
    ''' </summary>
    Private Sub GetPCAN_HARD_RESET_STATUS()
        Dim stsResult As TPCANStatus
        Dim iHardResetStatus As UInteger
        stsResult = PCANBasic.GetValue(PcanHandle, TPCANParameter.PCAN_HARD_RESET_STATUS, iHardResetStatus, CType(System.Runtime.InteropServices.Marshal.SizeOf(iHardResetStatus), UInteger))

        If stsResult = TPCANStatus.PCAN_ERROR_OK Then
            Console.WriteLine("-----------------------------------------------------------------------------------------")
            Console.WriteLine("Get PCAN_HARD_RESET_STATUS: " + ConvertToParameterOnOff(iHardResetStatus))
            Console.WriteLine()
        Else
            ShowStatus(stsResult)
        End If
    End Sub

    ''' <summary>
    ''' De/Activates the hard reset within the PCANBasic.Reset method
    ''' </summary>
    ''' <param name="value">True to turn on; False to turn off</param>
    Private Sub SetPCAN_HARD_RESET_STATUS(ByVal value As Boolean)
        Dim iHardResetStatus As UInteger
        If value Then
            iHardResetStatus = PCANBasic.PCAN_PARAMETER_ON
        Else
            iHardResetStatus = PCANBasic.PCAN_PARAMETER_OFF
        End If

        Dim stsResult As TPCANStatus
        stsResult = PCANBasic.SetValue(PcanHandle, TPCANParameter.PCAN_ALLOW_ECHO_FRAMES, iHardResetStatus, CType(System.Runtime.InteropServices.Marshal.SizeOf(iHardResetStatus), UInteger))

        If stsResult = TPCANStatus.PCAN_ERROR_OK Then
            Console.WriteLine("-----------------------------------------------------------------------------------------")
            Console.WriteLine("Set PCAN_HARD_RESET_STATUS: " + ConvertToParameterOnOff(iHardResetStatus))
            Console.WriteLine()
        Else
            ShowStatus(stsResult)
        End If
    End Sub
#End Region

#Region "PCAN_LAN_CHANNEL_DIRECTION"
    ''' <summary>
    ''' Shows the communication direction of a PCAN-Channel representing a LAN interface
    ''' </summary>
    Private Sub GetPCAN_LAN_CHANNEL_DIRECTION()
        Dim stsResult As TPCANStatus
        Dim iChannelDirection As UInteger
        stsResult = PCANBasic.GetValue(PcanHandle, TPCANParameter.PCAN_LAN_CHANNEL_DIRECTION, iChannelDirection, CType(System.Runtime.InteropServices.Marshal.SizeOf(iChannelDirection), UInteger))

        If stsResult = TPCANStatus.PCAN_ERROR_OK Then
            Console.WriteLine("-----------------------------------------------------------------------------------------")
            Console.WriteLine("Get PCAN_LAN_CHANNEL_DIRECTION: " + ConvertToChannelDirection(iChannelDirection))
            Console.WriteLine()
        Else
            ShowStatus(stsResult)
        End If
    End Sub
#End Region
#End Region
#End Region

#Region "Help-Functions"
    ''' <summary>
    ''' Checks for availability of the PCANBasic labrary
    ''' </summary>
    ''' <returns>If the library was found or not</returns>
    Private Function CheckForLibrary() As Boolean
        ' Check for dll file
        Try
            PCANBasic.Uninitialize(PCANBasic.PCAN_NONEBUS)
            Return True
        Catch ex As Exception
            Console.WriteLine("Unable to find the library: PCANBasic.dll !")
            Console.WriteLine("Press any key to close")
            Console.ReadKey()
        End Try
        Return False
    End Function

    ''' <summary>
    ''' Shows/prints the configurable parameters for this sample and information about them
    ''' </summary>
    Private Sub ShowConfigurationHelp()
        Console.WriteLine("=========================================================================================")
        Console.WriteLine("|                         PCAN-Basic GetSetParameter Example                             |")
        Console.WriteLine("=========================================================================================")
        Console.WriteLine("Following parameters are to be adjusted before launching, according to the hardware used |")
        Console.WriteLine("                                                                                         |")
        Console.WriteLine("* PcanHandle: Numeric value that represents the handle of the PCAN-Basic channel to use. |")
        Console.WriteLine("              See 'PCAN-Handle Definitions' within the documentation                     |")
        Console.WriteLine("* IsFD: Boolean value that indicates the communication mode, CAN (false) or CAN-FD (true)|")
        Console.WriteLine("* Bitrate: Numeric value that represents the BTR0/BR1 bitrate value to be used for CAN   |")
        Console.WriteLine("           communication                                                                 |")
        Console.WriteLine("* BitrateFD: String value that represents the nominal/data bitrate value to be used for  |")
        Console.WriteLine("             CAN-FD communication                                                        |")
        Console.WriteLine("=========================================================================================")
        Console.WriteLine("")
    End Sub

    ''' <summary>
    ''' Shows/prints the configured parameters
    ''' </summary>
    Private Sub ShowCurrentConfiguration()
        Console.WriteLine("Parameter values used")
        Console.WriteLine("----------------------")
        Console.WriteLine("* PCANHandle: " + FormatChannelName(PcanHandle, IsFD))
        Console.WriteLine("* IsFD: " + IsFD.ToString)
        Console.WriteLine("* Bitrate: " + ConvertBitrateToString(Bitrate))
        Console.WriteLine("* BitrateFD: " + BitrateFD)
        Console.WriteLine("")
    End Sub

    ''' <summary>
    ''' Shows formatted status
    ''' </summary>
    ''' <param name="status">Will be formatted</param>
    Private Sub ShowStatus(status As TPCANStatus)
        Console.WriteLine("=========================================================================================")
        Console.WriteLine(GetFormattedError(status))
        Console.WriteLine("=========================================================================================")
    End Sub

    ''' <summary>
    ''' Gets the formatted text for a PCAN-Basic channel handle
    ''' </summary>
    ''' <param name="handle">PCAN-Basic Handle to format</param>
    ''' <param name="isFD">If the channel is FD capable</param>
    ''' <returns>The formatted text for a channel</returns>
    Private Function FormatChannelName(ByVal handle As TPCANHandle, ByVal isFD As Boolean) As String
        Dim devDevice As TPCANDevice
        Dim byChannel As Byte

        ' Gets the owner device and channel for a PCAN-Basic handle
        '
        If handle < &H100 Then
            devDevice = DirectCast(CType(handle >> 4, Byte), TPCANDevice)
            byChannel = CByte((handle And &HF))
        Else
            devDevice = DirectCast(CType(handle >> 8, Byte), TPCANDevice)
            byChannel = CByte((handle And &HFF))
        End If

        ' Constructs the PCAN-Basic Channel name and return it
        '
        If (isFD) Then
            Return String.Format("{0}:FD {1} ({2:X2}h)", devDevice, byChannel, handle)
        Else
            Return String.Format("{0} {1} ({2:X2}h)", devDevice, byChannel, handle)
        End If
    End Function

    ''' <summary>
    ''' Help Function used to get an error as text
    ''' </summary>
    ''' <param name="error">Error code to be translated</param>
    ''' <returns>A text with the translated error</returns>
    Private Function GetFormattedError(ByVal [error] As TPCANStatus) As String
        Dim strTemp As StringBuilder

        ' Creates a buffer big enough for a error-text
        '
        strTemp = New StringBuilder(256)
        ' Gets the text using the GetErrorText API function
        ' If the function success, the translated error is returned. If it fails,
        ' a text describing the current error is returned.
        '
        If PCANBasic.GetErrorText([error], &H9, strTemp) <> TPCANStatus.PCAN_ERROR_OK Then
            Return String.Format("An error occurred. Error-code's text ({0:X}) couldn't be retrieved", [error])
        Else
            Return strTemp.ToString()
        End If
    End Function

    ''' <summary>
    ''' Convert bitrate c_short value to readable string
    ''' </summary>
    ''' <param name="bitrate">Bitrate to be converted</param>
    ''' <returns>A text with the converted bitrate</returns>
    Private Function ConvertBitrateToString(ByVal bitrate As TPCANBaudrate) As String
        Select Case bitrate
            Case TPCANBaudrate.PCAN_BAUD_1M
                Return "1 MBit/sec"
            Case TPCANBaudrate.PCAN_BAUD_800K
                Return "800 kBit/sec"
            Case TPCANBaudrate.PCAN_BAUD_500K
                Return "500 kBit/sec"
            Case TPCANBaudrate.PCAN_BAUD_250K
                Return "250 kBit/sec"
            Case TPCANBaudrate.PCAN_BAUD_125K
                Return "125 kBit/sec"
            Case TPCANBaudrate.PCAN_BAUD_100K
                Return "100 kBit/sec"
            Case TPCANBaudrate.PCAN_BAUD_95K
                Return "95,238 kBit/sec"
            Case TPCANBaudrate.PCAN_BAUD_83K
                Return "83,333 kBit/sec"
            Case TPCANBaudrate.PCAN_BAUD_50K
                Return "50 kBit/sec"
            Case TPCANBaudrate.PCAN_BAUD_47K
                Return "47,619 kBit/sec"
            Case TPCANBaudrate.PCAN_BAUD_33K
                Return "33,333 kBit/sec"
            Case TPCANBaudrate.PCAN_BAUD_20K
                Return "20 kBit/sec"
            Case TPCANBaudrate.PCAN_BAUD_10K
                Return "10 kBit/sec"
            Case TPCANBaudrate.PCAN_BAUD_5K
                Return "5 kBit/sec"
            Case Else
                Return "Unknown Bitrate"
        End Select
    End Function

    ''' <summary>
    ''' Convert uint value to readable string value
    ''' </summary>
    ''' <param name="value">A value representing a PCAN-Channel handle</param>
    ''' <returns>A text representing a PCAN-Channel handle</returns>
    Private Function ConvertToChannelHandle(ByVal value As UInteger) As String
        Select Case value
            Case PCANBasic.PCAN_USBBUS1
                Return "PCAN_USBBUS1"
            Case PCANBasic.PCAN_USBBUS2
                Return "PCAN_USBBUS2"
            Case PCANBasic.PCAN_USBBUS3
                Return "PCAN_USBBUS3"
            Case PCANBasic.PCAN_USBBUS4
                Return "PCAN_USBBUS4"
            Case PCANBasic.PCAN_USBBUS5
                Return "PCAN_USBBUS5"
            Case PCANBasic.PCAN_USBBUS6
                Return "PCAN_USBBUS6"
            Case PCANBasic.PCAN_USBBUS7
                Return "PCAN_USBBUS7"
            Case PCANBasic.PCAN_USBBUS8
                Return "PCAN_USBBUS8"
            Case PCANBasic.PCAN_USBBUS9
                Return "PCAN_USBBUS9"
            Case PCANBasic.PCAN_USBBUS10
                Return "PCAN_USBBUS10"
            Case PCANBasic.PCAN_USBBUS11
                Return "PCAN_USBBUS11"
            Case PCANBasic.PCAN_USBBUS12
                Return "PCAN_USBBUS12"
            Case PCANBasic.PCAN_USBBUS13
                Return "PCAN_USBBUS13"
            Case PCANBasic.PCAN_USBBUS14
                Return "PCAN_USBBUS14"
            Case PCANBasic.PCAN_USBBUS15
                Return "PCAN_USBBUS15"
            Case PCANBasic.PCAN_USBBUS16
                Return "PCAN_USBBUS16"

            Case PCANBasic.PCAN_ISABUS1
                Return "PCAN_ISABUS1"
            Case PCANBasic.PCAN_ISABUS2
                Return "PCAN_ISABUS2"
            Case PCANBasic.PCAN_ISABUS3
                Return "PCAN_ISABUS3"
            Case PCANBasic.PCAN_ISABUS4
                Return "PCAN_ISABUS4"
            Case PCANBasic.PCAN_ISABUS5
                Return "PCAN_ISABUS5"
            Case PCANBasic.PCAN_ISABUS6
                Return "PCAN_ISABUS6"
            Case PCANBasic.PCAN_ISABUS7
                Return "PCAN_ISABUS7"
            Case PCANBasic.PCAN_ISABUS8
                Return "PCAN_ISABUS8"

            Case PCANBasic.PCAN_LANBUS1
                Return "PCAN_LANBUS1"
            Case PCANBasic.PCAN_LANBUS2
                Return "PCAN_LANBUS2"
            Case PCANBasic.PCAN_LANBUS3
                Return "PCAN_LANBUS3"
            Case PCANBasic.PCAN_LANBUS4
                Return "PCAN_LANBUS4"
            Case PCANBasic.PCAN_LANBUS5
                Return "PCAN_LANBUS5"
            Case PCANBasic.PCAN_LANBUS6
                Return "PCAN_LANBUS6"
            Case PCANBasic.PCAN_LANBUS7
                Return "PCAN_LANBUS7"
            Case PCANBasic.PCAN_LANBUS8
                Return "PCAN_LANBUS8"
            Case PCANBasic.PCAN_LANBUS9
                Return "PCAN_LANBUS9"
            Case PCANBasic.PCAN_LANBUS10
                Return "PCAN_LANBUS10"
            Case PCANBasic.PCAN_LANBUS11
                Return "PCAN_LANBUS11"
            Case PCANBasic.PCAN_LANBUS12
                Return "PCAN_LANBUS12"
            Case PCANBasic.PCAN_LANBUS13
                Return "PCAN_LANBUS13"
            Case PCANBasic.PCAN_LANBUS14
                Return "PCAN_LANBUS14"
            Case PCANBasic.PCAN_LANBUS15
                Return "PCAN_LANBUS15"
            Case PCANBasic.PCAN_LANBUS16
                Return "PCAN_LANBUS16"

            Case PCANBasic.PCAN_PCCBUS1
                Return "PCAN_PCCBUS1"
            Case PCANBasic.PCAN_PCCBUS2
                Return "PCAN_PCCBUS2"

            Case PCANBasic.PCAN_PCIBUS1
                Return "PCAN_PCIBUS1"
            Case PCANBasic.PCAN_PCIBUS2
                Return "PCAN_PCIBUS2"
            Case PCANBasic.PCAN_PCIBUS3
                Return "PCAN_PCIBUS3"
            Case PCANBasic.PCAN_PCIBUS4
                Return "PCAN_PCIBUS4"
            Case PCANBasic.PCAN_PCIBUS5
                Return "PCAN_PCIBUS5"
            Case PCANBasic.PCAN_PCIBUS6
                Return "PCAN_PCIBUS6"
            Case PCANBasic.PCAN_PCIBUS7
                Return "PCAN_PCIBUS7"
            Case PCANBasic.PCAN_PCIBUS8
                Return "PCAN_PCIBUS8"
            Case PCANBasic.PCAN_PCIBUS9
                Return "PCAN_PCIBUS9"
            Case PCANBasic.PCAN_PCIBUS10
                Return "PCAN_PCIBUS10"
            Case PCANBasic.PCAN_PCIBUS11
                Return "PCAN_PCIBUS11"
            Case PCANBasic.PCAN_PCIBUS12
                Return "PCAN_PCIBUS12"
            Case PCANBasic.PCAN_PCIBUS13
                Return "PCAN_PCIBUS13"
            Case PCANBasic.PCAN_PCIBUS14
                Return "PCAN_PCIBUS14"
            Case PCANBasic.PCAN_PCIBUS15
                Return "PCAN_PCIBUS15"
            Case PCANBasic.PCAN_PCIBUS16
                Return "PCAN_PCIBUS16"
            Case Else
                Return "Handle unknown: " + value
        End Select
    End Function

    Private Function ConvertDeviceTypeToString(ByVal devicetype As TPCANDevice) As String
        Select Case devicetype
            Case TPCANDevice.PCAN_NONE
                Return "PCAN_NONE"
            Case TPCANDevice.PCAN_PEAKCAN
                Return "PCAN_PEAKCAN"
            Case TPCANDevice.PCAN_ISA
                Return "PCAN_ISA"
            Case TPCANDevice.PCAN_DNG
                Return "PCAN_DNG"
            Case TPCANDevice.PCAN_PCI
                Return "PCAN_PCI"
            Case TPCANDevice.PCAN_USB
                Return "PCAN_USB"
            Case TPCANDevice.PCAN_PCC
                Return "PCAN_PCC"
            Case TPCANDevice.PCAN_VIRTUAL
                Return "PCAN_VIRTUAL"
            Case TPCANDevice.PCAN_LAN
                Return "PCAN_LAN"
            Case Else
                Return ""
        End Select
    End Function

    ''' <summary>
    ''' Convert uint value to readable string value
    ''' </summary>
    ''' <returns></returns>
    Private Function ConvertToParameterOnOff(ByVal value As UInteger) As String
        Select Case value
            Case PCANBasic.PCAN_PARAMETER_OFF
                Return "PCAN_PARAMETER_OFF"
            Case PCANBasic.PCAN_PARAMETER_ON
                Return "PCAN_PARAMETER_ON"
            Case Else
                Return "Status unknown: " + value.ToString()
        End Select
    End Function

    ''' <summary>
    ''' Convert uint value to readable string value
    ''' </summary>
    ''' <param name="value">A value representing a communication direction</param>
    ''' <returns>A text representing a LAN channel direction</returns>
    Private Function ConvertToChannelDirection(ByVal value As Integer) As String
        Select Case value
            Case PCANBasic.LAN_DIRECTION_READ
                Return "incoming only"
            Case PCANBasic.LAN_DIRECTION_WRITE
                Return "outgoing only"
            Case PCANBasic.LAN_DIRECTION_READ_WRITE
                Return "bidirectional"
            Case Else
                Return String.Format("undefined (0x{0:X4})", value)
        End Select
    End Function

    ''' <summary>
    ''' Convert uint value to readable string value
    ''' </summary>
    ''' <param name="value">A value representing channel features</param>
    ''' <returns>A text representing channel features</returns>
    Private Function ConvertToChannelFeatures(ByVal value As UInteger) As String
        Dim sFeatures As String
        sFeatures = ""
        If (value And PCANBasic.FEATURE_FD_CAPABLE) = PCANBasic.FEATURE_FD_CAPABLE Then
            sFeatures += "FEATURE_FD_CAPABLE"
        End If
        If (value And PCANBasic.FEATURE_DELAY_CAPABLE) = PCANBasic.FEATURE_DELAY_CAPABLE Then
            If sFeatures <> "" Then
                sFeatures += ", FEATURE_DELAY_CAPABLE"
            Else
                sFeatures += "FEATURE_DELAY_CAPABLE"
            End If
        End If
        If (value And PCANBasic.FEATURE_IO_CAPABLE) = PCANBasic.FEATURE_IO_CAPABLE Then
            If sFeatures <> "" Then
                sFeatures += ", FEATURE_IO_CAPABLE"
            Else
                sFeatures += "FEATURE_IO_CAPABLE"
            End If
        End If
        Return sFeatures
    End Function

    ''' <summary>
    ''' Convert uint value to readable string value
    ''' </summary>
    ''' <param name="value">A value representing a channel condition</param>
    ''' <returns>A text representing a channel condition</returns>
    Private Function ConvertToChannelCondition(ByVal value As UInteger) As String
        Select Case value
            Case PCANBasic.PCAN_CHANNEL_UNAVAILABLE
                Return "PCAN_CHANNEL_UNAVAILABLE"
            Case PCANBasic.PCAN_CHANNEL_AVAILABLE
                Return "PCAN_CHANNEL_AVAILABLE"
            Case PCANBasic.PCAN_CHANNEL_OCCUPIED
                Return "PCAN_CHANNEL_OCCUPIED"
            Case PCANBasic.PCAN_CHANNEL_PCANVIEW
                Return "PCAN_CHANNEL_PCANVIEW"
            Case Else
                Return "Status unknow: " + value
        End Select
    End Function

    ''' <summary>
    ''' Convert uint value to readable string value
    ''' </summary>
    ''' <param name="value">A value representing filter status</param>
    ''' <returns>A text representing a filter status</returns>
    Private Function ConvertToFilterOpenCloseCustom(ByVal value As UInteger) As String
        Select Case value
            Case PCANBasic.PCAN_FILTER_CLOSE
                Return "PCAN_FILTER_CLOSE"
            Case PCANBasic.PCAN_FILTER_OPEN
                Return "PCAN_FILTER_OPEN"
            Case PCANBasic.PCAN_FILTER_CUSTOM
                Return "PCAN_FILTER_CUSTOM"
            Case Else
                Return "Status unknown: " + value
        End Select
    End Function
#End Region
End Class

Module Start
    Sub Main()
        Dim p = New GetSetParameter()
        p.Main()
    End Sub
End Module
