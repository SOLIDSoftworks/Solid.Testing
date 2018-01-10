[CmdletBinding()]
param(
    [Parameter()]
    [string]$nuget = $Env:NuGet,
    [Parameter()]
    [string]$msbuild = $Env:MsBuildExe,
    [Parameter()]
    [string]$srcPath = $Env:SourcesPath,
    [Parameter()]
    [string]$configuration = $Env:Configuration,
    [Parameter()]
    [string]$version = $Env:PackageVersion
)

function Find-Projects {
    [CmdletBinding()]
    param(
        
    )
    process {
        Write-Verbose "Finding all *.csproj files in $srcPath"
        Get-ChildItem -Path $srcPath -Filter *.csproj -Recurse -File
    }
}

function Test-LegacyProject {
    [CmdletBinding()]
    param(    
        [Parameter(Mandatory=$true)]
        [string] $project,
        [Parameter(Mandatory=$true)]
        [xml] $xml
    )
    process {
        !$xml.Project.HasAttribute("Sdk") -or ($xml.Project.Sdk -ne 'Microsoft.Net.Sdk')
    }
}

function Invoke-MsBuildPack {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory=$true)]
        [string] $project,
        [Parameter(Mandatory=$true)]
        [xml] $xml
    )
    process {
        Write-Verbose "Invoking msbuild /t:pack for $project"
        . $msbuild $project /nologo /t:pack /verbosity:minimal "/property:Configuration=$configuration"
    }
}

function Invoke-MsBuild {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory=$true)]
        [string] $project,
        [Parameter(Mandatory=$true)]
        [xml] $xml
    )
    process {
        Write-Verbose "Invoking msbuild for $project"
        . $msbuild $project /nologo /verbosity:minimal "/property:Configuration=$configuration"
    }
}

function Update-Nuspec {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory=$true)]
        [string] $path,
        [Parameter(Mandatory=$true)]
        [string] $assembly,
        [Parameter(Mandatory=$true)]
        [string] $id
    )
    process {
        $directory = [IO.Path]::GetDirectoryName($path)
        $new = [IO.Path]::Combine($directory, "$([Guid]::NewGuid()).nuspec")
        $xml = [xml](Get-Content $path)

        $xml.package.metadata.id = $id
        $xml.package.metadata.version = $version
        $xml.package.metadata.dependencies.dependency | Where-Object { $_.id.StartsWith('Solid.Testing.') } | ForEach-Object { $_.version = $version }

        $files = $xml.CreateElement('files', $xml.package.xmlns)
        $file = $xml.CreateElement('file', $xml.package.xmlns)
        $file.SetAttribute('src', "bin/$configuration/$assembly.dll") | Out-Null
        $file.SetAttribute('target', 'lib/net461') | Out-Null

        $files.AppendChild($file)  | Out-Null       

        $xml.package.AppendChild($files) | Out-Null

        $xml.Save($new) | Out-Null

        $new
    }
}

function Invoke-Nuget {
    [CmdletBinding()]
    param(
        [Parameter(Mandatory=$true)]
        [string] $project,
        [Parameter(Mandatory=$true)]
        [xml] $xml
    )
    begin {
        $path = $project
    }
    process {
        $name = [IO.Path]::GetFileNameWithoutExtension($project)
        $directory = [IO.Path]::GetDirectoryName($project)
        Write-Verbose "Checking for *.nuspec in $directory"
        $nuspec = Get-ChildItem -Path $directory -Filter *.nuspec 
        
        if($nuspec) {
            $assemblyName = $xml.Project.PropertyGroup.AssemblyName | Where-Object { $_ }
            $path = Update-Nuspec -path $nuspec.FullName -id $name -assembly $assemblyName
        }

        Write-Verbose "Invoking nuget for $path"
        . $nuget pack $path
    }
}

$artifacts = [IO.Path]::Combine($srcPath, 'artifacts')
if(!(Test-Path $artifacts)) {
    Write-Verbose "Creating folder: $artifacts"
    New-Item -Path $artifacts -ItemType directory | Out-Null
}

$projects = Find-Projects
foreach($project in $projects) {
    $path = $project.FullName
    $xml = [xml](Get-Content -Path $path)
    if(Test-LegacyProject -project $path -xml $xml) {
        Invoke-MsBuild -project $path -xml $xml
        Invoke-Nuget -project $path -xml $xml
    }
    else {
        Invoke-MsBuildPack -project $path -xml $xml        
    }
}

# C:\Program Files (x86)\Microsoft Visual Studio\2017\Professional\MSBuild\15.0\Bin\MSBuild.exe
# H:\nuget.exe