using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Aoc22.Day05
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

        public void Move(string statement, int part = 1)
        {
            // move 1 from 3 to 9
            string patternText = @"\d+";
            Regex r = new(patternText);
            var result = r.Matches(statement);

            int howMany = int.Parse(result[0].Value);
            int from = int.Parse(result[1].Value) - 1;
            int to = int.Parse(result[2].Value) - 1;

            if (part == 1)
                for (int i = 0; i < howMany; i++)
                    stacks[to].Push(stacks[from].Pop());
            else
            {
                char[] elements = new char[howMany];

                for (int i = 0; i < howMany; i++)
                    elements[i] = stacks[from].Pop();

                for (int i = howMany - 1; i >= 0; i--)
                    stacks[to].Push(elements[i]);
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

    public class CrateStackSolver
    {
        public string Solve(List<string> lines, int part)
        {
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
    }
}
