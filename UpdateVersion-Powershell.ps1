Param(
    [string]$fileName,
    [string]$version
)

[XML]$val = Get-Content $fileName
$propertyGroupNode = $val | Select-Xml -XPath "//Project//PropertyGroup" 
$properties = $propertyGroupNode.Node
Write-Output "Updating $fileName to $version..."
$currentVersion = $properties.Version
Write-Output "Current Version: $currentVersion"
$properties.Version = $version
$properties.AssemblyVersion = $version
$properties.FileVersion = $version

$currentVersion = $properties.Version
Write-Output "Updating version to $currentVersion"

$val.Save("$fileName")
Write-Output "Updating $fileName to $version successful!"