Param (
    [string]$versionString = '1.0.0.0',
    [int]$versionPartIndex = 3
)

[int]$versionPartInt = 0
$versionParts = $versionString.Split('.')
$versionPartsGroup = New-Object System.Collections.ArrayList
Foreach($versionPart in $versionParts){
   $isValid = [System.Int32]::TryParse($versionPart, [ref]$versionPartInt)
   if ($isValid) 
   {
        Write-Debug $versionPartsGroup.Add($versionPartInt)
   }
}

$versionPartsGroup[$versionPartIndex]++

$newVersionString = [System.String]::Join('.',$versionPartsGroup.ToArray())

Write-Output $newVersionString