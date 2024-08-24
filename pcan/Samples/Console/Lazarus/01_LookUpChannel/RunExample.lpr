program RunExample;

{$mode objfpc}{$H+}

uses
  LookUpChannel;

var
  Application: TLookUpChannel;
begin
  Application:=TLookUpChannel.Create(nil);
  Application.Title:='01_LookUpChannel';
  Application.Start;
  Application.Free;
end.

