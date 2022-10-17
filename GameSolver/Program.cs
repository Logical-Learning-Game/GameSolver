// See https://aka.ms/new-console-template for more information

using GameSolver.Collection;

BaseCommand cmds = new CommandParser("rrllrrllllllllllllllllllllllllllllrrrrrrrrrrrrrrruluuulllu").Parse();

CompositeCommand? ccmds = cmds as CompositeCommand;

var rle = new RunLengthEncoder(ccmds!);
CompositeCommand encoded = rle.Encode();

rle.EncodingCommand = encoded;
encoded = rle.Encode();

Console.WriteLine(encoded.ToRegex());




IIterator<GameAction> iter = encoded.CommandIterator();
while (iter.HasMore())
{
    Console.Write((char)iter.GetNext());
}
Console.WriteLine();


var cc = new Command(GameAction.Left, 2);
var dd = new Command(GameAction.Right, 2);
var ee = new CompositeCommand();
ee.AddCommand(cc);
ee.AddCommand(dd);
var i = ee.CommandIterator();

while (i.HasMore())
{
    Console.Write((char)i.GetNext()!);
}
Console.WriteLine();


// Testcase composite rle
Console.WriteLine("Testcase Composite RLE");

var a1 = new Command(GameAction.Left, 3);
var a2 = new Command(GameAction.Right, 1);
var a3 = new Command(GameAction.Left, 2);
var c1 = new CompositeCommand(3);
c1.AddCommand(a1);
c1.AddCommand(a2);
c1.AddCommand(a3);

Console.WriteLine(c1.ToFullString());

var n = c1.CommandIterator();
while (n.HasMore())
{
    Console.Write((char)n.GetNext());
}
Console.WriteLine();


var x1 = new Command(GameAction.Left, 1);
var x2 = new Command(GameAction.Right, 1);
var x3 = new Command(GameAction.Left, 2);
var z1 = new CompositeCommand(16);
z1.AddCommand(x1);
z1.AddCommand(x2);
z1.AddCommand(x3);

var com1 = new CompositeCommand();
com1.AddCommand(c1);
com1.AddCommand(z1);
Console.WriteLine($"Before: {com1.ToRegex()}");

var rle2 = new RunLengthEncoder(com1);
CompositeCommand result = rle2.Encode();
Console.WriteLine($"After: {result.ToRegex()}");

Console.WriteLine($"FullString: {result.ToFullString()}");

Console.WriteLine();

var kk = new CompositeCommand();


// Testcase suffix array construction
static void SuffixArrayConstructionTest()
{
    Console.WriteLine("Testcase suffix array construction");

    var con = new CompositeCommand();
    var con1 = new CompositeCommand(1);
    var con2 = new CompositeCommand(1);

    var c1 = new Command(GameAction.Up, 1);
    var c2 = new Command(GameAction.Up, 1);
    var c3 = new Command(GameAction.Left, 1);
    var c4 = new Command(GameAction.Up, 1);
    con1.AddCommand(c1);
    con1.AddCommand(c2);
    con2.AddCommand(c3);
    con2.AddCommand(c4);
    con.AddCommand(con1);
    con.AddCommand(con2);

    var suffixArray = new SuffixArray(con);
}

SuffixArrayConstructionTest();