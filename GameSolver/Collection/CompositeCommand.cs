using System.Text;

namespace GameSolver.Collection
{
    public class CompositeCommand : BaseCommand
    {
        public List<BaseCommand> Commands { get; init; } = new();

        public CompositeCommand()
        {
            Quantity = 1;
        }

        public CompositeCommand(int quantity)
        {
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
            return Quantity > 1 ? $"({strBuilder}){{{Quantity}}}" : $"({strBuilder})";
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

            if (otherComposite.Commands.Count != Commands.Count)
            {
                return false;
            }

            bool isEqual = true;
            for (int i = 0; i < Commands.Count; i++)
            {
                isEqual = isEqual && Commands[i].Equals(otherComposite.Commands[i])
                    && Commands[i].Quantity == otherComposite.Commands[i].Quantity;
                if (!isEqual)
                {
                    return false;
                }
            }

            return isEqual;
        }

        public override object Clone()
        {
            return new CompositeCommand
            {
                Commands = new List<BaseCommand>(Commands),
                Quantity = Quantity
            };
        }

        public override int CompareTo(BaseCommand? other)
        {
            throw new NotImplementedException();
        }

        public override IIterator<GameAction> CommandIterator()
        {
            return new CompositeCommandIterator(this);
        }
    }
}
