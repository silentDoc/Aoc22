using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoc22
{
    internal class RuckSack
    {
        string sack_contents;
        string compartment1;
        string compartment2;
        char sharedItem;
        public int Priority;

        public RuckSack(string contents, int part)
        {
            sack_contents = contents;
            var length = contents.Length;
            compartment1 = contents.Substring(0, length / 2);
            compartment2 = contents.Substring(length / 2, length / 2);

            

            var duplicateItems = contents
                                    .GroupBy(x => x)
                                    .Where(g => g.Count() > 1)
                                    .SelectMany(x => x)
                                    .ToList();
            
            if (!duplicateItems.Any())
                throw new ArgumentException("No duplicate items in element " + contents);

            sharedItem = duplicateItems
                                    .Where(x => compartment1.Contains(x.ToString())
                                                && compartment2.Contains(x.ToString()))
                                    .ToList()
                                    .FirstOrDefault();

            Priority = (char.IsUpper(sharedItem))
                            ? (int)(sharedItem - 'A' + 1 + 26)
                            : (int)(sharedItem - 'a' + 1);

        }

    }

    internal class RuckSackGroup
    {
        List<string> ruckSacks;
        char sharedItem;

        public RuckSackGroup()
        {
            ruckSacks = new();
        }

        public int AddSack(string contents)
        {
            ruckSacks.Add(contents);
            return ruckSacks.Count;
        }

        public int GetPriority()
        {
            string allItems = String.Empty;
            
            string item1 = ruckSacks[0];
            string item2 = ruckSacks[1];
            string item3 = ruckSacks[2];

            allItems = item1 + item2 + item3;

            var duplicateItems = allItems
                                    .GroupBy(x => x)
                                    .Where(g => g.Count() > 2)
                                    .SelectMany(x => x)
                                    .ToList();
            
            sharedItem = duplicateItems
                                    .Where(x => item1.Contains(x.ToString())
                                                && item2.Contains(x.ToString())
                                                && item3.Contains(x.ToString()) )
                                    .ToList()
                                    .FirstOrDefault();
            
            var priority = (char.IsUpper(sharedItem))
                            ? (int)(sharedItem - 'A' + 1 + 26)
                            : (int)(sharedItem - 'a' + 1);

            return priority;
        }


    }
}
