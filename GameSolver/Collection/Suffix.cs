using System.Text;

namespace GameSolver.Collection;

public class Suffix : IComparable<Suffix>
{
    public int Index { get; set; }
    public CompositeCommand SuffixCommands { get; set; }

    public Suffix(int index, CompositeCommand commands)
    {
        Index = index;
        SuffixCommands = commands;
    }

    public override string ToString()
    {
        var strBuilder = new StringBuilder();

        foreach (BaseCommand c in SuffixCommands.Commands)
        {
            strBuilder.Append(c.ToFullString());
        }

        return $"index: {Index} suffix: {strBuilder}";
    }

    public int CompareTo(Suffix? other)
    {
        if (other == null)
        {
            return 1;
        }

        return SuffixCommands.CompareTo(other.SuffixCommands);
    }
}