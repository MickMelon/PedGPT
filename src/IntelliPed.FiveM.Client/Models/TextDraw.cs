using CitizenFX.Core.Native;
using CitizenFX.Core;

namespace IntelliPed.FiveM.Client.Models;

public class TextDraw
{
    protected Vector3 Position;
    protected string Text;
    protected Color Color;

    public TextDraw(Vector3 position, string text, Color? color = null)
    {
        Position = position;
        Text = text;
        Color = color ?? Color.White;
    }

    public virtual void Draw()
    {
        float screenX = 0f;
        float screenY = 0f;

        bool isOnScreen = API.World3dToScreen2d(
            Position.X,
            Position.Y,
            Position.Z,
            ref screenX,
            ref screenY);

        Vector3 gameplayCamCoords = API.GetGameplayCamCoords();

        float distance = API.GetDistanceBetweenCoords(
            gameplayCamCoords.X,
            gameplayCamCoords.Y,
            gameplayCamCoords.Z,
            Position.X,
            Position.Y,
            Position.Z,
            true);

        float scale = 1 / distance;
        float fov = 1 / API.GetGameplayCamFov() * 100;
        scale *= fov;

        if (isOnScreen)
        {
            API.SetTextScale(0f, scale);
            API.SetTextFont(0);
            API.SetTextProportional(true);
            API.SetTextColour(Color.R, Color.G, Color.B, Color.A);
            API.SetTextDropshadow(0, 0, 0, 0, 255);
            API.SetTextOutline();
            API.SetTextEntry("STRING");
            API.SetTextCentre(true);
            API.AddTextComponentString(Text);
            API.DrawText(screenX, screenY);
        }
    }
}