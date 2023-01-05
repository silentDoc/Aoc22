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
        public bool ore_robots;
        public bool clay_robots;
        public bool obsidian_robots;
        public bool geode_robots;
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
            return geode_robots * 100000000 + ore_robots * 10000000 + clay_robots * 1000000 + obsidian_robots * 100000 + minute * 10000 + ore * 1000 + clay * 100 + obsidian * 10 + geode;
        }

        public void Collect(int ore, int clay, int obsidian, int geode)
        {
            this.ore += ore;
            this.clay+= clay;
            this.obsidian += obsidian;
            this.geode += geode;
        }

        public void BuildRobot(BuildOptions buildOrders, Blueprint bp)
        {
            if (buildOrders.geode_robots)
            {
                geode_robots++;
                ore -= bp.geode_robot_cost_ore;
                obsidian -= bp.geode_robot_cost_obsidian;
            }
            if (buildOrders.ore_robots)
            {
                ore_robots++;
                ore -= bp.ore_robot_cost;
            }
            if (buildOrders.clay_robots)
            {
                clay_robots++;
                ore -= bp.clay_robot_cost;
            }
            if (buildOrders.obsidian_robots)
            {
                obsidian_robots++;
                ore -=  bp.obsidian_robot_cost_ore;
                clay -= bp.obsidian_robot_cost_clay;
            }
        }

        public BuildOptions WhatCanIBuild(Blueprint bp)
        {
            BuildOptions opts = new BuildOptions();
            opts.ore_robots = ore >= bp.ore_robot_cost;
            opts.clay_robots = ore >= bp.clay_robot_cost;
            opts.obsidian_robots = (ore >= bp.obsidian_robot_cost_ore) && (clay >= bp.obsidian_robot_cost_clay);
            opts.geode_robots = (ore >= bp.geode_robot_cost_ore) && (obsidian >= bp.geode_robot_cost_obsidian);
            return opts;
        }
       
        public Step DoStep(BuildOptions buildOrders, Blueprint bp)
        {
            Step nextStep = new Step(this);
            nextStep.minute++;
            nextStep.BuildRobot(buildOrders, bp);
            nextStep.Collect(ore_robots, clay_robots, obsidian_robots, geode_robots);
            return nextStep;
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
            => FindBestBluePrint((part == 1) ? 24 : 32, part);

        int FindBestBluePrint(int numMinutes, int part=1)
        {
            List<Step> bestSteps = new();

            if (part == 2)
            {
                Parallel.For(0, 3, x => bestSteps.Add(SimulateBlueprint(blueprints[x], numMinutes)));
                return bestSteps.Select(x => x.geode).Aggregate(1, (acc, val) => acc * val);
            }

            // Part == 1
            blueprints.ForEach(x => bestSteps.Add(SimulateBlueprint(x, numMinutes)));
            return bestSteps.Select((x, index) => x.geode * (index+1)).Sum();
        }

        Step SimulateBlueprint(Blueprint bp, int numMinutes)
        {
            List<Step> activeSteps = new List<Step>();
            activeSteps.Add(new Step(0, 1, 0, 0, 0, 0, 0, 0, 0));
            HashSet<int> checker = new();   // Hashshet comparison is much faster than List<>.Contains, specially with ints

            for (int minute = 0; minute < numMinutes; minute++)
            {
                List<Step> nextSteps = new();
                foreach (var step in activeSteps)
                {
                    // The only optimization applied -- whenever we can build a geode robot, we go for it and ignore any other state. That cuts time by a lot, specially when some minutes have passed.
                    var possibilities = step.WhatCanIBuild(bp);
                    if (possibilities.ore_robots && !possibilities.geode_robots)
                    {
                        var nextStep1 = step.DoStep(new BuildOptions() { ore_robots = true, clay_robots = false, obsidian_robots = false, geode_robots = false }, bp);
                        var h1 = nextStep1.GetHashCode();
                        if (!checker.Contains(h1))
                        {
                            nextSteps.Add(nextStep1);
                            checker.Add(h1);
                        }
                    }

                    if (possibilities.clay_robots && !possibilities.geode_robots)
                    {
                        var nextStep2 = step.DoStep(new BuildOptions() { ore_robots = false, clay_robots = true, obsidian_robots = false, geode_robots = false }, bp);
                        var h2 = nextStep2.GetHashCode();
                        if (!checker.Contains(h2))
                        {
                            nextSteps.Add(nextStep2);
                            checker.Add(h2);
                        }
                    }

                    if (possibilities.obsidian_robots && !possibilities.geode_robots)
                    {
                        var nextStep3 = step.DoStep(new BuildOptions() { ore_robots = false, clay_robots = false, obsidian_robots = true, geode_robots = false }, bp);
                        var h3 = nextStep3.GetHashCode();
                        if (!checker.Contains(h3))
                        {
                            nextSteps.Add(nextStep3);
                            checker.Add(h3);
                        }
                    }

                    if (possibilities.geode_robots)
                    {
                        var nextStep4 = step.DoStep(new BuildOptions() { ore_robots = false, clay_robots = false, obsidian_robots = false, geode_robots = true }, bp);
                        var h4 = nextStep4.GetHashCode();
                        if (!checker.Contains(h4))
                        {
                            nextSteps.Add(nextStep4);
                            checker.Add(h4);
                        }
                    }
                    var nextStep5 = step.DoStep(new BuildOptions() { ore_robots = false, clay_robots = false, obsidian_robots = false, geode_robots = false }, bp);
                    var h5 = nextStep5.GetHashCode();
                    if (!checker.Contains(h5))
                    {
                        nextSteps.Add(nextStep5);
                        checker.Add(h5);
                    }
                }

                activeSteps.Clear();
                activeSteps.AddRange(nextSteps);
                nextSteps.Clear();
                checker.Clear();
            }
            var maxGeode = activeSteps.Max(x => x.geode);
            return activeSteps.Where(x => x.geode == maxGeode).First();
        }

    }
}
