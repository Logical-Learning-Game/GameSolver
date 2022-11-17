using GameSolver.Core.Action;

namespace GameSolver.Core;

public sealed class State : ICloneable
{
    public Vector2Int PlayerPosition { get; set; }
    public Vector2Int GoalTile { get; } 
    public List<Vector2Int> ScoreTiles { get; }
    public Direction PlayerDirection { get; set; }
    public int[,] Board { get; }
    public long ZobristHash { get; set; }
    public Core.Game Game { get; }
        
    public State(Core.Game game)
    {
        Board = (int[,])game.Board.Clone();
        PlayerPosition = game.StartPlayerTile;
        PlayerDirection = game.StartPlayerDirection;
        GoalTile = game.GoalTile;
        ScoreTiles = game.ScoreTiles.ToList();
        ZobristHash = CalculateZobristHash(game.HashComponent);
        Game = game;
    }

    private State(Vector2Int playerPosition, Vector2Int goalTile, List<Vector2Int> scoreTiles,
        Direction playerDirection, int[,] board, long zobristHash, Core.Game game)
    {
        PlayerPosition = playerPosition;
        GoalTile = goalTile;
        ScoreTiles = scoreTiles ?? throw new ArgumentNullException(nameof(scoreTiles), "score tile should not be null");
        PlayerDirection = playerDirection;
        Board = board ?? throw new ArgumentNullException(nameof(board), "board should not be null");
        ZobristHash = zobristHash;
        Game = game ?? throw new ArgumentNullException(nameof(game), "game should not be null");
    }

    public bool IsSolved()
    {
        Vector2Int goalPos = GoalTile;
        int goalTile = Board[goalPos.Y, goalPos.X];
        return (goalTile & Tile.Player) > 0;
    }

    public static State Update(State state, IGameAction action)
    {
        var copyThisState = (State)state.Clone();
        action.Do(copyThisState);
        return copyThisState;
    }
        
    public void Update(IGameAction action)
    {
        action.Do(this);
    }

    public void Undo(IGameAction action)
    {
        action.Undo(this);
    }

    public List<IGameAction> LegalGameActions()
    {
        var legalActions = new List<IGameAction>();

        int x = PlayerPosition.X;
        int y = PlayerPosition.Y;

        Vector2Int dir = DirectionUtility.DirectionToVector2(PlayerDirection);
        int dx = dir.X;
        int dy = dir.Y;

        Vector2Int[] nextPositions =
        {
            new Vector2Int(x + dx, y + dy),
            new Vector2Int(x + dy, y - dx),
            new Vector2Int(x - dx, y - dy),
            new Vector2Int(x - dy, y + dx)
        };

        Move[] availableMoves = {Move.Up, Move.Left, Move.Down, Move.Right};

        for (int i = 0; i < nextPositions.Length; i++)
        {
            Vector2Int nextPos = nextPositions[i];
            
            if (CheckPassableTile(nextPos.X, nextPos.Y))
            {
                var moveAction = new MoveAction(availableMoves[i]);
            
                if ((Board[nextPos.Y, nextPos.X] & Tile.Score) > 0)
                {
                    IGameAction action = new CollectAction(moveAction, Tile.Score);
                    legalActions.Add(action);
                }
                else
                {
                    legalActions.Add(moveAction);
                }
            }
        }
        
        return legalActions;
    }
    
    public override string ToString()
    {
        return Core.Game.BoardToString(Board);
    }

    public object Clone()
    {
        var copyScoreTiles = new List<Vector2Int>(ScoreTiles);
        var copyBoard = (int[,])Board.Clone();
        return new State(PlayerPosition, GoalTile, copyScoreTiles, PlayerDirection, copyBoard, ZobristHash, Game);
    }

    private long CalculateZobristHash(long[,] hashComponent)
    {
        long hash = 0;
        int boardWidth = Board.GetLength(1);

        foreach (Vector2Int tile in ScoreTiles)
        {
            int score1DPos = boardWidth * tile.Y + tile.X;
            hash ^= hashComponent[score1DPos, Hash.Score];
        }

        Vector2Int playerPos = PlayerPosition;
        int player1DPos = boardWidth * playerPos.Y + playerPos.X;

        int hashDir = Hash.DirectionToHashIndex(PlayerDirection);

        hash ^= hashComponent[player1DPos, hashDir];

        return hash;
    }

    private bool CheckPassableTile(int x, int y)
    {
        int height = Board.GetLength(0);
        int width = Board.GetLength(1);

        // Out of bound check
        if (y < 0 || x < 0 || y > height - 1 || x > width - 1)
        {
            return false;
        }

        int unPassableTile = Tile.Wall + Tile.Goal;
        int check = Board[y, x] & unPassableTile;

        if (ScoreTiles.Count == 0)
        {
            return check is 0 or Tile.Goal;
        }

        return check == 0;
    }
}