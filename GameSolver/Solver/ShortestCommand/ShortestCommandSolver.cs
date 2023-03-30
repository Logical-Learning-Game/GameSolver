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

                CommandNode copyStartNode = _startNode.CloneAll();
                LoopOptimize(copyStartNode);
                
                // If test run is passed then return optimized solution
                var cloneState = (State)_initialState.Clone();
                RunCommandResult runResult = cloneState.RunCommand(copyStartNode);

                return runResult.RunStatus ? copyStartNode : _startNode;
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
            
            
            // main branch
            if (nodeToFill.MainBranch is null)
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
            
            // conditional branch at conditional node
            if (stateSnapshot.Condition != ConditionalType.None)
            {
                //add intermediate conditional node
                var intermediateConditionalNode = new CommandNode(new NullAction(), conditionalType:stateSnapshot.Condition, isConditionalNode:true)
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
            
            if (
                state.Condition != ConditionalType.None && 
                currentNode.ConditionalType == state.Condition && 
                currentNode.ConditionalBranch is not null &&
                currentNode.IsConditionalNode
                )
            {
                currentNode.ConditionalBranchReachable = true;
                currentNode = currentNode.ConditionalBranch;
                state.Condition = ConditionalType.None;
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

    private bool NodeInLoopAllEqual(CommandNode node)
    {
        CommandNode referencedConditionalNode = node;
        CommandNode? currentNode = node.MainBranch;

        while (currentNode is not null && currentNode.MainBranch is not null && currentNode.MainBranch != referencedConditionalNode)
        {
            if (!currentNode.Action.Equals(currentNode.MainBranch.Action))
            {
                return false;
            }
            
            currentNode = currentNode.MainBranch;
        }

        return true;
    }

    private IDictionary<CommandNode, int> ReferencedTable(CommandNode startNode)
    {
        var result = new Dictionary<CommandNode, int>();
        
        IReadOnlyList<CommandNode> allNodes = startNode.AllNodes();

        foreach (CommandNode node in allNodes)
        {
            result[node] = 0;
        }

        foreach (CommandNode node in allNodes)
        {
            if (node.MainBranch is not null)
            {
                result[node.MainBranch]++;
            }

            if (node.ConditionalBranch is not null)
            {
                result[node.ConditionalBranch]++;
            }
        }

        return result;
    }
    
    private void LoopOptimizeCutNode(CommandNode startNode, CommandNode cutNode)
    {
        IDictionary<CommandNode, int> referencedTable = ReferencedTable(startNode);

        CommandNode referencedConditionalNode = cutNode;
        CommandNode? currentNode = cutNode;
        while (currentNode is not null && currentNode.MainBranch is not null && currentNode.MainBranch != referencedConditionalNode)
        {
            CommandNode? nextNode = currentNode.MainBranch;
            
            // find referencing node of next node
            // if more than than 1 and not current node then remove this node
            if (referencedTable[nextNode] == 1)
            {
                currentNode.MainBranch = nextNode.MainBranch;
            }
            else
            {
                currentNode = currentNode.MainBranch;
            }
        }
    }
    
    private void LoopOptimize(CommandNode startNode)
    {
        IReadOnlyList<CommandNode> allNodes = startNode.AllNodes();

        foreach (CommandNode node in allNodes)
        {
            if (!node.IsConditionalNode)
            {
                continue;
            }

            CommandNode? currentNode = node;
            CommandNode conditionalNode = node;

            //TODO Infinite loop potential,must have proper cycle checking mechanism
            var visitedNode = new HashSet<CommandNode>();
            while (currentNode is not null)
            {
                if (visitedNode.Contains(currentNode))
                {
                    if (currentNode == conditionalNode)
                    {
                        //cycle occur at conditional node
                        // detect if all action in loop are all equal
                        if (NodeInLoopAllEqual(conditionalNode))
                        {
                            LoopOptimizeCutNode(startNode,conditionalNode);
                        }
                    }
                    
                    break;
                }

                visitedNode.Add(currentNode);
                
                currentNode = currentNode.MainBranch;
            }
        }
    }
}