using CitizenFX.Core;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntelliPed.FiveM.Client.Scripts;

public class Speech : BaseScript
{
    private readonly List<SpeechData> _speechData = [];

    [Tick]
    public Task OnTick()
    {
        foreach (SpeechData speechData in _speechData.ToList())
        {
            if (Game.GameTime - speechData.TimeStarted > speechData.DurationMs
                || speechData.Ped.Exists() == false)
            {
                _speechData.Remove(speechData);
                continue;
            }

            if (Game.PlayerPed.Position.DistanceToSquared(speechData.Ped.Position) <= 75)
            {
                speechData.PedTextDraw.Draw();
            }
        }

        return Task.FromResult(0);
    }

    [EventHandler("Speech")]
    public void OnSpeech(int pedNetworkId, string message)
    {
        Debug.WriteLine($"Received speech: {message}");

        _speechData.RemoveAll(_ => _.Ped.NetworkId == pedNetworkId);

        Ped ped = (Ped)Entity.FromNetworkId(pedNetworkId);
        PedTextDraw textDraw = new(ped, message, Color.White);
        SpeechData speechData = new(ped, textDraw, 5000);

        _speechData.Add(speechData);
    }

    private record SpeechData
    {
        public Ped Ped { get; }
        public PedTextDraw PedTextDraw { get; }
        public int TimeStarted { get; }
        public int DurationMs { get; }

        public SpeechData(Ped ped, PedTextDraw pedTextDraw, int durationMs)
        {
            Ped = ped;
            PedTextDraw = pedTextDraw;
            DurationMs = durationMs;
            TimeStarted = Game.GameTime;
        }
    }
}