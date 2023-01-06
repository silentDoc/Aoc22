using Aoc22.Day16;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Aoc22.Day21
{
    class Monkey
    {
        public string name;
        public bool hasValue;
        public long value;
        public string firstOp;
        public string secondOp;
        public string operand;
        public bool dependsOnHuman; 

        public Monkey(string name, bool hasValue, long value, string firstOp, string secondOp, string operand)
        {
            this.name = name;
            this.hasValue = hasValue;
            this.value = value;
            this.firstOp = firstOp;
            this.secondOp = secondOp;
            this.operand = operand;
            dependsOnHuman = false;
        }   
    }

    internal class MonkeyMath
    {
        List<Monkey> monkeys = new();

        public void ParseInput(List<string> input, int part = 1)
        {
            string pattern1 = @"([a-z]{4}): ([a-z]{4}) (\+|-|\*|\/) ([a-z]{4})";
            string pattern2 = @"([a-z]{4}): (\d+)";
            
            Regex regexOp = new Regex(pattern1);
            Regex regexNum = new Regex(pattern2);

            foreach (var line in input)
            {
                if (regexOp.IsMatch(line))
                {
                    var groups = regexOp.Match(line).Groups;
                    monkeys.Add(new Monkey(groups[1].Value, false, -1, groups[2].Value, groups[4].Value, groups[3].Value));
                }
                else
                {
                    var groups = regexNum.Match(line).Groups;
                    monkeys.Add(new Monkey(groups[1].Value, true, long.Parse(groups[2].Value), "","",""));
                }
            }
        }

        void DoRound()
        {
            foreach (var monkey in monkeys)
            {
                if (monkey.hasValue)
                    continue;

                var monkeyOp1 = monkeys.Where(x => x.name == monkey.firstOp).First();
                if (!monkeyOp1.hasValue)
                    continue;
                var monkeyOp2 = monkeys.Where(x => x.name == monkey.secondOp).First();
                if (!monkeyOp2.hasValue)
                    continue;
   
                monkey.dependsOnHuman = monkeyOp1.dependsOnHuman || monkeyOp2.dependsOnHuman;

                monkey.value = monkey.operand switch
                {
                    "+" => monkeyOp1.value + monkeyOp2.value,
                    "-" => monkeyOp1.value - monkeyOp2.value,
                    "*" => monkeyOp1.value * monkeyOp2.value,
                    "/" => monkeyOp1.value / monkeyOp2.value,
                    _ => throw new Exception("Unrecognized operand " + monkey.operand),
                };

                monkey.hasValue = true;
            }
        }

        (Monkey node, long newValue) TraverseDown(Monkey monkey, long currentValue)
        {
            var op1 = monkeys.Where(x => x.name == monkey.firstOp).First();
            var op2 = monkeys.Where(x => x.name == monkey.secondOp).First();
            
            var solidValue = (op1.dependsOnHuman) ? op2.value : op1.value;
            var nextNode = (op1.dependsOnHuman) ? op1 : op2;

            var newVal = monkey.operand switch
            {
                "+" => currentValue - solidValue,
                "-" => (nextNode == op1) ? currentValue + solidValue : solidValue - currentValue,
                "*" => currentValue / solidValue,
                "/" => (nextNode == op1) ? currentValue * solidValue : currentValue / solidValue,
                _ => throw new Exception("Unrecognized operand " + monkey.operand),
            };

            return (nextNode, newVal);
        }

        private long FindMatch()
        {
            var rootElement = monkeys.Where(x => x.name == "root").First();
            var humanElement = monkeys.Where(x => x.name == "humn").First();
            humanElement.dependsOnHuman = true;
            while (!rootElement.hasValue)
                DoRound();

            // Now traverse from top down
            var op1 = monkeys.Where(x => x.name == rootElement.firstOp).First();
            var op2 = monkeys.Where(x => x.name == rootElement.secondOp).First();

            var valueShouldBe = (op1.dependsOnHuman) ? op2.value : op1.value;
            var currentNode = (op1.dependsOnHuman) ? op1 : op2;

            while (currentNode != humanElement)
                (currentNode, valueShouldBe) = TraverseDown(currentNode, valueShouldBe);

            return valueShouldBe;
        }

        private long Yell()
        {
            var rootElement = monkeys.Where(x => x.name == "root").First();
            while (!rootElement.hasValue)
                DoRound();
            return rootElement.value;
        }

        public long Solve(int part = 1)
            => (part == 1) ? Yell() : FindMatch();
    }
}
