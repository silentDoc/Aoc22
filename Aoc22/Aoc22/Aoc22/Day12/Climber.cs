using Aoc22.Day05;
using Aoc22.Day24;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Aoc22.Day12
{
    record struct Coord(int lat, int lon);
    record struct Symbol(char value);
    record struct Elevation(char value);
    record struct Poi(Symbol symbol, Elevation elevation, int distanceFromGoal);

    public class Climber
    {
        ImmutableDictionary<Coord, Symbol>? map;
        
        Symbol startSymbol = new('S');
        Symbol goalSymbol = new('E');
        Elevation lowestElevation = new('a');
        Elevation highestElevation = new('z');

        public object ShortestPath() =>
            SolveMap()
                .Single(poi => poi.symbol == startSymbol)
                .distanceFromGoal;

        public object ClosestFlat() =>
            SolveMap()
                .Where(poi => poi.elevation == lowestElevation)
                .Min(poi => poi.distanceFromGoal);


        public int Solve(int part = 1)
            => (part == 1) ? (int)ShortestPath() : (int)ClosestFlat();


        IEnumerable<Poi> SolveMap()
        {
            if (map == null)
                throw new Exception("Input has not been parsed");

            var goal = map.Keys.Single(point => map[point] == goalSymbol);
            var start = map.Keys.Single(point => map[point] == startSymbol);

            // Starting from goal, find hortest paths for each point of 
            // the map using BFS.

            var poiByCoord = new Dictionary<Coord, Poi>() {
                {goal, new Poi(goalSymbol, GetElevation(goalSymbol), 0)}
            };

            var q = new Queue<Coord>();
            q.Enqueue(goal);
            while (q.Any())
            {
                var thisCoord = q.Dequeue();
                var thisPoi = poiByCoord[thisCoord];

                foreach (var nextCoord in Neighbours(thisCoord).Where(map.ContainsKey))
                {
                    if (poiByCoord.ContainsKey(nextCoord))
                    {
                        continue;
                    }

                    var nextSymbol = map[nextCoord];
                    var nextElevation = GetElevation(nextSymbol);

                    if (thisPoi.elevation.value - nextElevation.value <= 1)
                    {
                        poiByCoord[nextCoord] = new Poi(
                            symbol: nextSymbol,
                            elevation: nextElevation,
                            distanceFromGoal: thisPoi.distanceFromGoal + 1
                        );
                        q.Enqueue(nextCoord);
                    }
                }

            }

            return poiByCoord.Values;
        }

        Elevation GetElevation(Symbol symbol) =>
            symbol.value switch
            {
                'S' => lowestElevation,
                'E' => highestElevation,
                _ => new Elevation(symbol.value)
            };

        // locations are parsed into a dictionary so that valid coordinates and
        // neighbours are easy to deal with
        public void ParseInput(List<string> lines)
        {
             map = ( from y in Enumerable.Range(0, lines.Count - 1)
                      from x in Enumerable.Range(0, lines[0].Length)
                          select new KeyValuePair<Coord, Symbol>(
                                       new Coord(x, y), new Symbol(lines[y][x]))
                   ).ToImmutableDictionary();
        }

        IEnumerable<Coord> Neighbours(Coord coord) =>
            new[] {
               coord with {lat = coord.lat + 1},
               coord with {lat = coord.lat - 1},
               coord with {lon = coord.lon + 1},
               coord with {lon = coord.lon - 1},
            };
    }
}
