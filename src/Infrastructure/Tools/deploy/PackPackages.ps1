param([String]$nugetRestoreAltSource = "")


$scriptRoot = Split-Path $MyInvocation.MyCommand.Path
. "$scriptRoot\ProjectList.ps1"

foreach ($package in $packages) {
	cd .\$($package.Directory)
    
    Write-Host "Packing $package..."

	& dotnet pack --configuration Release --output "..\.nupkgs" | Out-Host
	cd ..
}
