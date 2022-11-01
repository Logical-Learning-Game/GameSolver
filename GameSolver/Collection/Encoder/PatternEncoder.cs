using GameSolver.Collection.Iterator;
using GameSolver.Game;

namespace GameSolver.Collection.Encoder
{
    public struct PatternResult
    {
        public int MaxWidth { get; set; }
        public CompositeCommand CommandsPattern { get; set; }
        public List<Tuple<int, int>> Ranges { get; set; }

        public override string ToString()
        {
            var rangeList = new List<string>();
            foreach (Tuple<int, int> range in Ranges)
            {
                string pair = string.Format("({0}, {1}) ", range.Item1, range.Item2);
                rangeList.Add(pair);
            }
            string rangeString = string.Join(", ", rangeList);

            return $"Pattern: {CommandsPattern} MaxWidth: {MaxWidth} RangePair: {rangeString}";
        }
    }


    public class PatternEncoder
    {
        public CompositeCommand EncodingCommand { get; init; }

        public PatternEncoder(CompositeCommand compositeCommand)
        {
            EncodingCommand = compositeCommand;
        }

        private int[,] CreateLongestRepeatSubStringMatrix()
        {
            List<BaseCommand> commands = EncodingCommand.Commands;
            int n = commands.Count;

            var lrs = new int[n + 1, n + 1];

            for (int i = 1; i < n + 1; i++)
            {
                for (int j = i + 1; j < n + 1; j++)
                {
                    if (commands[i - 1].Equals(commands[j - 1]) && lrs[i - 1, j - 1] < j - i)
                    {
                        lrs[i, j] = lrs[i - 1, j - 1] + 1;
                    }
                    else
                    {
                        lrs[i, j] = 0;
                    }
                }
            }

            // Augment diagonal with i where i is the row number of matrix start with 1
            for (int i = 1, j = 1; i < n + 1 && j < n + 1; i++, j++)
            {
                lrs[i, j] = i;
            }

            return lrs;
        }

        private void PrintMatrix(int[,] matrix)
        {
            int n = EncodingCommand.Commands.Count;
            for (int i = 1; i < n + 1; i++)
            {
                for (int j = 1; j < n + 1; j++)
                {
                    Console.Write($"{matrix[i, j]} ");
                }
                Console.Write(EncodingCommand.Take(i).ToRegex());
                Console.WriteLine();
            }
        }

        private bool overlapCheck(List<Tuple<int, int>> ranges,Tuple<int, int> newRange)
        {
            foreach (Tuple<int, int> r in ranges)
            {
                // overlap check
                if (r.Item1 <= newRange.Item2 && newRange.Item1 <= r.Item2)
                {
                    return true;
                }
                else if (r.Item1 <= newRange.Item1 && r.Item2 >= newRange.Item2)
                {
                    return true;
                }
            }
            return false;
        }

        public PatternResult NextPattern()
        {
            int[,] lrsMatrix = CreateLongestRepeatSubStringMatrix();
            // PrintMatrix(lrsMatrix);

            int n = EncodingCommand.Commands.Count;

            // Analyze LRS Matrix for the best pattern at this iteration
            // Analyze each prefix
            int maxPatternWidth = 0;
            int patternSuffix = -1;
            int commandPrefix = -1;
            var rangePairs = new List<Tuple<int, int>>();
            for (int i = 1; i < n + 1; i++)
            {
                int patternWidth = 0;

                // Analyze best pattern in prefix
                int j = n;
                int rightBound = n;
                int previousPattern = -1;
                while (j >= i)
                {
                    // Skip zero match
                    if (lrsMatrix[i, j] == 0)
                    {
                        j--;
                        rightBound--;
                        previousPattern = -1;
                        continue;
                    }
                    
                    // retrieve current pattern and the next pattern to compare
                    int pattern = lrsMatrix[i, j];
                    int nextPattern = lrsMatrix[i, j - pattern];

                    // Found adjacent pattern
                    if (nextPattern >= pattern)
                    {
                        patternWidth += pattern;

                        // Next pattern is changing to bigger pattern
                        if (nextPattern > pattern)
                        {
                            patternWidth += pattern;

                            // Save new pattern to result if it is the largest spanning pattern
                            if (patternWidth > maxPatternWidth)
                            {
                                maxPatternWidth = patternWidth;
                                patternSuffix = pattern;
                                commandPrefix = i;
                                rangePairs.Clear();
                            }

                            // Save same pattern as before if found another interval
                            if (patternSuffix == pattern && commandPrefix == i)
                            {
                                var startEnd = new Tuple<int, int>(j - 2 * pattern, rightBound - 1);
                                if (!overlapCheck(rangePairs, startEnd))
                                {
                                    rangePairs.Add(startEnd);
                                }
                            }

                            rightBound = j - pattern;
                            patternWidth = 0;
                            previousPattern = -1;
                        }
                        else
                        {
                            previousPattern = pattern;
                        }
                        // Shift to next pattern
                        j -= pattern;
                    }
                    else
                    {
                        // Next pattern not match but previous is match then increment pattern width by pattern
                        if (previousPattern > 0 && pattern >= previousPattern)
                        {
                            patternWidth += previousPattern;

                            // Save new pattern to result if it is the largest spanning pattern
                            if (patternWidth > maxPatternWidth)
                            {
                                maxPatternWidth = patternWidth;
                                patternSuffix = pattern;
                                commandPrefix = i;
                                rangePairs.Clear();
                            }

                            // Save same pattern as before if found another interval
                            if (patternSuffix == pattern && commandPrefix == i)
                            {
                                var startEnd = new Tuple<int, int>(j - pattern, rightBound - 1);
                                if (!overlapCheck(rangePairs, startEnd))
                                {
                                    rangePairs.Add(startEnd);
                                }
                            }

                            rightBound = j;
                        }

                        previousPattern = pattern;

                        // Next pattern not match
                        patternWidth = 0;

                        j--;
                        rightBound--;
                    }
                }
                //DEBUG
                //Console.WriteLine($"Pattern: {EncodingCommand.Take(commandPrefix).Skip(commandPrefix - patternSuffix).ToFullString()}");
                //Console.WriteLine($"MaxPatternWidth: {maxPatternWidth}");
                //string debug = "";
                //foreach (var z in rangePairs)
                //{
                //    debug += $"({z.Item1}, {z.Item2})";
                //}
                //Console.WriteLine($"Ranges: {debug}");
            }

            // Result
            rangePairs = rangePairs.OrderBy(t => t.Item1).ToList();

            var result = new PatternResult
            {
                CommandsPattern = EncodingCommand.Take(commandPrefix).Skip(commandPrefix - patternSuffix),
                Ranges = rangePairs,
                MaxWidth = maxPatternWidth
            };
            return result;
        }

