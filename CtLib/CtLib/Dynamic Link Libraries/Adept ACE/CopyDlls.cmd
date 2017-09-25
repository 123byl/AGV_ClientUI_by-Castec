@ECHO OFF
SETLOCAL

REM ******* Version Changed ******
REM **
REM **	1.0.0	Ahern	[2016/11/02]
REM **		+ Initial Pilot
REM **
REM ******************************

REM Search %programfiles% and get Ace.exe location
SET ACEPATH=""
FOR /F "USEBACKQ DELIMS== TOKENS=1" %%i IN (`WHERE /R "%programfiles%" Ace.exe`) DO SET ACEPATH=%%~dpi
IF "%ACEPATH%"=="" GOTO Failed
ECHO.
ECHO Adept ACE found : %ACEPATH%
GOTO CopyFiles

:CopyFiles
ECHO.
ECHO Copying files...
REM Copy Adept ACE dynamic linked libraries
COPY /Y "%ACEPATH%Ace.*.Common.dll" "%~dp0" > NUL
COPY /Y "%ACEPATH%Ace.*.Common.xml" "%~dp0" > NUL
COPY /Y "%ACEPATH%Ace.Adept.dll" "%~dp0" > NUL
COPY /Y "%ACEPATH%Ace.Adept.xml" "%~dp0" > NUL
COPY /Y "%ACEPATH%Ace.Core.dll" "%~dp0" > NUL
COPY /Y "%ACEPATH%Ace.Core.xml" "%~dp0" > NUL
COPY /Y "%ACEPATH%AxInterop.HSDISPLAYLib.dll" "%~dp0" > NUL
GOTO Success

:Success
ECHO.
ECHO Finished. Press any key to exit script
PAUSE > NUL
EXIT

:Failed
ECHO.
ECHO There are no Adept ACE installed, downloading default version
ECHO.
ECHO Please enter the account information of SWServer for login...
ECHO.
SET usr=
SET /P usr=User Name : 
NET USE Z: "\\192.168.10.23\SWserver\Version_Control\CAMPro\CtLib_cs\Adept ACE DLL" /USER:%usr%
COPY /Y "Z:\\*" "%~dp0" > NUL
NET USE Z: /DELETE > NUL
ECHO Finished. Press any key to exit script
PAUSE > NUL
EXIT