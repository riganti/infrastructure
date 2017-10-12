param([String]$branchName, [String]$repoUrl)

$scriptRoot = Split-Path $MyInvocation.MyCommand.Path
. "$scriptRoot\Invoke-Git.ps1"

git config user.name "VSTS Build Agent"
git config user.email TFSBA

invoke-git checkout $branchName
invoke-git -c http.sslVerify=false pull $repoUrl $branchName
