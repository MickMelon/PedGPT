using CitizenFX.Core;

namespace IntelliPed.FiveM.Client.Models;

public class PedTextDraw : TextDraw
{
    private readonly Ped _ped;
    private readonly float _heightOffset;

    public PedTextDraw(Ped ped, string text, Color? color = null, float heightOffset = 0.5f) : base(ped.Position, text, color)
    {
        _ped = ped;
        _heightOffset = heightOffset;
    }

    public override void Draw()
    {
        Position = _ped.Bones[Bone.SKEL_Head].Position;
        Position.Z += _heightOffset;
        base.Draw();
    }
}