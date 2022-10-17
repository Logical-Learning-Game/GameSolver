namespace GameSolver.Collection
{
    public interface IIterator<T>
    {
        T GetNext();
        bool HasMore();
    }
}
