program RunExample;

{$mode objfpc}{$H+}

uses
  ManualRead;

var
  Application: TManualRead;
begin
  Application:=TManualRead.Create(nil);
  Application.Title:='03_ManualRead';
  Application.Start;
  Application.Free;
end.