        private bool IsOptimal(CompositeCommand compositeCommand)
        {
            List<BaseCommand> commands = compositeCommand.Commands;
            for (int i = 0; i < commands.Count - 1; i++)
            {
                if (commands[i].EqualAction(commands[i + 1]))
                {
                    return false;
                }
            }
            return true;
        }

        public CompositeCommand Encode()
        {
            var encodedCommand = new CompositeCommand
            {
                Quantity = EncodingCommand.Quantity
            };

            PatternResult pattern = NextPattern();
            // Console.WriteLine(pattern);
            if (pattern.MaxWidth == 0)
            {
                return EncodingCommand;
            }

            int patternSize = pattern.CommandsPattern.Commands.Count;

            List<BaseCommand> commands = EncodingCommand.Commands;
            int originalCommandSize = commands.Count;

            int k = 0;
            foreach (Tuple<int, int> r in pattern.Ranges)
            {
                int start = r.Item1;
                int end = r.Item2;

                // Add original command to new container if that command is not pattern
                while (k < originalCommandSize && k < start)
                {
                    encodedCommand.Commands.Add(commands[k]);
                    k++;
                }

                int encodeRound = (end - start + 1) / patternSize;

                // Transform pattern to compress pattern
                CompositeCommand compressPattern = new PatternEncoder(pattern.CommandsPattern).Encode();

                // Assume no sub-pattern is found in this step
                if (compressPattern.Commands.Count == 1)
                {
                    IIterator<GameAction> iter = compressPattern.CommandIterator();
                    GameAction action = iter.GetNext();

                    var encode = new Command(action, encodeRound);
                    encodedCommand.Commands.Add(encode);
                }
                else
                {
                    var encode = new CompositeCommand(compressPattern.Commands, encodeRound);
                    encodedCommand.Commands.Add(encode);
                }

                //// If pattern is optimal then we can add that pattern in the new container
                //// Optimal mean pattern only have subpattern that is contiguous action
                //if (IsOptimal(pattern.CommandsPattern))
                //{
                //    var encode = new CompositeCommand(pattern.CommandsPattern.Commands, encodeRound);
                //    encodedCommand.Commands.Add(encode);
                //}
                //else
                //{
                //    // If pattern is not optimal then we must use run length encoding to encode that pattern first
                //    // Assume pattern have no sub-pattern but only contiguous action
                //    var runLengthEncoder = new RunLengthEncoder(pattern.CommandsPattern);
                //    var optimalPattern = runLengthEncoder.Encode();

                //    var encode = new CompositeCommand(optimalPattern.Commands, encodeRound);
                //    encodedCommand.Commands.Add(encode);
                //}

                k = end + 1;
            }

            // Add original command to new container if that command is not pattern
            while (k < originalCommandSize)
            {
                encodedCommand.Commands.Add(commands[k]);
                k++;
            }

            return new PatternEncoder(encodedCommand).Encode();
        } 
    }
}
