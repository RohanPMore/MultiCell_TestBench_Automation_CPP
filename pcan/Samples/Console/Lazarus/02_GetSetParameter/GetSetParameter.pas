unit GetSetParameter;

{$mode objfpc}{$H+}

interface

uses
  Classes,Windows, SysUtils, CustApp, PCANBasic;

type

  { TGetSetParameter }

    TGetSetParameter = class(TCustomApplication)
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

      {Runs all commands for get or set parameters}
      procedure RunSelectedCommands;

      {Shows device identifier parameter}
      procedure GetPCAN_DEVICE_ID;

      {Sets device identifier parameter}
      procedure SetPCAN_DEVICE_ID(iDeviceID:uint32);

      {Shows all information about attached channels}
      procedure GetPCAN_ATTACHED_CHANNELS;

      {Shows the status of selected PCAN-Channel}
      procedure GetPCAN_CHANNEL_CONDITION;

      {Shows the status from the status LED of the USB devices}
      procedure GetPCAN_CHANNEL_IDENTIFYING;

      {De/Activates the status LED of the USB devices
       Parameters:
         value = True to turn on; False to turn off}
      procedure SetPCAN_CHANNEL_IDENTIFYING(value:Boolean);

      {Shows information about features}
      procedure GetPCAN_CHANNEL_FEATURES;

      {Shows the status from Bitrate-Adapting mode}
      procedure GetPCAN_BITRATE_ADAPTING;

      {De/Activates the Bitrate-Adapting mode
       Parameters:
         value = True to turn on; False to turn off}
      procedure SetPCAN_BITRATE_ADAPTING(value:Boolean);

      {Shows the status from the reception of status frames}
      procedure GetPCAN_ALLOW_STATUS_FRAMES;

      {De/Activates the reception of status frames
       Parameters:
         value = True to turn on; False to turn off}
      procedure SetPCAN_ALLOW_STATUS_FRAMES(value:Boolean);

      {Shows the status from the reception of RTR frames}
      procedure GetPCAN_ALLOW_RTR_FRAMES;

      {De/Activates the reception of RTR frames
       Parameters:
         value = True to turn on; False to turn off}
      procedure SetPCAN_ALLOW_RTR_FRAMES(value:Boolean);

      {Shows the status from the reception of CAN error frames}
      procedure GetPCAN_ALLOW_ERROR_FRAMES;

      {De/Activates the reception of CAN error frames
       Parameters:
         value = True to turn on; False to turn off}
      procedure SetPCAN_ALLOW_ERROR_FRAMES(value:Boolean);

      {Shows the status from the reception of CAN echo frames}
      procedure GetPCAN_ALLOW_ECHO_FRAMES;

      {De/Activates the reception of CAN echo frames
       Parameters:
         value = True to turn on; False to turn off}
      procedure SetPCAN_ALLOW_ECHO_FRAMES(value:Boolean);

      {Shows the reception filter with a specific 11-bit acceptance code and mask}
      procedure GetPCAN_ACCEPTANCE_FILTER_11BIT;

      {Sets the reception filter with a specific 11-bit acceptance code and mask
       Parameters:
         iacceptancefilter11bit = Acceptance code and mask}
      procedure SetPCAN_ACCEPTANCE_FILTER_11BIT(iacceptancefilter11bit:uint64);

      {Shows the reception filter with a specific 29-bit acceptance code and mask}
      procedure GetPCAN_ACCEPTANCE_FILTER_29BIT;

      {Sets the reception filter with a specific 29-bit acceptance code and mask
       Parameters:
         iacceptancefilter29bit = Acceptance code and mask}
      procedure SetPCAN_ACCEPTANCE_FILTER_29BIT(iacceptancefilter29bit:uint64);

      {Shows the status of the reception filter}
      procedure GetPCAN_MESSAGE_FILTER;

      {De/Activates the reception filter
       Parameters:
         imessagefilter = Configure reception filter}
      procedure SetPCAN_MESSAGE_FILTER(imessagefilter:uint32);

      {Shows the status of the hard reset within the PCANBasic.Reset method}
      procedure GetPCAN_HARD_RESET_STATUS;

      {De/Activates the hard reset within the PCANBasic.Reset method
       Parameters:
         value = True to turn on; False to turn off}
      procedure SetPCAN_HARD_RESET_STATUS(value:Boolean);

      {Shows the communication direction of a PCAN-Channel representing a LAN interface}
      procedure GetPCAN_LAN_CHANNEL_DIRECTION;

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
      {Convert uint value to readable string value}
      function ConvertToParameterOnOff(value:uint): string;
      {Convert uint value to readable string value}
      function ConvertToChannelDirection(value:uint): string;
      {Convert uint value to readable string value}
      function ConvertToChannelFeatures(value:uint): string;
      {Convert uint value to readable string value}
      function ConvertToChannelCondition(value:uint): string;
      {Convert uint value to readable string value}
      function ConvertToFilterOpenCloseCustom(value:uint): string;
      {Convert uint value to readable string value}
      function ConvertDeviceTypeToString(value:TPCANDevice): string;
      {Gets pressed key}
      function KeyPress():Word;
    public
      procedure Start;
      constructor Create(TheOwner: TComponent); override;
      destructor Destroy; override;
    end;

