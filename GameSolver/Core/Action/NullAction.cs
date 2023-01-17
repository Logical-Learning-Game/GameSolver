namespace GameSolver.Core.Action;

public class NullAction : IGameAction
{
    public void Do(State state)
    {
        
    }

    public void Undo(State state)
    {
        
    }

    public override string ToString()
    {
        return string.Empty;
    }
}