using GameSolver.Core.Action;

namespace GameSolver.Solver;

public interface ISolver
{
    IReadOnlyList<IGameAction> Solve();
}