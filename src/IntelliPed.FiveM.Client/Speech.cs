using CitizenFX.Core;
using CitizenFX.Core.Native;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntelliPed.FiveM.Client;

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

public class PedTextDraw : TextDraw
{
    private Ped _ped;

    public PedTextDraw(Ped ped, string text, Color color = null) : base(ped.Position, text, color)
    {
        _ped = ped;
    }

    public override void Draw()
    {
        _position = _ped.Bones[Bone.SKEL_Head].Position;
        _position.Z += 1f;
        base.Draw();
    }
}

public class TextDraw
{
    protected Vector3 _position;
    protected string _text;
    protected Color _color;

    public TextDraw(Vector3 position, string text, Color color = null)
    {
        _position = position;
        _text = text;
        _color = color ?? Color.White;
    }

    public virtual void Draw()
    {
        float screenX = 0f;
        float screenY = 0f;

        bool isOnScreen = API.World3dToScreen2d(
            _position.X,
            _position.Y,
            _position.Z,
            ref screenX,
            ref screenY);

        Vector3 gameplayCamCoords = API.GetGameplayCamCoords();

        float distance = API.GetDistanceBetweenCoords(
            gameplayCamCoords.X,
            gameplayCamCoords.Y,
            gameplayCamCoords.Z,
            _position.X,
            _position.Y,
            _position.Z,
            true);

        float scale = 1 / distance;
        float fov = 1 / API.GetGameplayCamFov() * 100;
        scale *= fov;

        if (isOnScreen)
        {
            API.SetTextScale(0f, scale);
            API.SetTextFont(0);
            API.SetTextProportional(true);
            API.SetTextColour(_color.R, _color.G, _color.B, _color.A);
            API.SetTextDropshadow(0, 0, 0, 0, 255);
            API.SetTextOutline();
            API.SetTextEntry("STRING");
            API.SetTextCentre(true);
            API.AddTextComponentString(_text);
            API.DrawText(screenX, screenY);
        }
    }
}

public class Color
{
    public int R { get; set; }
    public int G { get; set; }
    public int B { get; set; }
    public int A { get; set; }

    public Color(int r, int g, int b, int a = 255)
    {
        R = r;
        G = g;
        B = b;
        A = a;
    }

    public static Color Red => new Color(255, 0, 0);
    public static Color Blue => new Color(0, 255, 0);
    public static Color Green => new Color(0, 0, 255);
    public static Color White => new Color(255, 255, 255);
    public static Color Black => new Color(0, 0, 0);
    public static Color Purple => new Color(194, 162, 218);
}