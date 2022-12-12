using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoc22
{
    class MapPosition
    {
        public int x, y;
        public Char Value;
        private int Distance;
        public int Cost;
        public MapPosition? Previous;
        
        public bool IsDestination => Value == 'E';
        public bool IsStart => Value == 'S';
        public int CostDistance => Cost + Distance;

        public MapPosition(int x, int y, char value, MapPosition? previous = null)
        {
            this.x = x;
            this.y = y;
            Cost = 0;
            Distance = 0;
            this.Value = value;
            Previous = previous;
        }

        public MapPosition(MapPosition mapPosition)
        {
            this.x = mapPosition.x;
            this.y = mapPosition.y;
            this.Cost = mapPosition.Cost;
            this.Distance = mapPosition.Distance;
            this.Value = mapPosition.Value;
            this.Previous = mapPosition.Previous;
        }

        public void SetDistance(MapPosition target)
            => Distance = Math.Abs(target.x - x) + Math.Abs(target.y - y);

        public bool Walkable(MapPosition previous)
            => (previous.Value=='S' && Value=='a') 
               || (previous.Value=='z' && Value == 'E')
               || (Value<= previous.Value+1);

        public bool WalkableSafe(MapPosition previous)
        {
            var c1 = (previous.Value == 'S' && Value == 'a');
            var c2 = (previous.Value == 'z' && Value == 'E');
            var c3 = (Value!='E') && (Value <= previous.Value + 1);

            var crit = c1 || c2 || c3;
            return crit;
        }
    }

    internal class HillClimbing
    {
        List<MapPosition> visitedPositions = new();
        List<MapPosition> allPositions = new();
        List<MapPosition> activePositions = new();

        public int ParseMap(List<string> input)
        { 
            var width = input[0].Length;
            var height = input.Count;

            for (var j = 0; j < height; j++)
                for (var i = 0; i < width; i++)
                    allPositions.Add(new MapPosition(i, j, input[j][i]));

            return width * height;
        }

        List<MapPosition> getWalkable(MapPosition currentPosition, MapPosition destination)
        {
            var horList = allPositions.Where(p => (p.y == currentPosition.y) && ((p.x == currentPosition.x + 1) || (p.x == currentPosition.x - 1))).Where(x => x.WalkableSafe(currentPosition)).ToList();
            var verList = allPositions.Where(p => (p.x == currentPosition.x) && ((p.y == currentPosition.y + 1) || (p.y == currentPosition.y - 1))).Where(x => x.WalkableSafe(currentPosition)).ToList();

            List<MapPosition> walkable = new();

            horList.ForEach(x => walkable.Add(new MapPosition(x))); // We copy positions because we will be updating the costs
            verList.ForEach(x => walkable.Add(new MapPosition(x)));

            walkable.ForEach(x => x.Previous = currentPosition);
            walkable.ForEach(x => x.Cost = currentPosition.Cost+1);
            walkable.ForEach(x => x.SetDistance(destination));
            return walkable;
        }

        public int FindRoute()
        {
            var start = allPositions.Where(x => x.IsStart).First();
            var destination = allPositions.Where(x => x.IsDestination).First();

            List<MapPosition> path = new();
            bool pathFound = false;

            allPositions.ForEach(x => x.SetDistance(destination));
            activePositions.Add(start);

            while (activePositions.Any())
            {
                var check = activePositions.OrderBy(x => x.CostDistance).First();
                
                if (check.IsDestination)
                {
                    pathFound = true;
                    var currentPathPosition = check;

                    while (currentPathPosition.Previous is not null)
                    {
                        path.Add(new MapPosition(currentPathPosition));
                        currentPathPosition = currentPathPosition.Previous;
                    }
                    path.Add(new MapPosition(currentPathPosition));
                    break;
                }

                visitedPositions.Add(check);
                activePositions.Remove(check);

                var walkablePositions = getWalkable(check, destination);

                foreach (var walk in walkablePositions)
                {
                    if (visitedPositions.Any(p => p.x == walk.x && p.y == walk.y))
                        continue;

                    var existingPosition = activePositions.Where(p => p.x == walk.x && p.y == walk.y).FirstOrDefault();


                    if (existingPosition != null)
                    {
                        if (existingPosition.CostDistance > walk.CostDistance)
                        {
                            activePositions.Remove(existingPosition);
                            activePositions.Add(walk);
                        }
                    }
                    else
                        activePositions.Add(walk);
                }
            }
            if (pathFound)
                path.ForEach(x => Console.WriteLine("{0} , {1}", x.x, x.y));
            
            return (pathFound) ? path.Count-1 : -1;
        }
    }
}
