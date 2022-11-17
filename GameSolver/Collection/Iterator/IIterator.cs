namespace GameSolver.Collection.Iterator;

public interface IIterator<T>
{
    T GetNext();
    bool HasMore();
}