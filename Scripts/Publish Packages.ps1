Write-Output "Texnomic SecureDNS Publishing Script"

$Version = Read-Host "Please Enter Version"

$Projects = @(
    "..\Texnomic.FilterLists",
    "..\Texnomic.ENS.BaseRegistrar",
    "..\Texnomic.ENS.PublicResolver",
    "..\Texnomic.SecureDNS.Abstractions",
    "..\Texnomic.Sodium",
    "..\Texnomic.SecureDNS.Extensions",
    "..\Texnomic.SecureDNS.Core",
    "..\Texnomic.SecureDNS.Serialization",
    "..\Texnomic.Socks5",
	"..\Texnomic.Socks5.WebProxy",
    "..\Texnomic.SecureDNS.Protocols",
    "..\Texnomic.SecureDNS.Middlewares",
    "..\Texnomic.SecureDNS.Servers"
)

$Fingerprint = "d4518c71443aebbc0429ec005774bc01176a2dbb"

$DllPath = "bin\Release\net6.0\"

$PackagePath = "bin\Release\"

$Tools = "C:\Users\Texnomic.Lab\Repositories\SecureDNS\Tools"

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

    $PackageSymbols = (Get-ChildItem "*.$Version.symbols.nupkg").FullName

    Set-Location $Tools

    .\nuget.exe sign $Package -CertificateFingerprint $Fingerprint -Timestamper http://time.certum.pl

    .\nuget.exe sign $PackageSymbols -CertificateFingerprint $Fingerprint -Timestamper http://time.certum.pl

    .\nuget.exe push $Package -Source nuget

    .\nuget.exe push $PackageSymbols -Source nuget
    
    .\nuget.exe push $Package -Source github

    .\nuget.exe push $PackageSymbols -Source github
}