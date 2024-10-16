﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CitizenFX.Core;
using CitizenFX.Core.Native;

namespace IntelliPed.FiveM.Client;

public class Program : BaseScript
{
    [EventHandler("onClientResourceStart")]
    public void OnClientResourceStart(string resourceName)
    {
        if (API.GetCurrentResourceName() != resourceName)
        {
            return;
        }

        API.SetPoliceIgnorePlayer(API.PlayerId(), true);
        API.SetDispatchCopsForPlayer(API.PlayerId(), false);
        API.SetMaxWantedLevel(0);
        API.SetEntityCoords(API.PlayerPedId(), 0f, 0f, 72f, false, false, false, false);

        API.NetworkOverrideClockTime(12, 0, 0);

        Debug.WriteLine("Client resource started");
    }

    [EventHandler("gameEventTriggered")]
    public void OnGameEventTriggered(string name, List<dynamic> data)
    {
        Debug.WriteLine($"Game event triggered: {name} | {string.Join(", ", data)}");
    }

    [Tick]
    public Task OnTick()
    {
        // Disable the wanted level
        //API.SetPlayerWantedLevel(API.PlayerId(), 0, false);
        //API.SetPlayerWantedLevelNow(API.PlayerId(), false);
        //await Delay(1000); // Wait for 1 second before checking again

        API.RestorePlayerStamina(API.PlayerId(), 1);
        return Task.FromResult(0);
    }

    [Command("gun")]
    public void GiveGun()
    {
        // don't ask me why I'm giving a gun to the player >:)
        API.GiveWeaponToPed(API.PlayerPedId(), (uint) API.GetHashKey("weapon_pistol"), 999, false, true);
    }
}