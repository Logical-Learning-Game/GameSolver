namespace GameSolver.Core.Action;

public class CollectAction : IGameAction
{
    private readonly MoveAction _moveAction;
    private readonly TileComponent _component;

    public CollectAction(MoveAction moveAction, TileComponent component)
    {
        _moveAction = moveAction;
        _component = component;
    }

    public void Do(State state)
    {
        _moveAction.Do(state);
        
        // Collect some item
        Collect(state, _component);
    }

    public void Undo(State state)
    {
        // Undo collect some item
        Drop(state, _component);
        
        _moveAction.Undo(state);
    }
    
    private static void Collect(State state, TileComponent component)
    {
        Vector2Int playerPos = state.PlayerPosition;
        int currentTile = state.Board[playerPos.Y, playerPos.X];

        if (!component.In(currentTile))
        {
            throw new ArgumentException($"provided tile not found on current player's tile", nameof(component));
        }

        int hashIndex;
        if (component.Equals(TileComponent.Score))
        {
            hashIndex = Hash.Score;
            state.ScoreTiles.RemoveAll(v => v.Equals(playerPos));
        }
        else if (component.Equals(TileComponent.Key))
        {
            hashIndex = Hash.Key;
            state.Keys++;
            state.KeyTiles.RemoveAll(v => v.Equals(playerPos));
        }
        else
        {
            throw new ArgumentException("component cannot be collected by player", nameof(component));
        }
        
        state.RemoveComponent(playerPos, component);
        state.UpdateZobristHash(playerPos, hashIndex);
    }

    private static void Drop(State state, TileComponent component)
    {
        Vector2Int playerPos = state.PlayerPosition;

        int hashIndex;
        if (component.Equals(TileComponent.Score))
        {
            hashIndex = Hash.Score;
            state.ScoreTiles.Add(playerPos);
        }
        else if (component.Equals(TileComponent.Key))
        {
            hashIndex = Hash.Key;
            state.Keys--;
            state.KeyTiles.Add(playerPos);
        }
        else
        {
            throw new ArgumentException("component cannot be dropped by player", nameof(component));
        }
        
        state.AddComponent(playerPos, component);
        state.UpdateZobristHash(playerPos, hashIndex);
    }

    public override string ToString()
    {
        return _moveAction.ToString();
    }
}