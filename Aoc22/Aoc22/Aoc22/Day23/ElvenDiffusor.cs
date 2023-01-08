using Aoc22.Day05;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoc22.Day23
{
    class Elf
    {
        public int x;
        public int y;

        public int proposed_x;
        public int proposed_y;
        public bool moved; 
    }

    internal class ElvenDiffusor
    {
        List<Elf> elves = new();
        Dictionary<(int, int), int> proposals = new();

        public void ParseInput(List<string> input, int part = 1)
        {
            for (int yy = 0; yy < input.Count; yy++)
            {
                for (int xx = 0; xx < input[yy].Length; xx++)
                    if (input[yy][xx] == '#')
                        elves.Add(new Elf() { x = xx, y = yy, moved = false });
            }
        }

        List<Elf> GetNeighbors(Elf elf) 
            => elves.Where(e => e.x >= elf.x - 1 && 
                                e.x <= elf.x + 1 && 
                                e.y >= elf.y - 1 && 
                                e.y <= elf.y + 1 &&
                                !(e.x== elf.x && e.y == elf.y)).ToList();


        (bool canMove, int x, int y) Propose(Elf elf, List<Elf> neighbors, int direction)
        {
            int xx = 0;
            int yy = 0;

            var interestingNeighbors = direction switch
            {
                0 => neighbors.Where(e => e.y == elf.y - 1).ToList(),       // N
                1 => neighbors.Where(e => e.y == elf.y + 1).ToList(),       // S
                2 => neighbors.Where(e => e.x == elf.x - 1).ToList(),       // W
                3 => neighbors.Where(e => e.x == elf.x + 1).ToList(),       // W
                _ => throw new Exception("Invalid direction")
            };

            bool bCanMove = interestingNeighbors.Count == 0;
            if(bCanMove) 
            {
                xx = (direction == 2) ? -1 : (direction == 3) ? 1 : xx;
                yy = (direction == 0) ? -1 : (direction == 1) ? 1 : yy;
            }

            return (bCanMove, xx, yy);
        }

        void FirstHalf(Elf elf, int firstDirection)
        {
            var neighbors = GetNeighbors(elf);
            elf.proposed_x = elf.x; // Does not move by default
            elf.proposed_y = elf.y;

            if (neighbors.Count == 0)   // No move
            {
                var k = (elf.proposed_x, elf.proposed_y);
                proposals[k] = proposals.Keys.Contains(k) ? proposals[k] + 1 : 1;
                return;
            }

            for (int attempt = 0; attempt < 4; attempt++)
            {
                var dir = (firstDirection + attempt) % 4;
                var proposal = Propose(elf, neighbors, dir);

                if (proposal.canMove)
                {
                    elf.proposed_x += proposal.x;
                    elf.proposed_y += proposal.y;
                    break;
                }
            }
            var key = (elf.proposed_x, elf.proposed_y);
            proposals[key] = proposals.Keys.Contains(key) ? proposals[key] + 1 : 1;
        }

        void SecondHalf(Elf elf)
        {
            elf.moved = false;
            var key = (elf.proposed_x, elf.proposed_y);
            
            if (proposals[key] == 1)    // Only one proposal
            {
                if (elf.x != elf.proposed_x || elf.y != elf.proposed_y) // It only moves if its position changes
                    elf.moved = true;
                elf.x = elf.proposed_x;
                elf.y = elf.proposed_y;
            }
            elf.proposed_x = -1;
            elf.proposed_y = -1;
        }

        int Diffuse(int rounds)
        {
            for (int round = 0; round < rounds; round++)
            {
                proposals.Clear();
                int firstDirection = round % 4;
                foreach (Elf elf in elves)
                    FirstHalf(elf, firstDirection);

                foreach (Elf elf in elves)
                    SecondHalf(elf);
            }

            var width = elves.Max(e => e.x) - elves.Min(e => e.x) +1;
            var height = elves.Max(e => e.y) - elves.Min(e => e.y) + 1;

            return (width * height) - elves.Count;
        }

        int Stabilize()
        {
            int round = 0;
            int numMoved = 999;

            while(numMoved!=0)
            {
                proposals.Clear();
                int firstDirection = round % 4;
                foreach (Elf elf in elves)
                    FirstHalf(elf, firstDirection); // increments noMove for part 2

                foreach (Elf elf in elves)
                    SecondHalf(elf);

                numMoved = elves.Where(e => e.moved).Count();

                round++;
            }

            return round;
        }

        public int Solve(int part = 1)
            => part == 1 ? Diffuse(10) : Stabilize();
    }
}
