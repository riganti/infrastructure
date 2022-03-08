param([String]$version)


$scriptRoot = Split-Path $MyInvocation.MyCommand.Path
. "$scriptRoot\ProjectList.ps1"

foreach ($package in $packages) {
    $filePath = ".\$($package.Directory)\$($package.Directory).csproj"
    $file = [System.IO.File]::ReadAllText($filePath, [System.Text.Encoding]::UTF8)
	$file = [System.Text.RegularExpressions.Regex]::Replace($file, "\<PackageVersion\>([^<]+)\</PackageVersion\>", "<PackageVersion>" + $version + "</PackageVersion>")
	[System.IO.File]::WriteAllText($filePath, $file, [System.Text.Encoding]::UTF8)
}  
