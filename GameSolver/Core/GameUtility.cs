namespace GameSolver.Core;

public static class GameUtility
{
    public static bool OutOfBoundCheck(int[,] board, int x, int y)
    {
        int height = board.GetLength(0);
        int width = board.GetLength(1);
        return y < 0 || x < 0 || y > height - 1 || x > width - 1;
    }
}