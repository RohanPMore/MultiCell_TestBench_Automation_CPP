program RunExample;

{$mode objfpc}{$H+}

uses
  ManualWrite;

var
  Application: TManualWrite;
begin
  Application:=TManualWrite.Create(nil);
  Application.Title:='04_ManualWrite';
  Application.Start;
  Application.Free;
end.
