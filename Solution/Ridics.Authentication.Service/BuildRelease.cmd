@ECHO OFF

SET PROJ_DIR=%~dp0
echo Using project directory: %PROJ_DIR%

if "%1" == "" goto no_config
if "%1" NEQ "" goto set_config

REM ASPNETCORE_ENVIRONMENT is also required for filtering appsettings.{environment}.json files

:set_config
SET ASPNETCORE_ENVIRONMENT=%1
GOTO run

:no_config
SET ASPNETCORE_ENVIRONMENT=Development
GOTO run

:run

dotnet build "%PROJ_DIR%Ridics.Authentication.Service.csproj" --configuration Release /p:PublishProfile=Release /p:EnvironmentName=%ASPNETCORE_ENVIRONMENT% /p:PackageLocation="%PROJ_DIR%bin\Publish-%ASPNETCORE_ENVIRONMENT%" /p:OutDir="%PROJ_DIR%bin\Publish-build" /p:DeployOnBuild=true /p:WebPublishMethod=Package /p:PackageAsSingleFile=true /maxcpucount:1 /p:platform="Any CPU" /p:configuration="Release" /p:DesktopBuildPackageLocation="%PROJ_DIR%bin\temp-package.zip" /p:DeployIisAppPath="Default Web Site/Auth"

copy "%PROJ_DIR%Deploy.AuthService.ps1" "%PROJ_DIR%bin\Publish-%ASPNETCORE_ENVIRONMENT%" /Y
