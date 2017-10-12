param([String]$apiKey, [String]$server, [string]$enabled)

if (-Not ($enabled.ToLower() -eq "true")) {
  Write-Host "##vso[task.logissue type=warning;] Publishing to $server is disabled."
} else {
  Write-Host "Publishing to $server is enabled."
  dotnet nuget push ".\.nupkgs\*.nupkg" --source $server --api-key $apiKey | Out-Host
}
