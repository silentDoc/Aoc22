using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Aoc22.Day19
{
    class Blueprint
    {
        public int id;
        public int ore_robot_cost;
        public int clay_robot_cost;
        public int obsidian_robot_cost_ore;
        public int obsidian_robot_cost_clay;
        public int geode_robot_cost_ore;
        public int geode_robot_cost_obsidian;

        public Blueprint(int id, int ore_robot_cost, int clay_robot_cost, int obsidian_robot_cost_ore, int obsidian_robot_cost_clay, int geode_robot_cost_ore, int geode_robot_cost_obsidian)
        {
            this.id = id;
            this.ore_robot_cost = ore_robot_cost;
            this.clay_robot_cost = clay_robot_cost;
            this.obsidian_robot_cost_ore = obsidian_robot_cost_ore;
            this.obsidian_robot_cost_clay = obsidian_robot_cost_clay;
            this.geode_robot_cost_ore = geode_robot_cost_ore;
            this.geode_robot_cost_obsidian = geode_robot_cost_obsidian;
        }
    }

    record struct BuildOptions
    {
        public int ore_robots;
        public int clay_robots;
        public int obsidian_robots;
        public int geode_robots;
    }

    class Step
    {
        public int minute;
        public int ore_robots;
        public int clay_robots;
        public int obsidian_robots;
        public int geode_robots;

        public int ore;
        public int clay;
        public int obsidian;
        public int geode;


        public Step(int minute, int ore_robots, int clay_robots, int obsidian_robots, int geode_robots, int ore, int clay, int obsidian, int geode)
        {
            this.minute = minute;
            this.ore_robots = ore_robots;
            this.clay_robots = clay_robots;
            this.obsidian_robots = obsidian_robots;
            this.geode_robots = geode_robots;
            this.ore = ore;
            this.clay = clay;
            this.obsidian = obsidian;
            this.geode = geode;
        } 

        public Step(Step s)
        {
            minute = s.minute;
            ore_robots = s.ore_robots;
            clay_robots = s.clay_robots;
            obsidian_robots = s.obsidian_robots;
            geode_robots = s.geode_robots;
            ore = s.ore;
            clay = s.clay;
            obsidian = s.obsidian;
            geode = s.geode;
        }


        public override int GetHashCode()
        {
            return minute * 100000000 + ore_robots * 10000000 + clay_robots * 1000000 + obsidian_robots * 100000 + geode_robots * 10000 + ore * 1000 + clay * 100 + obsidian * 10 + geode;
        }

        public void Collect(int ore, int clay, int obsidian, int geode)
        {
            this.ore += ore;
            this.clay+= clay;
            this.obsidian += obsidian;
            this.geode += geode;
        }

        public void TryBuild(BuildOptions buildOrders, Blueprint bp)
        {
            if ((ore >= buildOrders.geode_robots * bp.geode_robot_cost_ore) &&
               (obsidian >= buildOrders.geode_robots * bp.geode_robot_cost_obsidian))
            {
                geode_robots += buildOrders.geode_robots;
                ore -= buildOrders.geode_robots * bp.geode_robot_cost_ore;
                obsidian -= buildOrders.geode_robots * bp.geode_robot_cost_obsidian;
            }
            if (ore >= buildOrders.ore_robots * bp.ore_robot_cost)
            {
                ore_robots += buildOrders.ore_robots;
                ore -= bp.ore_robot_cost * buildOrders.ore_robots;
            }
            if (ore >= buildOrders.clay_robots * bp.clay_robot_cost)
            {
                clay_robots += buildOrders.clay_robots;
                ore -= bp.clay_robot_cost * buildOrders.clay_robots;
            }
            if ((ore >= buildOrders.obsidian_robots * bp.obsidian_robot_cost_ore) &&
                (clay >= buildOrders.obsidian_robots * bp.obsidian_robot_cost_clay))
            {
                obsidian_robots += buildOrders.obsidian_robots;
                ore -= buildOrders.obsidian_robots * bp.obsidian_robot_cost_ore;
                clay -= buildOrders.obsidian_robots * bp.obsidian_robot_cost_clay;
            }
           
        }

        public BuildOptions WhatCanIBuild(Blueprint bp)
        {
            BuildOptions opts = new BuildOptions();
            opts.ore_robots = ore / bp.ore_robot_cost;
            opts.clay_robots = ore / bp.clay_robot_cost;
            opts.obsidian_robots = (ore / bp.obsidian_robot_cost_ore) < (clay / bp.obsidian_robot_cost_clay) ? (ore / bp.obsidian_robot_cost_ore) : (clay / bp.obsidian_robot_cost_clay);
            opts.geode_robots = (ore / bp.geode_robot_cost_ore) < (obsidian / bp.geode_robot_cost_obsidian) ? (ore / bp.geode_robot_cost_ore) : (obsidian / bp.geode_robot_cost_obsidian);
            return opts;
        }

        public Step DoStep(BuildOptions buildOrders, Blueprint bp)
        {
            Step retVal = new Step(this);
            retVal.minute++;
            retVal.TryBuild(buildOrders, bp);
            retVal.Collect(ore_robots, clay_robots, obsidian_robots, geode_robots);
            return retVal;
        }

    }

    internal class MiningSquad
    {
        List<Blueprint> blueprints = new();

        public void ParseInput(List<string> lines)
        {
            Regex regex = new Regex(@"Blueprint ([0-9]+): Each ore robot costs ([0-9]+) ore. Each clay robot costs ([0-9]+) ore. Each obsidian robot costs ([0-9]+) ore and ([0-9]+) clay. Each geode robot costs ([0-9]+) ore and ([0-9]+) obsidian.");
            foreach (var line in lines)
            {
                var matches = regex.Matches(line)[0].Groups;

                Blueprint bp = new Blueprint(int.Parse(matches[1].Value), int.Parse(matches[2].Value), int.Parse(matches[3].Value), int.Parse(matches[4].Value),
                                             int.Parse(matches[5].Value), int.Parse(matches[6].Value), int.Parse(matches[7].Value));
                blueprints.Add(bp);
            }
        }

        public int Solve(int part = 1)
            => (part == 1) ? FindBestBluePrint(24) : 0;

        int FindBestBluePrint(int numMinutes)
        {
            List<Step> bestSteps = new();
            foreach (var bp in blueprints)
                bestSteps.Add(SimulateBlueprint(bp, numMinutes));

            int result = 0;
            for (int i = 0; i < bestSteps.Count; i++)
            {
                result += bestSteps[i].geode * (i + 1);
            }

            return result;
        }

        Step SimulateBlueprint(Blueprint bp, int numMinutes)
        {
            
            List<Step> activeSteps = new List<Step>();
            activeSteps.Add(new Step(0, 1, 0, 0, 0, 0, 0, 0, 0));
            
            HashSet<int> checker = new();

            for (int minute = 0; minute < numMinutes; minute++)
            {
                List<Step> nextSteps = new();
                foreach (var step in activeSteps)
                {
                    var possibilities = step.WhatCanIBuild(bp);
                    if (possibilities.ore_robots > 0 && possibilities.geode_robots == 0)
                    {
                        var s1 = step.DoStep(new BuildOptions() { ore_robots = 1, clay_robots = 0, obsidian_robots = 0, geode_robots = 0 }, bp);
                        var h1 = s1.GetHashCode();
                        if (!checker.Contains(h1))
                        {
                            nextSteps.Add(s1);
                            checker.Add(h1);
                        }
                    }

                    if (possibilities.clay_robots > 0 && possibilities.geode_robots == 0)
                    {
                        var s2 = step.DoStep(new BuildOptions() { ore_robots = 0, clay_robots = 1, obsidian_robots = 0, geode_robots = 0 }, bp);
                        var h2 = s2.GetHashCode();
                        if (!checker.Contains(h2))
                        {
                            nextSteps.Add(s2);
                            checker.Add(h2);
                        }
                    }

                    if (possibilities.obsidian_robots > 0 && possibilities.geode_robots == 0)
                    {
                        var s3 = step.DoStep(new BuildOptions() { ore_robots = 0, clay_robots = 0, obsidian_robots = 1, geode_robots = 0 }, bp);
                        var h3 = s3.GetHashCode();
                        if (!checker.Contains(h3))
                        {
                            nextSteps.Add(s3);
                            checker.Add(h3);
                        }
                    }
                    
                    if (possibilities.geode_robots > 0)
                    {
                        var s4 = step.DoStep(new BuildOptions() { ore_robots = 0, clay_robots = 0, obsidian_robots = 0, geode_robots = 1 }, bp);
                        var h4 = s4.GetHashCode();
                        if (!checker.Contains(h4))
                        {
                            nextSteps.Add(s4);
                            checker.Add(h4);
                        }
                    }
                    var s5 = step.DoStep(new BuildOptions() { ore_robots = 0, clay_robots = 0, obsidian_robots = 0, geode_robots = 0 }, bp);
                    var h5 = s5.GetHashCode();
                    if (!checker.Contains(h5))
                    {
                        nextSteps.Add(s5);
                        checker.Add(h5);
                    }
                }

                activeSteps.Clear();
                activeSteps.AddRange(nextSteps);
                Trace.WriteLine(minute.ToString() + "-" + activeSteps.Count.ToString());
                nextSteps.Clear();
                checker.Clear();
            }
            var maxGeode = activeSteps.Max(x => x.geode);


            return activeSteps.Where(x => x.geode == maxGeode).First();
        }

    }
}
