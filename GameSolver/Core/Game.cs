using System.Text;
using GameSolver.Solver.ShortestCommand;

namespace GameSolver.Core;

public sealed class Game
{
    // Board
    public int[,] Board { get; set; }

    // Player
    public Vector2Int StartPlayerTile { get; set; }
    public Direction StartPlayerDirection { get; set; }
    public int KeysA { get; set; }
    public int KeysB { get; set; }
    public int KeysC { get; set; }
    public ConditionalType Condition { get; set; }


    // TilePosition
    public IList<Vector2Int> ScoreTiles { get; set; }
    public IList<Vector2Int> KeyTiles { get; set; }
    public IList<Vector2Int> DoorTiles { get; set; }
    public IList<Vector2Int> ConditionalTiles { get; set; }
    public Vector2Int GoalTile { get; set; }

    // HashComponent
    public long[,] HashComponent { get; set; }

    public Game()
    {
        Board = new int[0, 0];
        ScoreTiles = new List<Vector2Int>();
        KeyTiles = new List<Vector2Int>();
        DoorTiles = new List<Vector2Int>();
        ConditionalTiles = new List<Vector2Int>();
        HashComponent = new long[0, 0];
    }

    public static string BoardToString(int[,] board)
    {
        var strBuilder = new StringBuilder();
        for (int i = 0; i < board.GetLength(0); i++)
        {
            for (int j = 0; j < board.GetLength(1); j++)
            {
                int tile = board[i, j];

                strBuilder.Append(TileToChar(tile));
            }
            strBuilder.AppendLine();
        }
        return strBuilder.ToString();
    }

    public override string ToString()
    {
        return BoardToString(Board);
    }
        
    public static int ToOneDimension(int row, int col, int boardWidth)
    {
        return row * boardWidth + col;
    }

    public static int ToOneDimension(Vector2Int position, int boardWidth)
    {
        return position.Y * boardWidth + position.X;
    }

    public int ToOneDimension(Vector2Int position)
    {
        int boardWidth = Board.GetLength(1);
        return ToOneDimension(position, boardWidth);
    }

