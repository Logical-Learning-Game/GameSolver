using GameSolver.Game;

namespace GameSolver.Collection.Iterator
{
    public class CompositeCommandIterator : IIterator<GameAction>
    {
        private readonly CompositeCommand _compositeCommand;
        private int _currentPosition;
        private int _round;
        private IIterator<GameAction> _currentIterator;

        public CompositeCommandIterator(CompositeCommand compositeCommand)
        {
            _compositeCommand = compositeCommand;
            _currentPosition = 0;
            _round = 0;
            _currentIterator = _compositeCommand.Commands[0].CommandIterator();
        }

        public GameAction GetNext()
        {
            GameAction action = _currentIterator.GetNext();

            int commandsCount = _compositeCommand.Commands.Count;
            while (!_currentIterator.HasMore())
            {
                _currentPosition++;

                if (_currentPosition == commandsCount)
                {
                    _currentPosition %= commandsCount;
                    _round++;
                }

                _currentIterator = _compositeCommand.Commands[_currentPosition].CommandIterator();
            }

            return action;
        }

        public bool HasMore()
        {
            return _round < _compositeCommand.Quantity;
        }
    }
}
