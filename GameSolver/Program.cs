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
		..*.k.*.D
		.x.x.x.x.
		P.....R.G
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
        ..*.......*.D
        .x.x.x.x.x.x.
        P.........R.G
    ";

    const string boardStr3 = @"
        *x..G
        .*.xx
        .x*..
        .*...
        Px*x*
    ";
    
    var gameBuilder = new GameBuilder(boardStr3, Direction.Up);
    var initialState = new State(gameBuilder.Instance);
    Console.WriteLine("board: ");

    Console.WriteLine(initialState);
    
    
    var watch = Stopwatch.StartNew();
    
    var solver = new BreadthFirstSearch(gameBuilder.Instance);
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
    
    var gameBuilder = new GameBuilder(boardStr, Direction.Up);
    Console.WriteLine("Board:");
    Console.WriteLine(gameBuilder.Instance);
    Console.WriteLine("Solutions:");

    var dfsSolver = new DepthFirstSearch(gameBuilder.Instance, 16);
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
    
    var gameBuilder = new GameBuilder(boardStr, Direction.Up);
    var moveActionFactory = new MoveInteractActionFactory();
    var initialState = new State(gameBuilder.Instance);
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

    var gameBuilder = new GameBuilder(boardStr, Direction.Up);
    var moveActionFactory = new MoveInteractActionFactory();
    var testState = new State(gameBuilder.Instance);
    
    var startNode = new CommandNode(new NullAction());
    var node1 = new CommandNode(moveActionFactory.Right());
    var node2 = new CommandNode(moveActionFactory.Up());
    var node3 = new CommandNode(moveActionFactory.Left());
    var node4 = new CommandNode(moveActionFactory.Right());

    startNode.MainBranch = node1;
    node1.MainBranch = node2;
    node2.MainBranch = node3;
    node3.MainBranch = node4;

    var shortestCmdSolver = new ShortestCommandSolver(gameBuilder.Instance, 5);
    
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
        ....e
        .x...
        ..e..
        Px...
    ";
    
    const string boardStr4 = @"
        G.b..
        xx...
        ..b..
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
    
    const string boardStr7 = @"
        .....
        G.c..
        .....
        .x...
        .x...
        ..P..
    ";
    
    const string boardStr8 = @"
        .....
        .....
        G.a..
        .x...
        .x...
        ..P..
    ";

    const string boardStr9 = @"
        ..G..
        .....
        .....
        .....
        ..P..
    ";

    var gameBuilder = new GameBuilder(boardStr8, Direction.Up);
    var solver = new ShortestCommandSolver(gameBuilder.Instance, 20);
    Console.WriteLine("Board:");
    Console.WriteLine(gameBuilder.Instance);
    
    CommandNode? result = solver.Solve();

    if (result is null)
    {
        Console.WriteLine("Result not found");
    }
    else
    {
        var testState = new State(gameBuilder.Instance);

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
    }

}
TestShortestCommandSolver();

static void TestRandomAndSolveMap()
{
    Console.WriteLine("Test Random and solve map");
    
    const string boardStr1 = @"
        ....G
        .....
        .....
        .....
        P....
    ";

    var random = new Random();

    int maxItem = 3;
    int maxObstacle = 5;
    int randomIteration = 10;
    

    for (int iteration = 0; iteration < randomIteration; iteration++)
    {
        var gameBuilder = new GameBuilder(boardStr1, Direction.Up);
        Game game = gameBuilder.Instance;
        int boardHeight = game.Board.GetLength(0);
        int boardWidth = game.Board.GetLength(1);
        
        var occupiedPositions = new List<Vector2Int>
        {
            game.StartPlayerTile,
            game.GoalTile
        };
        
        // random obstacle positions
        for (int i = 0; i < maxObstacle; i++)
        {
            Vector2Int position;
            do
            {
                int randX = random.Next(0, boardWidth);
                int randY = random.Next(0, boardHeight);
                position = new Vector2Int(randX, randY);

            } while (occupiedPositions.Contains(position));
        
            occupiedPositions.Add(position);
            gameBuilder.AddItem(position, TileComponent.Wall.Value);
        }

        // random item positions
        for (int j = 0; j < maxItem; j++)
        {
            Vector2Int position;
            do
            {
                int randX = random.Next(0, boardWidth);
                int randY = random.Next(0, boardHeight);
                position = new Vector2Int(randX, randY);
            } while (occupiedPositions.Contains(position));
            
            occupiedPositions.Add(position);
            gameBuilder.AddItem(position, TileComponent.Score.Value);
        }
    
        gameBuilder.Build();

        // solve with shortest command and shortest path
        var shortestCommandSolver = new ShortestCommandSolver(gameBuilder.Instance, 30);
        var shortestActionSolver = new BreadthFirstSearch(gameBuilder.Instance);

        var shortestActionResult = shortestActionSolver.Solve();
        if (shortestActionResult.Count == 0)
        {
            continue;
        }
        
        var shortestCommandResult = shortestCommandSolver.Solve();
        

        if (shortestCommandResult is not null)
        {
            Console.WriteLine("Random map:");
            Console.WriteLine(gameBuilder.Instance);
            
            Console.WriteLine("Shortest Command Result:");
            Console.WriteLine($"Number of commands: {shortestCommandResult.Count()}");
            Console.WriteLine("Commands:");
            Console.WriteLine(shortestCommandResult);
    
            Console.WriteLine("Shortest Action Result:");
            Console.WriteLine($"Number of actions: {shortestActionResult.Count}");
            Utility.PrintList(shortestActionResult);
            
            Console.WriteLine("-----------------------------------------------------");
        }
    }
    
    Console.WriteLine("Finish");
}
//TestRandomAndSolveMap();