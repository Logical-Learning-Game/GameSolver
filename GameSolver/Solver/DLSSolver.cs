using GameSolver.Game;

namespace GameSolver.Solver
{
    public class DLSSolver
    {
        private readonly Board _board;
        private readonly int _limit;

        public DLSSolver(Board board, int limit)
        {
            _board = board;
            _limit = limit;
        }

        public State? Solve()
        {
            var initialState = new State(_board);
            return DLSSearch(initialState, _limit);
        }

        public List<List<GameAction>> AllSolutionAtDepth()
        {
            var initialState = new State(_board);
            var stateResult = new List<State>();

            DLSAllSolutionAtDepth(initialState, _limit, ref stateResult);

            var actionResults = new List<List<GameAction>>();
            foreach (State s in stateResult)
            {
                actionResults.Add(s.Solution());
            }

            return actionResults;
        }

        private void DLSAllSolutionAtDepth(State state, int limit, ref List<State> result)
        {
            if (state.Board.IsGoalState())
            {
                if (limit == 0)
                {
                    result.Add(state);
                }
                return;
            }
            else if (limit == 0)
            {
                return;
            }

            foreach (GameAction action in state.Board.GetValidActions())
            {
                Board updatedBoard = state.Board.Update(action);
                var childState = new State(updatedBoard, action, state);

                DLSAllSolutionAtDepth(childState, limit - 1, ref result);
            }
        }

        private State? DLSSearch(State state, int limit)
        {
            if (state.Board.IsGoalState())
            {
                return state;
            }
            else if (limit == 0)
            {
                return null;
            }

            foreach (GameAction action in state.Board.GetValidActions())
            {
                Board updatedBoard = state.Board.Update(action);
                var childState = new State(updatedBoard, action, state);

                State? result = DLSSearch(childState, limit - 1);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }
    }
}
