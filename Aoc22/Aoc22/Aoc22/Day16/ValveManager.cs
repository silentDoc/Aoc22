using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Aoc22.Day16
{
    class Valve
    {
        public string name;
        public int flowRate;
        public HashSet<string> connections; // Hashset is a list with no duplicate members, faster than list

        public Dictionary<string, int> shortestpath = new Dictionary<string, int>();

        public Valve(string name, int flowRate, string conns)
        { 
            this.name= name;
            this.flowRate = flowRate;
            connections = new();
            if (!string.IsNullOrEmpty(conns))
                AddConnections(conns);
        }

        public void AddConnections(string conns)
        {
            var split = conns.Split(",", StringSplitOptions.RemoveEmptyEntries);
            foreach (var connection in split)
                connections.Add(connection.Trim());
        }
    }


    internal class ValveManager
    {
        Dictionary<string, Valve> valves = new();
        int maxPressure = 0;

        public void ParseInput(List<string> input, int part = 1)
        {
            string pattern = @"Valve (\w+) has flow rate=([0-9]\d*); (tunnels lead to valves |tunnel leads to valve )(.*)";
            Regex regex = new Regex(pattern);
            
            foreach (var line in input)
            {
                var groups = regex.Match(line).Groups;
                var name = groups[1].Value;
                var flowrate = int.Parse(groups[2].Value);
                var conns = groups[4].Value;

                valves[name] = new Valve(name, flowrate, conns);
            }

            // Calculate the shortest path from each valve to all the others using BFS algorithm
            foreach (var v in valves.Values)
            {
                v.shortestpath[v.name] = 0;
                ShortestPathToTarget(valves, v, v.name);
            }
        }

        public int SolvePart1()
        {
            List<Valve> valvesToConsider = valves.Values.Where(x => x.flowRate > 0).ToList();

            return ComputeMaxReleasedPressure(30, valvesToConsider, "AA");
        }

        void ShortestPathToTarget(Dictionary<string, Valve> valves, Valve? current, string target)
        {
            var visited = new HashSet<string>();    

            while (current != null && visited.Count < valves.Count)
            {
                visited.Add(current.name);
                int distance = current.shortestpath[target] + 1;
                foreach (var t in current.connections)
                    if (!visited.Contains(t))
                    {
                        var c = valves[t];
                        if (c.shortestpath.ContainsKey(target))
                        {
                            if (distance < c.shortestpath[target]) c.shortestpath[target] = distance;
                        }
                        else c.shortestpath[target] = distance;
                    }

                current = valves.Values.Where(c => !visited.Contains(c.name) && 
                                              c.shortestpath.ContainsKey(target))
                                        .OrderBy(c => c.shortestpath[target])
                                        .FirstOrDefault();
            }
        }

        int ComputeMaxReleasedPressure(int timeRemaining, List<Valve> valvesWithFlow, string currentValve)
        {
            int maxGain = 0;
            var current = valves[currentValve];
            foreach (var valve in valvesWithFlow)
            {
                int newTimeRemaining = timeRemaining - current.shortestpath[valve.name] - 1;
                if (newTimeRemaining > 0)
                {
                    var remainingValves = valvesWithFlow.Where(v => v.name != valve.name).ToList();
                    int gain = newTimeRemaining * valve.flowRate + ComputeMaxReleasedPressure(newTimeRemaining, remainingValves, valve.name);
                    if (maxGain < gain) maxGain = gain;
                }
            }
            return maxGain;
        }

    }
}
