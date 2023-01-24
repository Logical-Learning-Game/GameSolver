// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using GameSolver.Benchmark;
using GameSolver.Collection;
using GameSolver.Collection.Encoder;
using GameSolver.Core;
using GameSolver.Core.Action;
using GameSolver.Solver;
using GameSolver.Solver.ShortestCommand;
using GameSolver.Solver.ShortestPath;
using GameSolver.Utility;


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
    Console.WriteLine("Game with key and door");
    Console.WriteLine("d - door");
    Console.WriteLine("k - key\n");

    /*
     * Blind Search:
         * BreadthFirstSearch:
         * Execution time: 15 ms
         * Peak memory: 27356 KB
         *
         * IterativeDeepeningDepthFirstSearch:
         * Execution time: 181 ms
         * Peak memory: 32172 KB
     */
    const string boardStr = @"
        ....*..xx
		.x.x.x.xx
		..*.k.*.d
		.x.x.x.x.
		P.....r.G
    ";

    /*
     * Blind Search:
         * BreadthFirstSearch:
         * Execution time: 109 ms
         * Peak memory: 32776 KB
         *
         * IterativeDeepeningDepthFirstSearch:
         * Execution time: 78939 ms
         * Peak memory: 32072 KB
     */
    const string boardStr2 = @"
        ......*....xx
        .x.x.x.x.x.xx
        ....*.k.*..xx
        .x.x.x.x.x.xx
        ..*.......*.d
        .x.x.x.x.x.x.
        P.........r.G
    ";

    var game = new Game(boardStr, Direction.Up);
    var initialState = new State(game);
    Console.WriteLine("board: ");

    Console.WriteLine(initialState);
    
    
    var watch = Stopwatch.StartNew();
    
    var solver = new BreadthFirstSearch(game);
    IEnumerable<IGameAction> result = solver.Solve();
    
    watch.Stop();
    var elapsedMs = watch.ElapsedMilliseconds;
    long peakWorkingSet = Process.GetCurrentProcess().PeakWorkingSet64 / 1024;
    
    var gameActions = result.ToList();
    
    Console.WriteLine("result:");
    Console.Write("action count: ");
    Utility.PrintList(gameActions);
    Console.WriteLine(gameActions.Count);
    Console.WriteLine($"Execution time: {elapsedMs} ms");
    Console.WriteLine($"Peak working set: {peakWorkingSet} KB");
    
    Console.WriteLine();
}
//TestNewGame();



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

    //24
    const string boardStr2 = @"
        ......*....xx
        .x.x.x.x.x.xx
        ....*.k.*..xx
        .x.x.x.x.x.xx
        ..*.......*..
        .x.x.x.x.x.x.
        P...........G
    ";
    
    var game = new Game(boardStr, Direction.Up);
    Console.WriteLine("Board:");
    Console.WriteLine(game);
    Console.WriteLine("Solutions:");

    var dfsSolver = new DepthFirstSearch(game, 16);
    IReadOnlyList<IReadOnlyList<IGameAction>> results = dfsSolver.SolveAllSolutionStrategy();
    
    for (int i = 0; i < results.Count; i++)
    {
        CompositeCommand commands = new CommandParser(results[i]).Parse();
        CompositeCommand encoded = new PatternEncoder(commands).Encode();
        Console.WriteLine($"{i + 1}: {encoded.ToFullString()} ==> {encoded.ToRegex()}");
    }
}
//TestAllSolution();


// var bench = new SolverBenchmark(10);
// bench.Run();

// Vector2Int test = Vector2Int.Up;
// Console.WriteLine(test.RotateLeft());
// Console.WriteLine(test);


