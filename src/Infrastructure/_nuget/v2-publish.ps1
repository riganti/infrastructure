param([String]$version, [String]$apiKey, [String]$server)

del ..\Riganti.Utils.Infrastructure.Core\bin\debug\*.nupkg -ErrorAction SilentlyContinue
del ..\Riganti.Utils.Infrastructure.EntityFrameworkCore\bin\debug\*.nupkg -ErrorAction SilentlyContinue
del ..\Riganti.Utils.Infrastructure.Services\bin\debug\*.nupkg -ErrorAction SilentlyContinue
del ..\Riganti.Utils.Infrastructure.AspNetCore\bin\debug\*.nupkg -ErrorAction SilentlyContinue

$filePath = "..\Riganti.Utils.Infrastructure.Core\project.json"
$file = [System.IO.File]::ReadAllText($filePath, [System.Text.Encoding]::UTF8)
$file = [System.Text.RegularExpressions.Regex]::Replace($file, """version"": ""([^""]+)""", """version"": """ + $version + """")
[System.IO.File]::WriteAllText($filePath, $file, [System.Text.Encoding]::UTF8)

$filePath = "..\Riganti.Utils.Infrastructure.EntityFrameworkCore\project.json"
$file = [System.IO.File]::ReadAllText($filePath, [System.Text.Encoding]::UTF8)
$file = [System.Text.RegularExpressions.Regex]::Replace($file, """version"": ""([^""]+)""", """version"": """ + $version + """")
[System.IO.File]::WriteAllText($filePath, $file, [System.Text.Encoding]::UTF8)

$filePath = "..\Riganti.Utils.Infrastructure.Services\project.json"
$file = [System.IO.File]::ReadAllText($filePath, [System.Text.Encoding]::UTF8)
$file = [System.Text.RegularExpressions.Regex]::Replace($file, """version"": ""([^""]+)""", """version"": """ + $version + """")
[System.IO.File]::WriteAllText($filePath, $file, [System.Text.Encoding]::UTF8)

$filePath = "..\Riganti.Utils.Infrastructure.AspNetCore\project.json"
$file = [System.IO.File]::ReadAllText($filePath, [System.Text.Encoding]::UTF8)
$file = [System.Text.RegularExpressions.Regex]::Replace($file, """version"": ""([^""]+)""", """version"": """ + $version + """")
[System.IO.File]::WriteAllText($filePath, $file, [System.Text.Encoding]::UTF8)

cd ..\Riganti.Utils.Infrastructure.Core
& dotnet restore
& dotnet pack
cd ..\Riganti.Utils.Infrastructure.EntityFrameworkCore
& dotnet restore
& dotnet pack
cd ..\Riganti.Utils.Infrastructure.Services
& dotnet restore
& dotnet pack
cd ..\Riganti.Utils.Infrastructure.AspNetCore
& dotnet restore
& dotnet pack
cd ..\_nuget

& .\nuget.exe push ..\Riganti.Utils.Infrastructure.Core\bin\debug\Riganti.Utils.Infrastructure.Core.$version.nupkg -source $server -apiKey $apiKey
& .\nuget.exe push ..\Riganti.Utils.Infrastructure.EntityFrameworkCore\bin\debug\Riganti.Utils.Infrastructure.EntityFrameworkCore.$version.nupkg -source $server -apiKey $apiKey
& .\nuget.exe push ..\Riganti.Utils.Infrastructure.Services\bin\debug\Riganti.Utils.Infrastructure.Services.$version.nupkg -source $server -apiKey $apiKey
& .\nuget.exe push ..\Riganti.Utils.Infrastructure.AspNetCore\bin\debug\Riganti.Utils.Infrastructure.AspNetCore.$version.nupkg -source $server -apiKey $apiKey
