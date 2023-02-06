using System.Text;
using GameSolver.Core.Action;

namespace GameSolver.Solver.ShortestCommand;

public sealed class CommandNode
{
    public IGameAction Action { get; set; }
    public CommandNode? MainBranch { get; set; }
    public CommandNode? ConditionalBranch { get; set; }
    public ConditionalType ConditionalType { get; set; }
    public bool ConditionalBranchFilled { get; set; }
    public bool MainBranchReachable { get; set; }
    public bool ConditionalBranchReachable { get; set; }
    public bool IsConditionalNode { get; set; }

    public CommandNode(
        IGameAction action, 
        CommandNode? mainBranch = null, 
        CommandNode? conditionalBranch = null,
        ConditionalType conditionalType = ConditionalType.None,
        bool conditionalBranchFilled = false, 
        bool mainBranchReachable = false, 
        bool conditionalBranchReachable = false,
        bool isConditionalNode = false
        )
    {
        Action = action;
        MainBranch = mainBranch;
        ConditionalBranch = conditionalBranch;
        ConditionalType = conditionalType;
        ConditionalBranchFilled = conditionalBranchFilled;
        MainBranchReachable = mainBranchReachable;
        ConditionalBranchReachable = conditionalBranchReachable;
        IsConditionalNode = isConditionalNode;
    }

    public IReadOnlyList<CommandNode> AllNodes()
    {
        var nodes = new List<CommandNode>();
        
        var queue = new Queue<CommandNode>();
        queue.Enqueue(this);
        var exploredSet = new HashSet<CommandNode> {this};

        while (queue.Count > 0)
        {
            CommandNode node = queue.Dequeue();
            
            nodes.Add(node);

            if (node.MainBranch is not null && !exploredSet.Contains(node.MainBranch))
            {
                exploredSet.Add(node.MainBranch);
                queue.Enqueue(node.MainBranch);
            }
            
            if (node.ConditionalBranch is not null && !exploredSet.Contains(node.ConditionalBranch))
            {
                exploredSet.Add(node.ConditionalBranch);
                queue.Enqueue(node.ConditionalBranch);
            } 
        }

        return nodes;
    }

    public int Count()
    {
        var queue = new Queue<CommandNode>();
        queue.Enqueue(this);
        var exploredSet = new HashSet<CommandNode> {this};

        int result = 0;
        
        while (queue.Count > 0)
        {
            CommandNode node = queue.Dequeue();
            result++;

            if (node.MainBranch is not null && !exploredSet.Contains(node.MainBranch))
            {
                exploredSet.Add(node.MainBranch);
                queue.Enqueue(node.MainBranch);
            }
            
            if (node.ConditionalBranch is not null && !exploredSet.Contains(node.ConditionalBranch))
            {
                exploredSet.Add(node.ConditionalBranch);
                queue.Enqueue(node.ConditionalBranch);
            } 
        }

        return result;
    }

    public override string ToString()
    {
        var strBuilder = new StringBuilder();
        IReadOnlyList<CommandNode> commandNodePositions = AllNodes();

        var invertIndexLookup = new Dictionary<CommandNode, int>();
        for (int i = 0; i < commandNodePositions.Count; i++)
        {
            CommandNode node = commandNodePositions[i];
            invertIndexLookup[node] = i;
        }

        for (int i = 0; i < commandNodePositions.Count; i++)
        {
            CommandNode node = commandNodePositions[i];

            if (node.MainBranch is not null)
            {
                int mainIndex = invertIndexLookup[node.MainBranch];
                
                if (node.ConditionalBranch is not null && node.IsConditionalNode)
                {
                    int conditionalIndex = invertIndexLookup[node.ConditionalBranch];

                    char conditionCh = node.ConditionalType switch
                    {
                        ConditionalType.None => ' ',
                        ConditionalType.ConditionalA => 'A',
                        ConditionalType.ConditionalB => 'B',
                        ConditionalType.ConditionalC => 'C',
                        ConditionalType.ConditionalD => 'D',
                        ConditionalType.ConditionalE => 'E',
                        _ => throw new ArgumentOutOfRangeException()
                    };
                    
                    strBuilder.AppendLine($"{i + 1}. if (Condition {conditionCh}) then {conditionalIndex + 1} else {mainIndex + 1}");
                }
                else
                {
                    strBuilder.AppendLine($"{i + 1}. {node.Action} -> {mainIndex + 1}");
                }
            }
            else
            {
                strBuilder.AppendLine($"{i + 1}. {node.Action}");
            }
        }
        
        return strBuilder.ToString();
    }
}