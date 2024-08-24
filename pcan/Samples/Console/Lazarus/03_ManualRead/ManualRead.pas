unit ManualRead;

{$mode objfpc}{$H+}

interface

uses
  Classes,Windows, SysUtils, CustApp, PCANBasic;

type

  { TManualRead }

    TManualRead = class(TCustomApplication)
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
      {Function for reading PCAN-Basic messages}
      procedure ReadMessages;
      {Function for reading messages on CAN-FD devices
       Returns:
         A TPCANStatus error code}
      function ReadMessageFD():TPCANStatus;
      {Function for reading CAN messages on normal CAN devices
       Returns:
         A TPCANStatus error code}
      function ReadMessage():TPCANStatus;
      {Processes a received CAN message
       Parameters:
         msg = The received PCAN-Basic CAN message
         itsTimeStamp = Timestamp of the message as TPCANTimestamp structure}
      procedure ProcessMessageCan(msg:TPCANMsg;itsTimeStamp:TPCANTimestamp);
      {Processes a received CAN-FD message
       Parameters:
         msg = The received PCAN-Basic CAN-FD message
         itsTimeStamp = Timestamp of the message as microseconds}
      procedure ProcessMessageCanFD(msg:TPCANMsgFD;itsTimeStamp:TPCANTimestampFD);
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
         _handle = TPCANHandle to get name
       Returns:
         Returns name of the TPCANHandle}
      function GetTPCANHandleName(handle:TPCANHandle):string;
      {Convert bitrate value to readable string
       Parameters:
         _bitrate = Bitrate to be converted
       Returns:
         A text with the converted bitrate}
      function ConvertBitrateToString(bitrate:TPCANBaudrate):string;
      {Help Function used to get an error as text
       Parameters:
         _error = Error code to be translated
       Returns:
         A text with the converted bitrate}
      function GetFormattedError(error: TPCANStatus): string;
      {Gets the string representation of the type of a CAN message
       Parameters:
         _msgType = Type of a CAN message
       Returns:
         The type of the CAN message as string}
      function GetMsgTypeString(msgType:TPCANMessageType):string;
      {Gets the string representation of the ID of a CAN message
       Parameters:
         _id = Id to be parsed
         _msgType = Type flags of the message the Id belong
       Returns:
         Hexadecimal representation of the ID of a CAN message}
      function GetIdString(id:Longword;msgType:TPCANMessageType) : string;
      {Gets the data length of a CAN message
       Parameters:
         _dlc = Data length code of a CAN message
       Returns:
         Data length as integer represented by the given DLC code}
      function GetLengthFromDLC(dlc: Integer): Integer;
      {Gets the string representation of the timestamp of a CAN message, in milliseconds
       Parameters:
         _time = Timestamp in microseconds
       Returns:
         String representing the timestamp in milliseconds}
      function GetTimeString(time:TPCANTimestampFD): string;
      {Gets the data of a CAN message as a string
       Parameters:
         _data = Array of bytes containing the data to parse
         _msgType = Type flags of the message the data belong
         _dataLength = The amount of bytes to take into account wihtin the given data
       Returns:
         A string with hexadecimal formatted data bytes of a CAN message}
      function GetDataString(data:array of Byte;msgType:TPCANMessageType;dataLength:Integer): string;
      {Gets pressed key}
      function KeyPress():Word;
      {Clears the console}
      procedure ClearScreen;
    public
      procedure Start;
      constructor Create(TheOwner: TComponent); override;
      destructor Destroy; override;
    end;

implementation

{ TManualRead }

procedure TManualRead.Start;
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
    Exit();
  end;

  // Reading messages...
  writeln('Successfully initialized.');
  writeln('Press any key to read');
  KeyPress();
  repeat
    ClearScreen();
    ReadMessages();
    writeln('Do you want to read again? yes[y] or any other key to close');
  until (not (KeyPress() = 89));
end;

constructor TManualRead.Create(TheOwner: TComponent);
begin
  inherited Create(TheOwner);
  StopOnException:=True;
  FPcanHandle := TPCANBasic.PCAN_USBBUS1;
  FIsFD := FALSE;
  FBitrate := TPCANBaudrate.PCAN_BAUD_500K;
  FBitrateFD := 'f_clock_mhz=20, nom_brp=5, nom_tseg1=2, nom_tseg2=1, nom_sjw=1, data_brp=2, data_tseg1=3, data_tseg2=1, data_sjw=1';
end;

destructor TManualRead.Destroy;
begin
  inherited Destroy;
end;

