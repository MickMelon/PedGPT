namespace PedGPT.Core.Actions;

public class WalkAction : IAction
{
    public Task<ActionResult> Execute(Dictionary<string, string> args)
    {
        var x = args.GetValueOrDefault("x");
        var y = args.GetValueOrDefault("y");
        var z = args.GetValueOrDefault("z");

        if (x is null || y is null || z is null) 
            return Task.FromResult(new ActionResult(false, "'walk' command takes x, y, and z args."));

        return Task.FromResult(new ActionResult(true, $"You have walked to the position '{x}, {y}, {z}'"));
    }
}