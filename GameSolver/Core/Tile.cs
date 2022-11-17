namespace GameSolver.Core;

public static class Tile
{
    // Tile Numeric
    public const int Player = 1 << 0;
    public const int Floor = 1 << 1;
    public const int Wall = 1 << 2;
    public const int Score = 1 << 3;
    public const int Goal = 1 << 4;
    
    // Tile Character
    public const char ChPlayer = 'P';
    public const char ChFloor = '.';
    public const char ChWall = 'x';
    public const char ChScore = '*';
    public const char ChGoal = 'G';
    
    public static char TileToChar(int tile)
    {
        char retVal = Tile.ChFloor;
        int check = tile & (Tile.Player + Tile.Wall + Tile.Goal + Tile.Score);

        switch (check)
        {
            case Tile.Player:
                retVal = Tile.ChPlayer;
                break;
            case Tile.Goal:
                retVal = Tile.ChGoal;
                break;
            case Tile.Score:
                retVal = Tile.ChScore;
                break;
            case Tile.Wall:
                retVal = Tile.ChWall;
                break;
        }

        return retVal;
    }

    public static int CharToTile(char tile)
    {
        int retVal = 0;

        switch (tile)
        {
            case Tile.ChPlayer:
                retVal = Tile.Player + Tile.Floor;
                break;
            case Tile.ChFloor:
                retVal = Tile.Floor;
                break;
            case Tile.ChGoal:
                retVal = Tile.Goal + Tile.Floor;
                break;
            case Tile.ChScore:
                retVal = Tile.Score + Tile.Floor;
                break;
            case Tile.ChWall:
                retVal = Tile.Wall;
                break;
        }

        return retVal;
    }
}