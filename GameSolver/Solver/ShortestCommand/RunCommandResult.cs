using GameSolver.Core;
using GameSolver.Core.Action;

namespace GameSolver.Solver.ShortestCommand;

public sealed class RunCommandResult
{
    public State? StateSnapshot { get; set; }
    public CommandNode? CheckpointNode { get; set; }
    public bool RunStatus { get; set; }
    public IList<IGameAction> ActionHistory { get; }

    public RunCommandResult(State? stateSnapshot, CommandNode? checkpointNode, bool runStatus)
    {
        StateSnapshot = stateSnapshot;
        CheckpointNode = checkpointNode;
        RunStatus = runStatus;
        ActionHistory = new List<IGameAction>();
    }

    public RunCommandResult Fail()
    {
        RunStatus = false;
        return this;
    }

    public RunCommandResult Success()
    {
        RunStatus = true;
        return this;
    }
}