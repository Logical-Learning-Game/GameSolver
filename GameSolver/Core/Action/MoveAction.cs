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
    
    public virtual void Do(State state)
    {
        Vector2Int playerPos = state.PlayerPosition;
        Vector2Int nextPos = state.PlayerPosition;
        Direction nextDir = state.PlayerDirection;

        nextDir = ToMove switch
        {
            Move.Left => DirectionUtility.RotateLeft(state.PlayerDirection),
            Move.Down => DirectionUtility.RotateBack(state.PlayerDirection),
            Move.Right => DirectionUtility.RotateRight(state.PlayerDirection),
            _ => nextDir
        };

        Vector2Int nextDirVec2 = DirectionUtility.DirectionToVector2(nextDir);
        nextPos.Sum(nextDirVec2);
        
        state.RemoveComponent(playerPos, TileComponent.Player);
        state.UpdateZobristHash(playerPos, Hash.DirectionToHashIndex(state.PlayerDirection));
        
        state.AddComponent(nextPos, TileComponent.Player);
        state.UpdateZobristHash(nextPos, Hash.DirectionToHashIndex(nextDir));
            
        state.PlayerPosition = nextPos;
        state.PlayerDirection = nextDir;
    }

    public virtual void Undo(State state)
    {
        Vector2Int playerPos = state.PlayerPosition;
        Vector2Int prevPos = state.PlayerPosition;
        Direction prevDir = state.PlayerDirection;

        Vector2Int currentDir = DirectionUtility.DirectionToVector2(state.PlayerDirection);
        prevPos.Minus(currentDir);

        prevDir = ToMove switch
        {
            Move.Left => DirectionUtility.RotateRight(state.PlayerDirection),
            Move.Down => DirectionUtility.RotateBack(state.PlayerDirection),
            Move.Right => DirectionUtility.RotateLeft(state.PlayerDirection),
            _ => prevDir
        };
        
        state.RemoveComponent(playerPos, TileComponent.Player);
        state.UpdateZobristHash(playerPos, Hash.DirectionToHashIndex(state.PlayerDirection));
        
        state.AddComponent(prevPos, TileComponent.Player);
        state.UpdateZobristHash(prevPos, Hash.DirectionToHashIndex(prevDir));

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

    public bool Equals(IGameAction? other)
    {
        return other is MoveAction moveAction && moveAction.ToMove == ToMove;
    }
}