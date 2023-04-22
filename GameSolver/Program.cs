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
using GameSolver;


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

    const string boardStr10 = @"
        21xxx
        P...G
    ";

    // const string boardStr11 = @"
    //     P.R.G
    // ";
    // var board11 = new GameBuilder(boardStr11, Direction.Up);
    // board11.AddDoor(2, 1,DoorType.DoorA, Direction.Right, false);
    // board11.AddDoor(3, 1,DoorType.DoorB, Direction.Right, false);
    
    // ---------------- final map set -----------------------
    //    E
    //N --x----S
    //    W
    //
    
    // T-1 A
    const string boardStrT1 = @"
        x.x
        x.G
        x.x
        x.x
        xPx
    ";
    var boardT1 = new GameBuilder(boardStrT1, Direction.Up);

    // T-2 L
    const string boardStrT2 = @"
        xPx
        ...
        .x.
        ..G
    ";
    var boardT2 = new GameBuilder(boardStrT2, Direction.Up);
    
    // T-3 M
    const string boardStrT3 = @"
        G
        .
        .
        .
        .
        P
    ";
    var boardT3 = new GameBuilder(boardStrT3, Direction.Up);

    // T-4 N
    const string boardStrT4 = @"
        ....
        .xx.
        Pxx.
        xG..
    ";
    var boardT4 = new GameBuilder(boardStrT4, Direction.Up);
    
    // T-5 O
    const string boardStrT5 = @"
        xGx
        ...
        1x2
        ...
        xPx
    ";
    var boardT5 = new GameBuilder(boardStrT5, Direction.Up);
    boardT5.AddDoor(1, 0, DoorType.DoorA, Direction.Down, false);
    
    // T-6 Q
    const string boardStrT6 = @"
        xxx..G
        x..1x.
        P.....
    ";
    var boardT6 = new GameBuilder(boardStrT6, Direction.Right);
    boardT6.AddDoor(5, 0, DoorType.DoorA, Direction.Left, false);
    boardT6.AddDoor(5, 0, DoorType.DoorA, Direction.Down, false);
    
    // W1-1 E
    const string boardStrW11 = @"
        x.x
        .aG
        x.x
        x.x
        xPx
    ";
    var boardW11 = new GameBuilder(boardStrW11, Direction.Up);
    
    // W1-2 K
    const string boardStrW12 = @"
        acba
        G.xc
        xx.b
        P..a
    ";
    var boardW12 = new GameBuilder(boardStrW12, Direction.Right);
    
    // W1-3 I
    const string boardStrW13 = @"
        ....
        G.x.
        xx..
        P...
    ";
    var boardW13 = new GameBuilder(boardStrW13, Direction.Right);
    
    // W1-4 C
    const string boardStrW14 = @"
        ....
        .xx.
        Pxx.
        ..a1
        xxGx
    ";
    var boardW14 = new GameBuilder(boardStrW14, Direction.Up);
    boardW14.AddDoor(2, 4, DoorType.DoorA, Direction.Up, false);
    
    // W1-5 D
    const string boardStrW15 = @"
        ....
        a.ax
        .x.x
        Gx.P
    ";
    var boardW15 = new GameBuilder(boardStrW15, Direction.Left);

    // W1-6 G
    const string boardStrW16 = @"
        ....*..xx
        .x.x.x.xx
        ..*...*..
        .x.x.x.x.
        P.......G
    ";
    var boardW16 = new GameBuilder(boardStrW16, Direction.Up);
    boardW16.AddItem(new Vector2Int(2, 2), TileComponent.KeyA.Value);
    boardW16.AddItem(new Vector2Int(4, 0), TileComponent.KeyB.Value);
    boardW16.AddItem(new Vector2Int(6, 2), TileComponent.KeyC.Value);
    
    // W1-7 F
    const string boardStrW17 = @"
        P...
        xx..
        .xx.
        a...
        Gxxx
    ";
    var boardW17 = new GameBuilder(boardStrW17, Direction.Up);
    
    // W2-1 B
    const string boardStrW21 = @"
        ....G
        .x1..
        ...x.
        P....
    ";
    var boardW21 = new GameBuilder(boardStrW21, Direction.Up);
    boardW21.AddDoor(3, 0, DoorType.DoorA, Direction.Right, false);
    boardW21.AddDoor(4, 1, DoorType.DoorA, Direction.Up, false);
    
    // W2-2 P
    const string boardStrW22 = @"
        xxxxG
        1x2x.
        .....
        xx.xx
        P..xx
    ";
    var boardW22 = new GameBuilder(boardStrW22, Direction.Right);
    boardW22.AddDoor(4, 1, DoorType.DoorA, Direction.Up, false);
    boardW22.AddDoor(4, 1, DoorType.DoorB, Direction.Down, false);
    
    // W2-3 H
    const string boardStrW23 = @"
        .a.b.a
        a.b.b.
        .a.G1a
        Pxb.b.
    ";
    var boardW23 = new GameBuilder(boardStrW23, Direction.Up);
    boardW23.AddDoor(3, 2, DoorType.DoorA, Direction.Up, false);
    boardW23.AddDoor(3, 2, DoorType.DoorA, Direction.Right, false);
    boardW23.AddDoor(3, 2, DoorType.DoorA, Direction.Down, false);
    boardW23.AddDoor(3, 2, DoorType.DoorA, Direction.Left, false);
    
    // W2-4 J
    const string boardStrW24 = @"
        xP1
        .a.
        .bx
        xax
        x.G
    ";
    var boardW24 = new GameBuilder(boardStrW24, Direction.Up);
    boardW24.AddDoor(1, 1, DoorType.DoorB, Direction.Up, false);
    boardW24.AddDoor(1, 1, DoorType.DoorB, Direction.Down, false);
    boardW24.AddDoor(2, 4, DoorType.DoorA, Direction.Left, false);

    const string boardStrBug = @"
        P.
        ..
        a.
        G.
    ";
    var boardBug = new GameBuilder(boardStrBug, Direction.Up);
    boardBug.AddDoor(0, 1, DoorType.DoorNoKey, Direction.Up, false);
    //boardBug.AddDoor(0, 1, DoorType.DoorNoKey, Direction.Down, false);
    //boardBug.AddDoor(1, 2, DoorType.DoorA, Direction.Left, false);

    const string boardStr13 = @"
        ....G
        .x1..
        ...x.
        P....
    ";
    var board13 = new GameBuilder(boardStr13, Direction.Up);
    board13.AddDoor(3, 0, DoorType.DoorA, Direction.Right, false);
    board13.AddDoor(4, 1, DoorType.DoorA, Direction.Up, false);
    board13.AddDoor(0, 3, DoorType.DoorNoKey, Direction.Right, false);
    board13.AddDoor(0, 3, DoorType.DoorNoKey, Direction.Up, false);

    const string boardStr14 = @"
        ....
        .xx.
        Pxx.
        ..a1
        xxGx
    ";
    var board14 = new GameBuilder(boardStr14, Direction.Up);
    board14.AddDoor(2, 4, DoorType.DoorA, Direction.Up, false);

    const string boardStr15 = @"
        ....
        a.ax
        .x.x
        Gx.P
    ";
    var board15 = new GameBuilder(boardStr15, Direction.Left);
    
    

    const string boardStr17 = @"
        P...
        xx..
        .xx.
        a...
        Gxxx
    ";
    var board17 = new GameBuilder(boardStr17, Direction.Up);
    
    const string boardStr18 = @"
        ....*..xx
        .x.x.x.xx
        ..*...*..
        .x.x.x.x.
        P.......G
    ";
    var board18 = new GameBuilder(boardStr18, Direction.Up);
    board18.AddItem(new Vector2Int(2, 2), TileComponent.KeyA.Value);
    board18.AddItem(new Vector2Int(4, 0), TileComponent.KeyB.Value);
    board18.AddItem(new Vector2Int(6, 2), TileComponent.KeyC.Value);
    
    const string boardStr19 = @"
        .a.b.a
        a.b.b.
        .a.G1a
        Pxb.b.
    ";
    var board19 = new GameBuilder(boardStr19, Direction.Up);
    board19.AddDoor(3, 2, DoorType.DoorA, Direction.Up, false);
    board19.AddDoor(3, 2, DoorType.DoorA, Direction.Right, false);
    board19.AddDoor(3, 2, DoorType.DoorA, Direction.Down, false);
    board19.AddDoor(3, 2, DoorType.DoorA, Direction.Left, false);
    
    const string boardStr20 = @"
        a..b
        G.x.
        xx..
        P..a
    ";
    var board20 = new GameBuilder(boardStr20, Direction.Right);
    
    const string boardStr21 = @"
        .a.b.a
        a.b.b.
        .a.G1a
        Pxb.b.
    ";
    var board21 = new GameBuilder(boardStr21, Direction.Up);
    board21.AddDoor(3, 2, DoorType.DoorA, Direction.Up, false);
    board21.AddDoor(3, 2, DoorType.DoorA, Direction.Right, false);
    board21.AddDoor(3, 2, DoorType.DoorA, Direction.Down, false);
    board21.AddDoor(3, 2, DoorType.DoorA, Direction.Left, false);
    
    const string boardStr22 = @"
        accxabcde
        PabcdxeaG
    ";
    var board22 = new GameBuilder(boardStr22, Direction.Right);
    
    const string boardStr23 = @"
        G.......c
        ..x......
        xxxxxxxx.
        .a......b
        ..x......
        .Px......
    ";
    var board23 = new GameBuilder(boardStr23, Direction.Up);
    
    const string boardStr24 = @"
        P.....G
    ";
    var board24 = new GameBuilder(boardStr24, Direction.Up);
    board24.AddDoor(0, 0, DoorType.DoorNoKey, Direction.Right, false);
    board24.AddDoor(1, 0, DoorType.DoorNoKey, Direction.Right, false);
    board24.AddDoor(2, 0, DoorType.DoorNoKey, Direction.Right, false);
    
    var game = boardW24.Instance;
    var solver = new ShortestCommandSolver(game, 15);
    Console.WriteLine("Board:");
    Console.WriteLine(game);

    string stringOfList = string.Join(", ", game.StandardBoardFormat());
    string stringOfListWithBracket = $"{{{stringOfList}}}";
    Console.WriteLine($"Board standard format: {stringOfListWithBracket}");
    Console.WriteLine($"Board Height: {game.Board.GetLength(0)}");
    Console.WriteLine($"Board Width: {game.Board.GetLength(1)}");
    Console.WriteLine($"Start Player Position: {game.StartPlayerTile}");
    Console.WriteLine($"Goal Position: {game.GoalTile}");

    CommandNode? result = solver.Solve();

    if (result is null)
    {
        Console.WriteLine("Result not found");
    }
    else
    {
        var testState = new State(game);

        RunCommandResult runResult = testState.RunCommand(result);
        int numberOfCommand = result.Count();
        int numberOfAction = runResult.CommandHistory.Count;

        Console.WriteLine($"Least Solvable Command: {numberOfCommand} {(int)Math.Ceiling(1.25 * numberOfCommand)} {(int)Math.Ceiling(1.25 * 1.25 * numberOfCommand)}");
        Console.WriteLine($"Least Solvable Action: {numberOfAction} {(int)Math.Ceiling(1.25 * numberOfAction)} {(int)Math.Ceiling(1.25 * 1.25 * numberOfAction)}");
        Console.WriteLine($"Number of actions: {numberOfAction}");
        Console.Write("Action do: ");
        foreach (IGameAction action in runResult.ActionHistory)
        {
            Console.Write(action);
        }
        Console.WriteLine();
        
        Console.WriteLine($"Run status: {runResult.RunStatus}");
        Console.WriteLine($"Number of commands: {numberOfCommand}");
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