static void DebugTest()
{
    Console.WriteLine("Debug Test");
    
    const string boardStr = @"
        ..G
        ...
        P.*
    ";
    
    var game = new Game(boardStr, Direction.Up);
    var moveActionFactory = new MoveInteractActionFactory();
    var initialState = new State(game);
    Console.WriteLine("board: ");

    Console.WriteLine(initialState);
    
    //initialState.Update(MoveAction.Up);
    //initialState.Update(MoveAction.Right);
    // initialState.Update(MoveAction.Up);
    // initialState.Update(new CollectAction(MoveAction.Right, TileComponent.Score));
    // initialState.Update(MoveAction.Right);
    // initialState.Update(MoveAction.Right);
    // initialState.Update(MoveAction.Up);
    //initialState.Update(MoveAction.Right);
    
    initialState.Update(MoveAction.Right);
    initialState.Update(moveActionFactory.Up());
    initialState.Update(MoveAction.Left);
    
    Console.WriteLine(initialState);
    Utility.PrintList(initialState.LegalGameActions().ToList());
    // var solver = new BreadthFirstSearch(game);
    // IEnumerable<IGameAction> result = solver.Solve();
    //
    // var gameActions = result.ToList();
    //
    // Console.WriteLine("result:");
    // Console.Write("action count: ");
    // Utility.PrintList(gameActions);
    // Console.WriteLine(gameActions.Count);
}
//DebugTest();

static void TestRunCommand()
{
    Console.WriteLine("Test Run Command");
    
    const string boardStr = @"
        ..G
        ..*
        P.c
    ";

    var game = new Game(boardStr, Direction.Up);
    var moveActionFactory = new MoveInteractActionFactory();
    var testState = new State(game);
    
    var startNode = new CommandNode(new NullAction());
    var node1 = new CommandNode(moveActionFactory.Right());
    var node2 = new CommandNode(moveActionFactory.Up());
    var node3 = new CommandNode(moveActionFactory.Left());
    var node4 = new CommandNode(moveActionFactory.Right());

    startNode.MainBranch = node1;
    node1.MainBranch = node2;
    node2.MainBranch = node3;
    node3.MainBranch = node4;

    var shortestCmdSolver = new ShortestCommandSolver(game, 5);
    
    var watch = Stopwatch.StartNew();

    
    
    //RunCommandResult runResult = shortestCmdSolver.RunCommand(node1, testState);
    //var a = shortestCmdSolver.LegalExistCommandNodes(node1, node4);
    
    watch.Stop();
    var elapsedMs = watch.ElapsedMilliseconds;
    
    // Console.WriteLine(testState);
    // Console.WriteLine($"Run status: {runResult.RunStatus}");
    // Console.WriteLine("State snapshot:");
    // Console.WriteLine(runResult.StateSnapshot);
    // Console.WriteLine($"Time usage: {elapsedMs} ms");
    //Utility.PrintList(a);
    //Console.WriteLine(shortestCmdSolver.LegalExistCommandNodes(node1, node4).Count());
}
//TestRunCommand();

static void TestShortestCommandSolver()
{
    Console.WriteLine("Test Shortest Command Solver");
    
    const string boardStr = @"
        ..G
        ...
        P.*
    ";
    
    const string boardStr2 = @"
        ..G..
        .....
        .....
        .....
        .....
        P....
    ";
    
    const string boardStr3 = @"
        ....G
        ....c
        .x...
        ..c..
        Px...
    ";
    
    const string boardStr4 = @"
        G.c..
        xx...
        ..c..
        Px...
    ";
    
    const string boardStr5 = @"
                     ....*..xx
              		.x.x.x.xx
              		..*...*..
              		.x.x.x.x.
              		P.......G
                  ";
    
    const string boardStr6 = @"
            P.G
        ";

    var game = new Game(boardStr4, Direction.Up);
    var solver = new ShortestCommandSolver(game, 20);
    Console.WriteLine("Board:");
    Console.WriteLine(game);
    
    CommandNode? result = solver.Solve();

    if (result is null)
    {
        Console.WriteLine("Result not found");
    }
    else
    {
        var testState = new State(game);

        RunCommandResult runResult = testState.RunCommand(result);

        Console.Write("Action do: ");
        foreach (IGameAction action in runResult.ActionHistory)
        {
            Console.Write(action);
        }
        Console.WriteLine();
        
        Console.WriteLine($"Run status: {runResult.RunStatus}");
        Console.WriteLine($"Number of commands: {result.Count()}");
        Console.WriteLine("Commands:");
        Console.WriteLine(result);
        
        Console.WriteLine(new ObserveConditionAction(new NullAction()));
    }

}
TestShortestCommandSolver();