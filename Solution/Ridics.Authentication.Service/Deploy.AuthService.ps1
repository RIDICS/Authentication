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