#!/usr/bin/env pwsh

$CurrentPath = (Get-Location -PSProvider FileSystem).ProviderPath

Write-Host
Write-Host "Using root directory: ${CurrentPath}"
Write-Host
Write-Host

# Test paths

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

# DB Migrator

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

# App deployment to IIS

Write-Host
Write-Host "Starting deployment"
Write-Host

$AdditionalArguments = "/y"
& $DeployScriptPath -enableRule:DoNotDeleteRule -enablerule:AppOffline $AdditionalArguments

# Add logs folder

Write-Host "Checking if logs folders must be created"
Write-Host

$DefaultWebSitePath = Get-WebFilePath 'IIS:\Sites\Default Web Site'
$AuthServiceLogsPath = Join-Path $DefaultWebSitePath "Auth\logs"
 
function SetAccessToItem {
    Param ([String]$itemPath, [Object]$accessRule)

    $Acl = Get-Acl $itemPath 
    $Acl.SetAccessRule($accessRule)
    Set-Acl $itemPath $Acl
}

if (-Not (Test-Path $AuthServiceLogsPath))
{
    Write-Host "Creating new logs folder"

    New-Item -Path $AuthServiceLogsPath -ItemType "directory" > $null

    $FolderAccessRule = New-Object System.Security.AccessControl.FileSystemAccessRule("IIS_IUSRS", "FullControl", "ContainerInherit,ObjectInherit", "None", "Allow")
    SetAccessToItem $AuthServiceLogsPath -accessRule $FolderAccessRule

    Write-Host "Logs folders created"
}

# Add modules-autogenerated.xml file

Write-Host
Write-Host "Checking if modules-autogenerated.xml file must be created"
Write-Host

$ModulesAutogeneratedPath = Join-Path $DefaultWebSitePath "Auth\modules-autogenerated.xml"
 
if (-Not (Test-Path $ModulesAutogeneratedPath))
{
    Write-Host "Creating a new file modules-autogenerated.xml"
    New-Item -Path $ModulesAutogeneratedPath -ItemType "file" -Value "<configuration></configuration>" > $null

    $FileAccessRule = New-Object System.Security.AccessControl.FileSystemAccessRule("IIS_IUSRS", "FullControl", "Allow")
    SetAccessToItem $ModulesAutogeneratedPath -accessRule $FileAccessRule

    Write-Host "File modules-autogenerated.xml created"
}

Write-Host

Write-Host
Write-Host "DEPLOYMENT FINISHED"
Write-Host