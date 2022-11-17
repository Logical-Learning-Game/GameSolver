using GameSolver.Collection.Iterator;


namespace GameSolver.Collection;

public abstract class BaseCommand : ICloneable, IEquatable<BaseCommand>, IComparable<BaseCommand>
{
    public int Quantity { get; set; }

    // Object clone
    public abstract object Clone();
        
    // String representation
    public abstract string ToRegex();
    public abstract string ToFullString();

    // Iterator
    public abstract IIterator<char> CommandIterator();

    // Equality check
    // Exact match - action and quantity between two instance must be the same to be equal
    public abstract bool Equals(BaseCommand? other);
    // Only action match require to be equal action
    public abstract bool EqualAction(BaseCommand? other);

    // Sorting
    public int CompareTo(BaseCommand? other)
    {
        if (other == null)
        {
            return 1;
        }

        IIterator<char> selfIterator = CommandIterator();
        IIterator<char> otherIterator = other.CommandIterator();

        while (selfIterator.HasMore() && otherIterator.HasMore())
        {
            char? selfAction = selfIterator.GetNext();
            char? otherAction = otherIterator.GetNext();

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