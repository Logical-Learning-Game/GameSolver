using GameSolver.Core;
using GameSolver.Core.Action;

namespace GameSolver.Solver;

public sealed class BreadthFirstSearchData
{
    public State State { get; }
    public List<IGameAction> Actions { get; }

    public BreadthFirstSearchData(State state, List<IGameAction> actions)
    {
        State = state;
        Actions = actions;
    }
}

public sealed class BFSStateData
{
    public IGameAction? Action { get; }
    public BFSStateData? PrevState { get; }
    public State State { get; }
    public int Depth { get; }

    public BFSStateData(IGameAction? action, BFSStateData? prevState, State state, int depth)
    {
        Action = action;
        PrevState = prevState;
        State = state;
        Depth = depth;
    }
    
    public List<IGameAction> Solution()
    {
        var solution = new List<IGameAction>();

        BFSStateData currentState = this;
        while (currentState.PrevState != null)
        {
            solution.Add(currentState.Action!);
            currentState = currentState.PrevState;
        }

        solution.Reverse();
        return solution;
    }
}

public sealed class BreadthFirstSearch : ISolver
{
    private readonly Game _game;

    public BreadthFirstSearch(Game game)
    {
        _game = game;
    }

    public IEnumerable<IGameAction> Solve()
    {
        var initialState = new State(_game);
        var frontier = new Queue<BFSStateData>();
        var bfsData = new BFSStateData(null, null, initialState, 0);
        frontier.Enqueue(bfsData);

        if (initialState.IsSolved())
        {
            return bfsData.Solution();
        }

        var exploredSet = new HashSet<long>();

        while (frontier.Count > 0)
        {
            BFSStateData data = frontier.Dequeue();
            State currentState = data.State;
            exploredSet.Add(currentState.ZobristHash);

            foreach (IGameAction action in currentState.LegalGameActions())
            {
                State childState = State.Update(currentState, action);

                if (!exploredSet.Contains(childState.ZobristHash) && 
                    frontier.All(d => d.State.ZobristHash != childState.ZobristHash))
                {
                    var childStateData = new BFSStateData(action, data, childState, data.Depth + 1);
                    
                    if (childState.IsSolved())
                    {
                        return childStateData.Solution();
                    }
                    
                    frontier.Enqueue(childStateData);
                }
            }
        }

        return new List<IGameAction>();
    }

    }
}