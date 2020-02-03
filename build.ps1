[CmdletBinding()]
param(
    #[Parameter()]
    #[string]$nuget = $Env:NuGet,
    #[Parameter()]
    #[string]$msbuild = $Env:MsBuildExe,
    [Parameter()]
    [string]$srcPath = $Env:SourcesPath,
    [Parameter()]
    [string]$configuration = $Env:Configuration
    #,
    #[Parameter()]
    #[string]$version = $Env:PackageVersion
)

# ./build.ps1 -srcPath . -configuration Release 

dotnet restore $srcPath
dotnet build $srcPath --no-restore --configuration $configuration
dotnet test $srcPath --no-restore --no-build --configuration $configuration
dotnet pack $srcPath --no-restore --no-build --configuration $configuration -o $srcPath/pack