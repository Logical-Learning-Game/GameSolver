using System.Text;

namespace GameSolver.Collection
{
    public class Suffix : IComparable<Suffix>
    {
        public int Index { get; set; }
        public List<BaseCommand>? List { get; set; }

        public Suffix() { }

        public Suffix(int index, List<BaseCommand> list)
        {
            Index = index;
            List = list;
        }

        public override string ToString()
        {
            if (List == null)
            {
                return string.Empty;
            }

            var strBuilder = new StringBuilder();

            foreach (BaseCommand c in List)
            {
                strBuilder.Append(c.ToFullString());
            }

            return $"index: {Index} suffix: {strBuilder}";
        }

        public int CompareTo(Suffix? other)
        {
            return 0;
        }
    }
}
