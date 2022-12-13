using System.ComponentModel.DataAnnotations;
using System.Numerics;

namespace Aoc22
{
    internal class Program
    {
        static void Main()
        {
            int day = 13;
            int part = 1;
            bool test = false;

            string input;
            input = "./Input/day" + day.ToString() + "_1";
            input += (test) ? "_test.txt" : ".txt";

            Console.WriteLine("AoC 2022 - Day {0} , Part {1} - Test Data {2}", day, part, test);
            string strResult;

            strResult = day switch
            {
                1 => Day1(input, part).ToString(),
                2 => Day2(input, part).ToString(),
                3 => Day3(input, part).ToString(),
                4 => Day4(input, part).ToString(),
                5 => Day5(input, part),
                6 => Day6(input, part).ToString(),
                7 => Day7(input, part).ToString(),
                8 => Day8(input, part).ToString(),
                9 => Day9(input, part).ToString(),
                10 => Day10(input, part).ToString(),
                11 => Day11(input, part).ToString(),
                12 => Day12(input, part).ToString(),
                13 => Day13(input, part).ToString(),
                _ => throw new ArgumentException("Wrong day number - unimplemented"),
            };
            Console.WriteLine("Result : {0}", strResult);

            Console.WriteLine("Key to exit");
            Console.ReadLine();
        }

        static int Day1(string input, int part)
        {
            List<Elf> elves = new();

            var lines = File.ReadLines(input).ToList();

            var i = 0;

            Elf currentElf = new();
            while (i < lines.Count)
            {
                if (string.IsNullOrEmpty(lines[i]))
                {
                    elves.Add(currentElf);
                    currentElf = new();
                    i++;
                    continue;
                }

                currentElf.AddCalories(int.Parse(lines[i]));
                i++;
            }

            if (part == 1)
            {
                var maxCal = elves.Max(x => x.GetCaloriesCount());
                return maxCal;
            }
            else
            {
                var top3Cal = elves.OrderByDescending(x => x.GetCaloriesCount()).ToList().Take(3);
                var sumCal = top3Cal.Sum(x => x.GetCaloriesCount());
                return sumCal;
            }


        }

        static int Day2(string input, int part)
        {
            var lines = File.ReadLines(input).ToList();
            List<RockPaperScissor> rounds = new();

            foreach (var line in lines)
            {
                RockPaperScissor round = new(line, part);
                rounds.Add(round);
            }

            var totalScore = rounds.Sum(x => x.Score);
            return totalScore;
        }

        static int Day3(string input, int part)
        {
            var lines = File.ReadLines(input).ToList();

            if (part == 1)
            {
                List<RuckSack> sacks = new();

                foreach (var line in lines)
                {
                    RuckSack sack = new(line, part);
                    sacks.Add(sack);
                }

                return sacks.Sum(x => x.Priority);
            }

            // Part 2 
            List<RuckSackGroup> groups = new();
            RuckSackGroup current = new();
            foreach (var line in lines)
            {
                var count = current.AddSack(line);
                if (count == 3)
                {
                    groups.Add(current);
                    current = new RuckSackGroup();
                }
            }

            return groups.Sum(x => x.GetPriority());
        }

        static int Day4(string input, int part)
        {
            var lines = File.ReadLines(input).ToList();
            List<AssignmentPair> pairs = new();

            foreach (var line in lines)
                pairs.Add(new AssignmentPair(line, part));

            return (part == 1)
                    ? pairs.Where(x => x.FullyContained).Count()
                    : pairs.Where(x => x.Overlap).Count();
        }

        static string Day5(string input, int part)
        {
            var lines = File.ReadLines(input).ToList();
            var movesSeparator = lines.IndexOf("");
            var stackNumbers = movesSeparator - 1;
            var stackNumberStrings = lines[stackNumbers].Split(" ", StringSplitOptions.RemoveEmptyEntries);
            var numberOfStacks = stackNumberStrings.Select(x => int.Parse(x)).Max();

            CrateStacks crates = new(numberOfStacks, part);
            for (int i = stackNumbers - 1; i >= 0; i--)
                crates.AddCrateRow(lines[i]);

            for (int i = movesSeparator + 1; i < lines.Count; i++)
            {
                crates.Move(lines[i], part);
            }

            return crates.Status();
        }

        static int Day6(string input, int part)
        {
            var lines = File.ReadLines(input).ToList();
            var marker = new SignalProcessor(lines[0], part).Marker;

            return marker;
        }

        static int Day7(string input, int part)
        {
            var lines = File.ReadLines(input).ToList();
            TerminalParser term = new();
            term.ParseCommands(lines);

            if (part == 1)
                return term.flatListDir.Where(x => x.size < 100000).Sum(x => x.size);
            else
            {
                var available = term.availableSpace;
                var needed = 30000000 - available;
                var dir = term.flatListDir.Where(x => x.size > needed).OrderBy(x => x.size).FirstOrDefault();
                if (dir == null)
                    throw new Exception("Something went south");
                return dir.size;
            }


        }

        static int Day8(string input, int part)
        {
            var lines = File.ReadLines(input).ToList();
            VisibleTreeGrid grid = new(lines);

            var result = (part == 1)
                            ? grid.HowManyVisibleTrees()
                            : grid.MaxScore();

            return result;
        }

        static int Day9(string input, int part)
        {
            var lines = File.ReadLines(input).ToList();
            RopeBridge rb = new();

            return (part == 1) ? rb.DoMoves(lines)
                               : rb.DoMovesP2(lines);
        }

        static int Day10(string input, int part)
        {
            var lines = File.ReadLines(input).ToList();
            VideoSignalProcessor vsp = new(lines);
            int[] eval = new int[] { 20, 60, 100, 140, 180, 220 };
            int suma = vsp.RunInstructions(eval);

            if (part == 2)
                vsp.DrawCrt();

            return suma;
        }

        static long Day11(string input, int part)
        {
            var lines = File.ReadLines(input).ToList();
            MonkeyGang gang = new();
            int num = gang.SetupGang(lines);
            Console.WriteLine("{0} monkeys in the gang", num);

            return gang.MonkeyBusiness((part == 1) ? 20 : 10000, part);
        }

        static int Day12(string input, int part)
        {
            var lines = File.ReadLines(input).ToList();
            HillClimbing hillClimb = new();
            hillClimb.ParseMap(lines);
            hillClimb.InvertMap();

            return hillClimb.FindRoute();
        }

        static int Day13(string input, int part)
        {
            string[] lines = File.ReadAllLines(input);
            return new SignalOrderChecker().Part1(lines);
        }
    }
}