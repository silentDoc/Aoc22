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
        public int fy;  // Face relative position, will help to solve the wraps
        public int fx;
        public TileType type;
        public int Face
            => (x / 50, y / 50) switch
            {
                (1, 0) => 1,
                (2, 0) => 2,
                (1, 1) => 3,
                (0, 2) => 4,
                (1, 2) => 5,
                (0, 3) => 6,
                _ => throw new Exception("Cannot resolve face")
            };


        public MazeTile? Tile_Up;
        public MazeTile? Tile_Down;
        public MazeTile? Tile_Left;
        public MazeTile? Tile_Right;

        public MazeTile(int y, int x, TileType type)
        {
            this.y = y;
            this.x = x;
            fx = x % 50;    // Face relative coordinates, useful to locate wrap continuations
            fy = y % 50;
            this.type = type;
        }
    }

    internal class MonkeyMap
    {
        List<MazeTile> tiles = new();
        List<Command> commands = new();
        int startSteps = -1;

        const int orientation_right = 0;
        const int orientation_down  = 1;
        const int orientation_left  = 2;
        const int orientation_up    = 3;


        MazeTile SolveWrap(MazeTile tile, int orientation)  // I just hardcode the folding of the problem data
            => (tile.Face, orientation) switch
            {
                (1, orientation_left) => tiles.Where(t => t.fx == 0 && t.fy == 49 - tile.fy && t.Face == 4).First(),
                (1, orientation_up) => tiles.Where(t => t.fx == 0 && t.fy == tile.fx && t.Face == 6).First(),

                (2, orientation_right) => tiles.Where(t => t.fx == 49 && t.fy == 49 - tile.fy && t.Face == 5).First(),
                (2, orientation_down) => tiles.Where(t => t.fx == 49 && t.fy == tile.fx && t.Face == 3).First(),
                (2, orientation_up) => tiles.Where(t => t.fx == tile.fx && t.fy == 49 && t.Face == 6).First(),

                (3, orientation_left) => tiles.Where(t => t.fx == tile.fy && t.fy == 0 && t.Face == 4).First(),
                (3, orientation_right) => tiles.Where(t => t.fx == tile.fy && t.fy == 49 && t.Face == 2).First(),

                (4, orientation_left) => tiles.Where(t => t.fx == 0 && t.fy == 49 - tile.fy && t.Face == 1).First(),
                (4, orientation_up) => tiles.Where(t => t.fx == 0 && t.fy == tile.fx && t.Face == 3).First(),

                (5, orientation_right) => tiles.Where(t => t.fx == 49 && t.fy == 49 - tile.fy && t.Face == 2).First(),
                (5, orientation_down) => tiles.Where(t => t.fx == 49 && t.fy == tile.fx && t.Face == 6).First(), 

                (6, orientation_right) => tiles.Where(t => t.fx == tile.fy && t.fy == 49 && t.Face == 5).First(), 
                (6, orientation_down) => tiles.Where(t => t.fx == tile.fx && t.fy == 0 && t.Face == 2).First(), 
                (6, orientation_left) => tiles.Where(t => t.fx == tile.fy && t.fy == 0 && t.Face == 1).First(), 

                _ => throw new Exception("This combination should not wrap")

            };

        int FindNewOrientation(MazeTile tile, int orientation)  
            => (tile.Face, orientation) switch
            {
                (1, orientation_left) => orientation_right,
                (1, orientation_up) => orientation_right,

                (2, orientation_right) => orientation_left,
                (2, orientation_down) => orientation_left,
                (2, orientation_up) => orientation_up,

                (3, orientation_left) => orientation_down,
                (3, orientation_right) => orientation_up,

                (4, orientation_left) => orientation_right,
                (4, orientation_up) => orientation_right,

                (5, orientation_right) => orientation_left,
                (5, orientation_down) => orientation_left,

                (6, orientation_right) => orientation_up,
                (6, orientation_down) => orientation_down,
                (6, orientation_left) => orientation_down,

                _ => -1 // -1 means that the orientation should NOT change
            };


        void AddTiles(string line, int rowNum)
        {
            for (int i = 0; i < line.Length; i++)
            {
                if (line[i] == ' ')
                    continue;

                tiles.Add(new MazeTile(rowNum, i, line[i] == '#' ? TileType.Wall : TileType.Floor));
            }
        }

        void FindNeighbors(MazeTile tile, int part =1)   
        {
            var row = tiles.Where(t => t.y == tile.y).ToList();
            int maxx = row.Max(t => t.x);
            int minx = row.Min(t => t.x);

            var col = tiles.Where(t => t.x == tile.x).ToList();
            int maxy = col.Max(t => t.y);
            int miny = col.Min(t => t.y);

            if (part == 1)  // Part 1 - looping in the same row/col
            {
                tile.Tile_Up = (tile.y > miny) ? col.Where(t => t.y == tile.y - 1).First() : col.Where(t => t.y == maxy).First();
                tile.Tile_Down = (tile.y < maxy) ? col.Where(t => t.y == tile.y + 1).First() : col.Where(t => t.y == miny).First();
                tile.Tile_Right = (tile.x < maxx) ? row.Where(t => t.x == tile.x + 1).First() : row.Where(t => t.x == minx).First();
                tile.Tile_Left = (tile.x > minx) ? row.Where(t => t.x == tile.x - 1).First() : row.Where(t => t.x == maxx).First();
            }
            else   // Part 2 - Solving the wrapping to find next tile
            {
                tile.Tile_Up = (tile.y > miny) ? col.Where(t => t.y == tile.y - 1).First() : SolveWrap(tile, orientation_up);
                tile.Tile_Down = (tile.y < maxy) ? col.Where(t => t.y == tile.y + 1).First() : SolveWrap(tile, orientation_down);
                tile.Tile_Right = (tile.x < maxx) ? row.Where(t => t.x == tile.x + 1).First() : SolveWrap(tile, orientation_right);
                tile.Tile_Left = (tile.x > minx) ? row.Where(t => t.x == tile.x - 1).First() : SolveWrap(tile, orientation_left);
            }
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
                FindNeighbors(tile, part);
        }

        int TurnR(int orientation)
            => (orientation + 1) % 4;

        int TurnL(int orientation)
            => (orientation - 1) < 0 ? 3 : orientation - 1;

        MazeTile Advance(MazeTile current, int orientation, int steps, out int nextOrientation, int part = 1 )
        {
            MazeTile next = current;
            int currentOrientation = orientation;

            for (var i = 0; i < steps; i++)
            {
                MazeTile? candidate = currentOrientation switch
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
                {
                    
                    if (part == 2 && candidate.Face != next.Face)
                    {
                        var newOrientation = FindNewOrientation(next, currentOrientation);
                        currentOrientation = (newOrientation != -1) ? newOrientation : currentOrientation;
                    }

                    next = candidate;
                }
                else
                    break;
            }
            nextOrientation = currentOrientation;
            return next;
        }

        int Walk(int part)
        {
            int orientation = 0;    // Starts at right
            int nextOrientation = 0;
            var start_x = tiles.Where(t => t.y == 0).Min(tt => tt.x);
            var startTile = tiles.Where(t => t.y == 0 && t.x == start_x).FirstOrDefault();

            var currentTile = startTile;
            if (currentTile == null)
                throw new Exception("Missing tile");

            currentTile = Advance(currentTile, orientation, startSteps, out nextOrientation, part);

            foreach (var command in commands)
            {
                orientation = (command.turn == 'R') ? TurnR(orientation) : TurnL(orientation);
                currentTile = Advance(currentTile, orientation, command.steps, out nextOrientation, part);
                if (nextOrientation != orientation)
                    orientation = nextOrientation;

            }


            int retVal = 1000 * (currentTile.y + 1) + 4 * (currentTile.x + 1) + orientation;
            return retVal;
        }

        public int Solve(int part = 1)
            => Walk(part);
    }
}
