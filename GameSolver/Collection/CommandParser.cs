using GameSolver.Game;

namespace GameSolver.Collection
{
    public class CommandParser
    {
        public string Text { get; init; }

        public CommandParser(string text)
        {
            Text = text;
        }

        private static Command TypeSelect(char c)
        {
            var action = (GameAction)c;
            if (!Enum.IsDefined(action))
            {
                throw new Exception($"GameAction is not defined {(char)action}");
            }

            Command command = action switch
            {
                GameAction.Up => new Command(GameAction.Up, 1),
                GameAction.Left => new Command(GameAction.Left, 1),
                GameAction.Right => new Command(GameAction.Right, 1),
                GameAction.Down => new Command(GameAction.Down, 1),
                _ => throw new Exception($"cannot select any action from {(char)action}"),
            };
            return command;
        }

        public BaseCommand Parse()
        {
            if (Text.Length == 0)
            {
                return new CompositeCommand();
            }

            var compositeCommand = new CompositeCommand();
            foreach (char c in Text)
            {
                Command toAddCommand = TypeSelect(c);
                compositeCommand.AddCommand(toAddCommand);
            }
            return compositeCommand;
        }
    }
}
