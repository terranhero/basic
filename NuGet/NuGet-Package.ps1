$path = Get-Location
if(Test-Path $path"\NuGet.exe")
{
    $parentPath = Split-Path -Parent $path
    $binPath = $parentPath+"\bin\"
    $dllPath = $parentPath+"\bin\netstandard2.0\"
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
    Write-Host "生成NuGet包结束"
}
cmd /c "pause"