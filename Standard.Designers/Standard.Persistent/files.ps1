#New-Item -type Directory HKCU:\Software\Test1

$currentUser = New-Object Security.Principal.WindowsPrincipal $([Security.Principal.WindowsIdentity]::GetCurrent())
$testadmin = $currentUser.IsInRole([Security.Principal.WindowsBuiltinRole]::Administrator)
if ($testadmin -eq $false) {
Start-Process powershell.exe -Verb RunAs -ArgumentList ('-noprofile -noexit -file "{0}" -elevated' -f ($myinvocation.MyCommand.Definition))
exit $LASTEXITCODE
}


$psi = New-object System.Diagnostics.ProcessStartInfo 
$psi.CreateNoWindow = $true 
$psi.UseShellExecute = $false 
$psi.RedirectStandardOutput = $true 
$psi.RedirectStandardError = $true 
$psi.FileName = "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe"
$psi.Arguments = "-nologo -property installationPath"
$process = New-Object System.Diagnostics.Process 
$process.StartInfo = $psi 
[void]$process.Start()
$output = $process.StandardOutput.ReadToEnd()
$process.WaitForExit() 
Write-Host 开始写入文件
Write-Host $output

New-PSDrive -name HKCR -PSProvider registry -root HKLM:\SOFTWARE\Classes
New-Item -ItemType String 'HKCR:\.dpdl' -Value $output

#start-process "${env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe" -ArgumentList "-nologo -property installationPath" -nonewwindow -wait -RedirectStandardOutput $output
#Write-Host 开始写入文件
#Write-Host $output.ReadToEnd()
Write-Host  '执行完毕,按任意键退出...'
Read-Host