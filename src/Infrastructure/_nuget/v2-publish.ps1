param([String]$version, [String]$apiKey, [String]$server)

function Clean() {
	foreach ($package in $packages) {
		del ..\Riganti.Utils.Infrastructure.$package\bin\debug\*.nupkg -ErrorAction SilentlyContinue
	}
}

function SetVersion() {
  	foreach ($package in $packages) {
		$filePath = "..\Riganti.Utils.Infrastructure.$package\project.json"
		$file = [System.IO.File]::ReadAllText($filePath, [System.Text.Encoding]::UTF8)
		$file = [System.Text.RegularExpressions.Regex]::Replace($file, """version"": ""([^""]+)""", """version"": """ + $version + """")
		[System.IO.File]::WriteAllText($filePath, $file, [System.Text.Encoding]::UTF8)
	}  
}

function BuildPackages() {
	foreach ($package in $packages) {
		cd ..\Riganti.Utils.Infrastructure.$package
		& dotnet restore
		& dotnet pack
	}
	cd ..\_nuget
}

function PushPackages() {
	foreach ($package in $packages) {
		& .\nuget.exe push ..\Riganti.Utils.Infrastructure.$package\bin\debug\Riganti.Utils.Infrastructure.$package.$version.nupkg -source $server -apiKey $apiKey
	}
}



$packages = @("Core",
	"EntityFramework",
	"EntityFrameworkCore",
	"Services",
	"Services.Azure",
	"Services.SendGrid",
	"Services.Amazon.SES",
	"DotVVM",
	"AutoMapper",
	"AspNetCore",
	"SystemWeb"
)
Clean;
SetVersion;
BuildPackages;
PushPackages;
