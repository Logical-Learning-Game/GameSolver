namespace GameSolver.Core;

// public static class Tile
// {
//     // Tile Numeric
//     public const int Player = 1 << 0;
//     public const int Floor = 1 << 1;
//     public const int Wall = 1 << 2;
//     public const int Score = 1 << 3;
//     public const int Key = 1 << 4;
//     public const int Goal = 1 << 5;
//     public const int DoorUp = 1 << 6;
//     public const int DoorUpOpen = 1 << 7;
//     public const int DoorLeft = 1 << 8;
//     public const int DoorLeftOpen = 1 << 9;
//     public const int DoorDown = 1 << 10;
//     public const int DoorDownOpen = 1 << 11;
//     public const int DoorRight = 1 << 12;
//     public const int DoorRightOpen = 1 << 13;
//
//     // Tile Character
//     public const char ChPlayer = 'P';
//     public const char ChFloor = '.';
//     public const char ChWall = 'x';
//     public const char ChScore = '*';
//     public const char ChKey = 'k';
//     public const char ChGoal = 'G';
//     public const char ChDoor = 'd';
//     public const char ChDoorLinkUp = 'u';
//     public const char ChDoorLinkLeft = 'l';
//     public const char ChDoorLinkDown = 'd';
//     public const char ChDoorLinkRight = 'r';
//
//     public static char TileToChar(int tile)
//     {
//         char retVal = ChFloor;
//         int check = tile & (Player + Wall + Goal + Score + Key);
//
//         switch (check)
//         {
//             case Player:
//                 retVal = ChPlayer;
//                 break;
//             case Goal:
//                 retVal = ChGoal;
//                 break;
//             case Score:
//                 retVal = ChScore;
//                 break;
//             case Key:
//                 retVal = ChKey;
//                 break;
//             case Wall:
//                 retVal = ChWall;
//                 break;
//             default:
//                 if ((tile & (DoorUp + DoorDown + DoorLeft + DoorRight)) > 0)
//                 {
//                     retVal = ChDoor;
//                 }
//                 break;
//         }
//
//         return retVal;
//     }
//
//     public static int CharToTile(char tile)
//     {
//         int retVal = tile switch
//         {
//             ChPlayer => Player + Floor,
//             ChFloor => Floor,
//             ChGoal => Goal + Floor,
//             ChScore => Score + Floor,
//             ChKey => Key + Floor,
//             ChWall => Wall,
//             _ => Floor
//         };
//
//         return retVal;
//     }
//
//     public static bool IsDoorLink(char tile)
//     {
//         return tile is ChDoorLinkUp or ChDoorLinkLeft or ChDoorLinkDown or ChDoorLinkRight;
//     }
//
//     public static bool IsItem(int tile)
//     {
//         return tile is Key or Score;
//     }
// }

public readonly struct TileComponent : IEquatable<TileComponent>, IEquatable<int>
{
    public static TileComponent Player { get; }
    public static TileComponent Floor { get; }
    public static TileComponent Wall { get; }
    public static TileComponent Score { get; }
    public static TileComponent Key { get; }
    public static TileComponent Goal { get; }
    public static TileComponent DoorUp { get; }
    public static TileComponent DoorUpOpen { get; }
    public static TileComponent DoorLeft { get; }
    public static TileComponent DoorLeftOpen { get; }
    public static TileComponent DoorDown { get; }
    public static TileComponent DoorDownOpen { get; }
    public static TileComponent DoorRight { get; }
    public static TileComponent DoorRightOpen { get; }

    public int Value { get; }

    public TileComponent(int value)
    {
        Value = value;
    }

    static TileComponent()
    {
        Player = new TileComponent(1 << 0);
        Floor = new TileComponent(1 << 1);
        Wall = new TileComponent(1 << 2);
        Score = new TileComponent(1 << 3);
        Key = new TileComponent(1 << 4);
        Goal = new TileComponent(1 << 5);
        DoorUp = new TileComponent(1 << 6);
        DoorUpOpen = new TileComponent(1 << 7);
        DoorLeft = new TileComponent(1 << 8);
        DoorLeftOpen = new TileComponent(1 << 9);
        DoorDown = new TileComponent(1 << 10);
        DoorDownOpen = new TileComponent(1 << 11);
        DoorRight = new TileComponent(1 << 12);
        DoorRightOpen = new TileComponent(1 << 13);
    }

    public static IReadOnlyCollection<TileComponent> AllComponents()
    {
        return new[]
        {
            Player, Floor, Wall, Score, Key, Goal, DoorUp, DoorUpOpen, DoorDown, DoorDownOpen,
        };
    }
    
    public static bool HaveDoorLink(int tile)
    {
        return DoorUp.In(tile) || DoorLeft.In(tile) || DoorDown.In(tile) || DoorRight.In(tile);
    }
    
    public bool IsDoorLink()
    {
        return Equals(DoorUp) || Equals(DoorLeft) || Equals(DoorDown) || Equals(DoorRight);
    }
    
    public static bool Have(int tile, TileComponent component)
    {
        return component.In(tile);
    }

    public bool In(int tile)
    {
        return (tile & Value) > 0;
    }
    
    public bool Equals(int other)
    {
        return Value == other;
    }

    public bool Equals(TileComponent other)
    {
        return Value == other.Value;
    }

    public override bool Equals(object? obj)
    {
        return obj is TileComponent component && Equals(component);
    }

    public override int GetHashCode()
    {
        return Value;
    }
}