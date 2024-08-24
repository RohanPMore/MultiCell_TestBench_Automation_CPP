unit TraceFiles;

{$mode objfpc}{$H+}

interface

uses
  Classes, Windows, SysUtils, CustApp, PCANBasic;

type

  { TReadThread }

  TThreadExecuteEvent = procedure() of Object;

  TReadThread = class(TThread)
  private
    FOnThreadExecute: TThreadExecuteEvent;
    procedure ThreadExecute;
  protected
    procedure Execute; override;
  public
    constructor Create(CreateSuspended : boolean);
    property OnThreadExecute: TThreadExecuteEvent read FOnThreadExecute write FOnThreadExecute;
  end;

type

  { TTraceFiles }

  TTraceFiles = class(TCustomApplication)
  private
    {Sets the PCANHandle (Hardware Channel)}
    FPcanHandle:TPCANHandle;
    {Sets the desired connection mode (CAN = false / CAN-FD = true)}
    FIsFD:Boolean;
    {Sets the bitrate for normal CAN devices}
    FBitrate:TPCANBaudrate;
    {Sets the bitrate for CAN FD devices.
     Example - Bitrate Nom: 1Mbit/s Data: 2Mbit/s:
       "f_clock_mhz=20, nom_brp=5, nom_tseg1=2, nom_tseg2=1, nom_sjw=1, data_brp=2, data_tseg1=3, data_tseg2=1, data_sjw=1"}
    FBitrateFD:TPCANBitrateFD;
    {Sets if trace continue after reaching maximum size for the first file}
    FTraceFileSingle:Boolean;
    {Set if date will be add to filename }
    FTraceFileDate:Boolean;
    {Set if time will be add to filename}
    FTraceFileTime:Boolean;
    {Set if existing tracefile overwrites when a new trace session is started}
    FTraceFileOverwrite:Boolean;
    {Set if the column "Data Length" should be used instead of the column "Data Length Code"}
    FTraceFileDataLength:Boolean;
    {Sets the size (megabyte) of an tracefile
     Example - 100 = 100 megabyte
     Range between 1 and 100}
    FTraceFileSize:uint32;
    {Sets a fully-qualified and valid path to an existing directory. In order to use the default path
     (calling process path) an empty string must be set.}
    FTracePath:string;
    {Thread for reading messages}
    FReadThread:TReadThread;
    {Shows if thread run}
    FThreadRun:Boolean;
    {Thread function for reading messages}
    procedure ThreadExecute;
    {Function for reading PCAN-Basic messages}
    procedure ReadMessages;
    {Deactivates the tracing process}
    procedure StopTrace();
    {Configures the way how trace files are formatted
     Returns:
       Returns true if no error occurr}
    function ConfigureTrace():Boolean;
    {Activates the tracing process
     Returns:
       Returns true if no error occurr}
    function StartTrace():Boolean;
    {Shows/prints the configurable parameters for this sample and information about them}
    procedure ShowConfigurationHelp;
    {Shows/prints the configured paramters}
    procedure ShowCurrentConfiguration;
    {Shows formatted status
     Parameters:
       status = Will be formatted}
    procedure ShowStatus(status:TPCANStatus);
    {Gets the formatted text for a PCAN-Basic channel handle
     Parameters:
       handle = PCAN-Basic Handle to format
       isFD = If the channel is FD capable
     Returns:
       The formatted text for a channel}
    function FormatChannelName(handle:TPCANHandle;isFD:Boolean):string;
    {Gets name of a TPCANHandle
     Parameters:
       handle = TPCANHandle to get name
     Returns:
       Returns name of the TPCANHandle}
    function GetTPCANHandleName(handle:TPCANHandle):string;
    {Convert bitrate value to readable string
     Parameters:
       bitrate = Bitrate to be converted
     Returns:
       A text with the converted bitrate}
    function ConvertBitrateToString(bitrate:TPCANBaudrate):string;
    {Help Function used to get an error as text
     Parameters:
       error = Error code to be translated
     Returns:
       A text with the converted bitrate}
    function GetFormattedError(error: TPCANStatus): string;
    {Converts boolean to readable string}
    function ConvertBooleanToString(value:Boolean):string;
    {Gets pressed key}
    function KeyPress():Word;
  public
    procedure Start;
    constructor Create(TheOwner: TComponent); override;
    destructor Destroy; override;
  end;

implementation

constructor TReadThread.Create(CreateSuspended : boolean);
begin
  FreeOnTerminate := True;
  inherited Create(CreateSuspended);
end;

procedure TReadThread.ThreadExecute;
begin
  if Assigned(FOnThreadExecute) then
    begin
      FOnThreadExecute();
    end;
end;

procedure TReadThread.Execute;
begin
  ThreadExecute();
end;

{ TTraceFiles }

