$path= Get-Location
if(Test-Path $path"\NuGet.exe")
{
    #Start-Process NuGet.exe "setApiKey 7a3459c0-d9ce-4c81-aa2f-3a9faf36a81d"
    $nugetApiKey = "oy2gx223l2rktrdqvsqqtbrirdzrukdebbd2l6r4qkfffq"
    $nugetSource = "https://www.nuget.org/api/v2/package"
    $parentPath = Split-Path -Parent $path
    $binPath = $parentPath+"\bin\"
    $dllPath = $parentPath+"\bin\net6.0\"
    if(Test-Path $path"\Package"){ Remove-Item $path"\Package\*.*" }
    else{$null = New-Item Package -type directory}
    #Write-Host $path 
    $packagePath = Join-Path $path -CHILDPATH "Package"
    #Write-Host $packagePath
    Write-Host "开始生成NuGet包"
    $fileVersion = [System.Diagnostics.FileVersionInfo]::GetVersionInfo($dllPath+"Basic.EntityLayer.dll")
    $version= $fileVersion.FileVersion
    #Start-Process NuGet.exe "pack Basic.Configuration.nuspec -Version $version -OutputDirectory Package" -NoNewWindow -Wait
    #Start-Process NuGet.exe "pack Basic.MvcLibrary.nuspec -Version $version -OutputDirectory Package" -NoNewWindow -Wait
    copy-item "$binPath\Standard.EntityLayer.$version.nupkg" -destination $packagePath
    Write-Host "Successfully copied package $binPath\Standard.EntityLayer.$version.nupkg"

    copy-item "$binPath\Standard.DataAccess.$version.nupkg" -destination $packagePath
    Write-Host "Successfully copied package $binPath\Standard.DataAccess.$version.nupkg"

    copy-item "$binPath\Basic.MvcLibrary.$version.nupkg" -destination $packagePath
    Write-Host "Successfully copied package $binPath\Basic.MvcLibrary.$version.nupkg"

    copy-item "$binPath\Standard.MvcLibrary.$version.nupkg" -destination $packagePath
    Write-Host "Successfully copied package $binPath\Standard.MvcLibrary.$version.nupkg"

    copy-item "$binPath\Standard.MySqlAccess.$version.nupkg" -destination $packagePath
    Write-Host "Successfully copied package $binPath\Standard.MySqlAccess.$version.nupkg"

    copy-item "$binPath\Standard.SqlAccess.$version.nupkg" -destination $packagePath
    Write-Host "Successfully copied package $binPath\Standard.SqlAccess.$version.nupkg"
    #Start-Process NuGet.exe "pack Standard.EntityLayer.nuspec -Version $version -OutputDirectory Package" -NoNewWindow -Wait
    #Start-Process NuGet.exe "pack Standard.DataAccess.nuspec -Version $version -OutputDirectory Package" -NoNewWindow -Wait
    #Start-Process NuGet.exe "pack Standard.MvcLibrary.nuspec -Version $version -OutputDirectory Package" -NoNewWindow -Wait
    #Start-Process NuGet.exe "pack Standard.MySqlAccess.nuspec -Version $version -OutputDirectory Package" -NoNewWindow -Wait
    #Start-Process NuGet.exe "pack Basic.EasyLibrary.nuspec -Version $version -OutputDirectory Package" -NoNewWindow -Wait
    #Start-Process NuGet.exe "pack Basic.WinForms.nuspec -Version $version -OutputDirectory Package" -NoNewWindow -Wait
    #Start-Process NuGet.exe "pack Basic.Windows.nuspec -Version $version -OutputDirectory Package" -NoNewWindow -Wait
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
            #Start-Process NuGet.exe "delete Basic.Configuration $oldver -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
            Start-Process NuGet.exe "delete Standard.EntityLayer $oldver -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
            Start-Process NuGet.exe "delete Standard.DataAccess $oldver -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
            Start-Process NuGet.exe "delete Basic.MvcLibrary $oldver -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
            Start-Process NuGet.exe "delete Standard.MvcLibrary $oldver -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
            Start-Process NuGet.exe "delete Standard.MySqlAccess $oldver -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
            Start-Process NuGet.exe "delete Standard.SqlAccess $oldver -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
            #Start-Process NuGet.exe "delete Basic.EasyLibrary $oldver -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
            #Start-Process NuGet.exe "delete Basic.WinForms $oldver -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
            #Start-Process NuGet.exe "delete Basic.Windows $oldver -Source $nugetSource -ApiKey $nugetApiKey -NonInteractive" -NoNewWindow -Wait
        }
	    #copy-item "$path\Package\Basic.Configuration.$version.nupkg" -destination "C:\Users\JACKY\AppData\Local\NuGet\Cache"
        #copy-item "$path\Package\Basic.EntityLayer.$version.nupkg" -destination "C:\Users\JACKY\AppData\Local\NuGet\Cache"
        #copy-item "$path\Package\Basic.DataAccess.$version.nupkg" -destination "C:\Users\JACKY\AppData\Local\NuGet\Cache"
        #copy-item "$path\Package\Basic.MySqlAccess.$version.nupkg" -destination "C:\Users\JACKY\AppData\Local\NuGet\Cache"
        #copy-item "$path\Package\Basic.MvcLibrary.$version.nupkg" -destination "C:\Users\JACKY\AppData\Local\NuGet\Cache"
        #copy-item "$path\Package\Basic.EasyLibrary.$version.nupkg" -destination "C:\Users\JACKY\AppData\Local\NuGet\Cache"
        #copy-item "$path\Package\Basic.WinForms.$version.nupkg" -destination "C:\Users\JACKY\AppData\Local\NuGet\Cache"
        #copy-item "$path\Package\Basic.Windows.$version.nupkg" -destination "C:\Users\JACKY\AppData\Local\NuGet\Cache"
        #Write-Host "复制NuGet包至NuGet缓存目录结束"

        #Start-Process NuGet.exe "push ""$path\Package\Basic.Configuration.$version.nupkg"" -Source ""$nugetSource"" -ApiKey $nugetApiKey" -NoNewWindow -Wait
        Start-Process NuGet.exe "push ""$path\Package\Standard.EntityLayer.$version.nupkg"" -Source ""$nugetSource"" -ApiKey $nugetApiKey" -NoNewWindow -Wait
        Start-Process NuGet.exe "push ""$path\Package\Standard.DataAccess.$version.nupkg"" -Source ""$nugetSource"" -ApiKey $nugetApiKey" -NoNewWindow -Wait
        Start-Process NuGet.exe "push ""$path\Package\Basic.MvcLibrary.$version.nupkg"" -Source ""$nugetSource"" -ApiKey $nugetApiKey" -NoNewWindow -Wait
        Start-Process NuGet.exe "push ""$path\Package\Standard.MvcLibrary.$version.nupkg"" -Source ""$nugetSource"" -ApiKey $nugetApiKey" -NoNewWindow -Wait
        Start-Process NuGet.exe "push ""$path\Package\Standard.MySqlAccess.$version.nupkg"" -Source ""$nugetSource"" -ApiKey $nugetApiKey" -NoNewWindow -Wait
        Start-Process NuGet.exe "push ""$path\Package\Standard.SqlAccess.$version.nupkg"" -Source ""$nugetSource"" -ApiKey $nugetApiKey" -NoNewWindow -Wait
        #Start-Process NuGet.exe "push ""$path\Package\Basic.EasyLibrary.$version.nupkg"" -Source ""$nugetSource"" -ApiKey $nugetApiKey" -NoNewWindow -Wait
        #Start-Process NuGet.exe "push ""$path\Package\Basic.WinForms.$version.nupkg"" -Source ""$nugetSource"" -ApiKey $nugetApiKey" -NoNewWindow -Wait
        #Start-Process NuGet.exe "push ""$path\Package\Basic.Windows.$version.nupkg"" -Source ""$nugetSource"" -ApiKey $nugetApiKey" -NoNewWindow -Wait
        Write-Host "向(https://www.nuget.org)推送NuGet包完成。"
    }
}
cmd /c "pause"
#Remove-Item -Recurse -Force $path"\Package"