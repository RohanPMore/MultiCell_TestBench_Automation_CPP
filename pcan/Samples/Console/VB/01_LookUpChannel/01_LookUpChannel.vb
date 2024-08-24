' Inclusion of PEAK PCAN-Basic namespace
'
Imports LookUpChannel.Peak.Can.Basic
Imports TPCANHandle = System.UInt16
Imports System.Text

Class LookUpChannel

#Region "Defines"
    ''' <summary>
    ''' Sets a TPCANDevice value. The input can be numeric, in hexadecimal or decimal format, or as string denoting 
    ''' a TPCANDevice value name.
    ''' </summary>
    Private Const DeviceType = "PCAN_USB"
    ''' <summary>
    ''' Sets value in range of a double. The input can be hexadecimal or decimal format.
    ''' </summary>
    Private Const DeviceID = ""
    ''' <summary>
    ''' Sets a zero-based index value in range of a double. The input can be hexadecimal or decimal format.
    ''' </summary>
    Private Const ControllerNumber = ""
    ''' <summary>
    ''' Sets a valid Internet Protocol address 
    ''' </summary>
    Private Const IPAddress = ""
#End Region

#Region "Members"
    ''' <summary>
    ''' Shows if DLL was found
    ''' </summary>
    Private m_DLLFound As Boolean
#End Region

    Sub Main()
        ShowConfigurationHelp() '' Shows information about this sample
        ShowCurrentConfiguration() '' Shows the current parameters configuration

        '' Checks if PCANBasic.dll Is available, if Not, the program terminates
        m_DLLFound = CheckForLibrary()
        If (Not m_DLLFound) Then
            Return
        End If
        Console.WriteLine("Press any key to start searching")
        Console.ReadKey()
        Console.WriteLine()

        Dim sParameters As String = ""
        If (DeviceType <> "") Then
            sParameters += PCANBasic.LOOKUP_DEVICE_TYPE + "=" + DeviceType
        End If
        If (DeviceID <> "") Then
            If (sParameters <> "") Then
                sParameters += ", "
            End If
            sParameters += PCANBasic.LOOKUP_DEVICE_ID + "=" + DeviceID
        End If
        If (ControllerNumber <> "") Then
            If (sParameters <> "") Then
                sParameters += ", "
            End If
            sParameters += PCANBasic.LOOKUP_CONTROLLER_NUMBER + "=" + ControllerNumber
        End If
        If (IPAddress <> "") Then
            If (sParameters <> "") Then
                sParameters += ", "
            End If
            sParameters += PCANBasic.LOOKUP_IP_ADDRESS + "=" + IPAddress
        End If

        Dim stsResult As TPCANStatus
        Dim handle As TPCANHandle
        stsResult = PCANBasic.LookUpChannel(sParameters, handle)

        If stsResult = TPCANStatus.PCAN_ERROR_OK Then
            If handle <> PCANBasic.PCAN_NONEBUS Then

                Dim iFeatures As UInteger
                stsResult = PCANBasic.GetValue(handle, TPCANParameter.PCAN_CHANNEL_FEATURES, iFeatures, CType(System.Runtime.InteropServices.Marshal.SizeOf(iFeatures), UInteger))

                If stsResult = TPCANStatus.PCAN_ERROR_OK Then
                    Console.WriteLine("The channel handle " + FormatChannelName(handle, (iFeatures And PCANBasic.FEATURE_FD_CAPABLE) = PCANBasic.FEATURE_FD_CAPABLE) + " was found")
                Else
                    Console.WriteLine("There was an issue retrieveing supported channel features")
                End If
            Else
                Console.WriteLine("A handle for these lookup-criteria was not found")
            End If
        End If

        If stsResult <> TPCANStatus.PCAN_ERROR_OK Then
            Console.WriteLine("There was an error looking up the device, are any hardware channels attached?")
            ShowStatus(stsResult)
        End If

        Console.WriteLine()
        Console.WriteLine("Press any key to close")
        Console.ReadKey()
    End Sub

    Protected Overrides Sub Finalize()
        If m_DLLFound Then
            PCANBasic.Uninitialize(PCANBasic.PCAN_NONEBUS)
        End If
    End Sub

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
        Console.WriteLine("|                        PCAN-Basic LookUpChannel Example                                |")
        Console.WriteLine("=========================================================================================")
        Console.WriteLine("Following parameters are to be adjusted before launching, according to the hardware used |")
        Console.WriteLine("                                                                                         |")
        Console.WriteLine("* DeviceType: Numeric value that represents a TPCANDevice                                |")
        Console.WriteLine("* DeviceID: Numeric value that represents the device identifier                          |")
        Console.WriteLine("* ControllerNumber: Numeric value that represents controller number                      |")
        Console.WriteLine("* IPAddress: String value that represents a valid Internet Protocol address              |")
        Console.WriteLine("                                                                                         |")
        Console.WriteLine("For more information see 'LookUp Parameter Definition' within the documentation          |")
        Console.WriteLine("=========================================================================================")
        Console.WriteLine("")
    End Sub

    ''' <summary>
    ''' Shows/prints the configured paramters
    ''' </summary>
    Private Sub ShowCurrentConfiguration()
        Console.WriteLine("Parameter values used")
        Console.WriteLine("----------------------")
        Console.WriteLine("* DeviceType: " + DeviceType)
        Console.WriteLine("* DeviceID: " + DeviceID)
        Console.WriteLine("* ControllerNumber: " + ControllerNumber)
        Console.WriteLine("* IPAddress: " + IPAddress)
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
#End Region

End Class

Module Start
    Sub Main()
        Dim p = New LookUpChannel()
        p.Main()
    End Sub
End Module