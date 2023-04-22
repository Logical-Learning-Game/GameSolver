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
        else if (item == TileComponent.KeyA.Value || item == TileComponent.KeyB.Value || item == TileComponent.KeyC.Value)
        {
            Instance.KeyTiles.Add(position);
        }
        
        return this;
    }

    public GameBuilder AddDoor(int x, int y, DoorType doorType, Direction doorDirection, bool isOpen)
    {
        int door = TileComponent.CreateDoor(doorType, doorDirection, isOpen);
        int doorPair = TileComponent.CreateDoor(doorType, DirectionUtility.RotateBack(doorDirection), isOpen);

        Instance.Board[y, x] |= door;
        
        var position = new Vector2Int(x, y);
        if (!Instance.DoorTiles.Contains(position))
        {
            Instance.DoorTiles.Add(position);
        }

        Vector2Int? pairPosition = null;
        if (doorDirection == Direction.Up)
        {
            if (!GameUtility.OutOfBoundCheck(Instance.Board, x, y - 1))
            {
                Instance.Board[y - 1, x] |= doorPair;

                pairPosition = new Vector2Int(x, y - 1);
            }
        }
        else if (doorDirection == Direction.Right)
        {
            if (!GameUtility.OutOfBoundCheck(Instance.Board, x + 1, y))
            {
                Instance.Board[y, x + 1] |= doorPair;
                
                pairPosition = new Vector2Int(x + 1, y);
            }
        }
        else if (doorDirection == Direction.Down)
        {
            if (!GameUtility.OutOfBoundCheck(Instance.Board, x, y + 1))
            {
                Instance.Board[y + 1, x] |= doorPair;
                
                pairPosition = new Vector2Int(x, y + 1);
            }
        }
        else if (doorDirection == Direction.Left)
        {
            if (!GameUtility.OutOfBoundCheck(Instance.Board, x - 1, y))
            {
                Instance.Board[y, x - 1] |= doorPair;
                
                pairPosition = new Vector2Int(x - 1, y);
            }
        }

        if (pairPosition.HasValue)
        {
            if (!Instance.DoorTiles.Contains(pairPosition.Value))
            {
                Instance.DoorTiles.Add(pairPosition.Value);
            }
        }

        return this;
    }
    
    public GameBuilder AddDoor(Vector2Int position, DoorType doorType, Direction doorDirection, bool isOpen)
    {
        return AddDoor(position.X, position.Y, doorType, doorDirection, isOpen);
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
                    
                    boardMatrix[i, j] |= component.Value;
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
    
    public static Game CreateFromStandardBoardFormat(List<List<int>> tile)
    {
        int height = tile.Count;
        int width = tile[0].Count;
        var board = new int[height, width];
        var game = new Game
        {
            Board = board
        };
        
        for (int i = height - 1; i >= 0; i--)
        {
            for (int j = width - 1; j >= 0; j--)
            {
                int tileValue = tile[i][j];
                
                // check first 4 bit LSB: Player data
                if ((tileValue & 0b0001) == 0b0001)
                {
                    game.Board[i, j] |= TileComponent.Player.Value;
                    game.StartPlayerTile = new Vector2Int(j, i);
                    
                    int playerDirection = tileValue & 0b0110;
                    game.StartPlayerDirection = playerDirection switch
                    {
                        0b0000 => Direction.Left,
                        0b0010 => Direction.Up,
                        0b0100 => Direction.Right,
                        0b0110 => Direction.Down,
                        _ => game.StartPlayerDirection
                    };
                }
                
                // next 4 bit: Tile data
                int tileData = (tileValue >> 4) & 0b1111;
                switch (tileData)
                {
                    // Wall
                    case 0b0001:
                        game.Board[i, j] |= TileComponent.Wall.Value;
                        break;
                    //  Goal
                    case 0b0010:
                        game.Board[i, j] |= TileComponent.Goal.Value;
                        game.GoalTile = new Vector2Int(j, i);
                        break;
                    // Condition A
                    case 0b0011:
                        game.Board[i, j] |= TileComponent.ConditionalA.Value;
                        game.ConditionalTiles.Add(new Vector2Int(j, i));
                        break;
                    // Condition B
                    case 0b0100:
                        game.Board[i, j] |= TileComponent.ConditionalB.Value;
                        game.ConditionalTiles.Add(new Vector2Int(j, i));
                        break;
                    // Condition C
                    case 0b0101:
                        game.Board[i, j] |= TileComponent.ConditionalC.Value;
                        game.ConditionalTiles.Add(new Vector2Int(j, i));
                        break;
                    // Condition D
                    case 0b0110:
                        game.Board[i, j] |= TileComponent.ConditionalD.Value;
                        game.ConditionalTiles.Add(new Vector2Int(j, i));
                        break;
                    // Condition E
                    case 0b0111:
                        game.Board[i, j] |= TileComponent.ConditionalE.Value;
                        game.ConditionalTiles.Add(new Vector2Int(j, i));
                        break;
                }
                
                // next 4 bit: Item on tile data
                int itemData = (tileValue >> 8) & 0b1111;
                switch (itemData)
                {
                    // Key A
                    case 0b0001:
                        game.Board[i, j] |= TileComponent.KeyA.Value;
                        game.KeyTiles.Add(new Vector2Int(j, i));
                        break;
                    // Key B
                    case 0b0010:
                        game.Board[i, j] |= TileComponent.KeyB.Value;
                        game.KeyTiles.Add(new Vector2Int(j, i));
                        break;
                    // Key C
                    case 0b0011:
                        game.Board[i, j] |= TileComponent.KeyC.Value;
                        game.KeyTiles.Add(new Vector2Int(j, i));
                        break;
                }

                bool containDoor = false;
                
                // next 4 bit: West door
                int westDoorData = (tileValue >> 12) & 0b1111;
                if ((westDoorData & 0b0001) == 0b0001)
                {
                    containDoor = true;

                    switch (westDoorData & 0b0110)
                    {
                        case 0b0000:
                            game.Board[i, j] |= TileComponent.CreateDoor(DoorType.DoorNoKey, Direction.Left, false);
                            break;
                        case 0b0010:
                            game.Board[i, j] |= TileComponent.CreateDoor(DoorType.DoorA, Direction.Left, false);
                            break;
                        case 0b0100:
                            game.Board[i, j] |= TileComponent.CreateDoor(DoorType.DoorB, Direction.Left, false);
                            break;
                        case 0b0110:
                            game.Board[i, j] |= TileComponent.CreateDoor(DoorType.DoorC, Direction.Left, false);
                            break;
                    }
                }
                
                // next 4 bit: North door
                int northDoorData = (tileValue >> 16) & 0b1111;
                if ((northDoorData & 0b0001) == 0b0001)
                {
                    containDoor = true;

                    switch (northDoorData & 0b0110)
                    {
                        case 0b0000:
                            game.Board[i, j] |= TileComponent.CreateDoor(DoorType.DoorNoKey, Direction.Up, false);
                            break;
                        case 0b0010:
                            game.Board[i, j] |= TileComponent.CreateDoor(DoorType.DoorA, Direction.Up, false);
                            break;
                        case 0b0100:
                            game.Board[i, j] |= TileComponent.CreateDoor(DoorType.DoorB, Direction.Up, false);
                            break;
                        case 0b0110:
                            game.Board[i, j] |= TileComponent.CreateDoor(DoorType.DoorC, Direction.Up, false);
                            break;
                    }
                }
                
                // next 4 bit: East door
                int eastDoorData = (tileValue >> 20) & 0b1111;
                if ((eastDoorData & 0b0001) == 0b0001)
                {
                    containDoor = true;

                    switch (eastDoorData & 0b0110)
                    {
                        case 0b0000:
                            game.Board[i, j] |= TileComponent.CreateDoor(DoorType.DoorNoKey, Direction.Right, false);
                            break;
                        case 0b0010:
                            game.Board[i, j] |= TileComponent.CreateDoor(DoorType.DoorA, Direction.Right, false);
                            break;
                        case 0b0100:
                            game.Board[i, j] |= TileComponent.CreateDoor(DoorType.DoorB, Direction.Right, false);
                            break;
                        case 0b0110:
                            game.Board[i, j] |= TileComponent.CreateDoor(DoorType.DoorC, Direction.Right, false);
                            break;
                    }
                }
                
                // next 4 bit: South door
                int southDoorData = (tileValue >> 24) & 0b1111;
                if ((southDoorData & 0b0001) == 0b0001)
                {
                    containDoor = true;

                    switch (southDoorData & 0b0110)
                    {
                        case 0b0000:
                            game.Board[i, j] |= TileComponent.CreateDoor(DoorType.DoorNoKey, Direction.Down, false);
                            break;
                        case 0b0010:
                            game.Board[i, j] |= TileComponent.CreateDoor(DoorType.DoorA, Direction.Down, false);
                            break;
                        case 0b0100:
                            game.Board[i, j] |= TileComponent.CreateDoor(DoorType.DoorB, Direction.Down, false);
                            break;
                        case 0b0110:
                            game.Board[i, j] |= TileComponent.CreateDoor(DoorType.DoorC, Direction.Down, false);
                            break;
                    }
                }

                if (containDoor)
                {
                    game.DoorTiles.Add(new Vector2Int(j, i));
                }
                
            }
        }

        game.KeysA = 0;
        game.KeysB = 0;
        game.KeysC = 0;
        game.Condition = ConditionalType.None;
        game.HashComponent = CreateZobristHashComponent(game.Board);

        return game;
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
            'a' => TileComponent.ConditionalA,
            'b' => TileComponent.ConditionalB,
            'c' => TileComponent.ConditionalC,
            'd' => TileComponent.ConditionalD,
            'e' => TileComponent.ConditionalE,
            _ => throw new ArgumentOutOfRangeException(nameof(ch), ch, "char argument not in range")
        };

        return component;
    }
    
    private static long[,] CreateZobristHashComponent(int[,] board) 
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