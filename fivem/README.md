# Fizzy FiveM

Follow these instructions to set up and start your server:

1. Set the `ArtifactVersion` in [FXServer.conf.json](FXServer.conf.json) to the appropriate version. You can find all available versions on the [FiveM artifact build page](https://runtime.fivem.net/artifacts/fivem/build_server_windows/master/).
2. Execute [Get-FxServer.ps1](Get-FxServer.ps1) to download the FXServer version specified in step 1. This script does not download the server again if it already exists locally.
3. Make a copy of [server.local.cfg.example](server.local.cfg.example) to `server.local.cfg` and make the necessary updates. Ensure you set the `sv_licenseKey`, which you can obtain from [Cfx.re Keymaster](https://keymaster.fivem.net/).
4. Run [Start-Server.ps1](Start-Server.ps1) to launch the server.
