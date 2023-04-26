namespace PedGPT.Core.Actions;

public class ActionLocator
{
    private readonly Dictionary<string, Type> _actionMap;

    public ActionLocator(Dictionary<string, Type> actionMap)
    {
        _actionMap = actionMap;
    }

    public IAction? Locate(string actionName)
    {
        if (!_actionMap.ContainsKey(actionName)) return null;

        var actionType = _actionMap[actionName];
        
        var constructor = actionType.GetConstructor(Type.EmptyTypes);
        
        var action = constructor?.Invoke(null);
        
        return action as IAction;
    }
}