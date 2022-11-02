// See https://aka.ms/new-console-template for more information

using GameSolver.Collection;
using GameSolver.Collection.Encoder;
using GameSolver.Game;

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
       BlockedTiles = new HashSet<Tile> { }
    };
    Console.WriteLine("Board:");

    List<GameAction> validActions = board.GetValidActions();
    PrintList(validActions);
    Console.WriteLine(board);


    Board updatedBoard = board.Update(GameAction.Up);
    validActions = updatedBoard.GetValidActions();
    PrintList(validActions);
    Console.WriteLine(updatedBoard);

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