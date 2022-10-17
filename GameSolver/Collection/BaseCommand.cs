namespace GameSolver.Collection
{
    public abstract class BaseCommand : ICloneable, IEquatable<BaseCommand>, IComparable<BaseCommand>
    {
        public int Quantity { get; set; }

        public abstract object Clone();
        public abstract bool Equals(BaseCommand? other);
        public abstract string ToRegex();
        public abstract string ToFullString();
        public abstract IIterator<GameAction> CommandIterator();

        public int CompareTo(BaseCommand? other)
        {
            if (other == null)
            {
                return 1;
            }

            IIterator<GameAction> selfIterator = CommandIterator();
            IIterator<GameAction> otherIterator = other.CommandIterator();

            while (selfIterator.HasMore() && otherIterator.HasMore())
            {
                GameAction? selfAction = selfIterator.GetNext();
                GameAction? otherAction = otherIterator.GetNext();

                if (selfAction < otherAction)
                {
                    return -1;
                }
                else if (selfAction > otherAction)
                {
                    return 1;
                }
            }

            if (selfIterator.HasMore())
            {
                return 1;
            }
            else if (otherIterator.HasMore())
            {
                return -1;
            }
            return 0;
        }
    }
}