procedure TTraceFiles.Start;
var
  stsResult:TPCANStatus;
begin
  ShowConfigurationHelp(); // Shows information about this sample
  ShowCurrentConfiguration(); // Shows the current parameters configuration

  // Initialization of the selected channel
  if (FIsFD) then
    stsResult := TPCANBasic.InitializeFD(FPcanHandle, FBitrateFD)
  else
    stsResult := TPCANBasic.Initialize(FPcanHandle, FBitrate);

  if (stsResult <> PCAN_ERROR_OK) then
  begin
    writeln('Can not initialize. Please check the defines in the code.');
    ShowStatus(stsResult);
    writeln('');
    writeln('Press any key to close');
    KeyPress();
    Exit;
  end;

  // Trace messages...
  writeln('Successfully initialized.');
  writeln('Press any key to start tracing.');
  KeyPress();
  if (ConfigureTrace()) then
  begin
    if (StartTrace()) then
    begin
      FReadThread.Start;
      writeln('Messages are being traced.');
      writeln('Press any key to stop trace and quit');
      KeyPress();
      StopTrace();
      Exit();
    end;
  end;
  writeln('');
  writeln('Press any key to close');
  KeyPress();
end;

constructor TTraceFiles.Create(TheOwner: TComponent);
begin
  inherited Create(TheOwner);
  StopOnException:=True;
  FPcanHandle := TPCANBasic.PCAN_USBBUS1;
  FIsFD := FALSE;
  FBitrate := TPCANBaudrate.PCAN_BAUD_500K;
  FBitrateFD := 'f_clock_mhz=20, nom_brp=5, nom_tseg1=2, nom_tseg2=1, nom_sjw=1, data_brp=2, data_tseg1=3, data_tseg2=1, data_sjw=1';
  FThreadRun := True;
  FTraceFileSingle := True;
  FTraceFileDate := True;
  FTraceFileTime := True;
  FTraceFileOverwrite := False;
  FTraceFileDataLength := False;
  FTraceFileSize := 2;
  FTracePath := '';
  FReadThread := TReadThread.Create(True);
  FReadThread.OnThreadExecute := @ThreadExecute;
end;

destructor TTraceFiles.Destroy;
begin
  FThreadRun := False;
  FReadThread.Terminate;
  inherited Destroy;
end;

procedure TTraceFiles.ThreadExecute();
begin
  while (FThreadRun) do
  begin
    Sleep(1); //Use Sleep to reduce the CPU load
    ReadMessages();
  end;
end;

procedure TTraceFiles.ReadMessages;
var
  canMsgFD: TPCANMsgFD;
  canTimestampFD: TPCANTimestampFD;
  canMsg: TPCANMsg;
  canTimestamp: TPCANTimestamp;
  stsResult: TPCANStatus;
begin
  canTimestampFD:=0;
  canMsgFD := Default(TPCANMsgFD);
  canMsg := Default(TPCANMsg);
  canTimestamp := Default(TPCANTimestamp);
  // We read at least one time the queue looking for messages.
  // If a message is found, we look again trying to find more.
  // If the queue is empty or an error occurr, we get out from
  // the dowhile statement.
  //
  repeat
    if FIsFD then
      stsResult := TPCANBasic.ReadFD(FPcanHandle, canMsgFD, canTimestampFD)
    Else
      stsResult := TPCANBasic.Read(FPcanHandle, canMsg, canTimestamp);

    if ((stsResult <> PCAN_ERROR_OK) and (stsResult <> PCAN_ERROR_QRCVEMPTY)) then
      begin
      ShowStatus(stsResult);
      Break;
      end;
  until (((LongWord(stsResult) and LongWord(PCAN_ERROR_QRCVEMPTY)) <> 0));
end;

procedure TTraceFiles.StopTrace();
var
  iStatus:uint32;
  stsResult:TPCANStatus;
begin
  iStatus := TPCANBasic.PCAN_PARAMETER_OFF;

  // We stop the tracing by setting the parameter.
  stsResult := TPCANBasic.SetValue(FPcanHandle,PCAN_TRACE_STATUS,PLongWord(@iStatus),SizeOf(uint32));

  if (stsResult <> PCAN_ERROR_OK) then
    ShowStatus(stsResult);
end;

function TTraceFiles.ConfigureTrace():Boolean;
var
  iSize:uint32;
  stsResult:TPCANStatus;
  config:uint32;
