Param(
    [string]$directory,
    [string]$output,
    [string]$version
)

if($directory -eq [System.String]::Empty) {
    $directory = $PSScriptRoot
}

if ($output -eq [System.String]::Empty){
    $output = "$directory\nuget"
}

cd $directory
dotnet clean
dotnet build
dotnet test

&"$directory\UpdateVersion-Powershell.ps1" -FileName $directory\Directory.Build.Props -Version $version  

$child_directories = Get-ChildItem $directory -Directory 

Foreach ($dir in $child_directories)
{
    "--------------- Processing $dir ---------------" 
    cd $dir.FullName
    dotnet pack --include-symbols --include-source -o "$output"
    cd ..
    "--------------- Processed $dir ---------------"
}