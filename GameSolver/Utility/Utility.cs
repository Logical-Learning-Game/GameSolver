namespace GameSolver.Utility;

public static class Utility
{
    public static void PrintList<T>(IEnumerable<T> list)
    {
        string s = string.Join("", list);
        Console.WriteLine(s);
    } 
}