// See https://aka.ms/new-console-template for more information

using GameSolver.Collection;
using System.Text;

//BaseCommand cmds = new CommandParser("rrllrrllllllllllllllllllllllllllllrrrrrrrrrrrrrrruluuulllu").Parse();

//CompositeCommand? ccmds = cmds as CompositeCommand;

//var rle = new RunLengthEncoder(ccmds!);
//CompositeCommand encoded = rle.Encode();

//rle.EncodingCommand = encoded;
//encoded = rle.Encode();

//Console.WriteLine(encoded.ToRegex());




//IIterator<GameAction> iter = encoded.CommandIterator();
//while (iter.HasMore())
//{
//    Console.Write((char)iter.GetNext());
//}
//Console.WriteLine();


//var cc = new Command(GameAction.Left, 2);
//var dd = new Command(GameAction.Right, 2);
//var ee = new CompositeCommand();
//ee.AddCommand(cc);
//ee.AddCommand(dd);
//var i = ee.CommandIterator();

//while (i.HasMore())
//{
//    Console.Write((char)i.GetNext()!);
//}
//Console.WriteLine();


//// Testcase composite rle
//Console.WriteLine("Testcase Composite RLE");

//var a1 = new Command(GameAction.Left, 1);
//var a2 = new Command(GameAction.Right, 1);
//var a3 = new Command(GameAction.Left, 2);
//var c1 = new CompositeCommand(2);
//c1.AddCommand(a1);
//c1.AddCommand(a2);
//c1.AddCommand(a3);

//Console.WriteLine(c1.ToFullString());

//var n = c1.CommandIterator();
//while (n.HasMore())
//{
//    Console.Write((char)n.GetNext());
//}
//Console.WriteLine();


//var x1 = new Command(GameAction.Left, 1);
//var x2 = new Command(GameAction.Right, 1);
//var x3 = new Command(GameAction.Left, 2);
//var z1 = new CompositeCommand(3);
//z1.AddCommand(x1);
//z1.AddCommand(x2);
//z1.AddCommand(x3);

//var com1 = new CompositeCommand();
//com1.AddCommand(c1);
//com1.AddCommand(z1);
//Console.WriteLine($"Before: {com1.ToRegex()}");

//var rle2 = new RunLengthEncoder(com1);
//CompositeCommand result = rle2.Encode();
//Console.WriteLine($"After: {result.ToRegex()}");

//Console.WriteLine($"FullString: {result.ToFullString()}");

//Console.WriteLine();

//var kk = new CompositeCommand();


// Testcase suffix array construction
static void SuffixArrayConstructionTest()
{
    Console.WriteLine("Testcase suffix array construction");

    var con = new CompositeCommand();

    var c1 = new Command(GameAction.Up, 1);
    var c2 = new Command(GameAction.Up, 1);
    var c3 = new Command(GameAction.Left, 1);
    var c4 = new Command(GameAction.Up, 1);
    con.AddCommand(c1);
    con.AddCommand(c2);
    con.AddCommand(c3);
    con.AddCommand(c4);

    var suffixArray = new SuffixArray(con);

    Console.WriteLine($"Suffix array: {string.Join(", ", suffixArray.Indexs)}");
    Console.WriteLine($"LCP array: {string.Join(", ", suffixArray.LeastCommonPrefixArray())}");
}

// Testcase pattern finding
static void PatternFindTest()
{
    //Console.WriteLine("Testcase pattern finding");

    //var con = new CompositeCommand();

    //var c1 = new Command(GameAction.Up, 1);
    //var c2 = new Command(GameAction.Left, 1);
    //var c3 = new Command(GameAction.Right, 1);


    //con.AddCommand(c1);
    //con.AddCommand(c2);
    //con.AddCommand(c3);

    //con.AddCommand(c1);
    //con.AddCommand(c2);
    //con.AddCommand(c3);

    //var patternEncoder = new PatternEncoder(con);
    //List<PatternEncodeResult> result = patternEncoder.Encode();

    //Console.WriteLine($"Text: {con.ToFullString()}");
    //foreach(PatternEncodeResult r in result)
    //{
    //    Console.WriteLine(r);
    //}
}

