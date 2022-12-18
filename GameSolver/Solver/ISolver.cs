using GameSolver.Core.Action;

namespace GameSolver.Solver;

public interface ISolver
{
    IEnumerable<IGameAction> Solve();
}