using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoc22.Day06
{
    internal class SignalProcessor
    {
        public string Signal;
        public int Marker;
        public SignalProcessor(string signal, int part)
        {
            Marker = -1;
            Signal = signal;

            Process(part);
        }

        void Process(int part)
        {
            var foundPosition = -1;
            var markerLength = part == 1 ? 4 : 14;

            for (int i = 0; i < Signal.Length - markerLength - 1; i++)
            {
                var subStr = Signal.Substring(i, markerLength);
                if (!(subStr.GroupBy(x => x).Count() == markerLength))
                    continue;

                foundPosition = i + markerLength;
                break;
            }
            Marker = foundPosition;
        }


    }
}