implementation

{ TGetSetParameter }

procedure TGetSetParameter.Start;
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
  writeln('Press any key to get/set parameter');
  KeyPress();
  writeln('');

  RunSelectedCommands();

  writeln('');
  writeln('Press any key to close');
  KeyPress();
end;

constructor TGetSetParameter.Create(TheOwner: TComponent);
begin
  inherited Create(TheOwner);
  StopOnException:=True;
  FPcanHandle := TPCANBasic.PCAN_USBBUS1;
  FIsFD := FALSE;
  FBitrate := TPCANBaudrate.PCAN_BAUD_500K;
  FBitrateFD := 'f_clock_mhz=20, nom_brp=5, nom_tseg1=2, nom_tseg2=1, nom_sjw=1, data_brp=2, data_tseg1=3, data_tseg2=1, data_sjw=1';
end;

destructor TGetSetParameter.Destroy;
begin
  inherited Destroy;
end;

procedure TGetSetParameter.RunSelectedCommands;
begin
  // Fill commands here
  writeln('Fill \"RunSelectedCommands\"-function with parameter functions from \"Parameter commands\"-Region in the code.');
end;

{$REGION 'Parameter commands'}
{$REGION 'PCAN_DEVICE_ID'}
procedure TGetSetParameter.GetPCAN_DEVICE_ID;
var
  stsResult:TPCANStatus;
  iDeviceID: Integer;
begin
  stsResult := TPCANBasic.GetValue(FPcanHandle, PCAN_DEVICE_ID, PLongWord(@iDeviceID),SizeOf(iDeviceID));

  if (stsResult = TPCANStatus.PCAN_ERROR_OK) then
  begin
    writeln('-----------------------------------------------------------------------------------------');
    writeln(Format('Get PCAN_DEVICE_ID: %d', [iDeviceID]));
    writeln('');
  end
  else
    ShowStatus(stsResult);
end;

procedure TGetSetParameter.SetPCAN_DEVICE_ID(iDeviceID:uint32);
var
  stsResult:TPCANStatus;
begin
  stsResult := TPCANBasic.SetValue(FPcanHandle, PCAN_DEVICE_ID, PLongWord(@iDeviceID),SizeOf(iDeviceID));

  if (stsResult = TPCANStatus.PCAN_ERROR_OK) then
  begin
    writeln('-----------------------------------------------------------------------------------------');
    writeln(Format('Set PCAN_DEVICE_ID: %d', [iDeviceID]));
    writeln('');
  end
  else
    ShowStatus(stsResult);
end;
{$ENDREGION}

{$REGION 'PCAN_ATTACHED_CHANNELS'}
procedure TGetSetParameter.GetPCAN_ATTACHED_CHANNELS;
var
  stsResult:TPCANStatus; 
  iChannelsCount: Integer;
  ciChannelInformation: array of TPCANChannelInformation;
  I: Integer;
begin
  ciChannelInformation := nil;
  stsResult := TPCANBasic.GetValue(TPCANBasic.PCAN_NONEBUS, PCAN_ATTACHED_CHANNELS_COUNT, PLongWord(@iChannelsCount), SizeOf(iChannelsCount));

  if (stsResult = TPCANStatus.PCAN_ERROR_OK) then
  begin
    SetLength(ciChannelInformation, iChannelsCount);

    stsResult := TPCANBasic.GetValue(TPCANBasic.PCAN_NONEBUS, PCAN_ATTACHED_CHANNELS, PTPCANChannelInformation(ciChannelInformation), SizeOf(TPCANChannelInformation) * iChannelsCount);
    if (stsResult = TPCANStatus.PCAN_ERROR_OK) then
    begin
      writeln('-----------------------------------------------------------------------------------------');
      writeln('Get PCAN_ATTACHED_CHANNELS:');

      for I := 0 To iChannelsCount - 1 do
      begin
        writeln('---------------------------');
        if (FIsFD) then
          writeln(Format('channel_handle:      %sBUS%d', [GetTPCANHandleName(ciChannelInformation[i].channel_handle),(ciChannelInformation[i].channel_handle and $FF)]))
        else
          writeln(Format('channel_handle:      %sBUS%d', [GetTPCANHandleName(ciChannelInformation[i].channel_handle),(ciChannelInformation[i].channel_handle and $F)]));
        writeln('device_type:         ' + ConvertDeviceTypeToString(ciChannelInformation[I].device_type));
        writeln('controller_number:   ' + IntToStr(int64(ciChannelInformation[I].controller_number)));
        writeln('device_features:     ' + ConvertToChannelFeatures(ciChannelInformation[I].device_features));
        writeln('device_name:         ' + ciChannelInformation[I].device_name);
        writeln('device_id:           ' + IntToStr(int64(ciChannelInformation[I].device_id)));
        writeln('channel_condition:   ' + ConvertToChannelCondition(ciChannelInformation[I].channel_condition));
      end;
    end;
  end;
  if (stsResult <> TPCANStatus.PCAN_ERROR_OK) then
    ShowStatus(stsResult);
