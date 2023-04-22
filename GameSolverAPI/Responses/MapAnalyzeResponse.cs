using GameSolverAPI.Models;

namespace GameSolverAPI.Responses;

public class MapAnalyzeResponse
{
    public List<CommandNode> CommandNodes { get; set; }
    
    public List<CommandEdge> CommandEdges { get; set; }
    
    public int LeastSolvableCommandGold { get; set; }
    
    public int LeastSolvableCommandSilver { get; set; }
    
    public int LeastSolvableCommandBronze { get; set; }
    
    public int LeastSolvableActionGold { get; set; }
    
    public int LeastSolvableActionSilver { get; set; }
    
    public int LeastSolvableActionBronze { get; set; }
}