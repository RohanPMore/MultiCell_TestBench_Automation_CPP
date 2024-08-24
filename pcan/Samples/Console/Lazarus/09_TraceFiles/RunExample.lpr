program RunExample;

{$mode objfpc}{$H+}
uses
  TraceFiles;

var
  Application: TTraceFiles;
begin
  Application:=TTraceFiles.Create(nil);
  Application.Title:='TraceFiles';
  Application.Start;
  Application.Free;
end.
