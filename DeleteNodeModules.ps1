#!/usr/bin/env pwsh

$CurrentPath = (Get-Location -PSProvider FileSystem).ProviderPath

$FoldersToRemove = Get-ChildItem .\ -include node_modules -Recurse -ErrorAction Continue | where {$_ -notlike '*\node_modules\*\node_modules*'} | foreach {$_.fullname}

$RemovedCount = 0
$FailedCount = 0

if($FoldersToRemove -ne $null)
{			
    Write-Host 
	foreach ($item in $FoldersToRemove) 
	{
		try
		{
			$linkItems = Get-ChildItem $item | Where-Object { $_.Attributes -match "ReparsePoint" };
			foreach ($linkItem in $linkItems)
			{
				$removeCommand = "rmdir " + $linkItem.FullName
				cmd /c $removeCommand
				Write-Host "Removed SymbolicLink/Junction:" $linkItem
			}
			
			remove-item $item -Force -Recurse -ErrorAction Stop;
			
			Write-Host "Removed: ." -nonewline;
			Write-Host $item.replace($CurrentPath, "");
			$RemovedCount++
		}
		catch
		{
			Write-Host "FAILED: ." -nonewline;
			Write-Host $item.replace($CurrentPath, "");
			$FailedCount++
		}
	}
}

Write-Host

if($FoldersToRemove -ne $null)
{
	Write-Host $RemovedCount "folders removed," $FailedCount "failed" -foregroundcolor green
}
else
{
 	Write-Host "No folders to remove" -foregroundcolor green
}	

Write-Host "Folders with path longer than 260 characters are probably not removed."

Write-Host
