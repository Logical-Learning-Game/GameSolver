namespace GameSolver.Core;

public static class DirectionUtility
{
    // Direction Rotate
    private static readonly Direction[] TurnLeftRotate = { Direction.Left, Direction.Down, Direction.Right, Direction.Up };
    private static readonly Direction[] TurnRightRotate = { Direction.Right, Direction.Up, Direction.Left, Direction.Down };
    private static readonly Direction[] TurnBackRotate = { Direction.Down, Direction.Right, Direction.Up, Direction.Left };

    private static readonly Dictionary<Direction, Vector2Int> DirToVec2 = new Dictionary<Direction, Vector2Int>
    {
        {Direction.Up, Vector2Int.Up},
        {Direction.Left, Vector2Int.Left},
        {Direction.Down, Vector2Int.Down},
        {Direction.Right, Vector2Int.Right}
    };

    public static Direction RotateLeft(Direction direction)
    {
        return TurnLeftRotate[(int)direction];
    }

    public static Direction RotateRight(Direction direction)
    {
        return TurnRightRotate[(int)direction];
    }

    public static Direction RotateBack(Direction direction)
    {
        return TurnBackRotate[(int)direction];
    }
        
    public static Vector2Int DirectionToVector2(Direction direction)
    {
        if (!DirToVec2.ContainsKey(direction))
        {
            throw new ArgumentException("given direction is invalid", nameof(direction));
        }
        
        return DirToVec2[direction];
    }
}