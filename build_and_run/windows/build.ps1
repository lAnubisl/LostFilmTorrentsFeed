cd $PSScriptRoot
cd ../../
dotnet publish LostFilmMonitoring.Web/LostFilmMonitoring.Web.csproj --runtime win-x64 --self-contained true /p:PublishTrimmed=true /p:PublishSingleFile=true -p:PublishReadyToRun=true --configuration Release --output ./build_and_run/windows/LostFilmFeed