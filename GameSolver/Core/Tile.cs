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

public enum DoorType
{
    DoorNoKey,
    DoorA,
    DoorB,
    DoorC
}

public readonly struct TileComponent : IEquatable<TileComponent>, IEquatable<int>
{
    public static TileComponent Player { get; }
    public static TileComponent Floor { get; }
    public static TileComponent Wall { get; }
    public static TileComponent Score { get; }
    public static TileComponent Goal { get; }
    public static TileComponent Conditional { get; }
    public static TileComponent ConditionalA { get; }
    public static TileComponent ConditionalB { get; }
    public static TileComponent ConditionalC { get; }
    public static TileComponent ConditionalD { get; }
    public static TileComponent ConditionalE { get; }
    
    public static TileComponent DoorUpMask { get; }
    public static TileComponent DoorUp { get; }
    public static TileComponent DoorUpNokey { get; }
    public static TileComponent DoorUpA { get; }
    public static TileComponent DoorUpB { get; }
    public static TileComponent DoorUpC { get; }
    public static TileComponent DoorUpOpen { get; }
    
    public static TileComponent DoorRightMask { get; }
    public static TileComponent DoorRight { get; }
    public static TileComponent DoorRightNokey { get; }
    public static TileComponent DoorRightA { get; }
    public static TileComponent DoorRightB { get; }
    public static TileComponent DoorRightC { get; }
    public static TileComponent DoorRightOpen { get; }

    public static TileComponent DoorDownMask { get; }
    public static TileComponent DoorDown { get; }
    public static TileComponent DoorDownNokey { get; }
    public static TileComponent DoorDownA { get; }
    public static TileComponent DoorDownB { get; }
    public static TileComponent DoorDownC { get; }
    public static TileComponent DoorDownOpen { get; }
    
    public static TileComponent DoorLeftMask { get; }
    public static TileComponent DoorLeft { get; }
    public static TileComponent DoorLeftNokey { get; }
    public static TileComponent DoorLeftA { get; }
    public static TileComponent DoorLeftB { get; }
    public static TileComponent DoorLeftC { get; }
    public static TileComponent DoorLeftOpen { get; }
    
    public static TileComponent KeyA { get; }
    public static TileComponent KeyB { get; }
    public static TileComponent KeyC { get; }
    
    public int Value { get; }
    public int Mask { get; }

    public TileComponent(int value, int mask)
    {
        Value = value;
        Mask = mask;
    }

    static TileComponent()
    {
        Player = new TileComponent(1 << 0, 1 << 0);
        Floor = new TileComponent(1 << 1, 1 << 1);
        Wall = new TileComponent(1 << 2, 1 << 2);
        Score = new TileComponent(1 << 3, 1 << 3);
        KeyA = new TileComponent(1 << 4, 1 << 4);
        KeyB = new TileComponent(1 << 5, 1 << 5);
        KeyC = new TileComponent(1 << 6, 1 << 6);
        Goal = new TileComponent(1 << 7, 1 << 7);
        
        DoorUp = new TileComponent(1 << 8, 1 << 8);
        DoorUpMask = new TileComponent(0b11 << 9, 0b11 << 9);
        DoorUpNokey = new TileComponent(0, 0b11 << 9);
        DoorUpA = new TileComponent(0b01 << 9, 0b11 << 9);
        DoorUpB = new TileComponent(0b10 << 9, 0b11 << 9);
        DoorUpC = new TileComponent(0b11 << 9, 0b11 << 9);
        DoorUpOpen = new TileComponent(1 << 11, 1 << 11);
        
        DoorRight = new TileComponent(1 << 12, 1 << 12);
        DoorRightMask = new TileComponent(0b11 << 13, 0b11 << 13);
        DoorRightNokey = new TileComponent(0, 0b11 << 13);
        DoorRightA = new TileComponent(0b01 << 13, 0b11 << 13);
        DoorRightB = new TileComponent(0b10 << 13, 0b11 << 13);
        DoorRightC = new TileComponent(0b11 << 13, 0b11 << 13);
        DoorRightOpen = new TileComponent(1 << 15, 1 << 15);
        
        DoorDown = new TileComponent(1 << 16, 1 << 16);
        DoorDownMask = new TileComponent(0b11 << 17, 0b11 << 17);
        DoorDownNokey = new TileComponent(0, 0b11 << 17);
        DoorDownA = new TileComponent(0b01 << 17, 0b11 << 17);
        DoorDownB = new TileComponent(0b10 << 17, 0b11 << 17);
        DoorDownC = new TileComponent(0b11 << 17, 0b11 << 17);
        DoorDownOpen = new TileComponent(1 << 19, 1 << 19);
        
        DoorLeft = new TileComponent(1 << 20, 1 << 20);
        DoorLeftMask = new TileComponent(0b11 << 21, 0b11 << 21);
        DoorLeftNokey = new TileComponent(0, 0b11 << 21);
        DoorLeftA = new TileComponent(0b01 << 21, 0b11 << 21);
        DoorLeftB = new TileComponent(0b10 << 21, 0b11 << 21);
        DoorLeftC = new TileComponent(0b11 << 21, 0b11 << 21);
        DoorLeftOpen = new TileComponent(1 << 23, 1 << 23);

        Conditional = new TileComponent(0b111 << 24, 0b111 << 24);
        ConditionalA = new TileComponent(0b001 << 24, 0b111 << 24);
        ConditionalB = new TileComponent(0b010 << 24, 0b111 << 24);
        ConditionalC = new TileComponent(0b011 << 24, 0b111 << 24);
        ConditionalD = new TileComponent(0b100 << 24, 0b111 << 24);
        ConditionalE = new TileComponent(0b101 << 24, 0b111 << 24);
    }

    public static IReadOnlyCollection<TileComponent> AllComponents()
    {
        return new[]
        {
            Player, Floor, Wall, Score, KeyA, KeyB, KeyC, Goal, DoorUp, DoorUpOpen, DoorDown, DoorDownOpen,
            DoorLeft, DoorLeftOpen, DoorRight, DoorRightOpen, Conditional, ConditionalA, ConditionalB,
            ConditionalC, ConditionalD, ConditionalE
        };
    }
    
    public static bool HaveDoorLink(int tile)
    {
        return DoorUp.In(tile) || DoorLeft.In(tile) || DoorDown.In(tile) || DoorRight.In(tile);
    }
    
    public bool IsDoorLink()
    {
        //return Equals(DoorUp) || Equals(DoorLeft) || Equals(DoorDown) || Equals(DoorRight);
        return IsInMask(DoorUpMask) || IsInMask(DoorRightMask) || IsInMask(DoorDownMask) || IsInMask(DoorLeftMask);
    }

    public static int CreateDoor(DoorType doorType, Direction doorDirection, bool isOpen)
    {
        int doorTypeValue = 0b0001;
        switch (doorType)
        {
            case DoorType.DoorNoKey:
                doorTypeValue |= 0b0000;
                break;
            case DoorType.DoorA:
                doorTypeValue |= 0b0010;
                break;
            case DoorType.DoorB:
                doorTypeValue |= 0b0100;
                break;
            case DoorType.DoorC:
                doorTypeValue |= 0b0110;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(doorType), doorType, "invalid door type");
        }

        if (isOpen)
        {
            doorTypeValue |= 0b1000;
        }

        switch (doorDirection)
        {
            case Direction.Up:
                doorTypeValue <<= 8;
                break;
            case Direction.Right:
                doorTypeValue <<= 12;
                break;
            case Direction.Down:
                doorTypeValue <<= 16;
                break;
            case Direction.Left:
                doorTypeValue <<= 20;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(doorDirection), doorDirection, "invalid direction type");
        }
        
        return doorTypeValue;
    }
    
    public bool IsInMask(TileComponent mask)
    {
        return (Value & mask.Mask) > 0;
    }
    
    public static bool In(TileComponent component, int tile)
    {
        return component.In(tile);
    }

    public bool In(int tile)
    {
        return (tile & Mask) == Value;
    }

    public static bool Any(TileComponent component, int tile)
    {
        return component.Any(tile);
    }
    
    public bool Any(int tile)
    {
        return (tile & Value) > 0;
    }
    
    public bool Equals(int other)
    {
        return Value == other;
    }

    public bool Equals(TileComponent other)
    {
        return Value == other.Value && Mask == other.Mask;
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