' Inclusion of PEAK PCAN-Basic namespace
'
Imports ManualWrite.Peak.Can.Basic
Imports TPCANHandle = System.UInt16
Imports System.Text

Class ManualWrite

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
        ShowConfigurationHelp() '' Shows information about this sample 
        ShowCurrentConfiguration() '' Shows the current parameters configuration

        '' Checks if PCANBasic.dll is available, if not, the program terminates
        m_DLLFound = CheckForLibrary()
        If Not m_DLLFound Then
            Return
        End If

        Dim stsResult As TPCANStatus
        '' Initialization of the selected channel
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

        '' Writing messages...
        Console.WriteLine("Successfully initialized.")
        Console.WriteLine("Press any key to write")
        Console.ReadKey()
        Do
            Console.Clear()
            WriteMessages()
            Console.WriteLine("Do you want to write again? yes[y] or any other key to close")
        Loop While Console.ReadKey().Key = ConsoleKey.Y
    End Sub

    Protected Overrides Sub Finalize()
        If m_DLLFound Then
            PCANBasic.Uninitialize(PCANBasic.PCAN_NONEBUS)
        End If
    End Sub

#Region "Main-Functions"
    ''' <summary>
    ''' Function for writing PCAN-Basic messages
    ''' </summary>
    Private Sub WriteMessages()
        Dim stsResult As TPCANStatus

        If IsFD Then
            stsResult = WriteMessageFD()
        Else
            stsResult = WriteMessage()
        End If

        '' Checks if the message was sent
        If stsResult <> TPCANStatus.PCAN_ERROR_OK Then
            ShowStatus(stsResult)
        Else
            Console.WriteLine("Message was successfully SENT")
        End If
    End Sub

    ''' <summary>
    ''' Function for writing messages on CAN devices
    ''' </summary>
    ''' <returns>A TPCANStatus error code</returns>
    Private Function WriteMessage()
        '' Sends a CAN message with extended ID, and 8 data bytes
        Dim msgCanMessage As TPCANMsg
        msgCanMessage = New TPCANMsg
        msgCanMessage.DATA = CType(Array.CreateInstance(GetType(Byte), 8), Byte())
        msgCanMessage.ID = &H100
        msgCanMessage.LEN = Convert.ToByte(8)
        msgCanMessage.MSGTYPE = TPCANMessageType.PCAN_MESSAGE_EXTENDED
        For i As Byte = 0 To 7
            msgCanMessage.DATA(i) = i
        Next
        Return PCANBasic.Write(PcanHandle, msgCanMessage)
    End Function

    ''' <summary>
    ''' Function for writing messages on CAN-FD devices
    ''' </summary>
    ''' <returns>A TPCANStatus error code</returns>
    Private Function WriteMessageFD()
        '' Sends a CAN-FD message with standard ID, 64 data bytes, and bitrate switch
        Dim msgCanMessageFD As TPCANMsgFD
        msgCanMessageFD = New TPCANMsgFD
        msgCanMessageFD.DATA = CType(Array.CreateInstance(GetType(Byte), 64), Byte())
        msgCanMessageFD.ID = &H100
        msgCanMessageFD.DLC = 15
        msgCanMessageFD.MSGTYPE = TPCANMessageType.PCAN_MESSAGE_FD Or TPCANMessageType.PCAN_MESSAGE_BRS
        For i As Byte = 0 To 63
            msgCanMessageFD.DATA(i) = i
        Next
        Return PCANBasic.WriteFD(PcanHandle, msgCanMessageFD)
    End Function
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
        Console.WriteLine("|                           PCAN-Basic ManualWrite Example                               |")
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
    ''' Shows/prints the configured paramters
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
#End Region

End Class

Module Start
    Sub Main()
        Dim p = New ManualWrite()
        p.Main()
    End Sub
End Module

