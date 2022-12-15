using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Dynamic;
using System.Linq;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Aoc22.Day12
{
    internal class HillClimbingDijkstra
    {
        List<MapPosition> allPositions = new();
        List<string> outputMap = new();

        public int ParseMap(List<string> input)
        {
            var width = input[0].Length;
            var height = input.Count;

            for (var j = 0; j < height; j++)
                for (var i = 0; i < width; i++)
                    allPositions.Add(new MapPosition(i, j, input[j][i]));

            allPositions.ForEach(x => x.Cost = int.MaxValue);
            allPositions.Where(x => x.IsStart).First().Cost = 0;

            input.ForEach(x => outputMap.Add(new string(x)));

            return width * height;
        }

        public int FloodMap()
        {
            List<MapPosition> visited = new();
            List<MapPosition> queue = new();

            // Invert the map
            foreach (var pos in allPositions)
                if (pos.Value != 'S' && pos.Value != 'E')
                {
                    var x = pos.Value;
                    pos.Value = (char)('z' - pos.Value + 'a');
                }

            var startNode = allPositions.Where(x => x.IsStart).First();
            var endNode = allPositions.Where(x => x.IsDestination).First();

            startNode.Value = 'E';
            endNode.Value = 'S';

            startNode = endNode;

            int width = allPositions.Select(p => p.x).Max();
            int height = allPositions.Select(p => p.y).Max();

            for (int j = 0; j <= height; j++)
            {
                StringBuilder sb = new("");
                for (int i = 0; i <= width; i++)
                {
                    sb.Append(allPositions.Where(p => p.x == i && p.y == j).Select(p => p.Value).Single());
                }
                Console.WriteLine(sb.ToString());
            }






            MapPosition? iterNode = null;
            startNode.Previous = null;
            startNode.Cost = 0;
            queue.Add(startNode);


            while (queue.Any())
            {
                queue = queue.OrderBy(x => x.Cost).ToList();
                var node = queue.First();
                queue.Remove(node);

                foreach (var connected in getNeighbors(node))
                {
                    if (visited.Contains(connected))
                        continue;
                    if (node.Cost + 1 < connected.Cost)
                    {
                        connected.Cost = node.Cost + 1;
                        connected.Previous = node;
                        if (!queue.Contains(connected))
                            queue.Add(connected);
                    }
                }
                visited.Add(node);
                if (node.IsDestination)
                {
                    iterNode = node;
                    break;
                }
            }

            return iterNode == null ? -1 : iterNode.Cost;

        }

        public int FindRoute()
        {
            List<MapPosition> visited = new();
            List<MapPosition> queue = new();

            var startNode = allPositions.Where(x => x.IsStart).First();
            MapPosition? iterNode = null;
            startNode.Previous = null;
            queue.Add(startNode);

            while (queue.Any())
            {
                queue = queue.OrderBy(x => x.Cost).ToList();
                var node = queue.First();
                queue.Remove(node);

                foreach (var connected in getNeighbors(node))
                {
                    if (visited.Contains(connected))
                        continue;
                    if (node.Cost + 1 < connected.Cost)
                    {
                        connected.Cost = node.Cost + 1;
                        connected.Previous = node;
                        if (!queue.Contains(connected))
                            queue.Add(connected);
                    }
                }
                visited.Add(node);
                if (node.IsDestination)
                {
                    iterNode = node;
                    break;
                }
            }

            if (iterNode != null)
            {
                return iterNode.Cost - 1;

                /*List<MapPosition> shortestPath = new();
                while (iterNode.Previous != null)
                {
                    shortestPath.Add(iterNode);
                    iterNode = iterNode.Previous;
                }
                shortestPath.Add(iterNode);

                return shortestPath.Count;*/
            }
            return -1;
        }

        List<MapPosition> getNeighbors(MapPosition node)
        {
            var x1 = node.x - 1;
            var x2 = node.x + 1;

            var horList = allPositions.Where(p => p.y == node.y && (p.x == node.x + 1 || p.x == node.x - 1)).Where(x => x.Walkable(node)).ToList();
            var verList = allPositions.Where(p => p.x == node.x && (p.y == node.y + 1 || p.y == node.y - 1)).Where(x => x.Walkable(node)).ToList();

            List<MapPosition> neighbors = new();

            horList.ForEach(x => neighbors.Add(x)); // We copy positions because we will be updating the costs
            verList.ForEach(x => neighbors.Add(x));
            return neighbors;
        }
    }
}
