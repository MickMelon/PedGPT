namespace PedGPT.Core.Actions;

public class DanceAction : IAction
{
    public Task<ActionResult> Execute(Dictionary<string, string> args)
    {
        return Task.FromResult(new ActionResult(true, "You are now dancing like Michael Jackson!"));
    }
}