    public IList<int> StandardBoardFormat()
    {
        var result = new List<int>();
        int boardHeight = Board.GetLength(0);
        int boardWidth = Board.GetLength(1);

        for (int i = boardHeight - 1; i >= 0; i--)
        {
            for (int j = boardWidth - 1; j >= 0; j--)
            {
                int tile = Board[i, j];
                int standardValue = 0;

                // first 4 bit from LSB is player data
                if (TileComponent.Player.In(tile))
                {
                    standardValue |= 0b0001;

                    int rotationValue = StartPlayerDirection switch
                    {
                        Direction.Left => 0b00,
                        Direction.Up => 0b01,
                        Direction.Right => 0b10,
                        Direction.Down => 0b11,
                        _ => throw new ArgumentOutOfRangeException(nameof(StartPlayerDirection), StartPlayerDirection, "start player direction out of range")
                    };

                    standardValue |= rotationValue << 1;
                }
                
                // next 4 bit is tile data
                int tileData = 0b0000;
                if (TileComponent.Wall.In(tile))
                {
                    tileData = 0b0001;
                }
                else if (TileComponent.Goal.In(tile))
                {
                    tileData = 0b0010;
                }
                else if (TileComponent.ConditionalA.In(tile))
                {
                    tileData = 0b0011;
                }
                else if (TileComponent.ConditionalB.In(tile))
                {
                    tileData = 0b0100;
                }
                else if (TileComponent.ConditionalC.In(tile))
                {
                    tileData = 0b0101;
                }
                else if (TileComponent.ConditionalD.In(tile))
                {
                    tileData = 0b0110;
                }
                else if (TileComponent.ConditionalE.In(tile))
                {
                    tileData = 0b0111;
                }
                
                standardValue |= tileData << 4;
                
                // next 4 bit is item on tile data
                int itemOnTileData = 0b0000;
                if (TileComponent.KeyA.In(tile))
                {
                    itemOnTileData = 0b0001;
                }
                else if (TileComponent.KeyB.In(tile))
                {
                    itemOnTileData = 0b0010;
                }
                else if (TileComponent.KeyC.In(tile))
                {
                    itemOnTileData = 0b0011;
                }

                standardValue |= itemOnTileData << 8;

                // next 4 bit is north door data
                int northDoorData = 0b0000;
                if (TileComponent.DoorLeft.In(tile))
                {
                    northDoorData = 0b0001;

                    int doorType = 0b00;
                    if (TileComponent.DoorLeftNokey.In(tile))
                    {
                        doorType = 0b00;
                    }
                    else if (TileComponent.DoorLeftA.In(tile))
                    {
                        doorType = 0b01;
                    }
                    else if (TileComponent.DoorLeftB.In(tile))
                    {
                        doorType = 0b10;
                    }
                    else if (TileComponent.DoorLeftC.In(tile))
                    {
                        doorType = 0b11;
                    }

                    northDoorData |= doorType << 1;

                    int doorOpen = 0b0;
                    if (TileComponent.DoorLeftOpen.In(tile))
                    {
                        doorOpen = 0b1;
                    }

                    northDoorData |= doorOpen << 3;
                }

                standardValue |= northDoorData << 12;
                
                // next 4 bit is east door data
                int eastDoorData = 0b0000;
                if (TileComponent.DoorUp.In(tile))
                {
                    eastDoorData = 0b0001;

                    int doorType = 0b00;
                    if (TileComponent.DoorUpNokey.In(tile))
                    {
                        doorType = 0b00;
                    }
                    else if (TileComponent.DoorUpA.In(tile))
                    {
                        doorType = 0b01;
                    }
                    else if (TileComponent.DoorUpB.In(tile))
                    {
                        doorType = 0b10;
                    }
                    else if (TileComponent.DoorUpC.In(tile))
                    {
                        doorType = 0b11;
                    }

                    eastDoorData |= doorType << 1;

                    int doorOpen = 0b0;
                    if (TileComponent.DoorUpOpen.In(tile))
                    {
                        doorOpen = 0b1;
                    }

                    eastDoorData |= doorOpen << 3;
                }

                standardValue |= eastDoorData << 16;
                
                // next 4 bit is south door data
                int southDoorData = 0b0000;
                if (TileComponent.DoorRight.In(tile))
                {
                    southDoorData = 0b0001;

                    int doorType = 0b00;
                    if (TileComponent.DoorRightNokey.In(tile))
                    {
                        doorType = 0b00;
                    }
                    else if (TileComponent.DoorRightA.In(tile))
                    {
                        doorType = 0b01;
                    }
                    else if (TileComponent.DoorRightB.In(tile))
                    {
                        doorType = 0b10;
                    }
                    else if (TileComponent.DoorRightC.In(tile))
                    {
                        doorType = 0b11;
                    }

                    southDoorData |= doorType << 1;

                    int doorOpen = 0b0;
                    if (TileComponent.DoorRightOpen.In(tile))
                    {
                        doorOpen = 0b1;
                    }

                    southDoorData |= doorOpen << 3;
                }

                standardValue |= southDoorData << 20;
                
                // next 4 bit is west door data
                int westDoorData = 0b0000;
                if (TileComponent.DoorDown.In(tile))
                {
                    westDoorData = 0b0001;

                    int doorType = 0b00;
                    if (TileComponent.DoorDownNokey.In(tile))
                    {
                        doorType = 0b00;
                    }
                    else if (TileComponent.DoorDownA.In(tile))
                    {
                        doorType = 0b01;
                    }
                    else if (TileComponent.DoorDownB.In(tile))
                    {
                        doorType = 0b10;
                    }
                    else if (TileComponent.DoorDownC.In(tile))
                    {
                        doorType = 0b11;
                    }

                    westDoorData |= doorType << 1;

                    int doorOpen = 0b0;
                    if (TileComponent.DoorDownOpen.In(tile))
                    {
                        doorOpen = 0b1;
                    }

                    westDoorData |= doorOpen << 3;
                }

                standardValue |= westDoorData << 24;
                
                result.Add(standardValue);
            }
        }
        
        return result;
    }

    private static char TileToChar(int tile)
    {
        char ch;
        
        if (TileComponent.Player.In(tile))
        {
            ch = 'P';
        }
        else if (TileComponent.Goal.In(tile))
        {
            ch = 'G';
        }
        else if (TileComponent.Score.In(tile))
        {
            ch = '*';
        }
        else if (TileComponent.KeyA.In(tile))
        {
            ch = '1';
        }
        else if (TileComponent.KeyB.In(tile))
        {
            ch = '2';
        }
        else if (TileComponent.KeyC.In(tile))
        {
            ch = '3';
        }
        else if (TileComponent.Wall.In(tile))
        {
            ch = 'x';
        }
        else if (TileComponent.ConditionalA.In(tile))
        {
            ch = 'a';
        }
        else if (TileComponent.ConditionalB.In(tile))
        {
            ch = 'b';
        }
        else if (TileComponent.ConditionalC.In(tile))
        {
            ch = 'c';
        }
        else if (TileComponent.ConditionalD.In(tile))
        {
            ch = 'd';
        }
        else if (TileComponent.ConditionalE.In(tile))
        {
            ch = 'e';
        }
        else if (TileComponent.HaveDoorLink(tile))
        {
            ch = 'D';
        }
        else
        {
            ch = '.';
        }

        return ch;
    }
}