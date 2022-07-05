Write-Output "Texnomic SecureDNS - Terminal Edition"
Write-Output "Publishing Script"

$Version = Read-Host "Please Enter Version: "

$ProjectPath = "..\Texnomic.SecureDNS.Terminal"

$Fingerprint = "d4518c71443aebbc0429ec005774bc01176a2dbb"

$Tools = "..\Tools"

$Builds = "..\Builds"

$Platforms = "win-x64", "win-x86", "linux-x64", "linux-arm64", "linux-musl-x64", "osx-x64" 

Set-Location $ProjectPath

$ProjectFilePath = (Get-ChildItem "*.csproj").FullName

[xml]$CSProject = Get-Content $ProjectFilePath

$CSProject.SelectSingleNode("//Version").InnerText = $Version

$CSProject.Save($ProjectFilePath)

dotnet clean

Remove-Item -Recurse -Force $Builds

Foreach ($Platform in $Platforms) 
{ 
    $Output = $Builds + "\" + $Platform

    New-Item -Path $Output -ItemType Directory

    dotnet publish $ProjectFilePath --self-contained true -p:PublishSingleFile=true --output $Output --runtime $Platform

    $Executable = $Builds + "\" + $Platform + "\" + "Texnomic.SecureDNS.Terminal.exe"

    if ($Platform.StartsWith("linux") -or $Platform.StartsWith("osx"))
    {
        $Executable = $Builds + "\" + $Platform + "\" + "Texnomic.SecureDNS.Terminal"
    }

    Set-AuthenticodeSignature -FilePath (Get-ChildItem $Executable) -Certificate (Get-ChildItem -path Cert:CurrentUser\My\*$Fingerprint) -IncludeChain All -TimestampServer "http://time.certum.pl"

    Get-AuthenticodeSignature -FilePath (Get-ChildItem $Executable)
}

Set-Location "..\Scripts"