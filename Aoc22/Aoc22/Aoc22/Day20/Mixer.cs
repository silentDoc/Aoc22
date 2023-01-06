using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Aoc22.Day20
{
    internal class Mixer
    {
        public int CheckRepeats()   // I was about to go nuts until I saw the list had repeated elements !!
            => sourceList.Select(x => x.value).Distinct().Count() - sourceList.Count();

        List<(long value, int index)> sourceList = new();    // Doing it like this ensures item uniqueness

        public void ParseInput(List<string> lines)
        {
            int index = 0;
            foreach (var line in lines)
                sourceList.Add( (int.Parse(line), index++) );
        }

        public long Solve(int part = 1)
            => (part==1) ? Mix(1,1,part) : Mix(10, 811589153, part);

        long Mix(int times, long key, int part = 1)
        {
            for (int i = 0; i < sourceList.Count; i++)
                sourceList[i] = (sourceList[i].value * key, sourceList[i].index);

            List<(long value, int index)> mixedList = new();
            mixedList.AddRange(sourceList);
            var count = (long) sourceList.Count;

            for (int mix = 0; mix < times; mix++)
                foreach (var element in sourceList)
                {
                    var oldIndex = (long)mixedList.IndexOf(element);
                    var newIndex = (oldIndex + element.value) % (count - 1);
                    if (newIndex < 0)
                        newIndex = count + newIndex - 1;
                    mixedList.Remove(element);
                    mixedList.Insert((int)newIndex, element);
                }

            int indexOfZero = mixedList.FindIndex(x => x.value==0);
            int index1000th = (indexOfZero + 1000) % mixedList.Count;
            int index2000th = (indexOfZero + 2000) % mixedList.Count;
            int index3000th = (indexOfZero + 3000) % mixedList.Count;

            return mixedList[index1000th].value + mixedList[index2000th].value + mixedList[index3000th].value;
        }
    }
}
