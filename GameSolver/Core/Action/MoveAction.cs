using System.ComponentModel;

namespace GameSolver.Core.Action;

public class MoveAction : IGameAction
{
    public static MoveAction Up { get; }
    public static MoveAction Left { get; }
    public static MoveAction Down { get; }
    public static MoveAction Right { get; }

    public const char ChUp = 'u';
    public const char ChLeft = 'l';
    public const char ChDown = 'd';
    public const char ChRight = 'r';
    
    public Move ToMove { get; }

    public MoveAction(Move toMove)
    {
        ToMove = toMove;
    }

    static MoveAction()
    {
        Up = new MoveAction(Move.Up);
        Left = new MoveAction(Move.Left);
        Down = new MoveAction(Move.Down);
        Right = new MoveAction(Move.Right);
    }
    
    public void Do(State state)
    {
        Vector2Int playerPos = state.PlayerPosition;
        Vector2Int nextPos = state.PlayerPosition;
        Direction nextDir = state.PlayerDirection;
        long[,] hashComponent = state.Game.HashComponent;

        nextDir = ToMove switch
        {
            Move.Left => DirectionUtility.RotateLeft(state.PlayerDirection),
            Move.Down => DirectionUtility.RotateBack(state.PlayerDirection),
            Move.Right => DirectionUtility.RotateRight(state.PlayerDirection),
            _ => nextDir
        };

        Vector2Int nextDirVec2 = DirectionUtility.DirectionToVector2(nextDir);
        nextPos.X += nextDirVec2.X;
        nextPos.Y += nextDirVec2.Y;
        
        int boardWidth = state.Board.GetLength(1);
        int start1DPos = Core.Game.ColRowToTileIndex(playerPos.Y, playerPos.X, boardWidth);
        int end1DPos = Core.Game.ColRowToTileIndex(nextPos.Y, nextPos.X, boardWidth);
        
        state.Board[playerPos.Y, playerPos.X] &= ~Tile.Player;
        int hashIndex = Hash.DirectionToHashIndex(state.PlayerDirection);
        state.ZobristHash ^= hashComponent[start1DPos, hashIndex];
        
        state.Board[nextPos.Y, nextPos.X] |= Tile.Player;
        hashIndex = Hash.DirectionToHashIndex(nextDir);
        state.ZobristHash ^= hashComponent[end1DPos, hashIndex];
            
        state.PlayerPosition = nextPos;
        state.PlayerDirection = nextDir;
    }

    public void Undo(State state)
    {
        Vector2Int playerPos = state.PlayerPosition;
        Vector2Int prevPos = state.PlayerPosition;
        Direction prevDir = state.PlayerDirection;
        long[,] hashComponent = state.Game.HashComponent;

        Vector2Int currentDir = DirectionUtility.DirectionToVector2(state.PlayerDirection);
        prevPos.X -= currentDir.X;
        prevPos.Y -= currentDir.Y;

        prevDir = ToMove switch
        {
            Move.Left => DirectionUtility.RotateRight(state.PlayerDirection),
            Move.Down => DirectionUtility.RotateBack(state.PlayerDirection),
            Move.Right => DirectionUtility.RotateLeft(state.PlayerDirection),
            _ => prevDir
        };

        int boardWidth = state.Board.GetLength(1);
        int start1DPos = Core.Game.ColRowToTileIndex(playerPos.Y, playerPos.X, boardWidth);
        int end1DPos = Core.Game.ColRowToTileIndex(prevPos.Y, prevPos.X, boardWidth);
        
        state.Board[playerPos.Y, playerPos.X] &= ~Tile.Player;
        int hashIndex = Hash.DirectionToHashIndex(state.PlayerDirection);
        state.ZobristHash ^= hashComponent[start1DPos, hashIndex];
        
        state.Board[prevPos.Y, prevPos.X] |= Tile.Player;
        hashIndex = Hash.DirectionToHashIndex(prevDir);
        state.ZobristHash ^= hashComponent[end1DPos, hashIndex];

        state.PlayerPosition = prevPos;
        state.PlayerDirection = prevDir;
    }

    public override string ToString()
    {
        char ch = ToMove switch
        {
            Move.Up => ChUp,
            Move.Left => ChLeft,
            Move.Down => ChDown,
            Move.Right => ChRight,
            _ => throw new InvalidEnumArgumentException(nameof(ToMove), (int)ToMove, ToMove.GetType())
        };
        return ch.ToString();
    }
}