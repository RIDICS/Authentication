# Authentication

The Authentication service is configured primarly for usage in *Vokabulář webový*, so it has some predefined values for this usage.

The Authentication service project is consists of the three important parts:
* Ridics.Authentication.Service - the server app, the authentication service
* Ridics.Authentication.Database.Migrator - the app for creating or updating database schema including data migrations
* Ridics.Authentication.HttpClient - the client for communication with Authentication Service API (NuGet package is generated)

## Development

The configuration for local Development is prepared directly in the solution.

### Required software:
* Microsoft Windows 10
* Microsoft Visual Studio 2017
  * ASP.NET and web development
  * .NET Core 2.2 SDK
* Yarn
* Git - for restoring Yarn dependencies
* Node.js - for restoring Yarn dependencies
* SQL Server or PostgreSQL 11

### Project initialization

> It is highly recommended to disable NPM and Bower restore in Visual Studio and use Yarn instead of it.

> Development environment is using app setting files with name "LocalDebug", e.g. `appsettings.LocalDebug.json` or `modules.LocalDebug.json`

* Checkout project from git
* Restore Node packages using Yarn (`YarnInstall.ps1` in root folder)
* Open solution in Visual Studio
* Run Ridics.Authentication.Database.Migrator project to create default database for `LocalDebug` environment
* Run Ridics.Authentication.Service to run Authentication Service with `LocalDebug` configuration
* Default username is "Admin" with password "administrator" or "PortalAdmin" (reduced permissions) with password "administrator"

## Deployment to the server

Deployment to Production or Staging server requires creating specific configurations. The app configuration can be placed in `C:\Pool\itjakub-secrets\Auth` and `C:\Pool\itjakub-secrets\DatabaseMigratorAuth` folder on build computer. The configuration is separated to avoid commiting sensitive files to git. The files in this folder are included to publish package during build.

Default deployment script assumes that the Authentication Service will be placed in `Default Web Site/Auth` site, e.g. `https://localhost/Auth`.

* Choose the environment name, e.g. Production (the name should be the same as will be used for Vokabulář deployment project - ITJakub)
* Create all configuration files for specified environment according to https://github.com/RIDICS/itjakub-secrets
  * e.g. `modules.Production.json` and so on
* Fill correct values to these files

### Requirements for sending notifications
* SMTP server for sending notifications (two-factor login, forgotten password, ...)
* SMS gateway for sending notifications (two-factor login)

> The Authentication service is using `IocComponentsRegistrationExtensions.RegisterMessageSenders()` method for registering component able to sending some notification messages. Defaultly there are registered `NullSmsSender` and `NullEmailSender` which discards all messages.
> * Sending e-mails can be enabled by registering component `SmtpEmailSender` instead of `NullEmailSender`
> * There is no implementation for sending SMS because each gateway have different API

### Required software:
* Microsoft Windows Server 2012 R2 +
* SQL Server or PostgreSQL 11
* IIS
* .NET Core 2.2 Runtime & Hosting bundle
  * Maybe run `IISRESET` command will be required
* Web Platform Installer (WebPI)
* Web Deploy (MsDeploy) from WebPI

### Configure the server
* Execute `GenerateSigningCertificate.ps1 {ENVIRONMENT_NAME}` in Solution/Ridics.Authentication.Service, fill password, copy to deploy server e.g. into `C:\intehub\certs\`. Configure `IdentityServer` in `appsettings.{ENVIRONMENT_NAME}.json`
* Create new Application Pool in IIS
  * .NET CLR set to "No Managed Code"
  * In Advanced Settings change `Load User Profile` to `True` (required for storing loaded certificates)
* Deploy Authentication Service using steps from "Deployment" section
* Change used Application Pool to the one created in previous steps
* Create file `modules-autogenerated.xml` with content `<configuration></configuration>` in {AUTH_SERVICE_FOLDER}
  * Add permission for user IIS_IUSRS to write to this file
* Add `logs` folder to {AUTH_SERVICE_FOLDER}
  * Add permission for user IIS_IUSRS to write to this folder

### Deployment
* Run `DeleteObjBinFolders.ps1` script to allow perform a clean build (optional but recommended step).
* Build solution with command line script `BuildSolution.ps1 {ENVIRONMENT_NAME}`. Environment names are Development, Production, etc.
* Copy build artifacts from `build\Publish-{ENVIRONMENT_NAME}` to target server.
  * Don't rename the Publish folder.
* Run database migration using `Ridics.Authentication.Database.Migrator\Migrate.ps1 {ENVIRONMENT_NAME}` to update database.
* Deploy Authentication service as **Admin** using `Deploy.AuthService.cmd`.

### The app configuration

This configuration can be performed in GUI after login:

* **It is required to change passwords for default users**
* Configure clients which can use this Authentication Service
  * Some values are preconfigured primarly for Development purposes
  * Remove all not valid clients

## Troubleshooting

* Not working login to Auth service on linux with application in insecure mode using `$env:ASPNETCORE_DISABLE_HTTPS_REDIRECT=true`  
  * Solution: Clear storage in Chrome in Application tab of the developer tools  

* Problem: YarnInstall (yarn) can not fetch/download packages
  * Solution: Try to disable your firewall and rerun script.

* Problem: Unable to start application in IIS.
  * Check if ".NET Core Windows Server Hosting" is installed.
  * Enable logging for additional info.

* Problem: Logging doesn't work on IIS.
  * System user IIS_IUSRS must has permission to write to "logs" folder.
  * If app crash during start, additional logging can be enabled in web.config file by changing stdoutLogEnabled="false" attribute to true.

* Problem: (IIS Express) Unable to start process dotnet.exe. The web server request failed with status code 403.
  * Solution: Enable SSL for the project and set `applicationUrl` in `launchSettings.json` to use HTTPS version.

* Problem: Load certificate from disk using X509Certificate2 constructor throws CryptographicException: Access denied (probably in IIS)
  * Solution: Ensure that the X509KeyStorageFlags.UserKeySet flag is specified in constructor and set "Load User Profile" to True in Application Pool > Advanced Settings.

* Problem: Logging doesn't work - no logs are appended to the log file.
  * There exists two logging configurations, one for Microsoft Logging abstraction in appsettings.json and second for logging library NLog in NLog.config file. Both configurations are applied (as logical AND).

* Problem: Communication with auth service failed with status 405.
  * Solution: Ensure that `Common HTTP Features > WebDAV Publishing` is not installed in IIS.

* Problem: Automatic logout from clients when user signs out from auth service does not work
  * Solution: Check if every client has only one frontchannel logout uri specified, if there are more frontchannel logout uris for one client automatic logout wont work properly. 

* Problem: Application loads first page slowly when deployed to IIS (app is idle after some period of time).
  * Solution: Change Application Pool settings: `Start Mode` to `AlwaysRunning` and `Idle Time-out (minutes)` to `0`.

* Problem: Unable to login on production server if deployed behind proxy server (infinite redirect between WebHub and Authentication service).
  * Solution: Ensure that time on all servers is synchronized and therefore the .AspNetCore.Correlation.OpenIdConnect cookie doesn't expire before sending HTTP response (the time specified in "Date" HTTP header).

* Problem: Unable to start ASP.NET Core application InProcess in IIS.
  * Solution: All our apps and services should be configured to run OutOfProcess. InProcess hosting model is not tested and requires to every app run in own Application Pool.

* Problem: Auth service failed to start with `InvalidOperationException: No service for type 'LiveManager' has been registered`.
  * Solution: Very likely some exception was thrown during `ConfigureServices()` method in `Startup` class. These exceptions are not propagated to `Program` class, so this exception is not logged.

