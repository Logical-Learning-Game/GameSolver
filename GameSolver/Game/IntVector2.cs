namespace GameSolver.Game
{
    public struct IntVector2
    {
        public int X { get; set; }
        public int Y { get; set; }

        public IntVector2(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override string ToString()
        {
            return $"x: {X}, y: {Y}";
        }
    }
}
