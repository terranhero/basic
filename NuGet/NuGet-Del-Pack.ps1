$path= Get-Location
if(Test-Path $path"\NuGet.exe")
{
    #Start-Process NuGet.exe "setApiKey 7a3459c0-d9ce-4c81-aa2f-3a9faf36a81d"
    $nugetApiKey = "oy2pruzl2dus2f34dijato3btf3bzgzedbileodkb3bfii"
    $nugetSource = "https://www.nuget.org/api/v2/package"
    Write-Host "开始删除NuGet包"
    Start-Process NuGet.exe "delete Basic.EntityLayer 4.7.13450 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.EntityLayer 4.5.13480 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.EntityLayer 4.5.13479 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.EntityLayer 4.5.13451 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.EntityLayer 4.5.13446 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.EntityLayer 4.5.13427 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.EntityLayer 4.5.13424 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.EntityLayer 4.0.13419 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.EntityLayer 4.0.13418 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.EntityLayer 4.0.13416 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.EntityLayer 4.0.13405 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.EntityLayer 4.0.2016.1339 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.EntityLayer 4.0.2016.1338 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.EntityLayer 4.0.2016.1337 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.EntityLayer 4.0.2016.1335 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.EntityLayer 4.0.2016.1332 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.EntityLayer 4.0.2016.1331 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.EntityLayer 4.0.2016.1329 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.EntityLayer 4.0.2016.1328 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.EntityLayer 4.0.2016.1309 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.EntityLayer 4.0.2016.1307 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.EntityLayer 4.0.2016.1306 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.EntityLayer 4.0.2016.1305 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.EntityLayer 4.0.2016.1304 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.EntityLayer 4.0.2016.1279 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.EntityLayer 4.0.2016.1265 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.EntityLayer 4.0.2016.1264 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.EntityLayer 4.0.2016.1251 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.EntityLayer 4.0.2016.1246 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
Start-Process NuGet.exe "delete Basic.EntityLayer 4.0.1612.901 -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
    Write-Host "删除NuGet包结束"
}
cmd /c "pause"
#Remove-Item -Recurse -Force $path"\Package"