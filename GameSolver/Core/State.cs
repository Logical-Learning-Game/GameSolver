using System.ComponentModel;
using System.Text;
using GameSolver.Core.Action;
using GameSolver.Solver.ShortestCommand;

namespace GameSolver.Core;

public sealed class State : ICloneable
{
    public Vector2Int PlayerPosition { get; set; }
    public Vector2Int GoalTile { get; }
    public List<Vector2Int> ScoreTiles { get; }
    public List<Vector2Int> KeyTiles { get; }
    public List<Vector2Int> DoorTiles { get; }
    public List<Vector2Int> ConditionalTiles { get; }
    public int Keys { get; set; }
    public int Conditions { get; set; }
    public Direction PlayerDirection { get; set; }
    public int[,] Board { get; }
    public long ZobristHash { get; set; }
    public Game Game { get; }
    
    public State(Game game)
    {
        Board = (int[,]) game.Board.Clone();
        PlayerPosition = game.StartPlayerTile;
        PlayerDirection = game.StartPlayerDirection;
        GoalTile = game.GoalTile;
        ScoreTiles = game.ScoreTiles.ToList();
        KeyTiles = game.KeyTiles.ToList();
        DoorTiles = game.DoorTiles.ToList();
        ConditionalTiles = game.ConditionalTiles.ToList();
        Keys = game.Keys;
        Conditions = game.Conditions;
        ZobristHash = CalculateZobristHash(game.HashComponent);
        Game = game;
    }

    private State(Vector2Int playerPosition, Vector2Int goalTile, List<Vector2Int> scoreTiles,
        List<Vector2Int> keyTiles, List<Vector2Int> conditionalTiles, int conditions,
        int keys, List<Vector2Int> doorTiles, Direction playerDirection, int[,] board, long zobristHash, Game game)
    {
        PlayerPosition = playerPosition;
        GoalTile = goalTile;
        ScoreTiles = scoreTiles ??
                     throw new ArgumentNullException(nameof(scoreTiles), "score tiles should not be null");
        PlayerDirection = playerDirection;
        Board = board ?? throw new ArgumentNullException(nameof(board), "board should not be null");
        ZobristHash = zobristHash;
        Game = game ?? throw new ArgumentNullException(nameof(game), "game should not be null");
        KeyTiles = keyTiles ?? throw new ArgumentNullException(nameof(keyTiles), "key tiles should not be null");
        ConditionalTiles = conditionalTiles ?? throw new ArgumentNullException(nameof(conditionalTiles), "conditional tiles should not be null");
        Keys = keys;
        Conditions = conditions;
        DoorTiles = doorTiles ?? throw new ArgumentNullException(nameof(doorTiles), "door tiles should not be null");
    }

    public object Clone()
    {
        var copyScoreTiles = new List<Vector2Int>(ScoreTiles);
        var copyKeyTiles = new List<Vector2Int>(KeyTiles);
        var copyDoorTiles = new List<Vector2Int>(DoorTiles);
        var copyConditionalTiles = new List<Vector2Int>(ConditionalTiles);
        var copyBoard = (int[,]) Board.Clone();
        return new State(PlayerPosition, GoalTile, copyScoreTiles, copyKeyTiles, copyConditionalTiles, Conditions, Keys, copyDoorTiles, PlayerDirection,
            copyBoard, ZobristHash, Game);
    }

