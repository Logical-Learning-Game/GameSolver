using System.Text;

namespace GameSolver.Collection
{
    public class Command : BaseCommand
    {
        public GameAction Action { get; init; }
        public char CharAction { get; init; }

        public Command() { }

        public Command(GameAction action, int quantity)
        {
            Action = action;
            Quantity = quantity;
            CharAction = (char)Action;
        }

        public override string ToRegex()
        {
            return Quantity > 1 ? $"{CharAction}{{{Quantity}}}" : $"{CharAction}";
        }

        public override string ToFullString()
        {
            var strBuilder = new StringBuilder();

            for (int i = 0; i < Quantity; i++)
            {
                strBuilder.Append(CharAction);
            }

            return strBuilder.ToString();
        }

        public override bool Equals(BaseCommand? other)
        {
            return other is Command otherCommand && Action == otherCommand.Action;
        }

        public override object Clone()
        {
            return new Command
            {
                Action = Action,
                Quantity = Quantity,
                CharAction = CharAction
            };
        }

        public override int CompareTo(BaseCommand? other)
        {
            return 0;
        }

        public override IIterator<GameAction> CommandIterator()
        {
            return new CommandIterator(this);
        }
    }
}
