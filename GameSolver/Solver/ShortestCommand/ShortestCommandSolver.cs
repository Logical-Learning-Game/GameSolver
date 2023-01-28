using GameSolver.Core;
using GameSolver.Core.Action;

namespace GameSolver.Solver.ShortestCommand;

public sealed class ShortestCommandSolver
{
    private readonly State _initialState;
    private readonly CommandNode _startNode;
    private readonly int _commandLimit;

    public ShortestCommandSolver(Game game, int commandLimit)
    {
        _initialState = new State(game);
        _commandLimit = commandLimit;
        
        IGameAction startAction = new StartAction();
        _startNode = new CommandNode(startAction, null, null);
    }

    public CommandNode? Solve()
    {
        for (int i = 1; i <= _commandLimit; i++)
        {
            if (SolveBacktrackingStrategy(0, i))
            {
                RemoveUnreachableEdge();
                return _startNode;
            }
        }
        
        return null;
    }

    private bool SolveBacktrackingStrategy(int depth, int limit)
    {
        // if depth is exceed limit then cutoff
        if (depth >= limit)
        {
            return false;
        }
        
        // If test run is passed then return solution
        var cloneState = (State)_initialState.Clone();
        RunCommandResult runResult = cloneState.RunCommand(_startNode);

        if (runResult.RunStatus)
        {
            return true;
        }

        foreach (Tuple<State, CommandNode> expansionPoint in runResult.ExpansionPoints)
        {
            State stateSnapshot = expansionPoint.Item1;
            CommandNode nodeToFill = expansionPoint.Item2;
            
            // Ignore edge to child of this nodeToFill and it self
            var exceptNodes = new HashSet<CommandNode> {nodeToFill};

            if (nodeToFill.MainBranch is not null)
            {
                exceptNodes.Add(nodeToFill.MainBranch);
            }
        
            if (nodeToFill.ConditionalBranch is not null)
            {
                exceptNodes.Add(nodeToFill.ConditionalBranch);
            }
            
            List<CommandNode> existCommandNodes = ExistCommandNodes(_startNode, exceptNodes).ToList();
            List<CommandNode> legalCommandNodes = LegalNewCommandNodes(stateSnapshot).ToList();
            
            
            // conditional branch at conditional node
            if (stateSnapshot.Conditions > 0)
            {
                //add intermediate conditional node
                var intermediateConditionalNode = new CommandNode(new NullAction(), isConditionalNode:true)
                {
                    MainBranch = nodeToFill.MainBranch
                };
                nodeToFill.MainBranch = intermediateConditionalNode;
                
                foreach (CommandNode command in existCommandNodes)
                {
                    intermediateConditionalNode.ConditionalBranch = command;

                    if (SolveBacktrackingStrategy(depth + 1, limit))
                    {
                        if (intermediateConditionalNode.MainBranch is null && intermediateConditionalNode.ConditionalBranch is not null)
                        {
                            nodeToFill.MainBranch = intermediateConditionalNode.ConditionalBranch;
                        }
                        return true;
                    }

                    intermediateConditionalNode.ConditionalBranch = null;
                }

                foreach (CommandNode command in legalCommandNodes)
                {
                    intermediateConditionalNode.ConditionalBranch = command;

                    if (SolveBacktrackingStrategy(depth + 2, limit))
                    {
                        if (intermediateConditionalNode.MainBranch is null && intermediateConditionalNode.ConditionalBranch is not null)
                        {
                            nodeToFill.MainBranch = intermediateConditionalNode.ConditionalBranch;
                        }
                        return true;
                    }

                    intermediateConditionalNode.ConditionalBranch = null;
                }
                
                // remove intermediate conditional node
                nodeToFill.MainBranch = intermediateConditionalNode.MainBranch;
            }
            // main branch
            else if (nodeToFill.MainBranch is null)
            {
                foreach (CommandNode command in existCommandNodes)
                {
                    // try fill main branch first
                    nodeToFill.MainBranch = command;
                    
                    if (SolveBacktrackingStrategy(depth, limit))
                    {
                        return true;
                    }

                    nodeToFill.MainBranch = null;
                }

                foreach (CommandNode command in legalCommandNodes)
                {
                    nodeToFill.MainBranch = command;
                    
                    if (SolveBacktrackingStrategy(depth + 1, limit))
                    {
                        return true;
                    }

                    nodeToFill.MainBranch = null;
                }
            }
        }
        
        return false;
    }

    private IEnumerable<CommandNode> ExistCommandNodes(CommandNode root, HashSet<CommandNode> exceptNodes)
    {
        if (root == _startNode)
        {
            if (root.MainBranch is not null)
            {
                root = root.MainBranch;
            }
            else
            {
                return Array.Empty<CommandNode>();
            }
        }

        var result = new List<CommandNode>();
        
        var queue = new Queue<CommandNode>();
        queue.Enqueue(root);

        var exploredSet = new HashSet<CommandNode> {root};

        while (queue.Count > 0)
        {
            CommandNode node = queue.Dequeue();

            // check if node is not except
            if (!exceptNodes.Contains(node))
            {
                result.Add(node);
            }
            
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

    private IEnumerable<CommandNode> LegalNewCommandNodes(State state)
    {
        var result = new List<CommandNode>();
        
        foreach (IGameAction action in state.LegalGameActions())
        {
            result.Add(new CommandNode(action));
        }

        return result;
    }
    
    private void FlagUnreachableEdge()
    {
        CommandNode? currentNode = _startNode;
        State state = (State)_initialState.Clone();
        
        while (currentNode is not null)
        {
            state.Update(currentNode.Action);

            if (state.IsSolved())
            {
                return;
            }
            
            if (state.Conditions > 0 && currentNode.ConditionalBranch is not null)
            {
                currentNode.ConditionalBranchReachable = true;
                currentNode = currentNode.ConditionalBranch;
                state.Conditions--;
            }
            else
            {
                currentNode.MainBranchReachable = true;
                currentNode = currentNode.MainBranch;
            }
        }
    }

    private void RemoveUnreachableEdge()
    {
        FlagUnreachableEdge();

        var queue = new Queue<CommandNode>();
        queue.Enqueue(_startNode);

        var exploredSet = new HashSet<CommandNode> {_startNode};

        while (queue.Count > 0)
        {
            CommandNode node = queue.Dequeue();

            if (!node.ConditionalBranchReachable)
            {
                node.ConditionalBranch = null;
            }

            if (!node.MainBranchReachable)
            {
                node.MainBranch = null;
            }

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
    }
}