    public bool IsSolved()
    {
        Vector2Int goalPos = GoalTile;
        int goalTile = Board[goalPos.Y, goalPos.X];
        return TileComponent.Player.In(goalTile);
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

    public IEnumerable<IGameAction> LegalGameActions()
    {
        var legalActions = new List<IGameAction>();

        int x = PlayerPosition.X;
        int y = PlayerPosition.Y;

        Vector2Int dir = DirectionUtility.DirectionToVector2(PlayerDirection);
        int dx = dir.X;
        int dy = dir.Y;

        Vector2Int[] nextPositions =
        {
            new(x + dx, y + dy),
            new(x + dy, y - dx),
            new(x - dx, y - dy),
            new(x - dy, y + dx)
        };

        Move[] availableMoves = {Move.Up, Move.Left, Move.Down, Move.Right};
        Direction[] playerDirections =
        {
            PlayerDirection,
            DirectionUtility.RotateLeft(PlayerDirection),
            DirectionUtility.RotateBack(PlayerDirection),
            DirectionUtility.RotateRight(PlayerDirection)
        };
        
        
        for (int i = 0; i < nextPositions.Length; i++)
        {
            Vector2Int nextPos = nextPositions[i];

            bool canPassTile = CheckPassableTile(nextPos);
            if (!canPassTile)
            {
                continue;
            }
            
            bool isDoor = IsDoor(nextPos, playerDirections[i], out bool doorOpen);
            if (isDoor && !doorOpen && Keys == 0)
            {
                continue;
            }
            
            var moveAction = new MoveAction(availableMoves[i]);
            IGameAction action = moveAction.WithInteraction();

            legalActions.Add(action);
        }

        return legalActions;
    }

    public void AddComponent(Vector2Int tilePosition, TileComponent component)
    {
        Board[tilePosition.Y, tilePosition.X] |= component.Value;
    }

    public void RemoveComponent(Vector2Int tilePosition, TileComponent component)
    {
        Board[tilePosition.Y, tilePosition.X] &= ~component.Value;
    }

    public void UpdateZobristHash(Vector2Int position, int hashIndex)
    {
        int position1d = Game.ToOneDimension(position);
        ZobristHash ^= Game.HashComponent[position1d, hashIndex];
    }

    public override string ToString()
    {
        return Game.BoardToString(Board);
    }

    private long CalculateZobristHash(long[,] hashComponent)
    {
        long hash = 0;
        int boardWidth = Board.GetLength(1);

        foreach (Vector2Int tile in ScoreTiles)
        {
            int score1DPos = Game.ToOneDimension(tile.Y, tile.X, boardWidth);
            hash ^= hashComponent[score1DPos, Hash.Score];
        }

        foreach (Vector2Int tile in KeyTiles)
        {
            int key1DPos = Game.ToOneDimension(tile.Y, tile.X, boardWidth);
            hash ^= hashComponent[key1DPos, Hash.Key];
        }

        foreach (Vector2Int tile in DoorTiles)
        {
            int door1DPos = Game.ToOneDimension(tile.Y, tile.X, boardWidth);
            hash ^= hashComponent[door1DPos, Hash.DoorUp];
            hash ^= hashComponent[door1DPos, Hash.DoorLeft];
            hash ^= hashComponent[door1DPos, Hash.DoorDown];
            hash ^= hashComponent[door1DPos, Hash.DoorRight];
        }

        foreach (Vector2Int tile in ConditionalTiles)
        {
            int conditional1DPos = Game.ToOneDimension(tile.Y, tile.X, boardWidth);
            hash ^= hashComponent[conditional1DPos, Hash.Condition];
        }

        Vector2Int playerPos = PlayerPosition;
        int player1DPos = Game.ToOneDimension(playerPos.Y, playerPos.X, boardWidth);

        int hashDir = Hash.DirectionToHashIndex(PlayerDirection);

        hash ^= hashComponent[player1DPos, hashDir];

        return hash;
    }

    public bool IsDoor(Vector2Int position, Direction inboundDirection, out bool doorOpen)
    {
        Tuple<TileComponent, TileComponent> doorDirToBlock = inboundDirection switch
        {
            Direction.Up => new(TileComponent.DoorDown, TileComponent.DoorDownOpen),
            Direction.Left => new(TileComponent.DoorRight, TileComponent.DoorRightOpen),
            Direction.Down => new(TileComponent.DoorUp, TileComponent.DoorUpOpen),
            Direction.Right => new(TileComponent.DoorLeft, TileComponent.DoorLeftOpen),
            _ => throw new InvalidEnumArgumentException(nameof(inboundDirection), (int)inboundDirection, inboundDirection.GetType())
        };

        TileComponent dirToBlock = doorDirToBlock.Item1;
        TileComponent isOpen = doorDirToBlock.Item2;

        int currentTile = Board[position.Y, position.X];
        bool isDoor = dirToBlock.In(currentTile);

        doorOpen = false;
        if (isDoor)
        {
            doorOpen = isOpen.In(currentTile);
        }

        return isDoor;
    }

    public bool CheckPassableTile(Vector2Int position)
    {
        if (OutOfBoundCheck(position))
        {
            return false;
        }

        int currentTile = Board[position.Y, position.X];

        bool isWall = TileComponent.Wall.In(currentTile);
        bool isGoal = TileComponent.Goal.In(currentTile);
        
        if (ScoreTiles.Count == 0)
        {
            return !isWall || isGoal;
        }

        return !isWall && !isGoal;
    }

    public bool OutOfBoundCheck(Vector2Int position)
    {
        int height = Board.GetLength(0);
        int width = Board.GetLength(1);
        return position.Y < 0 || position.X < 0 || position.Y > height - 1 || position.X > width - 1;
    }

    public RunCommandResult RunCommand(CommandNode node)
    {
        var result = new RunCommandResult(null, null, false);

        CommandNode? currentNode = node;

        var exploredSet = new HashSet<long>();
        
        while (currentNode is not null)
        {
            try
            {
                result.ActionHistory.Add(currentNode.Action);
                Update(currentNode.Action);
            }
            catch (IndexOutOfRangeException)
            {
                return result.Fail();
            }

            // validate player position after update the state
            bool validPlayerPos = CheckPassableTile(PlayerPosition);
            bool isDoor = IsDoor(PlayerPosition, PlayerDirection, out bool isDoorOpen);
            
            if (isDoor && !isDoorOpen || !isDoor && isDoorOpen || !validPlayerPos)
            {
                return result.Fail();
            }
            
            // detect cycle in command
            if (exploredSet.Contains(ZobristHash))
            {
                return result.Fail();
            }
            
            if (currentNode.MainBranch is null || Conditions > 0 && !currentNode.ConditionalBranchFilled)
            {
                result.CheckpointNode = currentNode;
                result.StateSnapshot = (State)Clone();
            }

            if (IsSolved())
            {
                return result.Success();
            }

            exploredSet.Add(ZobristHash);

            if (Conditions > 0 && currentNode.ConditionalBranch is not null)
            {
                currentNode = currentNode.ConditionalBranch;
                Conditions--;
            }
            else
            {
                currentNode = currentNode.MainBranch;
            }
        }

        return result.Fail();
    }
}