using System.Text;
using GameSolver.Solver.ShortestCommand;

namespace GameSolver.Core;

public sealed class Game
{
    // Board
    public int[,] Board { get; set; }

    // Player
    public Vector2Int StartPlayerTile { get; set; }
    public Direction StartPlayerDirection { get; set; }
    public int KeysA { get; set; }
    public int KeysB { get; set; }
    public int KeysC { get; set; }
    public ConditionalType Condition { get; set; }


    // TilePosition
    public IList<Vector2Int> ScoreTiles { get; set; }
    public IList<Vector2Int> KeyTiles { get; set; }
    public IList<Vector2Int> DoorTiles { get; set; }
    public IList<Vector2Int> ConditionalTiles { get; set; }
    public Vector2Int GoalTile { get; set; }

    // HashComponent
    public long[,] HashComponent { get; set; }

    public Game()
    {
        Board = new int[0, 0];
        ScoreTiles = Array.Empty<Vector2Int>();
        KeyTiles = Array.Empty<Vector2Int>();
        DoorTiles = Array.Empty<Vector2Int>();
        ConditionalTiles = Array.Empty<Vector2Int>();
        HashComponent = new long[0, 0];
    }

    public static string BoardToString(int[,] board)
    {
        var strBuilder = new StringBuilder();
        for (int i = 0; i < board.GetLength(0); i++)
        {
            for (int j = 0; j < board.GetLength(1); j++)
            {
                int tile = board[i, j];

                strBuilder.Append(TileToChar(tile));
            }
            strBuilder.AppendLine();
        }
        return strBuilder.ToString();
    }

    public override string ToString()
    {
        return BoardToString(Board);
    }
        
    public static int ToOneDimension(int row, int col, int boardWidth)
    {
        return row * boardWidth + col;
    }

    public static int ToOneDimension(Vector2Int position, int boardWidth)
    {
        return position.Y * boardWidth + position.X;
    }

    public int ToOneDimension(Vector2Int position)
    {
        int boardWidth = Board.GetLength(1);
        return ToOneDimension(position, boardWidth);
    }

    private static char TileToChar(int tile)
    {
        char ch;
        
        if (TileComponent.Player.In(tile))
        {
            ch = 'P';
        }
        else if (TileComponent.Goal.In(tile))
        {
            ch = 'G';
        }
        else if (TileComponent.Score.In(tile))
        {
            ch = '*';
        }
        else if (TileComponent.KeyA.In(tile))
        {
            ch = '1';
        }
        else if (TileComponent.KeyB.In(tile))
        {
            ch = '2';
        }
        else if (TileComponent.KeyC.In(tile))
        {
            ch = '3';
        }
        else if (TileComponent.Wall.In(tile))
        {
            ch = 'x';
        }
        else if (TileComponent.ConditionalA.In(tile))
        {
            ch = 'a';
        }
        else if (TileComponent.ConditionalB.In(tile))
        {
            ch = 'b';
        }
        else if (TileComponent.ConditionalC.In(tile))
        {
            ch = 'c';
        }
        else if (TileComponent.ConditionalD.In(tile))
        {
            ch = 'd';
        }
        else if (TileComponent.ConditionalE.In(tile))
        {
            ch = 'e';
        }
        else if (TileComponent.HaveDoorLink(tile))
        {
            ch = 'D';
        }
        else
        {
            ch = '.';
        }

        return ch;
    }
}