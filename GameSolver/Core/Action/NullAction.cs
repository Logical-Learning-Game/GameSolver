namespace GameSolver.Core.Action;

public sealed class NullAction : IGameAction
{
    public void Do(State state)
    {
        
    }

    public void Undo(State state)
    {
        
    }

    public bool Equals(IGameAction? other)
    {
        return ReferenceEquals(this, other);
    }

    public override string ToString()
    {
        return string.Empty;
    }
}