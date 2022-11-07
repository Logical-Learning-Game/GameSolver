// See https://aka.ms/new-console-template for more information

using GameSolver.Collection;
using GameSolver.Collection.Encoder;
using GameSolver.Game;
using GameSolver.Solver;

static void PrintList<T>(IEnumerable<T> list)
{
    string s = string.Join(" ", list);
    Console.WriteLine(s);
}

static void TestPatternEncoder()
{
    Console.WriteLine("Test pattern encoder");

    string[] testcases =
    {
        "rruuuurrrrddrrdd",
        "rruuuurrrrddddrr",
        "uurrrruurrddddrr",
        "uurruurrrrddrrdd",
        "rruuuurrrrddddrr",
        "rruurruuddrrddrr",
        "uuruluru"
    };

    for (int i = 0; i < testcases.Length; i++)
    {
        BaseCommand cmd = new CommandParser(testcases[i]).Parse();
        CompositeCommand? compositeCmd = cmd as CompositeCommand;

        CompositeCommand encodedCommand = new PatternEncoder(compositeCmd!).Encode();
        Console.WriteLine($"Testcase {i + 1}: {testcases[i]} => {encodedCommand.ToRegex()}");
    }

    Console.WriteLine();
}
TestPatternEncoder();

//static List<GameAction> Solution(State state)
//{
//    List<GameAction> result = new();
//    while (state.Action != null)
//    {
//        result.Add((GameAction)state.Action);
//        state = state.PrevState;
//    }
//}

static void TestGameBoard()
{
    Console.WriteLine("Test game board");

    string boardStr = @"
        ....*..xx
		.x.x.x.xx
		..*...*..
		.x.x.x.x.
		P.......G
    ";

    Board board = Board.FromString(boardStr);
    board.Player.Direction = new IntVector2(0, -1);
    board.TileManager = new TileManager
    {
       PassableTiles = new HashSet<Tile> { Tile.Empty, Tile.Score },
       BlockedTiles = new HashSet<Tile> { Tile.Block }
    };
    board.HashSetting = new HashSetting
    {
        HashTiles = new HashSet<Tile> { Tile.Player, Tile.Score, Tile.Goal }
    };

    //Console.WriteLine("Board:");

    //List<GameAction> validActions = board.GetValidActions();
    //PrintList(validActions);
    //Console.WriteLine($"Hash: {board.Hash()}");
    //Console.WriteLine(board);


    //Board updatedBoard = board.Update(GameAction.Right);
    //validActions = updatedBoard.GetValidActions();
    //PrintList(validActions);
    //Console.WriteLine($"Hash: {updatedBoard.Hash()}");
    //Console.WriteLine(updatedBoard);


    //updatedBoard = updatedBoard.Update(GameAction.Down);
    //validActions = updatedBoard.GetValidActions();
    //PrintList(validActions);
    //Console.WriteLine($"Hash: {updatedBoard.Hash()}");
    //Console.WriteLine(updatedBoard);

    

    

    //var bfsSolver = new BFSSolver(board);
    //finalState = bfsSolver.Solve();

    //if (finalState != null)
    //{
    //    List<GameAction> result = finalState.Solution();
    //    foreach (GameAction action in result)
    //    {
    //        Console.Write((char)action);
    //    }
    //    Console.WriteLine();
    //}
    //else
    //{
    //    Console.WriteLine("No solution");
    //}


    Console.WriteLine();
}
TestGameBoard();

//static void PrintArray(int[,] arr)
//{
//    foreach (int n in arr)
//    {
//        Console.Write(n);
//    }
//    Console.WriteLine();
//}

//int[,] a = { { 1, 2 }, { 3, 4 } };
//int[,] copy = (int[,])a.Clone();
//a[0, 0] = 9;

//PrintArray(a);
//PrintArray(copy);

//static void TestRandomSeed()
//{
//    var random = new Random();
//    long a = random.NextInt64();
//    Console.WriteLine(a);
//}
//TestRandomSeed();


static void TestGameSolution()
{
    Console.WriteLine("Test game solution");

    string boardStr = @"
        ....*..xx
		.x.x.x.xx
		..*.x.*..
		.x.x.x.x.
		P.......G
    ";

    Board board = Board.FromString(boardStr);
    board.Player.Direction = new IntVector2(0, -1);
    board.TileManager = new TileManager
    {
        PassableTiles = new HashSet<Tile> { Tile.Empty, Tile.Score },
        BlockedTiles = new HashSet<Tile> { Tile.Block }
    };
    board.HashSetting = new HashSetting
    {
        HashTiles = new HashSet<Tile> { Tile.Player, Tile.Score, Tile.Goal }
    };

    Console.WriteLine("Board:");
    Console.WriteLine(board);

    var dlsSolver = new DLSSolver(board, 16);
    List<List<GameAction>> results = dlsSolver.AllSolutionAtDepth();

    Console.WriteLine("Solutions:");
    if (results.Count > 0)
    {
        for (int i = 0; i < results.Count; i++)
        {
            CompositeCommand commands = new CommandParser(results[i]).Parse();
            CompositeCommand encodedCommand = new PatternEncoder(commands).Encode();
            Console.WriteLine($"{i + 1}: {encodedCommand.ToFullString()} => {encodedCommand.ToRegex()} ---- BlockCount: {encodedCommand.Commands.Count}");
        }
    }
    else
    {
        Console.WriteLine("No solution");
    }
}
TestGameSolution();