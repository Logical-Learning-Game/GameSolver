// See https://aka.ms/new-console-template for more information

using GameSolver.Collection;
using GameSolver.Collection.Encoder;
using GameSolver.Core;
using GameSolver.Core.Action;
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
//TestPatternEncoder();


static void TestNewGame()
{
    Console.WriteLine("Test new game");

    const string boardStr = @"
        k...*..xx
		.x.x.x.xx
		..*...*.d
		.x.x.x.x.
		P.....r.G
    ";

    var game = new Game(boardStr, Direction.Up);
    var initialState = new State(game);
    Console.WriteLine(initialState);

    var bfsSolver = new BreadthFirstSearch(game);
    List<IGameAction> result = bfsSolver.Solve();
    Console.WriteLine("result:");
    PrintList(result);

    Console.WriteLine();
}
TestNewGame();



static void TestAllSolution()
{
    Console.WriteLine("Test all solution");

    const string boardStr = @"
        ....*..xx
		.x.x.x.xx
		..*...*..
		.x.x.x.x.
		P.......G
    ";

    var game = new Game(boardStr, Direction.Up);

    var dfsSolver = new DepthFirstSearch(game, 16);
    List<List<IGameAction>> results = dfsSolver.AllSolutionAtDepth();

    for (int i = 0; i < results.Count; i++)
    {
        CompositeCommand commands = new CommandParser(results[i]).Parse();
        CompositeCommand encoded = new PatternEncoder(commands).Encode();
        Console.WriteLine($"{i + 1}: {encoded.ToRegex()} ---- BlockCount {encoded.Commands.Count}");
    }
}
//TestAllSolution();