end;
{$ENDREGION}

{$REGION 'PCAN_CHANNEL_CONDITION'}
procedure TGetSetParameter.GetPCAN_CHANNEL_CONDITION;
var
  stsResult:TPCANStatus;
  iChannelCondition: uint32;
begin
  stsResult := TPCANBasic.GetValue(FPcanHandle, PCAN_CHANNEL_CONDITION, PLongWord(@iChannelCondition),SizeOf(iChannelCondition));

  if (stsResult = TPCANStatus.PCAN_ERROR_OK) then
  begin
    writeln('-----------------------------------------------------------------------------------------');
    writeln(Format('Get PCAN_CHANNEL_CONDITION: %s', [ConvertToChannelCondition(iChannelCondition)]));
    writeln('');
  end
  else
    ShowStatus(stsResult);
end;
{$ENDREGION}

{$REGION 'PCAN_CHANNEL_IDENTIFYING'}
procedure TGetSetParameter.GetPCAN_CHANNEL_IDENTIFYING;
var
  stsResult:TPCANStatus;
  iChannelIdentifying: uint32;
begin
  stsResult := TPCANBasic.GetValue(FPcanHandle, PCAN_CHANNEL_IDENTIFYING, PLongWord(@iChannelIdentifying),SizeOf(iChannelIdentifying));

  if (stsResult = TPCANStatus.PCAN_ERROR_OK) then
  begin
    writeln('-----------------------------------------------------------------------------------------');
    writeln(Format('Get PCAN_CHANNEL_IDENTIFYING: %s', [ConvertToParameterOnOff(iChannelIdentifying)]));
    writeln('');
  end
  else
    ShowStatus(stsResult);
end;

procedure TGetSetParameter.SetPCAN_CHANNEL_IDENTIFYING(value:Boolean);
var
  stsResult:TPCANStatus;
  iChannelIdentifying: uint32;
begin
  if (value) then
    iChannelIdentifying := TPCANBasic.PCAN_PARAMETER_ON
  else
    iChannelIdentifying := TPCANBasic.PCAN_PARAMETER_OFF;

  stsResult := TPCANBasic.SetValue(FPcanHandle, PCAN_CHANNEL_IDENTIFYING, PLongWord(@iChannelIdentifying),SizeOf(iChannelIdentifying));

  if (stsResult = TPCANStatus.PCAN_ERROR_OK) then
  begin
    writeln('-----------------------------------------------------------------------------------------');
    writeln(Format('Set PCAN_CHANNEL_IDENTIFYING: %s', [ConvertToParameterOnOff(iChannelIdentifying)]));
    writeln('');
  end
  else
    ShowStatus(stsResult);
end;
{$ENDREGION}

{$REGION 'PCAN_CHANNEL_FEATURES'}
procedure TGetSetParameter.GetPCAN_CHANNEL_FEATURES;
var
  stsResult:TPCANStatus;
  iChannelFeatures: uint32;
begin
  stsResult := TPCANBasic.GetValue(FPcanHandle, PCAN_CHANNEL_FEATURES, PLongWord(@iChannelFeatures),SizeOf(iChannelFeatures));

  if (stsResult = TPCANStatus.PCAN_ERROR_OK) then
  begin
    writeln('-----------------------------------------------------------------------------------------');
    writeln(Format('Get PCAN_CHANNEL_FEATURES: %s', [ConvertToChannelFeatures(iChannelFeatures)]));
    writeln('');
  end
  else
    ShowStatus(stsResult);
end;
{$ENDREGION}

{$REGION 'PCAN_BITRATE_ADAPTING'}
procedure TGetSetParameter.GetPCAN_BITRATE_ADAPTING;
var
  stsResult:TPCANStatus;
  iBitrateAdapting: uint32;
