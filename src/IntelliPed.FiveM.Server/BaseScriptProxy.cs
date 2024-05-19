using CitizenFX.Core;

namespace IntelliPed.FiveM.Server;

public class BaseScriptProxy
{
    public PlayerList Players { get; }

    public BaseScriptProxy(PlayerList players)
    {
        Players = players;
    }
}