namespace GameSolver.Collection
{
    public abstract class BaseCommand : ICloneable, IEquatable<BaseCommand>, IComparable<BaseCommand>
    {
        public int Quantity { get; set; }

        public abstract object Clone();
        public abstract bool Equals(BaseCommand? other);
        public abstract string ToRegex();
        public abstract string ToFullString();
        public abstract int CompareTo(BaseCommand? other);

        public abstract IIterator<GameAction> CommandIterator();
    }
}
