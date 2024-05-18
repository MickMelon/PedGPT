using CitizenFX.Core;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System;

namespace IntelliPed.FiveM.Server;

public class GameScript : BaseScript
{
    public static GameScript Instance { get; private set; } = null!;

    private readonly ConcurrentQueue<Action> _actions = new ConcurrentQueue<Action>();

    public GameScript()
    {
        Instance = this;
        Tick += OnTick;
    }

    public void ExecuteOnGameThread(Action action)
    {
        _actions.Enqueue(action);
    }

    private async Task OnTick()
    {
        while (_actions.TryDequeue(out var action))
        {
            action();
        }

        await Task.FromResult(0); // Yield control back to the game loop
    }
}