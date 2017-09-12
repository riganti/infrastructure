param([String]$version, [String]$apiKey, [String]$server, [String]$branchName, [String]$repoUrl, [String]$nugetRestoreAltSource = "")

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
    If (Test-Path "./.nupkgs"){
	    Remove-Item "./.nupkgs"
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
		
		& dotnet pack --configuration Release --output "..\.nupkgs" | Out-Host
		cd ..
	}
}

function PushPackages() {
    dotnet nuget push ".\.nupkgs\*.nupkg" --source $server --api-key $apiKey | Out-Host 
}

function GitCheckout() {
	invoke-git checkout $branchName
	invoke-git -c http.sslVerify=false pull $repoUrl $branchName
}


function GitTagVersion() {
	invoke-git tag "v$($version)" HEAD
    invoke-git commit -am "NuGet package version $version"
	invoke-git rebase HEAD $branchName
    invoke-git push --follow-tags $repoUrl $branchName
}

### Configuration

$packages = @(
	[pscustomobject]@{ Package = "Riganti.Utils.Infrastructure.Core"; Directory = "Riganti.Utils.Infrastructure.Core" },
	[pscustomobject]@{ Package = "Riganti.Utils.Infrastructure.EntityFramework"; Directory = "Riganti.Utils.Infrastructure.EntityFramework" },
	[pscustomobject]@{ Package = "Riganti.Utils.Infrastructure.EntityFrameworkCore"; Directory = "Riganti.Utils.Infrastructure.EntityFrameworkCore" },
	[pscustomobject]@{ Package = "Riganti.Utils.Infrastructure.Azure.TableStorage"; Directory = "Riganti.Utils.Infrastructure.Azure.TableStorage" },
	[pscustomobject]@{ Package = "Riganti.Utils.Infrastructure.Configuration"; Directory = "Riganti.Utils.Infrastructure.Configuration" },
	[pscustomobject]@{ Package = "Riganti.Utils.Infrastructure.Services"; Directory = "Riganti.Utils.Infrastructure.Services" },
	[pscustomobject]@{ Package = "Riganti.Utils.Infrastructure.Services.Logging"; Directory = "Riganti.Utils.Infrastructure.Services.Logging" },
	[pscustomobject]@{ Package = "Riganti.Utils.Infrastructure.Services.Logging.Email"; Directory = "Riganti.Utils.Infrastructure.Services.Logging.Email" },
	[pscustomobject]@{ Package = "Riganti.Utils.Infrastructure.Services.Logging.ApplicationInsights"; Directory = "Riganti.Utils.Infrastructure.Services.Logging.ApplicationInsights" },
	[pscustomobject]@{ Package = "Riganti.Utils.Infrastructure.Services.Mailing"; Directory = "Riganti.Utils.Infrastructure.Services.Mailing" }
	[pscustomobject]@{ Package = "Riganti.Utils.Infrastructure.Services.Azure"; Directory = "Riganti.Utils.Infrastructure.Services.Azure" },
	[pscustomobject]@{ Package = "Riganti.Utils.Infrastructure.Services.SendGrid"; Directory = "Riganti.Utils.Infrastructure.Services.SendGrid" },
	[pscustomobject]@{ Package = "Riganti.Utils.Infrastructure.Services.Amazon.SES"; Directory = "Riganti.Utils.Infrastructure.Services.Amazon.SES" },
	[pscustomobject]@{ Package = "Riganti.Utils.Infrastructure.Services.Smtp"; Directory = "Riganti.Utils.Infrastructure.Services.Smtp" },
	[pscustomobject]@{ Package = "Riganti.Utils.Infrastructure.DotVVM"; Directory = "Riganti.Utils.Infrastructure.DotVVM" },
	[pscustomobject]@{ Package = "Riganti.Utils.Infrastructure.AutoMapper"; Directory = "Riganti.Utils.Infrastructure.AutoMapper" },
	[pscustomobject]@{ Package = "Riganti.Utils.Infrastructure.AutoMapper.EntityFramework"; Directory = "Riganti.Utils.Infrastructure.AutoMapper.EntityFramework" },
	[pscustomobject]@{ Package = "Riganti.Utils.Infrastructure.AutoMapper.EntityFrameworkCore"; Directory = "Riganti.Utils.Infrastructure.AutoMapper.EntityFrameworkCore" },
	[pscustomobject]@{ Package = "Riganti.Utils.Infrastructure.AspNetCore"; Directory = "Riganti.Utils.Infrastructure.AspNetCore" },
	[pscustomobject]@{ Package = "Riganti.Utils.Infrastructure.SystemWeb"; Directory = "Riganti.Utils.Infrastructure.SystemWeb" }
)


### Publish Workflow

CleanOldGeneratedPackages;
GitCheckout;
SetVersion;
BuildPackages;
PushPackages;
GitTagVersion;
