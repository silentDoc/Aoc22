using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoc22.Common
{
    static class MathUtils
    {
        static int GCD(int x, int y) 
        {
            int gcd = 0;
            int a = Math.Max(x, y);
            int b = Math.Min(x, y);
            do
            {
                gcd = b;
                b = a % b;
                a = gcd;
            } while(b!=0);
            return gcd;
        }

        static int LCM(int x, int y)
        {
            int a = Math.Max(x, y);
            int b = Math.Min(x, y);
            return (a / GCD(a, b)) * b;
        }

    }
}
