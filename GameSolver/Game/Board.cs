using System.Text;

namespace GameSolver.Game
{
    public struct IntVector2
    {
        public int X { get; set; }
        public int Y { get; set; }

        public IntVector2(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override string ToString()
        {
            return $"x: {X}, y: {Y}";
        }
    }

    public class PlayerPosition : ICloneable
    {
        public IntVector2 Position { get; set; }
        public IntVector2 Direction { get; set; }

        public object Clone()
        {
            return new PlayerPosition
            {
                Position = Position,
                Direction = Direction
            };
        }

        public override string ToString()
        {
            return $"Position:\n{Position}\nDirection:\n{Direction}\n";
        }
    }

    public class Board : ICloneable
    {
        public List<int> ZobristTable { get; init; }
        public int Score { get; init; }
        public int RemainingScore { get; set; }
        public PlayerPosition Player { get; set; }
        public Tile[,] Matrix { get; init; }
        public TileManager TileManager { get; set; }

        public Board(Tile[,] board, PlayerPosition player, TileManager tileManager, List<int> zobristTable, int score, int remainingScore)
        {
            Matrix = board;
            Score = score;
            RemainingScore = remainingScore;
            Player = player;
            ZobristTable = zobristTable;
            TileManager = tileManager;
        }

        public static Board FromString(string board)
        {
            Tile[,] boardMatrix = ToBoardMatrix(board);
            return Parse(boardMatrix);
        }

        public bool IsGoalState()
        {
            foreach (Tile tile in Matrix)
            {
                if (tile == Tile.Goal)
                {
                    return false;
                }
            }
            return true;
        }

        public List<GameAction> GetValidActions()
        {
            var validActions = new List<GameAction>();

            int x = Player.Position.X;
            int y = Player.Position.Y;

            int dx = Player.Direction.X;
            int dy = Player.Direction.Y;

            // Front
            if (CheckPassableTile(x + dx, y + dy))
            {
                validActions.Add(GameAction.Up);
            }

            // LeftHand
            if (CheckPassableTile(x + dy, y - dx ))
            {
                validActions.Add(GameAction.Left);
            }

            // RightHand
            if (CheckPassableTile(x - dy, y + dx))
            {
                validActions.Add(GameAction.Right);
            }

            // Back
            if (CheckPassableTile(x - dx, y - dy))
            {
                validActions.Add(GameAction.Down);
            }

            return validActions;
        }

        public Board Update(GameAction action)
        {
            int x = Player.Position.X;
            int y = Player.Position.Y;

            PlayerPosition nextPlayerPosition = NextPosition(action);
            IntVector2 Position = nextPlayerPosition.Position;

            var newBoard = (Board)Clone();
            newBoard.Player = nextPlayerPosition;

            newBoard.Matrix[y, x] = Tile.Empty;

            Tile moveToTile = newBoard.Matrix[Position.Y, Position.X];
            if (moveToTile == Tile.Score)
            {
                newBoard.RemainingScore--;
            }

            newBoard.Matrix[Position.Y, Position.X] = Tile.Player;

            return newBoard;
        }

        public override string ToString()
        {
            var strBuilder = new StringBuilder();

            int height = Matrix.GetLength(0);
            int width = Matrix.GetLength(1);

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    strBuilder.Append((char)Matrix[i, j]);
                }
                strBuilder.Append('\n');
            }
            return strBuilder.ToString();
        }

        private PlayerPosition NextPosition(GameAction action)
        {
            int x = Player.Position.X;
            int y = Player.Position.Y;

            int dx = Player.Direction.X;
            int dy = Player.Direction.Y;

            int nextDx = dx;
            int nextDy = dy;

            switch (action)
            {
                case GameAction.Left:
                    nextDx = -dy;
                    nextDy = dx;
                    break;
                case GameAction.Right:
                    nextDx = dy;
                    nextDy = -dx;
                    break;
                case GameAction.Up:
                    nextDx = dx;
                    nextDy = dy;
                    break;
                case GameAction.Down:
                    nextDx = -dx;
                    nextDy = -dy;
                    break;
            }

            var player = new PlayerPosition
            {
                Position = new IntVector2(x + nextDx, y + nextDy),
                Direction = new IntVector2(nextDx, nextDy)
            };

            return player;
        }

        private bool CheckPassableTile(int x, int y)
        {
            int height = Matrix.GetLength(0);
            int width = Matrix.GetLength(1);

            // Out of bound check
            if (y < 0 || x < 0 || y > height - 1 || x > width - 1)
            {
                return false;
            }

            bool isPassableTile = TileManager.PassableTiles.Contains(Matrix[y, x]);

            if (RemainingScore == 0)
            {
                return  isPassableTile || Matrix[y, x] == Tile.Goal;
            }
            return isPassableTile;
        }

        private static Tile[,] ToBoardMatrix(string board)
        {
            string trimmedBoard = board.Trim();
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

            var boardMatrix = new Tile[height, width];
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    boardMatrix[i, j] = (Tile)trimmedBoardList[i][j];
                }
            }

            return boardMatrix;
        }

        private static Board Parse(Tile[,] boardMatrix)
        {
            int height = boardMatrix.GetLength(0);
            int width = boardMatrix.GetLength(1);

            PlayerPosition? player = null;
            int score = 0;
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    Tile tile = boardMatrix[i, j];
                    if (tile == Tile.Player)
                    {
                        player = new PlayerPosition
                        {
                            Position = new IntVector2(j, i),
                            Direction = new IntVector2(0, -1)
                        };
                    }
                    else if (tile == Tile.Score)
                    {
                        score++;
                    }
                }
            }

            if (player == null)
            {
                throw new Exception("playerPosition should not be null");
            }

            var emptyTileManager = new TileManager();
            var zobristTable = new List<int>();

            return new Board(boardMatrix, player, emptyTileManager, zobristTable, score, score);
        }

        public object Clone()
        {
            var copyMatrix = (Tile[,])Matrix.Clone();
            var copyPlayerPosition = (PlayerPosition)Player.Clone();
            return new Board(copyMatrix, copyPlayerPosition, TileManager, ZobristTable, Score, RemainingScore);
        }
    }
}
