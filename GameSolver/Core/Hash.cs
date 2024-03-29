﻿namespace GameSolver.Core;

public static class Hash
{
    // Hash Setting
    public const int StateCount = 11; // PlayerDirUp, PlayerDirLeft, PlayerDirDown, PlayerDirRight, Score
    public const int DirUp = 0;
    public const int DirLeft = 1;
    public const int DirDown = 2;
    public const int DirRight = 3;
    public const int Score = 4;
    public const int Key = 5;
    public const int DoorUp = 6;
    public const int DoorLeft = 7;
    public const int DoorDown = 8;
    public const int DoorRight = 9;
    public const int Condition = 10;

    private static readonly Dictionary<Direction, int> DirToHash = new Dictionary<Direction, int>
    {
        {Direction.Up, DirUp},
        {Direction.Left, DirLeft},
        {Direction.Down, DirDown},
        {Direction.Right, DirRight}
    };
    
    public static int DirectionToHashIndex(Direction direction)
    {
        if (!DirToHash.ContainsKey(direction))
        {
            throw new ArgumentException("given direction is invalid", nameof(direction));
        }

        return DirToHash[direction];
    }
}