namespace GameSolver.Core.Action;

public interface IGameAction : IEquatable<IGameAction>
{
    void Do(State state);
    void Undo(State state);
}