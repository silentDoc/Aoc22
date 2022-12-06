using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoc22
{
    internal class SignalProcessor
    {
        public string Signal;
        public int Marker;
        public SignalProcessor(string signal, int part)
        {
            Marker = -1;
            Signal = signal;

            Process();
        }

        void Process()
        {
            var foundPosition = -1;
            for (int i = 0; i < Signal.Length - 4 - 1; i++)
            {
                var subStr = Signal.Substring(i, 4);
                if(! (subStr.GroupBy(x=>x).Count() == 4))
                    continue;

                foundPosition = i+4;
                break;
            }
            Marker = foundPosition;
        }


    }
}
