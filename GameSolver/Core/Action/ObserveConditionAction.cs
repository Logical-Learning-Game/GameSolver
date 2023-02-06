using GameSolver.Solver.ShortestCommand;

namespace GameSolver.Core.Action;

public sealed class ObserveConditionAction : IGameAction
{
    private readonly IGameAction _gameAction;
    private bool _isObserved;
    private ConditionalType _oldCondition;

    public ObserveConditionAction(IGameAction gameAction)
    {
        _gameAction = gameAction;
    }

    public void Do(State state)
    {
        _gameAction.Do(state);
        
        Vector2Int playerPos = state.PlayerPosition;
        int currentTile = state.Board[playerPos.Y, playerPos.X];

        if (TileComponent.Conditional.Any(currentTile))
        {
            _isObserved = true;
            _oldCondition = state.Condition;

            if (TileComponent.ConditionalA.In(currentTile))
            {
                state.Condition = ConditionalType.ConditionalA;
            }
            else if (TileComponent.ConditionalB.In(currentTile))
            {
                state.Condition = ConditionalType.ConditionalB;
            }
            else if (TileComponent.ConditionalC.In(currentTile))
            {
                state.Condition = ConditionalType.ConditionalC;
            }
            else if (TileComponent.ConditionalD.In(currentTile))
            {
                state.Condition = ConditionalType.ConditionalD;
            }
            else if (TileComponent.ConditionalE.In(currentTile))
            {
                state.Condition = ConditionalType.ConditionalE;
            }
        }
    }

    public void Undo(State state)
    {
        if (!_isObserved)
        {
            return;
        }

        state.Condition = _oldCondition;

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