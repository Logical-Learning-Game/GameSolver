using System.Collections;
using GameSolver.Core;
using GameSolver.Core.Action;

namespace GameSolver.Solver;

public sealed class IterativeDeepeningDepthFirstSearch : ISolver
{
    private readonly Game _game;

    public IterativeDeepeningDepthFirstSearch(Game game)
    {
        _game = game;
    }

    public IReadOnlyList<IGameAction> Solve()
    {
        int depth = 0;

        while (true)
        {
            var dfsSolver = new DepthFirstSearch(_game, depth);
            IReadOnlyList<IGameAction> solution = dfsSolver.SolveDefaultStrategy();

            if (solution.Count > 0)
            {
                return solution;
            }

            depth++;
        }
    }
}