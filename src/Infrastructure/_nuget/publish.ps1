param([String]$key)


$file = dir Riganti.Utils.Infrastructure.Core.nupkg
$file = $file.FullName
& .\nuget.exe pack $file

$file2 = dir Riganti.Utils.Infrastructure.EntityFramework.nupkg
$file2 = $file2.FullName
& .\nuget.exe pack $file2


& .\nuget.exe push $file $key
& .\nuget.exe push $file2 $key