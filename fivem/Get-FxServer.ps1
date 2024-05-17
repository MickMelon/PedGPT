[CmdletBinding(DefaultParameterSetName="Version")]
param (
    [Parameter(ParameterSetName="Version")]
    [string]
    $Version,

    [Parameter(ParameterSetName="ListVersion")]
    [switch]
    $ListVersion
)

$InformationPreference = "Continue"
$versionListUri = [uri]::new('https://runtime.fivem.net/artifacts/fivem/build_server_windows/master/')
$7zaBinary = "$env:ProgramFiles\7-Zip\7z.exe"
$config = Get-Content -Path "$PSScriptRoot/FXServer.conf.json" | ConvertFrom-Json -ErrorAction Stop
$fxServerDir = Join-Path -Path $PSScriptRoot -ChildPath $config.Location
$fxServerBinary = Join-Path -Path $fxServerDir -ChildPath "FXServer.exe"

# Test the installed version meets expected version
function Get-InstalledServerVersion
{
    if(-not (Test-Path -Path $fxServerBinary))
    {
        return -1
    }

    # Take the first line of --version
    $versionOutput = & $fxServerBinary --version | Select-Object -First 1

    if(-not ($versionOutput -match '.*v\d+\.\d+\.\d+\.(\d+).*'))
    {
        return -1
    }

    return $Matches[1]
}

function Install-Z7ip
{
    Write-Information "Installing 7Zip"
    $dlurl = 'https://7-zip.org/' + (Invoke-WebRequest -UseBasicParsing -Uri 'https://7-zip.org/' | Select-Object -ExpandProperty Links | Where-Object {($_.outerHTML -match 'Download')-and ($_.href -like "a/*") -and ($_.href -like "*-x64.exe")} | Select-Object -First 1 | Select-Object -ExpandProperty href)
    # modified to work without IE
    # above code from: https://perplexity.nl/windows-powershell/installing-or-updating-7-zip-using-powershell/
    $installerPath = Join-Path $env:TEMP (Split-Path $dlurl -Leaf)
    Invoke-WebRequest $dlurl -OutFile $installerPath
    Start-Process -FilePath $installerPath -Args "/S" -Verb RunAs -Wait
    Remove-Item $installerPath
}

Function Expand-7Zip {
    [CmdletBinding(HelpUri='http://gavineke.com/PS7Zip/Expand-7Zip')]
    Param(
        [Parameter(Mandatory=$True,Position=0,ValueFromPipelineByPropertyName=$True)]
        [ValidateScript({$_ | Test-Path -PathType Leaf})]
        [System.IO.FileInfo]$FullName,

        [Parameter()]
        [Alias('Destination')]
        [ValidateNotNullOrEmpty()]
        [string]$DestinationPath,

        [Parameter()]
        [switch]$Remove
    )
    
    Begin 
    {
        if(-not (Get-Command -Name $7zaBinary -ErrorAction SilentlyContinue))
        {
            Install-Z7ip
        }
    }
    
    Process {
        Write-Verbose -Message 'Extracting contents of compressed archive file'
        If ($DestinationPath) {
            & "$7zaBinary" x -o"$DestinationPath" "$FullName"
        } Else {
            & "$7zaBinary" x "$FullName"
        }

        If ($Remove) {
            Write-Verbose -Message 'Removing compressed archive file'
            Remove-Item -Path "$FullName" -Force
        }
    }
    
    End {}
    
}

function Get-FxVersionList
{
    Write-Information "Getting version list..."
    $webRequest = Invoke-WebRequest -Uri $versionListUri -UseBasicParsing

    $webRequest.Links |
        Where-Object href -Like '*.7z' |
        ForEach-Object {
        
            if($_.outerHTML -match '\<a href=.*?\".*\"\>\n\s*(.*)')
            {
                $description = $Matches[1]
                if($description -match '\d+')
                {
                    [pscustomobject]@{
                        Description = $description
                        Version = $Matches[0]
                        Link = [uri]::new($versionListUri, $_.href)
                    }
                }
            }
            elseif($_.href -match '\.\/(\d+)')
            {
                [pscustomobject]@{
                    Description = $Matches[1]
                    Version = $Matches[1]
                    Link = [uri]::new($versionListUri, $_.href)
                }
            }
        }
}

function Get-FxVersion
{
    [CmdletBinding()]
    param (
        [Parameter(Mandatory = $true)]
        [ValidateScript({$_.AbsoluteUri -like '*.7z'})]
        [Uri]
        $Uri
    )

    $tempFileLocation = Join-Path -Path $env:TEMP -ChildPath (New-Guid).Guid
    New-Item -Path $tempFileLocation -ItemType Directory | Out-Null
    $filename = $Uri.Segments | Select -Last 1
    $filePath = Join-Path -Path $tempFileLocation -ChildPath $filename
    
    try{
        Invoke-WebRequest -Uri $Uri -OutFile $filePath -UseBasicParsing

        if(Test-Path -Path $fxServerDir)
        {
            Remove-Item -Path $fxServerDir -Recurse -ErrorAction Stop
        }

        Expand-7Zip $filePath -DestinationPath $fxServerDir
    }
    finally{
        Remove-Item -Path $tempFileLocation -Recurse
    }
}

$desiredVersion = $config.ArtifactVersion
$installedVersion = Get-InstalledServerVersion
Write-Information "Installed version: $installedVersion. Desired: $desiredVersion"
if(-not $desiredVersion)
{
    Write-Warning "ArtifactVersion not found in FXServer.conf.json. Installing LATEST RECOMMENDED!"
    $desiredVersion = "LATEST RECOMMENDED*"
}

if($installedVersion -eq $desiredVersion)
{
    Write-Information "Server is the correct version!"
    exit 0
}

if(Test-Path -Path $fxServerDir)
{
    Remove-Item $fxServerDir -Force -Recurse
}

$versions = Get-FxVersionList
if($ListVersion.IsPresent)
{
    return $versions.Description
}


if([string]::IsNullOrEmpty($Version)) {
    
    $versionToDownload = $versions | Where-Object Description -like $desiredVersion

    if($null -eq $versionToDownload)
    {
        throw "Failed to find the version $desiredVersion. Try specifying a version in FXServer.conf.json"
    }

    Write-Information "Downloading ($($versionToDownload.Version))..."
}
else {
    $versionToDownload = $versions | Where-Object Version -eq $Version | Select-Object -First 1

    if($null -eq $versionToDownload)
    {
        throw "Failed to find version. Try running -ListVersion"
    }

    Write-Information "Downloading fxserver Version $($versionToDownload.Version) ($($versionToDownload.Link.AbsoluteUri))" 
}

Get-FxVersion -Uri $versionToDownload.Link
