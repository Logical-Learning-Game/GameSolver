﻿using GameSolver.Core;
using GameSolver.Core.Action;

namespace GameSolver.Solver.ShortestPath;

public sealed class DepthFirstSearchData
{
    public IList<IGameAction> Actions { get; }

    public DepthFirstSearchData(IList<IGameAction> actions)
    {
        Actions = actions;
    }
}

public sealed class StateData
{
    public IGameAction? Action { get; }
    public StateData? PrevState { get; }
    public State State { get; }
    public int Depth { get; }

    public StateData(IGameAction? action, State currentState, StateData? prevStateData, int depth)
    {
        Action = action;
        PrevState = prevStateData;
        State = currentState;
        Depth = depth;
    }

    public IReadOnlyList<IGameAction> Solution()
    {
        var solution = new List<IGameAction>();

        StateData currentState = this;
        while (currentState.PrevState != null)
        {
            solution.Add(currentState.Action!);
            currentState = currentState.PrevState;
        }

        solution.Reverse();
        return solution;
    }
}

public sealed class DepthFirstSearch : IShortestPathSolver
{
    private readonly Game _game;
    private readonly int _limit;

    public DepthFirstSearch(Game game, int limit)
    {
        _game = game;
        _limit = limit;
    }

    public IReadOnlyList<IGameAction> SolveBacktrackingStrategy()
    {
        var data = new DepthFirstSearchData(new IGameAction[_limit]);
        var initialState = new State(_game);
        SolveRecursive(initialState, data, 0);
        return data.Actions.ToList();
    }

    public IReadOnlyList<IGameAction> Solve()
    {
        return SolveDefaultStrategy();
    }

    public IReadOnlyList<IGameAction> SolveDefaultStrategy()
    {
        var frontier = new Stack<StateData>();
        var initialState = new State(_game);
        var initialStateData = new StateData(null, initialState, null, 0);
        frontier.Push(initialStateData);

        while (frontier.Count > 0)
        {
            StateData visitState = frontier.Pop();

            if (visitState.State.IsSolved())
            {
                return visitState.Solution();
            }

            if (visitState.Depth >= _limit)
            {
                continue;
            }

            if (!IsCycle(visitState))
            {
                foreach (IGameAction action in visitState.State.LegalGameActions())
                {
                    State childState = State.Update(visitState.State, action);
            
                    var childStateData = new StateData(action, childState, visitState, visitState.Depth + 1);
                    frontier.Push(childStateData);
                }
            }
        }
        
        return new List<IGameAction>();
    }
    
    public IReadOnlyList<IReadOnlyList<IGameAction>> SolveAllSolutionStrategy()
    {
        var initialState = new State(_game);
        ICollection<IReadOnlyList<IGameAction>> results = new List<IReadOnlyList<IGameAction>>();
        IList<IGameAction> actions = new List<IGameAction>();
        AllSolutionAtDepthRecursive(initialState, actions, results, 0);
        return (IReadOnlyList<IReadOnlyList<IGameAction>>)results;
    }

    private bool IsCycle(StateData stateData)
    {
        State state = stateData.State;
        StateData currentStateData = stateData;
        while (currentStateData.PrevState != null)
        {
            if (state.ZobristHash == currentStateData.PrevState.State.ZobristHash)
            {
                return true;
            }

            currentStateData = currentStateData.PrevState;
        }

        return false;
    }
    
    private bool SolveRecursive(State state, DepthFirstSearchData data, int depth)
    {
        if (state.IsSolved())
        {
            return true;
        }

        if (depth >= _limit)
        {
            return false;
        }

        foreach (IGameAction action in state.LegalGameActions())
        {
            state.Update(action);
            if (SolveRecursive(state, data, depth + 1))
            {
                data.Actions[depth] = action;
                return true;
            }
            state.Undo(action);
        }

        return false;
    }

    private void AllSolutionAtDepthRecursive(State state, IList<IGameAction> actions, ICollection<IReadOnlyList<IGameAction>> results, int depth)
    {
        if (depth > _limit)
        {
            return;
        }
        
        if (state.IsSolved())
        {
            var copyActions = new List<IGameAction>(actions);
            results.Add(copyActions);
        }
        else
        {
            foreach (IGameAction action in state.LegalGameActions())
            {
                state.Update(action);

                actions.Add(action);
                AllSolutionAtDepthRecursive(state, actions, results, depth + 1);
                actions.RemoveAt(actions.Count - 1);
                
                state.Undo(action);
            }
        }
    }
}