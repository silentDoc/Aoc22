using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Aoc22.Day24
{
    enum TileType
    { 
        Wall = 0, 
        Floor = 1, 
        Blizzard = 2
    }

    enum BlizzardDirection
    {
        Up = 0,
        Down = 1,
        Left = 2,
        Right = 3,

        None = 4
    }
    

    class Tile
    {
        public int x;
        public int y;
        public TileType tileType;
        public List<BlizzardDirection> blizzard_list;
        public int num_blizzard
            => blizzard_list.Count;

        public char TileDisplay()
        {
            int numBlizzards = blizzard_list.Count;
            if (numBlizzards > 1)
                return numBlizzards.ToString()[0];

            if (numBlizzards == 1)
            {
                char ret = blizzard_list[0] switch
                {
                    BlizzardDirection.Up => '^',
                    BlizzardDirection.Down => 'v',
                    BlizzardDirection.Left => '<',
                    BlizzardDirection.Right => '>',
                    _ => throw new Exception("Invalid blizard direction")
                };
                return ret;
            }

            return tileType switch
            {
                TileType.Wall => '#',
                TileType.Floor => '.',
                _ => throw new Exception("Invalid tile type : " + tileType.ToString())
            };
        }

        public void AddBlizzard(BlizzardDirection direction)
            => blizzard_list.Add(direction);

        public Tile(int x, int y, TileType type, BlizzardDirection direction)
        {
            blizzard_list = new();

            this.x = x;
            this.y = y;
            tileType = type;

            if (direction != BlizzardDirection.None)
                blizzard_list.Add(direction);
        }
    }

    internal class BlizzardNavigator
    {
        List<Tile> map = new();
        Dictionary<int, List<Tile>> minutes = new();

        public void ParseInput(List<string> input, int part = 1)
        {
            for (int row = 0; row < input.Count; row++)
            {
                var line = input[row].Trim();
                for (int col = 0; col < line.Length; col++)
                {
                    var tile = line[col] switch
                    {
                        '#' => new Tile(col, row, TileType.Wall, BlizzardDirection.None),
                        '.' => new Tile(col, row, TileType.Floor, BlizzardDirection.None),
                        '^' => new Tile(col, row, TileType.Blizzard, BlizzardDirection.Up),
                        'v' => new Tile(col, row, TileType.Blizzard, BlizzardDirection.Down),
                        '<' => new Tile(col, row, TileType.Blizzard, BlizzardDirection.Left),
                        '>' => new Tile(col, row, TileType.Blizzard, BlizzardDirection.Right),
                        _ => throw new Exception("Unrecognized input char")
                    };
                    map.Add(tile);
                }
            }
        }

        List<Tile> MoveBlizzards(List<Tile> currentMap)
        {
            List<Tile> newMap = new();

            int width = currentMap.Max(t => t.x) +1;
            int height = currentMap.Max(t => t.y) +1;

            // Setup new map
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                {
                    TileType type = ((i == 0) || (i == width - 1) || (j == 0) || (j == height - 1)) ? TileType.Wall : TileType.Floor;
                    newMap.Add(new Tile(i, j, type, BlizzardDirection.None));
                }

            var newStart = newMap.Where(t => t.x == 1 && t.y == 0).First();
            var newEnd = newMap.Where(t => t.x == width-2 && t.y == height-1).First();
            newStart.tileType = newEnd.tileType = TileType.Floor;

            // Update blizzards in new map
            foreach (var tile in currentMap)
            {
                if (tile.tileType != TileType.Blizzard)
                    continue;

                int x = tile.x;
                int y = tile.y;

                foreach (var bliz in tile.blizzard_list)
                {
                    var nextTile = bliz switch
                    {
                        BlizzardDirection.Up => (tile.y == 1) ? newMap.Where(t => t.x == tile.x && t.y == height - 2).First() : newMap.Where(t => t.x == tile.x && t.y == tile.y - 1).First(),
                        BlizzardDirection.Down => (tile.y == height - 2) ? newMap.Where(t => t.x == tile.x && t.y == 1).First() : newMap.Where(t => t.x == tile.x && t.y == tile.y + 1).First(),
                        BlizzardDirection.Left => (tile.x == 1) ? newMap.Where(t => t.x == width - 2 && t.y == tile.y).First() : newMap.Where(t => t.x == tile.x - 1 && t.y == tile.y).First(),
                        BlizzardDirection.Right => (tile.x == width - 2) ? newMap.Where(t => t.x == 1 && t.y == tile.y).First() : newMap.Where(t => t.x == tile.x + 1 && t.y == tile.y).First(),
                        _ => throw new Exception("Unsupported blizzar direction")
                    };

                    nextTile.AddBlizzard(bliz);
                    nextTile.tileType = TileType.Blizzard;
                }
            }

            return newMap;
        }

        void AnimateMap(int delay = 250)
        {
            for(int i = 0; i<minutes.Count; i++)
            {
                Console.SetCursorPosition(0, 0);
                ShowMap(minutes[i]);
                if (delay == 0)
                    Console.ReadKey();
                else
                    Thread.Sleep(delay);
            }
        }   // Helpers to see if the maps have been generated correctly
        void ShowMap(List<Tile> map)
        {
            int rows = map.Max(t => t.y);
            for (int j = 0; j <= rows; j++)
            {
                var row = map.Where(t => t.y == j).OrderBy(tt => tt.x).ToList();
                row.ForEach(x => Console.Write(x.TileDisplay()));
                Console.WriteLine("");
            }
        }

        int ShortestPath(Tile start, Tile end, int initial_cost = 0)
        {
            Queue<(Tile pos, int cost)> priorityQueue = new();
            HashSet<(int x, int y, int cost)> visited = new();
            priorityQueue.Enqueue((start, initial_cost));

            while (priorityQueue.Count > 0)
            {
                var item = priorityQueue.Dequeue();
                var tile = item.pos;
                var currentCost = item.cost;

                if (tile.x == end.x && tile.y == end.y)
                    return currentCost;
                                
                var newCost = currentCost + 1;
                var blizzardMap = minutes[newCost % minutes.Count];
                var candidates = blizzardMap.Where(t => (t.x == tile.x - 1 && t.y == tile.y) ||
                                                        (t.x == tile.x + 1 && t.y == tile.y) ||
                                                        (t.x == tile.x && t.y == tile.y - 1) ||
                                                        (t.x == tile.x && t.y == tile.y + 1) ||
                                                        (t.x == tile.x && t.y == tile.y) ).ToList();

                candidates = candidates.Where(t => t.tileType == TileType.Floor).ToList();

                foreach (var candidate in candidates)
                    if (visited.Add((candidate.x, candidate.y, newCost)))
                        priorityQueue.Enqueue((candidate, newCost));
            }

            throw new Exception("Path does not exist");
        }

        int Walk(int part = 1)
        {
            // Prepare all situations ahead
            int width = map.Max(t => t.x)+1;  
            int height = map.Max(t => t.y)+1;

            int widthField = width - 2;  // We do not consider wall rows / cols to find the repeat loop
            int heightField = height -2;

            int repeatLoopLength = Common.MathUtils.LCM(widthField, heightField);   // Every repeatLoopLength minutes, the blizzard positions are the original
            minutes[0] = map;
            var newMap = map;
            Console.WriteLine("1 - Generate calculated blizzard maps");
            for (int i = 1; i < repeatLoopLength; i++)
            {
                newMap = MoveBlizzards(newMap);
                minutes[i] = newMap;
                Console.SetCursorPosition(0, 2);
                Console.WriteLine(i.ToString() + " of " + repeatLoopLength.ToString());
            }
            
            // Now we can implement the path finding - knowing that we have all the possible blizzard configs.
            Console.WriteLine("2 - Finding path");

            var start = map.Where(t => t.x == 1 && t.y == 0).First();
            var end = map.Where(t => t.x == width-2 && t.y == height-1).First();

            if(part == 1)
                return ShortestPath(start, end);

            var cost_1 = ShortestPath(start, end);
            var cost_2 = ShortestPath(end, start, cost_1);
            return ShortestPath(start, end, cost_2);
        }


        public int Solve(int part = 1)
            => Walk(part);

    }
}
