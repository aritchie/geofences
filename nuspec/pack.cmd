@echo off
del *.nupkg
nuget pack Acr.Geofencing.nuspec
nuget pack Plugin.Geofencing.nuspec
pause