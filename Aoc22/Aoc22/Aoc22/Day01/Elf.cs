using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoc22.Day01
{
    internal class Elf
    {
        List<int> calories;

        public Elf()
        {
            calories = new List<int>();
        }

        public void AddCalories(int calory)
            => calories.Add(calory);

        public void AddCalories(List<int> caloryList)
            => caloryList.ForEach(x => AddCalories(x));
        

        public int GetCaloriesCount()
            => calories.Sum();
    }

    public class ElvenGroup
    {
        List<Elf> elves = new();

        public void ParseInput(List<string> lines)
        {
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
        }

        public int Solve(int part)
        {
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

    }
}
