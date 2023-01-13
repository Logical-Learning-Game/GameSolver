using GameSolver.Core;
using GameSolver.Core.Action;

namespace GameSolver.Solver.ShortestPath;

public sealed class BreadthFirstSearchData
{
    public State State { get; }
    public IList<IGameAction> Actions { get; }

    public BreadthFirstSearchData(State state, IList<IGameAction> actions)
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
    
    public IReadOnlyList<IGameAction> Solution()
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

public sealed class BreadthFirstSearch : IShortestPathSolver
{
    private readonly Game _game;

    public BreadthFirstSearch(Game game)
    {
        _game = game;
    }

    public IReadOnlyList<IGameAction> Solve()
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

    public IReadOnlyList<IReadOnlyList<IGameAction>> AllSolutionAtDepth(int depth)
    {
        var allSolutions = new List<IReadOnlyList<IGameAction>>();
        
        var initialState = new State(_game);
        var frontier = new Queue<BFSStateData>();
        var bfsData = new BFSStateData(null, null, initialState, 0);
        frontier.Enqueue(bfsData);

        if (initialState.IsSolved() && bfsData.Depth == depth)
        {
            allSolutions.Add(bfsData.Solution());
            return allSolutions;
        }

        while (frontier.Count > 0)
        {
            BFSStateData data = frontier.Dequeue();
            State currentState = data.State;

            if (data.Depth > depth)
            {
                return allSolutions;
            }

            if (!IsCycle(data))
            {
                foreach (IGameAction action in currentState.LegalGameActions())
                {
                    State childState = State.Update(currentState, action);
                
                    var childStateData = new BFSStateData(action, data, childState, data.Depth + 1);
            
                    if (childStateData.Depth == depth && childState.IsSolved())
                    {
                        allSolutions.Add(childStateData.Solution());
                        continue;
                    }
            
                    frontier.Enqueue(childStateData);
                }
            }
        }

        return new List<IReadOnlyList<IGameAction>>();
    }
    
    private static bool IsCycle(BFSStateData stateData)
    {
        State state = stateData.State;
        BFSStateData currentStateData = stateData;
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
}