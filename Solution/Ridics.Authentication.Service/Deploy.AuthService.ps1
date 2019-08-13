#!/usr/bin/env pwsh

$CurrentPath = (Get-Location -PSProvider FileSystem).ProviderPath

Write-Host
Write-Host "Using root directory: ${CurrentPath}"
Write-Host
Write-Host

$ProjectToDeploy = "Ridics.Authentication.Service"
$DeployScriptPath = Join-Path $CurrentPath "${ProjectToDeploy}.deploy.cmd"

if (Test-Path $DeployScriptPath)
{
    Write-Host "${ProjectToDeploy} FOUND" -foregroundcolor green
}
else
{
    Write-Host "${ProjectToDeploy} NOT FOUND" -foregroundcolor red
    exit 1
}

$MigrationToRun = "Ridics.Authentication.Database.Migrator"
$MigrationScriptPath = Join-Path $CurrentPath "${MigrationToRun}\Migrate.ps1"

if (Test-Path $MigrationScriptPath)
{
  Write-Host "${MigrationToRun} FOUND" -foregroundcolor green
}
else {
  Write-Host
  Write-Host "Migrator project for running migrations was not found" -foregroundcolor red
  Write-Host "Deployment cancelled" -foregroundcolor red
  Write-Host
  exit 1
}

Write-Host
Write-Host

$folderName = (Get-Item $CurrentPath).Name
$TargetEnvironment = $folderName.Split('-')[1]

$migratorPath = Join-Path $CurrentPath "${MigrationToRun}"

Set-Location $migratorPath
& $MigrationScriptPath ${TargetEnvironment}

Set-Location $CurrentPath

if ($LASTEXITCODE -ne 0)
{
  Write-Error "Mirgrations ${MigrationToRun} failed"
  exit 1
}


Write-Host
Write-Host "Starting deployment"
Write-Host

$AdditionalArguments = "/y"
& $DeployScriptPath -enableRule:DoNotDeleteRule -enablerule:AppOffline $AdditionalArguments

Write-Host "Creating logs folders"
Write-Host

$DefaultWebSitePath = Get-WebFilePath 'IIS:\Sites\Default Web Site'
$AuthServiceLogsPath = Join-Path $DefaultWebSitePath "Auth\logs"
 
if (-Not (Test-Path $AuthServiceLogsPath))
{
    New-Item -Path $AuthServiceLogsPath -ItemType "directory" > $null
}
function SetFullAccessToFolder {
    Param ([String]$FolderPath)

    $Acl = Get-Acl $FolderPath 

    $AccessRule = New-Object System.Security.AccessControl.FileSystemAccessRule("IIS_IUSRS", "FullControl", "ContainerInherit,ObjectInherit", "None", "Allow")

    $Acl.SetAccessRule($AccessRule)
    Set-Acl $FolderPath $Acl
}

SetFullAccessToFolder($AuthServiceLogsPath)

Write-Host "Logs folders created"
Write-Host

Write-Host
Write-Host "DEPLOYMENT FINISHED"
Write-Host