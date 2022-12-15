using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Sources;


namespace Aoc22.Day11
{
    class Monkey
    {
        public List<long> items = new();
        char op = '\0';
        long opFactor = 0;
        bool opFactorIsCurrent = false;
        public long TestFactor = 0;
        int destTrue = 0;
        int destFalse = 0;
        public int NumInspections = 0;

        public Monkey(char op, long opFactor, long testFactor,
                      int destTrue, int destFalse, List<long> startingItems,
                      bool opFactorIsCurrent = false)
        {
            this.op = op;
            this.opFactor = opFactor;
            TestFactor = testFactor;
            this.destTrue = destTrue;
            this.destFalse = destFalse;
            this.opFactorIsCurrent = opFactorIsCurrent;
            startingItems.ForEach(x => items.Add(x));
            NumInspections = 0;
        }

        public void ReceiveItem(long item)
            => items.Add(item);

        public void Turn(List<Monkey> gang, long mcm, int part = 1)
        {
            Inspect();
            TestAndSend(gang, mcm, part);
        }

        void Inspect()
        {
            items = items.Select(x => Operate(x)).ToList();
            NumInspections += items.Count;
        }

        void TestAndSend(List<Monkey> gang, long mcm, int part = 1)
        {
            if (part == 1)
                items = items.Select(x => x / 3).ToList();

            foreach (var i in items)
            {
                var item = part == 2 ? i % mcm : i;
                int dest = item % TestFactor == 0 ? destTrue : destFalse;
                gang[dest].ReceiveItem(item);
            }
            items.Clear();
        }

        long Operate(long x) =>
            (op, opFactorIsCurrent) switch
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

        public int SetupGang(List<string> input)
        {
            int numMonkeys = (input.Count() + 1) / 7;   // Each monkey definition takes 6 lines and a newline
            for (int i = 0; i < numMonkeys; i++)
            {
                int startIndex = i * 7;
                var itemsLine = input[startIndex + 1].Replace("Starting items: ", "").Replace(" ", "").Split(",", StringSplitOptions.RemoveEmptyEntries);
                var opLine = input[startIndex + 2].Replace("Operation: new = old ", "").Split(" ", StringSplitOptions.RemoveEmptyEntries);
                var tstLine = input[startIndex + 3].Replace("Test: divisible by ", "").Trim();
                var dstTrue = input[startIndex + 4].Replace("If true: throw to monkey ", "").Trim();
                var dstFalse = input[startIndex + 5].Replace("If false: throw to monkey ", "").Trim();

                List<long> startItems = itemsLine.Select(x => long.Parse(x)).ToList();
                char op = opLine[0][0];
                bool opFactorIsCurrent = opLine[1] == "old";
                long opFactor = opFactorIsCurrent ? 0 : long.Parse(opLine[1]);
                long tstFactor = long.Parse(tstLine);
                int destTrue = int.Parse(dstTrue);
                int destFalse = int.Parse(dstFalse);

                gang.Add(new Monkey(op, opFactor, tstFactor, destTrue, destFalse, startItems, opFactorIsCurrent));
            }
            return gang.Count();
        }

        public long MonkeyBusiness(int rounds, int part = 1)
        {
            // Part 2 - It's tricky, and of course it does not depend on the data types. 
            // We just have to make sure that the items we get KEEP the same divisibilty against all possible tests. 
            // To do so, we will find the minimum common multiple of all the test factors. 

            //long mcm = (long) gang.Select(x=>x.TestFactor).Aggregate(1, (acc, x) => acc * x);
            long mcm = 1;
            foreach (var monkey in gang)
                mcm *= monkey.TestFactor;

            for (int i = 0; i < rounds; i++)
                gang.ForEach(x => x.Turn(gang, mcm, part));


            var listInspections = gang.Select(x => x.NumInspections).OrderByDescending(x => x).ToList();
            return listInspections[0] * (long)listInspections[1];
        }
    }
}
