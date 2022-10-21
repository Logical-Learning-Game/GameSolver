namespace GameSolver.Collection
{
    public class SuffixArray
    {
        public CompositeCommand CompositeCommand { get; init; }
        public int[] Indexs { get; init; }

        public SuffixArray(CompositeCommand commands)
        {
            CompositeCommand = commands;

            List<BaseCommand> commandsList = CompositeCommand.Commands;
            int commandsLength = commandsList.Count;

            var suffixes = new Suffix[commandsLength];
            for (int i = 0; i < commandsLength; i++)
            {
                List<BaseCommand> slicedList = commandsList.Skip(i).ToList();
                var slicedCompositeCommand = new CompositeCommand(slicedList);
                suffixes[i] = new Suffix(i, slicedCompositeCommand);
            }

            Array.Sort(suffixes);

            var suffixArray = new int[commandsLength];
            for (int i = 0; i < commandsLength; i++)
            {
                suffixArray[i] = suffixes[i].Index;
            }

            Indexs = suffixArray;
        }

        public int[] LeastCommonPrefixArray()
        {
            int suffixArrSize = Indexs.Length;

            var lcpArray = new int[suffixArrSize];

            var invSuff = new int[suffixArrSize];
            for (int i = 0; i < suffixArrSize; i++)
            {
                invSuff[Indexs[i]] = i;
            }

            int k = 0;
            for (int i = 0; i < suffixArrSize; i++)
            {
                if (invSuff[i] == suffixArrSize - 1)
                {
                    k = 0;
                    continue;
                }

                int j = Indexs[invSuff[i] + 1];
                List<BaseCommand> commands = CompositeCommand.Commands;
                while (i + k < suffixArrSize && j + k < suffixArrSize && commands[i + k].Equals(commands[j + k]))
                {
                    k++;
                }

                lcpArray[invSuff[i]] = k;

                if (k > 0)
                {
                    k--;
                }
            }
            return lcpArray;
        }
    }
}
