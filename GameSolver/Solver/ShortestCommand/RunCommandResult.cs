using GameSolver.Core;
using GameSolver.Core.Action;

namespace GameSolver.Solver.ShortestCommand;

public sealed class RunCommandResult
{
    public IList<Tuple<State, CommandNode>> ExpansionPoints { get; }
    public bool RunStatus { get; set; }
    public IList<IGameAction> ActionHistory { get; }
    public IList<CommandNode> CommandHistory { get; }

    public RunCommandResult(bool runStatus)
    {
        RunStatus = runStatus;
        ActionHistory = new List<IGameAction>();
        CommandHistory = new List<CommandNode>();
        ExpansionPoints = new List<Tuple<State, CommandNode>>();
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