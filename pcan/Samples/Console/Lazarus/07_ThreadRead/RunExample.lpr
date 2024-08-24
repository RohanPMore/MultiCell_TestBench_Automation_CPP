program RunExample;

{$mode objfpc}{$H+}

uses
  ThreadRead;

var
  Application: TThreadRead;
begin
  Application:=TThreadRead.Create(nil);
  Application.Title:='07_ThreadRead';
  Application.Start;
  Application.Free;
end.

