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
        public List<ulong> items = new();
        char op = '\0';
        ulong opFactor = 0;
        bool opFactorIsCurrent = false;
        ulong testFactor = 0;
        int destTrue = 0;
        int destFalse = 0;
        public ulong NumInspections = 0;

        public Monkey(char op, ulong opFactor, ulong testFactor, 
                      int destTrue, int destFalse, List<ulong> startingItems, 
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

        public void ReceiveItem(ulong item)
            => items.Add(item);

        public void Turn(List<Monkey> gang, int part=1)
        {
            Inspect();
            TestAndSend(gang, part);
        }

        void Inspect()
        {
            items = items.Select(x => Operate(x)).ToList();
            NumInspections += (ulong) items.Count();
        }

        void TestAndSend(List<Monkey> gang, int part=1)
        {
            if(part==1)
                items = items.Select(x => x / 3).ToList();

            foreach (var i in items)
            {
                int dest = (i % testFactor == 0) ? destTrue : destFalse;
                gang[dest].ReceiveItem(i);
            }
            items.Clear();
        }

        ulong Operate(ulong x) =>
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

        public int SetupGang(List<string> input)
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

                List<ulong> startItems = itemsLine.Select(x => ulong.Parse(x)).ToList();
                char op = opLine[0][0];
                bool opFactorIsCurrent = (opLine[1] == "old");
                ulong opFactor = (opFactorIsCurrent) ? 0: ulong.Parse(opLine[1]);
                ulong tstFactor = ulong.Parse(tstLine);
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

        private void logInspections()
        {
            for (int i = 0; i < gang.Count(); i++)
                Console.WriteLine("Monkey {0} has inspected items {1} times", i, gang[i].NumInspections);
        }

        public ulong MonkeyBusiness(int rounds, int part=1)
        {
            int[] eval = new int[] { 1, 20, 1000, 2000, 3000, 4000, 5000, 6000, 7000, 8000, 9000, 1000 };


            for (int i = 0; i < rounds; i++)
            {
                gang.ForEach(x => x.Turn(gang, part));

                if( (part==2) && (eval.Contains(i + 1)))
                {
                    Console.WriteLine("End of round {0}", i+1);
                    logInspections();
                    Console.WriteLine("");
                }

                //Console.WriteLine("End of round {0}", i+1);
                //logWorryLevels();
                // Console.WriteLine("");
            }

            var listInspections = gang.Select(x => x.NumInspections).ToList();
            var max1 = listInspections.Max();
            listInspections.Remove(max1);
            var max2 = listInspections.Max();

            return max1 * max2;
        }
    }
}
