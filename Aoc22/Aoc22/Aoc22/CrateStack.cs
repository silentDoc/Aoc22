using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Aoc22
{
    internal class CrateStacks
    {
        List<Stack<char>> stacks;

        public CrateStacks(int numberStacks, int part)
        {
            stacks = new();
            for (int i = 0; i < numberStacks; i++)
                stacks.Add(new Stack<char>());
        }

        public void AddCrateRow(string crateRow)
        {
            // Using split may be messy because of empty elements, we go old school
            for (int i = 0; i < stacks.Count; i++)
            {
                var element = crateRow.Substring(i * 4, 3);
                if (element.IndexOf("[") == -1)
                    continue;
                stacks[i].Push(element[1]);
            }
        }

        public void Move(string statement)
        {
            // move 1 from 3 to 9
            string patternText = @"\d+";
            Regex r = new(patternText);
            var result = r.Matches(statement);

            int howMany = int.Parse(result[0].Value);
            int from = int.Parse(result[1].Value) -1;
            int to = int.Parse(result[2].Value) -1;

            for (int i = 0; i < howMany; i++)
            {
                var element = stacks[from].Pop();
                stacks[to].Push(element);
            }
        }

        public string Status()
        {
            string result = "";

            for (int i = 0; i < stacks.Count; i++)
            {
                if (stacks[i].Count > 0)
                    result += stacks[i].Peek();
                else
                    result += " ";
            }
            return result;
        }







    }
}
