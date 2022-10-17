using System.Text;

namespace GameSolver.Collection
{
    public class SuffixArray
    {
        public CompositeCommand CompositeCommand { get; init; }

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

            foreach (Suffix s in suffixes)
            {
                var strBuilder = new StringBuilder();
                foreach (BaseCommand c in s.SuffixCommands.Commands)
                {
                    strBuilder.Append(c.ToRegex());
                }

                Console.WriteLine($"index: {s.Index} suffix: {strBuilder}");
            }
        }
    }
}
