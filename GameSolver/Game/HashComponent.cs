using System.Text;

namespace GameSolver.Game
{
    public class HashComponent
    {
        public long[] ZobristTable { get; init; }

        private readonly Board _board;

        public HashComponent(Board board)
        {
            _board = board;

            int boardHeight = _board.Matrix.GetLength(0);
            int boardWidth = _board.Matrix.GetLength(1);
            int boardSize = boardHeight * boardWidth;

            ZobristTable = new long[boardSize];

            var selectedNumber = new HashSet<long>();
            var random = new Random();

            for (int i = 0; i < ZobristTable.Length; i++)
            {
                long rnd;
                do
                {
                    rnd = random.NextInt64();
                }
                while (rnd == 0 || selectedNumber.Contains(rnd));

                selectedNumber.Add(rnd);
                ZobristTable[i] = rnd;
            }
        }
    }
}