static void DynamicProgrammingLRS(string text)
{
    int n = text.Length;
    var lrs = new int[n + 1, n + 1];
    string result = "";
    int resultLength = 0;
    int index = 0;

    for (int i = 1, j = 1; i < n + 1 && j < n + 1; i++, j++)
    {
        lrs[i, j] = i;
    }

    for (int i = 1; i < n + 1; i++)
    {
        for (int j = i + 1; j < n + 1; j++)
        {
            if (text[i - 1] == text[j - 1] && lrs[i - 1, j - 1] < j - i)
            {
                lrs[i, j] = lrs[i - 1, j - 1] + 1;

                if (lrs[i, j] > resultLength)
                {
                    resultLength = lrs[i, j];
                    index = Math.Max(i, index);
                }
            } 
            else
            {
                lrs[i, j] = 0;
            }
        }
    }


    Console.WriteLine("lrs array");

    Console.WriteLine(text);

    int[] scores = new int[n + 1]; 
    for (int i = 1; i < n + 1; i++)
    {
        int j = n;
        int currentScore = 0;
        while (j >= i + 1)
        {
            int val = lrs[i, j];

            if (val > 0)
            {
                if (lrs[i, j - val] >= val)
                {
                    currentScore += val;
                    j -= val;
                }
                else
                {
                    j = j - val - 1;
                }
            }
            else
            {
                j--;
            }
        }
        scores[i - 1] = currentScore;
    }

    for (int i = 1; i < n + 1; i++)
    {
        // int score = -text.Length;
        int score = 0;
        for (int j = 1; j < n + 1; j++)
        {
            //if (j <= i)
            //{
            //    int val = lrs[i, j];

            //    //int patternSize = i;
            //    //while (patternSize < text.Length)
            //    //{
            //    //    if (lrs[i, patternSize + patternSize] == patternSize)
            //    //    {
            //    //        score += val;
            //    //    }
            //    //}

            //    //if (val > 0 && j - val >= 0 && lrs[i, j - val] >= val)
            //    //{
            //    //    score += val;
            //    //}
            //    //else if (val != 0)
            //    //{
            //    //    score--;
            //    //}
            //    //int k = j;
            //    //int startingPoint = text.Length - i;
            //    //while (startingPoint > 0)
            //    //{
            //    //    int val = lrs[i, j + startingPoint];
            //    //    if (startingPoint - k)
            //    //    {
            //    //        score += val;
            //    //    }
            //    //    startingPoint--;
            //    //}

            //    //while (k < n + 1)
            //    //{

            //    //}
            //}

            Console.Write(lrs[i, j]);
            Console.Write(" ");
        }
        Console.Write(text[0..i]);
        Console.Write(" ");
        Console.WriteLine(scores[i - 1]);
    }

    int maxOccurrence = -999999;
    int maxWidth = -999999;
    int suffixNumber = -1;
    int pattern = -1;
    // search
    for (int i = 1; i < n + 1; i++)
    {
        int occurrence = 0;
        
        int currentLeftBound = i;

        for (int j = i + 1; j < n + 1; j++)
        {
            if (lrs[i, j] == 0)
            {
                currentLeftBound++;
                continue;
            };

            int val = lrs[i, j];
            
            // check for previous
            if (j - val >= 0 && lrs[i, j - val] >= val)
            {
                occurrence++;

                int currentWidth = Math.Abs(j - currentLeftBound);
                if (currentWidth > maxWidth)
                {
                    maxWidth = currentWidth;
                    suffixNumber = val;
                    pattern = i;
                }
            }
            else
            {
                currentLeftBound = j;
                maxOccurrence = Math.Max(maxOccurrence, occurrence);
                occurrence = 0;
            }
        }
    }

    Console.WriteLine($"pattern: {pattern}, suffixNumber: {suffixNumber}");

    if (resultLength > 0)
    {
        var strBuilder = new StringBuilder();
        for (int i = index - resultLength + 1; i < index + 1; i++)
        {
            strBuilder.Append(text[i - 1]);
        }
        Console.WriteLine($"result: {strBuilder}");
    } 
    else
    {
        Console.WriteLine("result not found");
    }
}

//SuffixArrayConstructionTest();
//PatternFindTest();

// string text = "aabaaabaaabaaabaaabaaa";
//string text = "baaabaaabaaabaaabaaaaa";
// string text = "abaaabaaabaaabaa";
// string text = "abbbbaabbbbaabbbba";
// string text = "abbbaaabaaabaaabaaabaaa";
// string text = "kkkkkkkk";
// string text = "babananababa";
// string text = "tytujyjyabaaabaaabaaeriothujerobabbabbabfdkslfjlasdf";
//string text = "abbbbcababbbc";
//DynamicProgrammingLRS(text);

static void TestPatternEncoder()
{
    Console.WriteLine("Test pattern encoder");

    //string[] testcases =
    //{
    //    "d",
    //    "uu",
    //    "lllllll",
    //    "udulrulrduudulrulrdu",
    //    "uduuuduuuduudddduuuu",
    //    "uluuuluuuul",
    //    "uluuluuuluuuluu",
    //    "dudddudduudddduuuudddduududdduddddd",
    //    "ddddudddudduudddduuuudddduududddudd"
    //};

    string[] testcases =
    {
        "rruuuurrrrddrrdd",
        "rruuuurrrrddddrr",
        "uurrrruurrddddrr",
        "uurruurrrrddrrdd",
        "rruuuurrrrddddrr",
        "rruurruuddrrddrr"
    };

    for (int i = 0; i < testcases.Length; i++)
    {
        BaseCommand cmd = new CommandParser(testcases[i]).Parse();
        CompositeCommand? compositeCmd = cmd as CompositeCommand;

        CompositeCommand encodedCommand = new PatternEncoder(compositeCmd!).Encode();
        Console.WriteLine($"Testcase {i + 1}: {testcases[i]} => {encodedCommand.ToRegex()}");
    }
}
TestPatternEncoder();