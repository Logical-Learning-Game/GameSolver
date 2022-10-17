namespace GameSolver.Collection
{
    public class CompositeCommandIterator : IIterator<GameAction>
    {
        private readonly CompositeCommand _compositeCommand;
        private int _currentPosition;
        private IIterator<GameAction> _currentIterator;

        public CompositeCommandIterator(CompositeCommand compositeCommand)
        {
            _compositeCommand = compositeCommand;
            _currentPosition = 0;
            _currentIterator = _compositeCommand.Commands[0].CommandIterator();
        }

        public GameAction GetNext()
        {
            while (!_currentIterator.HasMore())
            {
                _currentPosition++;
                _currentIterator = _compositeCommand.Commands[_currentPosition].CommandIterator();
            }

            return _currentIterator.GetNext();
        }

        public bool HasMore()
        {
            return _currentPosition < _compositeCommand.Commands.Count - 1 || _currentIterator.HasMore();
        }
    }
}
