namespace GameSolver.Core.Action;

public class MoveActionWrapper : MoveAction
{
    private readonly MoveAction _moveAction;

    protected MoveActionWrapper(MoveAction moveAction) : base(moveAction.ToMove)
    {
        _moveAction = moveAction;
    }

    public override void Do(State state)
    {
        _moveAction.Do(state);
    }

    public override void Undo(State state)
    {
        _moveAction.Undo(state);
    }
}