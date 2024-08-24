unit ManualWrite;

{$mode objfpc}{$H+}

interface

uses
  Classes,Windows, SysUtils, CustApp, PCANBasic;

type

  { TManualWrite }

    TManualWrite = class(TCustomApplication)
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
      {Function for writing PCAN-Basic messages}
      procedure WriteMessages;
      {Function for writing messages on CAN devices
       Returns:
         A TPCANStatus error code}
      function WriteMessage():TPCANStatus;
      {Function for writing messages on CAN-FD devices
       Returns:
         A TPCANStatus error code}
      function WriteMessageFD():TPCANStatus;
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

{ TManualWrite }

procedure TManualWrite.Start;
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
  writeln('Press any key to write');
  KeyPress();
  repeat
    ClearScreen();
    WriteMessages();
    writeln('Do you want to write again? yes[y] or any other key to close');
  until (not (KeyPress() = 89));
end;

constructor TManualWrite.Create(TheOwner: TComponent);
begin
  inherited Create(TheOwner);
  StopOnException:=True;
  FPcanHandle := TPCANBasic.PCAN_USBBUS1;
  FIsFD := FALSE;
  FBitrate := TPCANBaudrate.PCAN_BAUD_500K;
  FBitrateFD := 'f_clock_mhz=20, nom_brp=5, nom_tseg1=2, nom_tseg2=1, nom_sjw=1, data_brp=2, data_tseg1=3, data_tseg2=1, data_sjw=1';
end;

destructor TManualWrite.Destroy;
begin
  inherited Destroy;
end;

procedure TManualWrite.WriteMessages();
var
  stsResult:TPCANStatus;
begin
  if (FIsFD) then
    stsResult := WriteMessageFD()
  else
    stsResult := WriteMessage();
  // Checks if the message was sent
  if (stsResult <> PCAN_ERROR_OK) then
    ShowStatus(stsResult)
  else
    writeln('Message was successfully SENT');
end;

function TManualWrite.WriteMessage():TPCANStatus;
var
  msgCanMessage: TPCANMsg;
  i: Integer;
begin
  // Sends a CAN-FD message with standard ID, 64 data bytes, and bitrate switch
  ZeroMemory(@msgCanMessage, SizeOf(msgCanMessage));
  msgCanMessage.ID  := $100;
  msgCanMessage.LEN := 8;
  msgCanMessage.MSGTYPE := PCAN_MESSAGE_EXTENDED;
  for i := 0 to 8 do
  begin
    msgCanMessage.DATA[i] := i;
  end;
  Result := TPCANBasic.Write(FPcanHandle, msgCanMessage);
end;

function TManualWrite.WriteMessageFD():TPCANStatus;
var
  msgCanMessageFD: TPCANMsgFD;
  i: Integer;
begin
  // Sends a CAN-FD message with standard ID, 64 data bytes, and bitrate switch
  ZeroMemory(@msgCanMessageFD, SizeOf(msgCanMessageFD));
  msgCanMessageFD.ID  := $100;
  msgCanMessageFD.dlc := 15;
  msgCanMessageFD.MSGTYPE := TPCANMessageType(Byte(msgCanMessageFD.MSGTYPE) OR Byte(PCAN_MESSAGE_BRS));
  for i := 0 to 64 do
  begin
    msgCanMessageFD.DATA[i] := i;
  end;
  Result := TPCANBasic.WriteFD(FPcanHandle, msgCanMessageFD);
end;

procedure TManualWrite.ShowConfigurationHelp;
begin
  writeln('==========================================================================================');
  writeln('|                           PCAN-Basic ManualWrite Example                                |');
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

procedure TManualWrite.ShowCurrentConfiguration;
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

procedure TManualWrite.ShowStatus(status:TPCANStatus);
begin
  writeln('=========================================================================================');
  writeln(GetFormattedError(status));
  writeln('=========================================================================================');
end;

function TManualWrite.FormatChannelName(handle:TPCANHandle;isFD:Boolean):string;
var
  strName: string;
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

function TManualWrite.GetTPCANHandleName(handle: TPCANHandle): string;
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

function TManualWrite.ConvertBitrateToString(bitrate: TPCANBaudrate): string;
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

function TManualWrite.GetFormattedError(error: TPCANStatus): string;
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

function TManualWrite.KeyPress: Word;
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

procedure TManualWrite.ClearScreen;
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

