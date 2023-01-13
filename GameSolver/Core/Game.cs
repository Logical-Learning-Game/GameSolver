using System.Text;

namespace GameSolver.Core;

public sealed class Game
{
    // Board
    public int[,] Board { get; }

    // Player
    public Vector2Int StartPlayerTile { get; }
    public Direction StartPlayerDirection { get; set; }
    public int Keys { get; set; }
    public int Conditions { get; set; }


    // TilePosition
    public IEnumerable<Vector2Int> ScoreTiles { get; }
    public IEnumerable<Vector2Int> KeyTiles { get; }
    public IEnumerable<Vector2Int> DoorTiles { get; }
    public IEnumerable<Vector2Int> ConditionalTiles { get; }
    public Vector2Int GoalTile { get; }

    // HashComponent
    public long[,] HashComponent { get; }

    public Game(string boardStr, Direction startPlayerDirection)
    {
        string trimmedBoard = boardStr.Trim();
        string[] splitBoard = trimmedBoard.Split("\n");

        int height = splitBoard.Length;
        int width = 0;
        var trimmedBoardList = new List<string>();
        foreach (string line in splitBoard)
        {
            string trimmedLine = line.Trim();
            trimmedBoardList.Add(trimmedLine);
            width = Math.Max(width, trimmedLine.Length);
        }

        var boardMatrix = new int[height, width];
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                if (j < trimmedBoardList[i].Length)
                {
                    char tileChar = trimmedBoardList[i][j];
                    TileComponent component = GetComponentFromChar(tileChar);

                    if (component.IsDoorLink())
                    {
                        if (component.Equals(TileComponent.DoorUp))
                        {
                            boardMatrix[i, j] |= TileComponent.DoorUp.Value + TileComponent.Floor.Value;
                            boardMatrix[i - 1, j] |= TileComponent.DoorDown.Value + TileComponent.Floor.Value;
                        }
                        else if (component.Equals(TileComponent.DoorLeft))
                        {
                            boardMatrix[i, j] |= TileComponent.DoorLeft.Value + TileComponent.Floor.Value;
                            boardMatrix[i, j - 1] |= TileComponent.DoorRight.Value + TileComponent.Floor.Value;
                        }
                        else if (component.Equals(TileComponent.DoorDown))
                        {
                            boardMatrix[i, j] |= TileComponent.DoorDown.Value + TileComponent.Floor.Value;
                            boardMatrix[i + 1, j] |= TileComponent.DoorUp.Value + TileComponent.Floor.Value;
                        }
                        else if (component.Equals(TileComponent.DoorRight))
                        {
                            boardMatrix[i, j] |= TileComponent.DoorRight.Value + TileComponent.Floor.Value;
                            boardMatrix[i, j + 1] |= TileComponent.DoorUp.Value + TileComponent.Floor.Value;
                        }
                    }
                    else
                    {
                        boardMatrix[i, j] |= component.Value;
                    }
                }
                else
                {
                    boardMatrix[i, j] = TileComponent.Wall.Value;
                }
            }
        }

        // Computing starting values
        Vector2Int? startPlayerTile = null;
        Vector2Int? goalTile = null;
        var scores = new List<Vector2Int>();
        var keys = new List<Vector2Int>();
        var doors = new List<Vector2Int>();
        var conditions = new List<Vector2Int>();
        for (int i = 0; i < boardMatrix.GetLength(0); i++)
        {
            for (int j = 0; j < boardMatrix.GetLength(1); j++)
            {
                int tile = boardMatrix[i, j];

                if (TileComponent.Player.In(tile))
                {
                    startPlayerTile = new Vector2Int(j, i);
                }
                else if (TileComponent.Goal.In(tile))
                {
                    goalTile = new Vector2Int(j, i);
                }
                else if (TileComponent.Score.In(tile))
                {
                    scores.Add(new Vector2Int(j, i));
                }
                else if (TileComponent.Key.In(tile))
                {
                    keys.Add(new Vector2Int(j, i));
                }
                else if (TileComponent.Conditional.In(tile))
                {
                    conditions.Add(new Vector2Int(j, i));
                }
                else if (TileComponent.HaveDoorLink(tile))
                {
                    doors.Add(new Vector2Int(j, i));
                }
            }
        }

        if (startPlayerTile == null)
        {
            throw new Exception("start player tile should not be null");
        }

        if (goalTile == null)
        {
            throw new Exception("goal tile should not be null");
        }

        Board = boardMatrix;
        StartPlayerTile = startPlayerTile.Value;
        ScoreTiles = scores.ToArray();
        KeyTiles = keys.ToArray();
        ConditionalTiles = conditions.ToArray();
        Keys = 0;
        Conditions = 0;
        DoorTiles = doors.ToArray();
        GoalTile = goalTile.Value;
        StartPlayerDirection = startPlayerDirection;
        HashComponent = ConstructZobristHashComponent();
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

    private static char TileToChar(int tile)
    {
        char ch;
        
        if (TileComponent.HaveDoorLink(tile))
        {
            ch = 'd';
        }
        else
        {
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
            else if (TileComponent.Key.In(tile))
            {
                ch = 'k';
            }
            else if (TileComponent.Wall.In(tile))
            {
                ch = 'x';
            }
            else
            {
                ch = '.';
            }
        }

        return ch;
    }
    
    private static TileComponent GetComponentFromChar(char ch)
    {
        TileComponent component = ch switch
        {
            'P' => TileComponent.Player,
            '.' => TileComponent.Floor,
            'x' => TileComponent.Wall,
            '*' => TileComponent.Score,
            'k' => TileComponent.Key,
            'G' => TileComponent.Goal,
            'u' => TileComponent.DoorUp,
            'l' => TileComponent.DoorLeft,
            'd' => TileComponent.DoorDown,
            'r' => TileComponent.DoorRight,
            _ => throw new ArgumentOutOfRangeException(nameof(ch), ch, "char argument not in range")
        };

        return component;
    }
    
    private long[,] ConstructZobristHashComponent() 
    {
        int boardHeight = Board.GetLength(0);
        int boardWidth = Board.GetLength(1);
        int boardSize = boardHeight * boardWidth;

        int stateCount = Hash.StateCount;
        
        var zobristTable = new long[boardSize, stateCount];

        var selectedNumber = new HashSet<long>();
        var random = new Random();

        for (int i = 0; i < zobristTable.GetLength(0); i++)
        {
            for (int j = 0; j < zobristTable.GetLength(1); j++)
            {
                long rnd;
                do
                {
                    rnd = random.NextInt64();
                }
                while (rnd == 0 || selectedNumber.Contains(rnd));

                selectedNumber.Add(rnd);
                zobristTable[i, j] = rnd;
            }
        }

        return zobristTable;
    }
}