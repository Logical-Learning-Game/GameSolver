using GameSolver.Core;
using GameSolver.Core.Action;

namespace GameSolver.Solver;

public sealed class DepthFirstSearchData
{
    public IGameAction[] Actions { get; }

    public DepthFirstSearchData(IGameAction[] actions)
    {
        Actions = actions;
    }
}

public sealed class DepthFirstSearch : ISolver
{
    private readonly Game _game;
    private readonly int _limit;

    public DepthFirstSearch(Game game, int limit)
    {
        _game = game;
        _limit = limit;
    }

    public List<IGameAction> SolveBacktrackingStrategy()
    {
        var data = new DepthFirstSearchData(new IGameAction[_limit]);
        var initialState = new State(_game);
        SolveRecursive(initialState, ref data, 0);
        return data.Actions.ToList();
    }

    public IEnumerable<IGameAction> Solve()
    public List<List<IGameAction>> SolveAllSolutionStrategy()
    { 
        var initialState = new State(_game);
        var results = new List<List<IGameAction>>();
        var actions = new List<IGameAction>();
        AllSolutionAtDepthRecursive(initialState, ref actions, ref results, 0);
        return results;
    }

    private bool SolveRecursive(State state, ref DepthFirstSearchData data, int depth)
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
            if (SolveRecursive(state, ref data, depth + 1))
            {
                data.Actions[depth] = action;
                return true;
            }
            state.Undo(action);
        }

        return false;
    }

    private void AllSolutionAtDepthRecursive(State state, ref List<IGameAction> actions, ref List<List<IGameAction>> results, int depth)
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
                AllSolutionAtDepthRecursive(state, ref actions, ref results, depth + 1);
                actions.RemoveAt(actions.Count - 1);
                
                state.Undo(action);
            }
        }
    }
}