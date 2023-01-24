namespace GameSolver.Core.Action;

public sealed class StartAction : IGameAction
{
    private readonly ObserveConditionAction _observe;
    
    public StartAction()
    {
        _observe = new ObserveConditionAction(new NullAction());
    }
    
    public void Do(State state)
    {
        _observe.Do(state);
    }

    public void Undo(State state)
    {
        _observe.Undo(state);
    }

    public override string ToString()
    {
        return "s";
    }
}