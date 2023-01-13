using System.Numerics;

namespace GameSolver.Core;

public struct Vector2Int : IEquatable<Vector2Int>
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

    public Vector2Int Sum(Vector2Int vec)
    {
        X += vec.X;
        Y += vec.Y;
        return this;
    }

    public static Vector2Int Sum(Vector2Int a, Vector2Int b)
    {
        Vector2Int summedVec = a;
        return summedVec.Sum(b);
    }

    public Vector2Int Minus(Vector2Int vec)
    {
        X -= vec.X;
        Y -= vec.Y;
        return this;
    }

    public static Vector2Int Minus(Vector2Int a, Vector2Int b)
    {
        Vector2Int minusVec = a;
        return minusVec.Minus(b);
    }
    
    public Vector2Int Mult(int a)
    {
        X *= a;
        Y *= a;
        return this;
    }

    public static Vector2Int Mult(Vector2Int vec, int a)
    {
        Vector2Int multipliedVec = vec;
        return multipliedVec.Mult(a);
    }

    public Vector2Int RotateLeft()
    {
        int temp = X;
        X = Y;
        Y = -temp;
        return this;
    }

    public static Vector2Int RotateLeft(Vector2Int vec)
    {
        Vector2Int rotatedVec = vec;
        return rotatedVec.RotateLeft();
    }
    
    public Vector2Int RotateRight()
    {
        int temp = X;
        X = -Y;
        Y = temp;
        return this;
    }

    public static Vector2Int RotateRight(Vector2Int vec)
    {
        Vector2Int rotatedVec = vec;
        return rotatedVec.RotateLeft();
    }
    
    public bool Equals(Vector2Int other)
    {
        return X == other.X && Y == other.Y;
    }

    public override bool Equals(object? obj)
    {
        return obj is Vector2Int other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y);
    }
    
    public override string ToString()
    {
        return $"({X}, {Y})";
    }
}