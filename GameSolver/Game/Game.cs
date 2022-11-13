using System.Text;

namespace GameSolver.Game
{
    public enum Direction
    {
        Up = 0,
        Left = 1,
        Down = 2,
        Right = 3
    }

    public sealed class Game
    {
        // Tile Numeric
        public const int Player = 1 << 0;
        public const int Floor = 1 << 1;
        public const int Wall = 1 << 2;
        public const int Score = 1 << 3;
        public const int Goal = 1 << 4;

        // Player Direction
        public const int PlayerDirUp = 0;
        public const int PlayerDirLeft = 1;
        public const int PlayerDirDown = 2;
        public const int PlayerDirRight = 3;

        // Tile Character
        public const char ChPlayer = 'P';
        public const char ChFloor = '.';
        public const char ChWall = 'x';
        public const char ChScore = '*';
        public const char ChGoal = 'G';

        // Hash Setting
        public const int HashStateCount = 5; // PlayerDirUp, PlayerDirLeft, PlayerDirDown, PlayerDirRight, Score
        public const int HashDirUp = 0;
        public const int HashDirLeft = 1;
        public const int HashDirDown = 2;
        public const int HashDirRight = 3;
        public const int HashScore = 4;

        // Action Numeric
        public const int Up = 0;
        public const int Left = 1;
        public const int Down = 2;
        public const int Right = 3;

        // Board
        private int[,] _board;
        private int _boardHeight;
        private int _boardWidth;

        // Player
        private IntVector2 _startPlayerTile;
        private Direction _startPlayerDirection;


        // TilePosition
        private IntVector2[] _scoreTiles;
        private IntVector2 _goalTile;

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
                        boardMatrix[i, j] = CharToTile(tileChar);
                    }
                    else
                    {
                        boardMatrix[i, j] = Wall;
                    }
                }
            }

            // Computing starting values
            IntVector2? startPlayerTile = null;
            IntVector2? goalTile = null;
            var scores = new List<IntVector2>();
            var keys = new List<IntVector2>();
            var doors = new List<IntVector2>();
            for (int i = 0; i < boardMatrix.GetLength(0); i++)
            {
                for (int j = 0; j < boardMatrix.GetLength(1); j++)
                {
                    int tile = boardMatrix[i, j];

                    if ((tile & Player) > 0)
                    {
                        startPlayerTile = new IntVector2(j, i);
                    }
                    else if ((tile & Goal) > 0)
                    {
                        goalTile = new IntVector2(j, i);
                    }
                    else if ((tile & Score) > 0)
                    {
                        scores.Add(new IntVector2(j, i));
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

            // Construct other element


            _board = boardMatrix;
            _boardHeight = height;
            _boardWidth = width;
            _startPlayerTile = startPlayerTile.Value;
            _scoreTiles = scores.ToArray();
            _goalTile = goalTile.Value;
            _startPlayerDirection = startPlayerDirection;
            HashComponent = ConstructZobristHashComponent();
        }

        public override string ToString()
        {
            var strBuilder = new StringBuilder();
            for (int i = 0; i < _board.GetLength(0); i++)
            {
                for (int j = 0; j < _board.GetLength(1); j++)
                {
                    strBuilder.Append(TileToChar(_board[i, j]));
                }
                strBuilder.AppendLine();
            }
            return strBuilder.ToString();
        }

        private static char TileToChar(int tile)
        {
            char retVal = ChFloor;
            int check = tile & (Player + Wall + Goal + Score);

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
                case Wall:
                    retVal = ChWall;
                    break;
            }

            return retVal;
        }

        private static int CharToTile(char tile)
        {
            int retVal = 0;

            switch (tile)
            {
                case ChPlayer:
                    retVal = Player + Floor;
                    break;
                case ChFloor:
                    retVal = Floor;
                    break;
                case ChGoal:
                    retVal = Goal + Floor;
                    break;
                case ChScore:
                    retVal = Score + Floor;
                    break;
                case ChWall:
                    retVal = Wall;
                    break;
            }

            return retVal;
        }

        private static int ColRowToTileIndex(int col, int row, int boardWidth)
        {
            return row * boardWidth + col;
        }

        private int ColRowToTileIndex(int col, int row)
        {
            return row * _boardWidth + col;
        }

        private long[,] ConstructZobristHashComponent() 
        {
            int boardHeight = _board.GetLength(0);
            int boardWidth = _board.GetLength(1);
            int boardSize = boardHeight * boardWidth;

            var zobristTable = new long[boardSize, HashStateCount];

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
}