begin
  stsResult := TPCANBasic.GetValue(FPcanHandle, PCAN_BITRATE_ADAPTING, PLongWord(@iBitrateAdapting),SizeOf(iBitrateAdapting));

  if (stsResult = TPCANStatus.PCAN_ERROR_OK) then
  begin
    writeln('-----------------------------------------------------------------------------------------');
    writeln(Format('Get PCAN_BITRATE_ADAPTING: %s', [ConvertToParameterOnOff(iBitrateAdapting)]));
    writeln('');
  end
  else
    ShowStatus(stsResult);
end;

procedure TGetSetParameter.SetPCAN_BITRATE_ADAPTING(value:Boolean);
var
  stsResult:TPCANStatus;
  iBitrateAdapting: uint32;
begin

  // Note: SetPCAN_BITRATE_ADAPTING requires an uninitialized channel
  //
  TPCANBasic.Uninitialize(TPCANBasic.PCAN_NONEBUS);

  if (value) then
    iBitrateAdapting := TPCANBasic.PCAN_PARAMETER_ON
  else
    iBitrateAdapting := TPCANBasic.PCAN_PARAMETER_OFF;
  stsResult := TPCANBasic.SetValue(FPcanHandle, PCAN_BITRATE_ADAPTING, PLongWord(@iBitrateAdapting),SizeOf(iBitrateAdapting));

  if (stsResult = TPCANStatus.PCAN_ERROR_OK) then
  begin
    writeln('-----------------------------------------------------------------------------------------');
    writeln(Format('Set PCAN_BITRATE_ADAPTING: %s', [ConvertToParameterOnOff(iBitrateAdapting)]));
    writeln('');
  end
  else
    ShowStatus(stsResult);
    
  // Channel will be connected again
  if (FIsFD) then
    stsResult := TPCANBasic.InitializeFD(FPcanHandle, FBitrateFD)
  else
    stsResult := TPCANBasic.Initialize(FPcanHandle, FBitrate);

  if (stsResult <> PCAN_ERROR_OK) then
  begin
    writeln('Error while re-initializing the channel.');
    ShowStatus(stsResult);
  end;
      
end;
{$ENDREGION}

{$REGION 'PCAN_ALLOW_STATUS_FRAMES'}
procedure TGetSetParameter.GetPCAN_ALLOW_STATUS_FRAMES;
var
  stsResult:TPCANStatus;
  iAllowStatusFrames: uint32;
begin
  stsResult := TPCANBasic.GetValue(FPcanHandle, PCAN_ALLOW_STATUS_FRAMES, PLongWord(@iAllowStatusFrames),SizeOf(iAllowStatusFrames));

  if (stsResult = TPCANStatus.PCAN_ERROR_OK) then
  begin
    writeln('-----------------------------------------------------------------------------------------');
    writeln(Format('Get PCAN_ALLOW_STATUS_FRAMES: %s', [ConvertToParameterOnOff(iAllowStatusFrames)]));
    writeln('');
  end
  else
    ShowStatus(stsResult);
end;

procedure TGetSetParameter.SetPCAN_ALLOW_STATUS_FRAMES(value:Boolean);
var
  stsResult:TPCANStatus;
  iAllowStatusFrames: uint32;
begin
  if (value) then
    iAllowStatusFrames := TPCANBasic.PCAN_PARAMETER_ON
  else
    iAllowStatusFrames := TPCANBasic.PCAN_PARAMETER_OFF;
  stsResult := TPCANBasic.SetValue(FPcanHandle, PCAN_ALLOW_STATUS_FRAMES, PLongWord(@iAllowStatusFrames),SizeOf(iAllowStatusFrames));

  if (stsResult = TPCANStatus.PCAN_ERROR_OK) then
  begin
    writeln('-----------------------------------------------------------------------------------------');
    writeln(Format('Set PCAN_ALLOW_STATUS_FRAMES: %s', [ConvertToParameterOnOff(iAllowStatusFrames)]));
    writeln('');
  end
  else
    ShowStatus(stsResult);
end;
{$ENDREGION}

{$REGION 'PCAN_ALLOW_RTR_FRAMES'}
procedure TGetSetParameter.GetPCAN_ALLOW_RTR_FRAMES;
var
  stsResult:TPCANStatus;
  iAllowRTRFrames: uint32;