begin
  iSize := FTraceFileSize;

  // Sets path to store files
  stsResult := TPCANBasic.SetValue(FPcanHandle,PCAN_TRACE_LOCATION,PLongWord(@FTracePath),SizeOf(uint32));

  if (stsResult = PCAN_ERROR_OK) then
  begin
    // Sets the maximum size of a tracefile
    stsResult := TPCANBasic.SetValue(FPcanHandle,PCAN_TRACE_SIZE,PLongWord(@iSize),SizeOf(uint32));

    if (stsResult = PCAN_ERROR_OK) then
    begin
      if (FTraceFileSingle) then
        config := TPCANBasic.TRACE_FILE_SINGLE // Creats one file
      else
        config := TPCANBasic.TRACE_FILE_SEGMENTED; // Creats more files

      // Activate overwriting existing tracefile
      if (FTraceFileOverwrite) then
        config := config or TPCANBasic.TRACE_FILE_OVERWRITE;
      
      // Uses Data Length instead of Data Length Code
      if (FTraceFileDataLength) then
        config := config or TPCANBasic.TRACE_FILE_DATA_LENGTH;

      // Adds date to tracefilename
      if (FTraceFileDate) then
        config := config or TPCANBasic.TRACE_FILE_DATE;

      // Adds time to tracefilename
      if (FTraceFileTime) then
        config := config or TPCANBasic.TRACE_FILE_TIME;

      // Sets the config
      stsResult := TPCANBasic.SetValue(FPcanHandle,PCAN_TRACE_CONFIGURE,PLongWord(@config),SizeOf(uint32));

      if (stsResult = PCAN_ERROR_OK) then
      begin
        Result := True;
        Exit();
      end;
    end;
  end;
  ShowStatus(stsResult);
  Result := False;
end;

function TTraceFiles.StartTrace():Boolean;
var
  iStatus:uint32;
  stsResult:TPCANStatus;
begin
  iStatus := TPCANBasic.PCAN_PARAMETER_ON;

  // We activate the tracing by setting the parameter.
  stsResult := TPCANBasic.SetValue(FPcanHandle,PCAN_TRACE_STATUS,PLongWord(@iStatus),SizeOf(uint32));

  if (stsResult <> PCAN_ERROR_OK) then
  begin
    ShowStatus(stsResult);
    Result := False;
    Exit();
  end;
  Result := True;
end;

procedure TTraceFiles.ShowConfigurationHelp;
begin
  writeln('==========================================================================================');
  writeln('|                           PCAN-Basic TraceFiles Example                                 |');
  writeln('==========================================================================================');
  writeln('Following parameters are to be adjusted before launching, according to the hardware used  |');
  writeln('                                                                                          |');
  writeln('* FPcanHandle: Numeric value that represents the handle of the PCAN-Basic channel to use. |');
  writeln('              See ''PCAN-Handle Definitions within'' the documentation                      |');
  writeln('* FIsFD: Boolean value that indicates the communication mode, CAN (false) or CAN-FD (true)|');
  writeln('* FBitrate: Numeric value that represents the BTR0/BR1 bitrate value to be used for CAN   |');
  writeln('           communication                                                                  |');
  writeln('* FBitrateFD: String value that represents the nominal/data bitrate value to be used for  |');
  writeln('             CAN-FD communication                                                         |');
  writeln('* FTraceFileSingle: Boolean value that indicates if tracing ends after one file (true) or |');
  writeln('                   continues                                                              |');
  writeln('* FTraceFileDate: Boolean value that indicates if the date will be added to filename      |');
  writeln('* FTraceFileTime: Boolean value that indicates if the time will be added to filename      |');
  writeln('* FTraceFileOverwrite: Boolean value that indicates if existing tracefiles should be      |');
  writeln('                      overwritten                                                         |');
  writeln('* TraceFileDataLength: Boolean value that indicates if the column ''Data Length'' is used   |');
  writeln('                       instead of the column ''Data Length Code''                           |');
  writeln('* FTraceFileSize: Numeric value that represents the size of a tracefile in meagabytes     |');
  writeln('* FTracePath: string value that represents a valid path to an existing directory          |');
  writeln('==========================================================================================');
  writeln();
end;

procedure TTraceFiles.ShowCurrentConfiguration;
begin
  writeln('Parameter values used');
  writeln('----------------------');
  writeln('* FPCANHandle: ', FormatChannelName(FPcanHandle,FIsFD));
  writeln('* FIsFD: ', ConvertBooleanToString(FIsFD));
  writeln('* FBitrate: ', ConvertBitrateToString(FBitrate));
  writeln('* FBitrateFD: ', FBitrateFD);
  writeln('* FTraceFileSingle: ', ConvertBooleanToString(FTraceFileSingle));
  writeln('* FTraceFileDate: ', ConvertBooleanToString(FTraceFileDate));
  writeln('* FTraceFileTime: ', ConvertBooleanToString(FTraceFileTime));
  writeln('* FTraceFileOverwrite: ', ConvertBooleanToString(FTraceFileOverwrite));
  writeln('* FTraceFileDataLength: ', ConvertBooleanToString(FTraceFileDataLength));
  writeln('* FTraceFileSize: ', IntToStr(FTraceFileSize),' MB');
  if FTracePath = '' then
    writeln('* FTracePath: (calling application path)')
  else
    writeln('* FTracePath: ', FTracePath);
  writeln();
