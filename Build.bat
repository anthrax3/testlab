@echo off

set fdir=%WINDIR%\Microsoft.NET\Framework64

if not exist %fdir% (
	set fdir=%WINDIR%\Microsoft.NET\Framework
)

set msbuild=%fdir%\v4.0.30319\msbuild.exe

%msbuild% testlab.sln /p:Configuration=Debug /t:Clean;Rebuild


pause