using GameSolver.Game;

namespace GameSolver.Solver
{
    public class State
    {
        public Board Board { get; init; }
        public GameAction? Action { get; init; }
        public State? PrevState { get; init; }

        public State(Board board, GameAction? action = null, State? prevState = null)
        {
            Board = board;
            Action = action;
            PrevState = prevState;
        }

        public List<GameAction> Solution()
        {
            State? currentState = this;
            var result = new List<GameAction>();
            while (currentState != null && currentState.Action != null)
            {
                result.Add((GameAction)currentState.Action);
                currentState = currentState.PrevState;
            }

            result.Reverse();
            return result;
        }
    }
}
