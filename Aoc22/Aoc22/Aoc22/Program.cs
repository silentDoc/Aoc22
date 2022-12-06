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
            int day = 2;
            int part = 2;

            switch (day)
            {
                case 1:
                    //input = "./Input/day1_1_test.txt";
                    input = "./Input/day1_1.txt";
                    result = _instance.Day1(input,part);
                    Console.WriteLine("Result : {0}", result);
                    
                    break;
                case 2:
                    //input = "./Input/day2_1_test.txt";
                    input = "./Input/day2_1.txt";
                    result = _instance.Day2(input, part);
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
    }
}