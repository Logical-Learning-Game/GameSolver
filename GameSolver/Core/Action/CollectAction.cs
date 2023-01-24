namespace GameSolver.Core.Action;

public sealed class CollectAction : MoveActionWrapper
{
    private bool _isCollected;
    private TileComponent _collectedComponent;

    public CollectAction(MoveAction moveAction) : base(moveAction) {}

    public override void Do(State state)
    {
        base.Do(state);
        
        // Collect some item
        Collect(state);
    }

    public override void Undo(State state)
    {
        // Undo collect some item
        Drop(state);
        
        base.Undo(state);
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
        else
        {
            throw new Exception("player is collected item but no tile component is matched");
        }

        state.AddComponent(playerPos, _collectedComponent);
        state.UpdateZobristHash(playerPos, hashIndex);
    }
}