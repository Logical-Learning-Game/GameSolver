namespace GameSolver.Core;

public static class Tile
{
    // Tile Numeric
    public const int Player = 1 << 0;
    public const int Floor = 1 << 1;
    public const int Wall = 1 << 2;
    public const int Score = 1 << 3;
    public const int Key = 1 << 4;
    public const int Goal = 1 << 5;
    public const int DoorUp = 1 << 6;
    public const int DoorUpOpen = 1 << 7;
    public const int DoorLeft = 1 << 8;
    public const int DoorLeftOpen = 1 << 9;
    public const int DoorDown = 1 << 10;
    public const int DoorDownOpen = 1 << 11;
    public const int DoorRight = 1 << 12;
    public const int DoorRightOpen = 1 << 13;

    // Tile Character
    public const char ChPlayer = 'P';
    public const char ChFloor = '.';
    public const char ChWall = 'x';
    public const char ChScore = '*';
    public const char ChKey = 'k';
    public const char ChGoal = 'G';
    public const char ChDoor = 'd';
    public const char ChDoorLinkUp = 'u';
    public const char ChDoorLinkLeft = 'l';
    public const char ChDoorLinkDown = 'd';
    public const char ChDoorLinkRight = 'r';

    public static char TileToChar(int tile)
    {
        char retVal = ChFloor;
        int check = tile & (Player + Wall + Goal + Score + Key);

        switch (check)
        {
            case Player:
                retVal = ChPlayer;
                break;
            case Goal:
                retVal = ChGoal;
                break;
            case Score:
                retVal = ChScore;
                break;
            case Key:
                retVal = ChKey;
                break;
            case Wall:
                retVal = ChWall;
                break;
            default:
                if ((tile & (DoorUp + DoorDown + DoorLeft + DoorRight)) > 0)
                {
                    retVal = ChDoor;
                }
                break;
        }

        return retVal;
    }

    public static int CharToTile(char tile)
    {
        int retVal = tile switch
        {
            ChPlayer => Player + Floor,
            ChFloor => Floor,
            ChGoal => Goal + Floor,
            ChScore => Score + Floor,
            ChKey => Key + Floor,
            ChWall => Wall,
            _ => Floor
        };

        return retVal;
    }

    public static bool IsDoorLink(char tile)
    {
        return tile is ChDoorLinkUp or ChDoorLinkLeft or ChDoorLinkDown or ChDoorLinkRight;
    }
}