@ECHO OFF
SETLOCAL

REM ******* Version Changed ******
REM **
REM **	1.0.0	Ahern	[2016/04/12]
REM **		+ Allow blank password of user account
REM **		+ Calling PostBuildEventScript.cmd
REM **
REM **	1.0.1	Ahern	[2016/04/13]
REM **		\ Using "Administrator" instead of "%username%"
REM **		+ Enable administrator account
REM **		+ Dry-run mode
REM **
REM **	1.0.2	Ahern	[2016/04/15]
REM **		\ Reorder dry-run
REM **
REM **	1.0.3	Ahern	[2016/07/19]
REM **		+ Hide Administrator account
REM **
REM **	1.0.4	Ahern	[2016/07/23]
REM **		\ Replace " " with "$" of passing-in parameters
REM **	1.0.5	Jay		[2017/08/16]
REM **		+ 增加產生測試批次檔，用於測試無法正常運作的情形		
REM ******************************

REM ***** Passing Parameters *****
REM **
REM **	%1	Project target folder. e.g. D:\CAMPro\CtLib\bin\Debug\
REM ** 	%2	Solution target file. e.g. D:\CAMPro\Huawei\bin\Debug\Huawei.exe
REM ** 	%3	Executable file. e.g. D:\CAMPro\CtLib\bin\Release\CtLib.exe
REM ** 	%4	XML file. e.g. D:\CAMPro\CtLib\bin\Release\CtLib.xml
REM **
REM ******************************

REM Set parameters to local variables
SET "SOLBIN=%~1"
SET "SOLTAR=%~2"
SET "PRJTAR=%~3"
SET "PRJXML=%~4"

REM Replace " " with "$". To make path without space, the "RUNAS" couldn't accept double quotes
SET "SOLBIN=%SOLBIN: =$%"
SET "SOLTAR=%SOLTAR: =$%"
SET "PRJTAR=%PRJTAR: =$%"
SET "PRJXML=%PRJXML: =$%"

CALL :CreateTestBat

REM If there are passing-in parameters (run script by VisualStudio) then execute PostBuildEventScript.cmd
IF "%~1" NEQ "" (
	IF "%~2" NEQ "" (
		IF "%~3" NEQ "" (

			REM Using Administrator rights to execute batch file
			RUNAS /ENV /USER:Administrator /SAVECRED ""%~dp0PostBuildEventScript.cmd" %SOLBIN% %SOLTAR% %PRJTAR% %PRJXML%"
			GOTO Exception
		) ELSE (GOTO DryRun)
	) ELSE (GOTO DryRun)
) ELSE (GOTO DryRun)

:Exception
ECHO Script Finished
PAUSE
EXIT

:DryRun
CLS

REM Check wheter user execute this script with administrator rights
CHOICE /T 30 /C YN /D N /M "Run script as Administrator?  (Y)Yes,execute dry-run (N)exit script  "
IF %ERRORLEVEL% EQU 2 (GOTO Exception)

CLS
ECHO.
ECHO Allowing blank password with user accounts...
ECHO.

REM Allow user account allow blank password
REG ADD "HKLM\SYSTEM\CurrentControlSet\Control\Lsa" /V "LimitBlankPasswordUse" /D "0x00000000" /T REG_DWORD /F

ECHO.
ECHO Enabling Administrator account...
ECHO.

REM Enable Administrator account, it have the right to access %programfiles%
NET USER Administrator /ACTIVE:YES

ECHO.
ECHO Try executing DirectX diagnostics...
ECHO.

REM Trying to execute diagnostics, it will point out Administrator is work or not
RUNAS /ENV /USER:Administrator /SAVECRED dxdiag

ECHO.
CHOICE /T 30 /C YN /D N /M "Hide Administrator account on Windows Login screen?  (Y)Yes,hide it (N)keep show  "
IF %ERRORLEVEL% EQU 2 (GOTO DryRunFinish)
ECHO.

REM Hide Administrator account
REG ADD "HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon\SpecialAccounts\UserList" /V "Administrator" /D "0x00000000" /T REG_DWORD /F

:DryRunFinish
ECHO.
ECHO Dry-run finished, press any key to exit script
PAUSE > NUL
EXIT

REM產生測試用批次檔
:CreateTestBat
SET Log=D:\CtLibBatTest.bat
IF EXIST "%Log%" DEL /f "%Log%"
@ECHO SET SolBin=%SOLBIN% >> "%Log%"
@ECHO SET SolTar=%SOLTAR% >> "%Log%"
@ECHO SET PrjTar=%PRJTAR% >> "%Log%"
@ECHO SET PrjXml=%PRJXML% >> "%Log%"
@ECHO.>>"%Log%"
@ECHO CALL "%~dp0RunScriptAsAdmin.cmd" %%SolBin%% %%SolTar%% %%PrjTar%% %%PrjXml%% >>  "%Log%"
@ECHO PAUSE >> "%Log%"
