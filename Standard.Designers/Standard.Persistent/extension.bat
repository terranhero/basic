set devpath=%~dp0:~0,2%
reg add "HKCR\VisualStudio.dpdl.15.0" /ve /t REG_SZ /d "ASP.NET MVC Data Persistent" /f
reg add "HKCR\VisualStudio.dpdl.15.0\DefaultIcon" /ve /t REG_SZ /d "D:\Program Files\Microsoft Visual Studio\2017\Enterprise\Common7\IDE\msenvico.dll,-218" /f

reg add "HKCR\.dpdl" /ve /t REG_SZ /d "VisualStudio.dpdl.15.0" /f
echo %devpath%
pause


reg add "HKCR\VisualStudio.dpdl.15.0" /ve /t REG_SZ /d "ASP.NET MVC Data Persistent" /f
reg add "HKCR\VisualStudio.dpdl.15.0\DefaultIcon" /ve /t REG_SZ /d "D:\Program Files\Microsoft Visual Studio\2017\Enterprise\Common7\IDE\msenvico.dll,-218" /f

reg add "HKCR\.dpdl" /ve /t REG_SZ /d "VisualStudio.dpdl.15.0" /f
echo %~dp0
pause