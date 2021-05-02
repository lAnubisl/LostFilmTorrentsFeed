
cd $PSScriptRoot
[Console]::OutputEncoding = [Text.UTF8Encoding]::UTF8
$Env:BASEFEEDCOOKIE = ""
./'LostFilmMonitoring.Web.exe'