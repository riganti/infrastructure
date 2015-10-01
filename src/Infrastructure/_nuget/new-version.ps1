param([String]$version)

$versionWithoutPre = $version
if ($versionWithoutPre.Contains("-")) {
	$versionWithoutPre = $versionWithoutPre.Substring(0, $versionWithoutPre.IndexOf("-"))
}

$nuspecPath = "Riganti.Utils.Infrastructure.Core.nuspec"
$nuspecPath2 = "Riganti.Utils.Infrastructure.EntityFramework.nuspec"
$assemblyInfoPath = "..\Riganti.Utils.Infrastructure.Core\Properties\AssemblyInfo.cs"
$assemblyInfoPath2 = "..\Riganti.Utils.Infrastructure.EntityFramework\Properties\AssemblyInfo.cs"

# change the nuspec
$nuspec = [System.IO.File]::ReadAllText($nuspecPath, [System.Text.Encoding]::UTF8)
$nuspec = [System.Text.RegularExpressions.Regex]::Replace($nuspec, "\<version\>([^""]+)\</version\>", "<version>" + $version + "</version>")
[System.IO.File]::WriteAllText($nuspecPath, $nuspec, [System.Text.Encoding]::UTF8)

# change the nuspec 2
$nuspec = [System.IO.File]::ReadAllText($nuspecPath2, [System.Text.Encoding]::UTF8)
$nuspec = [System.Text.RegularExpressions.Regex]::Replace($nuspec, "\<version\>([^""]+)\</version\>", "<version>" + $version + "</version>")
$nuspec = [System.Text.RegularExpressions.Regex]::Replace($nuspec, "\<dependency id=""Riganti\.Utils\.Infrastructure\.Core"" version=""([^""]+)"" /\>", "<dependency id=""Riganti.Utils.Infrastructure.Core"" version=""" + $version + """ />")
[System.IO.File]::WriteAllText($nuspecPath2, $nuspec, [System.Text.Encoding]::UTF8)

# change the AssemblyInfo project file
$assemblyInfo = [System.IO.File]::ReadAllText($assemblyInfoPath, [System.Text.Encoding]::UTF8)
$assemblyInfo = [System.Text.RegularExpressions.Regex]::Replace($assemblyInfo, "\[assembly: AssemblyVersion\(""([^""]+)""\)\]", "[assembly: AssemblyVersion(""" + $versionWithoutPre + """)]")
$assemblyInfo = [System.Text.RegularExpressions.Regex]::Replace($assemblyInfo, "\[assembly: AssemblyFileVersion\(""([^""]+)""\)]", "[assembly: AssemblyFileVersion(""" + $versionWithoutPre + """)]")
[System.IO.File]::WriteAllText($assemblyInfoPath, $assemblyInfo, [System.Text.Encoding]::UTF8)

# change the AssemblyInfo project file
$assemblyInfo = [System.IO.File]::ReadAllText($assemblyInfoPath2, [System.Text.Encoding]::UTF8)
$assemblyInfo = [System.Text.RegularExpressions.Regex]::Replace($assemblyInfo, "\[assembly: AssemblyVersion\(""([^""]+)""\)\]", "[assembly: AssemblyVersion(""" + $versionWithoutPre + """)]")
$assemblyInfo = [System.Text.RegularExpressions.Regex]::Replace($assemblyInfo, "\[assembly: AssemblyFileVersion\(""([^""]+)""\)\]", "[assembly: AssemblyFileVersion(""" + $versionWithoutPre + """)]")
[System.IO.File]::WriteAllText($assemblyInfoPath2, $assemblyInfo, [System.Text.Encoding]::UTF8)

