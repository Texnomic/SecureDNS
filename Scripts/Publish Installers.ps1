Write-Output "Texnomic SecureDNS Publishing Script"

devenv "..\Texnomic.SecureDNS.sln" /Project "..\Texnomic.SecureDNS.Terminal.Installer-x64\Texnomic.SecureDNS.Terminal.Installer-x64.vdproj" /Build Release

.\signtool.exe sign /sha1 "d4518c71443aebbc0429ec005774bc01176a2dbb" /tr http://time.certum.pl /fd sha256 /v "..\Texnomic.SecureDNS.Terminal.Installer-x64\Publish\Texnomic.SecureDNS.Terminal-x64.msi"