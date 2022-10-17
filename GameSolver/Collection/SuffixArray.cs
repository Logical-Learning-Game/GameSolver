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
                suffixes[i] = new Suffix
                {
                    Index = i,

                };
            }
        }
    }
}
