namespace GameSolver.Game
{
    public class TileManager
    {
        public HashSet<Tile> PassableTiles { get; init; }
        public HashSet<Tile> BlockedTiles { get; init; }

        public TileManager()
        {
            PassableTiles = new HashSet<Tile>();
            BlockedTiles = new HashSet<Tile>();
        }
    }
}
