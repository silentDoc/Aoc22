using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoc22
{
    internal class KnotPosition
    {
        public int x;
        public int y;

        public KnotPosition()
        {
            x = 0;
            y = 0;
        }

        public KnotPosition(int nx, int ny)
        {
            x = nx;
            y = ny;
        }

        public KnotPosition Move(string direction) =>
            direction switch
            {
                "R" => new KnotPosition(x + 1, y),
                "L" => new KnotPosition(x - 1, y),
                "D" => new KnotPosition(x, y + 1),
                "U" => new KnotPosition(x, y - 1),
                _ => throw new ArgumentException("Unknown direction"),
            };

    }

    internal class KnotPositionComparer : IEqualityComparer<KnotPosition>
    {
        public bool Equals(KnotPosition? a, KnotPosition? b) =>
            (a is not null && b is not null) ? (a.x == b.x) && (a.y == b.y) : false;

        public int GetHashCode(KnotPosition obj)
        {
            string S = obj.x.ToString() + "x" + obj.y.ToString();
            return S.GetHashCode();
        }
    }

    

    internal class RopeBridge
    {
        List<KnotPosition> visitedTailPositions = new();
        KnotPosition head;
        KnotPosition tail;

        public RopeBridge()
        {
            head = new KnotPosition();
            tail = new KnotPosition();
            visitedTailPositions.Add(tail);
        }

        public void DoMoves(List<string> input)
        {
            foreach (var entry in input)
            {
                var dir = entry.Split(" ").First();
                var num = int.Parse(entry.Split(" ").Last());

                for (int i = 0; i < num; i++)
                {
                    head = head.Move(dir);
                    var solve = SolvePlank(head, tail);
                    tail = new KnotPosition(tail.x + solve.Item1, tail.y + solve.Item2);
                    visitedTailPositions.Add(tail);
                }
            }
        }

        public Tuple<int, int> SolvePlank(KnotPosition head, KnotPosition tail)
        {
            int difX = head.x - tail.x;
            int difY = head.y - tail.y;
            int absX = Math.Abs(difX);
            int absY = Math.Abs(difY);

            if (Math.Max(absX, absY) <= 1)  // Take into account distance =1 and overlap of h over tail
                return new Tuple<int, int>(0, 0);   

            bool inAxisX = (head.x == tail.x); bool inAxisY = (head.y == tail.y);
            bool inAxis = inAxisX | inAxisY;

            return (!inAxis) ? new Tuple<int, int>(difX / absX, difY / absY)
                             :  (inAxisX) ? new Tuple<int, int>(0, difY/absY)
                                          : new Tuple<int, int>(difX / absX, 0);
        }

        public int VisitedPositions() =>
            visitedTailPositions.Distinct(new KnotPositionComparer()).Count();


    }
}
