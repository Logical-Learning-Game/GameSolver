using GameSolver.Collection.Iterator;
using System.Text;

namespace GameSolver.Collection;

public class Command : BaseCommand
{
    public char CharAction { get; init; }
    
    public Command() {}
    
    public Command(char action, int quantity)
    {
        Quantity = quantity;
        CharAction = action;
    }

    public override string ToRegex()
    {
        return Quantity > 1 ? $"{CharAction}{{{Quantity}}}" : $"{CharAction}";
    }

    public override string ToFullString()
    {
        var strBuilder = new StringBuilder();

        for (int i = 0; i < Quantity; i++)
        {
            strBuilder.Append(CharAction);
        }

        return strBuilder.ToString();
    }

    public override object Clone()
    {
        return new Command
        {
            Quantity = Quantity,
            CharAction = CharAction
        };
    }

    public override IIterator<char> CommandIterator()
    {
        return new CommandIterator(this);
    }

    public override bool Equals(BaseCommand? other)
    {
        return other is Command otherCommand
               && EqualActionSameType(otherCommand)
               && Quantity == otherCommand.Quantity;
    }

    public override bool EqualAction(BaseCommand? other)
    {
        return other is Command otherCommand && EqualActionSameType(otherCommand);
    }

    private bool EqualActionSameType(Command otherCommand)
    {
        return CharAction == otherCommand.CharAction;
    }
}