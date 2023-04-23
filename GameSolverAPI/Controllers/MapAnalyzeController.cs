using GameSolver.Core;
using GameSolver.Core.Action;
using GameSolver.Solver.ShortestCommand;
using GameSolver.Solver.ShortestPath;
using GameSolverAPI.Models;
using GameSolverAPI.Requests;
using GameSolverAPI.Responses;
using Microsoft.AspNetCore.Mvc;
using CommandNode = GameSolver.Solver.ShortestCommand.CommandNode;

namespace GameSolverAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MapAnalyzeController : ControllerBase
{
    [HttpPost]
    public ActionResult<MapAnalyzeResponse> MapAnalyze(MapAnalyzeRequest request)
    {
        var game = GameBuilder.CreateFromStandardBoardFormat(request.Tile);

        var bfsSolver = new BreadthFirstSearch(game);
        var solver = new ShortestCommandSolver(game, 15);
        
        try
        {
            IReadOnlyList<IGameAction> testResult = bfsSolver.Solve();
            if (testResult.Count == 0)
            {
                return NoContent();
            }
            
            CommandNode? result = solver.Solve();
            if (result is null)
            {
                return NoContent();
            }

            var commandNodes = new List<Models.CommandNode>();
            var commandEdges = new List<CommandEdge>();
            
            IReadOnlyList<CommandNode> nodes = result.AllNodes();
            var invertedIndexLookup = new Dictionary<CommandNode, int>();

            for (int i = 0; i < nodes.Count; i++)
            {
                commandNodes.Add(new Models.CommandNode
                {
                    Index = i,
                    Type = GetTypeFromCommandNode(nodes[i])
                });
                
                invertedIndexLookup.Add(nodes[i], i);
            }

            for (int i = 0; i < commandNodes.Count; i++)
            {
                int sourceNodeIndex = i;
                int destinationNodeIndex;
                CommandNode currentNode = nodes[i];

                if (currentNode.IsConditionalNode)
                {
                    if (currentNode.ConditionalBranch is not null)
                    {
                        destinationNodeIndex = invertedIndexLookup[currentNode.ConditionalBranch];
                        commandEdges.Add(new CommandEdge
                        {
                            SourceNodeIndex = sourceNodeIndex,
                            DestinationNodeIndex = destinationNodeIndex,
                            Type = CommandEdgeType.ConditionalBranch
                        });
                    }
                }

                if (currentNode.MainBranch is not null)
                {
                    destinationNodeIndex = invertedIndexLookup[currentNode.MainBranch];
                    commandEdges.Add(new CommandEdge
                    {
                        SourceNodeIndex = sourceNodeIndex,
                        DestinationNodeIndex = destinationNodeIndex,
                        Type = CommandEdgeType.MainBranch
                    });
                }
            }

            var testState = new State(game);
            RunCommandResult runResult = testState.RunCommand(result);
            
            int numberOfCommand = result.Count();
            int numberOfAction = runResult.ActionHistory.Count;
            int leastSolvableCommandGold = numberOfCommand;
            int leastSolvableCommandSilver = (int) Math.Ceiling(1.25 * numberOfCommand);
            int leastSolvableCommandBronze = (int) Math.Ceiling(1.25 * 1.25 * numberOfCommand);
            int leastSolvableActionGold = numberOfAction;
            int leastSolvableActionSilver = (int) Math.Ceiling(1.25 * numberOfAction);
            int leastSolvableActionBronze = (int) Math.Ceiling(1.25 * 1.25 * numberOfAction);

            return new MapAnalyzeResponse
            {
                CommandNodes = commandNodes,
                CommandEdges = commandEdges,
                LeastSolvableCommandGold = leastSolvableCommandGold,
                LeastSolvableCommandSilver = leastSolvableCommandSilver,
                LeastSolvableCommandBronze = leastSolvableCommandBronze,
                LeastSolvableActionGold = leastSolvableActionGold,
                LeastSolvableActionSilver = leastSolvableActionSilver,
                LeastSolvableActionBronze = leastSolvableActionBronze
            };
        }
        catch (TimeoutException)
        {
            return StatusCode(StatusCodes.Status504GatewayTimeout);
        }
    }

    private static CommandNodeType GetTypeFromCommandNode(CommandNode commandNode)
    {
        if (commandNode.IsConditionalNode)
        {
            switch (commandNode.ConditionalType)
            {
                case ConditionalType.ConditionalA:
                    return CommandNodeType.ConditionalA;
                case ConditionalType.ConditionalB:
                    return CommandNodeType.ConditionalB;
                case ConditionalType.ConditionalC:
                    return CommandNodeType.ConditionalC;
                case ConditionalType.ConditionalD:
                    return CommandNodeType.ConditionalD;
                case ConditionalType.ConditionalE:
                    return CommandNodeType.ConditionalE;
            }
        }
        else
        {
            IGameAction action = commandNode.Action;
            string? actionString = action.ToString();
            if (actionString is not null)
            {
                switch (actionString)
                {
                    case "u":
                        return CommandNodeType.Forward;
                    case "l":
                        return CommandNodeType.Left;
                    case "d":
                        return CommandNodeType.Back;
                    case "r":
                        return CommandNodeType.Right;
                    case "s":
                        return CommandNodeType.Start;
                }
            }
        }

        throw new ArgumentOutOfRangeException(nameof(commandNode), commandNode, "command node action is not match any case");
    }
}