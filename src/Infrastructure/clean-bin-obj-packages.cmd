for /d /r . %%d in (bin,obj,Packages) do @if exist "%%d" rd /s/q "%%d"
pause
