@ECHO OFF
SETLOCAL

REM ******* Version Changed ******
REM **
REM **	1.0.0	Ahern	[2016/04/12]
REM **		+ Define Adept ACE path and copy files
REM **		+ Copy multi-language resource files
REM **
REM **	1.0.1	Ahern	[2016/04/13]
REM **		\ Simplify copies
REM **
REM **	1.0.2	Ahern	[2016/04/15]
REM **		+ %2 using to wait building that copy fully resources files
REM **
REM **	1.0.3	Ahern	[2016/06/22]
REM **		+ Ax*.dll, Interop.*.dll
REM **
REM **	1.0.4	Ahern	[2016/07/05]
REM **		+ Ace.*.Common.dll & xml
REM **		\ Hide copy information
REM **
REM **	1.0.5	Ahern	[2016/07/23]
REM	**		\ Replace "&" with " " of passing-in parameters
REM ** 
REM ******************************

REM ***** Passing Parameters *****
REM **
REM ** %1	Project target folder. e.g. D:\CAMPro\CtLib\bin\Debug\
REM ** %2	Solution target file. e.g. D:\CAMPro\Huawei\bin\Debug\Huawei.exe
REM ** %3	Executable file. e.g. D:\CAMPro\CtLib\bin\Release\CtLib.exe
REM ** %4	XML file. e.g. D:\CAMPro\CtLib\bin\Release\CtLib.xml
REM **
REM ******************************

REM Set parameters to local variables
SET "SOLBIN=%~1"
SET "SOLTAR=%~2"
SET "PRJTAR=%~3"
SET "PRJXML=%~4"

REM Replace "$" with " ". Parsing to path
SET "SOLBIN=%SOLBIN:$= %"
SET "SOLTAR=%SOLTAR:$= %"
SET "PRJTAR=%PRJTAR:$= %"
SET "PRJXML=%PRJXML:$= %"

REM Search %programfiles% and get Ace.exe location
SET ACEPATH=""
FOR /F "USEBACKQ DELIMS== TOKENS=1" %%i IN (`WHERE /R "%programfiles%" Ace.exe`) DO SET ACEPATH=%%~dpi
IF "%ACEPATH%"=="" GOTO Failed

ECHO Omron ACE found: "%ACEPATH%"
ECHO.
ECHO Waiting "%SOLTAR%"

:WaitBuild
REM Wait solution building finished
IF NOT EXIST "%SOLTAR%" (
	TIMEOUT /T 1 /NOBREAK > NUL
	GOTO WaitBuild
) ELSE (
	TIMEOUT /T 1 /NOBREAK > NUL
	GOTO CopyFiles
)

:CopyFiles
ECHO.
ECHO Copying files...
ECHO.
REM Copy Adept ACE dynamic linked libraries
COPY /Y "%ACEPATH%Ace.*.Common.dll" "%SOLBIN%" > NUL
COPY /Y "%ACEPATH%Ace.*.Common.xml" "%SOLBIN%" > NUL
COPY /Y "%ACEPATH%Ace.Adept.dll" "%SOLBIN%" > NUL
COPY /Y "%ACEPATH%Ace.Adept.xml" "%SOLBIN%" > NUL
COPY /Y "%ACEPATH%Ace.Core.dll" "%SOLBIN%" > NUL
COPY /Y "%ACEPATH%Ace.Core.xml" "%SOLBIN%" > NUL
COPY /Y "%ACEPATH%Ace.AdeptSight.dll" "%SOLBIN%" > NUL
COPY /Y "%ACEPATH%Ace.Feeder.dll" "%SOLBIN%" > NUL
COPY /Y "%ACEPATH%Ace.HSVision.dll" "%SOLBIN%" > NUL
COPY /Y "%ACEPATH%Ace.ProcessManager.dll" "%SOLBIN%" > NUL
COPY /Y "%ACEPATH%Ace.UIBuilder.dll" "%SOLBIN%" > NUL
COPY /Y "%ACEPATH%ActiproSoftware*.dll" "%SOLBIN%" > NUL
COPY /Y "%ACEPATH%AxGlgoleLib.dll" "%SOLBIN%" > NUL
COPY /Y "%ACEPATH%AxInterop.HSDISPLAYLib.dll" "%SOLBIN%" > NUL
COPY /Y "%ACEPATH%BidirectionalTCP.dll" "%SOLBIN%" > NUL
COPY /Y "%ACEPATH%DotNetMagic2005.dll" "%SOLBIN%" > NUL
COPY /Y "%ACEPATH%Interop.HSCLASSLIBRARYLib.dll" "%SOLBIN%" > NUL
COPY /Y "%ACEPATH%Interop.HSDISPLAYLib.dll" "%SOLBIN%" > NUL
IF EXIST "%ACEPATH%ActiveV" (XCOPY "%ACEPATH%ActiveV" "%SOLBIN%ActiveV" /K /E /Y /C /I /H /D) > NUL
IF EXIST "%ACEPATH%HexSight\Controls" (XCOPY "%ACEPATH%HexSight\Controls" "%SOLBIN%HexSight\Controls" /K /E /Y /C /I /H /D) > NUL

REM Copy our libraries to ACE bin folder
COPY /Y "%PRJTAR%" "%ACEPATH%" > NUL
COPY /Y "%PRJXML%" "%ACEPATH%" > NUL

REM Copy Adept ACE resources to ourself
XCOPY "%ACEPATH%de" "%SOLBIN%en-US" /K /E /Y /C /I /H /D > NUL
XCOPY "%ACEPATH%de" "%SOLBIN%zh-TW" /K /E /Y /C /I /H /D > NUL
XCOPY "%ACEPATH%zh" "%SOLBIN%zh-CN" /K /E /Y /C /I /H /D > NUL

REM Copy resources to multi-language
XCOPY "%SOLBIN%en-US" "%SOLBIN%de" /K /E /Y /C /I /H /D > NUL
XCOPY "%SOLBIN%en-US" "%SOLBIN%fr" /K /E /Y /C /I /H /D > NUL
XCOPY "%SOLBIN%en-US" "%SOLBIN%ja" /K /E /Y /C /I /H /D > NUL
XCOPY "%SOLBIN%zh-TW" "%SOLBIN%zh-CHT" /K /E /Y /C /I /H /D > NUL
XCOPY "%SOLBIN%zh-TW" "%SOLBIN%zh-Hant" /K /E /Y /C /I /H /D > NUL
XCOPY "%SOLBIN%zh-CN" "%SOLBIN%zh" /K /E /Y /C /I /H /D > NUL
XCOPY "%SOLBIN%zh-CN" "%SOLBIN%zh-CHS" /K /E /Y /C /I /H /D > NUL
XCOPY "%SOLBIN%zh-CN" "%SOLBIN%zh-Hans" /K /E /Y /C /I /H /D > NUL

REM Finished
GOTO Success

:Success
ECHO.
ECHO Finished
PAUSE
EXIT

:Failed
ECHO.
ECHO There are no Adept ACE installed
PAUSE
EXIT