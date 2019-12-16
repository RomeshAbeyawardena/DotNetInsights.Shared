Param(
    [string]$installDirectory,
    [String]$projectName
)


$solutionFolder = "$installDirectory\$projectName"

$projects = "App", "Contracts", "Data", "Domains", "Shared";

if (!(Test-Path $installDirectory))
   {
        mkdir $solutionFolder
   }

cd $solutionFolder

Foreach($currentProject in $projects)
{
    $currentProjectFolder = "$solutionFolder\$projectName.$currentProject"
}