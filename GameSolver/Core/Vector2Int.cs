namespace GameSolver.Core;

public struct Vector2Int
{
    public static Vector2Int Up { get; }
    public static  Vector2Int Left { get; }
    public static  Vector2Int Down { get; }
    public static  Vector2Int Right { get; }
        
    public int X { get; set; }
    public int Y { get; set; }

    public Vector2Int(int x, int y)
    {
        X = x;
        Y = y;
    }

    static Vector2Int()
    {
        Up = new Vector2Int(0, -1);
        Left = new Vector2Int(-1, 0);
        Down = new Vector2Int(0, 1);
        Right = new Vector2Int(1, 0);
    }

    public override string ToString()
    {
        return $"x: {X}, y: {Y}";
    }
}