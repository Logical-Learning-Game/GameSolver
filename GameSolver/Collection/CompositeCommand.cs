using System.Text;

namespace GameSolver.Collection
{
    public class CompositeCommand : BaseCommand
    {
        public List<BaseCommand> Commands { get; init; } = new();

        public CompositeCommand(int quantity = 1)
        {
            Quantity = quantity;
        }

        public CompositeCommand(List<BaseCommand> commands, int quantity = 1)
        {
            Commands = commands;
            Quantity = quantity;
        }

        public void AddCommand(BaseCommand command)
        {
            Commands.Add(command);
        }

        public BaseCommand At(int index)
        {
            return Commands[index];
        }

        public override string ToRegex()
        {
            var strBuilder = new StringBuilder();

            foreach (BaseCommand c in Commands)
            {
                strBuilder.Append(c.ToRegex());
            }
            return Quantity > 1 ? $"({strBuilder}){{{Quantity}}}" : $"{strBuilder}";
        }

        public override string ToFullString()
        {
            var strBuilder = new StringBuilder();

            for (int i = 0; i < Quantity; i++)
            {
                foreach (BaseCommand c in Commands)
                {
                    strBuilder.Append(c.ToFullString());
                }
            }

            return strBuilder.ToString();
        }

        public override bool Equals(BaseCommand? other)
        {
            if (other is not CompositeCommand otherComposite)
            {
                return false;
            }
            return otherComposite.Quantity == Quantity && EqualActionSameType(otherComposite);
        }

        public override object Clone()
        {
            return new CompositeCommand
            {
                Commands = new List<BaseCommand>(Commands),
                Quantity = Quantity
            };
        }

        public override IIterator<GameAction> CommandIterator()
        {
            return new CompositeCommandIterator(this);
        }

        public override bool EqualAction(BaseCommand? other)
        {
            if (other is not CompositeCommand otherComposite)
            {
                return false;
            }

            return EqualActionSameType(otherComposite);
        }

        public CompositeCommand Skip(int n)
        {
            return new CompositeCommand(Commands.Skip(n).ToList(), Quantity);
        }

        public CompositeCommand Take(int n)
        {
            return new CompositeCommand(Commands.Take(n).ToList(), Quantity);
        }

        public override string ToString()
        {
            return ToRegex();
        }

        private bool EqualActionSameType(CompositeCommand otherComposite)
        {
            if (otherComposite.Commands.Count != Commands.Count)
            {
                return false;
            }

            bool isEqual = true;
            for (int i = 0; i < Commands.Count; i++)
            {
                isEqual = isEqual && Commands[i].Equals(otherComposite.Commands[i]);
                if (!isEqual)
                {
                    return false;
                }
            }

            return isEqual;
        }
    }
}