begin
  stsResult := TPCANBasic.GetValue(FPcanHandle, PCAN_ALLOW_RTR_FRAMES, PLongWord(@iAllowRTRFrames),SizeOf(iAllowRTRFrames));

  if (stsResult = TPCANStatus.PCAN_ERROR_OK) then
  begin
    writeln('-----------------------------------------------------------------------------------------');
    writeln(Format('Get PCAN_ALLOW_RTR_FRAMES: %s', [ConvertToParameterOnOff(iAllowRTRFrames)]));
    writeln('');
  end
  else
    ShowStatus(stsResult);
end;

procedure TGetSetParameter.SetPCAN_ALLOW_RTR_FRAMES(value:Boolean);
var
  stsResult:TPCANStatus;
  iAllowRTRFrames: uint32;
begin
  if (value) then
    iAllowRTRFrames := TPCANBasic.PCAN_PARAMETER_ON
  else
    iAllowRTRFrames := TPCANBasic.PCAN_PARAMETER_OFF;
  stsResult := TPCANBasic.SetValue(FPcanHandle, PCAN_ALLOW_RTR_FRAMES, PLongWord(@iAllowRTRFrames),SizeOf(iAllowRTRFrames));

  if (stsResult = TPCANStatus.PCAN_ERROR_OK) then
  begin
    writeln('-----------------------------------------------------------------------------------------');
    writeln(Format('Set PCAN_ALLOW_RTR_FRAMES: %s', [ConvertToParameterOnOff(iAllowRTRFrames)]));
    writeln('');
  end
  else
    ShowStatus(stsResult);
end;
{$ENDREGION}

{$REGION 'PCAN_ALLOW_ERROR_FRAMES'}
procedure TGetSetParameter.GetPCAN_ALLOW_ERROR_FRAMES;
var
  stsResult:TPCANStatus;
  iAllowErrorFrames: uint32;
begin
  stsResult := TPCANBasic.GetValue(FPcanHandle, PCAN_ALLOW_ERROR_FRAMES, PLongWord(@iAllowErrorFrames),SizeOf(iAllowErrorFrames));

  if (stsResult = TPCANStatus.PCAN_ERROR_OK) then
  begin
    writeln('-----------------------------------------------------------------------------------------');
    writeln(Format('Get PCAN_ALLOW_ERROR_FRAMES: %s', [ConvertToParameterOnOff(iAllowErrorFrames)]));
    writeln('');
  end
  else
    ShowStatus(stsResult);
end;

procedure TGetSetParameter.SetPCAN_ALLOW_ERROR_FRAMES(value:Boolean);
var
  stsResult:TPCANStatus;
  iAllowErrorFrames: uint32;
begin
  if (value) then
    iAllowErrorFrames := TPCANBasic.PCAN_PARAMETER_ON
  else
    iAllowErrorFrames := TPCANBasic.PCAN_PARAMETER_OFF;
  stsResult := TPCANBasic.SetValue(FPcanHandle, PCAN_ALLOW_ERROR_FRAMES, PLongWord(@iAllowErrorFrames),SizeOf(iAllowErrorFrames));

  if (stsResult = TPCANStatus.PCAN_ERROR_OK) then
  begin
    writeln('-----------------------------------------------------------------------------------------');
    writeln(Format('Set PCAN_ALLOW_ERROR_FRAMES: %s', [ConvertToParameterOnOff(iAllowErrorFrames)]));
    writeln('');
  end
  else
    ShowStatus(stsResult);
end;
{$ENDREGION}

{$REGION 'PCAN_ALLOW_ECHO_FRAMES'}
procedure TGetSetParameter.GetPCAN_ALLOW_ECHO_FRAMES;
var
  stsResult:TPCANStatus;
  iAllowEchoFrames: uint32;
begin
  stsResult := TPCANBasic.GetValue(FPcanHandle, PCAN_ALLOW_ECHO_FRAMES, PLongWord(@iAllowEchoFrames),SizeOf(iAllowEchoFrames));

  if (stsResult = TPCANStatus.PCAN_ERROR_OK) then
  begin
    writeln('-----------------------------------------------------------------------------------------');
    writeln(Format('Get PCAN_ALLOW_ECHO_FRAMES: %s', [ConvertToParameterOnOff(iAllowEchoFrames)]));
    writeln('');
  end
  else
    ShowStatus(stsResult);
end;

procedure TGetSetParameter.SetPCAN_ALLOW_ECHO_FRAMES(value:Boolean);
var
  stsResult:TPCANStatus;
  iAllowEchoFrames: uint32;
