using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aoc22.Day25
{
    internal class FuelManager
    {
        List<string> snafu_nums = new();

        public void ParseInput(List<string> input)
            => input.ForEach(x => snafu_nums.Add(x));

        long snafu_to_dec(string snafu_num)
        { 
            var len = snafu_num.Length;
            long value = 0;
            long pow = 0;
            for(int p = len-1; p >= 0; p--) 
            { 
                char c = snafu_num[p];
                long digit = c switch
                {
                    '2' => 2,
                    '1' => 1,
                    '0' => 0,
                    '-' => -1,
                    '=' => -2,
                    _ => throw new Exception("Unknown snafu digit")
                };

                value += digit * (long)Math.Pow(5, pow);
                pow++;
            }
            return value;
        }

        string dec_to_snafu(long num)
        {
            char[] digits = { '0' , '1', '2' , '=', '-'};
            StringBuilder retVal = new();

            if (num == 0)
                return "";

            var remainder = num % 5;
            char digit = digits[remainder];
            var newNum = (num + 2) / 5;
            retVal.Append(dec_to_snafu(newNum));
            retVal.Append(digit.ToString());
            return retVal.ToString();
        }

        string CalcSum()
        {
            long sum = snafu_nums.Select(x => snafu_to_dec(x)).Sum();
            return dec_to_snafu(sum);
        }

        public string Solve(int part = 1)
            => CalcSum();
    }
}
