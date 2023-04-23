using System.Text.Json.Serialization;

namespace GameSolverAPI.Models;

public class CommandEdge
{
    public int SourceNodeIndex { get; set; }
    public int DestinationNodeIndex { get; set; }
    public CommandEdgeType Type { get; set; }
}