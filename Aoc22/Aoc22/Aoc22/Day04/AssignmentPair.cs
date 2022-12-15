using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoc22.Day04
{
    internal class AssignmentPair
    {
        int pair1_start;
        int pair1_stop;
        int pair2_start;
        int pair2_stop;

        public bool FullyContained;
        public bool Overlap;

        public AssignmentPair(string line, int part)
        {
            FullyContained = false;
            Overlap = false;

            var assingments = line.Split(",");
            var assignment1 = assingments[0].Split("-");
            var assignment2 = assingments[1].Split("-");

            pair1_start = int.Parse(assignment1[0]);
            pair1_stop = int.Parse(assignment1[1]);

            pair2_start = int.Parse(assignment2[0]);
            pair2_stop = int.Parse(assignment2[1]);

            FullyContained = pair1_start <= pair2_start && pair1_stop >= pair2_stop
                             || pair2_start <= pair1_start && pair2_stop >= pair1_stop;

            Overlap = FullyContained
                      || pair1_start >= pair2_start && pair1_start <= pair2_stop
                      || pair1_stop >= pair2_start && pair1_stop <= pair2_stop;


        }

    }
}
