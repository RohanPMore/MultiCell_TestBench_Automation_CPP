program RunExample;

{$mode objfpc}{$H+}

uses
  GetSetParameter;

var
  Application: TGetSetParameter;
begin
  Application:=TGetSetParameter.Create(nil);
  Application.Title:='02_GetSetParameter';
  Application.Start;
  Application.Free;
end.

