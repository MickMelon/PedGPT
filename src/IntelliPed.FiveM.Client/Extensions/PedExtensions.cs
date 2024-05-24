using CitizenFX.Core.Native;
using CitizenFX.Core;

namespace IntelliPed.FiveM.Client.Extensions;

public static class PedExtensions
{
    public static string GetCurrentStreet(this Entity entity)
    {
        uint streetHashKey = 0;
        uint crossingRoad = 0;
        API.GetStreetNameAtCoord(entity.Position.X, entity.Position.Y, entity.Position.Z, ref streetHashKey, ref crossingRoad);
        string streetName = API.GetStreetNameFromHashKey(streetHashKey);
        return streetName;
    }
}