using System.ComponentModel.Design.Serialization;
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

    public static void AddDoor(int[,] board, int x, int y, DoorType doorType, Direction doorDirection, bool isOpen)
    {
        int door = TileComponent.CreateDoor(doorType, doorDirection, isOpen);
        int doorPair = TileComponent.CreateDoor(doorType, DirectionUtility.RotateBack(doorDirection), isOpen);
        
        if (doorDirection == Direction.Up)
        {
            board[y, x] |= door;
            if (!GameUtility.OutOfBoundCheck(board, x, y - 1))
            {
                board[y - 1, x] |= doorPair;
            }
        }
        else if (doorDirection == Direction.Right)
        {
            board[y, x] |= door;
            if (!GameUtility.OutOfBoundCheck(board, x + 1, y))
            {
                board[y, x + 1] |= doorPair;
            }
        }
        else if (doorDirection == Direction.Down)
        {
            board[y, x] |= door;
            if (!GameUtility.OutOfBoundCheck(board, x, y + 1))
            {
                board[y + 1, x] |= doorPair;
            }
        }
        else if (doorDirection == Direction.Left)
        {
            board[y, x] |= door;
            if (!GameUtility.OutOfBoundCheck(board, x - 1, y))
            {
                board[y, x - 1] |= doorPair;
            }
        }
    }
    
    public GameBuilder AddDoor(Vector2Int position, DoorType doorType, Direction doorDirection, bool isOpen)
    {
        return AddDoor(position.X, position.Y, doorType, doorDirection, isOpen);
    }

    public GameBuilder AddDoor(int x, int y, DoorType doorType, Direction doorDirection, bool isOpen)
    {
        AddDoor(Instance.Board, x, y, doorType, doorDirection, isOpen);
        
        var position = new Vector2Int(x, y);
        if (!Instance.DoorTiles.Contains(position))
        {
            Instance.DoorTiles.Add(position);
        }
        
        return this;
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
                        Direction doorDirection = tileChar switch
                        {
                            'U' => Direction.Up,
                            'R' => Direction.Right,
                            'D' => Direction.Down,
                            'L' => Direction.Left,
                            _ => throw new ArgumentOutOfRangeException(nameof(tileChar), tileChar, "tile character out of range")
                        };
                        AddDoor(boardMatrix, j, i, DoorType.DoorA, doorDirection, false);
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
                else if (TileComponent.KeyA.In(tile) || TileComponent.KeyB.In(tile) || TileComponent.KeyC.In(tile))
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
        Instance.KeysA = 0;
        Instance.KeysB = 0;
        Instance.KeysC = 0;
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
            '1' => TileComponent.KeyA,
            '2' => TileComponent.KeyB,
            '3' => TileComponent.KeyC,
            'G' => TileComponent.Goal,
            'U' => TileComponent.DoorUpA,
            'L' => TileComponent.DoorLeftA,
            'D' => TileComponent.DoorDownA,
            'R' => TileComponent.DoorRightA,
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