begin
  if (value) then
    iAllowEchoFrames := TPCANBasic.PCAN_PARAMETER_ON
  else
    iAllowEchoFrames := TPCANBasic.PCAN_PARAMETER_OFF;
  stsResult := TPCANBasic.SetValue(FPcanHandle, PCAN_ALLOW_ECHO_FRAMES, PLongWord(@iAllowEchoFrames),SizeOf(iAllowEchoFrames));

  if (stsResult = TPCANStatus.PCAN_ERROR_OK) then
  begin
    writeln('-----------------------------------------------------------------------------------------');
    writeln(Format('Set PCAN_ALLOW_ECHO_FRAMES: %s', [ConvertToParameterOnOff(iAllowEchoFrames)]));
    writeln('');
  end
  else
    ShowStatus(stsResult);
end;
{$ENDREGION}

{$REGION 'PCAN_ACCEPTANCE_FILTER_11BIT'}
procedure TGetSetParameter.GetPCAN_ACCEPTANCE_FILTER_11BIT;
var
  stsResult:TPCANStatus;
  iacceptancefilter11bit: uint64;
begin
  stsResult := TPCANBasic.GetValue(FPcanHandle, PCAN_ACCEPTANCE_FILTER_11BIT, PLongWord(@iacceptancefilter11bit),SizeOf(iacceptancefilter11bit));

  if (stsResult = TPCANStatus.PCAN_ERROR_OK) then
  begin
    writeln('-----------------------------------------------------------------------------------------');
    writeln(Format('Get PCAN_ACCEPTANCE_FILTER_11BIT: %sh', [IntToHex(iacceptancefilter11bit,16)]));
    writeln('');
  end
  else
    ShowStatus(stsResult);
end;

procedure TGetSetParameter.SetPCAN_ACCEPTANCE_FILTER_11BIT(iacceptancefilter11bit:uint64);
var
  stsResult:TPCANStatus;
begin
  stsResult := TPCANBasic.SetValue(FPcanHandle, PCAN_ACCEPTANCE_FILTER_11BIT, PLongWord(@iacceptancefilter11bit),SizeOf(iacceptancefilter11bit));

  if (stsResult = TPCANStatus.PCAN_ERROR_OK) then
  begin
    writeln('-----------------------------------------------------------------------------------------');
    writeln(Format('Set PCAN_ACCEPTANCE_FILTER_11BIT: %sh', [IntToHex(iacceptancefilter11bit,16)]));
    writeln('');
  end
  else
    ShowStatus(stsResult);
end;
{$ENDREGION}

{$REGION 'PCAN_ACCEPTANCE_FILTER_29BIT'}
procedure TGetSetParameter.GetPCAN_ACCEPTANCE_FILTER_29BIT;
var
  stsResult:TPCANStatus;
  iAcceptanceFilter29Bit: uint64;
begin
  stsResult := TPCANBasic.GetValue(FPcanHandle, PCAN_ACCEPTANCE_FILTER_29BIT, PLongWord(@iAcceptanceFilter29Bit),SizeOf(iAcceptanceFilter29Bit));

  if (stsResult = TPCANStatus.PCAN_ERROR_OK) then
  begin
    writeln('-----------------------------------------------------------------------------------------');
    writeln(Format('Get PCAN_ACCEPTANCE_FILTER_29BIT: %sh', [IntToHex(iAcceptanceFilter29Bit,16)]));
    writeln('');
  end
  else
    ShowStatus(stsResult);
end;

procedure TGetSetParameter.SetPCAN_ACCEPTANCE_FILTER_29BIT(iacceptancefilter29bit:uint64);
var
  stsResult:TPCANStatus;
begin
  stsResult := TPCANBasic.SetValue(FPcanHandle, PCAN_ACCEPTANCE_FILTER_29BIT, PLongWord(@iacceptancefilter29bit),SizeOf(iacceptancefilter29bit));

  if (stsResult = TPCANStatus.PCAN_ERROR_OK) then
  begin
    writeln('-----------------------------------------------------------------------------------------');
    writeln(Format('Set PCAN_ACCEPTANCE_FILTER_29BIT: %sh', [IntToHex(iacceptancefilter29bit,16)]));
    writeln('');
  end
  else
    ShowStatus(stsResult);
end;
{$ENDREGION}

{$REGION 'PCAN_MESSAGE_FILTER'}
procedure TGetSetParameter.GetPCAN_MESSAGE_FILTER;
var
  stsResult:TPCANStatus;
  imessagefilter: uint32;
begin
  stsResult := TPCANBasic.GetValue(FPcanHandle, PCAN_MESSAGE_FILTER, PLongWord(@imessagefilter),SizeOf(imessagefilter));

  if (stsResult = TPCANStatus.PCAN_ERROR_OK) then
  begin
    writeln('-----------------------------------------------------------------------------------------');
    writeln(Format('Get PCAN_MESSAGE_FILTER: %s', [ConvertToFilterOpenCloseCustom(imessagefilter)]));
    writeln('');
  end
  else
    ShowStatus(stsResult);
