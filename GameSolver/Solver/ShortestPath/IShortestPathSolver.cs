using GameSolver.Core.Action;

namespace GameSolver.Solver.ShortestPath;

public interface IShortestPathSolver
{
    IReadOnlyList<IGameAction> Solve();
}