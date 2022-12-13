using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Aoc22
{
    public abstract class Item
    {
        public abstract override string ToString();
    }

    public class IntItem : Item
    {
        public int Val;
        public override string ToString() { return Val.ToString(); }
    }

    public class ListItem : Item
    {
        public List<Item> Val;
        public override string ToString() { return $"[{string.Join(',', Val.Select(v => v.ToString()))}]"; }
    }

    public class SignalOrderChecker
    {

        public int Part1(string[] input)
        {
            var sum = 0;
            var pairs = 1;
            var inputIndex = 0;
            while (inputIndex < input.Length)
            {
                var lhs = Parse(input[inputIndex++]);
                var rhs = Parse(input[inputIndex++]);

                Trace.WriteLine(lhs.ToString() + " vs " + rhs.ToString());

                if (Compare(lhs, rhs) == -1)
                {
                    sum += pairs;
                }

                inputIndex++; // Empty line bw pairs
                pairs++;
            }

            return sum;
        }

        public int Part2(string[] input)
        {
            var items = new List<Item>();
            var inputIndex = 0;

            // Parse input
            while (inputIndex < input.Length)
            {
                var inputLine = input[inputIndex++];
                if (inputLine.Length == 0)
                    continue;

                items.Add(Parse(inputLine));
            }

            // Dividers
            IntItem i = new IntItem() { Val = 2 };
            ListItem li = new ListItem() { Val = new List<Item>() { i } };
            ListItem a = new ListItem() { Val = new List<Item>() { li } };

            IntItem ii = new IntItem() { Val = 6};
            ListItem lii = new ListItem() { Val = new List<Item>() { ii } };
            ListItem b = new ListItem() { Val = new List<Item>() { lii } }; 

            items.Add(a);
            items.Add(b);

            items.Sort(Compare);    // I love linq

            return (items.IndexOf(a) + 1) * (items.IndexOf(b) + 1);
        }

        Item Parse(string line)
        {
            List<Item> items = new();

            bool inArray = false;
            int start = 0;
            int end;
            int precedence = 0;

            if (line == "[]")
                return new ListItem() { Val = items };

            for (int i = 1; i < line.Length - 1; i++)
            {
                if (inArray)
                {
                    if (line[i] == '[')
                        precedence++;
                    if (line[i] == ']')
                        precedence--;
                    if (precedence == 0)
                    {
                        inArray = false;
                        end = i;
                        string child = line.Substring(start, end - start + 1);
                        items.Add(Parse(child));
                    }
                }
                else
                {
                    if (line[i] == '[')
                    {
                        inArray = true;
                        start = i;
                        precedence++;
                    }
                    else if (line[i] != ',')
                    {
                        int startNumber = i;
                        while (char.IsDigit(line[i]))
                            i++;
                        var strNumber = line.Substring(startNumber, i - startNumber);
                        var item = new IntItem() { Val = int.Parse(strNumber) };
                        items.Add(item);
                    }
                }
            }
            return new ListItem() { Val = items };
        }

        // 0 - Keep going, -1 - Right , 1 - Wrong
        int Compare(Item lhs, Item rhs)
        {
            if (lhs == rhs)
                return 0; 

            if (lhs is IntItem lIntItem && rhs is IntItem rIntItem)
            {
                if (lIntItem.Val == rIntItem.Val)
                    return 0;

                if (lIntItem.Val < rIntItem.Val)
                    return -1; 

                return 1; 
            }

            if (lhs is not ListItem lListItem) 
                lListItem = new ListItem()   {  Val = new List<Item> { lhs }  };

            if (rhs is not ListItem rListItem)
                rListItem = new ListItem()   {  Val = new List<Item> { rhs }  };
            

            var index = 0;
            while (index < lListItem.Val.Count)
            {
                if (index >= rListItem.Val.Count)
                    return 1;                       

                var comparison = Compare(lListItem.Val[index], rListItem.Val[index]);
                if (comparison != 0)
                    return comparison;      

                index++;
                continue;
            }

            if (lListItem.Val.Count() == rListItem.Val.Count())
                return 0;       

            return -1;
        }

        
    }
}
