@echo off
nuget push .\Plugin.Geofencing\bin\Release\*.nupkg -Source https://www.nuget.org/api/v2/package
nuget push .\Plugin.Geofencing\bin\Release\*.nupkg -Source https://www.myget.org/F/acr/api/v2/package
pause