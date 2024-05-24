using CitizenFX.Core;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IntelliPed.FiveM.Client.Scripts;

public class DamageScript : BaseScript
{
    private readonly Dictionary<int, int> _pedHealths = [];

    [Tick]
    private async Task OnTick()
    {
        Ped[] peds = World.GetAllPeds();

        foreach (Ped ped in peds)
        {
            if (ped.IsPlayer)
            {
                continue;
            }

            int currentHealth = ped.Health;

            if (_pedHealths.TryGetValue(ped.Handle, out int previousHealth))
            {
                if (currentHealth < previousHealth)
                {
                    Debug.WriteLine($"Ped {ped.Handle} took {previousHealth - currentHealth} damage!");
                    TriggerServerEvent("PedDamaged", ped.NetworkId, previousHealth, currentHealth);
                }
            }

            _pedHealths[ped.Handle] = currentHealth;
        }

        await Delay(500);
    }
}