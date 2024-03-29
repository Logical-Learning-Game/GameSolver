﻿using System.ComponentModel;

namespace GameSolver.Core.Action;

public sealed class OpenDoorAction : MoveActionWrapper
{
    private bool _isDoorOpen;
    private DoorType _doorType;

    public OpenDoorAction(MoveAction moveAction) : base(moveAction) {}

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
        bool isDoor = state.IsDoor(nextPosition, nextDir, out bool isDoorOpen, out DoorType doorType);

        if (isDoor && !isDoorOpen)
        {
            if (doorType == DoorType.DoorA && state.KeysA == 0 ||
                doorType == DoorType.DoorB && state.KeysB == 0 ||
                doorType == DoorType.DoorC && state.KeysC == 0)
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

            _isDoorOpen = true;
            _doorType = doorType;

            switch (doorType)
            {
                case DoorType.DoorA:
                    state.KeysA--;
                    break;
                case DoorType.DoorB:
                    state.KeysB--;
                    break;
                case DoorType.DoorC:
                    state.KeysC--;
                    break;
            }
        }

        base.Do(state);
    }

    public override void Undo(State state)
    {
        base.Undo(state);
        
        // Undo open door with key
        if (_isDoorOpen)
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

            switch (_doorType)
            {
                case DoorType.DoorA:
                    state.KeysA++;
                    break;
                case DoorType.DoorB:
                    state.KeysB++;
                    break;
                case DoorType.DoorC:
                    state.KeysC++;
                    break;
            }
        }
    }
}