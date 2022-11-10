namespace GameSolver.Game
{
    public class HashComponent
    {
        public long[,] ZobristTable { get; set; }

        private readonly Board _board;

        public HashComponent(Board board)
        {
            _board = board;
            ZobristTable = GenerateZobristTable();
        }

        private long[,] GenerateZobristTable()
        {
            int boardHeight = _board.Matrix.GetLength(0);
            int boardWidth = _board.Matrix.GetLength(1);
            int boardSize = boardHeight * boardWidth;
            int pieceSize = Utility.TileValue.Count;

            var zobristTable = new long[boardSize, pieceSize];

            var selectedNumber = new HashSet<long>();
            var random = new Random();

            for (int i = 0; i < zobristTable.GetLength(0); i++)
            {
                for (int j = 0; j < zobristTable.GetLength(1); j++)
                {
                    long rnd;
                    do
                    {
                        rnd = random.NextInt64();
                    }
                    while (rnd == 0 || selectedNumber.Contains(rnd));

                    selectedNumber.Add(rnd);
                    zobristTable[i, j] = rnd;
                }
            }

            return zobristTable;
        }
    }
}