procedure TManualRead.ReadMessages;
var
  stsResult: TPCANStatus;
begin
  // We read at least one time the queue looking for messages.
  // If a message is found, we look again trying to find more.
  // If the queue is empty or an error occurr, we get out from
  // the dowhile statement.
  //
  repeat
    if FIsFD then
      stsResult := ReadMessageFD()
    Else
      stsResult := ReadMessage();

    if ((stsResult <> PCAN_ERROR_OK) and (stsResult <> PCAN_ERROR_QRCVEMPTY)) then
      begin
      ShowStatus(stsResult);
      Break;
      end;
  until (((LongWord(stsResult) and LongWord(PCAN_ERROR_QRCVEMPTY)) <> 0));
end;

function TManualRead.ReadMessageFD():TPCANStatus;
var
  canMsg: TPCANMsgFD;
  canTimestamp: TPCANTimestampFD;
  stsResult: TPCANStatus;
begin
  canMsg := Default(TPCANMsgFD);
  canTimestamp := Default(TPCANTimestampFD);
  // We execute the "Read" function of the PCANBasic
  //
  stsResult := TPCANBasic.ReadFD(FPcanHandle, canMsg, canTimestamp);
  if (stsResult <> PCAN_ERROR_QRCVEMPTY) then
    // We process the received message
    //
    ProcessMessageCanFD(canMsg, canTimestamp);
  Result := stsResult;
end;

function TManualRead.ReadMessage():TPCANStatus;
var
  canMsg: TPCANMsg;
  canTimestamp: TPCANTimestamp;
  stsResult: TPCANStatus;
begin
  canMsg := Default(TPCANMsg);
  canTimestamp := Default(TPCANTimestamp);
  // We execute the "Read" function of the PCANBasic
  //
  stsResult := TPCANBasic.Read(FPcanHandle, canMsg, canTimestamp);
  // We process the message(s)
  //
  if (stsResult <> PCAN_ERROR_QRCVEMPTY) then
    ProcessMessageCan(canMsg, canTimestamp);
  Result := stsResult;
end;

procedure TManualRead.ProcessMessageCan(msg: TPCANMsg; itsTimeStamp: TPCANTimestamp);
var
  microsTimestamp: TPCANTimestampFD;
begin
  microsTimestamp := itsTimeStamp.micros + (Uint64(1000) * itsTimeStamp.millis) + (Uint64($100000000) * Uint64(1000) * itsTimeStamp.millis_overflow);

  writeln('Type: ' + GetMsgTypeString(msg.MSGTYPE));
  writeln('ID: ' + GetIDString(msg.ID,msg.MSGTYPE));
  writeln('Length: ' + IntToStr(msg.LEN));
  writeln('Time: ' + GetTimeString(microsTimestamp));
  writeln('Data: ' + GetDataString(msg.DATA,msg.MSGTYPE,msg.LEN));
  writeln('----------------------------------------------------------');
end;

procedure TManualRead.ProcessMessageCanFD(msg: TPCANMsgFD; itsTimeStamp: TPCANTimestampFD);
begin
  writeln('Type: ' + GetMsgTypeString(msg.MSGTYPE));
  writeln('ID: ' + GetIDString(msg.ID,msg.MSGTYPE));
  writeln('Length: ' + IntToStr(GetLengthFromDLC(msg.DLC)));
  writeln('Time: ' + GetTimeString(itsTimeStamp));
  writeln('Data: ' + GetDataString(msg.DATA,msg.MSGTYPE,GetLengthFromDLC(msg.DLC)));
  writeln('----------------------------------------------------------');
end;

procedure TManualRead.ShowConfigurationHelp;
begin
  writeln('==========================================================================================');
  writeln('|                           PCAN-Basic ManualRead Example                                 |');
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
  writeln('==========================================================================================');
  writeln();
end;

procedure TManualRead.ShowCurrentConfiguration;
begin
  writeln('Parameter values used');
  writeln('----------------------');
  writeln('* FPCANHandle: ', FormatChannelName(FPcanHandle,FIsFD));
  if (FIsFD) then
    writeln('* FIsFD: True')
  else
    writeln('* FIsFD: False');
  writeln('* FBitrate: ', ConvertBitrateToString(FBitrate));
  writeln('* FBitrateFD: ', FBitrateFD);
  writeln();
end;

procedure TManualRead.ShowStatus(status:TPCANStatus);
begin
  writeln('=========================================================================================');
  writeln(GetFormattedError(status));
  writeln('=========================================================================================');
end;

function TManualRead.FormatChannelName(handle:TPCANHandle;isFD:Boolean):string;
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

