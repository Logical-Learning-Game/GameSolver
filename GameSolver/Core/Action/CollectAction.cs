using System.ComponentModel;

namespace GameSolver.Core.Action;

public class CollectAction : IGameAction
{
    private readonly MoveAction _moveAction;
    private readonly int _tile;

    public CollectAction(MoveAction moveAction, int tile)
    {
        _moveAction = moveAction;
        _tile = tile;
    }

    public void Do(State state)
    {
        _moveAction.Do(state);
        
        // Collect some item
        Collect(state, _tile);
    }

    public void Undo(State state)
    {
        // Undo collect some item
        Drop(state, _tile);
        
        _moveAction.Undo(state);
    }
    
    private static void Collect(State state, int tile)
    {
        Vector2Int playerPos = state.PlayerPosition;
        int currentTile = state.Board[playerPos.Y, playerPos.X];

        if ((currentTile & tile) <= 0)
        {
            throw new ArgumentException($"provided tile not found on current player's tile", nameof(tile));
        }

        int hashIndex;
        switch (tile)
        {
            case Tile.Score:
                hashIndex = Hash.Score;
                state.ScoreTiles.RemoveAll(v => v.X == playerPos.X && v.Y == playerPos.Y);
                break;
            case Tile.Key:
                hashIndex = Hash.Key;
                state.Keys++;
                state.KeyTiles.RemoveAll(v => v.X == playerPos.X && v.Y == playerPos.Y);
                break;
            default:
                throw new InvalidEnumArgumentException(nameof(tile), tile, tile.GetType());
        }
        
        int boardWidth = state.Board.GetLength(1);
        
        int tile1dPos = Core.Game.ColRowToTileIndex(playerPos.Y, playerPos.X, boardWidth);
        state.Board[playerPos.Y, playerPos.X] &= ~tile;
        state.ZobristHash ^= state.Game.HashComponent[tile1dPos, hashIndex];
    }

    private static void Drop(State state, int tile)
    {
        Vector2Int playerPos = state.PlayerPosition;

        int hashIndex;
        switch (tile)
        {
            case Tile.Score:
                hashIndex = Hash.Score;
                state.ScoreTiles.Add(playerPos);
                break;
            case Tile.Key:
                hashIndex = Hash.Key;
                state.Keys--;
                state.KeyTiles.Add(playerPos);
                break;
            default:
                throw new InvalidEnumArgumentException(nameof(tile), tile, tile.GetType());
        }
        
        int boardWidth = state.Board.GetLength(1);

        int tile1DPos = Core.Game.ColRowToTileIndex(playerPos.Y, playerPos.X, boardWidth);
        state.Board[playerPos.Y, playerPos.X] |= tile;
        state.ZobristHash ^= state.Game.HashComponent[tile1DPos, hashIndex];
    }

    public override string ToString()
    {
        return _moveAction.ToString();
    }
}