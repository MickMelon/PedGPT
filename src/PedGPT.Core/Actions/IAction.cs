namespace PedGPT.Core.Actions;

public interface IAction
{
    Task<ActionResult> Execute(Dictionary<string, string> args);
}