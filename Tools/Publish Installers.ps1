Write-Output "Texnomic SecureDNS Publishing Script"

devenv "D:\Repositories\Texnomic.SecureDNS\Texnomic.SecureDNS.sln" /Project "D:\Repositories\Texnomic.SecureDNS\Texnomic.SecureDNS.Terminal.Installer-x64\Texnomic.SecureDNS.Terminal.Installer-x64.vdproj" /Build Release

.\signtool.exe sign /sha1 "33261449de13599f046617dd18994bab0e4346a8" /tr http://time.certum.pl /fd sha256 /v "D:\Repositories\Texnomic.SecureDNS\Texnomic.SecureDNS.Terminal.Installer-x64\Publish\Texnomic.SecureDNS.Terminal-x64.msi"