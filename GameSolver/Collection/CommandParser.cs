using System.ComponentModel;
using System.Text;
using GameSolver.Core.Action;

namespace GameSolver.Collection;

public sealed class CommandParser
{
    public string Text { get; init; }

    public CommandParser(string text)
    {
        Text = text;
    }

    public CommandParser(IEnumerable<IGameAction> actions)
    {
        var strBuilder = new StringBuilder();

        foreach (IGameAction action in actions)
        {
            strBuilder.Append(action);
        }
            
        Text = strBuilder.ToString();
    }

    private static Command TypeSelect(char c)
    {
        Command command = c switch
        {
            MoveAction.ChUp => new Command(MoveAction.ChUp, 1),
            MoveAction.ChLeft => new Command(MoveAction.ChLeft, 1),
            MoveAction.ChRight => new Command(MoveAction.ChRight, 1),
            MoveAction.ChDown => new Command(MoveAction.ChDown, 1),
            _ => throw new InvalidEnumArgumentException(nameof(c), (int)c, c.GetType())
        };
        return command;
    }

    public CompositeCommand Parse()
    {
        var compositeCommand = new CompositeCommand();
        foreach (char c in Text)
        {
            Command toAddCommand = TypeSelect(c);
            compositeCommand.AddCommand(toAddCommand);
        }
        return compositeCommand;
    }
}