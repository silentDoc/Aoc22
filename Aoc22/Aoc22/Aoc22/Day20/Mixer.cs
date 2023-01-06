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

        List<(int value, int index)> sourceList = new();    // Doing it like this ensures item uniqueness

        public void ParseInput(List<string> lines)
        {
            int index = 0;
            foreach (var line in lines)
                sourceList.Add( (int.Parse(line), index++) );
        }

        public int Solve(int part = 1)
            => Mix(part);

        int Mix(int part = 1)
        {
            List<(int value, int index)> mixedList = new();
            mixedList.AddRange(sourceList);
            var count = sourceList.Count;

            foreach (var element in sourceList)
            {
                var oldIndex = mixedList.IndexOf(element);
                var newIndex = (oldIndex + element.value) % (count - 1);
                if (newIndex < 0)
                    newIndex = count + newIndex - 1;
                mixedList.Remove(element);
                mixedList.Insert(newIndex, element);
            }

            int indexOfZero = mixedList.FindIndex(x => x.value==0);
            int index1000th = (indexOfZero + 1000) % mixedList.Count;
            int index2000th = (indexOfZero + 2000) % mixedList.Count;
            int index3000th = (indexOfZero + 3000) % mixedList.Count;

            return mixedList[index1000th].value + mixedList[index2000th].value + mixedList[index3000th].value;
        }
    }
}