end;

procedure TTraceFiles.ShowStatus(status:TPCANStatus);
begin
  writeln('=========================================================================================');
  writeln(GetFormattedError(status));
  writeln('=========================================================================================');
end;

function TTraceFiles.FormatChannelName(handle:TPCANHandle;isFD:Boolean):string;
var
  strName: AnsiString;
  byChannel:Byte;
begin
  // Gets the owner device and channel for a PCAN-Basic handle
  if handle < $100 then
    byChannel := handle and $F
  else
    byChannel := handle and $FF;

  // Constructs the PCAN-Basic Channel name and return it
  //
  strName := GetTPCANHandleName(handle);
  if isFD then
    Result := Format('%s:FD %d (%Xh)', [strName, byChannel, handle])
  else
    Result := Format('%s %d (%Xh)', [strName, byChannel, handle])
end;

function TTraceFiles.GetTPCANHandleName(handle: TPCANHandle): string;
begin
  if (handle = TPCANBasic.PCAN_DNGBUS1) then
    Result := 'PCAN_DNG'
  else if ((handle >= TPCANBasic.PCAN_PCIBUS1) and (handle <= TPCANBasic.PCAN_PCIBUS8)) then
    Result := 'PCAN_PCI'
  else if ((handle >= TPCANBasic.PCAN_USBBUS1) and (handle <= TPCANBasic.PCAN_USBBUS8)) then
    Result := 'PCAN_USB'
  else if ((handle >= TPCANBasic.PCAN_LANBUS1) and (handle <= TPCANBasic.PCAN_LANBUS8)) then
    Result := 'PCAN_LAN'
  else
    Result := 'PCAN_NONE';
end;

function TTraceFiles.ConvertBitrateToString(bitrate: TPCANBaudrate): string;
begin
  case bitrate of
    TPCANBaudrate.PCAN_BAUD_1M: Result:='1 MBit/sec';
    TPCANBaudrate.PCAN_BAUD_800K: Result:='800 kBit/sec';
    TPCANBaudrate.PCAN_BAUD_500K: Result:='500 kBit/sec';
    TPCANBaudrate.PCAN_BAUD_250K: Result:='250 kBit/sec';
    TPCANBaudrate.PCAN_BAUD_125K: Result:='125 kBit/sec';
    TPCANBaudrate.PCAN_BAUD_100K: Result:='100 kBit/sec';
    TPCANBaudrate.PCAN_BAUD_95K: Result:='95,238 kBit/sec';
    TPCANBaudrate.PCAN_BAUD_83K: Result:='83,333 kBit/sec';
    TPCANBaudrate.PCAN_BAUD_50K: Result:='50 kBit/sec';
    TPCANBaudrate.PCAN_BAUD_47K: Result:='47,619 kBit/sec';
    TPCANBaudrate.PCAN_BAUD_33K: Result:='33,333 kBit/sec';
    TPCANBaudrate.PCAN_BAUD_20K: Result:='20 kBit/sec';
    TPCANBaudrate.PCAN_BAUD_10K: Result:='10 kBit/sec';
    TPCANBaudrate.PCAN_BAUD_5K: Result:='5 kBit/sec';
    otherwise Result := 'Unknown Bitrate';
    Result := Result;
  end;
end;

function TTraceFiles.GetFormattedError(error: TPCANStatus): string;
var
  status: TPCANStatus;
  buffer: array [0 .. 255] of Ansichar;
begin
  // Gets the text using the GetErrorText API function
  // If the function success, the translated error is returned. If it fails,
  // a text describing the current error is returned.
  //
  status := TPCANBasic.GetErrorText(error, $9, buffer);
  if (status <> PCAN_ERROR_OK) then
    Result := Format('An error ocurred. Error-code''s text (%Xh) couldn''t be retrieved',[Integer(error)])
  else
    Result := buffer;
end;

function TTraceFiles.ConvertBooleanToString(value:Boolean):string;
begin
  if (value) then
    Result := 'True'
  else
    Result := 'False';
end;

function TTraceFiles.KeyPress: Word;
var
  Read: Cardinal;
  Hdl: THandle;
  Rec: _INPUT_RECORD;
begin
  Hdl := GetStdHandle(STD_INPUT_HANDLE);
  Read := 0;
  repeat
    Rec.EventType := KEY_EVENT;
    ReadConsoleInput(Hdl, Rec, 1, Read);
  until (Read = 1) and (Rec.Event.KeyEvent.bKeyDown) and (Rec.Event.KeyEvent.wVirtualKeyCode<>0) ;
  Result := Rec.Event.KeyEvent.wVirtualKeyCode;
end;

end.

