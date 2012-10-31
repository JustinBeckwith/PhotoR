@echo off

:: Specify project file
IF "%PROJECT%" == "" (
   SET PROJECT=PhotoR\PhotoR.csproj
)

IF "%PROJECT%" == "PROJECTFILEGOESHERE" goto MissingProject

:: Specify project configuration
SET CONFIG=Release
IF "%CONFIG%" == "" (
  SET CONFIG = Release
)

SET ARTIFACTS=%~dp0%artifacts

IF NOT DEFINED DEPLOYMENT_TARGET (
  SET DEPLOYMENT_TARGET=%ARTIFACTS%\wwwroot
)

IF NOT DEFINED DEPLOYMENT_SOURCE (
  SET DEPLOYMENT_SOURCE=%~dp0%
)

IF NOT DEFINED DEPLOYMENT_TEMP (
  SET DEPLOYMENT_TEMP=%ARTIFACTS%\temp
)

:: Specify the test dependencies
::SET TESTPROJECTNAME=DeploymentScriptDemo.Tests
::SET TESTPROJECT=%TESTPROJECTNAME%\%TESTPROJECTNAME%.csproj
::SET TESTDLL=%DEPLOYMENT_SOURCE%%TESTPROJECTNAME%\bin\%CONFIG%\%TESTPROJECTNAME%.dll

:: Build the test project
::echo Building test project
::%WINDIR%\Microsoft.NET\Framework\v4.0.30319\MSBuild %TESTPROJECT% /v:minimal /p:Configuration=%CONFIG% /p:SolutionDir=%DEPLOYMENT_SOURCE%
::if ERRORLEVEL 1 exit /b 1

:: Run tests
:: Download xunit
::echo Running tests
::%DEPLOYMENT_SOURCE%.nuget\NuGet.exe install -e xunit.runners -o tools
:: Run xunit
::%DEPLOYMENT_SOURCE%tools\xunit.runners\tools\xunit.console.clr4.exe %TESTDLL%
::if ERRORLEVEL 1 exit /b 1

:: Copy the project artifacts into temp
echo Building %PROJECT%...
%WINDIR%\Microsoft.NET\Framework\v4.0.30319\MSBuild %PROJECT% /v:minimal /t:pipelinePreDeployCopyAllFilesToOneFolder /p:Configuration=%CONFIG% /p:SolutionDir=%DEPLOYMENT_SOURCE% /p:_PackageTempDir="%DEPLOYMENT_TEMP%" /p:AutoParameterizationWebConfigConnectionStrings=false
if ERRORLEVEL 1 exit /b 1

:: Copy the artifacts to the target
echo Copying files to from '%DEPLOYMENT_TEMP%' to '%DEPLOYMENT_TARGET%'
xcopy "%DEPLOYMENT_TEMP%" "%DEPLOYMENT_TARGET%" /Y /Q /E
echo DEPLOY USING CUSTOM BATCH FILE COMPLETE!!!1

exit /b 0

:MissingProject
echo The target project (PROJECT) was not specifed
exit /b 1