Param(
    [string]$fileName
)

[XML]$val = Get-Content $fileName
$propertyGroupNode = $val | Select-Xml -XPath "//Project//PropertyGroup" 
$properties = $propertyGroupNode.Node
Write-Output $properties.Version