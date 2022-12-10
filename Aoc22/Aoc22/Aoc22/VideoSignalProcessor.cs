using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoc22
{
    struct instruction
    {
        public string op;
        public int val;
        public int cycles
        {
            get 
            {
                return (op == "noop") ? 1 : 2;
            }
        }
    }

    internal class VideoSignalProcessor
    {
        List<instruction> instructions = new();

        public  VideoSignalProcessor(List<string> instructions) 
        { 
            parseInstructions(instructions);
        }

        void parseInstructions(List<string> instructions)
        { 
            foreach(var ins in instructions)
            {
                var parts = ins.Split(" ");
                var name = parts[0]; 
                var value = (parts.Length > 1) ? int.Parse(parts[1]) : 0;
                this.instructions.Add(new instruction() { op = name, val = value }); 
            }
        }

        public int RunInstructions(int[] evals)
        {
            int xRegister = 1;
            int signalStrength;
            List<int> signalStrengths = new();
            List<int> xValues = new();
            int tick = 0;

            foreach (var ins in instructions)
            {
                for (int i = 0; i < ins.cycles; i++)
                {
                    signalStrength = ++tick * xRegister;
                    signalStrengths.Add(signalStrength);
                    xValues.Add(xRegister);
                }
                // Execute the op
                if (ins.op == "addx")
                    xRegister += ins.val;
            }

            int suma = 0;
            foreach (var p in evals)
                suma += signalStrengths[p - 1];

            return suma;
        }
    }
}
