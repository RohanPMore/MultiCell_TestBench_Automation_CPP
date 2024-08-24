program RunExample;

{$mode objfpc}{$H+}
uses
  EventDrivenRead;

var
  Application: TEventDrivenRead;
begin
  Application:=TEventDrivenRead.Create(nil);
  Application.Title:='08_EventDrivenRead';
  Application.Start;
  Application.Free;
end.

