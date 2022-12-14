using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoc22
{
    record struct Coord
    {
        public int x;
        public int y;
    }

    internal class SandCaveMap
    {
        List<char[]> actualMap;
        int max_x;
        int min_x;
        int max_y;
        int min_y;
        int rangeX;

        public SandCaveMap(List<List<Coord>> rockTrails, bool withFloor = false)
        {
            // Find Max and min Coords
            max_x = rockTrails.Select(x => x.Max(g => g.x)).Max();
            max_y = rockTrails.Select(x => x.Max(g => g.y)).Max();
            min_x = rockTrails.Select(x => x.Min(g => g.x)).Min();
            min_y = 0;

            if (withFloor)
            {
                max_y += 2;
                int maxWide = (max_y * 2 + 10) / 2;
                int centralPoint = min_x + (max_x - min_x) / 2;
                min_x = centralPoint - maxWide;
                max_x = centralPoint + maxWide;
            }


            rangeX = max_x - min_x +1;

            actualMap = new();
            for (int i = 0; i <= max_y; i++)
                actualMap.Add(new char[rangeX]);

            foreach (var line in actualMap)
                for (int i = 0; i < line.Length; i++)
                    line[i] = '.';

            foreach (var trail in rockTrails)
                SetTrail(trail);

            if (withFloor)
                for (int i = min_x; i <= max_x; i++)
                    SetRock(new Coord() { x = i, y = max_y });
        }

        public SandCaveMap(SandCaveMap otherSandCaveMap)
        {
            // Find Max and min Coords
            max_x = otherSandCaveMap.max_x;
            max_y = otherSandCaveMap.max_y;
            min_x = otherSandCaveMap.min_x;
            min_y = otherSandCaveMap.min_y; 

            rangeX = max_x - min_x + 1;

            actualMap = new();
            for (int i = 0; i <= max_y; i++)
                actualMap.Add(new char[rangeX]);

            foreach (var line in actualMap)
                for (int i = 0; i < line.Length; i++)
                    line[i] = otherSandCaveMap.GetPos(new Coord() {x = i+min_x, y = actualMap.IndexOf(line) });
        }

        void SetTrail(List<Coord> trail)
        {
            for (int n = 0; n < trail.Count - 1; n++)
            {
                var start = trail[n];
                var end = trail[n+1];

                var vertical = (start.x == end.x);
                var horizontal = !vertical;

                if (vertical)
                {
                    var range = (end.y > start.y)
                                ? Enumerable.Range(start.y, end.y - start.y + 1)
                                : Enumerable.Range(end.y, start.y - end.y + 1);

                    foreach (var yy in range)
                        SetRock(new Coord() { x = start.x, y = yy });
                }
                else
                {
                    var range = (end.x > start.x)
                                    ? Enumerable.Range(start.x, end.x - start.x + 1)
                                    : Enumerable.Range(end.x, start.x - end.x + 1);

                    foreach (var xx in range)
                        SetRock(new Coord() { x = xx, y = start.y });
                }
                    
            }
        }

        public char GetPos(Coord position)
            => actualMap[position.y][position.x - min_x];
        void SetRock(Coord position)
            => actualMap[position.y][position.x - min_x] = '#';
        void SetAir(Coord position)
            => actualMap[position.y][position.x - min_x] = '.';
        public void SetSand(Coord position)
            => actualMap[position.y][position.x - min_x] = 'o';
        void SetOrigin(Coord position)
            => actualMap[position.y][position.x - min_x] = '+';

        public bool CanMoveDown(Coord currentPos)
        {
            if (currentPos.y == max_y)
                throw new IndexOutOfRangeException();

            return GetPos(new Coord() { x = currentPos.x, y = currentPos.y + 1 }) == '.';
        }

        public bool CanMoveDownLeft(Coord currentPos)
        {
            if (currentPos.y == max_y)
                throw new IndexOutOfRangeException();
            if (currentPos.x == min_x)
                throw new IndexOutOfRangeException();

            return GetPos(new Coord() { x = currentPos.x - 1, y = currentPos.y + 1 }) == '.';
        }

        public bool CanMoveDownRight(Coord currentPos)
        {
            if (currentPos.y == max_y)
                throw new IndexOutOfRangeException();
            if (currentPos.x == max_x)
                throw new IndexOutOfRangeException();

            return GetPos(new Coord() { x = currentPos.x + 1, y = currentPos.y + 1 }) == '.';
        }

        public void Move(Coord currentPos, Coord nextPos)
        {
            SetSand(nextPos);
            SetAir(currentPos);
        }
        

        public void Log()
            => actualMap.ForEach(x => Console.WriteLine(x));

        public void LogNice()
        {
            int left = Console.CursorLeft;
            int top = Console.CursorTop;
            Console.SetCursorPosition(left, top);
            Log();
        }
    }

    internal class SandCave
    {
        SandCaveMap? sandCave;
        int part;
        List<List<Coord>> RockTrails = new();

        public int ParseInput(List<string> lines, int part = 1)
        {
            foreach(var line in lines)
                RockTrails.Add(ParseLine(line));
            this.part = part;
            sandCave = new(RockTrails , (part == 2));
            sandCave.Log();

            return RockTrails.Count;
        }

        List<Coord> ParseLine(string line)
        {
            var groups = line.Split(" -> ", StringSplitOptions.RemoveEmptyEntries);
            List<Coord> retVal = new();
            foreach (var g in groups)
            {
                var values = g.Split(",", StringSplitOptions.RemoveEmptyEntries);
                retVal.Add(new Coord() { x = int.Parse(values[0]), y = int.Parse(values[1]) });
            }
            return retVal;
        }

        public int Fill()
        {
            bool stop = false;
            Coord startCoord = new Coord() { x = 500, y = 0 };
            Coord currentCoord = new Coord() { x = 500, y = 0 };
            int rest = 0;
            try
            {
                while (true)
                {
                    var nextCoord = currentCoord;
                    if (sandCave.CanMoveDown(currentCoord))
                    {
                        nextCoord.y++;
                        sandCave.Move(currentCoord, nextCoord);
                        currentCoord = nextCoord;
                    }
                    else if (sandCave.CanMoveDownLeft(currentCoord))
                    {
                        nextCoord.y++;
                        nextCoord.x--;
                        sandCave.Move(currentCoord, nextCoord);
                        currentCoord = nextCoord;
                    }
                    else if (sandCave.CanMoveDownRight(currentCoord))
                    {
                        nextCoord.y++;
                        nextCoord.x++;
                        sandCave.Move(currentCoord, nextCoord);
                        currentCoord = nextCoord;
                    }
                    else
                    {
                        // Get back to starting position
                        rest++;
                        currentCoord = startCoord;
                    }
                    //Thread.Sleep(50);
                    //sandCave.LogNice();
                }
            }
            catch (IndexOutOfRangeException ex)
            {
                return rest;
            }
           
        }
    }
}
