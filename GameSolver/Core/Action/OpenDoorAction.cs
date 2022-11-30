using System.ComponentModel;

namespace GameSolver.Core.Action;

public class OpenDoorAction : IGameAction
{
    private readonly MoveAction _moveAction;

    public OpenDoorAction(MoveAction moveAction)
    {
        _moveAction = moveAction;
    }

    public void Do(State state)
    {
        // Open door with key
        Vector2Int playerPos = state.PlayerPosition;
        Move toMove = _moveAction.ToMove;

        Direction nextDir = toMove switch
        {
            Move.Up => state.PlayerDirection,
            Move.Left => DirectionUtility.RotateLeft(state.PlayerDirection),
            Move.Down => DirectionUtility.RotateBack(state.PlayerDirection),
            Move.Right => DirectionUtility.RotateRight(state.PlayerDirection),
            _ => throw new InvalidEnumArgumentException(nameof(toMove), (int)toMove, toMove.GetType())
        };

        Tuple<int, int> doorDirToOpen = nextDir switch
        {
            Direction.Up => new Tuple<int, int>(Tile.DoorUpOpen, Hash.DoorUp),
            Direction.Left => new Tuple<int, int>(Tile.DoorLeftOpen, Hash.DoorLeft),
            Direction.Down => new Tuple<int, int>(Tile.DoorDownOpen, Hash.DoorDown),
            Direction.Right => new Tuple<int, int>(Tile.DoorRightOpen, Hash.DoorRight),
            _ => throw new InvalidEnumArgumentException(nameof(nextDir), (int)nextDir, toMove.GetType())
        };
        
        Tuple<int, int> doorPairToOpen = nextDir switch
        {
            Direction.Up => new Tuple<int, int>(Tile.DoorDownOpen, Hash.DoorDown),
            Direction.Left => new Tuple<int, int>(Tile.DoorRightOpen, Hash.DoorRight),
            Direction.Down => new Tuple<int, int>(Tile.DoorUpOpen, Hash.DoorUp),
            Direction.Right => new Tuple<int, int>(Tile.DoorLeftOpen, Hash.DoorLeft),
            _ => throw new InvalidEnumArgumentException(nameof(nextDir), (int)nextDir, toMove.GetType())
        };

        int boardWidth = state.Board.GetLength(1);

        int doorDir = doorDirToOpen.Item1;
        int doorDirHash = doorDirToOpen.Item2;
        int door1DPos = Game.ColRowToTileIndex(playerPos.Y, playerPos.X, boardWidth);
        state.Board[playerPos.Y, playerPos.X] |= doorDir;
        state.ZobristHash ^= state.Game.HashComponent[door1DPos, doorDirHash];

        Vector2Int nextDoorOffset = DirectionUtility.DirectionToVector2(nextDir);
        int doorPairDir = doorPairToOpen.Item1;
        int doorPairHash = doorPairToOpen.Item2;
        var doorPairPos = new Vector2Int(playerPos.X + nextDoorOffset.X, playerPos.Y + nextDoorOffset.Y);
        int doorPair1DPos = Game.ColRowToTileIndex(doorPairPos.Y, doorPairPos.X, boardWidth);
        state.Board[doorPairPos.Y, doorPairPos.X] |= doorPairDir;
        state.ZobristHash ^= state.Game.HashComponent[doorPair1DPos, doorPairHash];
        
        state.Keys--;

        _moveAction.Do(state);
    }

    public void Undo(State state)
    {
        _moveAction.Undo(state);
        
        // Undo open door with key
        Vector2Int playerPos = state.PlayerPosition;
        Move toMove = _moveAction.ToMove;

        Direction nextDir = toMove switch
        {
            Move.Up => state.PlayerDirection,
            Move.Left => DirectionUtility.RotateLeft(state.PlayerDirection),
            Move.Down => DirectionUtility.RotateBack(state.PlayerDirection),
            Move.Right => DirectionUtility.RotateRight(state.PlayerDirection),
            _ => throw new InvalidEnumArgumentException(nameof(toMove), (int)toMove, toMove.GetType())
        };

        Tuple<int, int> doorDirToReverse = nextDir switch
        {
            Direction.Up => new Tuple<int, int>(Tile.DoorUpOpen, Hash.DoorUp),
            Direction.Left => new Tuple<int, int>(Tile.DoorLeftOpen, Hash.DoorLeft),
            Direction.Down => new Tuple<int, int>(Tile.DoorDownOpen, Hash.DoorDown),
            Direction.Right => new Tuple<int, int>(Tile.DoorRight, Hash.DoorRight),
            _ => throw new InvalidEnumArgumentException(nameof(nextDir), (int)nextDir, toMove.GetType())
        };

        Tuple<int, int> doorPairToReverse = nextDir switch
        {
            Direction.Up => new Tuple<int, int>(Tile.DoorDownOpen, Hash.DoorDown),
            Direction.Left =>  new Tuple<int, int>(Tile.DoorRightOpen, Hash.DoorRight),
            Direction.Down => new Tuple<int, int>(Tile.DoorUpOpen, Hash.DoorUp),
            Direction.Right =>  new Tuple<int, int>(Tile.DoorLeftOpen, Hash.DoorLeft),
            _ => throw new InvalidEnumArgumentException(nameof(nextDir), (int)nextDir, toMove.GetType())
        };

        int boardWidth = state.Board.GetLength(1);

        int doorDir = doorDirToReverse.Item1;
        int doorDirHash = doorDirToReverse.Item2;
        int door1DPos = Game.ColRowToTileIndex(playerPos.Y, playerPos.X, boardWidth);
        state.Board[playerPos.Y, playerPos.X] &= ~doorDir;
        state.ZobristHash ^= state.Game.HashComponent[door1DPos, doorDirHash];

        Vector2Int prevDoorOffset = DirectionUtility.DirectionToVector2(nextDir);
        int doorPairDir = doorPairToReverse.Item1;
        int doorPairHash = doorPairToReverse.Item2;
        var doorPairPos = new Vector2Int(playerPos.X + prevDoorOffset.X, playerPos.Y + prevDoorOffset.Y);
        int doorPair1DPos = Game.ColRowToTileIndex(doorPairPos.Y, doorPairPos.X, boardWidth);
        state.Board[doorPairPos.Y, doorPairPos.X] &= ~doorPairDir;
        state.ZobristHash ^= state.Game.HashComponent[doorPair1DPos, doorPairHash];

        state.Keys++;
    }

    public override string ToString()
    {
        return _moveAction.ToString().ToUpper();
    }
}