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
}
