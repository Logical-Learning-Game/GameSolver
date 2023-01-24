using System.ComponentModel;

namespace GameSolver.Core.Action;

public sealed class OpenDoorAction : MoveAction
{
    private readonly MoveAction _moveAction;
    private bool _isDoorOpenWithKey;

    public OpenDoorAction(MoveAction moveAction) : base(moveAction.ToMove)
    {
        _moveAction = moveAction;
    }

    public override void Do(State state)
    {
        // Open door with key
        Vector2Int playerPos = state.PlayerPosition;
        Move toMove = ToMove;

        Direction nextDir = toMove switch
        {
            Move.Up => state.PlayerDirection,
            Move.Left => DirectionUtility.RotateLeft(state.PlayerDirection),
            Move.Down => DirectionUtility.RotateBack(state.PlayerDirection),
            Move.Right => DirectionUtility.RotateRight(state.PlayerDirection),
            _ => throw new InvalidEnumArgumentException(nameof(toMove), (int)toMove, toMove.GetType())
        };

        Vector2Int directionVector = DirectionUtility.DirectionToVector2(nextDir);
        Vector2Int nextPosition = Vector2Int.Sum(playerPos, directionVector);
        bool isDoor = state.IsDoor(nextPosition, nextDir, out bool isDoorOpen);

        if (isDoor && !isDoorOpen)
        {
            if (state.Keys == 0)
            {
                throw new Exception("don't have key to open the closed door");
            }
            
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
            
            TileComponent doorPairDir = doorPairToOpen.Item1;
            int doorPairHash = doorPairToOpen.Item2;
            state.AddComponent(nextPosition, doorPairDir);
            state.UpdateZobristHash(nextPosition, doorPairHash);

            _isDoorOpenWithKey = true;
            
            state.Keys--;
        }

        _moveAction.Do(state);
    }

    public override void Undo(State state)
    {
        _moveAction.Undo(state);
        
        // Undo open door with key
        if (_isDoorOpenWithKey)
        {
            Vector2Int playerPos = state.PlayerPosition;
            Move toMove = ToMove;

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
    }
}