param([String]$key)

del *.nupkg

$file = dir Riganti.Utils.Infrastructure.Core.nuspec
$file = $file.FullName
& .\nuget.exe pack $file

$file2 = dir Riganti.Utils.Infrastructure.EntityFramework.nuspec
$file2 = $file2.FullName
& .\nuget.exe pack $file2



$files = dir *.nupkg
foreach ($file in $files) {
  & .\nuget.exe push $file.FullName $key
}


