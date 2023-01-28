namespace GameSolver.Core.Action;

public sealed class ObserveConditionAction : IGameAction
{
    private readonly IGameAction _gameAction;
    private bool _isObserved;
    private bool _haveOldCondition;

    public ObserveConditionAction(IGameAction gameAction)
    {
        _gameAction = gameAction;
    }

    public void Do(State state)
    {
        _gameAction.Do(state);
        
        Vector2Int playerPos = state.PlayerPosition;
        int currentTile = state.Board[playerPos.Y, playerPos.X];

        if (TileComponent.Conditional.In(currentTile))
        {
            _isObserved = true;

            if (state.Conditions == 1)
            {
                _haveOldCondition = true;
            }
            
            state.Conditions = 1;
        }
    }

    public void Undo(State state)
    {
        if (!_isObserved)
        {
            return;
        }

        state.Conditions = _haveOldCondition ? 1 : 0;

        _gameAction.Undo(state);
    }

    public bool Equals(IGameAction? other)
    {
        if (other is null)
        {
            return false;
        }
        
        return ReferenceEquals(this, other) || other.Equals(_gameAction);
    }

    public override string ToString()
    {
        string? toStringResult = _gameAction.ToString();
        
        return string.IsNullOrEmpty(toStringResult) ? string.Empty : toStringResult;
    }
}