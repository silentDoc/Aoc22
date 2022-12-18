using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace Aoc22.Common
{
    static class PGMFileHelper
    {
        static public void Dump(string outputfile, List<char[]> image, Dictionary<char, int> equivalence, bool reverse = false)
        {
            using (StreamWriter output = new(outputfile, false))
            {
                output.WriteLine("P2");
                output.WriteLine("# Created by silentD0c");
                int h = image.Count;
                int w = image[0].Length;
                output.WriteLine(w.ToString() + " " + h.ToString());
                int maxLevel = equivalence.Values.Max();
                output.WriteLine(maxLevel);

                if (!reverse)
                {
                    foreach (var line in image)
                    {
                        var values = line.Select(l => equivalence[l]).ToList();
                        StringBuilder sb = new("");
                        values.ForEach(x => sb.Append(x.ToString() + " "));
                        output.WriteLine(sb.ToString().Trim());
                    }
                    return;
                }

                for (int i = image.Count - 1; i >= 0; i--)
                {
                    var line = image[i];
                    var values = line.Select(l => equivalence[l]).ToList();
                    StringBuilder sb = new("");
                    values.ForEach(x => sb.Append(x.ToString() + " "));
                    output.WriteLine(sb.ToString().Trim());
                }
            }

        }
    }
}
