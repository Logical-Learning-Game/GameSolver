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
        var queue = new Queue<BreadthFirstSearchData>();
        var bfsData = new BreadthFirstSearchData(initialState, new List<IGameAction>());
        queue.Enqueue(bfsData);

        if (initialState.IsSolved())
        {
            return bfsData.Actions;
        }

        var exploredSet = new HashSet<long>();

        while (queue.Count > 0)
        {
            BreadthFirstSearchData data = queue.Dequeue();
            State currentState = data.State;
            exploredSet.Add(currentState.ZobristHash);

            foreach (IGameAction action in currentState.LegalGameActions())
            {
                State childState = State.Update(currentState, action);

                if (!exploredSet.Contains(childState.ZobristHash) && 
                    queue.All(d => d.State.ZobristHash != childState.ZobristHash))
                {
                    var copyActionList = new List<IGameAction>(data.Actions) {action};
                    
                    if (childState.IsSolved())
                    {
                        return copyActionList;
                    }
                    
                    var newBfsData = new BreadthFirstSearchData(childState, copyActionList);
                    queue.Enqueue(newBfsData);
                }
            }
        }

        return bfsData.Actions;
    }
}