end;

procedure TGetSetParameter.SetPCAN_MESSAGE_FILTER(imessagefilter:uint32);
var
  stsResult:TPCANStatus;
begin
  stsResult := TPCANBasic.SetValue(FPcanHandle, PCAN_MESSAGE_FILTER, PLongWord(@imessagefilter),SizeOf(imessagefilter));

  if (stsResult = TPCANStatus.PCAN_ERROR_OK) then
  begin
    writeln('-----------------------------------------------------------------------------------------');
    writeln(Format('Set PCAN_MESSAGE_FILTER: %s', [ConvertToFilterOpenCloseCustom(imessagefilter)]));
    writeln('');
  end
  else
    ShowStatus(stsResult);
end;
{$ENDREGION}

{$REGION 'PCAN_HARD_RESET_STATUS'}
procedure TGetSetParameter.GetPCAN_HARD_RESET_STATUS;
var
  stsResult:TPCANStatus;
  iHardResetStatus: uint32;
begin
  stsResult := TPCANBasic.GetValue(FPcanHandle, PCAN_HARD_RESET_STATUS, PLongWord(@iHardResetStatus),SizeOf(iHardResetStatus));

  if (stsResult = TPCANStatus.PCAN_ERROR_OK) then
  begin
    writeln('-----------------------------------------------------------------------------------------');
    writeln(Format('Get PCAN_HARD_RESET_STATUS: %s', [ConvertToParameterOnOff(iHardResetStatus)]));
    writeln('');
  end
  else
    ShowStatus(stsResult);
end;

procedure TGetSetParameter.SetPCAN_HARD_RESET_STATUS(value:Boolean);
var
  stsResult:TPCANStatus;
  iHardResetStatus: uint32;
begin
  if (value) then
    iHardResetStatus := TPCANBasic.PCAN_PARAMETER_ON
  else
    iHardResetStatus := TPCANBasic.PCAN_PARAMETER_OFF;
  stsResult := TPCANBasic.SetValue(FPcanHandle, PCAN_HARD_RESET_STATUS, PLongWord(@iHardResetStatus),SizeOf(iHardResetStatus));

  if (stsResult = TPCANStatus.PCAN_ERROR_OK) then
  begin
    writeln('-----------------------------------------------------------------------------------------');
    writeln(Format('Set PCAN_HARD_RESET_STATUS: %s', [ConvertToParameterOnOff(iHardResetStatus)]));
    writeln('');
  end
  else
    ShowStatus(stsResult);
end;
{$ENDREGION}

{$REGION 'PCAN_LAN_CHANNEL_DIRECTION'}
procedure TGetSetParameter.GetPCAN_LAN_CHANNEL_DIRECTION;
var
  stsResult:TPCANStatus;
  iChannelDirection: Integer;
begin
  stsResult := TPCANBasic.GetValue(FPcanHandle, PCAN_LAN_CHANNEL_DIRECTION, PLongWord(@iChannelDirection),SizeOf(iChannelDirection));

  if (stsResult = TPCANStatus.PCAN_ERROR_OK) then
  begin
    writeln('-----------------------------------------------------------------------------------------');
    writeln(Format('Get PCAN_LAN_CHANNEL_DIRECTION: %s', [ConvertToChannelDirection(iChannelDirection)]));
    writeln('');
  end
  else
    ShowStatus(stsResult);
end;
{$ENDREGION}
{$ENDREGION}

procedure TGetSetParameter.ShowConfigurationHelp;
begin
  writeln('==========================================================================================');
  writeln('|                           PCAN-Basic GetSetParameter Example                            |');
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

procedure TGetSetParameter.ShowCurrentConfiguration;
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

procedure TGetSetParameter.ShowStatus(status:TPCANStatus);
begin
  writeln('=========================================================================================');
  writeln(GetFormattedError(status));
  writeln('=========================================================================================');
end;

function TGetSetParameter.FormatChannelName(handle:TPCANHandle;isFD:Boolean):string;
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

function TGetSetParameter.GetTPCANHandleName(handle: TPCANHandle): string;
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

function TGetSetParameter.ConvertBitrateToString(bitrate: TPCANBaudrate): string;
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

function TGetSetParameter.ConvertToParameterOnOff(value:uint): string;
begin
  if ((value = TPCANBasic.PCAN_PARAMETER_OFF) or (value = TPCANBasic.PCAN_PARAMETER_ON)) then
  begin
    if value = TPCANBasic.PCAN_PARAMETER_OFF then
      Result:='PCAN_PARAMETER_OFF';
    if value = TPCANBasic.PCAN_PARAMETER_ON then
      Result:='PCAN_PARAMETER_ON';
  end
  else
    Result:='Status unknown: ' + IntToStr(value);
  Result := Result;
