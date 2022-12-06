using System.ComponentModel.DataAnnotations;

namespace Aoc22
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Program _instance = new Program();
            string input = "";
            int result = -1;
            int day = 3;
            int part = 2;
            bool test = false;

            input = "./Input/day" + day.ToString() + "_1";
            input += (test) ? "_test.txt" : ".txt";

            switch (day)
            {
                case 1:
                    result = _instance.Day1(input,part);
                    Console.WriteLine("Result : {0}", result);
                    
                    break;
                case 2:
                    result = _instance.Day2(input, part);
                    Console.WriteLine("Result : {0}", result);
                    break;
                case 3:
                    result = _instance.Day3(input, part);
                    Console.WriteLine("Result : {0}", result);
                    break;
                case 4:
                    result = _instance.Day4(input, part);
                    Console.WriteLine("Result : {0}", result);
                    break;

                default:
                    Console.WriteLine("Bad day ;)");
                    break;
            }
            Console.WriteLine("Key to exit");
            Console.ReadLine();
        }

        int Day1(string input, int part)
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

        int Day2(string input, int part)
        {
            var lines = File.ReadLines(input).ToList();
            List<RockPaperScissor> rounds = new();

            foreach (var line in lines)
            {
                RockPaperScissor round = new RockPaperScissor(line, part);
                rounds.Add(round);
            }

            var totalScore = rounds.Sum(x => x.Score);
            return totalScore;
        }

        int Day3(string input, int part)
        {
            var lines = File.ReadLines(input).ToList();

            if (part == 1)
            {
                List<RuckSack> sacks = new();

                foreach (var line in lines)
                {
                    RuckSack sack = new RuckSack(line, part);
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

        int Day4(string input, int part)
        {
            var lines = File.ReadLines(input).ToList();
           
            return 0;
        }
    }
}