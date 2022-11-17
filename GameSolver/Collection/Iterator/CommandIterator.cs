namespace GameSolver.Collection.Iterator;

public class CommandIterator : IIterator<char>
{
    private readonly Command _command;
    private int _currentPosition;

    public CommandIterator(Command command)
    {
        _command = command;
        _currentPosition = 0;
    }

    public char GetNext()
    {
        _currentPosition++;
        return _command.CharAction;
    }

    public bool HasMore()
    {
        return _currentPosition < _command.Quantity;
    }
}