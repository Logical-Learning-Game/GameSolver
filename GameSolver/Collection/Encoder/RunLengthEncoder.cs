namespace GameSolver.Collection.Encoder
{
    public class RunLengthEncoder
    {
        public CompositeCommand EncodingCommand { get; set; }

        public RunLengthEncoder(CompositeCommand compositeCommand)
        {
            EncodingCommand = compositeCommand;
        }

        public CompositeCommand Encode()
        {
            var encodedCommand = new CompositeCommand
            {
                Quantity = EncodingCommand.Quantity
            };

            List<BaseCommand> commands = EncodingCommand.Commands;

            int i = 0;
            while (i < commands.Count)
            {
                int count = 0;
                while (i < commands.Count - 1 && commands[i].EqualAction(commands[i + 1]))
                {
                    count += commands[i].Quantity;
                    i++;
                }
                count += commands[i].Quantity;

                var copyCommand = (BaseCommand)commands[i].Clone();
                copyCommand.Quantity = count;
                encodedCommand.AddCommand(copyCommand);

                i++;
            }
            return encodedCommand;
        }
    }
}
