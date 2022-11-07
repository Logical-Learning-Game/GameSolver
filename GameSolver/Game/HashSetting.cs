namespace GameSolver.Game
{
    public class HashSetting
    {
        public HashSet<Tile> HashTiles { get; init; }

        public HashSetting()
        {
            HashTiles = new HashSet<Tile>();
        }
    }
}
