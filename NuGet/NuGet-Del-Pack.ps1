$path= Get-Location
if(Test-Path $path"\NuGet.exe")
{
	Write-Host "如果执行nuget.exe命令报错，请检查nuget source 是否配置"
	Write-Host "https://api.nuget.org/v3/index.json"
	Write-Host "如果执行nuget.exe命令报错，请检查nuget apiKey 是否配置,如果没有可执行以下命令"
	Write-Host "nuget.exe setApiKey ********** source https://api.nuget.org/v3/index.json"
    $nugetSource = "https://www.nuget.org/api/v2/package"
    Write-Host "开始删除NuGet包"
    Start-Process dotnet "nuget delete Standard.SqlAccess 6.0.5482 --source $nugetSource -NonInteractive" -NoNewWindow -Wait
	Start-Process dotnet "nuget delete Standard.SqlAccess 4.7.13591 --source $nugetSource -NonInteractive" -NoNewWindow -Wait
    Write-Host "删除NuGet包结束"
}
cmd /c "pause"
#Remove-Item -Recurse -Force $path"\Package"