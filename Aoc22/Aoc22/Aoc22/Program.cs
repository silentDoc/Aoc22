using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Numerics;


namespace Aoc22
{
    internal class Program
    {
        static void Main()
        {
            int day = 22;
            int part = 1;
            bool test = false;   // True - test input, False = Real input

            string input;
            input = "./Input/day" + day.ToString() + "_1";
            input += (test) ? "_test.txt" : ".txt";

            Console.WriteLine("AoC 2022 - Day {0} , Part {1} - Test Data {2}", day, part, test);

            var strResult = day switch
            {
                1 => day1(input, part).ToString(),
                2 => day2(input, part).ToString(),
                3 => day3(input, part).ToString(),
                4 => day4(input, part).ToString(),
                5 => day5(input, part).ToString(),
                6 => day6(input, part).ToString(),
                7 => day7(input, part).ToString(),
                8 => day8(input, part).ToString(),
                9 => day9(input, part).ToString(),
                10 => day10(input, part).ToString(),
                11 => day11(input, part).ToString(),
                12 => day12(input, part).ToString(),
                13 => day13(input, part).ToString(),
                14 => day14(input, part).ToString(),
                15 => day15(input, part).ToString(),
                16 => day16(input, part).ToString(),
                17 => day17(input, part).ToString(),
                18 => day18(input, part).ToString(),
                19 => day19(input, part).ToString(),
                20 => day20(input, part).ToString(),
                21 => day21(input, part).ToString(),
                22 => day22(input, part).ToString(),
                _ => throw new ArgumentException("Wrong day number - unimplemented"),
            };
            Console.WriteLine("Result : {0}", strResult);
            Console.WriteLine("Key to exit");
            Console.ReadLine();
        }

        static int day1(string input, int part)
        {
            var lines = File.ReadLines(input).ToList();
            Day01.ElvenGroup group = new();
            group.ParseInput(lines);
            return group.Solve(part);
        }

        static int day2(string input, int part)
        {
            var lines = File.ReadLines(input).ToList();
            List<Day02.RockPaperScissor> rounds = new();

            foreach (var line in lines)
            {
                Day02.RockPaperScissor round = new(line, part);
                rounds.Add(round);
            }

            var totalScore = rounds.Sum(x => x.Score);
            return totalScore;
        }

        static int day3(string input, int part)
        {
            var lines = File.ReadLines(input).ToList();
            return new Day03.RuckSackSolver().Solve(lines, part);
        }

        static int day4(string input, int part)
        {
            var lines = File.ReadLines(input).ToList();
            List<Day04.AssignmentPair> pairs = new();

            foreach (var line in lines)
                pairs.Add(new Day04.AssignmentPair(line, part));

            return (part == 1)
                    ? pairs.Where(x => x.FullyContained).Count()
                    : pairs.Where(x => x.Overlap).Count();
        }

        static string day5(string input, int part)
        {
            var lines = File.ReadLines(input).ToList();
            return new Day05.CrateStackSolver().Solve(lines, part);
        }

        static int day6(string input, int part)
        {
            var lines = File.ReadLines(input).ToList();
            return new Day06.SignalProcessor(lines[0], part).Marker;
        }

        static int day7(string input, int part)
        {
            var lines = File.ReadLines(input).ToList();
            Day07.TerminalParser term = new();
            return term.Solve(lines, part);
        }

        static int day8(string input, int part)
        {
            var lines = File.ReadLines(input).ToList();
            Day08.VisibleTreeGrid grid = new(lines);

            return (part == 1)
                            ? grid.HowManyVisibleTrees()
                            : grid.MaxScore();

        }

        static int day9(string input, int part)
        {
            var lines = File.ReadLines(input).ToList();
            Day09.RopeBridge rb = new();

            return (part == 1) ? rb.DoMoves(lines)
                               : rb.DoMovesP2(lines);
        }

        static int day10(string input, int part)
        {
            var lines = File.ReadLines(input).ToList();
            Day10.VideoSignalProcessor vsp = new(lines);
            int[] eval = new int[] { 20, 60, 100, 140, 180, 220 };
            int suma = vsp.RunInstructions(eval);

            if (part == 2)
                vsp.DrawCrt();

            return suma;
        }

        static long day11(string input, int part)
        {
            var lines = File.ReadLines(input).ToList();
            Day11.MonkeyGang gang = new();
            int num = gang.SetupGang(lines);
            return gang.MonkeyBusiness((part == 1) ? 20 : 10000, part);
        }

        static int day12(string input, int part)
        {
            var lines = File.ReadLines(input).ToList();
            Day12.HillClimbing hillClimb = new();
            hillClimb.ParseMap(lines);
            hillClimb.InvertMap();

            return hillClimb.FindRoute();
        }

        static int day13(string input, int part)
        {
            string[] lines = File.ReadAllLines(input);
            return (part==1) ? new Day13.SignalOrderChecker().Part1(lines)
                             : new Day13.SignalOrderChecker().Part2(lines);
        }

        static int day14(string input, int part)
        {
            List<string> lines = File.ReadAllLines(input).ToList();
            Day14.SandCave sc = new();
            sc.ParseInput(lines, part);
            return sc.Fill(); 
        }

        static long day15(string input, int part)
        {
            List<string> lines = File.ReadAllLines(input).ToList();
            Day15.BeaconSensor bs = new();
            bs.ParseInput(lines);
            return bs.Solve(2000000, part);
        }

        static int day16(string input, int part)
        {
            List<string> lines = File.ReadAllLines(input).ToList();
            Day16.ValveManager vm = new();
            vm.ParseInput(lines);

            return vm.Solve(part);
        }

        static long day17(string input, int part)
        {
            List<string> lines = File.ReadAllLines(input).ToList();
            Day17.Tetris tetris = new();
            tetris.ParseInput(lines);
            
            return tetris.Solve(part);
        }

        static int day18(string input, int part)
        {
            List<string> lines = File.ReadAllLines(input).ToList();
            Day18.SurfaceExplorer surfExplorer = new();
            surfExplorer.ParseInput(lines);

            return surfExplorer.Solve(part);
        }

        static int day19(string input, int part)
        {
            List<string> lines = File.ReadAllLines(input).ToList();
            Day19.MiningSquad squad = new();
            squad.ParseInput(lines);
            return squad.Solve(part);
        }

        static long day20(string input, int part)
        {
            List<string> lines = File.ReadAllLines(input).ToList();
            Day20.Mixer mixer = new();
            mixer.ParseInput(lines);
            //return mixer.CheckRepeats();
            return mixer.Solve(part);
        }

        static long day21(string input, int part)
        {
            List<string> lines = File.ReadAllLines(input).ToList();
            Day21.MonkeyMath monkeyMath = new();
            monkeyMath.ParseInput(lines);
            
            return monkeyMath.Solve(part);
        }

        static int day22(string input, int part)
        {
            List<string> lines = File.ReadAllLines(input).ToList();
            Day22.MonkeyMap monkeyMap = new();
            monkeyMap.ParseInput(lines);

            return monkeyMap.Solve();
        }
    }
}