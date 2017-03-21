param([String]$version, [String]$apiKey, [String]$server, [String]$branchName, [String]$repoUrl, [String]$nugetRestoreAltSource = "", [bool]$pushTag)


### Helper Functions

function Invoke-Git {
<#
.Synopsis
Wrapper function that deals with Powershell's peculiar error output when Git uses the error stream.

.Example
Invoke-Git ThrowError
$LASTEXITCODE

#>
    [CmdletBinding()]
    param(
        [parameter(ValueFromRemainingArguments=$true)]
        [string[]]$Arguments
    )

    & {
        [CmdletBinding()]
        param(
            [parameter(ValueFromRemainingArguments=$true)]
            [string[]]$InnerArgs
        )
        git.exe $InnerArgs 2>&1
    } -ErrorAction SilentlyContinue -ErrorVariable fail @Arguments

    if ($fail) {
        $fail.Exception
    }
}

function CleanOldGeneratedPackages() {
	foreach ($package in $packages) {
		del .\$($package.Directory)\bin\debug\*.nupkg -ErrorAction SilentlyContinue
	}
}

function SetVersion() {
  	foreach ($package in $packages) {
		$filePath = ".\$($package.Directory)\$($package.Directory).csproj"
		$file = [System.IO.File]::ReadAllText($filePath, [System.Text.Encoding]::UTF8)
		$file = [System.Text.RegularExpressions.Regex]::Replace($file, "\<VersionPrefix\>([^<]+)\</VersionPrefix\>", "<VersionPrefix>" + $version + "</VersionPrefix>")
		$file = [System.Text.RegularExpressions.Regex]::Replace($file, "\<PackageVersion\>([^<]+)\</PackageVersion\>", "<PackageVersion>" + $version + "</PackageVersion>")
		[System.IO.File]::WriteAllText($filePath, $file, [System.Text.Encoding]::UTF8)
	}  
}

function BuildPackages() {
	foreach ($package in $packages) {
		cd .\$($package.Directory)
		
		if ($nugetRestoreAltSource -eq "") {
			& dotnet restore | Out-Host
		}
		else {
			& dotnet restore --source $nugetRestoreAltSource --source https://nuget.org/api/v2/ | Out-Host
		}
		
		& dotnet pack | Out-Host
		cd ..
	}
}

function PushPackages() {
	foreach ($package in $packages) {
		& .\Tools\nuget.exe push .\$($package.Directory)\bin\debug\$($package.Package).$version.nupkg -source $server -apiKey $apiKey | Out-Host
	}
}

function GitCheckout() {
	invoke-git checkout $branchName
	invoke-git -c http.sslVerify=false pull $repoUrl $branchName
}

function GitPush() {
	if ($pushTag) {
			invoke-git tag "v$($version)" HEAD
	}
	invoke-git commit -am "NuGet package version $version"
	invoke-git rebase HEAD $branchName
	invoke-git push --follow-tags $repoUrl $branchName
}



### Configuration

$packages = @(
	[pscustomobject]@{ Package = "Core"; Directory = "Riganti.Utils.Infrastructure.Core" },
	[pscustomobject]@{ Package = "EntityFramework"; Directory = "Riganti.Utils.Infrastructure.EntityFramework" },
	[pscustomobject]@{ Package = "EntityFrameworkCore"; Directory = "Riganti.Utils.Infrastructure.EntityFrameworkCore" },
	[pscustomobject]@{ Package = "Azure.TableStorage"; Directory = "Riganti.Utils.Infrastructure.Azure.TableStorage" },
	[pscustomobject]@{ Package = "Configuration"; Directory = "Riganti.Utils.Infrastructure.Configuration" },
	[pscustomobject]@{ Package = "Services"; Directory = "Riganti.Utils.Infrastructure.Services" },
	[pscustomobject]@{ Package = "Services.Azure"; Directory = "Riganti.Utils.Infrastructure.Services.Azure" },
	[pscustomobject]@{ Package = "Services.SendGrid"; Directory = "Riganti.Utils.Infrastructure.Services.SendGrid" },
	[pscustomobject]@{ Package = "Services.Amazon.SES"; Directory = "Riganti.Utils.Infrastructure.Services.Amazon.SES" },
	[pscustomobject]@{ Package = "Services.Smtp"; Directory = "Riganti.Utils.Infrastructure.Services.Smtp" },
	[pscustomobject]@{ Package = "DotVVM"; Directory = "Riganti.Utils.Infrastructure.DotVVM" },
	[pscustomobject]@{ Package = "AutoMapper"; Directory = "Riganti.Utils.Infrastructure.AutoMapper" },
	[pscustomobject]@{ Package = "AspNetCore"; Directory = "Riganti.Utils.Infrastructure.AspNetCore" },
	[pscustomobject]@{ Package = "SystemWeb"; Directory = "Riganti.Utils.Infrastructure.SystemWeb" }
)


### Publish Workflow

$versionWithoutPre = $version
if ($versionWithoutPre.Contains("-")) {
	$versionWithoutPre = $versionWithoutPre.Substring(0, $versionWithoutPre.IndexOf("-"))
}

CleanOldGeneratedPackages;
GitCheckout;
SetVersion;
BuildPackages;
PushPackages;
GitPush;
