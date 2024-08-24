' Inclusion of PEAK PCAN-Basic namespace
'
Imports TraceFiles.Peak.Can.Basic
Imports TPCANHandle = System.UInt16
Imports TPCANTimestampFD = System.UInt64
Imports System.Text
Imports System.Timers

Class TraceFiles

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
    ''' <summary>
    ''' Sets if trace continue after reaching maximum size for the first file
    ''' </summary>
    Private Const TraceFileSingle = True
    ''' <summary>
    ''' Set if date will be add to filename 
    ''' </summary>
    Private Const TraceFileDate = True
    ''' <summary>
    ''' Set if time will be add to filename
    ''' </summary>
    Private Const TraceFileTime = True
    ''' <summary>
    ''' Set if existing tracefile overwrites when a New trace session Is started
    ''' </summary>
    Private Const TraceFileOverwrite = False
    ''' <summary>
    ''' Set if the column "Data Length" should be used instead of the column "Data Length Code"
    ''' </summary>
    Private Const TraceFileDataLength = False
    ''' <summary>
    ''' Sets the size (megabyte) of an tracefile 
    ''' Example - 100 = 100 megabyte
    ''' Range between 1 And 100
    ''' </summary>
    Private Const TraceFileSize = 2
    ''' <summary>
    ''' Sets a fully-qualified And valid path to an existing directory. In order to use the default path 
    ''' (calling process path) an empty string must be set.
    ''' </summary>
    Private Const TracePath = ""
    ''' <summary>
    ''' Timerinterval (ms) for reading 
    ''' </summary>
    Private Const TimerInterval = 250
#End Region

#Region "Members"
    ''' <summary>
    ''' Shows if DLL was found
    ''' </summary>
    Private m_DLLFound As Boolean
    ''' <summary>
    ''' Used for writing
    ''' </summary>
    Private m_Timer As Timer
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

        ' Trace messages...
        Console.WriteLine("Successfully initialized.")
        Console.WriteLine("Press any key to start tracing...")
        Console.ReadKey()
        If ConfigureTrace() Then
            If StartTrace() Then
                SetTimer()
                Console.WriteLine("Messages are being traced.")
                Console.WriteLine("Press any key to stop trace and quit")
                Console.ReadKey()
                StopTrace()
                Return
            End If
        End If

        Console.WriteLine()
        Console.WriteLine("Press any key to close")
        Console.Read()
    End Sub

    Protected Overrides Sub Finalize()
        If m_DLLFound Then
            PCANBasic.Uninitialize(PCANBasic.PCAN_NONEBUS)
        End If
    End Sub

#Region "Main-Functions"
    ''' <summary>
    ''' Set timer
    ''' </summary>
    Private Sub SetTimer()
        m_Timer = New System.Timers.Timer(TimerInterval)
        AddHandler m_Timer.Elapsed, AddressOf OnTimedEvent '' Hook up the Elapsed Event For the timer. 
        m_Timer.AutoReset = True
        m_Timer.Enabled = True
    End Sub

    ''' <summary>
    ''' Handles the "time elapsed" event
    ''' </summary>
    ''' <param name="source">The source of the event.</param>
    ''' <param name="e">An <seealso cref="ElapsedEventArgs"/> object that contains the event data</param>
    Private Sub OnTimedEvent(ByVal source As Object, ByVal e As ElapsedEventArgs)
        ReadMessages()
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
                Dim CANMsgFD As TPCANMsgFD = New TPCANMsgFD()
                stsResult = PCANBasic.ReadFD(PcanHandle, CANMsgFD)
            Else
                Dim CANMsg As TPCANMsg = New TPCANMsg()
                stsResult = PCANBasic.Read(PcanHandle, CANMsg)
            End If
            If stsResult <> TPCANStatus.PCAN_ERROR_OK And stsResult <> TPCANStatus.PCAN_ERROR_QRCVEMPTY Then
                ShowStatus(stsResult)
                Return
            End If
        Loop While Not Convert.ToBoolean(stsResult And TPCANStatus.PCAN_ERROR_QRCVEMPTY)
    End Sub

    ''' <summary>
    ''' Deactivates the tracing process
    ''' </summary>
    Private Sub StopTrace()
        Dim iStatus As UInteger
        iStatus = PCANBasic.PCAN_PARAMETER_OFF

        Dim stsResult As TPCANStatus
        stsResult = PCANBasic.SetValue(PcanHandle, TPCANParameter.PCAN_TRACE_STATUS, iStatus, CType(System.Runtime.InteropServices.Marshal.SizeOf(iStatus), UInteger)) '' We stop the tracing by setting the parameter.

        If stsResult <> TPCANStatus.PCAN_ERROR_OK Then
            ShowStatus(stsResult)
        End If
    End Sub

    ''' <summary>
    ''' Configures the way how trace files are formatted
    ''' </summary>
    ''' <returns>Returns true if no error occurr</returns>
    Private Function ConfigureTrace() As Boolean
        Dim iSize As UInteger
        iSize = TraceFileSize

        '' Sets path to store files
        Dim stsResult As TPCANStatus
        stsResult = PCANBasic.SetValue(PcanHandle, TPCANParameter.PCAN_TRACE_LOCATION, TracePath, CType(System.Runtime.InteropServices.Marshal.SizeOf(Of UInteger), UInteger))

        If stsResult = TPCANStatus.PCAN_ERROR_OK Then
            ''Sets the maximum size of a tracefile 
            stsResult = PCANBasic.SetValue(PcanHandle, TPCANParameter.PCAN_TRACE_SIZE, iSize, CType(System.Runtime.InteropServices.Marshal.SizeOf(iSize), UInteger))

            If stsResult = TPCANStatus.PCAN_ERROR_OK Then
                Dim config As UInteger
                If TraceFileSingle Then
                    config = PCANBasic.TRACE_FILE_SINGLE '' Creats one file 
                Else
                    config = PCANBasic.TRACE_FILE_SEGMENTED '' Creats more files
                End If

                '' Activate overwriting existing tracefile
                If TraceFileOverwrite Then
                    config = config Or PCANBasic.TRACE_FILE_OVERWRITE
                End If
                '' Uses Data Length instead of Data Length Code
                If TraceFileDataLength Then
                    config = config Or PCANBasic.TRACE_FILE_DATA_LENGTH
                End If
                '' Adds date to tracefilename
                If TraceFileDate Then
                    config = config Or PCANBasic.TRACE_FILE_DATE
                End If
                '' Adds time to tracefilename
                If TraceFileTime Then
                    config = config Or PCANBasic.TRACE_FILE_TIME
                End If

                '' Sets the config
                stsResult = PCANBasic.SetValue(PcanHandle, TPCANParameter.PCAN_TRACE_CONFIGURE, config, CType(System.Runtime.InteropServices.Marshal.SizeOf(config), UInteger))
                If stsResult = TPCANStatus.PCAN_ERROR_OK Then
                    Return True
                End If
            End If
        End If
        ShowStatus(stsResult)
        Return False
    End Function

    ''' <summary>
    ''' Activates the tracing process
    ''' </summary>
    ''' <returns>Returns true if no error occurr</returns>
    Private Function StartTrace() As Boolean
        Dim iStatus As UInteger
        iStatus = PCANBasic.PCAN_PARAMETER_ON

        Dim stsResult As TPCANStatus
        stsResult = PCANBasic.SetValue(PcanHandle, TPCANParameter.PCAN_TRACE_STATUS, iStatus, CType(System.Runtime.InteropServices.Marshal.SizeOf(iStatus), UInteger)) '' We stop the tracing by setting the parameter.

        If stsResult <> TPCANStatus.PCAN_ERROR_OK Then
            ShowStatus(stsResult)
            Return False
        End If
        Return True
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
        Console.WriteLine("|                           PCAN-Basic TraceFiles Example                                |")
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
        Console.WriteLine("* TraceFileSingle: Boolean value that indicates if tracing ends after one file (true) or |")
        Console.WriteLine("                   continues                                                             |")
        Console.WriteLine("* TraceFileDate: Boolean value that indicates if the date will be added to filename      |")
        Console.WriteLine("* TraceFileTime: Boolean value that indicates if the time will be added to filename      |")
        Console.WriteLine("* TraceFileOverwrite: Boolean value that indicates if existing tracefiles should be      |")
        Console.WriteLine("                      overwritten                                                        |")
        Console.WriteLine("* TraceFileDataLength: Boolean value that indicates if the column 'Data Length' is used  |")
        Console.WriteLine("                       instead of the column 'Data Length Code'                          |")
        Console.WriteLine("* TraceFileSize: Numeric value that represents the size of a tracefile in meagabytes     |")
        Console.WriteLine("* TracePath: string value that represents a valid path to an existing directory          |")
        Console.WriteLine("* TimerInterval: The time, in milliseconds, to wait before trying to write a message     |")
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
        Console.WriteLine("* IsFD: " + IsFD.ToString())
        Console.WriteLine("* Bitrate: " + ConvertBitrateToString(Bitrate))
        Console.WriteLine("* BitrateFD: " + BitrateFD)
        Console.WriteLine("* TraceFileSingle: " + TraceFileSingle.ToString())
        Console.WriteLine("* TraceFileDate: " + TraceFileDate.ToString())
        Console.WriteLine("* TraceFileTime: " + TraceFileTime.ToString())
        Console.WriteLine("* TraceFileOverwrite: " + TraceFileOverwrite.ToString())
        Console.WriteLine("* TraceFileDataLength: " + TraceFileDataLength.ToString())
        Console.WriteLine("* TraceFileSize: " + TraceFileSize.ToString() + " MB")
        If (TracePath = "") Then
            Console.WriteLine("* TracePath: (calling application path)")
        Else
            Console.WriteLine("* TracePath: " + TracePath)
        End If
        Console.WriteLine("* TimerInterval: " + TimerInterval.ToString())
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
        Dim p = New TraceFiles()
        p.Main()
    End Sub
End Module