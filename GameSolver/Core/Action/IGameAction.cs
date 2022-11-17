namespace GameSolver.Core.Action;

public interface IGameAction
{
    void Do(State state);
    void Undo(State state);
}