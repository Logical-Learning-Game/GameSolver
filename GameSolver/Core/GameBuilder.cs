using GameSolver.Solver.ShortestCommand;

namespace GameSolver.Core;

public sealed class GameBuilder
{
    public string InitialBoardString { get; set; }
    public Direction StartPlayerDirection { get; set; }
    public Game Instance { get; set; }
    
    public GameBuilder(string initialBoardString, Direction startPlayerDirection)
    {
        InitialBoardString = initialBoardString;
        StartPlayerDirection = startPlayerDirection;
        Instance = new Game();
        
        BoardParse();
    }

    public GameBuilder AddItem(Vector2Int position, int item)
    {
        Instance.Board[position.Y, position.X] |= item;

        if (item == TileComponent.Score.Value)
        {
            Instance.ScoreTiles.Add(position);
        }
        
        return this;
    }

    public GameBuilder AddDoor(Vector2Int position, Direction direction)
    {
        return this;
    }

    public void Build()
    {
        Instance.HashComponent = CreateZobristHashComponent(Instance.Board);
    }
    
    public void Clear()
    {
        Instance = new Game();
    }
     
    private void BoardParse()
    {
        string trimmedBoard = InitialBoardString.Trim();
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
                else if (TileComponent.Conditional.Any(tile))
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

        Instance.Board = boardMatrix;
        Instance.StartPlayerTile = startPlayerTile.Value;
        Instance.ScoreTiles = scores.ToList();
        Instance.KeyTiles = keys.ToList();
        Instance.ConditionalTiles = conditions.ToList();
        Instance.Keys = 0;
        Instance.Condition = ConditionalType.None;
        Instance.DoorTiles = doors.ToList();
        Instance.GoalTile = goalTile.Value;
        Instance.StartPlayerDirection = StartPlayerDirection;
        Instance.HashComponent = CreateZobristHashComponent(Instance.Board);
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
            'U' => TileComponent.DoorUp,
            'L' => TileComponent.DoorLeft,
            'D' => TileComponent.DoorDown,
            'R' => TileComponent.DoorRight,
            'a' => TileComponent.ConditionalA,
            'b' => TileComponent.ConditionalB,
            'c' => TileComponent.ConditionalC,
            'd' => TileComponent.ConditionalD,
            'e' => TileComponent.ConditionalE,
            _ => throw new ArgumentOutOfRangeException(nameof(ch), ch, "char argument not in range")
        };

        return component;
    }
    
    private long[,] CreateZobristHashComponent(int[,] board) 
    {
        int boardHeight = board.GetLength(0);
        int boardWidth = board.GetLength(1);
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