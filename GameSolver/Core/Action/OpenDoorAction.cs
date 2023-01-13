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

        Tuple<TileComponent, int> doorDirToOpen = nextDir switch
        {
            Direction.Up => new(TileComponent.DoorUpOpen, Hash.DoorUp),
            Direction.Left => new(TileComponent.DoorLeftOpen, Hash.DoorLeft),
            Direction.Down => new(TileComponent.DoorDownOpen, Hash.DoorDown),
            Direction.Right => new(TileComponent.DoorRightOpen, Hash.DoorRight),
            _ => throw new InvalidEnumArgumentException(nameof(nextDir), (int)nextDir, toMove.GetType())
        };
        
        Tuple<TileComponent, int> doorPairToOpen = nextDir switch
        {
            Direction.Up => new(TileComponent.DoorDownOpen, Hash.DoorDown),
            Direction.Left => new(TileComponent.DoorRightOpen, Hash.DoorRight),
            Direction.Down => new(TileComponent.DoorUpOpen, Hash.DoorUp),
            Direction.Right => new(TileComponent.DoorLeftOpen, Hash.DoorLeft),
            _ => throw new InvalidEnumArgumentException(nameof(nextDir), (int)nextDir, toMove.GetType())
        };
        
        TileComponent doorDir = doorDirToOpen.Item1;
        int doorDirHash = doorDirToOpen.Item2;
        state.AddComponent(playerPos, doorDir);
        state.UpdateZobristHash(playerPos, doorDirHash);

        Vector2Int nextDoorOffset = DirectionUtility.DirectionToVector2(nextDir);
        TileComponent doorPairDir = doorPairToOpen.Item1;
        int doorPairHash = doorPairToOpen.Item2;
        Vector2Int doorPairPos = Vector2Int.Sum(playerPos, nextDoorOffset);
        state.AddComponent(doorPairPos, doorPairDir);
        state.UpdateZobristHash(doorPairPos, doorPairHash);
        
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

        Tuple<TileComponent, int> doorDirToReverse = nextDir switch
        {
            Direction.Up => new(TileComponent.DoorUpOpen, Hash.DoorUp),
            Direction.Left => new(TileComponent.DoorLeftOpen, Hash.DoorLeft),
            Direction.Down => new(TileComponent.DoorDownOpen, Hash.DoorDown),
            Direction.Right => new(TileComponent.DoorRight, Hash.DoorRight),
            _ => throw new InvalidEnumArgumentException(nameof(nextDir), (int)nextDir, toMove.GetType())
        };

        Tuple<TileComponent, int> doorPairToReverse = nextDir switch
        {
            Direction.Up => new(TileComponent.DoorDownOpen, Hash.DoorDown),
            Direction.Left =>  new(TileComponent.DoorRightOpen, Hash.DoorRight),
            Direction.Down => new(TileComponent.DoorUpOpen, Hash.DoorUp),
            Direction.Right =>  new(TileComponent.DoorLeftOpen, Hash.DoorLeft),
            _ => throw new InvalidEnumArgumentException(nameof(nextDir), (int)nextDir, toMove.GetType())
        };

        TileComponent doorDir = doorDirToReverse.Item1;
        int doorDirHash = doorDirToReverse.Item2;
        state.RemoveComponent(playerPos, doorDir);
        state.UpdateZobristHash(playerPos, doorDirHash);

        Vector2Int prevDoorOffset = DirectionUtility.DirectionToVector2(nextDir);
        TileComponent doorPairDir = doorPairToReverse.Item1;
        int doorPairHash = doorPairToReverse.Item2;
        Vector2Int doorPairPos = Vector2Int.Sum(playerPos, prevDoorOffset);
        state.RemoveComponent(doorPairPos, doorPairDir);
        state.UpdateZobristHash(doorPairPos, doorPairHash);

        state.Keys++;
    }

    public override string ToString()
    {
        return _moveAction.ToString().ToUpper();
    }
}