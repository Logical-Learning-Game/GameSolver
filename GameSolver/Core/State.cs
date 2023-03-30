using System.ComponentModel;
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
    public int KeysA { get; set; }
    public int KeysB { get; set; }
    public int KeysC { get; set; }
    public ConditionalType Condition { get; set; }
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
        KeysA = game.KeysA;
        KeysB = game.KeysB;
        KeysC = game.KeysC;
        Condition = game.Condition;
        ZobristHash = CalculateZobristHash(game.HashComponent);
        Game = game;
    }

    private State(Vector2Int playerPosition, Vector2Int goalTile, List<Vector2Int> scoreTiles,
        List<Vector2Int> keyTiles, List<Vector2Int> conditionalTiles, ConditionalType condition,
        int keysA, int keysB, int keysC, List<Vector2Int> doorTiles, Direction playerDirection, int[,] board, long zobristHash, Game game)
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
        KeysA = keysA;
        KeysB = keysB;
        KeysC = keysC;
        Condition = condition;
        DoorTiles = doorTiles ?? throw new ArgumentNullException(nameof(doorTiles), "door tiles should not be null");
    }

    public object Clone()
    {
        var copyScoreTiles = new List<Vector2Int>(ScoreTiles);
        var copyKeyTiles = new List<Vector2Int>(KeyTiles);
        var copyDoorTiles = new List<Vector2Int>(DoorTiles);
        var copyConditionalTiles = new List<Vector2Int>(ConditionalTiles);
        var copyBoard = (int[,]) Board.Clone();
        return new State(PlayerPosition, GoalTile, copyScoreTiles, copyKeyTiles, copyConditionalTiles, Condition, KeysA, KeysB, KeysC, copyDoorTiles, PlayerDirection,
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

        MoveActionFactory moveActionFactory = new MoveInteractActionFactory();
        
        for (int i = 0; i < nextPositions.Length; i++)
        {
            Vector2Int nextPos = nextPositions[i];

            bool canPassTile = CheckPassableTile(nextPos);
            if (!canPassTile)
            {
                continue;
            }
            
            bool isDoor = IsDoor(nextPos, playerDirections[i], out bool doorOpen, out DoorType doorType);
            if (isDoor && !doorOpen)
            {
                if (doorType == DoorType.DoorA && KeysA == 0 ||
                    doorType == DoorType.DoorB && KeysB == 0 ||
                    doorType == DoorType.DoorC && KeysC == 0)
                {
                    continue;
                }
            }
            
            var moveAction = new MoveAction(availableMoves[i]);
            IGameAction action = moveActionFactory.CreateFrom(moveAction);

            legalActions.Add(action);
        }
        
        return legalActions;
    }

    public void AddComponent(Vector2Int tilePosition, TileComponent component)
    {
        RemoveComponent(tilePosition, component);
        Board[tilePosition.Y, tilePosition.X] |= component.Value;
    }

    public void RemoveComponent(Vector2Int tilePosition, TileComponent component)
    {
        Board[tilePosition.Y, tilePosition.X] &= ~component.Mask;
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

        Vector2Int playerPos = PlayerPosition;
        int player1DPos = Game.ToOneDimension(playerPos.Y, playerPos.X, boardWidth);

        int hashDir = Hash.DirectionToHashIndex(PlayerDirection);

        hash ^= hashComponent[player1DPos, hashDir];

        return hash;
    }

    public bool IsDoor(Vector2Int position, Direction inboundDirection, out bool doorOpen, out DoorType doorType)
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
        
        // Determine door type
        doorType = DoorType.DoorNoKey;
        if (dirToBlock.Equals(TileComponent.DoorUp))
        {
            if (TileComponent.DoorUpNokey.In(currentTile))
            {
                doorType = DoorType.DoorNoKey;
            }
            else if (TileComponent.DoorUpA.In(currentTile))
            {
                doorType = DoorType.DoorA;
            }
            else if (TileComponent.DoorUpB.In(currentTile))
            {
                doorType = DoorType.DoorB;
            }
            else if (TileComponent.DoorUpC.In(currentTile))
            {
                doorType = DoorType.DoorC;
            }
        }
        else if (dirToBlock.Equals(TileComponent.DoorRight))
        {
            if (TileComponent.DoorRightNokey.In(currentTile))
            {
                doorType = DoorType.DoorNoKey;
            }
            else if (TileComponent.DoorRightA.In(currentTile))
            {
                doorType = DoorType.DoorA;
            }
            else if (TileComponent.DoorRightB.In(currentTile))
            {
                doorType = DoorType.DoorB;
            }
            else if (TileComponent.DoorRightC.In(currentTile))
            {
                doorType = DoorType.DoorC;
            }
        }
        else if (dirToBlock.Equals(TileComponent.DoorDown))
        {
            if (TileComponent.DoorDownNokey.In(currentTile))
            {
                doorType = DoorType.DoorNoKey;
            }
            else if (TileComponent.DoorDownA.In(currentTile))
            {
                doorType = DoorType.DoorA;
            }
            else if (TileComponent.DoorDownB.In(currentTile))
            {
                doorType = DoorType.DoorB;
            }
            else if (TileComponent.DoorDownC.In(currentTile))
            {
                doorType = DoorType.DoorC;
            }
        }
        else if (dirToBlock.Equals(TileComponent.DoorLeft))
        {
            if (TileComponent.DoorLeftNokey.In(currentTile))
            {
                doorType = DoorType.DoorNoKey;
            }
            else if (TileComponent.DoorLeftA.In(currentTile))
            {
                doorType = DoorType.DoorA;
            }
            else if (TileComponent.DoorLeftB.In(currentTile))
            {
                doorType = DoorType.DoorB;
            }
            else if (TileComponent.DoorLeftC.In(currentTile))
            {
                doorType = DoorType.DoorC;
            }
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
        return GameUtility.OutOfBoundCheck(Board, position.X, position.Y);
    }

    public RunCommandResult RunCommand(CommandNode node)
    {
        var result = new RunCommandResult(false);

        CommandNode? currentNode = node;

        var stateExploredSet = new HashSet<long>();
        var commandNodeExploredSet = new HashSet<CommandNode>();
        
        while (currentNode is not null)
        {
            try
            {
                result.ActionHistory.Add(currentNode.Action);
                result.CommandHistory.Add(currentNode);
                Update(currentNode.Action);
            }
            catch (Exception)
            {
                return result.Fail();
            }

            // validate player position after update the state because move action doesn't throw any error if it failed
            bool validPlayerPos = CheckPassableTile(PlayerPosition);
            if (!validPlayerPos)
            {
                return result.Fail();
            }
            
            // detect cycle in command
            if (stateExploredSet.Contains(ZobristHash))
            {
                // to detect cycle in conditional node
                if (currentNode.IsConditionalNode)
                {
                    if (commandNodeExploredSet.Contains(currentNode))
                    {
                        return result.Fail();
                    }
                    
                    commandNodeExploredSet.Add(currentNode);
                }
                else
                {
                    return result.Fail();
                }
            }
            else
            {
                commandNodeExploredSet.Clear();
            }

            //TODO maybe have duplicate expansion points
            if (Condition != ConditionalType.None)
            {
                result.ExpansionPoints.Add(new Tuple<State, CommandNode>((State)Clone(), currentNode));
            }
            else if (currentNode.MainBranch is null)
            {
                result.ExpansionPoints.Clear();
                result.ExpansionPoints.Add(new Tuple<State, CommandNode>((State)Clone(), currentNode));
            }

            if (IsSolved())
            {
                return result.Success();
            }

            stateExploredSet.Add(ZobristHash);

            if (
                Condition != ConditionalType.None && 
                currentNode.ConditionalType == Condition && 
                currentNode.ConditionalBranch is not null && 
                currentNode.IsConditionalNode
                )
            {
                currentNode = currentNode.ConditionalBranch;
                Condition = ConditionalType.None;
            }
            else
            {
                currentNode = currentNode.MainBranch;
            }
        }

        return result.Fail();
    }
}