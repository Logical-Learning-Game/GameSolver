using GameSolver.Game;

namespace GameSolver.Collection.Iterator
{
    public class CommandIterator : IIterator<GameAction>
    {
        private readonly Command _command;
        private int _currentPosition;

        public CommandIterator(Command command)
        {
            _command = command;
            _currentPosition = 0;
        }

        public GameAction GetNext()
        {
            _currentPosition++;
            return _command.Action;
        }

        public bool HasMore()
        {
            return _currentPosition < _command.Quantity;
        }
    }
}
