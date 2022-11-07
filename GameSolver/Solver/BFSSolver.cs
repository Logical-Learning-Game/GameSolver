using GameSolver.Game;

namespace GameSolver.Solver
{
    public class BFSSolver
    {
        private readonly Board _board;

        public BFSSolver(Board board)
        {
            _board = board;
        }

        private static bool QueueContain(Queue<State> queue, State state)
        {
            State[] transfer = queue.ToArray();
            foreach (State s in transfer)
            {
                if (s.Board.Hash() == state.Board.Hash())
                {
                    return true;
                }
            }
            return false;
        }

        public State? Solve()
        {
            var initialState = new State(_board);
            var queue = new Queue<State>();
            queue.Enqueue(initialState);

            if (initialState.Board.IsGoalState())
            {
                return initialState;
            }

            var exploredSet = new HashSet<long>();

            while (queue.Count > 0)
            {
                State state = queue.Dequeue();
                exploredSet.Add(state.Board.Hash());

                foreach (GameAction action in state.Board.GetValidActions())
                {
                    Board updatedBoard = state.Board.Update(action);
                    //Console.WriteLine(updatedBoard.RemainingScore);
                    //Console.Write(updatedBoard);
                    var childState = new State(updatedBoard, action, state);

                    if (!exploredSet.Contains(childState.Board.Hash()) && !QueueContain(queue, childState))
                    {
                        if (childState.Board.IsGoalState())
                        {
                            return childState;
                        }
                        queue.Enqueue(childState);
                    }
                }
            }

            return null;
        }
    }
}
