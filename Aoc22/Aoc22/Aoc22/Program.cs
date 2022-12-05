using System.ComponentModel.DataAnnotations;

namespace Aoc22
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Program _instance = new Program();
            int day = 1;

            switch (day)
            {
                case 1:
                    //string input = "./Input/day1_1_test.txt";
                    string input = "./Input/day1_1.txt";
                    var result = _instance.Day1(input);
                    Console.WriteLine("Result : {0}", result);
                    
                    break;
                default:
                    Console.WriteLine("Bad day ;)");
                    break;
            }
            Console.WriteLine("Key to exit");
            Console.ReadLine();
        }

        int Day1(string input)
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

            var maxCal = elves.Max(x => x.GetCaloriesCount());
            return maxCal;

        }
    }
}