function TManualRead.GetTPCANHandleName(handle: TPCANHandle): string;
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

function TManualRead.ConvertBitrateToString(bitrate: TPCANBaudrate): string;
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

function TManualRead.GetFormattedError(error: TPCANStatus): string;
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

function TManualRead.GetMsgTypeString(msgType:TPCANMessageType):string;
var
  strTemp: string;
begin
  strTemp := EmptyStr;

  if ((Byte(msgType) and Byte(PCAN_MESSAGE_STATUS)) <> 0) then
    strTemp := 'STATUS'
  else if ((Byte(msgType) and Byte(PCAN_MESSAGE_ERRFRAME)) <> 0) then
    strTemp := 'ERROR'
  else
  begin
    if ((Byte(msgType) and Byte(PCAN_MESSAGE_EXTENDED)) <> 0) then
      strTemp := 'EXT'
    else
      strTemp := 'STD';

    if ((Byte(msgType) and Byte(PCAN_MESSAGE_RTR)) = Byte(PCAN_MESSAGE_RTR)) then
      strTemp := (strTemp + '/RTR')
    else
    begin
      if (Byte(msgType) > Byte(PCAN_MESSAGE_EXTENDED)) then
      begin
        strTemp := strTemp + (' [ ');
        if ((Byte(msgType) and Byte(PCAN_MESSAGE_FD)) = Byte(PCAN_MESSAGE_FD)) then
          strTemp := strTemp + (' FD');
        if ((Byte(msgType) and Byte(PCAN_MESSAGE_BRS)) = Byte(PCAN_MESSAGE_BRS)) then
          strTemp := strTemp + (' BRS');
        if ((Byte(msgType) and Byte(PCAN_MESSAGE_ESI)) = Byte(PCAN_MESSAGE_ESI)) then
          strTemp := strTemp + (' ESI');
        strTemp := strTemp + (' ]');
      end;
    end;
  end;

  Result := strTemp;
end;

function TManualRead.GetIdString(id:Longword;msgType:TPCANMessageType) : string;
begin
  if ((Byte(msgType) and Byte(PCAN_MESSAGE_EXTENDED)) <> 0) then
    Result := IntToHex(id, 8) + 'h'
  else
    Result := IntToHex(id, 3) + 'h';
end;

function TManualRead.GetLengthFromDLC(dlc: Integer): Integer;
begin
  case dlc of
    9:
      Result := 12;
    10:
      Result := 16;
    11:
      Result := 20;
    12:
      Result := 24;
    13:
      Result := 32;
    14:
      Result := 48;
    15:
      Result := 64;
  else
    Result := dlc;
  end;
end;

function TManualRead.GetTimeString(time:TPCANTimestampFD): string;
var
  fTime: Double;
begin
  fTime := (time / 1000);
  Result := Format('%.1f', [fTime]);
end;

function TManualRead.GetDataString(data:array of Byte;msgType:TPCANMessageType;dataLength:Integer): string;
var
  strTemp: string;
  I: Integer;
begin
  strTemp := EmptyStr;

  if ((Byte(msgType) and Byte(PCAN_MESSAGE_RTR)) = Byte(PCAN_MESSAGE_RTR)) then
    Result := 'Remote Request'
  else
  begin
    for I := 0 To dataLength do
      strTemp := (strTemp + IntToHex(data[I], 2) + ' ');
    Result := strTemp;
  end;
end;

function TManualRead.KeyPress: Word;
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

procedure TManualRead.ClearScreen;
var
  stdout: THandle;
  csbi: TConsoleScreenBufferInfo;
  ConsoleSize: DWORD;
  NumWritten: DWORD;
  Origin: TCoord;
begin
  csbi := Default(TConsoleScreenBufferInfo);
  NumWritten := Default(DWORD);
  stdout := GetStdHandle(STD_OUTPUT_HANDLE);
  Win32Check(stdout<>INVALID_HANDLE_VALUE);
  Win32Check(GetConsoleScreenBufferInfo(stdout, csbi));
  ConsoleSize := csbi.dwSize.X * csbi.dwSize.Y;
  Origin.X := 0;
  Origin.Y := 0;
  Win32Check(FillConsoleOutputCharacter(stdout, ' ', ConsoleSize, Origin,
  NumWritten));
  Win32Check(FillConsoleOutputAttribute(stdout, csbi.wAttributes, ConsoleSize, Origin,
  NumWritten));
  Win32Check(SetConsoleCursorPosition(stdout, Origin));
end;

end.

