' Inclusion of PEAK PCAN-Basic namespace
'
Imports EventDrivenRead.Peak.Can.Basic
Imports TPCANHandle = System.UInt16
Imports TPCANTimestampFD = System.UInt64
Imports System.Text
Imports System.Threading

Class EventDrivenRead

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
    ''' <summary>
    ''' Thread for reading messages
    ''' </summary>
    Private m_ReadThread As Thread
    ''' <summary>
    ''' Shows if thread run
    ''' </summary>
    Private m_ThreadRun As Boolean
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

        ' Reading messages...
        Console.WriteLine("Successfully initialized.")
        m_ReadThread = New Thread(New ThreadStart(AddressOf Me.ThreadExecute))
        m_ThreadRun = True
        m_ReadThread.Start()
        Console.WriteLine("Started reading messages...")
        Console.WriteLine("")
        Console.WriteLine("Press any key to close")
        Console.ReadKey()
        m_ThreadRun = False
        m_ReadThread.Join()
    End Sub

    Protected Overrides Sub Finalize()
        If m_DLLFound Then
            PCANBasic.Uninitialize(PCANBasic.PCAN_NONEBUS)
        End If
    End Sub

#Region "Main-Functions"
    Private Sub ThreadExecute()
        '' Sets the handle of the Receive-Event.
        Dim evtReceiveEvent As AutoResetEvent
        evtReceiveEvent = New AutoResetEvent(False)
        Dim iBuffer As UInt32
        iBuffer = Convert.ToUInt32(evtReceiveEvent.SafeWaitHandle.DangerousGetHandle().ToInt32())

        Dim stsResult As TPCANStatus
        stsResult = PCANBasic.SetValue(PcanHandle, TPCANParameter.PCAN_RECEIVE_EVENT, iBuffer, CType(System.Runtime.InteropServices.Marshal.SizeOf(iBuffer), UInteger))

        If stsResult <> TPCANStatus.PCAN_ERROR_OK Then
            ShowStatus(stsResult)
            Return
        End If

        While m_ThreadRun
            '' Checks for messages when an event Is received
            If evtReceiveEvent.WaitOne(50) Then
                ReadMessages()
            End If
        End While
        '' Removes the Receive-Event again.
        iBuffer = 0
        stsResult = PCANBasic.SetValue(PcanHandle, TPCANParameter.PCAN_RECEIVE_EVENT, iBuffer, CType(System.Runtime.InteropServices.Marshal.SizeOf(iBuffer), UInteger))

        If stsResult <> TPCANStatus.PCAN_ERROR_OK Then
            ShowStatus(stsResult)
        End If
        evtReceiveEvent.Dispose()
    End Sub
    ''' <summary>
    ''' Function for reading PCAN-Basic messages
    ''' </summary>
    Private Sub ReadMessages()
        Dim stsResult As TPCANStatus

        ' We read at least one time the queue looking for messages. If a message is found, we look again trying to 
        ' find more. If the queue is empty or an error occurr, we get out from the dowhile statement.
        Do
            If IsFD Then
                stsResult = ReadMessageFD()
            Else
                stsResult = ReadMessage()
            End If
            If stsResult <> TPCANStatus.PCAN_ERROR_OK And stsResult <> TPCANStatus.PCAN_ERROR_QRCVEMPTY Then
                ShowStatus(stsResult)
                Return
            End If
        Loop While Not Convert.ToBoolean(stsResult And TPCANStatus.PCAN_ERROR_QRCVEMPTY)
    End Sub

    ''' <summary>
    ''' Function for reading messages on CAN-FD devices
    ''' </summary>
    ''' <returns>A TPCANStatus error code</returns>
    Private Function ReadMessageFD() As TPCANStatus
        Dim CANMsg As TPCANMsgFD = Nothing
        Dim CANTimeStamp As TPCANTimestampFD
        Dim stsResult As TPCANStatus

        ' We execute the "Read" function of the PCANBasic 
        '
        stsResult = PCANBasic.ReadFD(PcanHandle, CANMsg, CANTimeStamp)
        If (stsResult <> TPCANStatus.PCAN_ERROR_QRCVEMPTY) Then
            ' We process the received message
            '
            ProcessMessageCanFD(CANMsg, CANTimeStamp)
        End If

        Return stsResult
    End Function

    ''' <summary>
    ''' Function for reading CAN messages on normal CAN devices
    ''' </summary>
    ''' <returns>A TPCANStatus error code</returns>
    Private Function ReadMessage() As TPCANStatus
        Dim CANMsg As TPCANMsg = Nothing
        Dim CANTimeStamp As TPCANTimestamp
        Dim stsResult As TPCANStatus

        ' We execute the "Read" function of the PCANBasic 
        '
        stsResult = PCANBasic.Read(PcanHandle, CANMsg, CANTimeStamp)
        If (stsResult <> TPCANStatus.PCAN_ERROR_QRCVEMPTY) Then
            ' We process the received message
            '
            ProcessMessageCan(CANMsg, CANTimeStamp)
        End If

        Return stsResult
    End Function

    ''' <summary>
    ''' Processes a received CAN message
    ''' </summary>
    ''' <param name="msg">The received PCAN-Basic CAN message</param>
    ''' <param name="itsTimeStamp">Timestamp of the message as TPCANTimestamp structure</param>
    Private Sub ProcessMessageCan(ByVal msg As TPCANMsg, ByVal itsTimeStamp As TPCANTimestamp)
        Console.WriteLine("Type: " + GetMsgTypeString(msg.MSGTYPE))
        Console.WriteLine("ID: " + GetIdString(msg.ID, msg.MSGTYPE))
        Console.WriteLine("Length: " + msg.LEN.ToString())
        Console.WriteLine("Time: " + GetTimeString(itsTimeStamp.micros + (1000UL * itsTimeStamp.millis) + (&H100_000_000UL * 1000UL * itsTimeStamp.millis_overflow)))
        Console.WriteLine("Data: " + GetDataString(msg.DATA, msg.MSGTYPE, msg.LEN))
        Console.WriteLine("----------------------------------------------------------")
    End Sub

    ''' <summary>
    ''' Processes a received CAN-FD message
    ''' </summary>
    ''' <param name="msg">The received PCAN-Basic CAN-FD message</param>
    ''' <param name="itsTimeStamp">Timestamp of the message as microseconds (ulong)</param>
    Private Sub ProcessMessageCanFD(ByVal msg As TPCANMsgFD, ByVal itsTimeStamp As TPCANTimestampFD)
        Console.WriteLine("Type: " + GetMsgTypeString(msg.MSGTYPE))
        Console.WriteLine("ID: " + GetIdString(msg.ID, msg.MSGTYPE))
        Console.WriteLine("Length: " + GetLengthFromDLC(msg.DLC).ToString())
        Console.WriteLine("Time: " + GetTimeString(itsTimeStamp))
        Console.WriteLine("Data: " + GetDataString(msg.DATA, msg.MSGTYPE, GetLengthFromDLC(msg.DLC)))
        Console.WriteLine("----------------------------------------------------------")
    End Sub
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
        Console.WriteLine("|                        PCAN-Basic EventDrivenRead Example                              |")
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

    ''' <summary>
    ''' Gets the string representation of the type of a CAN message
    ''' </summary>
    ''' <param name="msgType">Type of a CAN message</param>
    ''' <returns>The type of the CAN message as string</returns>
    Private Function GetMsgTypeString(ByVal msgType As TPCANMessageType) As String
        Dim strTemp As String

        If (msgType And TPCANMessageType.PCAN_MESSAGE_STATUS) = TPCANMessageType.PCAN_MESSAGE_STATUS Then
            Return "STATUS"
        End If

        If (msgType And TPCANMessageType.PCAN_MESSAGE_ERRFRAME) = TPCANMessageType.PCAN_MESSAGE_ERRFRAME Then
            Return "ERROR"
        End If

        If (msgType And TPCANMessageType.PCAN_MESSAGE_EXTENDED) = TPCANMessageType.PCAN_MESSAGE_EXTENDED Then
            strTemp = "EXT"
        Else
            strTemp = "STD"
        End If

        If (msgType And TPCANMessageType.PCAN_MESSAGE_RTR) = TPCANMessageType.PCAN_MESSAGE_RTR Then
            strTemp += "/RTR"
        Else
            If (msgType > TPCANMessageType.PCAN_MESSAGE_EXTENDED) Then
                strTemp += " [ "
                If ((msgType And TPCANMessageType.PCAN_MESSAGE_FD) = TPCANMessageType.PCAN_MESSAGE_FD) Then
                    strTemp += " FD"
                End If
                If ((msgType And TPCANMessageType.PCAN_MESSAGE_BRS) = TPCANMessageType.PCAN_MESSAGE_BRS) Then
                    strTemp += " BRS"
                End If
                If ((msgType And TPCANMessageType.PCAN_MESSAGE_ESI) = TPCANMessageType.PCAN_MESSAGE_ESI) Then
                    strTemp += " ESI"
                End If
                strTemp += " ]"
            End If
        End If

        Return strTemp
    End Function

    ''' <summary>
    ''' Gets the string representation of the ID of a CAN message
    ''' </summary>
    ''' <param name="id">Id to be parsed</param>
    ''' <param name="msgType">Type flags of the message the Id belong</param>
    ''' <returns>Hexadecimal representation of the ID of a CAN message</returns>
    Private Function GetIdString(ByVal id As UInteger, ByVal msgType As TPCANMessageType) As String
        If (msgType And TPCANMessageType.PCAN_MESSAGE_EXTENDED) = TPCANMessageType.PCAN_MESSAGE_EXTENDED Then
            Return String.Format("{0:X8}h", id)
        Else
            Return String.Format("{0:X3}h", id)
        End If
    End Function

    ''' <summary>
    ''' Gets the data length of a CAN message
    ''' </summary>
    ''' <param name="dlc">Data length code of a CAN message</param>
    ''' <returns>Data length as integer represented by the given DLC code</returns>
    Private Function GetLengthFromDLC(ByVal dlc As Byte) As Integer
        Select Case dlc
            Case 9
                Return 12
            Case 10
                Return 16
            Case 11
                Return 20
            Case 12
                Return 24
            Case 13
                Return 32
            Case 14
                Return 48
            Case 15
                Return 64
            Case Else
                Return dlc
        End Select
    End Function

    ''' <summary>
    ''' Gets the string representation of the timestamp of a CAN message, in milliseconds
    ''' </summary>
    ''' <returns></returns>
    Private Function GetTimeString(ByVal time As TPCANTimestampFD) As String
        Dim fTime As Double

        fTime = (time / 1000.0R)
        Return fTime.ToString("F1")
    End Function

    ''' <summary>
    ''' Gets the data of a CAN message as a string
    ''' </summary>
    ''' <param name="data">Array of bytes containing the data to parse</param>
    ''' <param name="msgType">Type flags of the message the data belong</param>
    ''' <param name="dataLength">The amount of bytes to take into account wihtin the given data</param>
    ''' <returns>A string with hexadecimal formatted data bytes of a CAN message</returns>
    Private Function GetDataString(ByVal data As Byte(), ByVal msgType As TPCANMessageType, ByVal dataLength As Integer) As String
        If (msgType And TPCANMessageType.PCAN_MESSAGE_RTR) = TPCANMessageType.PCAN_MESSAGE_RTR Then
            Return "Remote Request"
        Else
            Dim strTemp As String
            strTemp = ""
            For i As Integer = 0 To dataLength - 1
                strTemp += String.Format("{0:X2} ", data(i))
            Next
            Return strTemp
        End If
    End Function
#End Region
End Class

Module Start
    Sub Main()
        Dim p = New EventDrivenRead()
        p.Main()
    End Sub
End Module