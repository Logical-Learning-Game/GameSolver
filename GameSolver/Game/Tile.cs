namespace GameSolver.Game
{
    public enum Tile
    {
        Empty = '.',
        Block = 'x',
        Score = '*',
        Player = 'P',
        Goal = 'G',
    }

    public static class Utility
    {
        public static readonly Dictionary<Tile, int> TileValue = new()
        {
            { Tile.Empty, 0 },
            { Tile.Block, 1 },
            { Tile.Score, 2 },
            { Tile.Player, 3 },
            { Tile.Goal, 4 }
        };
    }
}