end;

function TGetSetParameter.ConvertToChannelDirection(value:uint): string;
begin
  if ((value >= TPCANBasic.LAN_DIRECTION_READ) and (value <= TPCANBasic.LAN_DIRECTION_READ_WRITE)) then
  begin
    if value = TPCANBasic.LAN_DIRECTION_READ then
      Result:='incoming only';
    if value = TPCANBasic.LAN_DIRECTION_WRITE then
      Result:='outgoing only';
    if value = TPCANBasic.LAN_DIRECTION_READ_WRITE then
      Result:='bidirectional';
  end
  else
    Result:=Format('undefined (0x%.4X)', [value]);
end;

function TGetSetParameter.ConvertToChannelFeatures(value:uint): string;
var
  sFeatures:string;
begin
  sFeatures := '';
  if ((value and TPCANBasic.FEATURE_FD_CAPABLE) = TPCANBasic.FEATURE_FD_CAPABLE) then
    sFeatures += 'FEATURE_FD_CAPABLE';
  if ((value and TPCANBasic.FEATURE_DELAY_CAPABLE) = TPCANBasic.FEATURE_DELAY_CAPABLE) then
    if (sFeatures <> '') then
      sFeatures += ', FEATURE_DELAY_CAPABLE'
    else
      sFeatures += 'FEATURE_DELAY_CAPABLE';
  if ((value and TPCANBasic.FEATURE_IO_CAPABLE) = TPCANBasic.FEATURE_IO_CAPABLE) then
    if (sFeatures <> '') then
      sFeatures += ', FEATURE_IO_CAPABLE'
    else
      sFeatures += 'FEATURE_IO_CAPABLE';
  Result := sFeatures;
end;

function TGetSetParameter.ConvertToChannelCondition(value:uint): string;
begin
  if ((value >= TPCANBasic.PCAN_CHANNEL_UNAVAILABLE) and (value <= TPCANBasic.PCAN_CHANNEL_PCANVIEW)) then
  begin
    if value = TPCANBasic.PCAN_CHANNEL_UNAVAILABLE then
      Result:='PCAN_CHANNEL_UNAVAILABLE';
    if value = TPCANBasic.PCAN_CHANNEL_AVAILABLE then
      Result:='PCAN_CHANNEL_AVAILABLE';
    if value = TPCANBasic.PCAN_CHANNEL_OCCUPIED then
      Result:='PCAN_CHANNEL_OCCUPIED';
    if value = TPCANBasic.PCAN_CHANNEL_PCANVIEW then
      Result:='PCAN_CHANNEL_PCANVIEW';
  end
  else
    Result:='Status unknown: ' + IntToStr(value);
  Result := Result;
end;

function TGetSetParameter.ConvertToFilterOpenCloseCustom(value:uint): string;
begin
  if ((value >= TPCANBasic.PCAN_FILTER_CLOSE) and (value <= TPCANBasic.PCAN_FILTER_CUSTOM)) then
  begin
    if value = TPCANBasic.PCAN_FILTER_CLOSE then
      Result:='PCAN_FILTER_CLOSE';
    if value = TPCANBasic.PCAN_FILTER_OPEN then
      Result:='PCAN_FILTER_OPEN';
    if value = TPCANBasic.PCAN_FILTER_CUSTOM then
      Result:='PCAN_FILTER_CUSTOM';
  end
  else
    Result:='Status unknown: ' + IntToStr(value);
  Result := Result;
end;

function TGetSetParameter.ConvertDeviceTypeToString(value:TPCANDevice): string;
begin
  if ((value >= TPCANDevice.PCAN_NONE) and (value <= TPCANDevice.PCAN_LAN)) then
  begin
    if value = TPCANDevice.PCAN_PEAKCAN then
      Result:='PCAN_PEAKCAN';
    if value = TPCANDevice.PCAN_DNG then
      Result:='PCAN_DNG';
    if value = TPCANDevice.PCAN_PCI then
      Result:='PCAN_PCI';
    if value = TPCANDevice.PCAN_USB then
      Result:='PCAN_USB';
    if value = TPCANDevice.PCAN_VIRTUAL then
      Result:='PCAN_VIRTUAL';
    if value = TPCANDevice.PCAN_LAN then
      Result:='PCAN_LAN';
  end
  else
    Result:='PCAN_NONE';
  Result := Result;
end;

function TGetSetParameter.GetFormattedError(error: TPCANStatus): string;
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

function TGetSetParameter.KeyPress: Word;
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

