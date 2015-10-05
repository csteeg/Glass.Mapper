[CmdletBinding()]
Param(
   [Parameter(Mandatory=$True,Position=1)]
   [string]$releaseNumber)
 
$msbuild = $env:windir+"\Microsoft.NET\Framework\v4.0.30319\msbuild "

Function LogWrite
{
   Param ([string]$logstring)

   Write-Host $logstring
}

LogWrite "****** Updating version numbers ******"

$assInfos = Get-ChildItem -Path .\Source -Filter AssemblyInfo.cs -Recurse | ForEach-Object -Process {$_.FullName}

$newAssVersion = 'AssemblyVersion("'+$releaseNumber+'")'
$newFileVersion = 'AssemblyFileVersion("'+$releaseNumber+'")'
$copyright = 'AssemblyCopyright("Copyright Michael Edwards, Chris van de Steeg  2012")'

foreach($assInfo  in $assInfos){
    LogWrite $assInfo
    $content = Get-Content $assInfo | Out-String
    $content = $content -replace 'AssemblyVersion\("[\d\.]*"\)', $newAssVersion
    $content = $content -replace 'AssemblyFileVersion\("[\d\.]*"\)', $newFileVersion
    $content = $content -replace 'AssemblyCopyright\("[^"]*"\)', $copyright
    $content | Out-File $assInfo 
}

$build = $msbuild + '"Glass.Mapper - Release.sln"'
$build70 = $build+' /property:ScVersion=sc70'
$build71 = $build+' /property:ScVersion=sc71'
$build72 = $build+' /property:ScVersion=sc72'
$build75 = $build+' /property:ScVersion=sc75'
$build80 = $build+' /property:ScVersion=sc80'

Invoke-Expression $build70
Invoke-Expression $build71
Invoke-Expression $build72
Invoke-Expression $build75
Invoke-Expression $build80

New-Item -ItemType directory -Path nugets -Force
$nuget1 = "nuget pack nugetdefinitions\BoC.Glass.Mapper.Sc.symbols.nuspec -Version " + $releaseNumber + " -BasePath . -NoPackageAnalysis -OutputDirectory .\nugets"
$nuget2 = "nuget pack nugetdefinitions\BoC.Glass.Mapper.Sc.ContentSearch.LuceneProvider.symbols.nuspec -Version " + $releaseNumber + " -BasePath . -NoPackageAnalysis -OutputDirectory .\nugets"
Invoke-Expression $nuget1
Invoke-Expression $nuget2