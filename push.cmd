@echo off
nuget push .\Plugin.Geofencing\bin\Release\*.nupkg -Source https://www.nuget.org/api/v2/package
pause