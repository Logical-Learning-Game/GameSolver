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
        ....*..xx
		.x.x.x.xx
		..*...*..
		.x.x.x.x.
		Pk......G
    ";
    
    var game = new Game(boardStr, Direction.Up);

    var initialState = new State(game);
    initialState.Update(new CollectAction(MoveAction.Right, Tile.Key));
    initialState.Update(MoveAction.Up);
    initialState.Undo(MoveAction.Up);
    initialState.Undo(new CollectAction(MoveAction.Right, Tile.Key));
    PrintList(initialState.LegalGameActions());
    Console.Write(initialState);

    // var bfsSolver = new BreadthFirstSearch(game);
    // List<IGameAction> result = bfsSolver.Solve();

    // var dfsSolver = new DepthFirstSearch(game, 16);
    // List<IGameAction> result = dfsSolver.Solve();

    // CompositeCommand commands = new CommandParser(result).Parse();
    // CompositeCommand encodedCommand = new PatternEncoder(commands).Encode();
    // Console.WriteLine(encodedCommand.ToRegex());

    // var dfsSolver = new DepthFirstSearch(game, 16);
    // List<List<IGameAction>> results = dfsSolver.AllSolutionAtDepth();
    //
    // foreach (List<IGameAction> result in results)
    // {
    //     CompositeCommand commands = new CommandParser(result).Parse();
    //     CompositeCommand encoded = new PatternEncoder(commands).Encode();
    //     Console.WriteLine(encoded.ToRegex());
    // }
}
TestNewGame();