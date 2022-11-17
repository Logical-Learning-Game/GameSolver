using System.Text;

namespace GameSolver.Core;

public sealed class Game
{
    // Player Direction
    public const int PlayerDirUp = 0;
    public const int PlayerDirLeft = 1;
    public const int PlayerDirDown = 2;
    public const int PlayerDirRight = 3;
        
    // Action Numeric
    public const int Up = 0;
    public const int Left = 1;
    public const int Down = 2;
    public const int Right = 3;

    // Board
    public int[,] Board { get; }

    // Player
    public Vector2Int StartPlayerTile { get; }
    public Direction StartPlayerDirection { get; set; }
    public int Keys { get; set; }


    // TilePosition
    public Vector2Int[] ScoreTiles { get; }
    public Vector2Int[] KeyTiles { get; }
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
                    boardMatrix[i, j] = Tile.CharToTile(tileChar);
                }
                else
                {
                    boardMatrix[i, j] = Tile.Wall;
                }
            }
        }

        // Computing starting values
        Vector2Int? startPlayerTile = null;
        Vector2Int? goalTile = null;
        var scores = new List<Vector2Int>();
        var keys = new List<Vector2Int>();
        for (int i = 0; i < boardMatrix.GetLength(0); i++)
        {
            for (int j = 0; j < boardMatrix.GetLength(1); j++)
            {
                int tile = boardMatrix[i, j];

                if ((tile & Tile.Player) > 0)
                {
                    startPlayerTile = new Vector2Int(j, i);
                }
                else if ((tile & Tile.Goal) > 0)
                {
                    goalTile = new Vector2Int(j, i);
                }
                else if ((tile & Tile.Score) > 0)
                {
                    scores.Add(new Vector2Int(j, i));
                }
                else if ((tile & Tile.Key) > 0)
                {
                    keys.Add(new Vector2Int(j, i));
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
        Keys = 0;
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
                strBuilder.Append(Tile.TileToChar(board[i, j]));
            }
            strBuilder.AppendLine();
        }
        return strBuilder.ToString();
    }
        
    public override string ToString()
    {
        return BoardToString(Board);
    }
        
    public static int ColRowToTileIndex(int row, int col, int boardWidth)
    {
        return row * boardWidth + col;
    }

    private long[,] ConstructZobristHashComponent() 
    {
        int boardHeight = Board.GetLength(0);
        int boardWidth = Board.GetLength(1);
        int boardSize = boardHeight * boardWidth;

        var zobristTable = new long[boardSize, Hash.StateCount];

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