namespace GameSolver.Core.Action;

public abstract class MoveActionFactory
{
    public abstract IGameAction CreateFrom(MoveAction moveAction);
    
    public IGameAction Up()
    {
        return CreateFrom(MoveAction.Up);
    }
    
    public IGameAction Left()
    {
        return CreateFrom(MoveAction.Left);
    }
    
    public IGameAction Down()
    {
        return CreateFrom(MoveAction.Down);
    }
    
    public IGameAction Right()
    {
        return CreateFrom(MoveAction.Right);
    }
}