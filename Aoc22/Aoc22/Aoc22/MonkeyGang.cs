using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Sources;

namespace Aoc22
{
    class Monkey
    {
        public List<int> items = new();
        char op = '\0';
        int opFactor = 0;
        bool opFactorIsCurrent = false;
        int testFactor = 0;
        int destTrue = 0;
        int destFalse = 0;
        public int NumInspections = 0;

        public Monkey(char op, int opFactor, int testFactor, 
                      int destTrue, int destFalse, List<int> startingItems, 
                      bool opFactorIsCurrent = false)
        {
            this.op = op;
            this.opFactor = opFactor;
            this.testFactor = testFactor;
            this.destTrue = destTrue;
            this.destFalse = destFalse;
            this.opFactorIsCurrent = opFactorIsCurrent;
            startingItems.ForEach(x => items.Add(x));
            NumInspections = 0;
        }

        public void ReceiveItem(int item)
            => items.Add(item);

        public void Turn(List<Monkey> gang)
        {
            Inspect();
            TestAndSend(gang);
        }

        void Inspect()
        {
            items = items.Select(x => Operate(x)).ToList();
            NumInspections += items.Count();
        }

        void TestAndSend(List<Monkey> gang)
        {
            //items = items.Select(x => x / 3).ToList();
            foreach (var i in items)
            {
                int dest = (i % testFactor == 0) ? destTrue : destFalse;
                gang[dest].ReceiveItem(i);
            }
            items.Clear();
        }

        int Operate(int x) =>
            (op , opFactorIsCurrent) switch
            {
                ('+', false) => x + opFactor,
                ('*', false) => x * opFactor,
                ('+', true) => x + x,
                ('*', true) => x * x,
                _ => throw new NotImplementedException("Unknown Operand"),
            };
    }

    internal class MonkeyGang
    {
        List<Monkey> gang = new();

        public int SetupGang(List<string> input, int part =1)
        {
            int numMonkeys = (input.Count() + 1) / 7;   // Each monkey definition takes 6 lines and a newline
            for (int i = 0; i < numMonkeys; i++)
            {
                int startIndex = i * 7;
                var itemsLine = input[startIndex + 1].Replace("Starting items: ", "").Replace(" ", "").Split(",", StringSplitOptions.RemoveEmptyEntries);
                var opLine = input[startIndex + 2].Replace("Operation: new = old ","").Split(" ", StringSplitOptions.RemoveEmptyEntries);
                var tstLine = input[startIndex + 3].Replace("Test: divisible by ", "").Trim();
                var dstTrue = input[startIndex + 4].Replace("If true: throw to monkey ", "").Trim();
                var dstFalse = input[startIndex + 5].Replace("If false: throw to monkey ", "").Trim();

                List<int> startItems = itemsLine.Select(x => int.Parse(x)).ToList();
                char op = opLine[0][0];
                bool opFactorIsCurrent = (opLine[1] == "old");
                int opFactor = (opFactorIsCurrent) ? 0: int.Parse(opLine[1]);
                int tstFactor = int.Parse(tstLine);
                int destTrue = int.Parse(dstTrue);
                int destFalse = int.Parse(dstFalse);

                gang.Add(new Monkey(op, opFactor, tstFactor, destTrue, destFalse, startItems, opFactorIsCurrent));
            }
            return gang.Count();
        }

        private void logWorryLevels()
        {
            for (int i = 0; i < gang.Count(); i++)
            {
                var lst = gang[i].items;
                StringBuilder sb = new("");
                foreach (var x in lst)
                    sb.Append(x.ToString() + ",");

                Console.WriteLine("Monkey {0} : {1}", i, sb.ToString());
            }
        }

        public int MonkeyBusiness(int rounds)
        {
            for (int i = 0; i < rounds; i++)
            {
                gang.ForEach(x => x.Turn(gang));

                //Console.WriteLine("End of round {0}", i+1);
                //logWorryLevels();
                //Console.WriteLine("");
            }

            var listInspections = gang.Select(x => x.NumInspections).ToList();
            var max1 = listInspections.Max();
            listInspections.Remove(max1);
            var max2 = listInspections.Max();

            return max1 * max2;
        }
    }
}
