using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Aoc22.Day22
{
    // This approach is more readable since it prepares the data beforehand and the navigation and stop conditions are much cleaner.
    // It is slower than operating directly on a char[,], though.

    enum TileType
    {
        Wall = 0,
        Floor = 1
    }

    record struct Command
    {
        public char turn;
        public int steps;
    }


    class MazeTile
    {
        public int y;
        public int x;
        public TileType type;

        public MazeTile? Tile_Up;
        public MazeTile? Tile_Down;
        public MazeTile? Tile_Left;
        public MazeTile? Tile_Right;

        public MazeTile(int y, int x, TileType type)
        {
            this.y = y;
            this.x = x;
            this.type = type;
        }
    }

    internal class MonkeyMap
    {
        List<MazeTile> tiles = new();
        List<Command> commands = new();
        string strCommands = "";
        int startSteps = -1;

        void AddTiles(string line, int rowNum)
        {
            for (int i = 0; i < line.Length; i++)
            {
                if (line[i] == ' ')
                    continue;

                tiles.Add(new MazeTile(rowNum, i, line[i] == '#' ? TileType.Wall : TileType.Floor));
            }
        }

        void FindNeighbors(MazeTile tile)
        {
            var row = tiles.Where(t => t.y == tile.y).ToList();
            int maxx = row.Max(t => t.x);
            int minx = row.Min(t => t.x);

            var col = tiles.Where(t => t.x == tile.x).ToList();
            int maxy = col.Max(t => t.y);
            int miny = col.Min(t => t.y);

            tile.Tile_Up = (tile.y > miny) ? col.Where(t => t.y == tile.y - 1).First() : col.Where(t => t.y == maxy).First();
            tile.Tile_Down = (tile.y < maxy) ? col.Where(t => t.y == tile.y + 1).First() : col.Where(t => t.y == miny).First();
            tile.Tile_Right = (tile.x < maxx) ? row.Where(t => t.x == tile.x + 1).First() : row.Where(t => t.x == minx).First();
            tile.Tile_Left = (tile.x > minx) ? row.Where(t => t.x == tile.x - 1).First() : row.Where(t => t.x == maxx).First();
        }

        void ParseCommands(string strInput)
        {
            var strCommands = strInput;
            var i = 0;
            while (char.IsDigit(strCommands[i]))
                i++;
            startSteps = int.Parse(strCommands.Substring(0, i));
            strCommands = strCommands.Substring(i);

            while (!string.IsNullOrEmpty(strCommands))
            {
                char dir = strCommands[0];
                i = 1;
                while (i < strCommands.Length && char.IsDigit(strCommands[i]))
                    i++;
                int num = int.Parse(strCommands.Substring(1, i - 1));
                commands.Add(new Command() { turn = dir, steps = num });
                strCommands = strCommands.Substring(i);
            }
        }

        public void ParseInput(List<string> input, int part = 1)
        {
            ParseCommands(input[input.Count - 1]);

            for (int i = 0; i < input.Count - 2; i++)
                AddTiles(input[i], i);

            foreach (var tile in tiles)
                FindNeighbors(tile);
        }

        int TurnR(int orientation)
            => (orientation + 1) % 4;

        int TurnL(int orientation)
            => (orientation - 1) < 0 ? 3 : orientation - 1;

        MazeTile Advance(MazeTile current, int orientation, int steps)
        {
            MazeTile next = current;
            for (var i = 0; i < steps; i++)
            {
                MazeTile? candidate = orientation switch
                {
                    0 => next.Tile_Right,
                    1 => next.Tile_Down,
                    2 => next.Tile_Left,
                    3 => next.Tile_Up,
                    _ => throw new Exception("Invalid orientation")
                };

                if (candidate == null)
                    throw new Exception("Missing tile");

                if (candidate.type == TileType.Floor)
                    next = candidate;
                else
                    break;
            }
            return next;
        }

        int Walk()
        {
            int orientation = 0;    // Starts at right

            var start_x = tiles.Where(t => t.y == 0).Min(tt => tt.x);
            var startTile = tiles.Where(t => t.y == 0 && t.x == start_x).FirstOrDefault();

            var currentTile = startTile;
            if (currentTile == null)
                throw new Exception("Missing tile");

            currentTile = Advance(currentTile, orientation, startSteps);

            foreach (var command in commands)
            {
                orientation = (command.turn == 'R') ? TurnR(orientation) : TurnL(orientation);
                currentTile = Advance(currentTile, orientation, command.steps);
            }

            int retVal = 1000 * (currentTile.y + 1) + 4 * (currentTile.x + 1) + orientation;

            return retVal;
        }

        public int Solve(int part = 1)
            => Walk();
    }
}
