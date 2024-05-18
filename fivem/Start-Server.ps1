[CmdletBinding()]
param(
    # Sets the KeyServer key, and heartbeat settings
    [Parameter()]
    [ValidateSet('local', 'development', 'production')]
    $Environment = "local"
)

# Build
Push-Location -Path "$PSScriptRoot\..\src"
try {
    & dotnet build PedGPT.sln
}
finally {
    Pop-Location
}

$host.UI.RawUI.WindowTitle = "$Environment Server"
Push-Location -Path "$PSScriptRoot\cfx-server-data"
try {
    & "$PSScriptRoot\fxserver\FXServer.exe" +exec ..\server.$Environment.cfg
}
finally {
    Pop-Location
}
