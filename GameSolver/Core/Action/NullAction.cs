namespace GameSolver.Core.Action;

public sealed class NullAction : IGameAction
{
    public void Do(State state)
    {
        
    }

    public void Undo(State state)
    {
        
    }

    public override string ToString()
    {
        return "";
    }
}