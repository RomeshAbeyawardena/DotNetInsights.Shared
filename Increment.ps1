$solutionProperties = "$PSScriptRoot\Directory.Build.props"

$getVersionScriptPath = "$PSScriptRoot\.\Get-Version.ps1"

$incrementVersionScriptPath = "$PSScriptRoot\.\Increment-Version.ps1"

$updateVersionScriptPath = "$PSScriptRoot\.\UpdateVersion-Powershell.ps1"

$dotnetPackScript = "$PSScriptRoot\.\DotNet-Pack.ps1"

$currentVersion = & $getVersionScriptPath -fileName $solutionProperties

$newVersion = & $incrementVersionScriptPath -versionString $currentVersion -versionPartIndex 3

& $dotnetPackScript -version $newVersion -output "C:\Source\packages\nuget"