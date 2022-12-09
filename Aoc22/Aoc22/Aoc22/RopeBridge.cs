using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Aoc22
{
    internal class KnotPosition
    {
        public int x;
        public int y;

        public KnotPosition() => x = y = 0;

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

        public int GetHashCode(KnotPosition obj) =>
            (obj.x.ToString() + "x" + obj.y.ToString()).GetHashCode();
    }

    internal class RopeBridge
    {
        List<KnotPosition> visitedTailPositions = new();
        KnotPosition head;
        KnotPosition tail;

        KnotPosition[] stringP2;

        public RopeBridge()
        {
            head = new KnotPosition();
            tail = new KnotPosition();
            visitedTailPositions.Add(tail);

            stringP2 = new KnotPosition[10];    // 0 - head , 9 - tail
            for (var i = 0; i < 10; i++)
                stringP2[i] = new();

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

        public void DoMovesP2(List<string> input)
        {
            foreach (var entry in input)
            {
                var dir = entry.Split(" ").First();
                var num = int.Parse(entry.Split(" ").Last());

                for (int i = 0; i < num; i++)
                {
                    stringP2[0] = stringP2[0].Move(dir);
                    var solve = SolvePlank(stringP2[0], stringP2[1]);
                    stringP2[1] = new KnotPosition(stringP2[1].x + solve.Item1, stringP2[1].y + solve.Item2);
                    for (int j = 2; j < 10; j++)
                    {
                        var solve2 = SolvePlank(stringP2[j-1], stringP2[j]);
                        stringP2[j] = new KnotPosition(stringP2[j].x + solve2.Item1, stringP2[j].y + solve2.Item2);
                    }
                    visitedTailPositions.Add(stringP2[9]);
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
