unit LookUpChannel;

{$mode objfpc}{$H+}

interface

uses
  Classes,Windows, SysUtils, CustApp, PCANBasic;

type

  { TLookUpChannel }

    TLookUpChannel = class(TCustomApplication)
    private
      {Sets a TPCANDevice value. The input can be numeric, in hexadecimal or decimal
       format, or as string denotinga TPCANDevice value name.}
      FDeviceType:string;
      {Sets value in range of a double. The input can be hexadecimal or decimal format.}
      FDeviceID:string;
      {Sets a zero-based index value in range of a double. The input can be hexadecimal or decimal format.}
      FControllerNumber:string;
      {Sets a valid Internet Protocol address}
      FIPAddress:string;
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
      {Checks if the string is emtpy}
      function IsStringEmpty(checkstring:string):Boolean;
      {Gets pressed key}
      function KeyPress():Word;
    public
      procedure Start;
      constructor Create(TheOwner: TComponent); override;
      destructor Destroy; override;
    end;

implementation

{ TManualWrite }

procedure TLookUpChannel.Start;
var
  stsResult:TPCANStatus;
  handle:TPCANHandle;
  sParameters:string;
  iFeatures:uint32;
begin
  ShowConfigurationHelp(); // Shows information about this sample
  ShowCurrentConfiguration(); // Shows the current parameters configuration

  writeln('Press any key to start searching');
  KeyPress();
  writeln('');

  sParameters := '';
  if (IsStringEmpty(FDeviceType)) then
    sParameters := Format('%s=%s',[TPCANBasic.LOOKUP_DEVICE_TYPE,FDeviceType]);
  if (IsStringEmpty(FDeviceID)) then
    begin
    if (IsStringEmpty(sParameters)) then
      sParameters := Format('%s, ',[sParameters]);
    sParameters := Format('%s%s=%s',[sParameters,TPCANBasic.LOOKUP_DEVICE_ID,FDeviceID]);
    end;
  if (IsStringEmpty(FControllerNumber)) then
    begin
    if (IsStringEmpty(sParameters)) then
      sParameters := Format('%s, ',[sParameters]);
    sParameters := Format('%s%s=%s',[sParameters,TPCANBasic.LOOKUP_CONTROLLER_NUMBER,FControllerNumber]);
    end;
  if (IsStringEmpty(FIPAddress)) then
    begin
    if (IsStringEmpty(sParameters)) then
      sParameters := Format('%s, ',[sParameters]);
    sParameters := Format('%s%s=%s',[sParameters,TPCANBasic.LOOKUP_IP_ADDRESS,FIPAddress]);
    end;

  stsResult := TPCANBasic.LookUpChannel(PAnsiChar(sParameters),@handle);

  if (stsResult = TPCANStatus.PCAN_ERROR_OK) then
  begin
    if (handle <> TPCANBasic.PCAN_NONEBUS) then
    begin
      stsResult := TPCANBasic.GetValue(handle,PCAN_CHANNEL_FEATURES,PLongWord(@iFeatures),SizeOf(iFeatures));

      if (stsResult = TPCANStatus.PCAN_ERROR_OK) then
        writeln(Format('The channel handle %s was found',[FormatChannelName(handle,(iFeatures and TPCANBasic.FEATURE_FD_CAPABLE) = TPCANBasic.FEATURE_FD_CAPABLE)]))
      else
        writeln('There was an issue retrieveing supported channel features');
    end
    else
      writeln('A handle for these lookup-criteria was not found');
  end;

  if (stsResult <> TPCANStatus.PCAN_ERROR_OK) then
  begin
    writeln('There was an error looking up the device, are any hardware channels attached?');
    ShowStatus(stsResult);
  end;
  writeln('');
  writeln('Press any key to close');
  KeyPress();
end;

constructor TLookUpChannel.Create(TheOwner: TComponent);
begin
  inherited Create(TheOwner);
  StopOnException:=True;
  FDeviceType := 'PCAN_USB';
  FDeviceID := '';
  FControllerNumber := '';
  FIPAddress := '';
end;

destructor TLookUpChannel.Destroy;
begin
  inherited Destroy;
end;

procedure TLookUpChannel.ShowConfigurationHelp;
begin
  writeln('==========================================================================================');
  writeln('|                           PCAN-Basic LookUpChannel Example                              |');
  writeln('==========================================================================================');
  writeln('Following parameters are to be adjusted before launching, according to the hardware used  |');
  writeln('                                                                                          |');
  writeln('* FDeviceType: Numeric value that represents a TPCANDevice                                |');
  writeln('* FDeviceID: Numeric value that represents the device identifier                          |');
  writeln('* FControllerNumber: Numeric value that represents controller number                      |');
  writeln('* FIPAddress: String value that represents a valid Internet Protocol address              |');
  writeln('                                                                                          |');
  writeln('For more information see ''LookUp Parameter Definition'' within the documentation           |');
  writeln('==========================================================================================');
  writeln();
end;

procedure TLookUpChannel.ShowCurrentConfiguration;
begin
  writeln('Parameter values used');
  writeln('----------------------');
  writeln('* FDeviceType: '+ FDeviceType);
  writeln('* FDeviceID: '+ FDeviceID);
  writeln('* FControllerNumber: '+ FControllerNumber);
  writeln('* FIPAddress: '+ FIPAddress);
  writeln();
end;

procedure TLookUpChannel.ShowStatus(status:TPCANStatus);
begin
  writeln('=========================================================================================');
  writeln(GetFormattedError(status));
  writeln('=========================================================================================');
end;

function TLookUpChannel.FormatChannelName(handle:TPCANHandle;isFD:Boolean):string;
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

function TLookUpChannel.GetTPCANHandleName(handle: TPCANHandle): string;
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

function TLookUpChannel.ConvertBitrateToString(bitrate: TPCANBaudrate): string;
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

function TLookUpChannel.GetFormattedError(error: TPCANStatus): string;
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

function TLookUpChannel.IsStringEmpty(checkstring:string):Boolean;
begin
  Result := (checkstring <> '');
end;

function TLookUpChannel.KeyPress: Word;
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

