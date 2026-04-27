$path = Get-Location
#验证nuget.exe 文件是否存在
if(Test-Path $path"\NuGet.exe")
{
    $parentPath = Split-Path -Parent $path
	$nuget = $path.Path +"\NuGet.exe"
    $binPath = $parentPath+"\bin\"
    $dllPath = $parentPath+"\bin\net8.0\"
    if(Test-Path $path"\Package"){ Remove-Item $path"\Package\*.*" }
    else{$null = New-Item Package -type directory}
    #Write-Host $path 
    $packagePath = Join-Path $path -CHILDPATH "Package"
    #Write-Host $packagePath
	Write-Host "如果执行nuget.exe命令报错，请检查nuget source 是否配置"
	Write-Host "https://api.nuget.org/v3/index.json"
	Write-Host "如果执行nuget.exe命令报错，请检查nuget apiKey 是否配置,如果没有可执行以下命令"
	Write-Host "nuget.exe setApiKey ********** source https://api.nuget.org/v3/index.json"
    Write-Host "开始生成NuGet包"
    $fileVersion = [System.Diagnostics.FileVersionInfo]::GetVersionInfo($dllPath+"Basic.EntityLayer.dll")
    $version= $fileVersion.FileVersion
    #Start-Process NuGet.exe "pack Basic.Configuration.nuspec -Version $version -OutputDirectory Package" -NoNewWindow -Wait
    #Start-Process NuGet.exe "pack Basic.MvcLibrary.nuspec -Version $version -OutputDirectory Package" -NoNewWindow -Wait
    copy-item "$binPath\Standard.EntityLayer.$version.nupkg" -destination $packagePath
    Write-Host "01:Successfully copied package $binPath\Standard.EntityLayer.$version.nupkg"

    copy-item "$binPath\Standard.DataAccess.$version.nupkg" -destination $packagePath
    Write-Host "02:Successfully copied package $binPath\Standard.DataAccess.$version.nupkg"
	
    copy-item "$binPath\Standard.DependencyInjections.$version.nupkg" -destination $packagePath
    Write-Host "03:Successfully copied package $binPath\Standard.DependencyInjections.$version.nupkg"	
	
    copy-item "$binPath\Standard.MvcLibrary.$version.nupkg" -destination $packagePath
    Write-Host "04:Successfully copied package $binPath\Standard.MvcLibrary.$version.nupkg"

    copy-item "$binPath\Standard.SqlClientAccess.$version.nupkg" -destination $packagePath
    Write-Host "05:Successfully copied package $binPath\Standard.SqlClientAccess.$version.nupkg"

	copy-item "$binPath\Standard.OracleAccess.$version.nupkg" -destination $packagePath
    Write-Host "06:Successfully copied package $binPath\Standard.OracleAccess.$version.nupkg"

    copy-item "$binPath\Standard.MySqlAccess.$version.nupkg" -destination $packagePath
    Write-Host "07:Successfully copied package $binPath\Standard.MySqlAccess.$version.nupkg"
	
	copy-item "$binPath\Standard.PostgreAccess.$version.nupkg" -destination $packagePath
    Write-Host "08:Successfully copied package $binPath\Standard.PostgreAccess.$version.nupkg"	
	
    copy-item "$binPath\Standard.DB2Access.$version.nupkg" -destination $packagePath
    Write-Host "09:Successfully copied package $binPath\Standard.DB2Access.$version.nupkg"	

    copy-item "$binPath\Standard.SqliteAccess.$version.nupkg" -destination $packagePath
    Write-Host "10:Successfully copied package $binPath\Standard.SqliteAccess.$version.nupkg"	
	
    Write-Host "生成NuGet包结束"
    
    Write-Host "开始向(https://www.nuget.org)推送NuGet包。"
   
    $ws = New-Object -ComObject WScript.Shell
	
	$wsr1 = $ws.popup("是否需要继续运行批处理文件？", 0,"确认提示",4 + 64)
	if($wsr1 -eq 6)    {
        $wsr = $ws.popup("是否需要删除以前的NuGet包？", 0,"确认提示",4 + 64)
        if($wsr -eq 6){
            $major = $fileVersion.FileMajorPart
            $minor = $fileVersion.FileMinorPart
        
            if($fileVersion.FilePrivatePart -eq 0){ $build = $fileVersion.FileBuildPart - 1 }
            else{ $build = $fileVersion.FileBuildPart}

            if($fileVersion.FilePrivatePart -gt 0){ $private = $fileVersion.FilePrivatePart - 1 }

            $oldversion = "$major.$minor.$build.$private"
            if($fileVersion.FilePrivatePart -eq 0){ $oldversion = "$major.$minor.$build" }

            $oldver = Read-Host "回车直接删除此版本($oldversion)的NuGet包或者输入需要删除的NuGet包版本号"
            if($oldver -eq "" -or $oldver -eq $null){ $oldver = $oldversion}
            Start-Process dotnet "nuget delete Standard.EntityLayer $oldver --non-interactive" -NoNewWindow -Wait
            Start-Process dotnet "nuget delete Standard.DataAccess $oldver --non-interactive" -NoNewWindow -Wait
			Start-Process dotnet "nuget delete Standard.DependencyInjections $oldver --non-interactive" -NoNewWindow -Wait
            Start-Process dotnet "nuget delete Standard.MvcLibrary $oldver --non-interactive" -NoNewWindow -Wait
            Start-Process dotnet "nuget delete Standard.SqlClientAccess $oldver --non-interactive" -NoNewWindow -Wait
            Start-Process dotnet "nuget delete Standard.MySqlAccess $oldver --non-interactive" -NoNewWindow -Wait
            Start-Process dotnet "nuget delete Standard.PostgreAccess $oldver --non-interactive" -NoNewWindow -Wait
            Start-Process dotnet "nuget delete Standard.OracleAccess $oldver --non-interactive" -NoNewWindow -Wait
            Start-Process dotnet "nuget delete Standard.DB2Access $oldver --non-interactive" -NoNewWindow -Wait
			Start-Process dotnet "nuget delete Standard.SqliteAccess $oldver --non-interactive" -NoNewWindow -Wait
        }
		Write-Host "开始向(https://www.nuget.org)推送NuGet包。"
		
        Start-Process dotnet "nuget push ""$path\Package\Standard.EntityLayer.$version.nupkg""" -NoNewWindow -Wait
        Start-Process dotnet "nuget push ""$path\Package\Standard.DataAccess.$version.nupkg""" -NoNewWindow -Wait
		Start-Process dotnet "nuget push ""$path\Package\Standard.DependencyInjections.$version.nupkg""" -NoNewWindow -Wait
        Start-Process dotnet "nuget push ""$path\Package\Standard.MvcLibrary.$version.nupkg""" -NoNewWindow -Wait
        Start-Process dotnet "nuget push ""$path\Package\Standard.MySqlAccess.$version.nupkg""" -NoNewWindow -Wait
        Start-Process dotnet "nuget push ""$path\Package\Standard.SqlClientAccess.$version.nupkg""" -NoNewWindow -Wait
		Start-Process dotnet "nuget push ""$path\Package\Standard.PostgreAccess.$version.nupkg""" -NoNewWindow -Wait
		Start-Process dotnet "nuget push ""$path\Package\Standard.OracleAccess.$version.nupkg""" -NoNewWindow -Wait
		Start-Process dotnet "nuget push ""$path\Package\Standard.DB2Access.$version.nupkg""" -NoNewWindow -Wait
		Start-Process dotnet "nuget push ""$path\Package\Standard.SqliteAccess.$version.nupkg""" -NoNewWindow -Wait

        Write-Host "向(https://www.nuget.org)推送NuGet包完成。"
    }
}
cmd /c "pause"
#Remove-Item -Recurse -Force $path"\Package"