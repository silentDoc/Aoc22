namespace Aoc22
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Program _instance = new Program();
            _instance.Day1();
            
        }

        void Day1()
        {
            var lines = File.ReadLines("./Input/day1_1_test.txt");
            lines.ToList<string>().ForEach(x => Console.WriteLine(x));
            Console.ReadLine();

        }
    }
}