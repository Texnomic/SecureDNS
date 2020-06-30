Write-Output "Texnomic SecureDNS Publishing Script"

$Version = Read-Host "Please Enter Version"

$Projects = @(
    "D:\Repositories\Texnomic.SecureDNS\Texnomic.FilterLists",
    "D:\Repositories\Texnomic.SecureDNS\Texnomic.ENS.BaseRegistrar",
    "D:\Repositories\Texnomic.SecureDNS\Texnomic.ENS.PublicResolver",
    "D:\Repositories\Texnomic.SecureDNS\Texnomic.SecureDNS.Abstractions",
    "D:\Repositories\Texnomic.SecureDNS\Texnomic.Sodium",
    "D:\Repositories\Texnomic.SecureDNS\Texnomic.SecureDNS.Extensions",
    "D:\Repositories\Texnomic.SecureDNS\Texnomic.SecureDNS.Core",
    "D:\Repositories\Texnomic.SecureDNS\Texnomic.SecureDNS.Serialization",
    "D:\Repositories\Texnomic.SecureDNS\Texnomic.Socks5",
	"D:\Repositories\Texnomic.SecureDNS\Texnomic.Socks5.WebProxy",
    "D:\Repositories\Texnomic.SecureDNS\Texnomic.SecureDNS.Protocols",
    "D:\Repositories\Texnomic.SecureDNS\Texnomic.SecureDNS.Middlewares",
    "D:\Repositories\Texnomic.SecureDNS\Texnomic.SecureDNS.Servers"
)

$Fingerprint = "33261449de13599f046617dd18994bab0e4346a8"

$DllPath = "bin\Release\netstandard2.1\"

$PackagePath = "bin\Release\"

$Tools = "D:\Repositories\Texnomic.SecureDNS\Tools"

foreach ($Path in $Projects)
{
    Set-Location $Path

    $ProjectFilePath = (Get-ChildItem "*.csproj").FullName

    [xml]$CSProject = Get-Content $ProjectFilePath

    $CSProject.SelectSingleNode("//Version").InnerText = $Version

    $CSProject.Save($ProjectFilePath)

    dotnet clean

    dotnet build -c release

    Set-Location $Tools

    $Library = Join-Path -Path $Path -ChildPath $DllPath
    
    Set-Location $Library

    $Library = (Get-ChildItem "*.dll").FullName

    Set-Location $Tools

    .\signtool.exe sign /sha1 $Fingerprint /tr http://time.certum.pl /fd sha256 /v $Library

    Set-Location $Path

    dotnet pack --configuration Release --include-symbols --include-source
    
    $Package = Join-Path -Path $Path -ChildPath $PackagePath

    Set-Location $Package

    $Package = (Get-ChildItem "*.$Version.nupkg").FullName

    Set-Location $Tools

    .\nuget.exe sign $Package -CertificateFingerprint $Fingerprint -Timestamper http://time.certum.pl

    .\nuget.exe push $Package -Source github

    .\nuget.exe push $Package -Source nuget
}