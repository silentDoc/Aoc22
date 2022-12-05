using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoc22
{
    internal class Elf
    {
        List<int> calories;
       

        public Elf() 
        {
            calories = new List<int>();
        }

        public void AddCalories(int calory)
        {
            calories.Add(calory);
        }
        
        public void AddCalories(List<int> caloryList)
        {
            caloryList.ForEach(x => AddCalories(x));
        }

        public int GetCaloriesCount() 
        {
            return calories.Sum();
        }

    }
}
