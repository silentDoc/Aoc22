using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Aoc22.Day15
{
    record struct Coord
    {
        public int x;
        public int y;
    }

   
    class Sensor
    {
        public Coord position;
        public Coord closestBeacon;

        int SensorRange
            => Manhattan(position, closestBeacon);

        public bool WithinRange(Coord p)
            => Manhattan(p, position) <= SensorRange;

        int Manhattan(Coord c1, Coord c2)
            => Math.Abs(c1.x - c2.x) + Math.Abs(c1.y - c2.y);

        public List<Coord> GetOuterLimit()
        {
            List<Coord> retVal = new();
            var limitRange = SensorRange + 1;

            for (int i = 0; i <= limitRange; i++)
            {
                retVal.Add(new Coord() { x = position.x + (limitRange - i), y = position.y + i });
                retVal.Add(new Coord() { x = position.x - (limitRange - i), y = position.y + i });
                retVal.Add(new Coord() { x = position.x + (limitRange - i), y = position.y - i });
                retVal.Add(new Coord() { x = position.x - (limitRange - i), y = position.y - i });
            }
            return retVal;
        }

        public int[]? Coverage(int row)
        {
            var vertDistance = Math.Abs(position.y - row);
            var ourRange = SensorRange;
            var dif = ourRange- vertDistance;

            if (dif < 0)
                return null;
            else
                return new int[] { position.x - dif, position.x + dif };
        }
    }

    internal class BeaconSensor
    {
        List<Coord> beacons = new();
        List<Coord> posSensors = new();
        List<Sensor> sensors = new();
        int minX = 0;
        int minY = 0;
        int maxX = 0;
        int maxY = 0;

        public int ParseInput(List<string> input)
        {
            //                      Sensor at x=2           , y=18          : closest beacon is at x=-2          , y=15
            var regex = new Regex(@"Sensor at x=(-?[0-9]\d*), y=(-?[0-9]\d*): closest beacon is at x=(-?[0-9]\d*), y=(-?[0-9]\d*)");
            foreach (var line in input)
            {
                var g = regex.Match(line).Groups;
                Coord posSensor = new Coord() { x = int.Parse(g[1].Value), y = int.Parse(g[2].Value) };
                Coord beacon = new Coord() { x = int.Parse(g[3].Value), y = int.Parse(g[4].Value) };
                Sensor sensor = new Sensor
                {
                    position = posSensor,
                    closestBeacon = beacon,
                };

                beacons.Add(beacon);
                posSensors.Add(posSensor);
                sensors.Add(sensor);
            }
            beacons = beacons.Distinct().ToList();

            minX = Math.Min(posSensors.Select(p => p.x).Min(), beacons.Select(p => p.x).Min());
            maxX = Math.Max(posSensors.Select(p => p.x).Max(), beacons.Select(p => p.x).Max());

            minY = Math.Min(posSensors.Select(p => p.y).Min(), beacons.Select(p => p.y).Min());
            maxY = Math.Max(posSensors.Select(p => p.y).Max(), beacons.Select(p => p.y).Max());

            return beacons.Count;
        }


        long SolvePart1(int row)
        {
            var coverages = sensors.Select(x => x.Coverage(row)).ToList();
            List<int> coveredPositions = new List<int>();

            foreach (var coverage in coverages)
                if (coverage != null)
                    for(int i= coverage[0]; i<=coverage[1]; i++)
                        coveredPositions.Add(i);

            coveredPositions = coveredPositions.Distinct().ToList();

            var posBeacons = beacons.Where(bb => bb.y == row).Select(b => b.x).ToList();
            var posSensor = posSensors.Where(ss => ss.y == row).Select(s => s.x).ToList();
            
            posBeacons.ForEach(x => coveredPositions.Remove(x));
            posSensor.ForEach(x => coveredPositions.Remove(x));


            return (long) coveredPositions.Distinct().Count();
        }

        long SolvePart2()
        {
            var limit = 4000000;

            List<Coord> positionsToCheck = new();
            
            foreach (var sns in sensors)
            {
                var outerLimit = sns.GetOuterLimit();
                outerLimit = outerLimit.Where(l => l.x >= 0 && l.x <= limit)
                                       .Where(ll => ll.y >= 0 && ll.y <= limit).ToList();
                positionsToCheck.AddRange(outerLimit); 
            }

            Console.WriteLine(positionsToCheck.Count.ToString() + " positions to check");

            Coord c = new Coord();
            foreach(var p in positionsToCheck)
            {
                var outOfRange = sensors.Select(s => !s.WithinRange(p)).Aggregate(true, (acc, val) => acc && val);
                if (outOfRange)
                {
                    c = p;
                    break;
                }
            }

            return calcFrequency(c);
        }

        long calcFrequency(Coord pos)
            => (long) 4000000 * (long)pos.x + (long)pos.y;

        public long Solve(int row, int part = 1)
        {
            return (part == 1) ? SolvePart1(row)
                               : SolvePart2();
        }

    }
}
