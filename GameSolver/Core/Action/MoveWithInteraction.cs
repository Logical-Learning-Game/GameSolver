namespace GameSolver.Core.Action;

public sealed class MoveWithInteraction : MoveAction
{
    private readonly IGameAction _gameAction;

    public MoveWithInteraction(MoveAction moveAction) : base(moveAction.ToMove)
    {
        _gameAction = new ObserveConditionAction(new CollectAction(new OpenDoorAction(moveAction)));
    }

    public override void Do(State state)
    {
        _gameAction.Do(state);
    }

    public override void Undo(State state)
    {
        _gameAction.Undo(state);
    }
}