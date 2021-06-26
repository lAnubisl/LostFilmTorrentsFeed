
cd $PSScriptRoot
[Console]::OutputEncoding = [Text.UTF8Encoding]::UTF8
$Env:BASEFEEDCOOKIE = ""
$Env:BASELINKUID = ""
./'LostFilmFeed/LostFilmMonitoring.Web.exe'