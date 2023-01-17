namespace GameSolver.Core.Action;

public sealed class CollectAction : IGameAction
{
    private readonly IGameAction _gameAction;

    private bool _isCollected;
    private TileComponent _collectedComponent;

    public CollectAction(IGameAction gameAction)
    {
        _gameAction = gameAction;
    }

    public void Do(State state)
    {
        _gameAction.Do(state);
        
        // Collect some item
        Collect(state);
    }

    public void Undo(State state)
    {
        // Undo collect some item
        Drop(state);
        
        _gameAction.Undo(state);
    }
    
    private void Collect(State state)
    {
        Vector2Int playerPos = state.PlayerPosition;
        int currentTile = state.Board[playerPos.Y, playerPos.X];

        // check if current tile have any collectible item
        _isCollected = true;
        int hashIndex = -1;
        
        if (TileComponent.Score.In(currentTile))
        {
            hashIndex = Hash.Score;
            _collectedComponent = TileComponent.Score;
            state.ScoreTiles.RemoveAll(v => v.Equals(playerPos));
        }
        else if (TileComponent.Key.In(currentTile))
        {
            hashIndex = Hash.Key;
            _collectedComponent = TileComponent.Key;
            state.KeyTiles.RemoveAll(v => v.Equals(playerPos));
            state.Keys++;
        }
        else if (TileComponent.Conditional.In(currentTile))
        {
            hashIndex = Hash.Condition;
            _collectedComponent = TileComponent.Conditional;
            state.ConditionalTiles.RemoveAll(v => v.Equals(playerPos));
            state.Conditions++;
        }
        else
        {
            _isCollected = false;
        }
        
        if (_isCollected)
        {
            state.RemoveComponent(playerPos, _collectedComponent);
            state.UpdateZobristHash(playerPos, hashIndex);
        }
    }

    private void Drop(State state)
    {
        if (_isCollected)
        {
            return;
        }

        int hashIndex;
        Vector2Int playerPos = state.PlayerPosition;
        
        if (TileComponent.Score.Equals(_collectedComponent))
        {
            hashIndex = Hash.Score;
            state.ScoreTiles.Add(playerPos);
        }
        else if (TileComponent.Key.Equals(_collectedComponent))
        {
            hashIndex = Hash.Key;
            state.Keys--;
            state.KeyTiles.Add(playerPos);
        }
        else if (TileComponent.Conditional.Equals(_collectedComponent))
        {
            hashIndex = Hash.Condition;
            state.Conditions--;
            state.ConditionalTiles.Add(playerPos);
        }
        else
        {
            throw new Exception("player is collected item but no tile component is matched");
        }

        state.AddComponent(playerPos, _collectedComponent);
        state.UpdateZobristHash(playerPos, hashIndex);
    }

    public override string ToString()
    {
        return _gameAction.ToString()!;
    }
}